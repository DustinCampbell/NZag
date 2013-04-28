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
    return: 1
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
        return: 1
    return: 0
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
        return: 1
    return: 0
]]>

        Test(Zork1, &H4E42, expected)
    End Sub

    <Fact>
    Sub Zork1_4E5C()
        ' 4e5f:  4f 01 00 00             LOADW           L00,#00 -> -(SP)
        ' 4e63:  e7 bf 00 00             RANDOM          (SP)+ -> -(SP)
        ' 4e67:  6f 01 00 00             LOADW           L00,(SP)+ -> -(SP)
        ' 4e6b:  b8                      RET_POPPED

        Dim expected =
<![CDATA[
# temps: 10

LABEL 00
    temp00 <- L00
    temp01 <- 00
    temp02 <- (temp01 * 2)
    temp03 <- (temp00 + temp02)
    push-SP: read-byte(temp03)
    temp04 <- pop-SP
    temp05 <- int16(temp04)
    if (temp05 > 0) is false then
        jump-to: LABEL 01
    push-SP: random(temp05)
    jump-to: LABEL 02
LABEL 01
    randomize(temp05)
    push-SP: 0
LABEL 02
    temp06 <- L00
    temp07 <- pop-SP
    temp08 <- (temp07 * 2)
    temp09 <- (temp06 + temp08)
    push-SP: read-byte(temp09)
    return: pop-SP
]]>

        Test(Zork1, &H4E5C, expected)
    End Sub

    <Fact>
    Sub Zork1_4E6C()
        ' 4e79:  4f 01 00 02             LOADW           L00,#00 -> L01
        ' 4e7d:  4f 01 01 03             LOADW           L00,#01 -> L02
        ' 4e81:  96 02                   DEC             L01
        ' 4e83:  54 01 02 01             ADD             L00,#02 -> L00
        ' 4e87:  56 03 02 00             MUL             L02,#02 -> -(SP)
        ' 4e8b:  74 01 00 06             ADD             L00,(SP)+ -> L05
        ' 4e8f:  75 02 03 00             SUB             L01,L02 -> -(SP)
        ' 4e93:  e7 bf 00 04             RANDOM          (SP)+ -> L03
        ' 4e97:  6f 06 04 05             LOADW           L05,L03 -> L04
        ' 4e9b:  4f 06 01 00             LOADW           L05,#01 -> -(SP)
        ' 4e9f:  e1 ab 06 04 00          STOREW          L05,L03,(SP)+
        ' 4ea4:  e1 9b 06 01 05          STOREW          L05,#01,L04
        ' 4ea9:  95 03                   INC             L02
        ' 4eab:  61 03 02 45             JE              L02,L01 [FALSE] 4eb2
        ' 4eaf:  0d 03 00                STORE           L02,#00
        ' 4eb2:  e1 9b 01 00 03          STOREW          L00,#00,L02
        ' 4eb7:  ab 05                   RET             L04

        Dim expected =
<![CDATA[
# temps: 62

LABEL 00
    temp00 <- L00
    temp01 <- 00
    temp02 <- (temp01 * 2)
    temp03 <- (temp00 + temp02)
    L01 <- read-byte(temp03)
    temp04 <- L00
    temp05 <- 01
    temp06 <- (temp05 * 2)
    temp07 <- (temp04 + temp06)
    L02 <- read-byte(temp07)
    temp08 <- 02
    temp09 <- L01
    temp0a <- int16(temp09)
    L01 <- (temp0a - int16(1))
    temp0b <- L00
    temp0c <- 02
    temp0d <- int16(temp0b)
    temp0e <- int16(temp0c)
    L00 <- (temp0d + temp0e)
    temp0f <- L02
    temp10 <- 02
    temp11 <- int16(temp0f)
    temp12 <- int16(temp10)
    push-SP: (temp11 * temp12)
    temp13 <- L00
    temp14 <- pop-SP
    temp15 <- int16(temp13)
    temp16 <- int16(temp14)
    L05 <- (temp15 + temp16)
    temp17 <- L01
    temp18 <- L02
    temp19 <- int16(temp17)
    temp1a <- int16(temp18)
    push-SP: (temp19 - temp1a)
    temp1b <- pop-SP
    temp1c <- int16(temp1b)
    if (temp1c > 0) is false then
        jump-to: LABEL 01
    L03 <- random(temp1c)
    jump-to: LABEL 02
LABEL 01
    randomize(temp1c)
    L03 <- 0
LABEL 02
    temp1d <- L05
    temp1e <- L03
    temp1f <- (temp1e * 2)
    temp20 <- (temp1d + temp1f)
    L04 <- read-byte(temp20)
    temp21 <- L05
    temp22 <- 01
    temp23 <- (temp22 * 2)
    temp24 <- (temp21 + temp23)
    push-SP: read-byte(temp24)
    temp25 <- L05
    temp26 <- L03
    temp27 <- pop-SP
    temp28 <- (temp26 * 2)
    temp29 <- (temp25 + temp28)
    write-word(temp29) <- temp27
    temp2a <- L05
    temp2b <- 01
    temp2c <- L04
    temp2d <- (temp2b * 2)
    temp2e <- (temp2a + temp2d)
    write-word(temp2e) <- temp2c
    temp2f <- 03
    temp30 <- L02
    temp31 <- int16(temp30)
    L02 <- (temp31 + int16(1))
    temp32 <- L02
    temp33 <- L01
    temp34 <- (temp32 = temp33)
    if (temp34) is false then
        jump-to: LABEL 04
LABEL 03
    temp35 <- 03
    temp36 <- 00
    temp37 <- L02
    L02 <- temp36
LABEL 04
    temp38 <- L00
    temp39 <- 00
    temp3a <- L02
    temp3b <- (temp39 * 2)
    temp3c <- (temp38 + temp3b)
    write-word(temp3c) <- temp3a
    temp3d <- L04
    return: temp3d
]]>

        Test(Zork1, &H4E6C, expected)
    End Sub

    <Fact>
    Sub Zork1_4EBA()
        ' 4ebb:  41 88 2b 40             JE              G78,#2b [FALSE] RFALSE
        ' 4ebf:  e0 1f 48 5d a3 00       CALL            90ba (#a3) -> -(SP)
        ' 4ec5:  b1                      RFALSE       

        Dim expected =
<![CDATA[
# temps: 6

LABEL 00
    temp00 <- G78
    temp01 <- 2b
    temp02 <- (temp00 = temp01)
    if (temp02) is false then
        return: 0
    temp03 <- 485d
    temp04 <- a3
    if (temp03 = 0) is false then
        jump-to: LABEL 01
    push-SP: 0
    jump-to: LABEL 02
LABEL 01
    temp05 <- (temp03 * 2)
    push-SP: call temp03 (temp04)
LABEL 02
    return: 0
]]>

        Test(Zork1, &H4EBA, expected)
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
