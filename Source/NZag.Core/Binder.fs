namespace NZag.Core

open NZag.Utilities
open BoundNodeConstruction
open BoundNodeVisitors
open OperandPatterns

type InstructionBinder(memory: Memory, routine: Routine, builder: BoundTreeCreator, debugging: bool) =

    let packedMultiplier =
        match memory.Version with
        | 1 | 2 | 3 -> two
        | 4 | 5 | 6 | 7 -> four
        | 8 -> eight
        | v -> failcompilef "Invalid version number: %d" v

    let routinesOffset =
        int32Const (memory |> Header.readRoutinesOffset).IntValue

    let stringsOffset =
        int32Const (memory |> Header.readStringOffset).IntValue

    let unpackRoutineAddress address =
        let baseAddress = address .*. packedMultiplier
        if memory.Version = 6 || memory.Version = 7 then
            baseAddress .+. routinesOffset
        else
            baseAddress

    let unpackStringAddress address =
        let baseAddress = address .*. packedMultiplier
        if memory.Version = 6 || memory.Version = 7 then
            baseAddress .+. stringsOffset
        else
            baseAddress

    let addStatement s =
        builder.AddStatement(s)

    let ret expression =
        builder.Return(expression)

    let getTempIndex t =
        match t with
        | TempExpr(t) -> t
        | _ -> failcompile "Expected temp"

    let assignTemp t v =
        builder.AssignTemp t v

    let initTemp expression =
        builder.InitTemp expression

    let initMutableTemp expression =
        builder.InitMutableTemp expression

    let ifThenElse condition whenTrue whenFalse =
        builder.IfThenElse condition whenTrue whenFalse

    let ifThen condition whenTrue =
        builder.IfThen condition whenTrue

    let loopWhile condition whileFalse =
        builder.LoopWhile condition whileFalse

    let call address args processResult =
        ifThenElse (address .=. zero)
            (fun () ->
                processResult zero)
            (fun () ->
                let unpackedAddress = unpackRoutineAddress address
                processResult (CallExpr(unpackedAddress, args)))

    let objectTableAddress = (memory |> Header.readObjectTableAddress |> (fun a -> a.IntValue)) |> int32Const
    let propertyDefaultsSize = (if memory.Version <= 3 then 31 else 63) |> int32Const
    let objectEntriesAddress = objectTableAddress .+. (propertyDefaultsSize .*. two)
    let objectEntrySize = (if memory.Version <= 3 then 9 else 14) |> int32Const
    let objectParentOffset = (if memory.Version <= 3 then 4 else 6) |> int32Const
    let objectSiblingOffset = (if memory.Version <= 3 then 5 else 8) |> int32Const
    let objectChildOffset = (if memory.Version <= 3 then 6 else 10) |> int32Const
    let objectPropertyTableOffset = (if memory.Version <= 3 then 7 else 12) |> int32Const

    let readObjectNumber address =
        if memory.Version <= 3 then readByte address
        else readWord address

    let writeObjectNumber address value =
        if memory.Version <= 3 then writeByte address value
        else writeWord address value

    let computeObjectAddress objNum =
        ((objNum .-. one) .*. objectEntrySize) .+. objectEntriesAddress

    let readObjectChild objNum =
        let objAddress = computeObjectAddress objNum
        let childAddress = objAddress .+. objectChildOffset
        childAddress |> readObjectNumber

    let writeObjectChild objNum value =
        let objAddress = computeObjectAddress objNum
        let childAddress = objAddress .+. objectChildOffset
        writeObjectNumber childAddress value

    let readObjectParent objNum =
        let objAddress = computeObjectAddress objNum
        let parentAddress = objAddress .+. objectParentOffset
        parentAddress |> readObjectNumber

    let writeObjectParent objNum value =
        let objAddress = computeObjectAddress objNum
        let parentAddress = objAddress .+. objectParentOffset
        writeObjectNumber parentAddress value

    let readObjectSibling objNum =
        let objAddress = computeObjectAddress objNum
        let siblingAddress = objAddress .+. objectSiblingOffset
        siblingAddress |> readObjectNumber

    let writeObjectSibling objNum value =
        let objAddress = computeObjectAddress objNum
        let siblingAddress = objAddress .+. objectSiblingOffset
        writeObjectNumber siblingAddress value

    let computeAttributeByteAddress objNum attrNum =
        let objAddress = computeObjectAddress objNum
        objAddress .+. (attrNum ./. eight)

    let computeAttributeBitMask attrNum =
        one .<<. ((seven .-. (attrNum .%. eight)) .&. int32Const 0x1f)

    let readObjectAttribute objNum attrNum =
        let bitMask = computeAttributeBitMask attrNum
        let attrByteAddress = computeAttributeByteAddress objNum attrNum
        let attrByte = readByte attrByteAddress
        (attrByte .&. bitMask) .<>. zero

    let writeObjectAttribute objNum attrNum value =
        let bitMask = computeAttributeBitMask attrNum
        let attrByteAddress = initTemp (computeAttributeByteAddress objNum attrNum)
        let attrByte = readByte attrByteAddress
        let newAttrByte =
            if value then
                (attrByte .|. bitMask) |> toByte
            else
                (attrByte .&. (bitNot bitMask)) |> toByte

        writeByte attrByteAddress newAttrByte |> addStatement

    let readObjectName objNum =
        let objAddress = computeObjectAddress objNum
        let propTableAddress = objAddress .+. objectPropertyTableOffset
        let propsAddress = initTemp (readWord propTableAddress)
        let nameLength = readByte propsAddress
        let nameAddress = propsAddress .+. one
        readTextOfLength nameAddress nameLength

    let readObjectFirstPropertyAddress objNum =
        let objAddress = computeObjectAddress objNum
        let propTableAddress = objAddress .+. objectPropertyTableOffset
        let propsAddress = initTemp (readWord propTableAddress)

        // First property is address after object name
        let nameLength = readByte propsAddress
        let result = (propsAddress .+. one) .+. (nameLength .*. two)

        result |> toUInt16

    let readObjectNextPropertyAddress propAddress =
        let propSizeByte = initTemp (readByte propAddress)

        let propSize = 
            if memory.Version <= 3 then
                propSizeByte .>>. five
            else
                let read,write = initMutableTemp zero
                ifThenElse ((propSizeByte .&. (int32Const 0x80)) .<>. (int32Const 0x80))
                    (fun () ->
                        write (propSizeByte .>>. six))
                    (fun () -> 
                        let nextByteAddress = propAddress .+. one
                        let nextPropSizeByte = readByte nextByteAddress
                        let nextPropSizeByte' = nextPropSizeByte .&. (int32Const 0x3f)
                        ifThenElse (nextPropSizeByte' .=. zero)
                            (fun () ->
                                write (int32Const 64))
                            (fun () -> 
                                write nextPropSizeByte'))
                read

        let result = (propAddress .+. one) .+. (propSize .+. one)

        result |> toUInt16

    let readObjectPropertyDefault propNum =
        let result = readWord (objectTableAddress .+. ((propNum .-. one) .*. two))
        result |> toUInt16

    let removeObject objNum =
        let leftSiblingRead, leftSiblingWrite = initMutableTemp zero
        let rightSibling = readObjectSibling objNum
        let parent = readObjectParent objNum
        let parentChildRead, parentChildWrite = initMutableTemp zero

        ifThenElse (parent .=. zero)
            (fun () ->
                parentChildWrite zero)
            (fun () ->
                parentChildWrite (readObjectChild parent))

        ifThen (parentChildRead .<>. objNum)
            (fun () ->
                let nextTempRead, nextTempWrite = initMutableTemp parentChildRead
                let siblingTempRead, siblingTempWrite = initMutableTemp zero

                loopWhile (nextTempRead .<>. zero)
                    (fun () ->
                        siblingTempWrite (readObjectSibling nextTempRead)
                        ifThenElse (siblingTempRead .=. objNum)
                            (fun () ->
                                leftSiblingWrite nextTempRead
                                nextTempWrite zero)
                            (fun () ->
                                nextTempWrite siblingTempRead)))

        ifThen (leftSiblingRead .<>. zero)
            (fun () ->
                writeObjectSibling leftSiblingRead rightSibling |> addStatement)

        ifThen (parentChildRead .=. objNum)
            (fun () ->
                writeObjectChild parent rightSibling |> addStatement)

        writeObjectParent objNum zero |> addStatement
        writeObjectSibling objNum zero |> addStatement

    let insertObject objNum destObjNum =
        removeObject objNum

        ifThen (destObjNum .<>. zero)
            (fun () ->
                writeObjectParent objNum destObjNum |> addStatement
                writeObjectSibling objNum (readObjectChild destObjNum) |> addStatement
                writeObjectChild destObjNum objNum |> addStatement)

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
            let read i = ReadLocalExpr(byteConst i)
            let write i e = WriteLocalStmt(byteConst i, e) |> addStatement
            read, write
        else
            // Create a label for the block of local temps
            let label = builder.NewLabel()
            builder.MarkLabel(label)

            let localTemps = Array.zeroCreate routine.Locals.Length
            for i = 0 to localTemps.Length - 1 do
                let value = ReadLocalExpr(byteConst (byte i))
                localTemps.[i] <- initMutableTemp value

            let read i = fst localTemps.[int i]
            let write i = snd (localTemps.[int i])
            read, write

    let readVar v =
        match v with
        | StackVariable -> StackPopExpr
        | LocalVariable(i) -> readLocal i
        | GlobalVariable(i) -> ReadGlobalExpr(byteConst i)

    let writeVar e v =
        match v with
        | StackVariable -> StackPushStmt(e) |> addStatement
        | LocalVariable(i) -> writeLocal i e
        | GlobalVariable(i) -> WriteGlobalStmt(byteConst i, e) |> addStatement

    let bindOperand = function
        | LargeConstantOperand(v) -> wordConst v
        | SmallConstantOperand(v) -> byteConst v
        | VariableOperand(v)      -> readVar v

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
                |> builder.AddStatement

        let store expression =
            let storeVar =
                match instruction.StoreVariable with
                | Some(v) -> v
                | None -> failcompile "Expected instruction to have a valid store variable."

            storeVar |> writeVar expression

        let discard expression =
            match instruction.StoreVariable with
            | Some(_) -> failcompile "Expected instruction to not have a store variable."
            | None -> ()

            discardValue expression |> addStatement

        let scanTable x table len form =
            let address, writeAddress = initMutableTemp table
            let index, writeIndex = initMutableTemp zero
            let stopLoop, writeStopLoop = initMutableTemp zero
            let finished, writeFinished = initMutableTemp zero

            loopWhile (stopLoop .=. zero)
                (fun () ->
                    ifThenElse ((form .&. (byteConst 0x80uy)) .<>. zero)
                        (fun () ->
                            ifThen ((readWord address) .=. x)
                                (fun () ->
                                    writeStopLoop one
                                    writeFinished one
                                    store address
                                    branchIf one
                                )
                        )
                        (fun () ->
                            ifThen ((readByte address) .=. x)
                                (fun () ->
                                    writeStopLoop one
                                    writeFinished one
                                    store address
                                    branchIf one
                                )
                        )

                    writeAddress (address .+. (form .&. (byteConst 0x7fuy)))

                    writeIndex (index .+. one)

                    ifThen (index .=. len)
                        (fun () ->
                            writeStopLoop one
                        )
                )

            ifThen (finished .=. zero)
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
                | _ -> initTemp v)

        let operandMap = List.zip operands operandValues |> Map.ofList

        // If debugging, write the instruction to the debug output
        if debugging then
            let builder = StringBuilder.create()
            builder |> StringBuilder.appendString (sprintf "%04x: %s" (instruction.Address.IntValue) (instruction.Opcode.Name))

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

            debugOut (builder.ToString()) operands |> addStatement

        let byRefVariable variableIndex =

            let byRefVariableFromExpression expression =
                initTemp (ReadComputedVarExpr(expression)), (fun v -> WriteComputedVarStmt(expression, v) |> addStatement)

            let byRefVariableFromValue value =
                if value = 0uy then
                    initTemp StackPeekExpr, (fun v -> StackUpdateStmt(v) |> addStatement)
                elif value < 16uy then
                    let varIndex = value - 1uy
                    initTemp (readLocal varIndex), (fun v -> writeLocal varIndex v)
                else
                    let varIndex = byteConst (value - 16uy)
                    initTemp (ReadGlobalExpr(varIndex)), (fun v -> WriteGlobalStmt(varIndex, v) |> addStatement)

            let byRefVariableFromOperandTemp temp =
                match operandMap |> Map.tryFind temp with
                | Some(ConstantExpr(Byte(b))) ->
                    byRefVariableFromValue b
                | Some(e) ->
                    byRefVariableFromExpression temp
                | _ ->
                    failcompile "Expected operand temp for by-ref variable index"

            match variableIndex with
            | ConstantExpr(Byte(b)) ->
                byRefVariableFromValue b
            | TempExpr(_) as t ->
                byRefVariableFromOperandTemp t
            | e ->
                byRefVariableFromExpression e

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
                    store (number .<<. (places .&. (int32Const 0x1f))))
                (fun () ->
                    store (number .>>. ((negate places) .&. (int32Const 0x1f))))

        | "buffer_mode", AtLeast 4, Op1(flag) ->
            // TODO: Do we need to do anything with this -- we always buffer!
            discard zero

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
            writeObjectAttribute objNum attrNum false

        | "dec", Any, Op1(varIndex) ->
            let read, write = byRefVariable varIndex

            let read = read |> toInt16
            write (read .-. (one |> toInt16))

        | "dec_chk", Any, Op2(varIndex,value) ->
            let read, write = byRefVariable varIndex

            let read = read |> toInt16
            let newValue = initTemp (read .-. (one |> toInt16))
            write newValue
            branchIf (newValue |> toInt16 .<. (value |> toInt16))

        | "div", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left ./. right)

        | "erase_window", AtLeast 4, Op1(window) ->
            ClearWindowStmt(window) |> addStatement

        | "get_child", Any, Op1(objNum) ->
            let child = readObjectChild objNum
            store child
            branchIf (child .<>. zero)

        | "get_next_prop", Any, Op2(objNum, propNum) ->
            let firstPropertyAddress = readObjectFirstPropertyAddress(objNum)
            let propAddressRead, propAddressWrite = initMutableTemp firstPropertyAddress
            let firstByteRead, firstByteWrite = initMutableTemp (readByte propAddressRead)

            let mask = if memory.Version <= 3 then byteConst 0x1fuy else byteConst 0x3fuy

            ifThen (propNum .<>. zero) (fun () ->
                loopWhile ((firstByteRead .&. mask) .>. propNum) (fun () ->
                    firstByteWrite (readByte propAddressRead)
                    let nextPropAddress = readObjectNextPropertyAddress propAddressRead
                    propAddressWrite nextPropAddress))

            let result = (readByte propAddressRead) .&. mask
            store result

        | "get_parent", Any, Op1(objNum) ->
            let parent = readObjectParent objNum
            store parent

        | "get_prop", Any, Op2(objNum, propNum) ->
            let firstPropertyAddress = readObjectFirstPropertyAddress(objNum)
            let propAddressRead, propAddressWrite = initMutableTemp firstPropertyAddress
            let firstByteRead, firstByteWrite = initMutableTemp zero
            let stopLoopRead, stopLoopWrite = initMutableTemp zero
            let resultRead, resultWrite = initMutableTemp zero

            let mask = if memory.Version <= 3 then byteConst 0x1fuy else byteConst 0x3fuy
            let comp = if memory.Version <= 3 then byteConst 0xe0uy else byteConst 0xc0uy

            loopWhile (stopLoopRead .=. zero) (fun () ->
                firstByteWrite (readByte propAddressRead)
                ifThenElse ((firstByteRead .&. mask) .<=. propNum)
                    (fun () -> stopLoopWrite one)
                    (fun () -> propAddressWrite (readObjectNextPropertyAddress propAddressRead)))

            ifThenElse ((firstByteRead .&. mask) .=. propNum)
                (fun () ->
                    propAddressWrite (propAddressRead .+. one)

                    ifThenElse ((firstByteRead .&. comp) .=. zero)
                        (fun () ->
                            resultWrite (readByte propAddressRead))
                        (fun () ->
                            resultWrite (readWord propAddressRead)))
                (fun () ->
                    resultWrite (readObjectPropertyDefault propNum))

            store resultRead

        | "get_prop_addr", Any, Op2(objNum, propNum) ->
            let firstPropertyAddress = readObjectFirstPropertyAddress(objNum)

            let propAddressRead, propAddressWrite = initMutableTemp firstPropertyAddress
            let firstByteRead, firstByteWrite = initMutableTemp zero
            let stopLoopRead, stopLoopWrite = initMutableTemp zero

            let mask = if memory.Version <= 3 then byteConst 0x1fuy else byteConst 0x3fuy

            loopWhile (stopLoopRead .=. zero) (fun () ->
                firstByteWrite (readByte propAddressRead)

                ifThenElse ((firstByteRead .&. mask) .<=. propNum)
                    (fun () -> stopLoopWrite one)
                    (fun () -> propAddressWrite (readObjectNextPropertyAddress propAddressRead)))

            ifThenElse ((firstByteRead .&. mask) .=. propNum)
                (fun () ->
                    if memory.Version >= 4 then
                        ifThen ((firstByteRead .&. (int32Const 0x80)) .<>. zero)
                            (fun() -> propAddressWrite (propAddressRead .+. one))

                    store (propAddressRead .+. one))
                (fun () ->
                    store zero)

        | "get_prop_len", Any, Op1(dataAddress) ->
            let valueRead, valueWrite = initMutableTemp zero

            ifThenElse (dataAddress .=. zero)
                (fun () ->
                    store zero)
                (fun () ->
                    valueWrite (readByte (dataAddress .-. one))

                    if memory.Version <= 3 then
                        valueWrite (((valueRead .>>. five) .+. one) |> toByte)
                    else
                        ifThenElse ((valueRead .&. (int32Const 0x80)) .=. zero)
                            (fun () ->
                                valueWrite (((valueRead .>>. six) .+. one) |> toByte))
                            (fun () ->
                                valueWrite (valueRead .&. (int32Const 0x3f)))
                    ifThen (valueRead .=. zero)
                        (fun () ->
                            valueWrite (int32Const 64))

                    store valueRead)

        | "get_sibling", Any, Op1(objNum) ->
            let sibling = readObjectSibling objNum
            store sibling
            branchIf (sibling .<>. zero)

        | "inc", Any, Op1(varIndex) ->
            let read, write = byRefVariable varIndex

            let read = read |> toInt16
            write (read .+. (one |> toInt16))

        | "inc_chk", Any, Op2(varIndex,value) ->
            let read, write = byRefVariable varIndex

            let read = read |> toInt16
            let newValue = initTemp (read .+. (one |> toInt16))
            write newValue
            branchIf (newValue |> toInt16 .>. (value |> toInt16))

        | "insert_obj", Any, Op2(objNum, destObjNum) ->
            insertObject objNum destObjNum

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
            let obj1Parent = readObjectParent objNum1

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

        | "load", Any, Op1(varIndex) ->
            let read, write = byRefVariable varIndex

            let value = read
            store value

        | "loadb", Any, Op2(address,offset) ->
            let address = address .+. offset

            store (ReadMemoryByteExpr(address))

        | "loadw", Any, Op2(address,offset) ->
            let offset = offset .*. two
            let address = address .+. offset

            store (ReadMemoryWordExpr(address))

        | "log_shift", AtLeast 5, Op2(number, places) ->
            let places = places |> toInt16

            ifThenElse (places .>. zero)
                (fun () ->
                    store (number .<<. (places .&. (int32Const 0x1f))))
                (fun () ->
                    store (number .>>. ((negate places) .&. (int32Const 0x1f))))

        | "mod", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left .%. right)

        | "mul", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left .*. right)

        | "new_line", Any, NoOps ->
            textConst "\n" |> printText |> addStatement

        | "not", AtLeast 5, Op1(value) ->
            let result = UnaryOperationKind.Not |> unaryOp value
            store result

        | "or", Any, Op2(left, right) ->
            store (left .|. right)

        | "output_stream", AtLeast 3, Op1(number) ->
            SelectOutputStreamStmt(number) |> addStatement

        | "output_stream", AtLeast 5, Op2(number, table) ->
            SelectMemoryOutputStreamStmt(number, table) |> addStatement

        | "piracy", AtLeast 5, NoOps ->
            branchIf (one)

        | "print", Any, NoOps ->
            instruction.Text.Value
            |> textConst
            |> printText
            |> addStatement

        | "print_addr", Any, Op1(address) ->
            address
            |> readText 
            |> printText
            |> addStatement

        | "print_char", Any, Op1(ch) ->
            ch
            |> printChar
            |> addStatement

        | "print_num", Any, Op1(number) ->
            number
            |> toInt16
            |> numberToText
            |> printText
            |> addStatement

        | "print_obj", Any, Op1(objNum) ->
            readObjectName objNum
            |> printText
            |> addStatement

        | "print_paddr", Any, Op1(address) ->
            unpackStringAddress address
            |> readText 
            |> printText
            |> addStatement

        | "print_ret", Any, NoOps ->
            instruction.Text.Value
            |> textConst
            |> printText
            |> addStatement

            ret one

        | "pull", Any, Op1(varIndex) ->
            let value = initTemp StackPopExpr
            let read, write = byRefVariable varIndex

            write value

        | "put_prop", Any, Op3(objNum, propNum, value) ->
            let firstPropertyAddress = readObjectFirstPropertyAddress(objNum)
            let propAddressRead, propAddressWrite = initMutableTemp firstPropertyAddress
            let firstByteRead, firstByteWrite = initMutableTemp zero
            let stopLoopRead, stopLoopWrite = initMutableTemp zero

            let mask = if memory.Version <= 3 then byteConst 0x1fuy else byteConst 0x3fuy
            let comp = if memory.Version <= 3 then byteConst 0xe0uy else byteConst 0xc0uy

            loopWhile (stopLoopRead .=. zero) (fun () ->
                firstByteWrite (readByte propAddressRead)
                ifThenElse ((firstByteRead .&. mask) .<=. propNum)
                    (fun () -> stopLoopWrite one)
                    (fun () -> propAddressWrite (readObjectNextPropertyAddress propAddressRead)))

            ifThen ((firstByteRead .&. mask) .<>. propNum)
                (fun () ->
                    RuntimeExceptionStmt("Property not found!") |> addStatement)

            propAddressWrite (propAddressRead .+. one)

            ifThenElse ((firstByteRead .&. comp) .=. zero)
                (fun () ->
                    writeByte propAddressRead (value |> toByte) |> addStatement)
                (fun () ->
                    writeWord propAddressRead value |> addStatement)

        | "push", Any, Op1(value) ->
            StackPushStmt(value) |> addStatement

        | "quit", Any, NoOps ->
            QuitStmt |> addStatement

        | "random", Any, Op1(range) ->
            let range = range |> toInt16

            ifThenElse (range .>. zero)
                (fun () ->
                    store (random range))
                (fun () ->
                    randomize range |> addStatement
                    store zero)

        | "read_char", AtLeast 4, NoOps ->
            store (ReadInputCharExpr)

        | "read_char", AtLeast 4, Op1(input) ->
            store (ReadInputCharExpr)

        | "read_char", AtLeast 4, Op3(input, time, routine) ->
            store (ReadTimedInputCharExpr(time, routine))

        | "remove_obj", Any, Op1(objNum) ->
            removeObject objNum

        | "ret", Any, Op1(value) ->
            ret value

        | "ret_popped", Any, NoOps ->
            ret StackPopExpr

        | "rfalse", Any, NoOps ->
            ret zero

        | "rtrue", Any, NoOps ->
            ret one

        | "scan_table", AtLeast 4, Op3(x, table, len) ->
            scanTable x table len (byteConst 0x82uy)

        | "scan_table", AtLeast 4, Op4(x, table, len, form) ->
            scanTable x table len form

        | "set_attr", Any, Op2(objNum, attrNum) ->
            writeObjectAttribute objNum attrNum true

        | "set_cursor", Is 6, Op3(line, column, window) ->
            failcompile "set_cursor not implemented for version 6"

        | "set_cursor", AtLeast 4, Op2(line, column) ->
            SetCursorStmt(line, column) |> addStatement

        | "set_text_style", AtLeast 4, Op1(textStyle) ->
            SetTextStyleStmt(textStyle) |> addStatement

        | "set_window", AtLeast 3, Op1(window) ->
            SetWindowStmt(window) |> addStatement

        | "split_window", AtLeast 3, Op1(lines) ->
            SplitWindowStmt(lines) |> addStatement

        | "sread", AtMost 3, Op2(textBuffer, parseBuffer) ->
            discard (ReadInputTextExpr(textBuffer, parseBuffer))

        | "sread", Is 4, Op2(textBuffer, parseBuffer) ->
            discard (ReadInputTextExpr(textBuffer, parseBuffer))

        | "store", Any, Op2(varIndex, value) ->
            let read, write = byRefVariable varIndex

            write value

        | "storeb", Any, Op3(address, offset, value) ->
            let address = address .+. offset

            WriteMemoryByteStmt(address, value) |> addStatement

        | "storew", Any, Op3(address, offset, value) ->
            let offset = offset .*. two
            let address = address .+. offset

            WriteMemoryWordStmt(address, value) |> addStatement

        | "sub", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left .-. right)

        | "test", Any, Op2(bitmap, flags) ->
            branchIf ((bitmap .&. flags) .=. flags)

        | "test_attr", Any, Op2(objNum, attrNum) ->
            let attributeValue = readObjectAttribute objNum attrNum

            branchIf (attributeValue .=. one)

        | "verify", AtLeast 3, NoOps ->
            branchIf VerifyExpr

        | (n,k,ops) ->
            runtimeException "Unsupported opcode: %s (v.%d) with %d operands" n k ops.Length |> addStatement

type RoutineBinder(memory: Memory, debugging: bool) =

    let updateTree f tree =
        let updater = new BoundTreeUpdater(tree)
        updater.Update f
        updater.GetTree()

    let lower_GlobalVariableReadsAndWrites tree =
        let globalVariableTableAddress =
            let x = memory |> Header.readGlobalVariableTableAddress
            int32Const x.IntValue

        tree |> updateTree (fun s updater ->
            let computeGlobalVariableAddress index =
                (index .*. two) .+. globalVariableTableAddress

            let s' =
                s |> rewriteStatement
                    (fun s ->
                        match s with
                        | WriteGlobalStmt(i,v) -> writeWord (computeGlobalVariableAddress i) v
                        | s -> s)
                    (fun e ->
                        match e with
                        | ReadGlobalExpr(i) -> readWord (computeGlobalVariableAddress i)
                        | e -> e)

            updater.AddStatement(s'))

    let lower_ComputedVariableReadsAndWrites tree =
        let sixteen = int32Const 16

        tree |> updateTree (fun s updater ->

            let rewriteReadComputedVar index =
                let varIndex = updater.InitTemp(index)
                let valueRead, valueWrite = updater.InitMutableTemp(zero)

                updater.IfThenElse (varIndex .=. zero)
                    (fun () ->
                        valueWrite StackPeekExpr

                        if debugging then updater.AddStatement(debugOut "** indirectly read SP: {0:x4}" [valueRead]))
                    (fun () ->
                        updater.IfThenElse (varIndex .<. sixteen)
                            (fun () ->
                                let varIndex' = varIndex .-. one
                                valueWrite (ReadLocalExpr(varIndex'))

                                if debugging then updater.AddStatement(debugOut "** indirectly read L{0:x2}: {1:x4}" [varIndex'; valueRead]))
                            (fun () ->
                                let varIndex' = varIndex .-. sixteen
                                valueWrite (ReadGlobalExpr(varIndex'))

                                if debugging then updater.AddStatement(debugOut "** indirectly read G{0:x2}: {1:x4}" [varIndex'; valueRead])))

                valueRead

            let rewriteWriteComputedVar index value =
                let varIndex = updater.InitTemp(index)

                updater.IfThenElse (varIndex .=. zero)
                    (fun () ->
                        updater.AddStatement(StackUpdateStmt(value))

                        if debugging then updater.AddStatement(debugOut "** indirectly write SP: {0:x4}" [value]))
                    (fun () ->
                        updater.IfThenElse (varIndex .<. sixteen)
                            (fun () ->
                                let varIndex' = varIndex .-. one
                                updater.AddStatement(WriteLocalStmt(varIndex', value))

                                if debugging then updater.AddStatement(debugOut "** indirectly write L{0:x2}: {1:x4}" [varIndex'; value]))
                            (fun () ->
                                let varIndex' = varIndex .-. sixteen
                                updater.AddStatement(WriteGlobalStmt(varIndex', value))

                                if debugging then updater.AddStatement(debugOut "** indirectly write G{0:x2}: {1:x4}" [varIndex'; value])))

            // First, rewrite any computed variable reads
            let s' =
                s |> rewriteStatement
                    (fun s -> s)
                    (fun e ->
                        match e with
                        | ReadComputedVarExpr(index) -> rewriteReadComputedVar(index)
                        | e -> e)

            // Next, check to see if this is a computed variable write
            match s' with
            | WriteComputedVarStmt(index, value) -> rewriteWriteComputedVar index value
            | s -> updater.AddStatement(s))

    let lower tree =
        tree
        |> lower_ComputedVariableReadsAndWrites
        |> lower_GlobalVariableReadsAndWrites

    member x.BindRoutine(routine: Routine) =

        let builder = new BoundTreeCreator(routine)
        let binder = new InstructionBinder(memory, routine, builder, debugging)

        for i in routine.Instructions do
            binder.BindInstruction(i)

        builder.GetTree()
            |> Optimization.cleanupLabels
            |> Optimization.cleanupTemps
            |> lower
