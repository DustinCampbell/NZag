namespace NZag.Core

open System
open System.Reflection
open System.Reflection.Emit
open NZag.Utilities

type IArguments =
    abstract member LoadMachine : unit -> unit
    abstract member LoadMemory : unit -> unit
    abstract member LoadLocals : unit -> unit
    abstract member LoadStack : unit -> unit
    abstract member LoadSP : unit -> unit
    abstract member StoreSP : unit -> unit
    abstract member LoadArgCount : unit -> unit

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

type IConvert =
    abstract member ToInt16 : unit -> unit

type IArrays =
    abstract member LoadUInt16 : unit -> unit
    abstract member StoreUInt16 : unit -> unit

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
            member y.LoadArgCount() =
                generator.Emit(OpCodes.Ldarg_S, 5uy) }

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

    member x.Convert =
        { new IConvert with
            member y.ToInt16() =
                generator.Emit(OpCodes.Conv_I2) }

    member x.Arrays =
        { new IArrays with
            member y.LoadUInt16() =
                generator.Emit(OpCodes.Ldelem_U2)
            member y.StoreUInt16() =
                generator.Emit(OpCodes.Stelem_I2) }

    member x.Call(methodInfo: MethodInfo) =
        generator.Emit(OpCodes.Call, methodInfo)

    member x.Return() =
        generator.Emit(OpCodes.Ret)

    member x.NewLabel() =
        let label = generator.DefineLabel()
        let marked = ref false

        let emitBranch br br_s short =
            if short then generator.Emit(br_s, label)
            else generator.Emit(br, label)

        { new ILabel with
            member x.Mark() =
                if (!marked) then
                    invalidOperation "Attempted to mark label that has already been marked."

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
                | _  -> invalidOperation "Unsupported branch condition: %A (short = %b)" condition short }

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
            else invalidOperation "Unsupported array element type: %s" elementType.FullName

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
