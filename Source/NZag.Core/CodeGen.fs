namespace NZag.Core

open System
open System.Reflection
open System.Reflection.Emit
open NZag.Utilities

type IMachine =

    abstract member GetInitialLocalArray : Routine -> uint16[]
    abstract member ReleaseLocalArray : uint16[] -> unit

    abstract member Compile : Routine -> ZCompileResult
    abstract member GetCallSite : address:int -> ZFuncCallSite

    abstract member ReadZText : address:int -> string

    abstract member WriteOutputChar : ch:char -> unit
    abstract member WriteOutputText : s:string -> unit

    abstract member Randomize : seed:int16 -> unit
    abstract member NextRandomNumber : range:int16 -> uint16

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

type IArguments =
    abstract member LoadMachine : unit -> unit
    abstract member LoadMemory : unit -> unit
    abstract member LoadLocals : unit -> unit
    abstract member LoadStack : unit -> unit
    abstract member LoadSP : unit -> unit
    abstract member StoreSP : unit -> unit
    abstract member LoadCallSites : unit -> unit
    abstract member LoadArgCount : unit -> unit

type IRuntimeFunctions =
    abstract member GetCallSite : loadAddress:(unit -> unit) -> unit
    abstract member ReadZText : loadAddress:(unit -> unit) -> unit
    abstract member WriteOutputChar : loadChar:(unit -> unit) -> unit
    abstract member WriteOutputText : loadText:(unit -> unit) -> unit

type IEvaluationStack =
    abstract member Load : bool -> unit
    abstract member Load : byte -> unit
    abstract member Load : uint16 -> unit
    abstract member Load : int -> unit
    abstract member Load : string -> unit
    abstract member Duplicate : unit -> unit
    abstract member Pop : unit -> unit

type IMath =
    abstract member Add : unit -> unit
    abstract member Add : int -> unit
    abstract member Subtract : unit -> unit
    abstract member Subtract : int -> unit
    abstract member Multiply : unit -> unit
    abstract member Multiply : int -> unit
    abstract member Divide : unit -> unit
    abstract member Divide : int -> unit
    abstract member Remainder : unit -> unit
    abstract member Remainder : int -> unit
    abstract member And : unit -> unit
    abstract member And : int -> unit
    abstract member Or : unit -> unit
    abstract member Or : int -> unit
    abstract member ShiftLeft : unit -> unit
    abstract member ShiftLeft : int -> unit
    abstract member ShiftRight : unit -> unit
    abstract member ShiftRight : int -> unit
    abstract member Not : unit -> unit
    abstract member Negate : unit -> unit

type ICompare =
    abstract member Equal : unit -> unit
    abstract member NotEqual : unit -> unit
    abstract member LessThan : unit -> unit
    abstract member GreaterThan : unit -> unit
    abstract member AtLeast : unit -> unit
    abstract member AtMost : unit -> unit

type IConvert =
    abstract member ToInt16 : unit -> unit

type IArrays =
    abstract member LoadUInt16 : unit -> unit
    abstract member StoreUInt16 : unit -> unit
    abstract member LoadRefElement : unit -> unit
    abstract member StoreRefElement : unit -> unit

type Condition =
    | False = 0
    | True = 1
    | Equal = 2
    | NotEqual = 3
    | AtLeast = 4
    | AtMost = 5
    | LessThan = 6
    | GreaterThan = 7

type ILabel =
    abstract member Mark : unit -> unit
    abstract member Branch : short:bool -> unit
    abstract member BranchIf : condition:Condition * short:bool -> unit

type ILocal =
    abstract member Load : unit -> unit
    abstract member LoadAddress : unit -> unit
    abstract member LoadAndBox : unit -> unit
    abstract member Store : unit -> unit

    abstract member Index : int
    abstract member Type : Type

type IArrayLocal =
    inherit ILocal

    abstract member Create : int -> unit
    abstract member LoadLength : unit -> unit

    abstract member LoadElement : (unit -> unit) -> unit
    abstract member StoreElement : (unit -> unit) * (unit -> unit) -> unit

type ILBuilder (generator: ILGenerator) =

    let loadInt32 value =
        match value with
        | 0  -> generator.Emit(OpCodes.Ldc_I4_0)
        | 1  -> generator.Emit(OpCodes.Ldc_I4_1)
        | 2  -> generator.Emit(OpCodes.Ldc_I4_2)
        | 3  -> generator.Emit(OpCodes.Ldc_I4_3)
        | 4  -> generator.Emit(OpCodes.Ldc_I4_4)
        | 5  -> generator.Emit(OpCodes.Ldc_I4_5)
        | 6  -> generator.Emit(OpCodes.Ldc_I4_6)
        | 7  -> generator.Emit(OpCodes.Ldc_I4_7)
        | 8  -> generator.Emit(OpCodes.Ldc_I4_8)
        | -1 -> generator.Emit(OpCodes.Ldc_I4_M1)
        | v  -> if v >= -128 && value <= 127 then
                    generator.Emit(OpCodes.Ldc_I4_S, sbyte v)
                else
                    generator.Emit(OpCodes.Ldc_I4, v)

    let loadLocal (local: LocalBuilder) =
        match local.LocalIndex with
        | 0 -> generator.Emit(OpCodes.Ldloc_0)
        | 1 -> generator.Emit(OpCodes.Ldloc_1)
        | 2 -> generator.Emit(OpCodes.Ldloc_2)
        | 3 -> generator.Emit(OpCodes.Ldloc_3)
        | i -> if i >= 4 && i <= 255 then generator.Emit(OpCodes.Ldloc_S, local)
               else generator.Emit(OpCodes.Ldloc, local)

    let loadLocalAddress (local: LocalBuilder) =
        if local.LocalIndex >= 0 && local.LocalIndex <= 255 then generator.Emit(OpCodes.Ldloca_S, local)
        else generator.Emit(OpCodes.Ldloca, local)

    let loadLocalAndBox (local: LocalBuilder) =
        loadLocal local

        if local.LocalType.IsValueType then
            generator.Emit(OpCodes.Box, local.LocalType)

    let storeLocal (local: LocalBuilder) =
        match local.LocalIndex with
        | 0 -> generator.Emit(OpCodes.Stloc_0)
        | 1 -> generator.Emit(OpCodes.Stloc_1)
        | 2 -> generator.Emit(OpCodes.Stloc_2)
        | 3 -> generator.Emit(OpCodes.Stloc_3)
        | i -> if i >= 4 && i <= 255 then generator.Emit(OpCodes.Stloc_S, local)
               else generator.Emit(OpCodes.Stloc, local)

    member x.Arguments =
        { new IArguments with
            member y.LoadMachine() =
                generator.Emit(OpCodes.Ldarg_0)
            member y.LoadMemory() =
                generator.Emit(OpCodes.Ldarg_1)
            member y.LoadLocals() =
                generator.Emit(OpCodes.Ldarg_2)
            member y.LoadStack() =
                generator.Emit(OpCodes.Ldarg_3)
            member y.LoadSP() =
                generator.Emit(OpCodes.Ldarg_S, 4uy)
            member y.StoreSP() =
                generator.Emit(OpCodes.Starg_S, 4uy)
            member y.LoadCallSites() =
                generator.Emit(OpCodes.Ldarg_S, 5uy)
            member y.LoadArgCount() =
                generator.Emit(OpCodes.Ldarg_S, 6uy) }

    member x.RuntimeFunctions =
        let getCallSite = typeof<IMachine>.GetMethod("GetCallSite")
        let readZText = typeof<IMachine>.GetMethod("ReadZText")
        let writeOutputChar = typeof<IMachine>.GetMethod("WriteOutputChar")
        let writeOutputText = typeof<IMachine>.GetMethod("WriteOutputText")

        let invoke loadArgs methodInfo =
            x.Arguments.LoadMachine()
            loadArgs()
            x.Call(methodInfo)

        { new IRuntimeFunctions with
            member y.GetCallSite loadAddress =
                invoke loadAddress getCallSite
            member y.ReadZText loadAddress =
                invoke loadAddress readZText
            member y.WriteOutputChar loadChar =
                invoke loadChar writeOutputChar
            member y.WriteOutputText loadText =
                invoke loadText writeOutputText }

    member x.EvaluationStack =
        { new IEvaluationStack with
            member y.Load(value: bool) =
                if value then loadInt32 1
                else loadInt32 0
            member y.Load(value: byte) =
                loadInt32 (int value)
            member y.Load(value: uint16) =
                loadInt32 (int value)
            member y.Load(value: int) =
                loadInt32 value
            member y.Load(value: string) =
                generator.Emit(OpCodes.Ldstr, value)
            member y.Duplicate() =
                generator.Emit(OpCodes.Dup)
            member y.Pop() =
                generator.Emit(OpCodes.Pop) }

    member x.Math =
        { new IMath with
            member y.Add() =
                generator.Emit(OpCodes.Add)
            member y.Add(value) =
                loadInt32 value
                generator.Emit(OpCodes.Add)
            member y.Subtract() =
                generator.Emit(OpCodes.Sub)
            member y.Subtract(value) =
                loadInt32 value
                generator.Emit(OpCodes.Sub)
            member y.Multiply() =
                generator.Emit(OpCodes.Mul)
            member y.Multiply(value) =
                loadInt32 value
                generator.Emit(OpCodes.Mul)
            member y.Divide() =
                generator.Emit(OpCodes.Div)
            member y.Divide(value) =
                loadInt32 value
                generator.Emit(OpCodes.Div)
            member y.Remainder() =
                generator.Emit(OpCodes.Rem)
            member y.Remainder(value) =
                loadInt32 value
                generator.Emit(OpCodes.Rem)
            member y.And() =
                generator.Emit(OpCodes.And)
            member y.And(value) =
                loadInt32 value
                generator.Emit(OpCodes.And)
            member y.Or() =
                generator.Emit(OpCodes.Or)
            member y.Or(value) =
                loadInt32 value
                generator.Emit(OpCodes.Or)
            member y.ShiftLeft() =
                generator.Emit(OpCodes.Shl)
            member y.ShiftLeft(value) =
                loadInt32 value
                generator.Emit(OpCodes.Shl)
            member y.ShiftRight() =
                generator.Emit(OpCodes.Shr)
            member y.ShiftRight(value) =
                loadInt32 value
                generator.Emit(OpCodes.Shr)
            member y.Not() =
                generator.Emit(OpCodes.Not)
            member y.Negate() =
                generator.Emit(OpCodes.Neg) }

    member x.Compare =
        { new ICompare with
            member y.Equal() =
                generator.Emit(OpCodes.Ceq)
            member y.NotEqual() =
                generator.Emit(OpCodes.Ceq)
                loadInt32 0
                generator.Emit(OpCodes.Ceq)
            member y.LessThan() =
                generator.Emit(OpCodes.Clt)
            member y.GreaterThan() =
                generator.Emit(OpCodes.Cgt)
            member y.AtLeast() =
                generator.Emit(OpCodes.Clt)
                loadInt32 0
                generator.Emit(OpCodes.Ceq)
            member y.AtMost() =
                generator.Emit(OpCodes.Cgt)
                loadInt32 0
                generator.Emit(OpCodes.Ceq) }

    member x.Convert =
        { new IConvert with
            member y.ToInt16() =
                generator.Emit(OpCodes.Conv_I2) }

    member x.Arrays =
        { new IArrays with
            member y.LoadUInt16() =
                generator.Emit(OpCodes.Ldelem_U2)
            member y.StoreUInt16() =
                generator.Emit(OpCodes.Stelem_I2)
            member y.LoadRefElement() =
                generator.Emit(OpCodes.Ldelem_Ref)
            member y.StoreRefElement() =
                generator.Emit(OpCodes.Stelem_Ref) }

    member x.Call(methodInfo: MethodInfo) =
        generator.Emit(OpCodes.Call, methodInfo)

    member x.Return() =
        generator.Emit(OpCodes.Ret)

    member x.DebugOutput(loadMessage: unit -> unit) =
        let writeLine = typeof<System.Diagnostics.Debug>.GetMethod("WriteLine", [|typeof<string>|])
        loadMessage()
        x.Call(writeLine)

    member x.ThrowException<'T when 'T :> Exception>() =
        let exceptionType = typeof<'T>
        let exceptionCtor = exceptionType.GetConstructor([||])
        generator.Emit(OpCodes.Newobj, exceptionCtor)
        generator.Emit(OpCodes.Throw)

    member x.ThrowException<'T when 'T :> Exception>(message: string) =
        let exceptionType = typeof<'T>
        let exceptionCtor = exceptionType.GetConstructor([|typeof<string>|])
        x.EvaluationStack.Load(message)
        generator.Emit(OpCodes.Newobj, exceptionCtor)
        generator.Emit(OpCodes.Throw)

    member x.NewLabel() =
        let label = generator.DefineLabel()
        let marked = ref false

        let emitBranch br br_s short =
            if short then generator.Emit(br_s, label)
            else generator.Emit(br, label)

        { new ILabel with
            member x.Mark() =
                if (!marked) then
                    failcompile "Attempted to mark label that has already been marked."

                generator.MarkLabel(label)
                marked := true

            member x.Branch(short) =
                emitBranch OpCodes.Br OpCodes.Br_S short

            member x.BranchIf(condition, short) =
                match condition with
                | Condition.False       -> emitBranch OpCodes.Brfalse OpCodes.Brfalse_S short
                | Condition.True        -> emitBranch OpCodes.Brtrue OpCodes.Brtrue_S short
                | Condition.NotEqual    -> emitBranch OpCodes.Bne_Un OpCodes.Bne_Un_S short
                | Condition.Equal       -> emitBranch OpCodes.Beq OpCodes.Beq_S short
                | Condition.AtLeast     -> emitBranch OpCodes.Bge OpCodes.Bge_S short
                | Condition.AtMost      -> emitBranch OpCodes.Ble OpCodes.Ble_S short
                | Condition.LessThan    -> emitBranch OpCodes.Blt OpCodes.Blt_S short
                | Condition.GreaterThan -> emitBranch OpCodes.Bgt OpCodes.Bgt_S short
                | _  -> failcompilef "Unsupported branch condition: %A (short = %b)" condition short }

    member x.NewLocal(localType: Type) =
        let local = generator.DeclareLocal(localType)

        { new ILocal with
            member y.Load() =
                loadLocal local
            member y.LoadAddress() =
                loadLocalAddress local
            member y.LoadAndBox() =
                loadLocalAndBox local
            member y.Store() =
                storeLocal local

            member y.Index = local.LocalIndex
            member y.Type = local.LocalType }

    member x.NewArrayLocal(elementType: Type) =
        let arrayType = elementType.MakeArrayType()

        let Ldelem, Stelem =
            if elementType = typeof<int> then OpCodes.Ldelem_I4, OpCodes.Stelem_I4
            elif elementType = typeof<uint16> then OpCodes.Ldelem_U2, OpCodes.Stelem_I2
            elif elementType = typeof<byte> then OpCodes.Ldelem_U1, OpCodes.Stelem_I1
            elif elementType.IsClass then OpCodes.Ldelem_Ref, OpCodes.Stelem_Ref
            else failcompilef "Unsupported array element type: %s" elementType.FullName

        let local = generator.DeclareLocal(arrayType)

        { new IArrayLocal with
            member y.Load() =
                loadLocal local
            member y.LoadAddress() =
                loadLocalAddress local
            member y.LoadAndBox() =
                loadLocalAndBox local
            member y.Store() =
                storeLocal local

            member y.Index = local.LocalIndex
            member y.Type = local.LocalType

            member y.Create(length) =
                loadInt32 length
                generator.Emit(OpCodes.Newarr, elementType)
                storeLocal local

            member y.LoadLength() =
                loadLocal local
                generator.Emit(OpCodes.Ldlen)

            member y.LoadElement(loadIndex) =
                loadLocal local
                loadIndex()
                generator.Emit(Ldelem)

            member y.StoreElement(loadIndex, loadValue) =
                loadLocal local
                loadIndex()
                loadValue()
                generator.Emit(Stelem) }

type CodeGenerator private (tree: BoundTree, machine: IMachine, builder: ILBuilder, callSites: ResizeArray<ZFuncCallSite>) =

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

    let emitConversion = function
        | ConversionKind.ToInt16 ->
            builder.Convert.ToInt16()
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
        | ReadMemoryByteExpr(a) ->
            builder.Arguments.LoadMemory()
            emitExpression a
            builder.Call(readByte)
        | ReadMemoryWordExpr(a) ->
            builder.Arguments.LoadMemory()
            emitExpression a
            builder.Call(readWord)
        | ReadMemoryTextExpr(a) ->
            builder.RuntimeFunctions.ReadZText
                (fun () -> emitExpression a)
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
        builder.RuntimeFunctions.GetCallSite (fun () -> emitExpression address)
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
        | PrintTextStmt(e) ->
            builder.RuntimeFunctions.WriteOutputText
                (fun () -> emitExpression e)
        | DebugOutputStmt(e) ->
            builder.DebugOutput
                (fun () -> emitExpression e)
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
