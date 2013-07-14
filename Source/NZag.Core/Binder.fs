namespace NZag.Core

open NZag.Utilities
open BoundNodeConstruction
open BoundNodeVisitors
open OperandPatterns

[<Struct>]
type ByRefOperand(read: (unit -> Expression), write: (Expression -> unit)) =

    member x.Read() = read()
    member x.Write value = write value

    static member (!!) (op: ByRefOperand) = op.Read()
    static member (<--) (op: ByRefOperand, v: Expression) =
        op.Write(v)

[<AbstractClass>]
type Binder(memory: Memory, builder: BoundTreeCreator, debugging: bool) =

    member x.ReadByte address =
        ReadMemoryByteExpr(address)
    member x.ReadWord address =
        ReadMemoryWordExpr(address)

    member x.WriteByte address value =
        WriteMemoryByteStmt(address, value)
            |> builder.AddStatement
    member x.WriteWord address value =
        WriteMemoryWordStmt(address, value)
            |> builder.AddStatement

    member x.DebugOut message args =
        DebugOutputStmt(textConst message, args)
            |> builder.AddStatement

    member x.RuntimeException message =
        RuntimeExceptionStmt(message)
            |> builder.AddStatement

type ObjectBinder(memory, builder, debugging) as this =
    inherit Binder(memory, builder, debugging)

    let objectTableAddress = int (memory |> Header.readObjectTableAddress)
    let propertyDefaultsSize = if memory.Version <= 3 then 31 else 63
    let objectEntriesAddress = objectTableAddress + (propertyDefaultsSize * 2)
    let entrySize = if memory.Version <= 3 then 9 else 14
    let parentOffset = if memory.Version <= 3 then 4 else 6
    let siblingOffset = if memory.Version <= 3 then 5 else 8
    let childOffset = if memory.Version <= 3 then 6 else 10
    let propertyTableOffset = if memory.Version <= 3 then 7 else 12

    let computePropertyDefaultAddress propNum =
        match propNum with
        | ConstantExpr(Int32Value propNum) ->
            int32Const (objectTableAddress + ((propNum - 1) * 2))
        | _ ->
            (int32Const objectTableAddress) .+. ((propNum .-. one) .*. two)

    let computeObjectAddress objNum =
        match objNum with
        | ConstantExpr(Int32Value objNum) ->
            int32Const (objectEntriesAddress + ((objNum - 1) * entrySize))
        | _ ->
            ((objNum .-. one) .*. (int32Const entrySize)) .+. (int32Const objectEntriesAddress)

    let computeChildAddress objNum =
        let objAddress = computeObjectAddress objNum
        match objAddress with
        | ConstantExpr(Int32Value objAddress) ->
            int32Const (objAddress + childOffset)
        | _ ->
            objAddress .+. (int32Const childOffset)

    let computeParentAddress objNum =
        let objAddress = computeObjectAddress objNum
        match objAddress with
        | ConstantExpr(Int32Value objAddress) ->
            int32Const (objAddress + parentOffset)
        | _ ->
            objAddress .+. (int32Const parentOffset)

    let computeSiblingAddress objNum =
        let objAddress = computeObjectAddress objNum
        match objAddress with
        | ConstantExpr(Int32Value objAddress) ->
            int32Const (objAddress + siblingOffset)
        | _ ->
            objAddress .+. (int32Const siblingOffset)

    let computePropertyTableAddress objNum =
        let objAddress = computeObjectAddress objNum
        match objAddress with
        | ConstantExpr(Int32Value objAddress) ->
            int32Const (objAddress + propertyTableOffset)
        | _ ->
            objAddress .+. (int32Const propertyTableOffset)

    let computeAttributeByteAddress objNum attrNum =
        let objAddress = computeObjectAddress objNum
        match objAddress, attrNum with
        | ConstantExpr(Int32Value objAddress), ConstantExpr(Int32Value attrNum) ->
            int32Const (objAddress + (attrNum / 8))
        | _ ->
            objAddress .+. (attrNum ./. eight)

    let computeAttributeBitMask attrNum =
        match attrNum with
        | ConstantExpr(Int32Value attrNum) ->
            int32Const (1 <<< ((7 - (attrNum % 8)) &&& 0x1f))
        | _ ->
            one .<<. ((seven .-. (attrNum .%. eight)) .&. int32Const 0x1f)

    let getFirstPropertyAddress objNum =
        let propTableAddress = computePropertyTableAddress objNum
        let propsAddress = builder.InitTemp (this.ReadWord propTableAddress)

        // First property is address after object name
        let nameLength = this.ReadByte !!propsAddress
        let result = (!!propsAddress .+. one) .+. (nameLength .*. two)

        result |> toUInt16

    let getNextPropertyAddress propAddress =
        let propSizeByte = builder.InitTemp (this.ReadByte propAddress)

        let propSize = 
            if memory.Version <= 3 then
                !!propSizeByte .>>. five
            else
                let scratch = builder.InitTemp zero
                builder.IfThenElse ((!!propSizeByte .&. (int32Const 0x80)) .<>. (int32Const 0x80))
                    (fun () ->
                        scratch <-- (!!propSizeByte .>>. six)
                    )
                    (fun () -> 
                        let nextByteAddress = propAddress .+. one
                        let nextPropSizeByte = this.ReadByte nextByteAddress
                        let nextPropSizeByte' = nextPropSizeByte .&. (int32Const 0x3f)
                        builder.IfThenElse (nextPropSizeByte' .=. zero)
                            (fun () ->
                                scratch <-- (int32Const 64)
                            )
                            (fun () -> 
                                scratch <-- nextPropSizeByte')
                            )
                !!scratch

        let result = (propAddress .+. one) .+. (propSize .+. one)

        result |> toUInt16

    member x.ReadPropertyDefault propNum =
        let propDefaultAddress = computePropertyDefaultAddress propNum
        x.ReadWord propDefaultAddress

    member x.ReadObjectNumber address =
        if memory.Version <= 3 then x.ReadByte address
        else x.ReadWord address

    member x.WriteObjectNumber address value =
        if memory.Version <= 3 then x.WriteByte address value
        else x.WriteWord address value

    member x.ReadChild objNum =
        let childAddress = computeChildAddress objNum
        x.ReadObjectNumber childAddress

    member x.WriteChild objNum value =
        let childAddress = computeChildAddress objNum
        x.WriteObjectNumber childAddress value

    member x.ReadParent objNum =
        let parentAddress = computeParentAddress objNum
        x.ReadObjectNumber parentAddress

    member x.WriteParent objNum value =
        let parentAddress = computeParentAddress objNum
        x.WriteObjectNumber parentAddress value

    member x.ReadSibling objNum =
        let siblingAddress = computeSiblingAddress objNum
        x.ReadObjectNumber siblingAddress

    member x.WriteSibling objNum value =
        let siblingAddress = computeSiblingAddress objNum
        x.WriteObjectNumber siblingAddress value

    member x.ReadAttribute objNum attrNum =
        let bitMask = computeAttributeBitMask attrNum
        let attrByteAddress = computeAttributeByteAddress objNum attrNum
        let attrByte = x.ReadByte attrByteAddress
        (attrByte .&. bitMask) .<>. zero

    member x.WriteAttribute objNum attrNum value =
        let bitMask = computeAttributeBitMask attrNum
        let attrByteAddress = builder.InitTemp (computeAttributeByteAddress objNum attrNum)
        let attrByte = x.ReadByte !!attrByteAddress
        let newAttrByte =
            if value then
                (attrByte .|. bitMask) |> toByte
            else
                (attrByte .&. (bitNot bitMask)) |> toByte

        x.WriteByte !!attrByteAddress newAttrByte

    member x.ReadShortName objNum =
        let propTableAddress = computePropertyTableAddress objNum
        let propsAddress = builder.InitTemp (x.ReadWord propTableAddress)
        let nameLength = x.ReadByte !!propsAddress
        let nameAddress = !!propsAddress .+. one
        readTextOfLength nameAddress nameLength

    member x.GetPropertyAddress objNum propNum =
        let firstPropertyAddress = getFirstPropertyAddress objNum

        let propAddress = builder.InitTemp(firstPropertyAddress)
        let firstByte = builder.InitTemp(zero)
        let stop = builder.InitBoolTemp(false)
        let result = builder.InitTemp(zero)

        let mask = if memory.Version <= 3 then byteConst 0x1fuy else byteConst 0x3fuy

        builder.LoopWhile (stop.IsFalse)
            (fun () ->
                firstByte <-- (x.ReadByte !!propAddress)

                builder.IfThenElse ((!!firstByte .&. mask) .<=. propNum)
                    (fun () ->
                        stop <-- true
                    )
                    (fun () ->
                        propAddress <-- (getNextPropertyAddress !!propAddress)
                    )
            )

        builder.IfThen ((!!firstByte .&. mask) .=. propNum)
            (fun () ->
                if memory.Version >= 4 then
                    builder.IfThen ((!!firstByte .&. (int32Const 0x80)) .<>. zero)
                        (fun() ->
                            propAddress <-- (!!propAddress .+. one)
                        )

                result <-- (!!propAddress .+. one))

        !!result

    member x.GetPropertyLength dataAddress =
        let result = builder.InitTemp(zero)

        builder.IfThen (dataAddress .<>. zero)
            (fun () ->
                result <-- (x.ReadByte (dataAddress .-. one))

                if memory.Version <= 3 then
                    result <-- (((!!result .>>. five) .+. one) |> toByte)
                else
                    builder.IfThenElse ((!!result .&. (int32Const 0x80)) .=. zero)
                        (fun () ->
                            result <-- (((!!result .>>. six) .+. one) |> toByte)
                        )
                        (fun () ->
                            result <-- (!!result .&. (int32Const 0x3f))
                        )
                builder.IfThen (!!result .=. zero)
                    (fun () ->
                        result <-- int32Const 64
                    )
            )

        !!result

    member x.GetNextPropertyNumber objNum propNum =
        let firstPropertyAddress = getFirstPropertyAddress(objNum)
        let propAddress = builder.InitTemp(firstPropertyAddress)
        let firstByte = builder.InitTemp (x.ReadByte !!propAddress)

        let mask = if memory.Version <= 3 then byteConst 0x1fuy else byteConst 0x3fuy

        builder.IfThen (propNum .<>. zero)
            (fun () ->
                builder.LoopWhile ((!!firstByte .&. mask) .>. propNum)
                    (fun () ->
                        firstByte <-- (x.ReadByte !!propAddress)
                        let nextPropAddress = getNextPropertyAddress !!propAddress
                        propAddress <-- nextPropAddress
                    )
            )

        (x.ReadByte !!propAddress) .&. mask

    member x.ReadPropertyValue objNum propNum =
        let firstPropertyAddress = getFirstPropertyAddress(objNum)
        let propAddress = builder.InitTemp(firstPropertyAddress)
        let firstByte = builder.InitTemp(zero)
        let stop = builder.InitBoolTemp(false)
        let result = builder.InitTemp(zero)

        let mask = if memory.Version <= 3 then byteConst 0x1fuy else byteConst 0x3fuy
        let comp = if memory.Version <= 3 then byteConst 0xe0uy else byteConst 0xc0uy

        builder.LoopWhile (stop.IsFalse)
            (fun () ->
                firstByte <-- (x.ReadByte !!propAddress)
                builder.IfThenElse ((!!firstByte .&. mask) .<=. propNum)
                    (fun () ->
                        stop <-- true
                    )
                    (fun () ->
                        propAddress <-- (getNextPropertyAddress !!propAddress)
                    )
            )

        builder.IfThenElse ((!!firstByte .&. mask) .=. propNum)
            (fun () ->
                propAddress <-- (!!propAddress .+. one)

                builder.IfThenElse ((!!firstByte .&. comp) .=. zero)
                    (fun () ->
                        result <-- (x.ReadByte !!propAddress)
                    )
                    (fun () ->
                        result <-- (x.ReadWord !!propAddress)
                    )
            )
            (fun () ->
                result <-- (x.ReadPropertyDefault propNum)
            )

        !!result

    member x.WritePropertyValue objNum propNum value =
        let firstPropertyAddress = getFirstPropertyAddress(objNum)
        let propAddress = builder.InitTemp(firstPropertyAddress)
        let firstByte = builder.InitTemp(zero)
        let stop = builder.InitBoolTemp(false)

        let mask = if memory.Version <= 3 then byteConst 0x1fuy else byteConst 0x3fuy
        let comp = if memory.Version <= 3 then byteConst 0xe0uy else byteConst 0xc0uy

        builder.LoopWhile (stop.IsFalse)
            (fun () ->
                firstByte <-- x.ReadByte !!propAddress
                builder.IfThenElse ((!!firstByte .&. mask) .<=. propNum)
                    (fun () ->
                        stop <-- true
                    )
                    (fun () ->
                        propAddress <-- getNextPropertyAddress !!propAddress
                    )
            )

        builder.IfThen ((!!firstByte .&. mask) .<>. propNum)
            (fun () ->
                x.RuntimeException("Property not found!")
            )

        propAddress <-- (!!propAddress .+. one)

        builder.IfThenElse ((!!firstByte .&. comp) .=. zero)
            (fun () ->
                x.WriteByte !!propAddress (value |> toByte)
            )
            (fun () ->
                x.WriteWord !!propAddress value
            )

    member x.Remove objNum =
        let leftSibling = builder.InitTemp zero
        let rightSibling = x.ReadSibling objNum
        let parent = x.ReadParent objNum
        let parentChild = builder.InitTemp zero

        builder.IfThenElse (parent .=. zero)
            (fun () ->
                parentChild <-- zero
            )
            (fun () ->
                parentChild <-- (x.ReadChild parent)
            )

        builder.IfThen (!!parentChild .<>. objNum)
            (fun () ->
                let next = builder.InitTemp !!parentChild
                let sibling = builder.InitTemp zero

                builder.LoopWhile (!!next .<>. zero)
                    (fun () ->
                        sibling <-- x.ReadSibling !!next
                        builder.IfThenElse (!!sibling .=. objNum)
                            (fun () ->
                                leftSibling <-- !!next
                                next <-- zero
                            )
                            (fun () ->
                                next <-- !!sibling
                            )
                    )
            )

        builder.IfThen (!!leftSibling .<>. zero)
            (fun () ->
                x.WriteSibling !!leftSibling rightSibling
            )

        builder.IfThen (!!parentChild .=. objNum)
            (fun () ->
                x.WriteChild parent rightSibling
            )

        x.WriteParent objNum zero
        x.WriteSibling objNum zero

    member x.Insert objNum destObjNum =
        x.Remove objNum

        builder.IfThen (destObjNum .<>. zero)
            (fun () ->
                x.WriteParent objNum destObjNum
                x.WriteSibling objNum (x.ReadChild destObjNum)
                x.WriteChild destObjNum objNum
            )

type ScreenBinder(memory, builder, debugging) =
    inherit Binder(memory, builder, debugging)

    member x.SelectOutputStream number =
        SelectOutputStreamStmt(number) |> builder.AddStatement
    member x.SelectMemoryOutputStream number table =
        SelectMemoryOutputStreamStmt(number, table) |> builder.AddStatement

    member x.PrintChar ch =
        PrintCharStmt(ch) |> builder.AddStatement
    member x.PrintText text =
        PrintTextStmt(text) |> builder.AddStatement

    member x.ClearWindow window =
        ClearWindowStmt(window) |> builder.AddStatement
    member x.SetWindow window =
        SetWindowStmt(window) |> builder.AddStatement
    member x.SplitWindow lines =
        SplitWindowStmt(lines) |> builder.AddStatement

    member x.SetCursor line column =
        SetCursorStmt(line, column) |> builder.AddStatement
    member x.GetCursorColumn() =
        GetCursorColumnExpr
    member x.GetCursorLine() =
        GetCursorLineExpr

    member x.SetColor foreground background =
        SetColorsStmt(foreground, background) |> builder.AddStatement
    member x.SetTextStyle style =
        SetTextStyleStmt(style) |> builder.AddStatement

    member x.ShowStatus() =
        ShowStatusStmt |> builder.AddStatement

type InstructionBinder(routine: Routine, memory, builder, debugging) as this =
    inherit Binder(memory, builder, debugging)

    let objects = new ObjectBinder(memory, builder, debugging)
    let screen = new ScreenBinder(memory, builder, debugging)

    let addStatement s =
        builder.AddStatement(s)

    let ret expression =
        builder.Return(expression)

    let initTemp expression =
        builder.InitTemp expression

    let initBoolTemp value =
        builder.InitBoolTemp value

    let ifThenElse condition whenTrue whenFalse =
        builder.IfThenElse condition whenTrue whenFalse

    let ifThen condition whenTrue =
        builder.IfThen condition whenTrue

    let loopWhile condition whileFalse =
        builder.LoopWhile condition whileFalse

    let packedMultiplier =
        match memory.Version with
        | 1 | 2 | 3 -> 2
        | 4 | 5 | 6 | 7 -> 4
        | 8 -> 8
        | v -> failcompilef "Invalid version number: %d" v

    let routinesOffset =
        memory |> Header.readRoutinesOffset |> int

    let stringsOffset =
        memory |> Header.readStringOffset |> int

    let unpackAddress address offset =
        match address with
        | ConstantExpr(Int32Value address) ->
            let baseAddress = address * packedMultiplier
            if memory.Version = 6 || memory.Version = 7 then
                int32Const (baseAddress + offset)
            else
                int32Const baseAddress
        | _ ->
            let baseAddress = address .*. (int32Const packedMultiplier)
            if memory.Version = 6 || memory.Version = 7 then
                baseAddress .+. (int32Const offset)
            else
                baseAddress

    let unpackRoutineAddress address =
        unpackAddress address routinesOffset

    let unpackStringAddress address =
        unpackAddress address stringsOffset

    let call address args processResult =
        match address with
        | ConstantExpr(Int32Value _) ->
            let unpackedAddress = unpackRoutineAddress address
            processResult (CallExpr(unpackedAddress, args))
        | _ ->
            builder.IfThenElse (address .=. zero)
                (fun () ->
                    processResult zero
                )
                (fun () ->
                    let unpackedAddress = unpackRoutineAddress address
                    processResult (CallExpr(unpackedAddress, args))
                )

    let readStack, writeStack =
        let read indirect =
            if indirect then StackPeekExpr
            else StackPopExpr

        let write indirect v =
            if indirect then StackUpdateStmt(v) |> addStatement
            else StackPushStmt(v) |> addStatement

        read,write

    let peekStack = readStack true
    let popStack = readStack false
    let pushStack = writeStack false
    let updateStack = writeStack true

    let readLocal, writeLocal =
        // Determine whether this routine contains any instructions that use computed
        // variables. If not, we can optimize by using temps for locals.
        let usesComputedVariables =
            routine.Instructions
            |> List.exists (fun i ->
                if i.Opcode.IsFirstOpByRef then
                    match i.Operands.Head with
                    | VariableOperand(_) -> true
                    | _ -> false
                else
                    false)

        if usesComputedVariables then
            let read i = ReadLocalExpr(i)
            let write i e = WriteLocalStmt(i, e) |> addStatement
            read, write
        else
            // Create a label for the block of local temps
            let label = builder.NewLabel()
            builder.MarkLabel(label)

            let localTemps = Array.zeroCreate routine.Locals.Length
            for i = 0 to localTemps.Length - 1 do
                let value = ReadLocalExpr(byteConst (byte i))
                localTemps.[i] <- initTemp value

            let read i =
                match i with
                | ByteConst(b) -> !!localTemps.[int b]
                | _ -> failcompile "Expected byte"
            let write i v =
                match i with
                | ByteConst(b) -> localTemps.[int b] <-- v
                | _ -> failcompile "Expected byte"

            read, write

    let readGlobal, writeGlobal =
        let globalVariableTableAddress = memory |> Header.readGlobalVariableTableAddress |> int

        let computeAddress i =
            match i with
            | ConstantExpr(Int32Value i) ->
                int32Const ((i * 2) + globalVariableTableAddress)
            | _ ->
                (i .*. two) .+. (int32Const globalVariableTableAddress)

        let read i = this.ReadWord (computeAddress i)
        let write i e = this.WriteWord (computeAddress i) e
        read, write

    let readVariable variable =
        match variable with
        | StackVariable -> popStack
        | LocalVariable(i) -> readLocal (byteConst i)
        | GlobalVariable(i) -> readGlobal (byteConst i)

    let writeVariable value variable =
        match variable with
        | StackVariable -> pushStack value
        | LocalVariable(i) -> writeLocal (byteConst i) value
        | GlobalVariable(i) -> writeGlobal (byteConst i) value

    let bindOperand = function
        | LargeConstantOperand(v) -> wordConst v
        | SmallConstantOperand(v) -> byteConst v
        | VariableOperand(v)      -> readVariable v

    member x.BindInstruction(instruction: Instruction) =

        let branchIf expression =
            let branch =
                match instruction.Branch with
                | Some(b) -> b
                | None -> failcompile "Expected instruction to have a valid branch."

            let statement =
                match branch with
                | RTrueBranch(_) -> ReturnStmt(one)
                | RFalseBranch(_) -> ReturnStmt(zero)
                | OffsetBranch(_,_) -> JumpStmt(builder.GetJumpTargetLabel(instruction.BranchAddress.Value))

            BranchStmt(branch.Condition, expression, statement)
                |> addStatement

        let store expression =
            let storeVar =
                match instruction.StoreVariable with
                | Some(v) -> v
                | None -> failcompile "Expected instruction to have a valid store variable."

            writeVariable expression storeVar

        let discard expression =
            match instruction.StoreVariable with
            | Some(_) -> failcompile "Expected instruction to not have a store variable."
            | None -> ()

            DiscardValueStmt(expression)
                |> addStatement

        let copyTable first second size =
            ifThenElse (second .=. zero)
                (fun () ->
                    // if second is zero, zero out first table

                    let i = initTemp zero

                    loopWhile (!!i .<. size)
                        (fun () ->
                            this.WriteByte (first .+. !!i) zero

                            i.Increment()
                        )
                )
                (fun () ->
                    ifThenElse (((size |> toInt16) .<. zero) .|. first .>. second)
                        (fun () ->
                            // copy forwards

                            let sizeCopy = initTemp size
                            ifThen ((!!sizeCopy |> toInt16) .<. zero)
                                (fun () ->
                                    sizeCopy <-- toUInt16 (negate (size |> toInt16))
                                )

                            let i = initTemp zero

                            loopWhile (!!i .<. !!sizeCopy)
                                (fun () ->
                                    let value = this.ReadByte (first .+. !!i)
                                    this.WriteByte (second .+. !!i) value

                                    i.Increment()
                                )
                        )
                        (fun () ->
                            // copy backwards
                            let i = initTemp (size .-. one)

                            loopWhile (!!i .>=. zero)
                                (fun () ->
                                    let value = this.ReadByte (first .+. !!i)
                                    this.WriteByte (second .+. !!i) value
                                )

                            i.Decrement()
                        )
                )

        let printTable table width height skip =
            let address = initTemp table
            let left = initTemp (screen.GetCursorColumn())
            let i = initTemp zero

            loopWhile (!!i .<. height)
                (fun () ->
                    ifThen (!!i .<>. zero)
                        (fun () ->
                            let y = screen.GetCursorLine() .+. one
                            screen.SetCursor y !!left
                        )

                    let j = initTemp zero

                    loopWhile (!!j .<. width)
                        (fun () ->
                            let ch = initTemp (this.ReadByte !!address)
                            address.Increment()
                            screen.PrintChar !!ch

                            j.Increment()
                        )

                    address <-- !!address .+. skip

                    i.Increment()
                )

        let scanTable x table len form =
            let address = initTemp table
            let i = initTemp zero
            let stopLoop = initBoolTemp false
            let finished = initBoolTemp false

            loopWhile (stopLoop.IsFalse)
                (fun () ->
                    ifThenElse ((form .&. (byteConst 0x80uy)) .<>. zero)
                        (fun () ->
                            ifThen ((this.ReadWord !!address) .=. x)
                                (fun () ->
                                    stopLoop <-- true
                                    finished <-- true
                                    store !!address
                                    branchIf one
                                )
                        )
                        (fun () ->
                            ifThen ((this.ReadByte !!address) .=. x)
                                (fun () ->
                                    stopLoop <-- true
                                    finished <-- true
                                    store !!address
                                    branchIf one
                                )
                        )

                    address <-- !!address .+. (form .&. (byteConst 0x7fuy))

                    i.Increment()

                    ifThen (!!i .=. len)
                        (fun () ->
                            stopLoop <-- true
                        )
                )

            ifThen (finished.IsFalse)
                (fun () ->
                    store zero
                    branchIf zero
                )

        // If this instruction is a jump target, mark its label
        if builder.IsJumpTarget(instruction.Address) then
            let label = builder.GetJumpTargetLabel(instruction.Address)
            builder.MarkLabel(label)

        // Create temps for all operands except constants
        let operandValues =
            instruction.Operands
            |> List.map bindOperand

        let operands =
            operandValues
            |> List.map (fun v ->
                match v with
                | ConstantExpr(_) -> v
                | _ -> !!(initTemp v))

        let operandMap = List.zip operands operandValues |> Map.ofList

        // If debugging, write the instruction to the debug output
        if debugging then
            let builder = StringBuilder.create()
            builder |> StringBuilder.appendString (sprintf "%04x: %s" instruction.Address instruction.Opcode.Name)

            let index = ref 0

            let formatItem (specifier: string) =
                let start = "{" + (!index).ToString()
                let res = if specifier.Length > 0 then start + ":" + specifier + "}"
                          else start + "}"
                incr index
                res

            instruction.Operands
                |> List.iter (fun op -> 
                    match op with
                    | LargeConstantOperand(_) -> builder |> StringBuilder.appendString (" " + formatItem "x4")
                    | SmallConstantOperand(_) -> builder |> StringBuilder.appendString (" " + formatItem "x2")
                    | VariableOperand(v) ->
                        match v with
                        | StackVariable ->
                            builder |> StringBuilder.appendString (" SP=" + formatItem "x")
                        | LocalVariable(i) ->
                            builder |> StringBuilder.appendString ((sprintf " L%02x=" i) + formatItem "x")
                        | GlobalVariable(i) ->
                            builder |> StringBuilder.appendString ((sprintf " G%02x=" i) + formatItem "x"))

            this.DebugOut (builder.ToString()) operands

        let byRefVariable variableIndex =

            let fromByte value =
                if value = 0uy then
                    let read() = (initTemp peekStack).Read
                    let write v = updateStack v
                    read, write
                elif value < 16uy then
                    let i = byteConst (value - 1uy)
                    let read() = readLocal i
                    let write v = writeLocal i v
                    read, write
                else
                    let i = byteConst (value - 16uy)
                    let read() = readGlobal i
                    let write v = writeGlobal i v
                    read, write

            let fromExpression expression =
                let read() =
                    let index = initTemp expression
                    let value = initTemp zero

                    ifThenElse (!!index .=. zero)
                        (fun () ->
                            value <-- peekStack
                        )
                        (fun () ->
                            ifThenElse (!!index .<. sixteen)
                                (fun () ->
                                    let i = !!index .-. one
                                    value <-- readLocal i
                                )
                                (fun () ->
                                    let i = !!index .-. sixteen
                                    value <-- readGlobal i
                                )
                        )

                    !!value

                let write v =
                    let index = initTemp expression

                    ifThenElse (!!index .=. zero)
                        (fun () ->
                            updateStack v
                        )
                        (fun () ->
                            ifThenElse (!!index .<. sixteen)
                                (fun () ->
                                    let i = !!index .-. one
                                    writeLocal i v
                                )
                                (fun () ->
                                    let i = !!index .-. sixteen
                                    writeGlobal i v
                                )
                        )

                read, write

            let fromOperandTemp temp =
                match operandMap |> Map.tryFind temp with
                | Some(ByteConst b) ->
                    fromByte b
                | Some(e) ->
                    fromExpression temp
                | _ ->
                    failcompile "Expected operand temp for by-ref variable index"

            match variableIndex with
            | ByteConst b ->
                fromByte b
            | TempExpr(_) as t ->
                fromOperandTemp t
            | e ->
                fromExpression e

        let (|ByRef|_|) = function
            | op ->
                let read,write = byRefVariable op
                Some(ByRefOperand(read, write))

        // Bind the instruction
        match (instruction.Opcode.Name, int instruction.Opcode.Version, operands) with
        | "add", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left .+. right)

        | "and", Any, Op2(left, right) ->
            store (left .&. right)

        | "aread", AtLeast 5, Op1(textBuffer) ->
            store (ReadInputTextExpr(textBuffer, zero))

        | "aread", AtLeast 5, Op2(textBuffer, parseBuffer) ->
            store (ReadInputTextExpr(textBuffer, parseBuffer))

        | "art_shift", AtLeast 5, Op2(number, places) ->
            let number = number |> toInt16
            let places = places |> toInt16

            ifThenElse (places .>. zero)
                (fun () ->
                    store (number .<<. (places .&. (int32Const 0x1f)))
                )
                (fun () ->
                    store (number .>>. ((negate places) .&. (int32Const 0x1f)))
                )

        | "buffer_mode", AtLeast 4, Op1(flag) ->
            // TODO: Do we need to do anything with this -- we always buffer!
            ()

        | "call", Any, OpAndList(address, args)
        | "call_1s", AtLeast 4, OpAndList(address, args)
        | "call_2s", AtLeast 4, OpAndList(address, args)
        | "call_vs", Any, OpAndList(address, args)
        | "call_vs2", AtLeast 4, OpAndList(address, args) ->

            call address args store

        | "call_1n", AtLeast 5, OpAndList(address, args)
        | "call_2n", AtLeast 5, OpAndList(address, args)
        | "call_vn", AtLeast 5, OpAndList(address, args)
        | "call_vn2", AtLeast 4, OpAndList(address, args) ->

            call address args discard

        | "check_arg_count", AtLeast 5, Op1(number) ->
            branchIf (number .<=. ArgCountExpr)

        | "clear_attr", Any, Op2(objNum, attrNum) ->
            objects.WriteAttribute objNum attrNum false

        | "copy_table", AtLeast 5, Op3(first, second, size) ->
            copyTable first second size

        | "dec", Any, Op1(ByRef(variable)) ->
            let read = !!variable |> toInt16
            variable <-- (read .-. (one |> toInt16))

        | "dec_chk", Any, Op2(ByRef(variable), value) ->
            let read = !!variable |> toInt16
            let newValue = initTemp (read .-. (one |> toInt16))
            variable <-- !!newValue
            branchIf (!!newValue |> toInt16 .<. (value |> toInt16))

        | "div", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left ./. right)

        | "erase_window", AtLeast 4, Op1(window) ->
            screen.ClearWindow window

        | "get_child", Any, Op1(objNum) ->
            let child = objects.ReadChild objNum
            store child
            branchIf (child .<>. zero)

        | "get_next_prop", Any, Op2(objNum, propNum) ->
            let result = objects.GetNextPropertyNumber objNum propNum
            store result

        | "get_parent", Any, Op1(objNum) ->
            let parent = objects.ReadParent objNum
            store parent

        | "get_prop", Any, Op2(objNum, propNum) ->
            let result = objects.ReadPropertyValue objNum propNum
            store result

        | "get_prop_addr", Any, Op2(objNum, propNum) ->
            let result = objects.GetPropertyAddress objNum propNum
            store result

        | "get_prop_len", Any, Op1(dataAddress) ->
            let result = objects.GetPropertyLength dataAddress
            store result

        | "get_sibling", Any, Op1(objNum) ->
            let sibling = objects.ReadSibling objNum
            store sibling
            branchIf (sibling .<>. zero)

        | "inc", Any, Op1(ByRef(variable)) ->
            let read = !!variable |> toInt16
            variable <-- (read .+. (one |> toInt16))

        | "inc_chk", Any, Op2(ByRef(variable), value) ->
            let read = !!variable |> toInt16
            let newValue = initTemp (read .+. (one |> toInt16))
            variable <-- !!newValue
            branchIf (!!newValue |> toInt16 .>. (value |> toInt16))

        | "insert_obj", Any, Op2(objNum, destObjNum) ->
            objects.Insert objNum destObjNum

        | "je", Any, OpAndList(left, values) ->
            // je can have 2 to 4 operands to test for equality.
            let conditions = values |> List.map (fun v -> left .=. v)

            branchIf
                ((List.tail conditions)
                |> List.fold
                    (fun res c -> res .|. c)
                    (List.head conditions))

        | "jg", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            branchIf (left .>. right)

        | "jin", Any, Op2(objNum1, objNum2) ->
            let obj1Parent = objects.ReadParent objNum1

            branchIf (obj1Parent .=. objNum2)

        | "jl", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            branchIf (left .<. right)

        | "jump", Any, Op1(_) ->
            let label = builder.GetJumpTargetLabel(instruction.JumpAddress)
            builder.JumpTo(label)

        | "jz", Any, Op1(left) ->
            branchIf (left .=. zero)

        | "load", Any, Op1(ByRef(variable)) ->
            let value = !!variable
            store value

        | "loadb", Any, Op2(address,offset) ->
            let address =
                match address, offset with
                | ConstantExpr(Int32Value address), ConstantExpr(Int32Value offset) ->
                    int32Const (address + offset)
                | _ ->
                    address .+. offset

            store (this.ReadByte address)

        | "loadw", Any, Op2(address,offset) ->
            let address =
                match address, offset with
                | ConstantExpr(Int32Value address), ConstantExpr(Int32Value offset) ->
                    int32Const (address + (offset * 2))
                | _ ->
                    address .+. (offset .*. two)

            store (this.ReadWord address)

        | "log_shift", AtLeast 5, Op2(number, places) ->
            let places = places |> toInt16

            ifThenElse (places .>. zero)
                (fun () ->
                    store (number .<<. (places .&. (int32Const 0x1f)))
                )
                (fun () ->
                    store (number .>>. ((negate places) .&. (int32Const 0x1f)))
                )

        | "mod", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left .%. right)

        | "mul", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left .*. right)

        | "new_line", Any, NoOps ->
            screen.PrintText (textConst "\n")

        | "not", AtLeast 5, Op1(value) ->
            let result = UnaryOperationKind.Not |> unaryOp value
            store result

        | "or", Any, Op2(left, right) ->
            store (left .|. right)

        | "output_stream", AtLeast 3, Op1(number) ->
            screen.SelectOutputStream number

        | "output_stream", AtLeast 4, Op2(number, table) ->
            screen.SelectMemoryOutputStream number table

        | "piracy", AtLeast 5, NoOps ->
            branchIf (one)

        | "print", Any, NoOps ->
            let text = textConst instruction.Text.Value
            screen.PrintText text

        | "print_addr", Any, Op1(address) ->
            let text = readText address
            screen.PrintText text

        | "print_char", Any, Op1(ch) ->
            screen.PrintChar ch

        | "print_num", Any, Op1(number) ->
            let text = numberToText(number |> toInt16)
            screen.PrintText text

        | "print_obj", Any, Op1(objNum) ->
            let text = objects.ReadShortName objNum
            screen.PrintText text

        | "print_paddr", Any, Op1(address) ->
            let text = readText (unpackStringAddress address)
            screen.PrintText text

        | "print_ret", Any, NoOps ->
            let text = textConst instruction.Text.Value
            screen.PrintText text
            ret one

        | "print_table", AtLeast 5, Op2(table, width) ->
            printTable table width one zero

        | "print_table", AtLeast 5, Op3(table, width, height) ->
            printTable table width height zero

        | "print_table", AtLeast 5, Op4(table, width, height, skip) ->
            printTable table width height skip

        | "pull", Any, Op1(ByRef(variable)) ->
            let value = initTemp popStack
            variable <-- !!value

        | "put_prop", Any, Op3(objNum, propNum, value) ->
            objects.WritePropertyValue objNum propNum value

        | "push", Any, Op1(value) ->
            pushStack value

        | "quit", Any, NoOps ->
            QuitStmt |> addStatement

        | "random", Any, Op1(range) ->
            let range = range |> toInt16

            ifThenElse (range .>. zero)
                (fun () ->
                    store (random range)
                )
                (fun () ->
                    randomize range |> addStatement
                    store zero
                )

        | "read_char", AtLeast 4, NoOps ->
            store (ReadInputCharExpr)

        | "read_char", AtLeast 4, Op1(input) ->
            store (ReadInputCharExpr)

        | "read_char", AtLeast 4, Op3(input, time, routine) ->
            store (ReadTimedInputCharExpr(time, routine))

        | "remove_obj", Any, Op1(objNum) ->
            objects.Remove objNum

        | "ret", Any, Op1(value) ->
            ret value

        | "ret_popped", Any, NoOps ->
            ret popStack

        | "rfalse", Any, NoOps ->
            ret zero

        | "rtrue", Any, NoOps ->
            ret one

        | "save_undo", AtLeast 5, NoOps ->
            // TODO: Support undo
            ()

        | "scan_table", AtLeast 4, Op3(x, table, len) ->
            scanTable x table len (byteConst 0x82uy)

        | "scan_table", AtLeast 4, Op4(x, table, len, form) ->
            scanTable x table len form

        | "set_attr", Any, Op2(objNum, attrNum) ->
            objects.WriteAttribute objNum attrNum true

        | "set_color", Is 6, Op3(foreground, background, window) ->
            failcompile "set_color not implemented for version 6"

        | "set_color", AtLeast 5, Op2(foreground, background) ->
            screen.SetColor foreground background

        | "set_cursor", Is 6, Op3(line, column, window) ->
            failcompile "set_cursor not implemented for version 6"

        | "set_cursor", AtLeast 4, Op2(line, column) ->
            screen.SetCursor line column

        | "set_text_style", AtLeast 4, Op1(style) ->
            screen.SetTextStyle style

        | "set_window", AtLeast 3, Op1(window) ->
            screen.SetWindow window

        | "show_status", AtLeast 3, NoOps ->
            screen.ShowStatus()

        | "split_window", AtLeast 3, Op1(lines) ->
            screen.SplitWindow lines

        | "sread", AtMost 3, Op2(textBuffer, parseBuffer) ->
            discard (ReadInputTextExpr(textBuffer, parseBuffer))

        | "sread", Is 4, Op2(textBuffer, parseBuffer) ->
            discard (ReadInputTextExpr(textBuffer, parseBuffer))

        | "store", Any, Op2(ByRef(variable), value) ->
            variable <-- value

        | "storeb", Any, Op3(address, offset, value) ->
            let address =
                match address, offset with
                | ConstantExpr(Int32Value address), ConstantExpr(Int32Value offset) ->
                    int32Const (address + offset)
                | _ ->
                    address .+. offset

            this.WriteByte address value

        | "storew", Any, Op3(address, offset, value) ->
            let address =
                match address, offset with
                | ConstantExpr(Int32Value address), ConstantExpr(Int32Value offset) ->
                    int32Const (address + (offset * 2))
                | _ ->
                    address .+. (offset .*. two)

            this.WriteWord address value

        | "sub", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left .-. right)

        | "test", Any, Op2(bitmap, flags) ->
            branchIf ((bitmap .&. flags) .=. flags)

        | "test_attr", Any, Op2(objNum, attrNum) ->
            let attributeValue = objects.ReadAttribute objNum attrNum

            branchIf (attributeValue .=. one)

        | "tokenize", AtLeast 5, Op2(textBuffer, parseBuffer) ->
            TokenizeStmt(textBuffer, parseBuffer, zero, zero) |> addStatement

        | "tokenize", AtLeast 5, Op3(textBuffer, parseBuffer, dictionaryAddress) ->
            TokenizeStmt(textBuffer, parseBuffer, dictionaryAddress, zero) |> addStatement

        | "tokenize", AtLeast 5, Op4(textBuffer, parseBuffer, dictionaryAddress, ignoreUnrecognizedWords) ->
            TokenizeStmt(textBuffer, parseBuffer, dictionaryAddress, ignoreUnrecognizedWords) |> addStatement

        | "verify", AtLeast 3, NoOps ->
            branchIf VerifyExpr

        | (n,k,ops) ->
            x.RuntimeException (sprintf "Unsupported opcode: %s (v.%d) with %d operands" n k ops.Length)

type RoutineBinder(memory: Memory, debugging: bool) =

    member x.BindRoutine(routine: Routine) =

        let builder = new BoundTreeCreator(routine)
        let binder = new InstructionBinder(routine, memory, builder, debugging)

        for i in routine.Instructions do
            binder.BindInstruction(i)

        builder.GetTree()
            |> Optimization.cleanupLabels
            |> Optimization.cleanupTemps
