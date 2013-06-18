namespace NZag.Core

open System.Text
open NZag.Utilities

type Constant =
    | Byte of byte
    | Word of uint16
    | Int32 of int32
    | Text of string

[<AutoOpen>]
module ConstantPatterns =

    let (|Int32Value|_|) = function
        | Byte(v) -> Some(int v)
        | Word(v) -> Some(int v)
        | Int32(v) -> Some(v)
        | _ -> None

    let (|Zero|_|) = function
        | Byte(0uy) -> Some()
        | Word(0us) -> Some()
        | Int32(0) -> Some()
        | _ -> None

    let (|One|_|) = function
        | Byte(1uy) -> Some()
        | Word(1us) -> Some()
        | Int32(1) -> Some()
        | _ -> None

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

[<AutoOpen>]
module BinaryOperationKindPatterns =

    let (|IsArithmetic|_|) = function
        | BinaryOperationKind.Add
        | BinaryOperationKind.Subtract
        | BinaryOperationKind.Multiply
        | BinaryOperationKind.Divide
        | BinaryOperationKind.Remainder -> Some()
        | _ -> None

    let (|IsLogical|_|) = function
        | BinaryOperationKind.Equal
        | BinaryOperationKind.NotEqual
        | BinaryOperationKind.LessThan
        | BinaryOperationKind.AtMost
        | BinaryOperationKind.AtLeast
        | BinaryOperationKind.GreaterThan -> Some()
        | _ -> None

    let (|IsBitwise|_|) = function
        | BinaryOperationKind.And
        | BinaryOperationKind.Or
        | BinaryOperationKind.ShiftLeft
        | BinaryOperationKind.ShiftRight -> Some()
        | _ -> None

type ConversionKind =
    | ToByte = 1
    | ToInt16 = 2
    | ToUInt16 = 3

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

    /// Converts the number represents by the given expression to text
    | NumberToTextExpr of Expression

    /// Calls the routine at the specified address with the list of given arguments
    | CallExpr of Expression * list<Expression>

    /// Returns the number of arguments passed to the current routine
    | ArgCountExpr

    /// Reads a byte from game memory at the specified address
    | ReadMemoryByteExpr of Expression

    /// Reads a word from game memory at the specified address
    | ReadMemoryWordExpr of Expression

    /// Reads the text from game memory at the specified address
    | ReadMemoryTextExpr of Expression

    /// Reads the text from game memory at the specified address with the given length
    | ReadMemoryTextOfLengthExpr of Expression * Expression

    | ReadObjectNextPropertyAddressExpr of Expression
    | ReadObjectPropertyDefaultExpr of Expression

    /// Generates a random number using the given range expression
    | GenerateRandomNumberExpr of Expression

    /// Reads user char input.
    | ReadInputCharExpr

    /// Reads user text input using the specified addresses for storing text and parse results respectively.
    | ReadInputTextExpr of Expression * Expression
    
    /// Reads timed user char input.
    | ReadTimedInputCharExpr of Expression * Expression

    /// Reads timed user text input using the specified addresses for storing text and parse results respectively.
    | ReadTimedInputTextExpr of Expression * Expression * Expression * Expression

    | VerifyExpr

type Statement =

    /// Declares a new label with the specified index
    | LabelStmt of int

    /// Returns the given expression
    | ReturnStmt of Expression

    /// Jumps to the specified label
    | JumpStmt of int

    /// Executes the specified statement if the given expression evaluates to the given condition
    | BranchStmt of bool * Expression * Statement

    /// Quits by throwing a ZMachineQuitException
    | QuitStmt

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

    /// Write the given value to the variable whose index is computed by the specified
    /// expression. Note that computed variables are always write indirectly. That is, stack
    /// writes are updates rather than pushes.
    | WriteComputedVarStmt of Expression * Expression

    /// Writes a byte from game memory at the specified address
    | WriteMemoryByteStmt of Expression * Expression

    /// Writes a word from game memory at the specified address
    | WriteMemoryWordStmt of Expression * Expression

    /// Discards a value (i.e. does nothing with it).
    | DiscardValueStmt of Expression
    
    /// Seeds the random number generator with the given expression
    | SetRandomNumberSeedStmt of Expression

    /// Selects the given output stream
    | SelectOutputStreamStmt of Expression

    /// Selects the given memory output stream
    | SelectMemoryOutputStreamStmt of Expression * Expression

    /// Prints the char represented by the given expression to the active window
    | PrintCharStmt of Expression

    /// Prints the text represented by the given expression to the active window
    | PrintTextStmt of Expression

    /// Sets the text style of the screen
    | SetTextStyleStmt of Expression

    /// Sets the foreground and background colors of the screen
    | SetColorsStmt of Expression * Expression

    /// Sets the active window
    | SetWindowStmt of Expression

    /// Clears the active window
    | ClearWindowStmt of Expression

    /// Splits the active window into two
    | SplitWindowStmt of Expression

    /// Moves the cursor in the active window to the given line and column
    | SetCursorStmt of Expression * Expression

    /// Updates the status bar
    | ShowStatusStmt

    /// Outputs the text represented by the given expression for debugging
    | DebugOutputStmt of Expression * list<Expression>

    /// Throws a runtime exception
    | RuntimeExceptionStmt of string

type BoundTree =
  { Statements : list<Statement>
    TempCount : int
    LabelCount : int
    Routine : Routine }

module BoundNodeConstruction =

    let unaryOp e k = UnaryOperationExpr(k, e)
    let binaryOp l r k = BinaryOperationExpr(k, l, r)

    let inline negate e = UnaryOperationKind.Negate |> unaryOp e
    let inline bitNot e = UnaryOperationKind.Not |> unaryOp e

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
    let nine = int32Const 9
    let ten = int32Const 10
    let eleven = int32Const 11
    let twelve = int32Const 12
    let thirteen = int32Const 13
    let fourteen = int32Const 14
    let fifteen = int32Const 15
    let sixteen = int32Const 16

    let toByte v = ConversionExpr(ConversionKind.ToByte, v)
    let toInt16 v = ConversionExpr(ConversionKind.ToInt16, v)
    let toUInt16 v = ConversionExpr(ConversionKind.ToUInt16, v)
    let numberToText v = NumberToTextExpr(v)

    let readText a = ReadMemoryTextExpr(a)
    let readTextOfLength a l = ReadMemoryTextOfLengthExpr(a, l)

    let random range = GenerateRandomNumberExpr(range)
    let randomize seed = SetRandomNumberSeedStmt(seed)

    let debugOut s args =
        DebugOutputStmt(textConst s, args)

[<AutoOpen>]
module ConversionPatterns =

    let (|ToInt16|_|) = function
        | ConversionExpr(ConversionKind.ToInt16, v) -> Some(v)
        | _ -> None

[<AutoOpen>]
module ConstantExprPatterns =

    let (|ByteConst|_|) = function
        | ConstantExpr(Byte(b)) -> Some(b)
        | _ -> None

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
        | NumberToTextExpr(e) ->
            fexpr (NumberToTextExpr(rewriteExpr e))
        | CallExpr(e,elist) ->
            fexpr (CallExpr(rewriteExpr e, elist |> List.map rewriteExpr))
        | ArgCountExpr ->
            fexpr ArgCountExpr
        | ReadMemoryByteExpr(e) ->
            fexpr (ReadMemoryByteExpr(rewriteExpr e))
        | ReadMemoryWordExpr(e) ->
            fexpr (ReadMemoryWordExpr(rewriteExpr e))
        | ReadMemoryTextExpr(e) ->
            fexpr (ReadMemoryTextExpr(rewriteExpr e))
        | ReadMemoryTextOfLengthExpr(e,l) ->
            fexpr (ReadMemoryTextOfLengthExpr(rewriteExpr e, rewriteExpr l))
        | ReadObjectNextPropertyAddressExpr(e) ->
            fexpr (ReadObjectNextPropertyAddressExpr(rewriteExpr e))
        | ReadObjectPropertyDefaultExpr(e) ->
            fexpr (ReadObjectPropertyDefaultExpr(rewriteExpr e))
        | GenerateRandomNumberExpr(e) ->
            fexpr (GenerateRandomNumberExpr(rewriteExpr e))
        | ReadInputCharExpr ->
            fexpr ReadInputCharExpr
        | ReadInputTextExpr(e1,e2) ->
            fexpr (ReadInputTextExpr(rewriteExpr e1, rewriteExpr e2))
        | ReadTimedInputCharExpr(e1,e2) ->
            fexpr (ReadTimedInputCharExpr(rewriteExpr e1, rewriteExpr e2))
        | ReadTimedInputTextExpr(e1,e2,e3,e4) ->
            fexpr (ReadTimedInputTextExpr(rewriteExpr e1, rewriteExpr e2, rewriteExpr e3, rewriteExpr e4))
        | VerifyExpr ->
            fexpr VerifyExpr

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
        | QuitStmt ->
            fstmt (QuitStmt)
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
        | DiscardValueStmt(e) ->
            fstmt (DiscardValueStmt(rewriteExpr e))
        | SetRandomNumberSeedStmt(e) ->
            fstmt (SetRandomNumberSeedStmt(rewriteExpr e))
        | SelectOutputStreamStmt(e) ->
            fstmt (SelectOutputStreamStmt(rewriteExpr e))
        | SelectMemoryOutputStreamStmt(e1,e2) ->
            fstmt (SelectMemoryOutputStreamStmt(rewriteExpr e1, rewriteExpr e2))
        | PrintCharStmt(e) ->
            fstmt (PrintCharStmt(rewriteExpr e))
        | PrintTextStmt(e) ->
            fstmt (PrintTextStmt(rewriteExpr e))
        | SetTextStyleStmt(e) ->
            fstmt (SetTextStyleStmt(rewriteExpr e))
        | SetColorsStmt(e1,e2) ->
            fstmt (SetColorsStmt(rewriteExpr e1, rewriteExpr e2))
        | SetWindowStmt(e) ->
            fstmt (SetWindowStmt(rewriteExpr e))
        | ClearWindowStmt(e) ->
            fstmt (ClearWindowStmt(rewriteExpr e))
        | SplitWindowStmt(e) ->
            fstmt (SplitWindowStmt(rewriteExpr e))
        | SetCursorStmt(e1,e2) ->
            fstmt (SetCursorStmt(rewriteExpr e1, rewriteExpr e2))
        | ShowStatusStmt ->
            fstmt (ShowStatusStmt)
        | DebugOutputStmt(e, elist) ->
            fstmt (DebugOutputStmt(rewriteExpr e, elist |> List.map rewriteExpr))
        | RuntimeExceptionStmt(s) ->
            fstmt (RuntimeExceptionStmt(s))

    let rec rewriteTree fstmt fexpr tree : BoundTree =
        let rewriteStmt = rewriteStatement fstmt fexpr
        let newStatements = tree.Statements |> List.map rewriteStmt

        { Statements = newStatements; TempCount = tree.TempCount; LabelCount = tree.LabelCount; Routine = tree.Routine }

    let rec visitExpression fexpr expr =
        let visitExpr = visitExpression fexpr

        if fexpr expr then
            match expr with
            | ConstantExpr(_)
            | TempExpr(_)
            | StackPopExpr
            | StackPeekExpr
            | ArgCountExpr
            | ReadInputCharExpr
            | VerifyExpr ->
                ()
            | ReadLocalExpr(e)
            | ReadGlobalExpr(e)
            | ReadComputedVarExpr(e)
            | UnaryOperationExpr(_,e)
            | ConversionExpr(_,e)
            | NumberToTextExpr(e)
            | ReadMemoryByteExpr(e)
            | ReadMemoryWordExpr(e)
            | ReadMemoryTextExpr(e)
            | ReadObjectNextPropertyAddressExpr(e)
            | ReadObjectPropertyDefaultExpr(e)
            | GenerateRandomNumberExpr(e) ->
                visitExpr e
            | BinaryOperationExpr(_,e1,e2)
            | ReadMemoryTextOfLengthExpr(e1,e2)
            | ReadInputTextExpr(e1,e2)
            | ReadTimedInputCharExpr(e1,e2) ->
                visitExpr e1
                visitExpr e2
            | ReadTimedInputTextExpr(e1,e2,e3,e4) ->
                visitExpr e1
                visitExpr e2
                visitExpr e3
                visitExpr e4
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
            | QuitStmt
            | ShowStatusStmt
            | RuntimeExceptionStmt(_) ->
                ()
            | ReturnStmt(e)
            | WriteTempStmt(_,e)
            | StackPushStmt(e)
            | StackUpdateStmt(e)
            | DiscardValueStmt(e)
            | SetRandomNumberSeedStmt(e)
            | SelectOutputStreamStmt(e)
            | PrintCharStmt(e)
            | PrintTextStmt(e)
            | SetTextStyleStmt(e)
            | SetWindowStmt(e)
            | ClearWindowStmt(e)
            | SplitWindowStmt(e) ->
                visitExpr e
            | BranchStmt(_,e,s) ->
                visitExpr e
                visitStmt s
            | WriteLocalStmt(e1,e2)
            | WriteGlobalStmt(e1,e2)
            | WriteComputedVarStmt(e1,e2)
            | WriteMemoryByteStmt(e1,e2)
            | WriteMemoryWordStmt(e1,e2)
            | SelectMemoryOutputStreamStmt(e1,e2)
            | SetColorsStmt(e1,e2)
            | SetCursorStmt(e1,e2) ->
                visitExpr e1
                visitExpr e2
            | DebugOutputStmt(e, elist) ->
                visitExpr e
                elist |> List.iter visitExpr

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

[<Struct>]
type Temp(temp: int, builder: BoundTreeBuilder) =

    member x.Read = TempExpr(temp)
    member x.Write value = WriteTempStmt(temp, value) |> builder.AddStatement

    static member (!!) (m: Temp) = m.Read
    static member (<--) (m: Temp, v: Expression) =
        m.Write(v)

and [<AbstractClass>] BoundTreeBuilder() =

    member x.InitTemp value =
        let temp = x.NewTemp()
        let result = new Temp(temp, x)
        result <-- value
        result

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
        { Statements = statements |> Seq.toList; TempCount = tempCount; LabelCount = labelCount; Routine = routine }

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
        { Statements = statements |> Seq.toList; TempCount = tempCount; LabelCount = labelCount; Routine = tree.Routine }

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
        | x -> failcompilef "Unknown unary operator kind: %A" x

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
        | x -> failcompilef "Unknown binary operator kind: %A" x

    let dumpConversionKind = function
        | ConversionKind.ToByte -> append "byte"
        | ConversionKind.ToInt16 -> append "int16"
        | ConversionKind.ToUInt16 -> append "uint16"
        | x -> failcompilef "Unknown conversion kind: %A" x

    let rec dumpExpression = function
        | ConstantExpr(c) ->
            dumpConstant c
        | TempExpr(i) ->
            appendf "temp%02x" i
        | ReadLocalExpr(i) ->
            append "L"
            match i with
            | ConstantExpr(_) -> dumpExpression i
            | _ -> parenthesize (fun () -> dumpExpression i)
        | ReadGlobalExpr(i) ->
            append "G"
            match i with
            | ConstantExpr(_) -> dumpExpression i
            | _ -> parenthesize (fun () -> dumpExpression i)
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

            let parenthesizeIfBinaryOp e =
                match e with
                | BinaryOperationExpr(_,_,_) -> 
                    append "("
                    dumpExpression e
                    append ")"
                | _ -> dumpExpression e

            parenthesize (fun () ->
                parenthesizeIfBinaryOp l
                dumpBinaryOperationKind  k
                parenthesizeIfBinaryOp r)
        | ConversionExpr(k,e) ->
            dumpConversionKind k
            parenthesize(fun () ->
                dumpExpression e)
        | NumberToTextExpr(e) ->
            append "number-to-text"
            parenthesize (fun () ->
                dumpExpression e)
        | CallExpr(a,args) ->
            append "call "
            dumpExpression a
            append " "
            parenthesize (fun () ->
                args |> List.iteri (fun i v -> if i > 0 then append ", "
                                               dumpExpression v))
        | ArgCountExpr ->
            append "arg-count"
        | ReadMemoryByteExpr(e) ->
            append "read-byte"
            parenthesize (fun () ->
                dumpExpression e)
        | ReadMemoryWordExpr(e) ->
            append "read-word"
            parenthesize (fun () ->
                dumpExpression e)
        | ReadMemoryTextExpr(e) ->
            append "read-text"
            parenthesize (fun () ->
                dumpExpression e)
        | ReadMemoryTextOfLengthExpr(e,l) ->
            append "read-text"
            parenthesize (fun () ->
                dumpExpression e
                append ", "
                dumpExpression l)
        | ReadObjectNextPropertyAddressExpr(e) ->
            append "obj-next-property-address"
            parenthesize (fun () ->
                dumpExpression e)
        | ReadObjectPropertyDefaultExpr(e) ->
            append "obj-property-default"
            parenthesize (fun () ->
                dumpExpression e)
        | GenerateRandomNumberExpr(e) ->
            append "random"
            parenthesize (fun () ->
                dumpExpression e)
        | ReadInputCharExpr ->
            append "read-input-char"
        | ReadInputTextExpr(e1,e2) ->
            append "read-input-text"
            parenthesize (fun () ->
                dumpExpression e1
                append ", "
                dumpExpression e2)
        | ReadTimedInputCharExpr(e1,e2) ->
            append "read-timed-input-char"
            parenthesize (fun () ->
                dumpExpression e1
                append ", "
                dumpExpression e2)
        | ReadTimedInputTextExpr(e1,e2,e3,e4) ->
            append "read-timed-input-text"
            parenthesize (fun () ->
                dumpExpression e1
                append ", "
                dumpExpression e2
                append ", "
                dumpExpression e3
                append ", "
                dumpExpression e4)
        | VerifyExpr ->
            append "verify"

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
        | QuitStmt ->
            append "quit"
        | WriteTempStmt(i,v) ->
            appendf "temp%02x <- " i
            dumpExpression v
        | WriteLocalStmt(i,v) ->
            append "L"
            match i with
            | ConstantExpr(_) -> dumpExpression i
            | _ -> parenthesize (fun () -> dumpExpression i)
            append " <- "
            dumpExpression v
        | WriteGlobalStmt(i,v) ->
            append "G"
            match i with
            | ConstantExpr(_) -> dumpExpression i
            | _ -> parenthesize (fun () -> dumpExpression i)
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
        | DiscardValueStmt(e) ->
            append "discard: "
            dumpExpression e
        | SetRandomNumberSeedStmt(e) ->
            append "randomize"
            parenthesize (fun () ->
                dumpExpression e)
        | SelectOutputStreamStmt(e) ->
            append "select-output-stream"
            parenthesize (fun () ->
                dumpExpression e)
        | SelectMemoryOutputStreamStmt(number, table) ->
            append "select-output-stream"
            parenthesize (fun () ->
                dumpExpression number
                append ", "
                dumpExpression table)
        | PrintCharStmt(e)
        | PrintTextStmt(e) ->
            append "print: "
            dumpExpression e
        | SetTextStyleStmt(e) ->
            append "set-text-style"
            parenthesize (fun () ->
                dumpExpression e)
        | SetColorsStmt(foreground, background) ->
            append "set-colors"
            parenthesize (fun () ->
                dumpExpression foreground
                append ", "
                dumpExpression background)
        | SetWindowStmt(e) ->
            append "set-window"
            parenthesize (fun () ->
                dumpExpression e)
        | ClearWindowStmt(e) ->
            append "clear-window"
            parenthesize (fun () ->
                dumpExpression e)
        | SplitWindowStmt(e) ->
            append "split-window"
            parenthesize (fun () ->
                dumpExpression e)
        | SetCursorStmt(line,column) ->
            append "set-cursor"
            parenthesize (fun () ->
                dumpExpression line
                append ", "
                dumpExpression column)
        | ShowStatusStmt ->
            append "show-status"
        | RuntimeExceptionStmt(s) ->
            appendf "RUNTIME EXCEPTION: %s" s
        | DebugOutputStmt(e, args) ->
            append "DEBUG: "
            dumpExpression e
            append " "
            parenthesize (fun () ->
                args |> List.iteri (fun i v -> if i > 0 then append ", "
                                               dumpExpression v))

    member x.Dump tree =
        appendf "# temps: %d" tree.TempCount
        newLine()

        indent()
        for s in tree.Statements do
            dumpStatement s
