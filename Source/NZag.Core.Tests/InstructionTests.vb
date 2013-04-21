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

        ValidateInstruction(i, a, validators)
    End Sub

End Module
