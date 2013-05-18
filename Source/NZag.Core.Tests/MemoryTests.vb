Public Module MemoryTests

    <Fact>
    Sub ReadByte()
        Dim memory = CreateMemory(8, &H30000)

        ' Write bytes
        Const length As Integer = &H30000 - &H40
        For i = 0 To length - 1
            Dim a = RawAddress(&H40 + i)
            memory.WriteByte(a, CByte(i Mod Byte.MaxValue))
        Next

        ' Read bytes
        For i = 0 To length - 1
            Dim a = RawAddress(&H40 + i)
            Dim b = i Mod Byte.MaxValue
            Dim v = memory.ReadByte(a)
            Assert.Equal(b, v)
        Next
    End Sub

    <Fact>
    Sub Read()
        Dim memory = CreateMemory(8, &H30000)

        ' Write bytes
        Const length As Integer = &H30000 - &H40
        For i = 0 To length - 1
            Dim a = RawAddress(&H40 + i)
            memory.WriteByte(a, CByte(i Mod Byte.MaxValue))
        Next

        'Read bytes
        Dim bytes = New Byte(length - 1) {} ' minus one because VB adds extra array element
        memory.Read(bytes, 0, length, RawAddress(&H40))

        Assert.NotNull(bytes)
        Assert.Equal(length, bytes.Length)

        For i = 0 To length - 1
            Dim b = i Mod Byte.MaxValue
            Dim v = bytes(i)
            Assert.Equal(b, v)
        Next
    End Sub

    <Fact>
    Sub ReadBytes()
        Dim memory = CreateMemory(8, &H30000)

        ' Write bytes
        Const length As Integer = &H30000 - &H40
        For i = 0 To length - 1
            Dim a = RawAddress(&H40 + i)
            memory.WriteByte(a, CByte(i Mod Byte.MaxValue))
        Next

        'Read bytes
        Dim bytes = memory.ReadBytes(RawAddress(&H40), length)

        Assert.NotNull(bytes)
        Assert.Equal(length, bytes.Length)

        For i = 0 To length - 1
            Dim b = i Mod Byte.MaxValue
            Dim v = bytes(i)
            Assert.Equal(b, v)
        Next
    End Sub

    <Fact>
    Sub ReadWord()
        Dim memory = CreateMemory(8, &H30000)

        ' Write words
        Const length As Integer = &H30000 - &H40
        For i = 0 To (length - 1) \ 2 Step 2
            Dim a = RawAddress(&H40 + i)
            memory.WriteWord(a, CUShort(i Mod UShort.MaxValue))
        Next

        ' Read words
        For i = 0 To (length - 1) \ 2 Step 2
            Dim a = RawAddress(&H40 + i)
            Dim w = i Mod UShort.MaxValue
            Dim v = memory.ReadWord(a)
            Assert.Equal(w, v)
        Next
    End Sub

    <Fact>
    Sub ReadDWord()
        Dim memory = CreateMemory(8, &H30000)

        ' Write dwords
        Const length As Integer = &H30000 - &H40
        For i = 0 To (length - 1) \ 4 Step 4
            Dim a = RawAddress(&H40 + i)
            memory.WriteDWord(a, CUInt(i Mod UInteger.MaxValue))
        Next

        ' Read dwords
        For i = 0 To (length - 1) \ 4 Step 4
            Dim a = RawAddress(&H40 + i)
            Dim dw = i Mod UInteger.MaxValue
            Dim v = memory.ReadDWord(a)
            Assert.Equal(dw, v)
        Next
    End Sub

    <Fact>
    Sub MemoryReader_NextByte()
        Dim memory = CreateMemory(8, &H30000)

        ' Write bytes
        Const length As Integer = &H30000 - &H40
        For i = 0 To length - 1
            Dim a = RawAddress(&H40 + i)
            memory.WriteByte(a, CByte(i Mod Byte.MaxValue))
        Next

        ' Read bytes
        Dim reader = memory.CreateMemoryReader(RawAddress(&H40))
        For i = 0 To length - 1
            Dim b = i Mod Byte.MaxValue
            Dim v = reader.NextByte()
            Assert.Equal(b, v)
        Next
    End Sub

    <Fact>
    Sub MemoryReader_NextWord()
        Dim memory = CreateMemory(8, &H30000)

        ' Write words
        Const length As Integer = &H30000 - &H40
        For i = 0 To (length - 1) \ 2 Step 2
            Dim a = RawAddress(&H40 + i)
            memory.WriteWord(a, CUShort(i Mod UShort.MaxValue))
        Next

        ' Read words
        Dim reader = memory.CreateMemoryReader(RawAddress(&H40))
        For i = 0 To (length - 1) \ 2 Step 2
            Dim w = i Mod UShort.MaxValue
            Dim v = reader.NextWord()
            Assert.Equal(w, v)
        Next
    End Sub

    <Fact>
    Sub MemoryReader_NextDWord()
        Dim memory = CreateMemory(8, &H30000)

        ' Write dwords
        Const length As Integer = &H30000 - &H40
        For i = 0 To (length - 1) \ 4 Step 4
            Dim a = RawAddress(&H40 + i)
            memory.WriteDWord(a, CUInt(i Mod UInteger.MaxValue))
        Next

        ' Read dwords
        Dim reader = memory.CreateMemoryReader(RawAddress(&H40))
        For i = 0 To (length - 1) \ 4 Step 4
            Dim dw = i Mod UInteger.MaxValue
            Dim v = reader.NextDWord()
            Assert.Equal(dw, v)
        Next
    End Sub

    <Fact>
    Sub BadVersionNumber()
        Assert.Throws(Of Exception)(
            Sub()
                CreateMemory(0, {})
            End Sub)

        For i As Byte = 1 To 8
            Dim v = i
            Assert.DoesNotThrow(
                Sub()
                    CreateMemory(v, {})
                End Sub)
        Next

        Assert.Throws(Of Exception)(
            Sub()
                CreateMemory(9, {})
            End Sub)
    End Sub

    <Fact>
    Sub WriteBytes()
        Dim memory = CreateMemory(8, &H30000)

        'Initially, write memory with zeros
        Const length As Integer = &H30000 - &H40
        For i = 0 To length - 1
            Dim a = RawAddress(&H40 + i)
            memory.WriteByte(a, 0)
        Next

        'Create value to write
        Dim value = New Byte(length) {}
        For i = 0 To length - 1
            value(i) = CByte(i Mod Byte.MaxValue)
        Next

        'Actually write the bytes
        memory.WriteBytes(RawAddress(&H40), value)

        'Read the bytes back

        For i = 0 To length - 1
            Dim a = RawAddress(&H40 + i)
            Dim b = i Mod Byte.MaxValue
            Dim v = memory.ReadByte(a)
            Assert.Equal(b, v)
        Next
    End Sub


End Module
