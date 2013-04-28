namespace NZag.Core

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

    /// Performs the specified unary operation on the given expression
    | UnaryOperationExpr of UnaryOperationKind * Expression

    /// Performs the specifed binary operation on the given expressions
    | BinaryOperationExpr of BinaryOperationKind * Expression * Expression

    /// Converts the given expression to the type indicated by the conversion kind
    | ConversionExpr of ConversionKind * Expression

    /// Calls the routine at the specified address with the list of given arguments
    | CallExpr of Expression * list<Expression>

    /// Reads name of the object with the number represented by the given expression
    | ReadObjectNameExpr of Expression

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

    /// Declares a new temp with the specified index
    | DeclareTempStmt of int

    /// Assigns the temp with the specified index to the value of the given expression
    | AssignTempStmt of int * Expression

    /// Writes the given value to the local variable at the specified index
    | WriteLocalStmt of Expression * Expression

    /// Writes the given value to the global variable at the specified index
    | WriteGlobalStmt of Expression * Expression

    /// Pushes the given statement onto the evaluation stack
    | StackPushStmt of Expression

    /// Prints the text represented by the given expression
    | PrintTextStmt of Expression

    /// Seeds the random number generator with the given expression
    | SetRandomNumberSeedStmt of Expression

    /// Throws a runtime exception
    | RuntimeExceptionStmt of string

type BoundTree =
  { Statements : list<Statement> }

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
        | UnaryOperationExpr(k,e) ->
            fexpr (UnaryOperationExpr(k, rewriteExpr e))
        | BinaryOperationExpr(k,e1,e2) ->
            fexpr (BinaryOperationExpr(k, rewriteExpr e1, rewriteExpr e2))
        | ConversionExpr(k,e) ->
            fexpr (ConversionExpr(k, rewriteExpr e))
        | CallExpr(e,elist) ->
            fexpr (CallExpr(rewriteExpr e, elist |> List.map rewriteExpr))
        | ReadObjectNameExpr(e) ->
            fexpr (ReadObjectNameExpr(rewriteExpr e))
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
        | DeclareTempStmt(i) ->
            fstmt (DeclareTempStmt(i))
        | AssignTempStmt(i,e) ->
            fstmt (AssignTempStmt(i, rewriteExpr e))
        | WriteLocalStmt(i,e) ->
            fstmt (WriteLocalStmt(rewriteExpr i, rewriteExpr e))
        | WriteGlobalStmt(i,e) ->
            fstmt (WriteGlobalStmt(rewriteExpr i, rewriteExpr e))
        | StackPushStmt(e) ->
            fstmt (StackPushStmt(rewriteExpr e))
        | PrintTextStmt(e) ->
            fstmt (PrintTextStmt(rewriteExpr e))
        | SetRandomNumberSeedStmt(e) ->
            fstmt (SetRandomNumberSeedStmt(rewriteExpr e))
        | RuntimeExceptionStmt(s) ->
            fstmt (RuntimeExceptionStmt(s))

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
            | UnaryOperationExpr(_,e)
            | ConversionExpr(_,e)
            | ReadObjectNameExpr(e)
            | GenerateRandomNumberExpr(e) ->
                visitExpr e
            | BinaryOperationExpr(_,e1,e2) ->
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
            | DeclareTempStmt(_)
            | RuntimeExceptionStmt(_) ->
                ()
            | ReturnStmt(e)
            | AssignTempStmt(_,e)
            | StackPushStmt(e)
            | PrintTextStmt(e)
            | SetRandomNumberSeedStmt(e) ->
                visitExpr e
            | BranchStmt(_,e,s) ->
                visitExpr e
                visitStmt s
            | WriteLocalStmt(e1,e2)
            | WriteGlobalStmt(e1,e2) ->
                visitExpr e1
                visitExpr e2

type BoundNodeDumper (builder : StringBuilder) =

    let indentLevel = ref 1
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
        | Text(v)  -> appendf "\"%s\"" v

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
        | ReadObjectNameExpr(e) ->
            append "obj-name"
            parenthesize (fun () ->
                dumpExpression e)
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
            append "return "
            dumpExpression e
        | JumpStmt(i) ->
            appendf "jump-to LABEL %02x" i
        | BranchStmt(b,e,s) ->
            append "if "
            parenthesize (fun () ->
                dumpExpression e)
            appendf " is %b then" b
            indent()
            dumpStatement s
            unindent()
        | DeclareTempStmt(i) ->
            appendf "declare temp%02x" i
        | AssignTempStmt(i,e) ->
            appendf "temp%02x <- " i
            dumpExpression e
        | WriteLocalStmt(i,e) ->
            append "L"
            dumpExpression i
            append " <- "
            dumpExpression e
        | WriteGlobalStmt(i,e) ->
            append "G"
            dumpExpression i
            append " <- "
            dumpExpression e
        | StackPushStmt(e) ->
            append "push-SP: "
            dumpExpression e
        | PrintTextStmt(e) ->
            append "print "
            dumpExpression e
        | SetRandomNumberSeedStmt(e) ->
            append "randomize"
            parenthesize (fun () ->
                dumpExpression e)
        | RuntimeExceptionStmt(s) ->
            appendf "RUNTIME EXCEPTION: %s" s

    member x.Dump tree =
        for s in tree.Statements do
            dumpStatement s

open BoundNodeConstruction
open BoundNodeVisitors
open OperandPatterns

type InstructionBinder (memory : Memory, routine : Routine, statements : ResizeArray<_>) =

    let mutable nextLabelIndex = 0

    let newLabel() =
        let index = nextLabelIndex
        nextLabelIndex <- nextLabelIndex + 1
        index

    let labelMap =
        let d = Dictionary.create()
        for a in routine.JumpTargets do
            d.Add(a, newLabel())
        d

    let addLabel address =
        let index = newLabel()
        labelMap.Add(address, index)
        index

    let tryFindLabel address =
        labelMap |> Dictionary.tryFind address

    let findLabel address =
        labelMap |> Dictionary.find address

    let mutable tempCount = 0

    let newTemp() =
        let index = tempCount
        tempCount <- tempCount + 1
        index

    let addStatement s =
        statements.Add(s)

    let bindOperand = function
        | LargeConstantOperand(v) -> wordConst v
        | SmallConstantOperand(v) -> byteConst v
        | VariableOperand(v)      -> readVar v

    let ret expression =
        ReturnStmt(expression) |> addStatement

    let jumpTo label =
        JumpStmt(label) |> addStatement

    let branchTo condition label =
        BranchStmt(false, condition, JumpStmt(label)) |> addStatement

    let mark label =
        LabelStmt(label) |> addStatement

    let declareTemp t = 
        DeclareTempStmt(t) |> addStatement

    let assignTemp t v =
        AssignTempStmt(t, v) |> addStatement

    let initTemp expression =
        let t = newTemp()
        declareTemp(t)
        assignTemp t expression
        TempExpr(t)

    let ifThenElse condition whenTrue whenFalse =
        let elseLabel = newLabel()
        let doneLabel = newLabel()

        branchTo condition elseLabel

        whenTrue()
        jumpTo doneLabel

        mark elseLabel
        whenFalse()

        mark doneLabel

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
                | OffsetBranch(_,_) -> JumpStmt(findLabel instruction.BranchAddress.Value)

            BranchStmt(branch.Condition, expression, statement) |> addStatement

        let store expression =
            let storeVar =
                match instruction.StoreVariable with
                | Some(v) -> v
                | None -> invalidOperation "Expected instruction to have a valid store variable."

            storeVar |> writeVar expression |> addStatement

        // Add a label statement if necessary
        match tryFindLabel instruction.Address with
        | Some(l) -> mark l
        | None -> ()

        // Create temps for all operands
        let operands = instruction.Operands
        let operandTemps = operands |> List.map (fun _ -> newTemp())
        let operandTempExprs = operandTemps |> List.map (fun t -> TempExpr(t))
        let operandValues = operands |> List.map bindOperand
        let operandMap = List.zip operandTemps operandValues |> Map.ofList

        List.iter2
            (fun t v ->
                declareTemp t
                assignTemp t v)
            operandTemps
            operandValues

        // Bind the instruction
        match (instruction.Opcode.Name, instruction.Opcode.Version, operandTempExprs) with
        | "jg", Any, Op2(left, right) ->
            let left = initTemp (left |> toInt16)
            let right = initTemp (right |> toInt16)

            branchIf (left .>. right)

        | "jl", Any, Op2(left, right) ->
            let left = initTemp (left |> toInt16)
            let right = initTemp (right |> toInt16)

            branchIf (left .<. right)

        | "jz", Any, Op1(left) ->
            branchIf (left .=. zero)

        | "print", Any, NoOps ->
            textConst instruction.Text.Value |> printText |> addStatement

        | "print_obj", Any, Op1(objNum) ->
            objectName objNum |> printText |> addStatement

        | "random", Any, Op1(range) ->
            let range = initTemp (range |> toInt16)

            ifThenElse (range .>. zero)
                (fun () ->
                    store (random range))
                (fun () ->
                    randomize range |> addStatement
                    store zero)

        | "rfalse", Any, NoOps ->
            ret zero

        | "rtrue", Any, NoOps ->
            ret one

        | (n,k,ops) ->
            runtimeException "Unsupported opcode: %s (v.%d) with %d operands" n k ops.Length |> addStatement

type RoutineBinder (memory : Memory) =

    let normalizeLabels tree =
        tree

    member x.BindRoutine (routine : Routine) =

        let statements = new ResizeArray<_>()
        let binder = new InstructionBinder(memory, routine, statements)

        for i in routine.Instructions do
            binder.BindInstruction(i)

        { Statements = statements |> List.ofSeq }
