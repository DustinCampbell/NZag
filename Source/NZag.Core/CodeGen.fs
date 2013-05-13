namespace NZag.Core

open NZag.Utilities

type IMachine =

    abstract member GetInitialLocalArray : Routine -> uint16[]
    abstract member ReleaseLocalArray : uint16[] -> unit

    abstract member Compile : Routine -> ZCompileResult

and ZCompileResult =
  { Routine : Routine
    ZFunc : ZFunc
    CallSites : ZFuncCallSite[] }

and ZFunc = delegate of memory:Memory
                       * locals:uint16[]
                       * stack:uint16[]
                       * sp:int
                       * callSites:ZFuncCallSite[]
                       * argCount:int
                      -> uint16

and ZFuncCallSite(machine: IMachine, routine: Routine, zfunc: ZFunc) =

    let mutable compileResult = None

    let getCompileResult() = 
        if compileResult = None then
            compileResult <- Some(machine.Compile(routine))

        compileResult.Value

    let invoke memory locals stack sp argCount =
        try
            let compileResult = getCompileResult()
            compileResult.ZFunc.Invoke(memory, locals, stack, sp, compileResult.CallSites, argCount)
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

type CodeGenerator private (tree: BoundTree, builder: ILBuilder, callSites: ResizeArray<ZFuncCallSite>) =

    let labels = Array.init tree.LabelCount (fun i -> builder.NewLabel())
    let temps = Array.init tree.TempCount (fun i -> builder.NewLocal(typeof<uint16>))

    let unexpectedNodeFound o = failcompilef "Encountered %s, which should not appear in a lowered tree." (o.GetType().Name)

    let readByte = typeof<Memory>.GetMethod("ReadByte", [|typeof<int>|])
    let readWord = typeof<Memory>.GetMethod("ReadWord", [|typeof<int>|])
    let writeByte = typeof<Memory>.GetMethod("WriteByte", [|typeof<int>; typeof<byte>|])
    let writeWord = typeof<Memory>.GetMethod("WriteWord", [|typeof<int>; typeof<uint16>|])

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

    let emitUnaryOperation = function
        | UnaryOperationKind.Not ->
            builder.Math.Not()
        | UnaryOperationKind.Negate ->
            builder.Math.Negate()
        | kind ->
            builder.ThrowException<RuntimeException>(sprintf "Can't emit code for unary operator: %A" kind)

    let emitBinaryOperation = function
        | BinaryOperationKind.Add ->
            builder.Math.Add()
        | BinaryOperationKind.Subtract ->
            builder.Math.Subtract()
        | BinaryOperationKind.Multiply ->
            builder.Math.Multiply()
        | BinaryOperationKind.Divide ->
            builder.Math.Divide()
        | BinaryOperationKind.Remainder ->
            builder.Math.Remainder()
        | BinaryOperationKind.And ->
            builder.Math.And()
        | BinaryOperationKind.Or ->
            builder.Math.Or()
        | BinaryOperationKind.ShiftLeft ->
            builder.Math.ShiftLeft()
        | BinaryOperationKind.ShiftRight ->
            builder.Math.ShiftRight()
        | BinaryOperationKind.Equal ->
            builder.Compare.Equal()
        | BinaryOperationKind.NotEqual ->
            builder.Compare.NotEqual()
        | BinaryOperationKind.LessThan ->
            builder.Compare.LessThan()
        | BinaryOperationKind.AtMost ->
            builder.Compare.AtMost()
        | BinaryOperationKind.AtLeast ->
            builder.Compare.AtLeast()
        | BinaryOperationKind.GreaterThan ->
            builder.Compare.GreaterThan()
        | kind ->
            builder.ThrowException<RuntimeException>(sprintf "Can't emit code for unary operator: %A" kind)

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
        | UnaryOperationExpr(k,e) ->
            emitExpression e
            emitUnaryOperation k
        | BinaryOperationExpr(k,l,r) ->
            emitExpression l
            emitExpression r
            emitBinaryOperation k
        | ReadMemoryByteExpr(a) ->
            builder.Arguments.LoadMemory()
            emitExpression a
            builder.Call(readByte)
        | ReadMemoryWordExpr(a) ->
            builder.Arguments.LoadMemory()
            emitExpression a
            builder.Call(readWord)
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
        | WriteMemoryByteStmt(a,v) ->
            builder.Arguments.LoadMemory()
            emitExpression a
            emitExpression v
            builder.Call(writeByte)
        | WriteMemoryWordStmt(a,v) ->
            builder.Arguments.LoadMemory()
            emitExpression a
            emitExpression v
            builder.Call(writeWord)
        | s ->
            unexpectedNodeFound s

    member x.CompileTree(tree: BoundTree) =
        for s in tree.Statements do
            emitStatement s

    static member Compile (memory: Memory, routine: Routine, builder: ILBuilder, callSites: ResizeArray<ZFuncCallSite>) =
        let binder = new RoutineBinder(memory)
        let tree = binder.BindRoutine(routine)

        let codeGenerator = new CodeGenerator(tree, builder, callSites)
        codeGenerator.CompileTree(tree)
