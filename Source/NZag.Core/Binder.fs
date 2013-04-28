﻿namespace NZag.Core

open System.Text
open NZag.Utilities

type Constant =
    | Byte of byte
    | Word of uint16
    | Int32 of int32
    | Text of string

type UnaryOperationKind =
    | Not = 1
    | Negate = 2

type BinaryOperationKind =
    | Add = 1
    | Subtract = 2
    | Multiply = 3
    | Divide = 4
    | Remainder = 5
    | And = 6
    | Or = 7
    | ShiftLeft = 8
    | ShiftRight = 9
    | Equal = 10
    | NotEqual = 11
    | LessThan = 12
    | AtMost = 13
    | AtLeast = 14
    | GreaterThan = 15

type ConversionKind =
    | ToInt16 = 1

type Expression =

    /// A constant value
    | ConstantExpr of Constant

    /// The value of the temp with the specified index
    | TempExpr of int

    /// Reads the local variable at the specified index
    | ReadLocalExpr of Expression

    // Reads the global variable at the specified index
    | ReadGlobalExpr of Expression

    /// Pops a value off of the VM stack and returns it
    | StackPopExpr

    /// Returns the top value on the VM stack without popping it.
    | StackPeekExpr

    /// Reads the given value to the variable whose index is computed by the specified
    /// expression. Note that computed variables are always read indirectly. That is, stack
    /// writes are peeks rather than pops.
    | ReadComputedVarExpr of Expression

    /// Performs the specified unary operation on the given expression
    | UnaryOperationExpr of UnaryOperationKind * Expression

    /// Performs the specifed binary operation on the given expressions
    | BinaryOperationExpr of BinaryOperationKind * Expression * Expression

    /// Converts the given expression to the type indicated by the conversion kind
    | ConversionExpr of ConversionKind * Expression

    /// Calls the routine at the specified address with the list of given arguments
    | CallExpr of Expression * list<Expression>

    /// Reads a byte from game memory at the specified address
    | ReadMemoryByteExpr of Expression

    /// Reads a word from game memory at the specified address
    | ReadMemoryWordExpr of Expression

    /// Reads name of the object whose number is represented by the given expression
    | ReadObjectNameExpr of Expression

    /// Reads the specified attribute of the object whose number is represented by the given expression
    | ReadObjectAttributeExpr of Expression * Expression

    /// Generates a random number using the given range expression
    | GenerateRandomNumberExpr of Expression

type Statement =

    /// Declares a new label with the specified index
    | LabelStmt of int

    /// Returns the given expression
    | ReturnStmt of Expression

    /// Jumps to the specified label
    | JumpStmt of int

    /// Executes the specified statement if the given expression evaluates to the given condition
    | BranchStmt of bool * Expression * Statement

    /// Writes the given value to the temp at the specified index
    | WriteTempStmt of int * Expression

    /// Writes the given value to the local variable at the specified index
    | WriteLocalStmt of Expression * Expression

    /// Writes the given value to the global variable at the specified index
    | WriteGlobalStmt of Expression * Expression

    /// Pushes the given expression onto the evaluation stack
    | StackPushStmt of Expression

    /// Replaces the top value on the evaluation stack with the given expression
    | StackUpdateStmt of Expression

    /// Writer the given value to the variable whose index is computed by the specified
    /// expression. Note that computed variables are always write indirectly. That is, stack
    /// writes are updates rather than pushes.
    | WriteComputedVarStmt of Expression * Expression

    /// Writes a byte from game memory at the specified address
    | WriteMemoryByteStmt of Expression * Expression

    /// Writes a word from game memory at the specified address
    | WriteMemoryWordStmt of Expression * Expression

    /// Prints the text represented by the given expression
    | PrintTextStmt of Expression

    /// Seeds the random number generator with the given expression
    | SetRandomNumberSeedStmt of Expression

    /// Throws a runtime exception
    | RuntimeExceptionStmt of string

type BoundTree =
  { Statements : list<Statement>
    TempCount : int }

module BoundNodeConstruction =

    let unaryOp e k = UnaryOperationExpr(k, e)
    let binaryOp l r k = BinaryOperationExpr(k, l, r)

    let inline (.-) e = UnaryOperationKind.Negate |> unaryOp e

    let inline (.+.) l r = BinaryOperationKind.Add |> binaryOp l r
    let inline (.-.) l r = BinaryOperationKind.Subtract |> binaryOp l r
    let inline (.*.) l r = BinaryOperationKind.Multiply |> binaryOp l r
    let inline (./.) l r = BinaryOperationKind.Divide |> binaryOp l r
    let inline (.%.) l r = BinaryOperationKind.Remainder |> binaryOp l r
    let inline (.&.) l r = BinaryOperationKind.And |> binaryOp l r
    let inline (.|.) l r = BinaryOperationKind.Or |> binaryOp l r
    let inline (.<<.) l r = BinaryOperationKind.ShiftLeft |> binaryOp l r
    let inline (.>>.) l r = BinaryOperationKind.ShiftRight |> binaryOp l r
    let inline (.=.) l r = BinaryOperationKind.Equal |> binaryOp l r
    let inline (.<>.) l r = BinaryOperationKind.NotEqual |> binaryOp l r
    let inline (.<.) l r = BinaryOperationKind.LessThan |> binaryOp l r
    let inline (.<=.) l r = BinaryOperationKind.AtMost |> binaryOp l r
    let inline (.>.) l r = BinaryOperationKind.GreaterThan |> binaryOp l r
    let inline (.>=.) l r = BinaryOperationKind.AtLeast |> binaryOp l r

    let byteConst v = ConstantExpr(Byte(v))
    let wordConst v = ConstantExpr(Word(v))
    let int32Const v = ConstantExpr(Int32(v))
    let textConst v = ConstantExpr(Text(v))

    let zero = int32Const 0
    let one = int32Const 1
    let two = int32Const 2
    let three = int32Const 3
    let four = int32Const 4
    let five = int32Const 5
    let six = int32Const 6
    let seven = int32Const 7
    let eight = int32Const 8

    let toInt16 v = ConversionExpr(ConversionKind.ToInt16, v)

    let readVar v =
        match v with
        | StackVariable -> StackPopExpr
        | LocalVariable(i) -> ReadLocalExpr(byteConst i)
        | GlobalVariable(i) -> ReadGlobalExpr(byteConst i)

    let writeVar e v =
        match v with
        | StackVariable -> StackPushStmt(e)
        | LocalVariable(i) -> WriteLocalStmt(byteConst i, e)
        | GlobalVariable(i) -> WriteGlobalStmt(byteConst i, e)

    let printText text = PrintTextStmt(text)

    let objectName objNum = ReadObjectNameExpr(objNum)

    let random range = GenerateRandomNumberExpr(range)
    let randomize seed = SetRandomNumberSeedStmt(seed)

    let runtimeException message =
        Printf.ksprintf (fun s -> RuntimeExceptionStmt(s)) message

module BoundNodeVisitors =

    let rec rewriteExpression fexpr expr =
        let rewriteExpr = rewriteExpression fexpr

        match expr with
        | ConstantExpr(c) ->
            fexpr (ConstantExpr(c))
        | TempExpr(i) ->
            fexpr (TempExpr(i))
        | ReadLocalExpr(e) ->
            fexpr (ReadLocalExpr(rewriteExpr e))
        | ReadGlobalExpr(e) ->
            fexpr (ReadGlobalExpr(rewriteExpr e))
        | StackPopExpr ->
            fexpr StackPopExpr
        | StackPeekExpr ->
            fexpr StackPeekExpr
        | ReadComputedVarExpr(e) ->
            fexpr (ReadComputedVarExpr(rewriteExpr e))
        | UnaryOperationExpr(k,e) ->
            fexpr (UnaryOperationExpr(k, rewriteExpr e))
        | BinaryOperationExpr(k,e1,e2) ->
            fexpr (BinaryOperationExpr(k, rewriteExpr e1, rewriteExpr e2))
        | ConversionExpr(k,e) ->
            fexpr (ConversionExpr(k, rewriteExpr e))
        | CallExpr(e,elist) ->
            fexpr (CallExpr(rewriteExpr e, elist |> List.map rewriteExpr))
        | ReadMemoryByteExpr(e) ->
            fexpr (ReadMemoryByteExpr(rewriteExpr e))
        | ReadMemoryWordExpr(e) ->
            fexpr (ReadMemoryWordExpr(rewriteExpr e))
        | ReadObjectNameExpr(e) ->
            fexpr (ReadObjectNameExpr(rewriteExpr e))
        | ReadObjectAttributeExpr(e1, e2) ->
            fexpr (ReadObjectAttributeExpr(rewriteExpr e1, rewriteExpr e2))
        | GenerateRandomNumberExpr(e) ->
            fexpr (GenerateRandomNumberExpr(rewriteExpr e))

    let rec rewriteStatement fstmt fexpr stmt =
        let rewriteStmt = rewriteStatement fstmt fexpr
        let rewriteExpr = rewriteExpression fexpr

        match stmt with
        | LabelStmt(i) ->
            fstmt (LabelStmt(i))
        | ReturnStmt(e) ->
            fstmt (ReturnStmt(rewriteExpr e))
        | JumpStmt(i) ->
            fstmt (JumpStmt(i))
        | BranchStmt(b,e,s) ->
            fstmt (BranchStmt(b, rewriteExpr e, rewriteStmt s))
        | WriteTempStmt(i,e) ->
            fstmt (WriteTempStmt(i, rewriteExpr e))
        | WriteLocalStmt(i,e) ->
            fstmt (WriteLocalStmt(rewriteExpr i, rewriteExpr e))
        | WriteGlobalStmt(i,e) ->
            fstmt (WriteGlobalStmt(rewriteExpr i, rewriteExpr e))
        | StackPushStmt(e) ->
            fstmt (StackPushStmt(rewriteExpr e))
        | StackUpdateStmt(e) ->
            fstmt (StackUpdateStmt(rewriteExpr e))
        | WriteComputedVarStmt(i,e) ->
            fstmt (WriteComputedVarStmt(rewriteExpr i, rewriteExpr e))
        | WriteMemoryByteStmt(e1,e2) ->
            fstmt (WriteMemoryByteStmt(rewriteExpr e1, rewriteExpr e2))
        | WriteMemoryWordStmt(e1,e2) ->
            fstmt (WriteMemoryWordStmt(rewriteExpr e1, rewriteExpr e2))
        | PrintTextStmt(e) ->
            fstmt (PrintTextStmt(rewriteExpr e))
        | SetRandomNumberSeedStmt(e) ->
            fstmt (SetRandomNumberSeedStmt(rewriteExpr e))
        | RuntimeExceptionStmt(s) ->
            fstmt (RuntimeExceptionStmt(s))

    let rec rewriteTree fstmt fexpr tree : BoundTree =
        let rewriteStmt = rewriteStatement fstmt fexpr
        let newStatements = tree.Statements |> List.map rewriteStmt

        { Statements = newStatements; TempCount = tree.TempCount }

    let rec visitExpression fexpr expr =
        let visitExpr = visitExpression fexpr

        if fexpr expr then
            match expr with
            | ConstantExpr(_)
            | TempExpr(_)
            | StackPopExpr
            | StackPeekExpr ->
                ()
            | ReadLocalExpr(e)
            | ReadGlobalExpr(e)
            | ReadComputedVarExpr(e)
            | UnaryOperationExpr(_,e)
            | ConversionExpr(_,e)
            | ReadMemoryByteExpr(e)
            | ReadMemoryWordExpr(e)
            | ReadObjectNameExpr(e)
            | GenerateRandomNumberExpr(e) ->
                visitExpr e
            | BinaryOperationExpr(_,e1,e2)
            | ReadObjectAttributeExpr(e1, e2) ->
                visitExpr e1
                visitExpr e2
            | CallExpr(e,elist) ->
                visitExpr e
                elist |> List.iter visitExpr

    let rec visitStatement fstmt fexpr stmt =
        let visitStmt = visitStatement fstmt fexpr
        let visitExpr = visitExpression fexpr

        if fstmt stmt then
            match stmt with
            | LabelStmt(_)
            | JumpStmt(_)
            | RuntimeExceptionStmt(_) ->
                ()
            | ReturnStmt(e)
            | WriteTempStmt(_,e)
            | StackPushStmt(e)
            | StackUpdateStmt(e)
            | PrintTextStmt(e)
            | SetRandomNumberSeedStmt(e) ->
                visitExpr e
            | BranchStmt(_,e,s) ->
                visitExpr e
                visitStmt s
            | WriteLocalStmt(e1,e2)
            | WriteGlobalStmt(e1,e2)
            | WriteComputedVarStmt(e1,e2)
            | WriteMemoryByteStmt(e1,e2)
            | WriteMemoryWordStmt(e1,e2) ->
                visitExpr e1
                visitExpr e2

    let visitTree fstmt fexpr tree =
        let visitStmt = visitStatement fstmt fexpr
        tree.Statements |> List.iter visitStmt

    let walkExpression fexpr expr =
        expr |> visitExpression (fun e -> fexpr e; true)

    let walkStatement fstmt fexpr stmt =
        stmt |> visitStatement (fun s -> fstmt s; true) (fun e -> fexpr e; true)

    let walkTree fstmt fexpr tree =
        let walkStmt = walkStatement fstmt fexpr
        tree.Statements |> List.iter walkStmt

type BoundNodeDumper (builder : StringBuilder) =

    let indentLevel = ref 0
    let atLineStart = ref true

    let indent() = incr indentLevel
    let unindent() = decr indentLevel

    let indentIfNeeded() =
        if !atLineStart then
            let indentText = " " |> String.replicate (4 * !indentLevel)
            builder |> StringBuilder.appendString indentText
            atLineStart := false

    let append s =
        indentIfNeeded()
        builder |> StringBuilder.appendString s

    let appendf format =
        indentIfNeeded()
        builder |> StringBuilder.appendFormat format

    let newLine() =
        if builder.Length > 0 then
            builder |> StringBuilder.appendLineBreak
            atLineStart := true

    let lastChar() =
        if builder.Length > 0 then
            Some(builder.Chars(builder.Length - 1))
        else
            None

    let parenthesize f =
        match lastChar() with
        | Some('(')-> f()
        | _ ->
            append "("
            f()
            append ")"

    let dumpConstant = function
        | Byte(v)  -> appendf "%02x" v
        | Word(v)  -> appendf "%04x" v
        | Int32(v) -> appendf "%x" v
        | Text(v)  -> append (sprintf "\"%s\"" v |> String.replace "\n" "\\n")

    let dumpUnaryOperationKind = function
        | UnaryOperationKind.Negate -> append "-"
        | UnaryOperationKind.Not    -> append "not "
        | x -> Exceptions.invalidOperation "Unknown unary operator kind: %A" x

    let dumpBinaryOperationKind = function
        | BinaryOperationKind.Add         -> append " + "
        | BinaryOperationKind.Subtract    -> append " - "
        | BinaryOperationKind.Multiply    -> append " * "
        | BinaryOperationKind.Divide      -> append " / "
        | BinaryOperationKind.Remainder   -> append " % "
        | BinaryOperationKind.And         -> append " & "
        | BinaryOperationKind.Or          -> append " | "
        | BinaryOperationKind.ShiftLeft   -> append " << "
        | BinaryOperationKind.ShiftRight  -> append " >> "
        | BinaryOperationKind.Equal       -> append " = "
        | BinaryOperationKind.NotEqual    -> append " <> "
        | BinaryOperationKind.LessThan    -> append " < "
        | BinaryOperationKind.GreaterThan -> append " > "
        | BinaryOperationKind.AtMost      -> append " <= "
        | BinaryOperationKind.AtLeast     -> append " >= "
        | x -> Exceptions.invalidOperation "Unknown binary operator kind: %A" x

    let dumpConversionKind = function
        | ConversionKind.ToInt16 -> append "int16"
        | x -> Exceptions.invalidOperation "Unknown conversion kind: %A" x

    let rec dumpExpression = function
        | ConstantExpr(c) ->
            dumpConstant c
        | TempExpr(i) ->
            appendf "temp%02x" i
        | ReadLocalExpr(e) ->
            append "L"
            dumpExpression e
        | ReadGlobalExpr(e) ->
            append "G"
            dumpExpression e
        | StackPopExpr ->
            append "pop-SP"
        | StackPeekExpr ->
            append "peek-SP"
        | ReadComputedVarExpr(e) ->
            append "["
            dumpExpression e
            append "]"
        | UnaryOperationExpr(k,e) ->
            dumpUnaryOperationKind k
            dumpExpression e
        | BinaryOperationExpr(k,l,r) ->
            parenthesize (fun () ->
                dumpExpression l
                dumpBinaryOperationKind  k
                dumpExpression r)
        | ConversionExpr(k,e) ->
            dumpConversionKind k
            parenthesize(fun () ->
                dumpExpression e)
        | CallExpr(a,args) ->
            append "call "
            dumpExpression a
            append " "
            parenthesize (fun () ->
                args |> List.iteri (fun i v -> if i > 0 then append ", "
                                               dumpExpression v))
        | ReadMemoryByteExpr(e) ->
            append "read-byte"
            parenthesize (fun () ->
                dumpExpression e)
        | ReadMemoryWordExpr(e) ->
            append "read-byte"
            parenthesize (fun () ->
                dumpExpression e)
        | ReadObjectNameExpr(e) ->
            append "obj-name"
            parenthesize (fun () ->
                dumpExpression e)
        | ReadObjectAttributeExpr(o,a) ->
            append "obj-attribute"
            parenthesize (fun () ->
                dumpExpression o
                append ", "
                dumpExpression a)
        | GenerateRandomNumberExpr(e) ->
            append "random"
            parenthesize (fun () ->
                dumpExpression e)

    let rec dumpStatement s =
        newLine()

        match s with
        | LabelStmt(i) ->
            unindent()
            appendf "LABEL %02x" i
            indent()
        | ReturnStmt(e) ->
            append "return: "
            dumpExpression e
        | JumpStmt(i) ->
            appendf "jump-to: LABEL %02x" i
        | BranchStmt(b,e,s) ->
            append "if "
            parenthesize (fun () ->
                dumpExpression e)
            appendf " is %b then" b
            indent()
            dumpStatement s
            unindent()
        | WriteTempStmt(i,v) ->
            appendf "temp%02x <- " i
            dumpExpression v
        | WriteLocalStmt(i,v) ->
            append "L"
            dumpExpression i
            append " <- "
            dumpExpression v
        | WriteGlobalStmt(i,v) ->
            append "G"
            dumpExpression i
            append " <- "
            dumpExpression v
        | StackPushStmt(e) ->
            append "push-SP: "
            dumpExpression e
        | StackUpdateStmt(e) ->
            append "update-SP: "
            dumpExpression e
        | WriteComputedVarStmt(i,v) ->
            append "["
            dumpExpression i
            append "] <- "
            dumpExpression v
        | WriteMemoryByteStmt(a,v) ->
            append "write-byte"
            parenthesize (fun () ->
                dumpExpression a)
            append " <- "
            dumpExpression v
        | WriteMemoryWordStmt(a,v) ->
            append "write-word"
            parenthesize (fun () ->
                dumpExpression a)
            append " <- "
            dumpExpression v
        | PrintTextStmt(e) ->
            append "print: "
            dumpExpression e
        | SetRandomNumberSeedStmt(e) ->
            append "randomize"
            parenthesize (fun () ->
                dumpExpression e)
        | RuntimeExceptionStmt(s) ->
            appendf "RUNTIME EXCEPTION: %s" s

    member x.Dump tree =
        appendf "# temps: %d" tree.TempCount
        newLine()

        indent()
        for s in tree.Statements do
            dumpStatement s

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
        let elseLabel = builder.NewLabel()
        let doneLabel = builder.NewLabel()

        elseLabel |> builder.BranchIfFalse condition

        whenTrue()
        builder.JumpTo(doneLabel)

        builder.MarkLabel(elseLabel)
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
