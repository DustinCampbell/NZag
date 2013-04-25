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

    /// Performs the specified unary operation on the given expression
    | UnaryOperationExpr of UnaryOperationKind * Expression

    /// Performs the specifed binary operation on the given expressions
    | BinaryOperationExpr of BinaryOperationKind * Expression * Expression

    /// Calls the routine at the specified address with the list of given arguments
    | CallExpr of Expression * list<Expression>

type Statement =

    /// Declares a new temp with the specified index
    | DeclareTempStmt of int

    /// Assigns the temp with the specified index to the value of the given expression
    | AssignTempStmt of int * Expression

    /// Pushes the given statement onto the evaluation stack
    | StackPushStmt of Expression

module Visitors =

    let rec rewriteExpression fexpr expr =
        let rewriteExpr = rewriteExpression fexpr

        match expr with
        | ConstantExpr(c) ->
            fexpr (ConstantExpr(c))
        | TempExpr(i) ->
            fexpr (TempExpr(i))
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
        | DeclareTempStmt(i) ->
            fstmt (DeclareTempStmt(i))
        | AssignTempStmt(i,e) ->
            fstmt (AssignTempStmt(i, rewriteExpr e))
        | StackPushStmt(e) ->
            fstmt (StackPushStmt(rewriteExpr e))

    let rec visitExpression fexpr expr =
        let visitExpr = visitExpression fexpr

        if fexpr expr then
            match expr with
            | ConstantExpr(_)
            | TempExpr(_) ->
                ()
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
            | DeclareTempStmt(_) ->
                ()
            | AssignTempStmt(_,e)
            | StackPushStmt(e) ->
                visitExpr(e)

type BoundNodeDumper (?builder : StringBuilder) =

    let builder =
        match builder with
        | Some(b) -> b
        | None -> StringBuilder.create()

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
        builder |> StringBuilder.appendString s

    let appendf format =
        builder |> StringBuilder.appendFormat format

    let newLine() =
        builder |> StringBuilder.appendLineBreak
        atLineStart := true

    let dumpConstant = function
        | Byte(v)  -> appendf "%02x" v
        | Word(v)  -> appendf "%04x" v
        | Int32(v) -> appendf "%x" v
        | Text(v)  -> appendf "\"%s\"" v

    let dumpUnaryOperationKind = function
        | UnaryOperationKind.Negate -> append "-"
        | UnaryOperationKind.Not    -> append "!"
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
        | BinaryOperationKind.Equal       -> append " == "
        | BinaryOperationKind.NotEqual    -> append " != "
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
        | DeclareTempStmt(i) ->
            appendf "declare temp%02x" i
        | AssignTempStmt(i,e) ->
            appendf "temp%02x <- " i
            dumpExpression e
        | StackPushStmt(e) ->
            append "push-SP: "
            dumpExpression e

