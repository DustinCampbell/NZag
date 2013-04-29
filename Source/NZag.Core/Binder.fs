namespace NZag.Core

open NZag.Utilities
open BoundNodeConstruction
open BoundNodeVisitors
open OperandPatterns

type BoundTreeBuilder (routine : Routine) =

    let statements = new ResizeArray<_>()
    let mutable tempCount = 0
    let mutable labelCount = 0

    let newLabel() =
        let newLabel = labelCount
        labelCount <- labelCount + 1
        newLabel

    let jumpTargetMap =
        let d = Dictionary.create()
        for a in routine.JumpTargets do
            d.Add(a, newLabel())
        d

    member x.AddStatement(statement : Statement) =
        statements.Add(statement)

    member x.NewTemp() =
        let newTemp = tempCount
        tempCount <- tempCount + 1
        newTemp

    member x.NewLabel() =
        newLabel()

    member x.MarkLabel label =
        let label = LabelStmt(label)
        x.AddStatement(label)

    member x.IsJumpTarget address =
        jumpTargetMap.ContainsKey(address)

    member x.GetJumpTargetLabel address =
        jumpTargetMap.[address]

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

    member x.Statements = statements |> List.ofSeq
    member x.TempCount = tempCount

type InstructionBinder (memory : Memory, builder : BoundTreeBuilder) =

    let packedMultiplier =
        match memory.Version with
        | 1 | 2 | 3 -> two
        | 4 | 5 | 6 | 7 -> four
        | 8 -> eight
        | v -> invalidOperation "Invalid version number: %d" v

    let routinesOffset =
        int32Const (memory |> Header.readRoutinesOffset).IntValue

    let addStatement s =
        builder.AddStatement(s)

    let ret expression =
        builder.Return(expression)

    let assignTemp t v =
        WriteTempStmt(t, v) |> addStatement

    let initTemp expression =
        let t = builder.NewTemp()
        assignTemp t expression
        TempExpr(t)

    let ifThenElse condition whenTrue whenFalse =
        let whenTrueLabel = builder.NewLabel()
        let whenFalseLabel = builder.NewLabel()
        let doneLabel = builder.NewLabel()

        whenFalseLabel |> builder.BranchIfFalse condition

        builder.MarkLabel(whenTrueLabel)
        whenTrue()
        builder.JumpTo(doneLabel)

        builder.MarkLabel(whenFalseLabel)
        whenFalse()

        builder.MarkLabel(doneLabel)

    let bindOperand = function
        | LargeConstantOperand(v) -> wordConst v
        | SmallConstantOperand(v) -> byteConst v
        | VariableOperand(v)      -> readVar v

    member x.BindInstruction (instruction : Instruction) =

        let branchIf expression =
            let branch =
                match instruction.Branch with
                | Some(b) -> b
                | None -> invalidOperation "Expected instruction to have a valid branch."

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
                | None -> invalidOperation "Expected instruction to have a valid store variable."

            storeVar |> writeVar expression |> addStatement

        // If this instruction is a jump target, mark its label
        if builder.IsJumpTarget(instruction.Address) then
            let label = builder.GetJumpTargetLabel(instruction.Address)
            builder.MarkLabel(label)

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
                    invalidOperation "Expected operand temp for by-ref variable index"

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
            let left = initTemp (left |> toInt16)
            let right = initTemp (right |> toInt16)

            store (left .+. right)

        | "call", Any, OpAndList(address, args)
        | "call_vs", Any, OpAndList(address, args) ->

            ifThenElse (address .=. zero)
                (fun () ->
                    store zero)
                (fun () ->
                    let baseAddress = initTemp (address .*. packedMultiplier)
                    let address = 
                        if memory.Version = 6 || memory.Version = 7 then
                            initTemp (baseAddress .+. routinesOffset)
                        else
                            baseAddress

                    store (CallExpr(address, args)))

        | "dec", Any, Op1(varIndex) ->
            let read, write = byRefVariable varIndex

            let read = initTemp (read |> toInt16)
            write (read .-. (one |> toInt16)) |> addStatement

        | "div", Any, Op2(left, right) ->
            let left = initTemp (left |> toInt16)
            let right = initTemp (right |> toInt16)

            store (left ./. right)

        | "inc", Any, Op1(varIndex) ->
            let read, write = byRefVariable varIndex

            let read = initTemp (read |> toInt16)
            write (read .+. (one |> toInt16)) |> addStatement

        | "je", Any, OpAndList(left, values) ->
            // je can have 2 to 4 operands to test for equality.
            let conditions = values |> List.map (fun v -> initTemp (left .=. v))

            branchIf
                ((List.tail conditions)
                |> List.fold
                    (fun res c -> res .|. c)
                    (List.head conditions))

        | "jg", Any, Op2(left, right) ->
            let left = initTemp (left |> toInt16)
            let right = initTemp (right |> toInt16)

            branchIf (left .>. right)

        | "jl", Any, Op2(left, right) ->
            let left = initTemp (left |> toInt16)
            let right = initTemp (right |> toInt16)

            branchIf (left .<. right)

        | "jump", Any, Op1(_) ->
            let label = builder.GetJumpTargetLabel(instruction.JumpAddress)
            builder.JumpTo(label)

        | "jz", Any, Op1(left) ->
            branchIf (left .=. zero)

        | "loadb", Any, Op2(address,offset) ->
            let address = initTemp (address .+. offset)

            store (ReadMemoryByteExpr(address))

        | "loadw", Any, Op2(address,offset) ->
            let offset = initTemp (offset .*. two)
            let address = initTemp (address .+. offset)

            store (ReadMemoryWordExpr(address))

        | "mod", Any, Op2(left, right) ->
            let left = initTemp (left |> toInt16)
            let right = initTemp (right |> toInt16)

            store (left .%. right)

        | "mul", Any, Op2(left, right) ->
            let left = initTemp (left |> toInt16)
            let right = initTemp (right |> toInt16)

            store (left .*. right)

        | "new_line", Any, NoOps ->
            textConst "\n" |> printText |> addStatement

        | "print", Any, NoOps ->
            textConst instruction.Text.Value |> printText |> addStatement

        | "print_obj", Any, Op1(obj) ->
            objectName obj |> printText |> addStatement

        | "random", Any, Op1(range) ->
            let range = initTemp (range |> toInt16)

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

        | "store", Any, Op2(varIndex, value) ->
            let read, write = byRefVariable varIndex

            write value |> addStatement

        | "storeb", Any, Op3(address, offset, value) ->
            let address = initTemp (address .+. offset)

            WriteMemoryByteStmt(address, value) |> addStatement

        | "storew", Any, Op3(address, offset, value) ->
            let offset = initTemp (offset .*. two)
            let address = initTemp (address .+. offset)

            WriteMemoryWordStmt(address, value) |> addStatement

        | "sub", Any, Op2(left, right) ->
            let left = initTemp (left |> toInt16)
            let right = initTemp (right |> toInt16)

            store (left .-. right)

        | "test_attr", Any, Op2(obj, attribute) ->
            let attributeValue = initTemp (ReadObjectAttributeExpr(obj, attribute))

            branchIf (attributeValue .=. one)

        | (n,k,ops) ->
            runtimeException "Unsupported opcode: %s (v.%d) with %d operands" n k ops.Length |> addStatement

type RoutineBinder (memory : Memory) =

    let sortLabels tree =
        let nextLabelIndex = ref 0
        let labels = Dictionary.create()

        // First, collect the existing labels and create new ones
        tree |> walkTree
            (fun s ->
                match s with
                | LabelStmt(l) ->
                    let newLabel = !nextLabelIndex
                    labels.Add(l, newLabel)
                    incr nextLabelIndex
                | _ -> ())
            (fun e -> ())

        // Next, rewrite the tree, replacing the old labels with new ones
        tree |> rewriteTree
            (fun s -> 
                match s with
                | LabelStmt(l) -> LabelStmt(labels.[l])
                | JumpStmt(l) -> JumpStmt(labels.[l])
                | s -> s)
            (fun e -> e)

    member x.BindRoutine (routine : Routine) =

        let builder = new BoundTreeBuilder(routine)
        let binder = new InstructionBinder(memory, builder)

        for i in routine.Instructions do
            binder.BindInstruction(i)

        { Statements = builder.Statements; TempCount = builder.TempCount }
            |> sortLabels
