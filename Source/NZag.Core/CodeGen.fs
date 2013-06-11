namespace NZag.Core

open NZag.Reflection
open NZag.Utilities

type IMachine =

    abstract member GetInitialLocalArray : Routine -> uint16[]
    abstract member ReleaseLocalArray : uint16[] -> unit

    abstract member Compile : Routine -> ZCompileResult
    abstract member GetCallSite : address:int -> ZFuncCallSite

    abstract member Verify : unit -> bool

    abstract member Randomize : seed:int16 -> unit
    abstract member NextRandomNumber : range:int16 -> uint16

    abstract member ReadZText : address:int -> string
    abstract member ReadZTextOfLength : address:int * length:int -> string

    abstract member ReadInputChar : unit -> char
    abstract member ReadInputText : textBuffer:int * parseBuffer:int -> int

    abstract member WriteOutputChar : ch:char -> unit
    abstract member WriteOutputText : s:string -> unit

    abstract member SetWindow : window:int -> unit
    abstract member SplitWindow : lines:int -> unit

    abstract member SetCursor : line:int * column:int -> unit

    abstract member SetTextStyle : style:ZTextStyle -> unit

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

and ZFuncCallSite(machine: IMachine, routine: Routine) =

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

    static member GetInvokeMethod(argCount: int) =
        let name = sprintf "Invoke%d" argCount

        let parameterTypes = Array.zeroCreate (argCount + 3)

        for i = 0 to argCount-1 do
            parameterTypes.[i] <- typeof<uint16>

        parameterTypes.[argCount] <- typeof<Memory>
        parameterTypes.[argCount+1] <- typeof<uint16[]>
        parameterTypes.[argCount+2] <- typeof<int>

        typeof<ZFuncCallSite>.GetMethod(name, parameterTypes)

type CodeGenerator private (tree: BoundTree, machine: IMachine, builder: ILBuilder, callSites: ResizeArray<ZFuncCallSite>) =

    let labels = Array.init tree.LabelCount (fun i -> builder.NewLabel())
    let temps = Array.init tree.TempCount (fun i -> builder.NewLocal(typeof<uint16>))

    let unexpectedNodeFound o = failcompilef "Encountered %s, which should not appear in a lowered tree." (o.GetType().Name)

    let readByte = typeof<Memory>.GetMethod("ReadByte", [|typeof<int>|])
    let readWord = typeof<Memory>.GetMethod("ReadWord", [|typeof<int>|])
    let writeByte = typeof<Memory>.GetMethod("WriteByte", [|typeof<int>; typeof<byte>|])
    let writeWord = typeof<Memory>.GetMethod("WriteWord", [|typeof<int>; typeof<uint16>|])

    let getCallSite = typeof<IMachine>.GetMethod("GetCallSite")
    let verify = typeof<IMachine>.GetMethod("Verify")
    let randomize = typeof<IMachine>.GetMethod("Randomize")
    let nextRandomNumber = typeof<IMachine>.GetMethod("NextRandomNumber")
    let readZText = typeof<IMachine>.GetMethod("ReadZText")
    let readZTextOfLength = typeof<IMachine>.GetMethod("ReadZTextOfLength")
    let readInputChar = typeof<IMachine>.GetMethod("ReadInputChar")
    let readInputText = typeof<IMachine>.GetMethod("ReadInputText")
    let writeOutputChar = typeof<IMachine>.GetMethod("WriteOutputChar")
    let writeOutputText = typeof<IMachine>.GetMethod("WriteOutputText")
    let splitWindow = typeof<IMachine>.GetMethod("SplitWindow")
    let setWindow = typeof<IMachine>.GetMethod("SetWindow")
    let setCursor = typeof<IMachine>.GetMethod("SetCursor")
    let setTextStyle = typeof<IMachine>.GetMethod("SetTextStyle")

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

    let emitConversion = function
        | ConversionKind.ToByte ->
            builder.Convert.ToByte()
        | ConversionKind.ToInt16 ->
            builder.Convert.ToInt16()
        | ConversionKind.ToUInt16 ->
            builder.Convert.ToUInt16()
        | kind ->
            failcompilef "Can't emit code for conversion: %A" kind

    let emitUnaryOperation = function
        | UnaryOperationKind.Not ->
            builder.Math.Not()
        | UnaryOperationKind.Negate ->
            builder.Math.Negate()
        | kind ->
            failcompilef "Can't emit code for unary operator: %A" kind

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
            failcompilef "Can't emit code for binary operator: %A" kind

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
        | ConversionExpr(k,e) ->
            emitExpression e
            emitConversion k
        | NumberToTextExpr(e) ->
            let temp = builder.NewLocal(typeof<int16>)
            emitExpression e
            temp.Store()

            temp.LoadAddress()
            builder.Call(typeof<int16>.GetMethod("ToString", [||]))

        | CallExpr(a,args) ->
            match a with
            | ConstantExpr(Int32Value a) -> emitDirectCall a args
            | _ -> emitComputedCall a args
        | ArgCountExpr ->
            builder.Arguments.LoadArgCount()
        | ReadMemoryByteExpr(a) ->
            builder.Arguments.LoadMemory()
            emitExpression a
            builder.Call(readByte)
        | ReadMemoryWordExpr(a) ->
            builder.Arguments.LoadMemory()
            emitExpression a
            builder.Call(readWord)
        | ReadMemoryTextExpr(a) ->
            builder.Arguments.LoadMachine()
            emitExpression a
            builder.Call(readZText)
        | ReadMemoryTextOfLengthExpr(a,l) ->
            builder.Arguments.LoadMachine()
            emitExpression a
            emitExpression l
            builder.Call(readZTextOfLength)
        | GenerateRandomNumberExpr(range) ->
            builder.Arguments.LoadMachine()
            emitExpression range
            builder.Call(nextRandomNumber)
        | ReadInputTextExpr(textBuffer, parseBuffer) ->
            builder.Arguments.LoadMachine()
            emitExpression textBuffer
            emitExpression parseBuffer
            builder.Call(readInputText)
        | ReadInputCharExpr ->
            builder.Arguments.LoadMachine()
            builder.Call(readInputChar)
        | VerifyExpr ->
            builder.Arguments.LoadMachine()
            builder.Call(verify)
        | e ->
            unexpectedNodeFound e

    and emitCallSiteInvoke args =
        // NOTE: At this point a ZFuncCallSite should already be on the stack

        args
        |> List.iter (fun a -> emitExpression a)

        // The memory, stack and SP are the last arguments passed in case any arguments
        // manipulate them (e.g. globals, push/pop SP).
        builder.Arguments.LoadMemory()
        builder.Arguments.LoadStack()
        builder.Arguments.LoadSP()

        builder.Call(ZFuncCallSite.GetInvokeMethod(args.Length))

    and emitDirectCall address args =
        // Create a new routine caller
        let callSite = machine.GetCallSite(address)
        let index = callSites.Count
        callSites.Add(callSite)

        // load routine caller
        builder.Arguments.LoadCallSites()
        builder.EvaluationStack.Load(index)
        builder.Arrays.LoadRefElement()

        emitCallSiteInvoke args

    and emitComputedCall address args =
        builder.Arguments.LoadMachine()
        emitExpression address
        builder.Call(getCallSite)

        emitCallSiteInvoke args

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
        | DiscardValueStmt(e) ->
            emitExpression e
            builder.EvaluationStack.Pop()
        | SetRandomNumberSeedStmt(seed) ->
            builder.Arguments.LoadMachine()
            emitExpression seed
            builder.Call(randomize)
        | PrintCharStmt(e) ->
            builder.Arguments.LoadMachine()
            emitExpression e
            builder.Call(writeOutputChar)
        | PrintTextStmt(e) ->
            builder.Arguments.LoadMachine()
            emitExpression e
            builder.Call(writeOutputText)
        | SetTextStyleStmt(e) ->
            builder.Arguments.LoadMachine()
            emitExpression e
            builder.Call(setTextStyle)
        | SetWindowStmt(e) ->
            builder.Arguments.LoadMachine()
            emitExpression e
            builder.Call(setWindow)
        | SplitWindowStmt(e) ->
            builder.Arguments.LoadMachine()
            emitExpression e
            builder.Call(splitWindow)
        | SetCursorStmt(line, column) ->
            builder.Arguments.LoadMachine()
            emitExpression line
            emitExpression column
            builder.Call(setCursor)
        | DebugOutputStmt(e, elist) ->
            builder.DebugOutput(
                (fun () -> emitExpression e),
                elist.Length,
                (fun i -> elist.[i] |> emitExpression))
        | RuntimeExceptionStmt(s) ->
            builder.ThrowException<RuntimeException>(s)
        | s ->
            unexpectedNodeFound s

    member x.CompileTree(tree: BoundTree) =
        for s in tree.Statements do
            emitStatement s

    static member Compile(memory: Memory, routine: Routine, machine: IMachine, builder: ILBuilder, callSites: ResizeArray<ZFuncCallSite>, debugging: bool) =
        let binder = new RoutineBinder(memory, debugging)
        let tree = binder.BindRoutine(routine)

        let codeGenerator = new CodeGenerator(tree, machine, builder, callSites)
        codeGenerator.CompileTree(tree)
