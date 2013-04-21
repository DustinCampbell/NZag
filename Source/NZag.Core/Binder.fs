namespace NZag.Core

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

