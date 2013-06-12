Imports NZag.Utilities

Public Module BitSetTests

    Private Sub SimpleTests(bitSet As IBitSet)
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

        SimpleTests(bs)
    End Sub

    <Fact>
    Sub Test64Bits()
        Dim bs = BitSet.Create(64)

        Assert.NotNull(bs)
        Assert.Equal("NZag.Utilities.BitSet+BitSet64", bs.GetType().FullName)

        SimpleTests(bs)
    End Sub

    <Fact>
    Sub Test256Bits()
        Dim bs = BitSet.Create(256)

        Assert.NotNull(bs)
        Assert.Equal("NZag.Utilities.BitSet+BitSetN", bs.GetType().FullName)

        SimpleTests(bs)
    End Sub

    Private Sub OrWithTests(bitSet1 As IBitSet, bitSet2 As IBitSet)
        Dim len = bitSet1.Length
        Dim mid = len \ 2

        bitSet1.Clear()
        bitSet2.Clear()

        For i = 0 To mid - 1
            bitSet1(i) = True
        Next

        For i = mid To len - 1
            bitSet2(i) = True
        Next

        bitSet1.OrWith(bitSet2)

        For i = 0 To len - 1
            Assert.True(bitSet1.Contains(i))
            Assert.True(bitSet1(i))
        Next

        bitSet1.Clear()
        bitSet2.Clear()

        For i = 0 To len - 1 Step 4
            bitSet1(i) = True
        Next

        For i = 0 To len - 1 Step 2
            bitSet2(i) = True
        Next

        bitSet1.OrWith(bitSet2)

        For i = 0 To len - 1 Step 2
            Assert.True(bitSet1.Contains(i))
            Assert.True(bitSet1(i))
            Assert.False(bitSet1.Contains(i + 1))
            Assert.False(bitSet1(i + 1))
        Next
    End Sub

    <Fact>
    Sub TestOrWith32()
        Dim bs1 = BitSet.Create(32)
        Dim bs2 = BitSet.Create(32)

        OrWithTests(bs1, bs2)
    End Sub

    <Fact>
    Sub TestOrWith64()
        Dim bs1 = BitSet.Create(64)
        Dim bs2 = BitSet.Create(64)

        OrWithTests(bs1, bs2)
    End Sub

    <Fact>
    Sub TestOrWith256()
        Dim bs1 = BitSet.Create(256)
        Dim bs2 = BitSet.Create(256)

        OrWithTests(bs1, bs2)
    End Sub

    Sub RemoveWhereTests(bitSet As IBitSet)

        bitSet.Clear()

        For i = 0 To bitSet.Length - 1
            bitSet(i) = True
        Next

        bitSet.RemoveWhere(Function(i, v) i Mod 2 = 0)

        For i = 0 To bitSet.Length - 1
            If i Mod 2 = 0 Then
                Assert.False(bitSet(i))
            Else
                Assert.True(bitSet(i))
            End If
        Next
    End Sub

    <Fact>
    Sub TestRemoveWhere32()
        RemoveWhereTests(BitSet.Create(32))
    End Sub

    <Fact>
    Sub TestRemoveWhere64()
        RemoveWhereTests(BitSet.Create(64))
    End Sub

    <Fact>
    Sub TestRemoveWhere256()
        RemoveWhereTests(BitSet.Create(256))
    End Sub

End Module
