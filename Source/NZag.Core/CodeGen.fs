namespace NZag.Core

open NZag.Utilities

type IMachine =

    abstract member GetInitialLocalArray : Routine -> uint16[]
    abstract member ReleaseLocalArray : uint16[] -> unit

    abstract member GetOrCompileZFunc : Routine -> ZFunc

and ZFuncInvoker(machine: IMachine, routine: Routine, zfunc: ZFunc) =

    let invoke locals argCount =
        try
            zfunc.Invoke(machine, locals, argCount)
        finally
            machine.ReleaseLocalArray(locals)

    member x.Invoke0() =
        let locals = machine.GetInitialLocalArray(routine)
        invoke locals 0

    member x.Invoke1(arg1) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        invoke locals 1

    member x.Invoke2(arg1, arg2) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        invoke locals 2

    member x.Invoke3(arg1, arg2, arg3) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        invoke locals 3

    member x.Invoke4(arg1, arg2, arg3, arg4) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        invoke locals 4

    member x.Invoke5(arg1, arg2, arg3, arg4, arg5) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        locals.[4] <- arg5
        invoke locals 5

    member x.Invoke6(arg1, arg2, arg3, arg4, arg5, arg6) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        locals.[4] <- arg5
        locals.[5] <- arg6
        invoke locals 6

    member x.Invoke7(arg1, arg2, arg3, arg4, arg5, arg6, arg7) =
        let locals = machine.GetInitialLocalArray(routine)
        locals.[0] <- arg1
        locals.[1] <- arg2
        locals.[2] <- arg3
        locals.[3] <- arg4
        locals.[4] <- arg5
        locals.[5] <- arg6
        locals.[6] <- arg7
        invoke locals 7

and ZFunc = delegate of IMachine * uint16[] * int -> uint16

type CodeGenerator private (tree: BoundTree, builder: ILBuilder) =

    let labels = Array.init tree.LabelCount (fun i -> builder.NewLabel())
    let temps = Array.init tree.TempCount (fun i -> builder.NewLocal(typeof<uint16>))

    let unexpectedNodeFound o = invalidOperation "Encountered %s, which should not appear in a lowered tree." (o.GetType().Name)

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
            emitExpression e
            builder.EvaluationStack.Load(c)

            let conditionNotMet = builder.NewLabel()
            conditionNotMet.BranchIf(Condition.NotEqual, short = false)

            emitStatement s

            conditionNotMet.Mark()
        | WriteTempStmt(t,e) ->
            emitExpression e
            temps.[t].Store()
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
