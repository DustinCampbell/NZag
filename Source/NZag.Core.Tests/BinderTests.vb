Imports System.Text

Public Module BinderTests

    <Fact>
    Sub CZech_7DC()
        ' 7dd:  e0 3f 09 12 ff          call_vs         2448 -> gef
        ' 7e2:  ba                      quit  

        Dim expected =
<![CDATA[
# temps: 0

LABEL 00
    if (0) is false then
        jump-to: LABEL 02
LABEL 01
    write-word(09bb) <- 0
    jump-to: LABEL 03
LABEL 02
    write-word(09bb) <- call 2448 ()
LABEL 03
    quit
]]>

        Test(CZech, &H7DC, expected)
    End Sub

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
# temps: 11

LABEL 00
    temp00 <- read-byte(4f7d)
    if (temp00 = 0) is true then
        jump-to: LABEL 06
LABEL 01
    temp01 <- int16(64)
    if (temp01 > 0) is false then
        jump-to: LABEL 03
LABEL 02
    push-SP: random(temp01)
    jump-to: LABEL 04
LABEL 03
    randomize(temp01)
    push-SP: 0
LABEL 04
    temp02 <- L00
    temp03 <- pop-SP
    temp04 <- int16(temp02)
    temp05 <- int16(temp03)
    if (temp04 > temp05) is true then
        return: 1
LABEL 05
    return: 0
LABEL 06
    temp06 <- int16(012c)
    if (temp06 > 0) is false then
        jump-to: LABEL 08
LABEL 07
    push-SP: random(temp06)
    jump-to: LABEL 09
LABEL 08
    randomize(temp06)
    push-SP: 0
LABEL 09
    temp07 <- L00
    temp08 <- pop-SP
    temp09 <- int16(temp07)
    temp0a <- int16(temp08)
    if (temp09 > temp0a) is true then
        return: 1
LABEL 0a
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
# temps: 8

LABEL 00
    temp00 <- L00
    temp01 <- (temp00 + 0000)
    push-SP: read-byte(temp01)
    temp02 <- pop-SP
    temp03 <- int16(temp02)
    if (temp03 > 0) is false then
        jump-to: LABEL 02
LABEL 01
    push-SP: random(temp03)
    jump-to: LABEL 03
LABEL 02
    randomize(temp03)
    push-SP: 0
LABEL 03
    temp04 <- L00
    temp05 <- pop-SP
    temp06 <- (temp05 * 2)
    temp07 <- (temp04 + temp06)
    push-SP: read-byte(temp07)
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
# temps: 45

LABEL 00
    temp00 <- L00
    temp01 <- (temp00 + 0000)
    L01 <- read-byte(temp01)
    temp02 <- L00
    temp03 <- (temp02 + 0002)
    L02 <- read-byte(temp03)
    temp04 <- L01
    temp05 <- int16(temp04)
    L01 <- (temp05 - int16(1))
    temp06 <- L00
    temp07 <- int16(temp06)
    temp08 <- int16(02)
    L00 <- (temp07 + temp08)
    temp09 <- L02
    temp0a <- int16(temp09)
    temp0b <- int16(02)
    push-SP: (temp0a * temp0b)
    temp0c <- L00
    temp0d <- pop-SP
    temp0e <- int16(temp0c)
    temp0f <- int16(temp0d)
    L05 <- (temp0e + temp0f)
    temp10 <- L01
    temp11 <- L02
    temp12 <- int16(temp10)
    temp13 <- int16(temp11)
    push-SP: (temp12 - temp13)
    temp14 <- pop-SP
    temp15 <- int16(temp14)
    if (temp15 > 0) is false then
        jump-to: LABEL 02
LABEL 01
    L03 <- random(temp15)
    jump-to: LABEL 03
LABEL 02
    randomize(temp15)
    L03 <- 0
LABEL 03
    temp16 <- L05
    temp17 <- L03
    temp18 <- (temp17 * 2)
    temp19 <- (temp16 + temp18)
    L04 <- read-byte(temp19)
    temp1a <- L05
    temp1b <- (temp1a + 0002)
    push-SP: read-byte(temp1b)
    temp1c <- L05
    temp1d <- L03
    temp1e <- pop-SP
    temp1f <- (temp1d * 2)
    temp20 <- (temp1c + temp1f)
    write-word(temp20) <- temp1e
    temp21 <- L05
    temp22 <- L04
    temp23 <- (temp21 + 0002)
    write-word(temp23) <- temp22
    temp24 <- L02
    temp25 <- int16(temp24)
    L02 <- (temp25 + int16(1))
    temp26 <- L02
    temp27 <- L01
    temp28 <- (temp26 = temp27)
    if (temp28) is false then
        jump-to: LABEL 05
LABEL 04
    L02 <- 00
LABEL 05
    temp29 <- L00
    temp2a <- L02
    temp2b <- (temp29 + 0000)
    write-word(temp2b) <- temp2a
    temp2c <- L04
    return: temp2c
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
# temps: 2

LABEL 00
    temp00 <- read-byte(4ff5)
    temp01 <- (temp00 = 2b)
    if (temp01) is false then
        return: 0
LABEL 01
    if (0) is false then
        jump-to: LABEL 03
LABEL 02
    push-SP: 0
    jump-to: LABEL 04
LABEL 03
    push-SP: call 90ba (a3)
LABEL 04
    return: 0
]]>

        Test(Zork1, &H4EBA, expected)
    End Sub

    <Fact>
    Sub Zork1_4EC6()
        ' 4ec7:  a0 3e d9                JZ              G2e [TRUE] 4ee1
        ' 4eca:  0a ae 0b 44             TEST_ATTR       "grating",#0b [FALSE] 4ed0
        ' 4ece:  9b 39                   RET             #39
        ' 4ed0:  b2 ...                  PRINT           "The grating is closed!"
        ' 4ed9:  bb                      NEW_LINE        
        ' 4eda:  e0 1f 4a 98 ae 00       CALL            9530 (#ae) -> -(SP)
        ' 4ee0:  b1                      RFALSE          
        ' 4ee1:  b2 ...                  PRINT           "You can't go that way."
        ' 4eec:  bb                      NEW_LINE        
        ' 4eed:  b1                      RFALSE     

        Dim expected =
<![CDATA[
# temps: 2

LABEL 00
    temp00 <- read-byte(4f61)
    if (temp00 = 0) is true then
        jump-to: LABEL 07
LABEL 01
    temp01 <- obj-attribute(ae, 0b)
    if (temp01 = 1) is false then
        jump-to: LABEL 03
LABEL 02
    return: 39
LABEL 03
    print: "The grating is closed!"
    print: "\n"
    if (0) is false then
        jump-to: LABEL 05
LABEL 04
    push-SP: 0
    jump-to: LABEL 06
LABEL 05
    push-SP: call 9530 (ae)
LABEL 06
    return: 0
LABEL 07
    print: "You can't go that way."
    print: "\n"
    return: 0
]]>

        Test(Zork1, &H4EC6, expected)
    End Sub

    <Fact>
    Sub Zork1_4EEE()
        ' 4ef1:  41 01 01 40             JE              L00,#01 [FALSE] RFALSE
        ' 4ef5:  41 88 45 40             JE              G78,#45 [FALSE] RFALSE
        ' 4ef9:  a0 86 40                JZ              G76 [FALSE] RFALSE
        ' 4efc:  e0 0f 83 33 98 3b 00    CALL            10666 (S148) -> -(SP)
        ' 4f03:  b0                      RTRUE           

        Dim expected =
<![CDATA[
# temps: 5

LABEL 00
    temp00 <- L00
    temp01 <- (temp00 = 01)
    if (temp01) is false then
        return: 0
LABEL 01
    temp02 <- read-byte(4ff5)
    temp03 <- (temp02 = 45)
    if (temp03) is false then
        return: 0
LABEL 02
    temp04 <- read-byte(4ff1)
    if (temp04 = 0) is false then
        return: 0
LABEL 03
    if (0) is false then
        jump-to: LABEL 05
LABEL 04
    push-SP: 0
    jump-to: LABEL 06
LABEL 05
    push-SP: call 0666 (983b)
LABEL 06
    return: 1
]]>

        Test(Zork1, &H4EEE, expected)
    End Sub

    <Fact>
    Sub Zork1_4F04()
        ' 4f05:  e0 03 2a 39 80 10 ff ff 00 
        '                               CALL            5472 (#8010,#ffff) -> -(SP)
        ' 4f0e:  e1 97 00 00 01          STOREW          (SP)+,#00,#01
        ' 4f13:  e0 03 2a 39 80 7c ff ff 00 
        '                               CALL            5472 (#807c,#ffff) -> -(SP)
        ' 4f1c:  e0 03 2a 39 80 f0 ff ff 00 
        '                               CALL            5472 (#80f0,#ffff) -> -(SP)
        ' 4f25:  e1 97 00 00 01          STOREW          (SP)+,#00,#01
        ' 4f2a:  e0 07 2a 39 6f 6a 28 00 CALL            5472 (#6f6a,#28) -> -(SP)
        ' 4f32:  e0 07 2a 39 6f 55 c8 00 CALL            5472 (#6f55,#c8) -> -(SP)
        ' 4f3a:  e3 57 9c 06 04          PUT_PROP        "magic boat",#06,#04
        ' 4f3f:  54 20 02 00             ADD             G10,#02 -> -(SP)
        ' 4f43:  e1 9b 1a 01 00          STOREW          G0a,#01,(SP)+
        ' 4f48:  54 20 04 00             ADD             G10,#04 -> -(SP)
        ' 4f4c:  e1 9b 1a 02 00          STOREW          G0a,#02,(SP)+
        ' 4f51:  54 1e 02 00             ADD             G0e,#02 -> -(SP)
        ' 4f55:  e1 9b 19 02 00          STOREW          G09,#02,(SP)+
        ' 4f5a:  54 1e 04 00             ADD             G0e,#04 -> -(SP)
        ' 4f5e:  e1 9b 19 03 00          STOREW          G09,#03,(SP)+
        ' 4f63:  54 1d 02 00             ADD             G0d,#02 -> -(SP)
        ' 4f67:  e1 9b 18 01 00          STOREW          G08,#01,(SP)+
        ' 4f6c:  54 1c 02 00             ADD             G0c,#02 -> -(SP)
        ' 4f70:  e1 9b 18 03 00          STOREW          G08,#03,(SP)+
        ' 4f75:  0d 10 b4                STORE           G00,#b4
        ' 4f78:  e0 1f 4a 98 a0 00       CALL            9530 (#a0) -> -(SP)
        ' 4f7e:  4a 10 03 c8             TEST_ATTR       G00,#03 [TRUE] 4f88
        ' 4f82:  e0 3f 37 70 00          CALL            6ee0 -> -(SP)
        ' 4f87:  bb                      NEW_LINE        
        ' 4f88:  0d 52 01                STORE           G42,#01
        ' 4f8b:  0d 7f 04                STORE           G6f,#04
        ' 4f8e:  2d 90 7f                STORE           G80,G6f
        ' 4f91:  6e 7f 10                INSERT_OBJ      G6f,G00
        ' 4f94:  e0 3f 3f 02 00          CALL            7e04 -> -(SP)
        ' 4f99:  e0 3f 2a 95 00          CALL            552a -> -(SP)
        ' 4f9e:  8c ff 66                JUMP            4f05

        Dim expected =
<![CDATA[
# temps: 43

LABEL 00
    if (0) is false then
        jump-to: LABEL 02
LABEL 01
    push-SP: 0
    jump-to: LABEL 03
LABEL 02
    push-SP: call 5472 (8010, ffff)
LABEL 03
    temp00 <- pop-SP
    temp01 <- (temp00 + 0000)
    write-word(temp01) <- 01
    if (0) is false then
        jump-to: LABEL 05
LABEL 04
    push-SP: 0
    jump-to: LABEL 06
LABEL 05
    push-SP: call 5472 (807c, ffff)
LABEL 06
    if (0) is false then
        jump-to: LABEL 08
LABEL 07
    push-SP: 0
    jump-to: LABEL 09
LABEL 08
    push-SP: call 5472 (80f0, ffff)
LABEL 09
    temp02 <- pop-SP
    temp03 <- (temp02 + 0000)
    write-word(temp03) <- 01
    if (0) is false then
        jump-to: LABEL 0b
LABEL 0a
    push-SP: 0
    jump-to: LABEL 0c
LABEL 0b
    push-SP: call 5472 (6f6a, 28)
LABEL 0c
    if (0) is false then
        jump-to: LABEL 0e
LABEL 0d
    push-SP: 0
    jump-to: LABEL 0f
LABEL 0e
    push-SP: call 5472 (6f55, c8)
LABEL 0f
    RUNTIME EXCEPTION: Unsupported opcode: put_prop (v.3) with 3 operands
    temp04 <- read-byte(4f25)
    temp05 <- int16(temp04)
    temp06 <- int16(02)
    push-SP: (temp05 + temp06)
    temp07 <- read-byte(4f19)
    temp08 <- pop-SP
    temp09 <- (temp07 + 0002)
    write-word(temp09) <- temp08
    temp0a <- read-byte(4f25)
    temp0b <- int16(temp0a)
    temp0c <- int16(04)
    push-SP: (temp0b + temp0c)
    temp0d <- read-byte(4f19)
    temp0e <- pop-SP
    temp0f <- (temp0d + 0004)
    write-word(temp0f) <- temp0e
    temp10 <- read-byte(4f21)
    temp11 <- int16(temp10)
    temp12 <- int16(02)
    push-SP: (temp11 + temp12)
    temp13 <- read-byte(4f17)
    temp14 <- pop-SP
    temp15 <- (temp13 + 0004)
    write-word(temp15) <- temp14
    temp16 <- read-byte(4f21)
    temp17 <- int16(temp16)
    temp18 <- int16(04)
    push-SP: (temp17 + temp18)
    temp19 <- read-byte(4f17)
    temp1a <- pop-SP
    temp1b <- (temp19 + 0006)
    write-word(temp1b) <- temp1a
    temp1c <- read-byte(4f1f)
    temp1d <- int16(temp1c)
    temp1e <- int16(02)
    push-SP: (temp1d + temp1e)
    temp1f <- read-byte(4f15)
    temp20 <- pop-SP
    temp21 <- (temp1f + 0002)
    write-word(temp21) <- temp20
    temp22 <- read-byte(4f1d)
    temp23 <- int16(temp22)
    temp24 <- int16(02)
    push-SP: (temp23 + temp24)
    temp25 <- read-byte(4f15)
    temp26 <- pop-SP
    temp27 <- (temp25 + 0006)
    write-word(temp27) <- temp26
    write-word(4f05) <- b4
    if (0) is false then
        jump-to: LABEL 11
LABEL 10
    push-SP: 0
    jump-to: LABEL 12
LABEL 11
    push-SP: call 9530 (a0)
LABEL 12
    temp28 <- read-byte(4f05)
    temp29 <- obj-attribute(temp28, 03)
    if (temp29 = 1) is true then
        jump-to: LABEL 17
LABEL 13
    if (0) is false then
        jump-to: LABEL 15
LABEL 14
    push-SP: 0
    jump-to: LABEL 16
LABEL 15
    push-SP: call 6ee0 ()
LABEL 16
    print: "\n"
LABEL 17
    write-word(4f89) <- 01
    write-word(4fe3) <- 04
    temp2a <- read-byte(4fe3)
    write-word(5005) <- temp2a
    RUNTIME EXCEPTION: Unsupported opcode: insert_obj (v.3) with 2 operands
    if (0) is false then
        jump-to: LABEL 19
LABEL 18
    push-SP: 0
    jump-to: LABEL 1a
LABEL 19
    push-SP: call 7e04 ()
LABEL 1a
    if (0) is false then
        jump-to: LABEL 1c
LABEL 1b
    push-SP: 0
    jump-to: LABEL 1d
LABEL 1c
    push-SP: call 552a ()
LABEL 1d
    jump-to: LABEL 00
]]>

        Test(Zork1, &H4F04, expected)
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
