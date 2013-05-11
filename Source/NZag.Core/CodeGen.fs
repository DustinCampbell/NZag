namespace NZag.Core

open NZag.Utilities

type ZFunc = delegate of memory:Memory
                       * locals:uint16[]
                       * stack:uint16[]
                       * sp:int
                       * argCount:int
                      -> uint16

type IMachine =

    abstract member GetInitialLocalArray : Routine -> uint16[]
    abstract member ReleaseLocalArray : uint16[] -> unit

    abstract member GetOrCompileZFunc : Routine -> ZFunc

and ZFuncInvoker(machine: IMachine, routine: Routine, zfunc: ZFunc) =

    let invoke memory locals stack sp argCount =
        try
            zfunc.Invoke(memory, locals, stack, sp, argCount)
        finally
            machine.ReleaseLocalArray(locals)

    member x.Invoke0(memory, stack, sp) =
        let locals = machine.GetInitialLocalArray(routine)
        invoke memory locals stack sp 0

    member x.Invoke1(arg1, memory, stack, sp) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        invoke memory locals stack sp 1

    member x.Invoke2(arg1, arg2, memory, stack, sp) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        invoke memory locals stack sp 2

    member x.Invoke3(arg1, arg2, arg3, memory, stack, sp) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        invoke memory locals stack sp 3

    member x.Invoke4(arg1, arg2, arg3, arg4, memory, stack, sp) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        invoke memory locals stack sp 4

    member x.Invoke5(arg1, arg2, arg3, arg4, arg5, memory, stack, sp) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        locals.[4] <- arg5
        invoke memory locals stack sp 5

    member x.Invoke6(arg1, arg2, arg3, arg4, arg5, arg6, memory, stack, sp) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        locals.[4] <- arg5
        locals.[5] <- arg6
        invoke memory locals stack sp 6

    member x.Invoke7(arg1, arg2, arg3, arg4, arg5, arg6, arg7, memory, stack, sp) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        locals.[4] <- arg5
        locals.[5] <- arg6
        locals.[6] <- arg7
        invoke memory locals stack sp 7

type CodeGenerator private (tree: BoundTree, builder: ILBuilder) =

    let labels = Array.init tree.LabelCount (fun i -> builder.NewLabel())
    let temps = Array.init tree.TempCount (fun i -> builder.NewLocal(typeof<uint16>))

    let unexpectedNodeFound o = failcompilef "Encountered %s, which should not appear in a lowered tree." (o.GetType().Name)

    let peekStack() =
        builder.Arguments.LoadStack()
        builder.Arguments.LoadSP()
        builder.Arrays.LoadUInt16()

    let popStack() =
        peekStack()

        builder.Arguments.LoadSP()
        builder.Math.Subtract(1)
        builder.Arguments.StoreSP()

    let pushStack loadValue =
        builder.Arguments.LoadStack()
        builder.Arguments.LoadSP()

        builder.Math.Add(1)
        builder.EvaluationStack.Duplicate()
        builder.Arguments.StoreSP()

        loadValue()
        builder.Arrays.StoreUInt16()

    let updateStack loadValue =
        builder.Arguments.LoadStack()
        builder.Arguments.LoadSP()
        loadValue()
        builder.Arrays.StoreUInt16()

    let emitConstant = function
        | Byte(v)  -> builder.EvaluationStack.Load(v)
        | Word(v)  -> builder.EvaluationStack.Load(v)
        | Int32(v) -> builder.EvaluationStack.Load(v)
        | Text(v)  -> builder.EvaluationStack.Load(v)

    let rec emitExpression = function
        | ConstantExpr(c) ->
            emitConstant c
        | TempExpr(t) ->
            temps.[t].Load()
        | ReadLocalExpr(i) ->
            builder.Arguments.LoadLocals()
            emitExpression i
            builder.Arrays.LoadUInt16()
        | StackPopExpr ->
            popStack()
        | StackPeekExpr ->
            peekStack()
        | e ->
            unexpectedNodeFound e

    let rec emitStatement = function
        | LabelStmt(i) ->
            labels.[i].Mark()
        | ReturnStmt(e) ->
            emitExpression e
            builder.Return()
        | JumpStmt(i) ->
            labels.[i].Branch(short = false)
        | BranchStmt(c,e,s) ->
            let conditionNotMet = builder.NewLabel()
            emitExpression e
            builder.EvaluationStack.Load(c)
            conditionNotMet.BranchIf(Condition.NotEqual, short = false)
            emitStatement s
            conditionNotMet.Mark()
        | QuitStmt ->
            builder.ThrowException<ZMachineQuitException>()
        | WriteTempStmt(t,e) ->
            emitExpression e
            temps.[t].Store()
        | WriteLocalStmt(i,v) ->
            builder.Arguments.LoadLocals()
            emitExpression i
            emitExpression v
            builder.Arrays.StoreUInt16()
        | StackPushStmt(e) ->
            pushStack
                (fun () -> emitExpression e)
        | StackUpdateStmt(e) ->
            updateStack
                (fun () -> emitExpression e)
        | s ->
            unexpectedNodeFound s

    member x.CompileTree(tree: BoundTree) =
        for s in tree.Statements do
            emitStatement s

    static member Compile (memory: Memory, routine: Routine, builder: ILBuilder) =
        let binder = new RoutineBinder(memory)
        let tree = binder.BindRoutine(routine)

        let codeGenerator = new CodeGenerator(tree, builder)
        codeGenerator.CompileTree(tree)
