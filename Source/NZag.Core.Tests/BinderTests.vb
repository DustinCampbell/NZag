Imports System.Text

Public Module BinderTests

    <Fact>
    Sub CZech_7DC()
        ' 7dd:  e0 3f 09 12 ff          call_vs         2448 -> gef
        ' 7e2:  ba                      quit  

        Dim expected =
<![CDATA[
# temps: 6

LABEL 00
    temp00 <- 0912
    if (temp00 = 0) is false then
        jump-to: LABEL 02
LABEL 01
    temp01 <- (ef * 2)
    temp02 <- (temp01 + 7dd)
    write-word(temp02) <- 0
    jump-to: LABEL 03
LABEL 02
    temp03 <- (temp00 * 4)
    temp04 <- (ef * 2)
    temp05 <- (temp04 + 7dd)
    write-word(temp05) <- call temp03 ()
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
# temps: 15

LABEL 00
    temp00 <- (3c * 2)
    temp01 <- (temp00 + 4f05)
    temp02 <- read-byte(temp01)
    if (temp02 = 0) is true then
        jump-to: LABEL 06
LABEL 01
    temp03 <- 64
    temp04 <- int16(temp03)
    if (temp04 > 0) is false then
        jump-to: LABEL 03
LABEL 02
    push-SP: random(temp04)
    jump-to: LABEL 04
LABEL 03
    randomize(temp04)
    push-SP: 0
LABEL 04
    temp05 <- L00
    temp06 <- pop-SP
    temp07 <- int16(temp05)
    temp08 <- int16(temp06)
    if (temp07 > temp08) is true then
        return: 1
LABEL 05
    return: 0
LABEL 06
    temp09 <- 012c
    temp0a <- int16(temp09)
    if (temp0a > 0) is false then
        jump-to: LABEL 08
LABEL 07
    push-SP: random(temp0a)
    jump-to: LABEL 09
LABEL 08
    randomize(temp0a)
    push-SP: 0
LABEL 09
    temp0b <- L00
    temp0c <- pop-SP
    temp0d <- int16(temp0b)
    temp0e <- int16(temp0c)
    if (temp0d > temp0e) is true then
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
        jump-to: LABEL 02
LABEL 01
    push-SP: random(temp05)
    jump-to: LABEL 03
LABEL 02
    randomize(temp05)
    push-SP: 0
LABEL 03
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
        jump-to: LABEL 02
LABEL 01
    L03 <- random(temp1c)
    jump-to: LABEL 03
LABEL 02
    randomize(temp1c)
    L03 <- 0
LABEL 03
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
        jump-to: LABEL 05
LABEL 04
    temp35 <- 03
    temp36 <- 00
    temp37 <- L02
    L02 <- temp36
LABEL 05
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
# temps: 8

LABEL 00
    temp00 <- (78 * 2)
    temp01 <- (temp00 + 4f05)
    temp02 <- read-byte(temp01)
    temp03 <- 2b
    temp04 <- (temp02 = temp03)
    if (temp04) is false then
        return: 0
LABEL 01
    temp05 <- 485d
    temp06 <- a3
    if (temp05 = 0) is false then
        jump-to: LABEL 03
LABEL 02
    push-SP: 0
    jump-to: LABEL 04
LABEL 03
    temp07 <- (temp05 * 2)
    push-SP: call temp07 (temp06)
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
# temps: 10

LABEL 00
    temp00 <- (2e * 2)
    temp01 <- (temp00 + 4f05)
    temp02 <- read-byte(temp01)
    if (temp02 = 0) is true then
        jump-to: LABEL 07
LABEL 01
    temp03 <- ae
    temp04 <- 0b
    temp05 <- obj-attribute(temp03, temp04)
    if (temp05 = 1) is false then
        jump-to: LABEL 03
LABEL 02
    temp06 <- 39
    return: temp06
LABEL 03
    print: "The grating is closed!"
    print: "\n"
    temp07 <- 4a98
    temp08 <- ae
    if (temp07 = 0) is false then
        jump-to: LABEL 05
LABEL 04
    push-SP: 0
    jump-to: LABEL 06
LABEL 05
    temp09 <- (temp07 * 2)
    push-SP: call temp09 (temp08)
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
# temps: 14

LABEL 00
    temp00 <- L00
    temp01 <- 01
    temp02 <- (temp00 = temp01)
    if (temp02) is false then
        return: 0
LABEL 01
    temp03 <- (78 * 2)
    temp04 <- (temp03 + 4f05)
    temp05 <- read-byte(temp04)
    temp06 <- 45
    temp07 <- (temp05 = temp06)
    if (temp07) is false then
        return: 0
LABEL 02
    temp08 <- (76 * 2)
    temp09 <- (temp08 + 4f05)
    temp0a <- read-byte(temp09)
    if (temp0a = 0) is false then
        return: 0
LABEL 03
    temp0b <- 8333
    temp0c <- 983b
    if (temp0b = 0) is false then
        jump-to: LABEL 05
LABEL 04
    push-SP: 0
    jump-to: LABEL 06
LABEL 05
    temp0d <- (temp0b * 2)
    push-SP: call temp0d (temp0c)
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
# temps: 162

LABEL 00
    temp00 <- 2a39
    temp01 <- 8010
    temp02 <- ffff
    if (temp00 = 0) is false then
        jump-to: LABEL 02
LABEL 01
    push-SP: 0
    jump-to: LABEL 03
LABEL 02
    temp03 <- (temp00 * 2)
    push-SP: call temp03 (temp01, temp02)
LABEL 03
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
        jump-to: LABEL 05
LABEL 04
    push-SP: 0
    jump-to: LABEL 06
LABEL 05
    temp0c <- (temp09 * 2)
    push-SP: call temp0c (temp0a, temp0b)
LABEL 06
    temp0d <- 2a39
    temp0e <- 80f0
    temp0f <- ffff
    if (temp0d = 0) is false then
        jump-to: LABEL 08
LABEL 07
    push-SP: 0
    jump-to: LABEL 09
LABEL 08
    temp10 <- (temp0d * 2)
    push-SP: call temp10 (temp0e, temp0f)
LABEL 09
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
        jump-to: LABEL 0b
LABEL 0a
    push-SP: 0
    jump-to: LABEL 0c
LABEL 0b
    temp19 <- (temp16 * 2)
    push-SP: call temp19 (temp17, temp18)
LABEL 0c
    temp1a <- 2a39
    temp1b <- 6f55
    temp1c <- c8
    if (temp1a = 0) is false then
        jump-to: LABEL 0e
LABEL 0d
    push-SP: 0
    jump-to: LABEL 0f
LABEL 0e
    temp1d <- (temp1a * 2)
    push-SP: call temp1d (temp1b, temp1c)
LABEL 0f
    temp1e <- 9c
    temp1f <- 06
    temp20 <- 04
    RUNTIME EXCEPTION: Unsupported opcode: put_prop (v.3) with 3 operands
    temp21 <- (10 * 2)
    temp22 <- (temp21 + 4f05)
    temp23 <- read-byte(temp22)
    temp24 <- 02
    temp25 <- int16(temp23)
    temp26 <- int16(temp24)
    push-SP: (temp25 + temp26)
    temp27 <- (0a * 2)
    temp28 <- (temp27 + 4f05)
    temp29 <- read-byte(temp28)
    temp2a <- 01
    temp2b <- pop-SP
    temp2c <- (temp2a * 2)
    temp2d <- (temp29 + temp2c)
    write-word(temp2d) <- temp2b
    temp2e <- (10 * 2)
    temp2f <- (temp2e + 4f05)
    temp30 <- read-byte(temp2f)
    temp31 <- 04
    temp32 <- int16(temp30)
    temp33 <- int16(temp31)
    push-SP: (temp32 + temp33)
    temp34 <- (0a * 2)
    temp35 <- (temp34 + 4f05)
    temp36 <- read-byte(temp35)
    temp37 <- 02
    temp38 <- pop-SP
    temp39 <- (temp37 * 2)
    temp3a <- (temp36 + temp39)
    write-word(temp3a) <- temp38
    temp3b <- (0e * 2)
    temp3c <- (temp3b + 4f05)
    temp3d <- read-byte(temp3c)
    temp3e <- 02
    temp3f <- int16(temp3d)
    temp40 <- int16(temp3e)
    push-SP: (temp3f + temp40)
    temp41 <- (09 * 2)
    temp42 <- (temp41 + 4f05)
    temp43 <- read-byte(temp42)
    temp44 <- 02
    temp45 <- pop-SP
    temp46 <- (temp44 * 2)
    temp47 <- (temp43 + temp46)
    write-word(temp47) <- temp45
    temp48 <- (0e * 2)
    temp49 <- (temp48 + 4f05)
    temp4a <- read-byte(temp49)
    temp4b <- 04
    temp4c <- int16(temp4a)
    temp4d <- int16(temp4b)
    push-SP: (temp4c + temp4d)
    temp4e <- (09 * 2)
    temp4f <- (temp4e + 4f05)
    temp50 <- read-byte(temp4f)
    temp51 <- 03
    temp52 <- pop-SP
    temp53 <- (temp51 * 2)
    temp54 <- (temp50 + temp53)
    write-word(temp54) <- temp52
    temp55 <- (0d * 2)
    temp56 <- (temp55 + 4f05)
    temp57 <- read-byte(temp56)
    temp58 <- 02
    temp59 <- int16(temp57)
    temp5a <- int16(temp58)
    push-SP: (temp59 + temp5a)
    temp5b <- (08 * 2)
    temp5c <- (temp5b + 4f05)
    temp5d <- read-byte(temp5c)
    temp5e <- 01
    temp5f <- pop-SP
    temp60 <- (temp5e * 2)
    temp61 <- (temp5d + temp60)
    write-word(temp61) <- temp5f
    temp62 <- (0c * 2)
    temp63 <- (temp62 + 4f05)
    temp64 <- read-byte(temp63)
    temp65 <- 02
    temp66 <- int16(temp64)
    temp67 <- int16(temp65)
    push-SP: (temp66 + temp67)
    temp68 <- (08 * 2)
    temp69 <- (temp68 + 4f05)
    temp6a <- read-byte(temp69)
    temp6b <- 03
    temp6c <- pop-SP
    temp6d <- (temp6b * 2)
    temp6e <- (temp6a + temp6d)
    write-word(temp6e) <- temp6c
    temp6f <- 10
    temp70 <- b4
    temp71 <- (00 * 2)
    temp72 <- (temp71 + 4f05)
    temp73 <- read-byte(temp72)
    temp74 <- (00 * 2)
    temp75 <- (temp74 + 4f05)
    write-word(temp75) <- temp70
    temp76 <- 4a98
    temp77 <- a0
    if (temp76 = 0) is false then
        jump-to: LABEL 11
LABEL 10
    push-SP: 0
    jump-to: LABEL 12
LABEL 11
    temp78 <- (temp76 * 2)
    push-SP: call temp78 (temp77)
LABEL 12
    temp79 <- (00 * 2)
    temp7a <- (temp79 + 4f05)
    temp7b <- read-byte(temp7a)
    temp7c <- 03
    temp7d <- obj-attribute(temp7b, temp7c)
    if (temp7d = 1) is true then
        jump-to: LABEL 17
LABEL 13
    temp7e <- 3770
    if (temp7e = 0) is false then
        jump-to: LABEL 15
LABEL 14
    push-SP: 0
    jump-to: LABEL 16
LABEL 15
    temp7f <- (temp7e * 2)
    push-SP: call temp7f ()
LABEL 16
    print: "\n"
LABEL 17
    temp80 <- 52
    temp81 <- 01
    temp82 <- (42 * 2)
    temp83 <- (temp82 + 4f05)
    temp84 <- read-byte(temp83)
    temp85 <- (42 * 2)
    temp86 <- (temp85 + 4f05)
    write-word(temp86) <- temp81
    temp87 <- 7f
    temp88 <- 04
    temp89 <- (6f * 2)
    temp8a <- (temp89 + 4f05)
    temp8b <- read-byte(temp8a)
    temp8c <- (6f * 2)
    temp8d <- (temp8c + 4f05)
    write-word(temp8d) <- temp88
    temp8e <- 90
    temp8f <- (6f * 2)
    temp90 <- (temp8f + 4f05)
    temp91 <- read-byte(temp90)
    temp92 <- (80 * 2)
    temp93 <- (temp92 + 4f05)
    temp94 <- read-byte(temp93)
    temp95 <- (80 * 2)
    temp96 <- (temp95 + 4f05)
    write-word(temp96) <- temp91
    temp97 <- (6f * 2)
    temp98 <- (temp97 + 4f05)
    temp99 <- read-byte(temp98)
    temp9a <- (00 * 2)
    temp9b <- (temp9a + 4f05)
    temp9c <- read-byte(temp9b)
    RUNTIME EXCEPTION: Unsupported opcode: insert_obj (v.3) with 2 operands
    temp9d <- 3f02
    if (temp9d = 0) is false then
        jump-to: LABEL 19
LABEL 18
    push-SP: 0
    jump-to: LABEL 1a
LABEL 19
    temp9e <- (temp9d * 2)
    push-SP: call temp9e ()
LABEL 1a
    temp9f <- 2a95
    if (temp9f = 0) is false then
        jump-to: LABEL 1c
LABEL 1b
    push-SP: 0
    jump-to: LABEL 1d
LABEL 1c
    tempa0 <- (temp9f * 2)
    push-SP: call tempa0 ()
LABEL 1d
    tempa1 <- ff66
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
