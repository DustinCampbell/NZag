Public Module InstructionTests

    <Fact>
    Sub Zork1_4E3B()
        ' 4e3b: b2 ...  PRINT  "a "
        Test(Zork1, &H4E3B,
             Opcode("print"),
             Text("a "))
    End Sub

    <Fact>
    Sub Zork1_4E3E()
        ' 4e3e: aa 01  PRINT_OBJ  L00
        Test(Zork1, &H4E3E,
             Opcode("print_obj"),
             Operands(LocalVarOp(0)))
    End Sub

    <Fact>
    Sub Zork1_4E40()
        ' 4e40: b0  RTRUE
        Test(Zork1, &H4E40,
             Opcode("rtrue"),
             NoOperands)
    End Sub

    <Fact>
    Sub Zork1_4E45()
        ' 4e45: a0 4c cb  JZ  G3c [TRUE] 4e51
        Test(Zork1, &H4E45,
             Opcode("jz"),
             Operands(GlobalVarOp(&H3C)),
             OffsetBranch(True, 11))
    End Sub

    <Fact>
    Sub Zork1_4E48()
        ' 4e48: e7 7f 64 00  RANDOM  #64 -> -(SP)
        Test(Zork1, &H4E48,
             Opcode("random"),
             Operands(SmallConst(&H64)),
             Store(StackVar))
    End Sub

    <Fact>
    Sub Zork1_4E4c()
        ' 4e4c: 63 01 00 c1  JG  L00,(SP)+ [TRUE] RTRUE
        Test(Zork1, &H4E4C,
             Opcode("jg"),
             Operands(LocalVarOp(0), StackVarOp),
             RTrueBranch(True))
    End Sub

    <Fact>
    Sub Zork1_4E50()
        ' 4e50: b1  RFALSE
        Test(Zork1, &H4E50,
             Opcode("rfalse"),
             NoOperands)
    End Sub

    Private Sub Test(gameName As String, address As Integer, ParamArray validators() As Action(Of Instruction))
        Dim memory = GameMemory(gameName)
        Dim reader = New InstructionReader(memory)

        Dim a = RawAddress(address)
        Dim i = reader.ReadInstruction(a)

        Assert.Equal(a, i.Address)

        For Each validator In validators
            validator(i)
        Next
    End Sub

    Private Function RawAddress(value As Integer) As Address
        Return Address.NewRawAddress(value)
    End Function

    Private Function Opcode(name As String) As Action(Of Instruction)
        Return Sub(i)
                   Assert.Equal(name, i.Opcode.Name)
               End Sub
    End Function

    Private Function Text(value As String) As Action(Of Instruction)
        Return Sub(i)
                   Assert.Equal(value, i.Text.Value)
               End Sub
    End Function

    Private Function SmallConst(value As Byte) As Operand
        Return Operand.NewSmallConstantOperand(value)
    End Function

    Private Function LargeConst(value As UShort) As Operand
        Return Operand.NewLargeConstantOperand(value)
    End Function

    Private ReadOnly StackVar As Variable = Variable.StackVariable
    Private ReadOnly StackVarOp As Operand = Operand.NewVariableOperand(StackVar)

    Private Function LocalVar(index As Byte) As Variable
        Return Variable.NewLocalVariable(index)
    End Function

    Private Function LocalVarOp(index As Byte) As Operand
        Return Operand.NewVariableOperand(LocalVar(index))
    End Function

    Private Function GlobalVar(index As Byte) As Variable
        Return Variable.NewGlobalVariable(index)
    End Function

    Private Function GlobalVarOp(index As Byte) As Operand
        Return Operand.NewVariableOperand(GlobalVar(index))
    End Function

    Private Function Operands(ParamArray ops() As Operand) As Action(Of Instruction)
        Return Sub(i)
                   Assert.Equal(ops.Length, i.Operands.Length)

                   For j = 0 To ops.Length - 1
                       Assert.Equal(ops(j), i.Operands(j))
                   Next
               End Sub
    End Function

    Private ReadOnly NoOperands As Action(Of Instruction) = Sub(i) Assert.Equal(0, i.Operands.Length)

    Private Function RTrueBranch(condition As Boolean) As Action(Of Instruction)
        Return Sub(i)
                   Assert.True(i.Branch.Value.IsRTrueBranch)
                   Assert.Equal(condition, CType(i.Branch.Value, Branch.RTrueBranch).Item)
               End Sub
    End Function

    Private Function RFalseBranch(condition As Boolean) As Action(Of Instruction)
        Return Sub(i)
                   Assert.True(i.Branch.Value.IsRFalseBranch)
                   Assert.Equal(condition, CType(i.Branch.Value, Branch.RFalseBranch).Item)
               End Sub
    End Function

    Private Function OffsetBranch(condition As Boolean, offset As Short) As Action(Of Instruction)
        Return Sub(i)
                   Assert.True(i.Branch.Value.IsOffsetBranch)
                   Assert.Equal(condition, CType(i.Branch.Value, Branch.OffsetBranch).Item1)
                   Assert.Equal(offset, CType(i.Branch.Value, Branch.OffsetBranch).Item2)
               End Sub
    End Function

    Private Function Store(var As Variable) As Action(Of Instruction)
        Return Sub(i)
                   Assert.Equal(var, i.StoreVariable.Value)
               End Sub
    End Function

End Module
