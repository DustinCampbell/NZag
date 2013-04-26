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

type Expression =

    /// A constant value
    | ConstantExpr of Constant

    /// The value of the temp with the specified index
    | TempExpr of int

    /// Reads the local variable at the specified index
    | ReadLocalExpr of Expression

    // Reads the global variable at the specified index
    | ReadGlobalExpr of Expression

    /// Pops a value off of the VM stack and returns it.
    | StackPopExpr

    /// Returns the top value on the VM stack without popping it.
    | StackPeekExpr

    /// Performs the specified unary operation on the given expression
    | UnaryOperationExpr of UnaryOperationKind * Expression

    /// Performs the specifed binary operation on the given expressions
    | BinaryOperationExpr of BinaryOperationKind * Expression * Expression

    /// Calls the routine at the specified address with the list of given arguments
    | CallExpr of Expression * list<Expression>

type Statement =

    /// Declares a new label with the specified index
    | LabelStmt of int

    /// Declares a new temp with the specified index
    | DeclareTempStmt of int

    /// Assigns the temp with the specified index to the value of the given expression
    | AssignTempStmt of int * Expression

    /// Writes the given value to the local variable at the specified index.
    | WriteLocalStmt of Expression * Expression

    /// Writes the given value to the global variable at the specified index.
    | WriteGlobalStmt of Expression * Expression

    /// Pushes the given statement onto the evaluation stack
    | StackPushStmt of Expression

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

    let readVar v =
        match v with
        | StackVariable -> StackPopExpr
        | LocalVariable(i) -> ReadLocalExpr(byteConst i)
        | GlobalVariable(i) -> ReadGlobalExpr(byteConst i)

    let writeVar v e =
        match v with
        | StackVariable -> StackPushStmt(e)
        | LocalVariable(i) -> WriteLocalStmt(byteConst i, e)
        | GlobalVariable(i) -> WriteGlobalStmt(byteConst i, e)

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
        | CallExpr(address,argList) ->
            fexpr (CallExpr(rewriteExpr address, argList |> List.map rewriteExpr))

    let rec rewriteStatement fstmt fexpr stmt =
        let rewriteStmt = rewriteStatement fstmt fexpr
        let rewriteExpr = rewriteExpression fexpr

        match stmt with
        | LabelStmt(i) ->
            fstmt (LabelStmt(i))
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
            | UnaryOperationExpr(_,e) ->
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
            | DeclareTempStmt(_) ->
                ()
            | AssignTempStmt(_,e)
            | StackPushStmt(e) ->
                visitExpr(e)
            | WriteLocalStmt(e1,e2)
            | WriteGlobalStmt(e1,e2) ->
                visitExpr e1
                visitExpr e2

type BoundNodeDumper (?builder : StringBuilder) =

    let builder =
        match builder with
        | Some(b) -> b
        | None -> StringBuilder.create()

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
        builder |> StringBuilder.appendString s

    let appendf format =
        builder |> StringBuilder.appendFormat format

    let newLine() =
        if builder.Length > 0 then
            builder |> StringBuilder.appendLineBreak
            atLineStart := true

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
            append "("
            dumpExpression l
            dumpBinaryOperationKind  k
            dumpExpression r
            append ")"
        | CallExpr(a,args) ->
            append "call "
            dumpExpression a
            append " ("
            args |> List.iteri (fun i v -> if i > 0 then append ", "
                                           dumpExpression v)
            append ")"

    let rec dumpStatement s =
        newLine()

        match s with
        | LabelStmt(i) ->
            unindent()
            appendf "LABEL %02x" i
            indent()
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

open BoundNodeConstruction
open BoundNodeVisitors

type Binder private (routine : Routine) =

    let statements = new ResizeArray<_>()

    let labelMap =
        let targets = routine.JumpTargets
        let labels = targets |> Seq.mapi (fun i v -> i)
        Seq.zip targets labels |> Map.ofSeq

    let tempCount = ref 0

    let bindOperand = function
        | LargeConstantOperand(v) -> wordConst v
        | SmallConstantOperand(v) -> byteConst v
        | VariableOperand(v)      -> readVar v

    let bindInstruction (i : Instruction) =
        ()

    member x.Statements =
        statements |> List.ofSeq

    static member BindRoutine (routine : Routine) =
        let binder = new Binder(routine)
        { Statements = binder.Statements }
