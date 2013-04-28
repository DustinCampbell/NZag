Imports System.Text

Public Module BinderTests

    <Fact>
    Sub Zork1_4E3B()
        ' 4e3b:  b2 ...                  PRINT           "a "
        ' 4e3e:  aa 01                   PRINT_OBJ       L00
        ' 4e40:  b0                      RTRUE

        Dim expected =
<![CDATA[
LABEL 00
    print "a "
    declare temp00
    temp00 <- L00
    print obj-name(temp00)
    return 1
]]>

        Test(Zork1, &H4E38, expected)
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

        Dim expected =
<![CDATA[
LABEL 00
    declare temp00
    temp00 <- G3c
    if (temp00 = 0) is true then
        jump-to LABEL 02
LABEL 01
    declare temp01
    temp01 <- 64
    declare temp02
    temp02 <- int16(temp01)
    if (temp02 > 0) is false then
        jump-to LABEL 03
    push-SP: random(temp02)
    jump-to LABEL 04
LABEL 03
    randomize(temp02)
    push-SP: 0
LABEL 04
    declare temp03
    temp03 <- L00
    declare temp04
    temp04 <- pop-SP
    declare temp05
    temp05 <- int16(temp03)
    declare temp06
    temp06 <- int16(temp04)
    if (temp05 > temp06) is true then
        return 1
    return 0
LABEL 02
    declare temp07
    temp07 <- 012c
    declare temp08
    temp08 <- int16(temp07)
    if (temp08 > 0) is false then
        jump-to LABEL 05
    push-SP: random(temp08)
    jump-to LABEL 06
LABEL 05
    randomize(temp08)
    push-SP: 0
LABEL 06
    declare temp09
    temp09 <- L00
    declare temp0a
    temp0a <- pop-SP
    declare temp0b
    temp0b <- int16(temp09)
    declare temp0c
    temp0c <- int16(temp0a)
    if (temp0b > temp0c) is true then
        return 1
    return 0
]]>

        Test(Zork1, &H4E42, expected)
    End Sub

    Private Sub Test(gameName As String, address As Integer, expected As XCData)
        Dim memory = GameMemory(gameName)
        Dim reader = New RoutineReader(memory)

        Dim a = RawAddress(address)
        Dim r = reader.ReadRoutine(a)

        Assert.Equal(a, r.Address)

        Dim binder = New RoutineBinder(memory)
        Dim tree = binder.BindRoutine(r)

        Dim builder = New StringBuilder()
        Dim dumper = New BoundNodeDumper(builder)
        dumper.Dump(tree)

        Dim expectedText = expected.Value _
                                   .Trim() _
                                   .Replace(vbLf, vbCrLf)

        Assert.Equal(expectedText, builder.ToString())
    End Sub

End Module
