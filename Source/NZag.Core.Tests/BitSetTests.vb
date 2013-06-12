Imports NZag.Utilities

Public Module BitSetTests

    Private Sub TestBitSet(bitSet As IBitSet)
        ' Verify that IBitSet is initially cleared
        For i = 0 To bitSet.Length - 1
            Assert.Equal(False, bitSet(i))
        Next

        ' Add each bit
        For i = 0 To bitSet.Length - 1
            bitSet.Add(i)
        Next

        For i = 0 To bitSet.Length - 1
            Assert.True(bitSet.Contains(i))
            Assert.True(bitSet(i))
        Next

        ' Remove each bit
        For i = 0 To bitSet.Length - 1
            bitSet.Remove(i)
        Next

        For i = 0 To bitSet.Length - 1
            Assert.False(bitSet.Contains(i))
            Assert.False(bitSet(i))
        Next

        ' Set each bit
        For i = 0 To bitSet.Length - 1
            bitSet(i) = True
        Next

        For i = 0 To bitSet.Length - 1
            Assert.True(bitSet.Contains(i))
            Assert.True(bitSet(i))
        Next

        ' Clear each bit
        For i = 0 To bitSet.Length - 1
            bitSet(i) = False
        Next

        For i = 0 To bitSet.Length - 1
            Assert.False(bitSet.Contains(i))
            Assert.False(bitSet(i))
        Next

        ' Add every other bit
        For i = 0 To bitSet.Length - 1 Step 2
            bitSet.Add(i)
        Next

        For i = 0 To bitSet.Length - 1 Step 2
            Assert.True(bitSet.Contains(i))
            Assert.True(bitSet(i))
            Assert.False(bitSet.Contains(i + 1))
            Assert.False(bitSet(i + 1))
        Next

        ' Clear
        bitSet.Clear()

        For i = 0 To bitSet.Length - 1
            Assert.False(bitSet.Contains(i))
            Assert.False(bitSet(i))
        Next
    End Sub

    <Fact>
    Sub Test32Bits()
        Dim bs = BitSet.Create(32)

        Assert.NotNull(bs)
        Assert.Equal("NZag.Utilities.BitSet+BitSet32", bs.GetType().FullName)

        TestBitSet(bs)
    End Sub

    <Fact>
    Sub Test64Bits()
        Dim bs = BitSet.Create(64)

        Assert.NotNull(bs)
        Assert.Equal("NZag.Utilities.BitSet+BitSet64", bs.GetType().FullName)

        TestBitSet(bs)
    End Sub

    <Fact>
    Sub Test256Bits()
        Dim bs = BitSet.Create(256)

        Assert.NotNull(bs)
        Assert.Equal("NZag.Utilities.BitSet+BitSetN", bs.GetType().FullName)

        TestBitSet(bs)
    End Sub

End Module
