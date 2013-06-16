Public Module RoutineTests

    <Fact>
    Sub Zork1_4E3B()
        ' 4e3b:  b2 ...                  PRINT           "a "
        ' 4e3e:  aa 01                   PRINT_OBJ       L00
        ' 4e40:  b0                      RTRUE

        Test(Zork1, &H4E38, {0},
             Instruction(&H4E3B, Opcode("print"), Text("a ")),
             Instruction(&H4E3E, Opcode("print_obj"), Operands(LocalVarOp(0))),
             Instruction(&H4E40, Opcode("rtrue"), NoOperands))
    End Sub

    <Fact>
    Sub Zork1_4E42()
        ' 4e45:  a0 4c cb                JZ              G3c [TRUE] 4e51
        ' 4e48:  e7 7f 64 00             RANDOM          #64 -> -(SP)
        ' 4e4c:  63 01 00 c1             JG              L00,(SP)+ [TRUE] RTRUE
        ' 4e50:  b1                      RFALSE
        ' 4e51:  e7 3f 01 2c 00          RANDOM          #012c -> -(SP)
        ' 4e56:  63 01 00 c1             JG              L00,(SP)+ [TRUE] RTRUE
        ' 4e5a:  b1                      RFALSE

        Test(Zork1, &H4E42, {0},
             Instruction(&H4E45, Opcode("jz"), Operands(GlobalVarOp(&H3C)), OffsetBranch(True, 11)),
             Instruction(&H4E48, Opcode("random"), Operands(SmallConst(&H64)), Store(StackVar)),
             Instruction(&H4E4C, Opcode("jg"), Operands(LocalVarOp(0), StackVarOp), RTrueBranch(True)),
             Instruction(&H4E50, Opcode("rfalse"), NoOperands),
             Instruction(&H4E51, Opcode("random"), Operands(LargeConst(&H12C)), Store(StackVar)),
             Instruction(&H4E56, Opcode("jg"), Operands(LocalVarOp(0), StackVarOp), RTrueBranch(True)),
             Instruction(&H4E5A, Opcode("rfalse"), NoOperands))
    End Sub

    Private Sub Test(gameName As String, address As Integer, locals() As UShort, ParamArray instructions() As Action(Of Instruction))
        Dim memory = GameMemory(gameName)
        Dim reader = New RoutineReader(memory)

        Dim r = reader.ReadRoutine(address)

        Assert.Equal(address, r.Address)

        Assert.Equal(locals.Length, r.Locals.Length)
        For i = 0 To locals.Length - 1
            Assert.Equal(locals(i), r.Locals(i))
        Next

        Assert.Equal(instructions.Length, r.Instructions.Length)
        For i = 0 To instructions.Length - 1
            instructions(i)(r.Instructions(i))
        Next
    End Sub

End Module
