namespace NZag.Core

open NZag.Utilities
open BoundNodeConstruction
open BoundNodeVisitors
open OperandPatterns

[<AbstractClass>]
type BoundTreeBuilder() =

    member x.AssignTemp temp value =
        let write = WriteTempStmt(temp, value)
        x.AddStatement(write)

    member x.InitTemp value =
        let temp = x.NewTemp()
        x.AssignTemp temp value
        TempExpr(temp)

    member x.InitMutableTemp value =
        let temp = x.NewTemp()
        x.AssignTemp temp value

        let read = TempExpr(temp)
        let write v = WriteTempStmt(temp, v) |> x.AddStatement

        read, write

    member x.MarkLabel label =
        let label = LabelStmt(label)
        x.AddStatement(label)

    member x.JumpTo label =
        let jump = JumpStmt(label)
        x.AddStatement(jump)

    member x.BranchIf condition expression label =
        let jump = JumpStmt(label)
        let branch = BranchStmt(condition, expression, jump)
        x.AddStatement(branch)

    member x.BranchIfFalse expression label =
        label |> x.BranchIf false expression

    member x.BranchIfTrue expression label =
        label |> x.BranchIf true expression

    member x.Return expression =
        let ret = ReturnStmt(expression)
        x.AddStatement(ret)

    member x.IfThen condition whenTrue =
        let whenTrueLabel = x.NewLabel()
        let doneLabel = x.NewLabel()

        doneLabel |> x.BranchIfFalse condition

        x.MarkLabel(whenTrueLabel)
        whenTrue()

        x.MarkLabel(doneLabel)

    member x.IfThenElse condition whenTrue whenFalse =
        let whenTrueLabel = x.NewLabel()
        let whenFalseLabel = x.NewLabel()
        let doneLabel = x.NewLabel()

        whenFalseLabel |> x.BranchIfFalse condition

        x.MarkLabel(whenTrueLabel)
        whenTrue()
        x.JumpTo(doneLabel)

        x.MarkLabel(whenFalseLabel)
        whenFalse()

        x.MarkLabel(doneLabel)

    member x.LoopWhile condition whileFalse =
        let startLabel = x.NewLabel()

        x.MarkLabel(startLabel)
        whileFalse()

        startLabel |> x.BranchIfTrue condition

    abstract member AddStatement : statement:Statement -> unit
    abstract member NewTemp : unit -> int
    abstract member NewLabel : unit -> int

    abstract member Statements : List<Statement>
    abstract member TempCount : int
    abstract member LabelCount : int

    abstract member GetTree : unit -> BoundTree

type BoundTreeCreator(routine: Routine) =
    inherit BoundTreeBuilder()

    let statements = ResizeArray.create()
    let mutable tempCount = 0
    let mutable labelCount = 0

    let newLabel() =
        let newLabel = labelCount
        labelCount <- labelCount + 1
        newLabel

    let newTemp() =
        let newTemp = tempCount
        tempCount <- tempCount + 1
        newTemp

    let jumpTargetMap =
        let d = Dictionary.create()
        for a in routine.JumpTargets do
            d.Add(a, newLabel())
        d

    member x.IsJumpTarget address =
        jumpTargetMap.ContainsKey(address)

    member x.GetJumpTargetLabel address =
        jumpTargetMap.[address]

    override x.AddStatement(statement: Statement) =
        statements.Add(statement)

    override x.NewTemp() =
        newTemp()

    override x.NewLabel() =
        newLabel()

    override x.Statements = statements |> List.ofSeq
    override x.TempCount = tempCount
    override x.LabelCount = labelCount

    override x.GetTree() =
        { Statements = statements |> Seq.toList; TempCount = tempCount; LabelCount = labelCount }

type BoundTreeUpdater(tree : BoundTree) =
    inherit BoundTreeBuilder()

    let mutable statements = ResizeArray.create()
    let mutable tempCount = tree.TempCount
    let mutable labelCount = tree.LabelCount

    override x.AddStatement(statement: Statement) =
        statements.Add(statement)

    override x.NewTemp() =
        let newTemp = tempCount
        tempCount <- tempCount + 1
        newTemp

    override x.NewLabel() =
        let newLabel = labelCount
        labelCount <- labelCount + 1
        newLabel

    override x.Statements = statements |> List.ofSeq
    override x.TempCount = tempCount
    override x.LabelCount = labelCount

    member x.Update f =
        for s in tree.Statements do
            f s x

    override x.GetTree() =
        { Statements = statements |> Seq.toList; TempCount = tempCount; LabelCount = labelCount }

type InstructionBinder(memory: Memory, builder: BoundTreeCreator, debugging: bool) =

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
        let objNum = if memory.Version <= 3 then readByte address
                     else readWord address
        initTemp objNum

    let computeObjectAddress objNum =
        let t1 = initTemp (objNum .-. one)
        let t2 = initTemp (t1 .*. objectEntrySize)
        let result = initTemp (t2 .+. objectEntriesAddress)
        result

    let readObjectChild objNum =
        let objAddress = computeObjectAddress objNum
        let result = initTemp (objAddress .+. objectChildOffset)
        result |> readObjectNumber

    let readObjectParent objNum =
        let objAddress = computeObjectAddress objNum
        let result = initTemp (objAddress .+. objectParentOffset)
        result |> readObjectNumber

    let readObjectSibling objNum =
        let objAddress = computeObjectAddress objNum
        let result = initTemp (objAddress .+. objectSiblingOffset)
        result |> readObjectNumber

    let computeAttributeByteAddress objNum attrNum =
        let objAddress = computeObjectAddress objNum
        let byteIndex = initTemp (attrNum ./. eight)
        let result = initTemp (objAddress .+. byteIndex)
        result

    let computeAttributeBitMask attrNum =
        let bit = initTemp (attrNum .%. eight)
        let t1 = initTemp (seven .-. bit)
        let t2 = initTemp (t1 .&. int32Const 0x1f)
        let result = initTemp (one .<<. t2)
        result

    let readObjectAttribute objNum attrNum =
        let bitMask = computeAttributeBitMask attrNum
        let attrByteAddress = initTemp (computeAttributeByteAddress objNum attrNum)
        let attrByte = initTemp (readByte attrByteAddress)
        let t1 = initTemp (attrByte .&. bitMask)
        let result = initTemp (t1 .<>. zero)
        result

    let writeObjectAttribute objNum attrNum value =
        let bitMask = initTemp (computeAttributeBitMask attrNum)
        let attrByteAddress = initTemp (computeAttributeByteAddress objNum attrNum)
        let attrByte = initTemp (readByte attrByteAddress)
        let newAttrByte =
            if value then
                (attrByte .|. bitMask) |> toByte
            else
                (attrByte .&. (bitNot bitMask)) |> toByte

        writeByte attrByteAddress newAttrByte |> addStatement

    let readObjectName objNum =
        let objAddress = computeObjectAddress objNum
        let propTableAddress = initTemp (objAddress .+. objectPropertyTableOffset)
        let propsAddress = initTemp (readWord propTableAddress)
        let nameLength = initTemp (readByte propsAddress)
        let nameAddress = initTemp (propsAddress .+. one)
        let result = readTextOfLength nameAddress nameLength
        result

    let readObjectFirstPropertyAddress objNum =
        let objAddress = computeObjectAddress objNum
        let propTableAddress = initTemp (objAddress .+. objectPropertyTableOffset)
        let propsAddress = initTemp (readWord propTableAddress)

        // First property is address after object name
        let nameLength = initTemp (readByte propsAddress)
        let t1 = initTemp (propsAddress .+. one)
        let t2 = initTemp (nameLength .*. two)
        let result = initTemp (t1 .+. t2)

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
                        let nextByteAddress = initTemp (propAddress .+. one)
                        let nextPropSizeByte = readByte nextByteAddress
                        let nextPropSizeByte' = nextPropSizeByte .&. (int32Const 0x3f)
                        ifThenElse (nextPropSizeByte' .=. zero)
                            (fun () ->
                                write (int32Const 64))
                            (fun () -> 
                                write nextPropSizeByte'))
                read

        let result = initTemp ((propAddress .+. one) .+. (propSize .+. one))
        result |> toUInt16

    let readObjectPropertyDefault propNum =
        let t1 = initTemp (propNum .-. one)
        let t2 = initTemp (t1 .*. two)
        let t3 = initTemp (objectTableAddress .+. t2)

        let result = initTemp (readWord t3)
        result |> toUInt16

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

            storeVar |> writeVar expression |> addStatement

        let discard expression =
            match instruction.StoreVariable with
            | Some(_) -> failcompile "Expected instruction to not have a store variable."
            | None -> ()

            discardValue expression |> addStatement

        // If this instruction is a jump target, mark its label
        if builder.IsJumpTarget(instruction.Address) then
            let label = builder.GetJumpTargetLabel(instruction.Address)
            builder.MarkLabel(label)

        // If debugging, write the instruction to the debug output
        if debugging then
            debugOut (instruction.ToString()) [] |> addStatement

        // Create temps for all operands
        let operandValues = instruction.Operands |> List.map bindOperand
        let operandTemps = operandValues |> List.map (fun v -> initTemp v)
        let operandMap = List.zip operandTemps operandValues |> Map.ofList

        let byRefVariable variableIndex =

            let byRefVariableFromExpression expression =
                initTemp (ReadComputedVarExpr(expression)), (fun v -> WriteComputedVarStmt(expression, v))

            let byRefVariableFromValue value =
                if value = 0uy then
                    initTemp StackPeekExpr, (fun v -> StackUpdateStmt(v))
                elif value < 16uy then
                    let varIndex = byteConst (value - 1uy)
                    initTemp (ReadLocalExpr(varIndex)), (fun v -> WriteLocalStmt(varIndex, v))
                else
                    let varIndex = byteConst (value - 16uy)
                    initTemp (ReadGlobalExpr(varIndex)), (fun v -> WriteGlobalStmt(varIndex, v))

            let byRefVariableFromOperandTemp temp =
                match operandMap |> Map.tryFind temp with
                | Some(ConstantExpr(Byte(b))) ->
                    byRefVariableFromValue b
                | Some(e) ->
                    byRefVariableFromExpression e
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
        match (instruction.Opcode.Name, instruction.Opcode.Version, operandTemps) with
        | "add", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left .+. right)

        | "and", Any, Op2(left, right) ->
            store (left .&. right)

        | "art_shift", AtLeast 5uy, Op2(number, places) ->
            let number = number |> toInt16
            let places = places |> toInt16

            ifThenElse (places .>. zero)
                (fun () ->
                    store (number .<<. (places .&. (int32Const 0x1f))))
                (fun () ->
                    store (number .>>. ((negate places) .&. (int32Const 0x1f))))

        | "call", Any, OpAndList(address, args)
        | "call_1s", AtLeast 4uy, OpAndList(address, args)
        | "call_2s", AtLeast 4uy, OpAndList(address, args)
        | "call_vs", Any, OpAndList(address, args)
        | "call_vs2", AtLeast 4uy, OpAndList(address, args) ->

            call address args store

        | "call_1n", AtLeast 5uy, OpAndList(address, args)
        | "call_2n", AtLeast 5uy, OpAndList(address, args)
        | "call_vn", AtLeast 5uy, OpAndList(address, args)
        | "call_vn2", AtLeast 4uy, OpAndList(address, args) ->

            call address args discard

        | "check_arg_count", AtLeast 5uy, Op1(number) ->
            branchIf (number .<=. ArgCountExpr)

        | "clear_attr", Any, Op2(objNum, attrNum) ->
            writeObjectAttribute objNum attrNum false

        | "dec", Any, Op1(varIndex) ->
            let read, write = byRefVariable varIndex

            let read = read |> toInt16
            write (read .-. (one |> toInt16)) |> addStatement

        | "dec_chk", Any, Op2(varIndex,value) ->
            let read, write = byRefVariable varIndex

            let read = read |> toInt16
            let newValue = initTemp (read .-. (one |> toInt16))
            write newValue |> addStatement
            branchIf (newValue |> toInt16 .<. (value |> toInt16))

        | "div", Any, Op2(left, right) ->
            let left = left |> toInt16
            let right = right |> toInt16

            store (left ./. right)

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

            let result = initTemp ((readByte propAddressRead) .&. mask)
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
            write (read .+. (one |> toInt16)) |> addStatement

        | "inc_chk", Any, Op2(varIndex,value) ->
            let read, write = byRefVariable varIndex

            let read = read |> toInt16
            let newValue = initTemp (read .+. (one |> toInt16))
            write newValue |> addStatement
            branchIf (newValue |> toInt16 .>. (value |> toInt16))

        | "je", Any, OpAndList(left, values) ->
            // je can have 2 to 4 operands to test for equality.
            let conditions = values |> List.map (fun v -> initTemp (left .=. v))

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

            let value = initTemp read
            store value

        | "loadb", Any, Op2(address,offset) ->
            let address = address .+. offset

            store (ReadMemoryByteExpr(address))

        | "loadw", Any, Op2(address,offset) ->
            let offset = offset .*. two
            let address = address .+. offset

            store (ReadMemoryWordExpr(address))

        | "log_shift", AtLeast 5uy, Op2(number, places) ->
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

        | "not", AtLeast 5uy, Op1(value) ->
            let result = UnaryOperationKind.Not |> unaryOp value
            store result

        | "or", Any, Op2(left, right) ->
            store (left .|. right)

        | "print", Any, NoOps ->
            textConst instruction.Text.Value |> printText |> addStatement

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

        | "pull", Any, Op1(varIndex) ->
            let read, write = byRefVariable varIndex

            let value = StackPopExpr
            write value |> addStatement

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

        | "ret", Any, Op1(value) ->
            ret value

        | "ret_popped", Any, NoOps ->
            ret StackPopExpr

        | "rfalse", Any, NoOps ->
            ret zero

        | "rtrue", Any, NoOps ->
            ret one

        | "set_attr", Any, Op2(objNum, attrNum) ->
            writeObjectAttribute objNum attrNum true

        | "store", Any, Op2(varIndex, value) ->
            let read, write = byRefVariable varIndex

            write value |> addStatement

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

        | "test_attr", Any, Op2(objNum, attrNum) ->
            let attributeValue = readObjectAttribute objNum attrNum

            branchIf (attributeValue .=. one)

        | (n,k,ops) ->
            runtimeException "Unsupported opcode: %s (v.%d) with %d operands" n k ops.Length |> addStatement

type RoutineBinder(memory: Memory, debugging: bool) =

    let updateTree f tree =
        let updater = new BoundTreeUpdater(tree)
        updater.Update f
        updater.GetTree()

    let cleanupLabels tree =
        // There are two goals here:
        //
        // * First, we want to reorder the labels so that the label numbers
        //   are in ascending order.
        //
        // * Second, we want to remove any redundant labels where a label
        //   immediately follows another one. This ensures that a sensible
        //   control flow graph can be created.

        let nextLabelIndex = ref 0
        let labels = Dictionary.create()
        let lastLabel = ref None

        // First, collect the existing labels and create new ones
        tree |> walkTree
            (fun s ->
                match s with
                | LabelStmt(l) ->
                    match !lastLabel with
                    | Some(newLabel) -> 
                        labels |> Dictionary.add l newLabel
                    | None ->
                        let newLabel = !nextLabelIndex
                        labels |> Dictionary.add l newLabel
                        incr nextLabelIndex
                        lastLabel := Some(newLabel)
                | _ ->
                    lastLabel := None)
            (fun e -> ())

        // Next, rewrite the tree, replacing the old labels with new ones
        let createdLabels = ref Set.empty

        tree |> updateTree (fun s updater ->
            match s with
            | LabelStmt(l) ->
                // Be careful not to add duplicate labels
                let l' = labels.[l]
                if not (!createdLabels |> Set.contains l') then
                    LabelStmt(l') |> updater.AddStatement
                    createdLabels := !createdLabels |> Set.add l'
            | JumpStmt(l) ->
                JumpStmt(labels.[l]) |> updater.AddStatement
            | BranchStmt(c,e,JumpStmt(l)) ->
                BranchStmt(c,e,JumpStmt(labels.[l])) |> updater.AddStatement
            | s ->
                updater.AddStatement(s))

    let cleanupTemps tree =
        let nextTempIndex = ref 0
        let temps = Dictionary.create()

        let getOrAddTemp t =
            match temps |> Dictionary.tryFind t with
            | Some(t) -> t
            | None    -> let newTemp = !nextTempIndex
                         temps |> Dictionary.add t newTemp
                         incr nextTempIndex
                         newTemp

        // Rewrite the tree's statements, replacing old temps with new ones
        let rewriteTemps s =
            s |> rewriteStatement
                (fun s -> 
                    match s with
                    | WriteTempStmt(t,e) ->
                        let newTemp = getOrAddTemp t
                        WriteTempStmt(newTemp,e)
                    | s -> s)
                (fun e ->
                    match e with
                    | TempExpr(t) ->
                        let newTemp = getOrAddTemp t
                        TempExpr(newTemp)
                    | e -> e)

        let newStatements = tree.Statements |> List.map rewriteTemps

        { Statements = newStatements; TempCount = !nextTempIndex; LabelCount = tree.LabelCount }

    let lower_GlobalVariableReadsAndWrites tree =
        let globalVariableTableAddress =
            let x = memory |> Header.readGlobalVariableTableAddress
            int32Const x.IntValue

        tree |> updateTree (fun s updater ->
            let computeGlobalVariableAddress index =
                let x = updater.InitTemp(index .*. two)
                updater.InitTemp(x .+. globalVariableTableAddress)

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

    let optimize_PropagateConstants tree =
        let graph = Graphs.buildControlFlowGraph tree
        let reachingDefinitions = Graphs.computeReachingDefinitions graph

        let block = ref None
        let index = ref 0

        let setBlock id =
            block := Some(reachingDefinitions.Graph.Blocks |> List.find (fun b -> b.ID = id))

        let getDefinitions t =
            match !block with
            | Some(b) ->
                b.Data.Statements.[!index].InDefinitions
                    |> Set.filter (fun d -> d.Temp = t)
                    |> Set.map (fun d -> d.Value)
                    |> Set.toList
            | None ->
                failcompile "Couldn't find statement info"

        tree |> updateTree (fun s updater ->
            let s' =
                s |> rewriteStatement
                    (fun s ->
                        match s with
                        | LabelStmt(l) ->
                            index := 0
                            setBlock(l)
                            incr index
                            s
                        | s ->
                            incr index
                            s)
                    (fun e ->
                        match e with
                        | TempExpr(t) ->
                            // If this temp only has a single definition of a constant expression
                            // at this point, use the constant.
                            match getDefinitions t with
                            | [ConstantExpr(c) as v] -> v
                            | _ -> e
                        | e -> e)

            updater.AddStatement(s'))

    let optimize_FoldConstants tree =
        let binaryOp l r k =
            match k with
            | BinaryOperationKind.Add         -> Some(wordConst (uint16 ((int16 l) + (int16 r))))
            | BinaryOperationKind.Subtract    -> Some(wordConst (uint16 ((int16 l) - (int16 r))))
            | BinaryOperationKind.Multiply    -> Some(wordConst (uint16 ((int16 l) * (int16 r))))
            | BinaryOperationKind.Divide      -> Some(wordConst (uint16 ((int16 l) / (int16 r))))
            | BinaryOperationKind.Remainder   -> Some(wordConst (uint16 ((int16 l) % (int16 r))))
            | BinaryOperationKind.Or          -> Some(wordConst (uint16 l ||| uint16 r))
            | BinaryOperationKind.And         -> Some(wordConst (uint16 l &&& uint16 r))
            | BinaryOperationKind.ShiftLeft   -> Some(wordConst (uint16 l <<< r))
            | BinaryOperationKind.ShiftRight  -> Some(wordConst (uint16 l >>> r))
            | BinaryOperationKind.Equal       -> if l = r then Some(one) else Some(zero)
            | BinaryOperationKind.NotEqual    -> if l <> r then Some(one) else Some(zero)
            | BinaryOperationKind.LessThan    -> if (int16 l) < (int16 r) then Some(one) else Some(zero)
            | BinaryOperationKind.GreaterThan -> if (int16 l) > (int16 r) then Some(one) else Some(zero)
            | BinaryOperationKind.AtLeast     -> if (int16 l) >= (int16 r) then Some(one) else Some(zero)
            | BinaryOperationKind.AtMost      -> if (int16 l) <= (int16 r) then Some(one) else Some(zero)
            | _ -> None

        tree |> updateTree (fun s updater ->
            let s' =
                s |> rewriteStatement
                    (fun s -> s)
                    (fun e ->
                        match e with
                        | BinaryOperationExpr(k,l,r) ->
                            match l,r with
                            | ConstantExpr(Int32Value v1),ConstantExpr(Int32Value v2) ->
                                match binaryOp v1 v2 k with
                                | Some(res) -> res
                                | None -> e
                            | (TempExpr(_) as t), ConstantExpr(Zero)
                            | ConstantExpr(Zero), (TempExpr(_) as t) ->
                                match k with
                                | BinaryOperationKind.Add -> t
                                | BinaryOperationKind.Multiply -> zero
                                | _ -> e
                            | _ -> e
                        | e -> e)

            updater.AddStatement(s'))

    let optimize_RemoveUnusedTemps tree =
        let graph = Graphs.buildControlFlowGraph tree
        let reachingDefinitions = Graphs.computeReachingDefinitions graph

        let block = ref None
        let index = ref 0

        let setBlock id =
            block := Some(reachingDefinitions.Graph.Blocks |> List.find (fun b -> b.ID = id))

        let hasUsages t =
            match !block with
            | Some(b) ->
                let defs =
                    reachingDefinitions.Definitions.[t]
                    |> Set.filter (fun d -> d.BlockID = b.ID && d.StatementIndex = !index)
                    |> Set.toList

                match defs with
                | [d] ->
                    match reachingDefinitions.Usages |> Map.tryFind d with
                    | Some(u) -> u > 0
                    | None -> false
                | _ -> failcompile "Expected a single definition"

            | None ->
                failcompile "Couldn't find block"

        tree |> updateTree (fun s updater ->
            let s' = match s with
                     | LabelStmt(l) ->
                         index := 0
                         setBlock(l)
                         Some(s)
                     | WriteTempStmt(t,_) ->
                         if hasUsages t then Some(s)
                         else None
                     | s ->
                         Some(s)

            incr index

            match s' with
            | Some(s') -> updater.AddStatement(s')
            | None -> ())

    let optimize_EliminateDeadBranches tree =
        tree |> updateTree (fun s updater ->
            let s' = match s with
                     | BranchStmt(b,e,j) ->
                         if b && e = one then
                             Some(j)
                         elif not b && e = zero then
                             Some(j)
                         else
                             Some(s)
                     | s ->
                         Some(s)

            match s' with
            | Some(s') -> updater.AddStatement(s')
            | None -> ())

    let optimize_EliminateDeadBlocks tree =
        let graph = Graphs.buildControlFlowGraph tree

        let block = ref None

        let setBlock id =
            block := Some(graph.Blocks |> List.find (fun b -> b.ID = id))

        let hasPredecessors() =
            match !block with
            | Some(b) -> b.Predecessors.Length > 0
            | None -> failcompile "Couldn't find block"

        tree |> updateTree (fun s updater ->
            match s with
            | LabelStmt(l) ->
                setBlock(l)
            | s -> ()

            if hasPredecessors() then
                updater.AddStatement(s))

    let optimize_EliminateRedundantLabels tree =
        let graph = Graphs.buildControlFlowGraph tree

        let labelsToRemove = ref Set.empty

        for i = 0 to tree.Statements.Length - 2 do
            let s1 = tree.Statements.[i]
            let s2 = tree.Statements.[i+1]

            match s1, s2 with
            | BranchStmt(_,_,_), LabelStmt(_)
            | ReturnStmt(_), LabelStmt(_)
            | QuitStmt(_), LabelStmt(_) -> ()

            | JumpStmt(j), LabelStmt(l) ->
                if j = l then
                    let b = graph.Blocks |> List.find (fun b -> b.ID = l)
                    if b.Predecessors.Length = 1 then
                        labelsToRemove := !labelsToRemove |> Set.add l
            | _, LabelStmt(l) ->
                let b = graph.Blocks |> List.find (fun b -> b.ID = l)
                if b.Predecessors.Length = 1 then
                    labelsToRemove := !labelsToRemove |> Set.add l
            | _ -> ()

        tree |> updateTree (fun s updater ->
            match s with
            | LabelStmt(l)
            | JumpStmt(l) ->
                if not (!labelsToRemove |> Set.contains l) then
                    updater.AddStatement(s)
            | s -> updater.AddStatement(s))

    let lower tree =
        tree
        |> lower_GlobalVariableReadsAndWrites

    let optimize tree =
        tree |> fixedpoint (fun tree ->
            tree
            |> optimize_PropagateConstants
            |> optimize_FoldConstants
            |> optimize_RemoveUnusedTemps
            |> optimize_EliminateDeadBranches
            |> optimize_EliminateDeadBlocks
            |> optimize_EliminateRedundantLabels)

    member x.BindRoutine(routine: Routine) =

        let builder = new BoundTreeCreator(routine)
        let binder = new InstructionBinder(memory, builder, debugging)

        for i in routine.Instructions do
            binder.BindInstruction(i)

        builder.GetTree()
            |> cleanupLabels
            |> cleanupTemps
            |> lower
            |> optimize
            |> cleanupLabels
            |> cleanupTemps
