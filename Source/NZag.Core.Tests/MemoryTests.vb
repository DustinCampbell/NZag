Imports Xunit

Public Module MemoryTests

    <Fact>
    Sub WriteAndReadBytes()
        Dim memory = CreateMemory(8, &H30000)
        Assert.NotNull(memory)

        ' Write bytes
        Const length As Integer = &H30000 - &H64
        For i = 0 To length - 1
            Dim a = Address.NewRawAddress(&H64 + i)
            memory.WriteByte(a, i Mod Byte.MaxValue)
        Next

        ' Read bytes
        For i = 0 To length - 1
            Dim a = Address.NewRawAddress(&H64 + i)
            Dim b = i Mod Byte.MaxValue
            Dim v = memory.ReadByte(a)
            Assert.Equal(b, v)
        Next
    End Sub

    <Fact>
    Sub WriteAndReadWords()
        Dim memory = CreateMemory(8, &H30000)
        Assert.NotNull(memory)

        ' Write bytes
        Const length As Integer = &H30000 - &H64
        For i = 0 To (length - 1) / 2 Step 2
            Dim a = Address.NewRawAddress(&H64 + i)
            memory.WriteWord(a, i Mod UShort.MaxValue)
        Next

        ' Read bytes
        For i = 0 To (length - 1) / 2 Step 2
            Dim a = Address.NewRawAddress(&H64 + i)
            Dim w = i Mod UShort.MaxValue
            Dim v = memory.ReadWord(a)
            Assert.Equal(w, v)
        Next
    End Sub

    <Fact>
    Sub WriteAndReadDWords()
        Dim memory = CreateMemory(8, &H30000)
        Assert.NotNull(memory)

        ' Write bytes
        Const length As Integer = &H30000 - &H64
        For i = 0 To (length - 1) / 4 Step 4
            Dim a = Address.NewRawAddress(&H64 + i)
            memory.WriteDWord(a, i Mod UInteger.MaxValue)
        Next

        ' Read bytes
        For i = 0 To (length - 1) / 4 Step 4
            Dim a = Address.NewRawAddress(&H64 + i)
            Dim dw = i Mod UInteger.MaxValue
            Dim v = memory.ReadDWord(a)
            Assert.Equal(dw, v)
        Next
    End Sub

    <Fact>
    Sub BadVersionNumber()
        Assert.Throws(Of InvalidOperationException)(
            Sub()
                CreateMemory(0, {})
            End Sub)

        For i = 1 To 8
            Dim v = i
            Assert.DoesNotThrow(
                Sub()
                    CreateMemory(v, {})
                End Sub)
        Next

        Assert.Throws(Of InvalidOperationException)(
            Sub()
                CreateMemory(9, {})
            End Sub)
    End Sub

End Module
