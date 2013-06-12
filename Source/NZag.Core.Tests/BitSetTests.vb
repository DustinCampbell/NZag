Imports NZag.Utilities

Public Module BitSetTests

    Private Function CreateBitSet(length As Integer) As IBitSet
        Dim bs = BitSet.Create(length)

        Assert.NotNull(bs)
        If bs.Length <= 32 Then
            Assert.Equal("NZag.Utilities.BitSet+BitSet32", bs.GetType().FullName)
        ElseIf bs.Length <= 64 Then
            Assert.Equal("NZag.Utilities.BitSet+BitSet64", bs.GetType().FullName)
        Else
            Assert.Equal("NZag.Utilities.BitSet+BitSetN", bs.GetType().FullName)
        End If

        Return bs
    End Function

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

    Private Sub UnionWithTests(bitSet1 As IBitSet, bitSet2 As IBitSet)
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

        bitSet1.UnionWith(bitSet2)

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

        bitSet1.UnionWith(bitSet2)

        For i = 0 To len - 1 Step 2
            Assert.True(bitSet1.Contains(i))
            Assert.True(bitSet1(i))
            Assert.False(bitSet1.Contains(i + 1))
            Assert.False(bitSet1(i + 1))
        Next
    End Sub

    Private Sub RemoveWhereTests(bitSet As IBitSet)
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

    Private Sub EqualsTests(bitSet1 As IBitSet, bitSet2 As IBitSet)
        Dim len = bitSet1.Length
        Dim mid = len \ 2

        bitSet1.Clear()
        bitSet2.Clear()

        Assert.True(bitSet1.Equals(bitSet2))
        Assert.True(bitSet2.Equals(bitSet1))

        For i = 0 To mid - 1
            bitSet1(i) = True
        Next

        For i = mid To len - 1
            bitSet2(i) = True
        Next

        Assert.False(bitSet1.Equals(bitSet2))
        Assert.False(bitSet2.Equals(bitSet1))

        bitSet1.Clear()
        bitSet2.Clear()

        For i = 0 To len - 1
            bitSet1(i) = True
            bitSet2(i) = True
        Next

        Assert.True(bitSet1.Equals(bitSet2))
        Assert.True(bitSet2.Equals(bitSet1))
    End Sub

    <Fact>
    Sub Test32Bits()
        SimpleTests(CreateBitSet(32))
    End Sub

    <Fact>
    Sub Test64Bits()
        SimpleTests(CreateBitSet(64))
    End Sub

    <Fact>
    Sub Test256Bits()
        SimpleTests(CreateBitSet(256))
    End Sub

    <Fact>
    Sub TestUnionWith32()
        UnionWithTests(CreateBitSet(32), CreateBitSet(32))
    End Sub

    <Fact>
    Sub TestUnionWith64()
        UnionWithTests(CreateBitSet(64), CreateBitSet(64))
    End Sub

    <Fact>
    Sub TestUnionWith256()
        UnionWithTests(CreateBitSet(256), CreateBitSet(256))
    End Sub

    <Fact>
    Sub TestRemoveWhere32()
        RemoveWhereTests(CreateBitSet(32))
    End Sub

    <Fact>
    Sub TestRemoveWhere64()
        RemoveWhereTests(CreateBitSet(64))
    End Sub

    <Fact>
    Sub TestRemoveWhere256()
        RemoveWhereTests(CreateBitSet(256))
    End Sub

    <Fact>
    Sub TestEquals32()
        EqualsTests(CreateBitSet(32), CreateBitSet(32))
    End Sub

    <Fact>
    Sub TestEquals64()
        EqualsTests(CreateBitSet(64), CreateBitSet(64))
    End Sub

    <Fact>
    Sub TestEquals256()
        EqualsTests(CreateBitSet(256), CreateBitSet(256))
    End Sub

End Module
