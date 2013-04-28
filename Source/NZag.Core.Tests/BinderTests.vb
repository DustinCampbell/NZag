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
    push-SP: call temp05 (temp04)
LABEL 02
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
# temps: 8

LABEL 00
    temp00 <- G2e
    if (temp00 = 0) is true then
        jump-to: LABEL 06
LABEL 01
    temp01 <- ae
    temp02 <- 0b
    temp03 <- obj-attribute(temp01, temp02)
    if (temp03 = 1) is false then
        jump-to: LABEL 03
LABEL 02
    temp04 <- 39
    return: temp04
LABEL 03
    print: "The grating is closed!"
    print: "\n"
    temp05 <- 4a98
    temp06 <- ae
    if (temp05 = 0) is false then
        jump-to: LABEL 04
    push-SP: 0
    jump-to: LABEL 05
LABEL 04
    temp07 <- (temp05 * 2)
    push-SP: call temp07 (temp06)
LABEL 05
    return: 0
LABEL 06
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
# temps: 10

LABEL 00
    temp00 <- L00
    temp01 <- 01
    temp02 <- (temp00 = temp01)
    if (temp02) is false then
        return: 0
    temp03 <- G78
    temp04 <- 45
    temp05 <- (temp03 = temp04)
    if (temp05) is false then
        return: 0
    temp06 <- G76
    if (temp06 = 0) is false then
        return: 0
    temp07 <- 8333
    temp08 <- 983b
    if (temp07 = 0) is false then
        jump-to: LABEL 01
    push-SP: 0
    jump-to: LABEL 02
LABEL 01
    temp09 <- (temp07 * 2)
    push-SP: call temp09 (temp08)
LABEL 02
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
# temps: 114

LABEL 00
    temp00 <- 2a39
    temp01 <- 8010
    temp02 <- ffff
    if (temp00 = 0) is false then
        jump-to: LABEL 01
    push-SP: 0
    jump-to: LABEL 02
LABEL 01
    temp03 <- (temp00 * 2)
    push-SP: call temp03 (temp01, temp02)
LABEL 02
    temp04 <- pop-SP
    temp05 <- 00
    temp06 <- 01
    temp07 <- (temp05 * 2)
    temp08 <- (temp04 + temp07)
    write-word(temp08) <- temp06
    temp09 <- 2a39
    temp0a <- 807c
    temp0b <- ffff
    if (temp09 = 0) is false then
        jump-to: LABEL 03
    push-SP: 0
    jump-to: LABEL 04
LABEL 03
    temp0c <- (temp09 * 2)
    push-SP: call temp0c (temp0a, temp0b)
LABEL 04
    temp0d <- 2a39
    temp0e <- 80f0
    temp0f <- ffff
    if (temp0d = 0) is false then
        jump-to: LABEL 05
    push-SP: 0
    jump-to: LABEL 06
LABEL 05
    temp10 <- (temp0d * 2)
    push-SP: call temp10 (temp0e, temp0f)
LABEL 06
    temp11 <- pop-SP
    temp12 <- 00
    temp13 <- 01
    temp14 <- (temp12 * 2)
    temp15 <- (temp11 + temp14)
    write-word(temp15) <- temp13
    temp16 <- 2a39
    temp17 <- 6f6a
    temp18 <- 28
    if (temp16 = 0) is false then
        jump-to: LABEL 07
    push-SP: 0
    jump-to: LABEL 08
LABEL 07
    temp19 <- (temp16 * 2)
    push-SP: call temp19 (temp17, temp18)
LABEL 08
    temp1a <- 2a39
    temp1b <- 6f55
    temp1c <- c8
    if (temp1a = 0) is false then
        jump-to: LABEL 09
    push-SP: 0
    jump-to: LABEL 0a
LABEL 09
    temp1d <- (temp1a * 2)
    push-SP: call temp1d (temp1b, temp1c)
LABEL 0a
    temp1e <- 9c
    temp1f <- 06
    temp20 <- 04
    RUNTIME EXCEPTION: Unsupported opcode: put_prop (v.3) with 3 operands
    temp21 <- G10
    temp22 <- 02
    temp23 <- int16(temp21)
    temp24 <- int16(temp22)
    push-SP: (temp23 + temp24)
    temp25 <- G0a
    temp26 <- 01
    temp27 <- pop-SP
    temp28 <- (temp26 * 2)
    temp29 <- (temp25 + temp28)
    write-word(temp29) <- temp27
    temp2a <- G10
    temp2b <- 04
    temp2c <- int16(temp2a)
    temp2d <- int16(temp2b)
    push-SP: (temp2c + temp2d)
    temp2e <- G0a
    temp2f <- 02
    temp30 <- pop-SP
    temp31 <- (temp2f * 2)
    temp32 <- (temp2e + temp31)
    write-word(temp32) <- temp30
    temp33 <- G0e
    temp34 <- 02
    temp35 <- int16(temp33)
    temp36 <- int16(temp34)
    push-SP: (temp35 + temp36)
    temp37 <- G09
    temp38 <- 02
    temp39 <- pop-SP
    temp3a <- (temp38 * 2)
    temp3b <- (temp37 + temp3a)
    write-word(temp3b) <- temp39
    temp3c <- G0e
    temp3d <- 04
    temp3e <- int16(temp3c)
    temp3f <- int16(temp3d)
    push-SP: (temp3e + temp3f)
    temp40 <- G09
    temp41 <- 03
    temp42 <- pop-SP
    temp43 <- (temp41 * 2)
    temp44 <- (temp40 + temp43)
    write-word(temp44) <- temp42
    temp45 <- G0d
    temp46 <- 02
    temp47 <- int16(temp45)
    temp48 <- int16(temp46)
    push-SP: (temp47 + temp48)
    temp49 <- G08
    temp4a <- 01
    temp4b <- pop-SP
    temp4c <- (temp4a * 2)
    temp4d <- (temp49 + temp4c)
    write-word(temp4d) <- temp4b
    temp4e <- G0c
    temp4f <- 02
    temp50 <- int16(temp4e)
    temp51 <- int16(temp4f)
    push-SP: (temp50 + temp51)
    temp52 <- G08
    temp53 <- 03
    temp54 <- pop-SP
    temp55 <- (temp53 * 2)
    temp56 <- (temp52 + temp55)
    write-word(temp56) <- temp54
    temp57 <- 10
    temp58 <- b4
    temp59 <- G00
    G00 <- temp58
    temp5a <- 4a98
    temp5b <- a0
    if (temp5a = 0) is false then
        jump-to: LABEL 0b
    push-SP: 0
    jump-to: LABEL 0c
LABEL 0b
    temp5c <- (temp5a * 2)
    push-SP: call temp5c (temp5b)
LABEL 0c
    temp5d <- G00
    temp5e <- 03
    temp5f <- obj-attribute(temp5d, temp5e)
    if (temp5f = 1) is true then
        jump-to: LABEL 10
LABEL 0d
    temp60 <- 3770
    if (temp60 = 0) is false then
        jump-to: LABEL 0e
    push-SP: 0
    jump-to: LABEL 0f
LABEL 0e
    temp61 <- (temp60 * 2)
    push-SP: call temp61 ()
LABEL 0f
    print: "\n"
LABEL 10
    temp62 <- 52
    temp63 <- 01
    temp64 <- G42
    G42 <- temp63
    temp65 <- 7f
    temp66 <- 04
    temp67 <- G6f
    G6f <- temp66
    temp68 <- 90
    temp69 <- G6f
    temp6a <- G80
    G80 <- temp69
    temp6b <- G6f
    temp6c <- G00
    RUNTIME EXCEPTION: Unsupported opcode: insert_obj (v.3) with 2 operands
    temp6d <- 3f02
    if (temp6d = 0) is false then
        jump-to: LABEL 11
    push-SP: 0
    jump-to: LABEL 12
LABEL 11
    temp6e <- (temp6d * 2)
    push-SP: call temp6e ()
LABEL 12
    temp6f <- 2a95
    if (temp6f = 0) is false then
        jump-to: LABEL 13
    push-SP: 0
    jump-to: LABEL 14
LABEL 13
    temp70 <- (temp6f * 2)
    push-SP: call temp70 ()
LABEL 14
    temp71 <- ff66
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
