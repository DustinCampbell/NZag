Imports System.Text

Public Module BinderTests

    <Fact>
    Sub Zork1_4E3B()
        ' 4e3b:  b2 ...                  PRINT           "a "
        ' 4e3e:  aa 01                   PRINT_OBJ       L00
        ' 4e40:  b0                      RTRUE

        Dim expected =
<![CDATA[
# temps: 1

LABEL 00
    print: "a "
    temp00 <- L00
    print: obj-name(temp00)
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
# temps: 13

LABEL 00
    temp00 <- G3c
    if (temp00 = 0) is true then
        jump-to: LABEL 04
LABEL 01
    temp01 <- 64
    temp02 <- int16(temp01)
    if (temp02 > 0) is false then
        jump-to: LABEL 02
    push-SP: random(temp02)
    jump-to: LABEL 03
LABEL 02
    randomize(temp02)
    push-SP: 0
LABEL 03
    temp03 <- L00
    temp04 <- pop-SP
    temp05 <- int16(temp03)
    temp06 <- int16(temp04)
    if (temp05 > temp06) is true then
        return 1
    return 0
LABEL 04
    temp07 <- 012c
    temp08 <- int16(temp07)
    if (temp08 > 0) is false then
        jump-to: LABEL 05
    push-SP: random(temp08)
    jump-to: LABEL 06
LABEL 05
    randomize(temp08)
    push-SP: 0
LABEL 06
    temp09 <- L00
    temp0a <- pop-SP
    temp0b <- int16(temp09)
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
