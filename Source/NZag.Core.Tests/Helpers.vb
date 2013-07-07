Imports System.IO
Imports System.Reflection
Imports System.Text

Module Helpers

    Public Const Zork1 = "zork1.z3"
    Public Const CZech = "czech.z5"
    Public Const Advent = "Advent.z5"
    Public Const Count = "COUNT.Z5"

    Function GameMemory(name As String) As Memory
        Dim asm = Assembly.GetExecutingAssembly()
        Using stream = asm.GetManifestResourceStream(asm.GetName().Name + "." + name)
            Return Memory.CreateFrom(stream)
        End Using
    End Function

    Function CreateMemory(version As Byte, bytes As Byte()) As Memory
        Dim size As UInteger = CUInt(&H40 + bytes.Length)

        Dim multiplier As Integer
        If version >= 1 AndAlso version <= 3 Then
            multiplier = 2
        ElseIf version >= 4 AndAlso version <= 5 Then
            multiplier = 4
        Else ' 6, 7, 8
            multiplier = 8
        End If

        Dim packedSize As UShort = CUShort(size \ multiplier)
        If size Mod multiplier > 0 Then
            packedSize = CUShort(packedSize + 1)
        End If

        Using stream = New MemoryStream(CInt(size))
            ' Write version
            stream.Seek(0, SeekOrigin.Begin)
            stream.WriteByte(version)

            ' Write size
            stream.Seek(&H1A, SeekOrigin.Begin)
            stream.WriteByte(CByte(packedSize >> 8))
            stream.WriteByte(CByte(packedSize And &HFF))

            ' Write data
            stream.Seek(&H40, SeekOrigin.Begin)
            stream.Write(bytes, 0, bytes.Length)

            Return Memory.CreateFrom(stream)
        End Using
    End Function

    Function CreateMemory(version As Byte, dataSize As Integer) As Memory
        Return CreateMemory(version, New Byte(dataSize) {})
    End Function

    Function Instruction(address As Integer, ParamArray validators() As Action(Of Instruction)) As Action(Of Instruction)
        Return Sub(i)
                   ValidateInstruction(i, address, validators)
               End Sub
    End Function

    Sub ValidateInstruction(i As Instruction, address As Integer, ParamArray validators() As Action(Of Instruction))
        Assert.Equal(address, i.Address)

        For Each validator In validators
            validator(i)
        Next
    End Sub

    Function Opcode(name As String) As Action(Of Instruction)
        Return Sub(i)
                   Assert.Equal(name, i.Opcode.Name)
               End Sub
    End Function

    Function Text(value As String) As Action(Of Instruction)
        Return Sub(i)
                   Assert.Equal(value, i.Text.Value)
               End Sub
    End Function

    Function SmallConst(value As Byte) As Operand
        Return Operand.NewSmallConstantOperand(value)
    End Function

    Function LargeConst(value As UShort) As Operand
        Return Operand.NewLargeConstantOperand(value)
    End Function

    Public ReadOnly StackVar As Variable = Variable.StackVariable
    Public ReadOnly StackVarOp As Operand = Operand.NewVariableOperand(StackVar)

    Function LocalVar(index As Byte) As Variable
        Return Variable.NewLocalVariable(index)
    End Function

    Function LocalVarOp(index As Byte) As Operand
        Return Operand.NewVariableOperand(LocalVar(index))
    End Function

    Function GlobalVar(index As Byte) As Variable
        Return Variable.NewGlobalVariable(index)
    End Function

    Function GlobalVarOp(index As Byte) As Operand
        Return Operand.NewVariableOperand(GlobalVar(index))
    End Function

    Function Operands(ParamArray ops() As Operand) As Action(Of Instruction)
        Return Sub(i)
                   Assert.Equal(ops.Length, i.Operands.Length)

                   For j = 0 To ops.Length - 1
                       Assert.Equal(ops(j), i.Operands(j))
                   Next
               End Sub
    End Function

    Public ReadOnly NoOperands As Action(Of Instruction) = Sub(i) Assert.Equal(0, i.Operands.Length)

    Function RTrueBranch(condition As Boolean) As Action(Of Instruction)
        Return Sub(i)
                   Assert.True(i.Branch.Value.IsRTrueBranch)
                   Assert.Equal(condition, CType(i.Branch.Value, Branch.RTrueBranch).Item)
               End Sub
    End Function

    Function RFalseBranch(condition As Boolean) As Action(Of Instruction)
        Return Sub(i)
                   Assert.True(i.Branch.Value.IsRFalseBranch)
                   Assert.Equal(condition, CType(i.Branch.Value, Branch.RFalseBranch).Item)
               End Sub
    End Function

    Function OffsetBranch(condition As Boolean, offset As Short) As Action(Of Instruction)
        Return Sub(i)
                   Assert.True(i.Branch.Value.IsOffsetBranch)
                   Assert.Equal(condition, CType(i.Branch.Value, Branch.OffsetBranch).Item1)
                   Assert.Equal(offset, CType(i.Branch.Value, Branch.OffsetBranch).Item2)
               End Sub
    End Function

    Function Store(var As Variable) As Action(Of Instruction)
        Return Sub(i)
                   Assert.Equal(var, i.StoreVariable.Value)
               End Sub
    End Function

    Public Sub TestBinder(gameName As String, address As Integer, expected As XCData, Optional debugging As Boolean = False)
        Dim memory = GameMemory(gameName)
        Dim reader = New RoutineReader(memory)

        Dim r = reader.ReadRoutine(address)

        Assert.Equal(address, r.Address)

        Dim binder = New RoutineBinder(memory, debugging)
        Dim tree = binder.BindRoutine(r)
        Dim optimized = Optimization.optimize(tree)

        Dim builder = New StringBuilder()
        Dim dumper = New BoundNodeDumper(builder)
        dumper.Dump(optimized)

        Dim expectedText = expected.Value _
                                   .Trim() _
                                   .Replace(vbLf, vbCrLf)

        Assert.Equal(expectedText, builder.ToString())
    End Sub

End Module
