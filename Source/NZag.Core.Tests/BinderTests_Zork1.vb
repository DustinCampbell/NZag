Public Module BinderTests_Zork1

#Region "Zork1_4E3B"

#Region "ZCode"
    ' 4e3b:  b2 ...                  PRINT           "a "
    ' 4e3e:  aa 01                   PRINT_OBJ       L00
    ' 4e40:  b0                      RTRUE
#End Region

    <Fact>
    Sub Zork1_4E3B()
        Dim expected =
<![CDATA[
# temps: 2

LABEL 00
    temp00 <- L00
LABEL 01
    print: "a "
    temp01 <- read-word((((temp00 - 1) * 9) + 2ee) + 7)
    print: read-text(temp01 + 1, read-byte(temp01))
    return: 1
]]>

        TestBinder(Zork1, &H4E38, expected)
    End Sub

#End Region
#Region "Zork1_4E42"

#Region "ZCode"
    ' 4e45:  a0 4c cb                JZ              G3c [TRUE] 4e51
    ' 4e48:  e7 7f 64 00             RANDOM          #64 -> -(SP)
    ' 4e4c:  63 01 00 c1             JG              L00,(SP)+ [TRUE] RTRUE
    ' 4e50:  b1                      RFALSE
    ' 4e51:  e7 3f 01 2c 00          RANDOM          #012c -> -(SP)
    ' 4e56:  63 01 00 c1             JG              L00,(SP)+ [TRUE] RTRUE
    ' 4e5a:  b1                      RFALSE
#End Region

    <Fact>
    Sub Zork1_4E42()

        Dim expected =
<![CDATA[
# temps: 4

LABEL 00
    temp00 <- L00
LABEL 01
    temp01 <- read-word(22e9)
    if (temp01 = 0) is true then
        jump-to: LABEL 07
LABEL 02
    if (int16(64) > 0) is false then
        jump-to: LABEL 04
LABEL 03
    push-SP: random(int16(64))
    jump-to: LABEL 05
LABEL 04
    randomize(int16(64))
    push-SP: 0
LABEL 05
    temp02 <- pop-SP
    if (int16(temp00) > int16(temp02)) is true then
        return: 1
LABEL 06
    return: 0
LABEL 07
    if (int16(012c) > 0) is false then
        jump-to: LABEL 09
LABEL 08
    push-SP: random(int16(012c))
    jump-to: LABEL 0a
LABEL 09
    randomize(int16(012c))
    push-SP: 0
LABEL 0a
    temp03 <- pop-SP
    if (int16(temp00) > int16(temp03)) is true then
        return: 1
LABEL 0b
    return: 0
]]>

        TestBinder(Zork1, &H4E42, expected)
    End Sub

#End Region
#Region "Zork1_4E5C"

#Region "ZCode"
    ' 4e5f:  4f 01 00 00             LOADW           L00,#00 -> -(SP)
    ' 4e63:  e7 bf 00 00             RANDOM          (SP)+ -> -(SP)
    ' 4e67:  6f 01 00 00             LOADW           L00,(SP)+ -> -(SP)
    ' 4e6b:  b8                      RET_POPPED
#End Region

    <Fact>
    Sub Zork1_4E5C()
        Dim expected =
<![CDATA[
# temps: 3

LABEL 00
    temp00 <- L00
LABEL 01
    temp01 <- read-word(temp00)
    if (int16(temp01) > 0) is false then
        jump-to: LABEL 03
LABEL 02
    push-SP: random(int16(temp01))
    jump-to: LABEL 04
LABEL 03
    randomize(int16(temp01))
    push-SP: 0
LABEL 04
    temp02 <- pop-SP
    return: read-word(temp00 + (temp02 * 2))
]]>

        TestBinder(Zork1, &H4E5C, expected)
    End Sub

#End Region
#Region "Zork1_4E6C"

#Region "ZCode"
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
#End Region

    <Fact>
    Sub Zork1_4E6C()
        Dim expected =
<![CDATA[
# temps: 9

LABEL 00
    temp00 <- L00
LABEL 01
    temp01 <- read-word(temp00)
    temp02 <- read-word(temp00 + 2)
    temp01 <- (int16(temp01) - int16(1))
    temp00 <- (int16(temp00) + int16(02))
    temp03 <- (int16(temp02) * int16(02))
    temp04 <- (int16(temp00) + int16(temp03))
    temp05 <- (int16(temp01) - int16(temp02))
    if (int16(temp05) > 0) is false then
        jump-to: LABEL 03
LABEL 02
    temp06 <- random(int16(temp05))
    jump-to: LABEL 04
LABEL 03
    randomize(int16(temp05))
    temp06 <- 0
LABEL 04
    temp07 <- read-word(temp04 + (temp06 * 2))
    temp08 <- read-word(temp04 + 2)
    write-word(temp04 + (temp06 * 2)) <- temp08
    write-word(temp04 + 2) <- temp07
    temp02 <- (int16(temp02) + int16(1))
    if (temp02 = temp01) is false then
        jump-to: LABEL 06
LABEL 05
    temp02 <- 00
LABEL 06
    write-word(temp00) <- temp02
    return: temp07
]]>

        TestBinder(Zork1, &H4E6C, expected)
    End Sub

#End Region
#Region "Zork1_4EBA"

#Region "ZCode"
    ' 4ebb:  41 88 2b 40             JE              G78,#2b [FALSE] RFALSE
    ' 4ebf:  e0 1f 48 5d a3 00       CALL            90ba (#a3) -> -(SP)
    ' 4ec5:  b1                      RFALSE       
#End Region

    <Fact>
    Sub Zork1_4EBA()
        Dim expected =
<![CDATA[
# temps: 1

LABEL 00
    temp00 <- read-word(2361)
    if (temp00 = 2b) is false then
        return: 0
LABEL 01
    push-SP: call 90ba (a3)
    return: 0
]]>

        TestBinder(Zork1, &H4EBA, expected)
    End Sub

#End Region
#Region "Zork1_4EC6"

#Region "ZCode"
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
#End Region

    <Fact>
    Sub Zork1_4EC6()
        Dim expected =
<![CDATA[
# temps: 1

LABEL 00
    temp00 <- read-word(22cd)
    if (temp00 = 0) is true then
        jump-to: LABEL 04
LABEL 01
    if (((read-byte(904) & 0010) <> 0) = 1) is false then
        jump-to: LABEL 03
LABEL 02
    return: 39
LABEL 03
    print: "The grating is closed!"
    print: "\n"
    push-SP: call 9530 (ae)
    return: 0
LABEL 04
    print: "You can't go that way."
    print: "\n"
    return: 0
]]>

        TestBinder(Zork1, &H4EC6, expected)
    End Sub

#End Region
#Region "Zork1_4EEE"

#Region "ZCode"
    ' 4ef1:  41 01 01 40             JE              L00,#01 [FALSE] RFALSE
    ' 4ef5:  41 88 45 40             JE              G78,#45 [FALSE] RFALSE
    ' 4ef9:  a0 86 40                JZ              G76 [FALSE] RFALSE
    ' 4efc:  e0 0f 83 33 98 3b 00    CALL            10666 (S148) -> -(SP)
    ' 4f03:  b0                      RTRUE           
#End Region

    <Fact>
    Sub Zork1_4EEE()
        Dim expected =
<![CDATA[
# temps: 3

LABEL 00
    temp00 <- L00
LABEL 01
    if (temp00 = 01) is false then
        return: 0
LABEL 02
    temp01 <- read-word(2361)
    if (temp01 = 45) is false then
        return: 0
LABEL 03
    temp02 <- read-word(235d)
    if (temp02 = 0) is false then
        return: 0
LABEL 04
    push-SP: call 10666 (983b)
    return: 1
]]>

        TestBinder(Zork1, &H4EEE, expected)
    End Sub

#End Region
#Region "Zork1_4F04"

#Region "ZCode"
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
#End Region

    <Fact>
    Sub Zork1_4F04()
        Dim expected =
<![CDATA[
# temps: 33

LABEL 00
    temp00 <- call 5472 (8010, ffff)
    write-word(temp00) <- 01
    push-SP: call 5472 (807c, ffff)
    push-SP: call 5472 (80f0, ffff)
    temp01 <- pop-SP
    write-word(temp01) <- 01
    push-SP: call 5472 (6f6a, 28)
    push-SP: call 5472 (6f55, c8)
    temp02 <- read-word(868)
    temp03 <- uint16((temp02 + 1) + (read-byte(temp02) * 2))
    temp04 <- 0
LABEL 01
    temp05 <- read-byte(temp03)
    if ((temp05 & 1f) <= 06) is false then
        jump-to: LABEL 03
LABEL 02
    temp04 <- 1
    jump-to: LABEL 04
LABEL 03
    temp06 <- read-byte(temp03)
    temp03 <- uint16((temp03 + 1) + ((temp06 >> 5) + 1))
LABEL 04
    if (temp04 = 0) is true then
        jump-to: LABEL 01
    if ((temp05 & 1f) <> 06) is false then
        jump-to: LABEL 06
LABEL 05
    RUNTIME EXCEPTION: Property not found!
LABEL 06
    temp03 <- (temp03 + 1)
    if ((temp05 & e0) = 0) is false then
        jump-to: LABEL 08
LABEL 07
    write-byte(temp03) <- byte(04)
    jump-to: LABEL 09
LABEL 08
    write-word(temp03) <- 04
LABEL 09
    temp07 <- read-word(2291)
    push-SP: (int16(temp07) + int16(02))
    temp08 <- read-word(2285)
    temp09 <- pop-SP
    write-word(temp08 + 2) <- temp09
    temp0a <- read-word(2291)
    push-SP: (int16(temp0a) + int16(04))
    temp0b <- read-word(2285)
    temp0c <- pop-SP
    write-word(temp0b + 4) <- temp0c
    temp0d <- read-word(228d)
    push-SP: (int16(temp0d) + int16(02))
    temp0e <- read-word(2283)
    temp0f <- pop-SP
    write-word(temp0e + 4) <- temp0f
    temp10 <- read-word(228d)
    push-SP: (int16(temp10) + int16(04))
    temp11 <- read-word(2283)
    temp12 <- pop-SP
    write-word(temp11 + 6) <- temp12
    temp13 <- read-word(228b)
    push-SP: (int16(temp13) + int16(02))
    temp14 <- read-word(2281)
    temp15 <- pop-SP
    write-word(temp14 + 2) <- temp15
    temp16 <- read-word(2289)
    push-SP: (int16(temp16) + int16(02))
    temp17 <- read-word(2281)
    temp18 <- pop-SP
    write-word(temp17 + 6) <- temp18
    write-word(2271) <- b4
    push-SP: call 9530 (a0)
    temp19 <- read-word(2271)
    if (((read-byte(((temp19 - 1) * 9) + 2ee) & 0010) <> 0) = 1) is true then
        jump-to: LABEL 0b
LABEL 0a
    push-SP: call 6ee0 ()
    print: "\n"
LABEL 0b
    write-word(22f5) <- 01
    write-word(234f) <- 04
    temp1a <- read-word(234f)
    write-word(2371) <- temp1a
    temp1b <- read-word(234f)
    temp1c <- read-word(2271)
    temp1d <- 0
    if (read-byte((((temp1b - 1) * 9) + 2ee) + 4) = 0) is false then
        jump-to: LABEL 0d
LABEL 0c
    temp1e <- 0
    jump-to: LABEL 0e
LABEL 0d
    temp1e <- read-byte((((read-byte((((temp1b - 1) * 9) + 2ee) + 4) - 1) * 9) + 2ee) + 6)
LABEL 0e
    if (temp1e <> temp1b) is false then
        jump-to: LABEL 14
LABEL 0f
    temp1f <- temp1e
LABEL 10
    temp20 <- read-byte((((temp1f - 1) * 9) + 2ee) + 5)
    if (temp20 = temp1b) is false then
        jump-to: LABEL 12
LABEL 11
    temp1d <- temp1f
    temp1f <- 0
    jump-to: LABEL 13
LABEL 12
    temp1f <- temp20
LABEL 13
    if (temp1f <> 0) is true then
        jump-to: LABEL 10
LABEL 14
    if (temp1d <> 0) is false then
        jump-to: LABEL 16
LABEL 15
    write-byte((((temp1d - 1) * 9) + 2ee) + 5) <- read-byte((((temp1b - 1) * 9) + 2ee) + 5)
LABEL 16
    if (temp1e = temp1b) is false then
        jump-to: LABEL 18
LABEL 17
    write-byte((((read-byte((((temp1b - 1) * 9) + 2ee) + 4) - 1) * 9) + 2ee) + 6) <- read-byte((((temp1b - 1) * 9) + 2ee) + 5)
LABEL 18
    write-byte((((temp1b - 1) * 9) + 2ee) + 4) <- 0
    write-byte((((temp1b - 1) * 9) + 2ee) + 5) <- 0
    if (temp1c <> 0) is false then
        jump-to: LABEL 1a
LABEL 19
    write-byte((((temp1b - 1) * 9) + 2ee) + 4) <- temp1c
    write-byte((((temp1b - 1) * 9) + 2ee) + 5) <- read-byte((((temp1c - 1) * 9) + 2ee) + 6)
    write-byte((((temp1c - 1) * 9) + 2ee) + 6) <- temp1b
LABEL 1a
    push-SP: call 7e04 ()
    push-SP: call 552a ()
    jump-to: LABEL 00
]]>

        TestBinder(Zork1, &H4F04, expected)
    End Sub

#End Region
#Region "Zork1_4FA2"

#Region "ZCode"
    ' 4fa9:  41 86 0b 59             JE              G76,#0b [FALSE] 4fc4
    ' 4fad:  41 87 0b 55             JE              G77,#0b [FALSE] 4fc4
    ' 4fb1:  b3 ...                  PRINT_RET       "Those things aren't here!"
    ' 4fc4:  41 86 0b 48             JE              G76,#0b [FALSE] 4fce
    ' 4fc8:  2d 01 66                STORE           L00,G56
    ' 4fcb:  8c 00 08                JUMP            4fd4
    ' 4fce:  2d 01 65                STORE           L00,G55
    ' 4fd1:  0d 02 00                STORE           L01,#00
    ' 4fd4:  0d 7c 00                STORE           G6c,#00
    ' 4fd7:  0d 70 00                STORE           G60,#00
    ' 4fda:  61 7f 90 56             JE              G6f,G80 [FALSE] 4ff2
    ' 4fde:  b2 ...                  PRINT           "You can't see any"
    ' 4fe7:  e0 2f 28 49 02 00       CALL            5092 (L01) -> -(SP)
    ' 4fed:  b3 ...                  PRINT_RET       " here!"
    ' 4ff2:  b2 ...                  PRINT           "The "
    ' 4ff5:  aa 7f                   PRINT_OBJ       G6f
    ' 4ff7:  b2 ...                  PRINT           " seems confused. "I don't see
    'any"
    ' 500a:  e0 2f 28 49 02 00       CALL            5092 (L01) -> -(SP)
    ' 5010:  b3 ...                  PRINT_RET       " here!""
#End Region

    <Fact>
    Sub Zork1_4FA2()
        Dim expected =
<![CDATA[
# temps: 8

LABEL 00
    temp00 <- L01
LABEL 01
    temp01 <- read-word(235d)
    if (temp01 = 0b) is false then
        jump-to: LABEL 04
LABEL 02
    temp02 <- read-word(235f)
    if (temp02 = 0b) is false then
        jump-to: LABEL 04
LABEL 03
    print: "Those things aren't here!"
    return: 1
LABEL 04
    temp03 <- read-word(235d)
    if (temp03 = 0b) is false then
        jump-to: LABEL 06
LABEL 05
    jump-to: LABEL 07
LABEL 06
    temp00 <- 00
LABEL 07
    write-word(2349) <- 00
    write-word(2331) <- 00
    temp04 <- read-word(234f)
    temp05 <- read-word(2371)
    if (temp04 = temp05) is false then
        jump-to: LABEL 09
LABEL 08
    print: "You can't see any"
    push-SP: call 5092 (temp00)
    print: " here!"
    return: 1
LABEL 09
    print: "The "
    temp06 <- read-word(234f)
    temp07 <- read-word((((temp06 - 1) * 9) + 2ee) + 7)
    print: read-text(temp07 + 1, read-byte(temp07))
    print: " seems confused. "I don't see any"
    push-SP: call 5092 (temp00)
    print: " here!""
    return: 1
]]>

        TestBinder(Zork1, &H4FA2, expected)
    End Sub

#End Region
#Region "Zork1_552A"

#Region "ZCode"
    ' 553f:  0d 04 00                STORE           L03,#00
    ' 5542:  0d 05 00                STORE           L04,#00
    ' 5545:  0d 08 01                STORE           L07,#01
    ' 5548:  e0 3f 2c 40 8f          CALL            5880 -> G7f
    ' 554d:  a0 8f 81 fc             JZ              G7f [TRUE] 574b
    ' 5551:  6f 65 61 01             LOADW           G55,G51 -> L00
    ' 5555:  6f 66 61 02             LOADW           G56,G51 -> L01
    ' 5559:  a0 02 48                JZ              L01 [FALSE] 5562
    ' 555c:  e8 bf 02                PUSH            L01
    ' 555f:  8c 00 33                JUMP            5593
    ' 5562:  43 02 01 58             JG              L01,#01 [FALSE] 557c
    ' 5566:  2d 06 66                STORE           L05,G56
    ' 5569:  a0 01 48                JZ              L00 [FALSE] 5572
    ' 556c:  0d 05 00                STORE           L04,#00
    ' 556f:  8c 00 06                JUMP            5576
    ' 5572:  4f 65 01 05             LOADW           G55,#01 -> L04
    ' 5576:  e8 bf 02                PUSH            L01
    ' 5579:  8c 00 19                JUMP            5593
    ' 557c:  43 01 01 52             JG              L00,#01 [FALSE] 5590
    ' 5580:  0d 08 00                STORE           L07,#00
    ' 5583:  2d 06 65                STORE           L05,G55
    ' 5586:  4f 66 01 05             LOADW           G56,#01 -> L04
    ' 558a:  e8 bf 01                PUSH            L00
    ' 558d:  8c 00 05                JUMP            5593
    ' 5590:  e8 7f 01                PUSH            #01
    ' 5593:  2d 03 00                STORE           L02,(SP)+
    ' 5596:  a0 05 4a                JZ              L04 [FALSE] 55a1
    ' 5599:  41 01 01 46             JE              L00,#01 [FALSE] 55a1
    ' 559d:  4f 65 01 05             LOADW           G55,#01 -> L04
    ' 55a1:  41 88 89 4c             JE              G78,#89 [FALSE] 55af
    ' 55a5:  e0 2b 2b be 88 86 07    CALL            577c (G78,G76) -> L06
    ' 55ac:  8c 01 6a                JUMP            5717
    ' 55af:  a0 03 00 4f             JZ              L02 [FALSE] 5600
    ' 55b3:  50 83 00 00             LOADB           G73,#00 -> -(SP)
    ' 55b7:  49 00 03 00             AND             (SP)+,#03 -> -(SP)
    ' 55bb:  a0 00 4e                JZ              (SP)+ [FALSE] 55ca
    ' 55be:  e0 2f 2b be 88 07       CALL            577c (G78) -> L06
    ' 55c4:  0d 86 00                STORE           G76,#00
    ' 55c7:  8c 01 4f                JUMP            5717
    ' 55ca:  a0 52 53                JZ              G42 [FALSE] 55de
    ' 55cd:  b2 ...                  PRINT           "It's too dark to see."
    ' 55da:  bb                      NEW_LINE        
    ' 55db:  8c 01 3b                JUMP            5717
    ' 55de:  b2 ...                  PRINT           "It's not clear what you're referring to."
    ' 55f9:  bb                      NEW_LINE        
    ' 55fa:  0d 07 00                STORE           L06,#00
    ' 55fd:  8c 01 19                JUMP            5717
    ' 5600:  0d 8a 00                STORE           G7a,#00
    ' 5603:  0d 8b 00                STORE           G7b,#00
    ' 5606:  43 03 01 45             JG              L02,#01 [FALSE] 560d
    ' 560a:  0d 8b 01                STORE           G7b,#01
    ' 560d:  0d 0a 00                STORE           L09,#00
    ' 5610:  25 04 03 00 5b          INC_CHK         L03,L02 [FALSE] 566e
    ' 5615:  43 8a 00 00 3d          JG              G7a,#00 [FALSE] 5655
    ' 561a:  b2 ...                  PRINT           "The "
    ' 561d:  61 8a 03 c5             JE              G7a,L02 [TRUE] 5624
    ' 5621:  b2 ...                  PRINT           "other "
    ' 5624:  b2 ...                  PRINT           "object"
    ' 5629:  41 8a 01 c5             JE              G7a,#01 [TRUE] 5630
    ' 562d:  b2 ...                  PRINT           "s"
    ' 5630:  b2 ...                  PRINT           " that you mentioned "
    ' 563d:  41 8a 01 c8             JE              G7a,#01 [TRUE] 5647
    ' 5641:  b2 ...                  PRINT           "are"
    ' 5644:  8c 00 05                JUMP            564a
    ' 5647:  b2 ...                  PRINT           "is"
    ' 564a:  b2 ...                  PRINT           "n't here."
    ' 5651:  bb                      NEW_LINE
    ' 5652:  8c 00 c4                JUMP            5717
    ' 5655:  a0 0a 00 c0             JZ              L09 [FALSE] 5717
    ' 5659:  b2 ...                  PRINT           "There's nothing here you can take."
    ' 566a:  bb                      NEW_LINE
    ' 566b:  8c 00 ab                JUMP            5717
    ' 566e:  a0 08 c9                JZ              L07 [TRUE] 5678
    ' 5671:  6f 66 04 09             LOADW           G56,L03 -> L08
    ' 5675:  8c 00 06                JUMP            567c
    ' 5678:  6f 65 04 09             LOADW           G55,L03 -> L08
    ' 567c:  a0 08 c8                JZ              L07 [TRUE] 5685
    ' 567f:  e8 bf 09                PUSH            L08
    ' 5682:  8c 00 05                JUMP            5688
    ' 5685:  e8 bf 05                PUSH            L04
    ' 5688:  2d 86 00                STORE           G76,(SP)+
    ' 568b:  a0 08 c8                JZ              L07 [TRUE] 5694
    ' 568e:  e8 bf 05                PUSH            L04
    ' 5691:  8c 00 05                JUMP            5697
    ' 5694:  e8 bf 09                PUSH            L08
    ' 5697:  2d 87 00                STORE           G77,(SP)+
    ' 569a:  43 03 01 d1             JG              L02,#01 [TRUE] 56ad
    ' 569e:  4f 74 06 00             LOADW           G64,#06 -> -(SP)
    ' 56a2:  4f 00 00 00             LOADW           (SP)+,#00 -> -(SP)
    ' 56a6:  c1 8f 00 3b 7c 00 59    JE              (SP)+,"all" [FALSE] 5704
    ' 56ad:  41 09 0b 47             JE              L08,#0b [FALSE] 56b6
    ' 56b1:  95 8a                   INC             G7a
    ' 56b3:  8c ff 5c                JUMP            5610
    ' 56b6:  41 88 5d 5a             JE              G78,#5d [FALSE] 56d2
    ' 56ba:  a0 87 d7                JZ              G77 [TRUE] 56d2
    ' 56bd:  4f 74 06 00             LOADW           G64,#06 -> -(SP)
    ' 56c1:  4f 00 00 00             LOADW           (SP)+,#00 -> -(SP)
    ' 56c5:  c1 8f 00 3b 7c 49       JE              (SP)+,"all" [FALSE] 56d2
    ' 56cb:  66 86 87 c5             JIN             G76,G77 [TRUE] 56d2
    ' 56cf:  8c ff 40                JUMP            5610
    ' 56d2:  41 60 01 62             JE              G50,#01 [FALSE] 56f6
    ' 56d6:  41 88 5d 5e             JE              G78,#5d [FALSE] 56f6
    ' 56da:  a3 09 00                GET_PARENT      L08 -> -(SP)
    ' 56dd:  c1 ab 00 7f 10 ca       JE              (SP)+,G6f,G00 [TRUE] 56eb
    ' 56e3:  a3 09 00                GET_PARENT      L08 -> -(SP)
    ' 56e6:  4a 00 0a 3f 27          TEST_ATTR       (SP)+,#0a [FALSE] 5610
    ' 56eb:  4a 09 11 c9             TEST_ATTR       L08,#11 [TRUE] 56f6
    ' 56ef:  4a 09 0d c5             TEST_ATTR       L08,#0d [TRUE] 56f6
    ' 56f3:  8c ff 1c                JUMP            5610
    ' 56f6:  41 09 0c 47             JE              L08,#0c [FALSE] 56ff
    ' 56fa:  aa 7b                   PRINT_OBJ       G6b
    ' 56fc:  8c 00 04                JUMP            5701
    ' 56ff:  aa 09                   PRINT_OBJ       L08
    ' 5701:  b2 ...                  PRINT           ": "
    ' 5704:  0d 0a 01                STORE           L09,#01
    ' 5707:  e0 2a 2b be 88 86 87 07 CALL            577c (G78,G76,G77) -> L06
    ' 570f:  41 07 02 3e fe          JE              L06,#02 [FALSE] 5610
    ' 5714:  8c 00 02                JUMP            5717
    ' 5717:  41 07 02 ce             JE              L06,#02 [TRUE] 5727
    ' 571b:  a3 7f 00                GET_PARENT      G6f -> -(SP)
    ' 571e:  51 00 11 00             GET_PROP        (SP)+,#11 -> -(SP)
    ' 5722:  e0 9f 00 06 07          CALL            (SP)+ (#06) -> L06
    ' 5727:  c1 95 88 08 89 0f d5    JE              G78,#08,#89,#0f [TRUE] 5741
    ' 572e:  c1 95 88 0c 09 07 45    JE              G78,#0c,#09,#07 [FALSE] 5738
    ' 5735:  8c 00 0b                JUMP            5741
    ' 5738:  2d 8e 88                STORE           G7e,G78
    ' 573b:  2d 8d 86                STORE           G7d,G76
    ' 573e:  2d 8c 87                STORE           G7c,G77
    ' 5741:  41 07 02 4b             JE              L06,#02 [FALSE] 574e
    ' 5745:  0d 7c 00                STORE           G6c,#00
    ' 5748:  8c 00 05                JUMP            574e
    ' 574b:  0d 7c 00                STORE           G6c,#00
    ' 574e:  a0 8f bd ef             JZ              G7f [TRUE] 553f
    ' 5752:  c1 95 88 02 01 6f bd e7 JE              G78,#02,#01,#6f [TRUE] 553f
    ' 575a:  c1 95 88 0c 08 00 bd df JE              G78,#0c,#08,#00 [TRUE] 553f
    ' 5762:  c1 95 88 09 06 05 bd d7 JE              G78,#09,#06,#05 [TRUE] 553f
    ' 576a:  c1 95 88 07 0b 0a 45    JE              G78,#07,#0b,#0a [FALSE] 5774
    ' 5771:  8c fd cd                JUMP            553f
    ' 5774:  e0 3f 2a 62 07          CALL            54c4 -> L06
    ' 5779:  8c fd c5                JUMP            553f
#End Region

    <Fact>
    Sub Zork1_552A()
        Dim expected =
<![CDATA[
# temps: 75

LABEL 00
    temp00 <- L06
LABEL 01
    temp01 <- 00
    temp02 <- 00
    temp03 <- 01
    write-word(236f) <- call 5880 ()
    temp04 <- read-word(236f)
    if (temp04 = 0) is true then
        jump-to: LABEL 59
LABEL 02
    temp05 <- read-word(231b)
    temp06 <- read-word(2313)
    temp07 <- read-word(temp05 + (temp06 * 2))
    temp08 <- read-word(231d)
    temp09 <- read-word(2313)
    temp0a <- read-word(temp08 + (temp09 * 2))
    if (temp0a = 0) is false then
        jump-to: LABEL 04
LABEL 03
    push-SP: temp0a
    jump-to: LABEL 0c
LABEL 04
    if (int16(temp0a) > int16(01)) is false then
        jump-to: LABEL 09
LABEL 05
    if (temp07 = 0) is false then
        jump-to: LABEL 07
LABEL 06
    temp02 <- 00
    jump-to: LABEL 08
LABEL 07
    temp0b <- read-word(231b)
    temp02 <- read-word(temp0b + 2)
LABEL 08
    push-SP: temp0a
    jump-to: LABEL 0c
LABEL 09
    if (int16(temp07) > int16(01)) is false then
        jump-to: LABEL 0b
LABEL 0a
    temp03 <- 00
    temp0c <- read-word(231d)
    temp02 <- read-word(temp0c + 2)
    push-SP: temp07
    jump-to: LABEL 0c
LABEL 0b
    push-SP: 01
LABEL 0c
    temp0d <- pop-SP
    if (temp02 = 0) is false then
        jump-to: LABEL 0f
LABEL 0d
    if (temp07 = 01) is false then
        jump-to: LABEL 0f
LABEL 0e
    temp0e <- read-word(231b)
    temp02 <- read-word(temp0e + 2)
LABEL 0f
    temp0f <- read-word(2361)
    if (temp0f = 89) is false then
        jump-to: LABEL 11
LABEL 10
    temp10 <- read-word(2361)
    temp11 <- read-word(235d)
    temp00 <- call 577c (temp10, temp11)
    jump-to: LABEL 45
LABEL 11
    if (temp0d = 0) is false then
        jump-to: LABEL 17
LABEL 12
    temp12 <- read-word(2357)
    temp13 <- read-byte(temp12)
    temp14 <- (temp13 & 03)
    if (temp14 = 0) is false then
        jump-to: LABEL 14
LABEL 13
    temp15 <- read-word(2361)
    temp00 <- call 577c (temp15)
    write-word(235d) <- 00
    jump-to: LABEL 45
LABEL 14
    temp16 <- read-word(22f5)
    if (temp16 = 0) is false then
        jump-to: LABEL 16
LABEL 15
    print: "It's too dark to see."
    print: "\n"
    jump-to: LABEL 45
LABEL 16
    print: "It's not clear what you're referring to."
    print: "\n"
    temp00 <- 00
    jump-to: LABEL 45
LABEL 17
    write-word(2365) <- 00
    write-word(2367) <- 00
    if (int16(temp0d) > int16(01)) is false then
        jump-to: LABEL 19
LABEL 18
    write-word(2367) <- 01
LABEL 19
    temp17 <- 00
LABEL 1a
    temp18 <- (int16(temp01) + int16(1))
    temp01 <- temp18
    if (int16(temp18) > int16(temp0d)) is false then
        jump-to: LABEL 26
LABEL 1b
    temp19 <- read-word(2365)
    if (int16(temp19) > int16(00)) is false then
        jump-to: LABEL 24
LABEL 1c
    print: "The "
    temp1a <- read-word(2365)
    if (temp1a = temp0d) is true then
        jump-to: LABEL 1e
LABEL 1d
    print: "other "
LABEL 1e
    print: "object"
    temp1b <- read-word(2365)
    if (temp1b = 01) is true then
        jump-to: LABEL 20
LABEL 1f
    print: "s"
LABEL 20
    print: " that you mentioned "
    temp1c <- read-word(2365)
    if (temp1c = 01) is true then
        jump-to: LABEL 22
LABEL 21
    print: "are"
    jump-to: LABEL 23
LABEL 22
    print: "is"
LABEL 23
    print: "n't here."
    print: "\n"
    jump-to: LABEL 45
LABEL 24
    if (temp17 = 0) is false then
        jump-to: LABEL 45
LABEL 25
    print: "There's nothing here you can take."
    print: "\n"
    jump-to: LABEL 45
LABEL 26
    if (temp03 = 0) is true then
        jump-to: LABEL 28
LABEL 27
    temp1d <- read-word(231d)
    temp1e <- read-word(temp1d + (temp18 * 2))
    jump-to: LABEL 29
LABEL 28
    temp1f <- read-word(231b)
    temp1e <- read-word(temp1f + (temp18 * 2))
LABEL 29
    if (temp03 = 0) is true then
        jump-to: LABEL 2b
LABEL 2a
    push-SP: temp1e
    jump-to: LABEL 2c
LABEL 2b
    push-SP: temp02
LABEL 2c
    temp20 <- pop-SP
    write-word(235d) <- temp20
    if (temp03 = 0) is true then
        jump-to: LABEL 2e
LABEL 2d
    push-SP: temp02
    jump-to: LABEL 2f
LABEL 2e
    push-SP: temp1e
LABEL 2f
    temp21 <- pop-SP
    write-word(235f) <- temp21
    if (int16(temp0d) > int16(01)) is true then
        jump-to: LABEL 31
LABEL 30
    temp22 <- read-word(2339)
    temp23 <- read-word(temp22 + c)
    temp24 <- read-word(temp23)
    if (temp24 = 3b7c) is false then
        jump-to: LABEL 43
LABEL 31
    if (temp1e = 0b) is false then
        jump-to: LABEL 33
LABEL 32
    temp25 <- read-word(2365)
    write-word(2365) <- (int16(temp25) + int16(1))
    jump-to: LABEL 1a
LABEL 33
    temp26 <- read-word(2361)
    if (temp26 = 5d) is false then
        jump-to: LABEL 38
LABEL 34
    temp27 <- read-word(235f)
    if (temp27 = 0) is true then
        jump-to: LABEL 38
LABEL 35
    temp28 <- read-word(2339)
    temp29 <- read-word(temp28 + c)
    temp2a <- read-word(temp29)
    if (temp2a = 3b7c) is false then
        jump-to: LABEL 38
LABEL 36
    temp2b <- read-word(235d)
    temp2c <- read-word(235f)
    if (read-byte((((temp2b - 1) * 9) + 2ee) + 4) = temp2c) is true then
        jump-to: LABEL 38
LABEL 37
    jump-to: LABEL 1a
LABEL 38
    temp2d <- read-word(2311)
    if (temp2d = 01) is false then
        jump-to: LABEL 3f
LABEL 39
    temp2e <- read-word(2361)
    if (temp2e = 5d) is false then
        jump-to: LABEL 3f
LABEL 3a
    temp2f <- read-byte((((temp1e - 1) * 9) + 2ee) + 4)
    temp30 <- read-word(234f)
    temp31 <- read-word(2271)
    if ((temp2f = temp30) | (temp2f = temp31)) is true then
        jump-to: LABEL 3c
LABEL 3b
    temp32 <- read-byte((((temp1e - 1) * 9) + 2ee) + 4)
    if (((read-byte((((temp32 - 1) * 9) + 2ee) + 1) & 0020) <> 0) = 1) is false then
        jump-to: LABEL 1a
LABEL 3c
    if (((read-byte((((temp1e - 1) * 9) + 2ee) + 2) & 0040) <> 0) = 1) is true then
        jump-to: LABEL 3f
LABEL 3d
    if (((read-byte((((temp1e - 1) * 9) + 2ee) + 1) & 0004) <> 0) = 1) is true then
        jump-to: LABEL 3f
LABEL 3e
    jump-to: LABEL 1a
LABEL 3f
    if (temp1e = 0c) is false then
        jump-to: LABEL 41
LABEL 40
    temp33 <- read-word(2347)
    temp34 <- read-word((((temp33 - 1) * 9) + 2ee) + 7)
    print: read-text(temp34 + 1, read-byte(temp34))
    jump-to: LABEL 42
LABEL 41
    temp35 <- read-word((((temp1e - 1) * 9) + 2ee) + 7)
    print: read-text(temp35 + 1, read-byte(temp35))
LABEL 42
    print: ": "
LABEL 43
    temp17 <- 01
    temp36 <- read-word(2361)
    temp37 <- read-word(235d)
    temp38 <- read-word(235f)
    temp00 <- call 577c (temp36, temp37, temp38)
    if (temp00 = 02) is false then
        jump-to: LABEL 1a
LABEL 44
    jump-to: LABEL 45
LABEL 45
    if (temp00 = 02) is true then
        jump-to: LABEL 53
LABEL 46
    temp39 <- read-word(234f)
    temp3a <- read-byte((((temp39 - 1) * 9) + 2ee) + 4)
    temp3b <- read-word((((temp3a - 1) * 9) + 2ee) + 7)
    temp3c <- uint16((temp3b + 1) + (read-byte(temp3b) * 2))
    temp3d <- 0
LABEL 47
    temp3e <- read-byte(temp3c)
    if ((temp3e & 1f) <= 11) is false then
        jump-to: LABEL 49
LABEL 48
    temp3d <- 1
    jump-to: LABEL 4a
LABEL 49
    temp3f <- read-byte(temp3c)
    temp3c <- uint16((temp3c + 1) + ((temp3f >> 5) + 1))
LABEL 4a
    if (temp3d = 0) is true then
        jump-to: LABEL 47
    if ((temp3e & 1f) = 11) is false then
        jump-to: LABEL 4f
LABEL 4b
    temp3c <- (temp3c + 1)
    if ((temp3e & e0) = 0) is false then
        jump-to: LABEL 4d
LABEL 4c
    temp40 <- read-byte(temp3c)
    jump-to: LABEL 4e
LABEL 4d
    temp40 <- read-word(temp3c)
LABEL 4e
    jump-to: LABEL 50
LABEL 4f
    temp40 <- uint16(read-word(2d0))
LABEL 50
    if (temp40 = 0) is false then
        jump-to: LABEL 52
LABEL 51
    temp00 <- 0
    jump-to: LABEL 53
LABEL 52
    temp00 <- call (temp40 * 2) (06)
LABEL 53
    temp41 <- read-word(2361)
    if (((temp41 = 08) | (temp41 = 89)) | (temp41 = 0f)) is true then
        jump-to: LABEL 57
LABEL 54
    temp42 <- read-word(2361)
    if (((temp42 = 0c) | (temp42 = 09)) | (temp42 = 07)) is false then
        jump-to: LABEL 56
LABEL 55
    jump-to: LABEL 57
LABEL 56
    temp43 <- read-word(2361)
    write-word(236d) <- temp43
    temp44 <- read-word(235d)
    write-word(236b) <- temp44
    temp45 <- read-word(235f)
    write-word(2369) <- temp45
LABEL 57
    if (temp00 = 02) is false then
        jump-to: LABEL 5a
LABEL 58
    write-word(2349) <- 00
    jump-to: LABEL 5a
LABEL 59
    write-word(2349) <- 00
LABEL 5a
    temp46 <- read-word(236f)
    if (temp46 = 0) is true then
        jump-to: LABEL 01
LABEL 5b
    temp47 <- read-word(2361)
    if (((temp47 = 02) | (temp47 = 01)) | (temp47 = 6f)) is true then
        jump-to: LABEL 01
LABEL 5c
    temp48 <- read-word(2361)
    if (((temp48 = 0c) | (temp48 = 08)) | (temp48 = 00)) is true then
        jump-to: LABEL 01
LABEL 5d
    temp49 <- read-word(2361)
    if (((temp49 = 09) | (temp49 = 06)) | (temp49 = 05)) is true then
        jump-to: LABEL 01
LABEL 5e
    temp4a <- read-word(2361)
    if (((temp4a = 07) | (temp4a = 0b)) | (temp4a = 0a)) is false then
        jump-to: LABEL 60
LABEL 5f
    jump-to: LABEL 01
LABEL 60
    temp00 <- call 54c4 ()
    jump-to: LABEL 01
]]>

        TestBinder(Zork1, &H552A, expected)
    End Sub

#End Region
#Region "Zork1_577C"

#Region "ZCode"
    ' 578b:  2d 05 88                STORE           L04,G78
    ' 578e:  2d 06 86                STORE           L05,G76
    ' 5791:  2d 07 87                STORE           L06,G77
    ' 5794:  c1 6b 0c 03 02 60       JE              #0c,L02,L01 [FALSE] 57b8
    ' 579a:  61 7a 10 dc             JE              G6a,G00 [TRUE] 57b8
    ' 579e:  b2 ...                  PRINT           "I don't see what you are
    'referring to."
    ' 57b5:  bb                      NEW_LINE        
    ' 57b6:  9b 02                   RET             #02
    ' 57b8:  41 02 0c 45             JE              L01,#0c [FALSE] 57bf
    ' 57bc:  2d 02 7b                STORE           L01,G6b
    ' 57bf:  41 03 0c 45             JE              L02,#0c [FALSE] 57c6
    ' 57c3:  2d 03 7b                STORE           L02,G6b
    ' 57c6:  2d 88 01                STORE           G78,L00
    ' 57c9:  2d 86 02                STORE           G76,L01
    ' 57cc:  a0 86 d0                JZ              G76 [TRUE] 57dd
    ' 57cf:  41 87 0c cc             JE              G77,#0c [TRUE] 57dd
    ' 57d3:  41 88 89 c8             JE              G78,#89 [TRUE] 57dd
    ' 57d7:  2d 7b 86                STORE           G6b,G76
    ' 57da:  2d 7a 10                STORE           G6a,G00
    ' 57dd:  2d 87 03                STORE           G77,L02
    ' 57e0:  c1 6b 0b 86 87 4d       JE              #0b,G76,G77 [FALSE] 57f1
    ' 57e6:  e0 3f 27 d1 04          CALL            4fa2 -> L03
    ' 57eb:  a0 04 c5                JZ              L03 [TRUE] 57f1
    ' 57ee:  8c 00 85                JUMP            5874
    ' 57f1:  2d 02 86                STORE           L01,G76
    ' 57f4:  2d 03 87                STORE           L02,G77
    ' 57f7:  51 7f 11 00             GET_PROP        G6f,#11 -> -(SP)
    ' 57fb:  e0 bf 00 04             CALL            (SP)+ -> L03
    ' 57ff:  a0 04 c5                JZ              L03 [TRUE] 5805
    ' 5802:  8c 00 71                JUMP            5874
    ' 5805:  a3 7f 00                GET_PARENT      G6f -> -(SP)
    ' 5808:  51 00 11 00             GET_PROP        (SP)+,#11 -> -(SP)
    ' 580c:  e0 9f 00 01 04          CALL            (SP)+ (#01) -> L03
    ' 5811:  a0 04 c5                JZ              L03 [TRUE] 5817
    ' 5814:  8c 00 5f                JUMP            5874
    ' 5817:  6f ac 01 00             LOADW           G9c,L00 -> -(SP)
    ' 581b:  e0 bf 00 04             CALL            (SP)+ -> L03
    ' 581f:  a0 04 c5                JZ              L03 [TRUE] 5825
    ' 5822:  8c 00 51                JUMP            5874
    ' 5825:  a0 03 d0                JZ              L02 [TRUE] 5836
    ' 5828:  51 03 11 00             GET_PROP        L02,#11 -> -(SP)
    ' 582c:  e0 bf 00 04             CALL            (SP)+ -> L03
    ' 5830:  a0 04 c5                JZ              L03 [TRUE] 5836
    ' 5833:  8c 00 40                JUMP            5874
    ' 5836:  a0 02 dd                JZ              L01 [TRUE] 5854
    ' 5839:  41 01 89 d9             JE              L00,#89 [TRUE] 5854
    ' 583d:  a3 02 00                GET_PARENT      L01 -> -(SP)
    ' 5840:  a0 00 d3                JZ              (SP)+ [TRUE] 5854
    ' 5843:  a3 02 00                GET_PARENT      L01 -> -(SP)
    ' 5846:  51 00 02 00             GET_PROP        (SP)+,#02 -> -(SP)
    ' 584a:  e0 bf 00 04             CALL            (SP)+ -> L03
    ' 584e:  a0 04 c5                JZ              L03 [TRUE] 5854
    ' 5851:  8c 00 22                JUMP            5874
    ' 5854:  a0 02 d4                JZ              L01 [TRUE] 5869
    ' 5857:  41 01 89 d0             JE              L00,#89 [TRUE] 5869
    ' 585b:  51 02 11 00             GET_PROP        L01,#11 -> -(SP)
    ' 585f:  e0 bf 00 04             CALL            (SP)+ -> L03
    ' 5863:  a0 04 c5                JZ              L03 [TRUE] 5869
    ' 5866:  8c 00 0d                JUMP            5874
    ' 5869:  6f ab 01 00             LOADW           G9b,L00 -> -(SP)
    ' 586d:  e0 bf 00 04             CALL            (SP)+ -> L03
    ' 5871:  a0 04 c2                JZ              L03 [TRUE] 5874
    ' 5874:  2d 88 05                STORE           G78,L04
    ' 5877:  2d 86 06                STORE           G76,L05
    ' 587a:  2d 87 07                STORE           G77,L06
    ' 587d:  ab 04                   RET             L03
#End Region

    <Fact>
    Sub Zork1_577C()
        Dim expected =
<![CDATA[
# temps: 59

LABEL 00
    temp00 <- L00
    temp01 <- L01
    temp02 <- L02
LABEL 01
    temp03 <- read-word(2361)
    temp04 <- read-word(235d)
    temp05 <- read-word(235f)
    if ((0c = temp02) | (0c = temp01)) is false then
        jump-to: LABEL 04
LABEL 02
    temp06 <- read-word(2345)
    temp07 <- read-word(2271)
    if (temp06 = temp07) is true then
        jump-to: LABEL 04
LABEL 03
    print: "I don't see what you are referring to."
    print: "\n"
    return: 02
LABEL 04
    if (temp01 = 0c) is false then
        jump-to: LABEL 06
LABEL 05
    temp08 <- read-word(2347)
    temp01 <- temp08
LABEL 06
    if (temp02 = 0c) is false then
        jump-to: LABEL 08
LABEL 07
    temp09 <- read-word(2347)
    temp02 <- temp09
LABEL 08
    write-word(2361) <- temp00
    write-word(235d) <- temp01
    temp0a <- read-word(235d)
    if (temp0a = 0) is true then
        jump-to: LABEL 0c
LABEL 09
    temp0b <- read-word(235f)
    if (temp0b = 0c) is true then
        jump-to: LABEL 0c
LABEL 0a
    temp0c <- read-word(2361)
    if (temp0c = 89) is true then
        jump-to: LABEL 0c
LABEL 0b
    temp0d <- read-word(235d)
    write-word(2347) <- temp0d
    temp0e <- read-word(2271)
    write-word(2345) <- temp0e
LABEL 0c
    write-word(235f) <- temp02
    temp0f <- read-word(235d)
    temp10 <- read-word(235f)
    if ((0b = temp0f) | (0b = temp10)) is false then
        jump-to: LABEL 0f
LABEL 0d
    temp11 <- call 4fa2 ()
    if (temp11 = 0) is true then
        jump-to: LABEL 0f
LABEL 0e
    jump-to: LABEL 69
LABEL 0f
    temp12 <- read-word(235d)
    temp13 <- read-word(235f)
    temp14 <- read-word(234f)
    temp15 <- read-word((((temp14 - 1) * 9) + 2ee) + 7)
    temp16 <- uint16((temp15 + 1) + (read-byte(temp15) * 2))
    temp17 <- 0
LABEL 10
    temp18 <- read-byte(temp16)
    if ((temp18 & 1f) <= 11) is false then
        jump-to: LABEL 12
LABEL 11
    temp17 <- 1
    jump-to: LABEL 13
LABEL 12
    temp19 <- read-byte(temp16)
    temp16 <- uint16((temp16 + 1) + ((temp19 >> 5) + 1))
LABEL 13
    if (temp17 = 0) is true then
        jump-to: LABEL 10
    if ((temp18 & 1f) = 11) is false then
        jump-to: LABEL 18
LABEL 14
    temp16 <- (temp16 + 1)
    if ((temp18 & e0) = 0) is false then
        jump-to: LABEL 16
LABEL 15
    temp1a <- read-byte(temp16)
    jump-to: LABEL 17
LABEL 16
    temp1a <- read-word(temp16)
LABEL 17
    jump-to: LABEL 19
LABEL 18
    temp1a <- uint16(read-word(2d0))
LABEL 19
    if (temp1a = 0) is false then
        jump-to: LABEL 1b
LABEL 1a
    temp11 <- 0
    jump-to: LABEL 1c
LABEL 1b
    temp11 <- call (temp1a * 2) ()
LABEL 1c
    if (temp11 = 0) is true then
        jump-to: LABEL 1e
LABEL 1d
    jump-to: LABEL 69
LABEL 1e
    temp1b <- read-word(234f)
    temp1c <- read-byte((((temp1b - 1) * 9) + 2ee) + 4)
    temp1d <- read-word((((temp1c - 1) * 9) + 2ee) + 7)
    temp1e <- uint16((temp1d + 1) + (read-byte(temp1d) * 2))
    temp1f <- 0
LABEL 1f
    temp20 <- read-byte(temp1e)
    if ((temp20 & 1f) <= 11) is false then
        jump-to: LABEL 21
LABEL 20
    temp1f <- 1
    jump-to: LABEL 22
LABEL 21
    temp21 <- read-byte(temp1e)
    temp1e <- uint16((temp1e + 1) + ((temp21 >> 5) + 1))
LABEL 22
    if (temp1f = 0) is true then
        jump-to: LABEL 1f
    if ((temp20 & 1f) = 11) is false then
        jump-to: LABEL 27
LABEL 23
    temp1e <- (temp1e + 1)
    if ((temp20 & e0) = 0) is false then
        jump-to: LABEL 25
LABEL 24
    temp22 <- read-byte(temp1e)
    jump-to: LABEL 26
LABEL 25
    temp22 <- read-word(temp1e)
LABEL 26
    jump-to: LABEL 28
LABEL 27
    temp22 <- uint16(read-word(2d0))
LABEL 28
    if (temp22 = 0) is false then
        jump-to: LABEL 2a
LABEL 29
    temp11 <- 0
    jump-to: LABEL 2b
LABEL 2a
    temp11 <- call (temp22 * 2) (01)
LABEL 2b
    if (temp11 = 0) is true then
        jump-to: LABEL 2d
LABEL 2c
    jump-to: LABEL 69
LABEL 2d
    temp23 <- read-word(23a9)
    temp24 <- read-word(temp23 + (temp00 * 2))
    if (temp24 = 0) is false then
        jump-to: LABEL 2f
LABEL 2e
    temp11 <- 0
    jump-to: LABEL 30
LABEL 2f
    temp11 <- call (temp24 * 2) ()
LABEL 30
    if (temp11 = 0) is true then
        jump-to: LABEL 32
LABEL 31
    jump-to: LABEL 69
LABEL 32
    if (temp13 = 0) is true then
        jump-to: LABEL 42
LABEL 33
    temp25 <- read-word((((temp13 - 1) * 9) + 2ee) + 7)
    temp26 <- uint16((temp25 + 1) + (read-byte(temp25) * 2))
    temp27 <- 0
LABEL 34
    temp28 <- read-byte(temp26)
    if ((temp28 & 1f) <= 11) is false then
        jump-to: LABEL 36
LABEL 35
    temp27 <- 1
    jump-to: LABEL 37
LABEL 36
    temp29 <- read-byte(temp26)
    temp26 <- uint16((temp26 + 1) + ((temp29 >> 5) + 1))
LABEL 37
    if (temp27 = 0) is true then
        jump-to: LABEL 34
    if ((temp28 & 1f) = 11) is false then
        jump-to: LABEL 3c
LABEL 38
    temp26 <- (temp26 + 1)
    if ((temp28 & e0) = 0) is false then
        jump-to: LABEL 3a
LABEL 39
    temp2a <- read-byte(temp26)
    jump-to: LABEL 3b
LABEL 3a
    temp2a <- read-word(temp26)
LABEL 3b
    jump-to: LABEL 3d
LABEL 3c
    temp2a <- uint16(read-word(2d0))
LABEL 3d
    if (temp2a = 0) is false then
        jump-to: LABEL 3f
LABEL 3e
    temp11 <- 0
    jump-to: LABEL 40
LABEL 3f
    temp11 <- call (temp2a * 2) ()
LABEL 40
    if (temp11 = 0) is true then
        jump-to: LABEL 42
LABEL 41
    jump-to: LABEL 69
LABEL 42
    if (temp12 = 0) is true then
        jump-to: LABEL 54
LABEL 43
    if (temp00 = 89) is true then
        jump-to: LABEL 54
LABEL 44
    temp2b <- read-byte((((temp12 - 1) * 9) + 2ee) + 4)
    if (temp2b = 0) is true then
        jump-to: LABEL 54
LABEL 45
    temp2c <- read-byte((((temp12 - 1) * 9) + 2ee) + 4)
    temp2d <- read-word((((temp2c - 1) * 9) + 2ee) + 7)
    temp2e <- uint16((temp2d + 1) + (read-byte(temp2d) * 2))
    temp2f <- 0
LABEL 46
    temp30 <- read-byte(temp2e)
    if ((temp30 & 1f) <= 02) is false then
        jump-to: LABEL 48
LABEL 47
    temp2f <- 1
    jump-to: LABEL 49
LABEL 48
    temp31 <- read-byte(temp2e)
    temp2e <- uint16((temp2e + 1) + ((temp31 >> 5) + 1))
LABEL 49
    if (temp2f = 0) is true then
        jump-to: LABEL 46
    if ((temp30 & 1f) = 02) is false then
        jump-to: LABEL 4e
LABEL 4a
    temp2e <- (temp2e + 1)
    if ((temp30 & e0) = 0) is false then
        jump-to: LABEL 4c
LABEL 4b
    temp32 <- read-byte(temp2e)
    jump-to: LABEL 4d
LABEL 4c
    temp32 <- read-word(temp2e)
LABEL 4d
    jump-to: LABEL 4f
LABEL 4e
    temp32 <- uint16(read-word(2b2))
LABEL 4f
    if (temp32 = 0) is false then
        jump-to: LABEL 51
LABEL 50
    temp11 <- 0
    jump-to: LABEL 52
LABEL 51
    temp11 <- call (temp32 * 2) ()
LABEL 52
    if (temp11 = 0) is true then
        jump-to: LABEL 54
LABEL 53
    jump-to: LABEL 69
LABEL 54
    if (temp12 = 0) is true then
        jump-to: LABEL 65
LABEL 55
    if (temp00 = 89) is true then
        jump-to: LABEL 65
LABEL 56
    temp33 <- read-word((((temp12 - 1) * 9) + 2ee) + 7)
    temp34 <- uint16((temp33 + 1) + (read-byte(temp33) * 2))
    temp35 <- 0
LABEL 57
    temp36 <- read-byte(temp34)
    if ((temp36 & 1f) <= 11) is false then
        jump-to: LABEL 59
LABEL 58
    temp35 <- 1
    jump-to: LABEL 5a
LABEL 59
    temp37 <- read-byte(temp34)
    temp34 <- uint16((temp34 + 1) + ((temp37 >> 5) + 1))
LABEL 5a
    if (temp35 = 0) is true then
        jump-to: LABEL 57
    if ((temp36 & 1f) = 11) is false then
        jump-to: LABEL 5f
LABEL 5b
    temp34 <- (temp34 + 1)
    if ((temp36 & e0) = 0) is false then
        jump-to: LABEL 5d
LABEL 5c
    temp38 <- read-byte(temp34)
    jump-to: LABEL 5e
LABEL 5d
    temp38 <- read-word(temp34)
LABEL 5e
    jump-to: LABEL 60
LABEL 5f
    temp38 <- uint16(read-word(2d0))
LABEL 60
    if (temp38 = 0) is false then
        jump-to: LABEL 62
LABEL 61
    temp11 <- 0
    jump-to: LABEL 63
LABEL 62
    temp11 <- call (temp38 * 2) ()
LABEL 63
    if (temp11 = 0) is true then
        jump-to: LABEL 65
LABEL 64
    jump-to: LABEL 69
LABEL 65
    temp39 <- read-word(23a7)
    temp3a <- read-word(temp39 + (temp00 * 2))
    if (temp3a = 0) is false then
        jump-to: LABEL 67
LABEL 66
    temp11 <- 0
    jump-to: LABEL 68
LABEL 67
    temp11 <- call (temp3a * 2) ()
LABEL 68
    if (temp11 = 0) is true then
        jump-to: LABEL 69
LABEL 69
    write-word(2361) <- temp03
    write-word(235d) <- temp04
    write-word(235f) <- temp05
    return: temp11
]]>

        TestBinder(Zork1, &H577C, expected)
    End Sub

#End Region
#Region "Zork1_6A52"

#Region "ZCode"
    ' 6a5d:  a2 01 01 40             GET_CHILD       L00 -> L00 [FALSE] RFALSE
    ' 6a61:  41 03 02 da             JE              L02,#02 [TRUE] 6a7d
    ' 6a65:  52 01 12 00             GET_PROP_ADDR   L00,#12 -> -(SP)
    ' 6a69:  a0 00 d3                JZ              (SP)+ [TRUE] 6a7d
    ' 6a6c:  e0 2b 36 8e 01 02 00    CALL            6d1c (L00,L01) -> -(SP)
    ' 6a73:  a0 00 c9                JZ              (SP)+ [TRUE] 6a7d
    ' 6a76:  e0 2b 35 5d 01 02 00    CALL            6aba (L00,L01) -> -(SP)
    ' 6a7d:  41 03 00 4a             JE              L02,#00 [FALSE] 6a89
    ' 6a81:  4a 01 08 c6             TEST_ATTR       L00,#08 [TRUE] 6a89
    ' 6a85:  4a 01 0a 6d             TEST_ATTR       L00,#0a [FALSE] 6ab4
    ' 6a89:  a2 01 05 69             GET_CHILD       L00 -> L04 [FALSE] 6ab4
    ' 6a8d:  4a 01 0b c6             TEST_ATTR       L00,#0b [TRUE] 6a95
    ' 6a91:  4a 01 0c 61             TEST_ATTR       L00,#0c [FALSE] 6ab4
    ' 6a95:  4a 01 0a 48             TEST_ATTR       L00,#0a [FALSE] 6a9f
    ' 6a99:  e8 7f 01                PUSH            #01
    ' 6a9c:  8c 00 0f                JUMP            6aac
    ' 6a9f:  4a 01 08 48             TEST_ATTR       L00,#08 [FALSE] 6aa9
    ' 6aa3:  e8 7f 01                PUSH            #01
    ' 6aa6:  8c 00 05                JUMP            6aac
    ' 6aa9:  e8 7f 00                PUSH            #00
    ' 6aac:  e0 2a 35 29 01 02 00 04 CALL            6a52 (L00,L01,(SP)+) -> L03
    ' 6ab4:  a1 01 01 bf aa          GET_SIBLING     L00 -> L00 [TRUE] 6a61
    ' 6ab9:  b0                      RTRUE
#End Region

    <Fact>
    Sub Zork1_6A52()
        Dim expected =
<![CDATA[
# temps: 14

LABEL 00
    temp00 <- L00
    temp01 <- L01
    temp02 <- L02
LABEL 01
    temp03 <- temp00
    temp00 <- read-byte((((temp03 - 1) * 9) + 2ee) + 6)
    if (read-byte((((temp03 - 1) * 9) + 2ee) + 6) <> 0) is false then
        return: 0
LABEL 02
    if (temp02 = 02) is true then
        jump-to: LABEL 0d
LABEL 03
    temp04 <- read-word((((temp00 - 1) * 9) + 2ee) + 7)
    temp05 <- uint16((temp04 + 1) + (read-byte(temp04) * 2))
    temp06 <- 0
LABEL 04
    temp07 <- read-byte(temp05)
    if ((temp07 & 1f) <= 12) is false then
        jump-to: LABEL 06
LABEL 05
    temp06 <- 1
    jump-to: LABEL 07
LABEL 06
    temp08 <- read-byte(temp05)
    temp05 <- uint16((temp05 + 1) + ((temp08 >> 5) + 1))
LABEL 07
    if (temp06 = 0) is true then
        jump-to: LABEL 04
    if ((temp07 & 1f) = 12) is false then
        jump-to: LABEL 09
LABEL 08
    push-SP: (temp05 + 1)
    jump-to: LABEL 0a
LABEL 09
    push-SP: 0
LABEL 0a
    temp09 <- pop-SP
    if (temp09 = 0) is true then
        jump-to: LABEL 0d
LABEL 0b
    temp0a <- call 6d1c (temp00, temp01)
    if (temp0a = 0) is true then
        jump-to: LABEL 0d
LABEL 0c
    push-SP: call 6aba (temp00, temp01)
LABEL 0d
    if (temp02 = 00) is false then
        jump-to: LABEL 10
LABEL 0e
    if (((read-byte((((temp00 - 1) * 9) + 2ee) + 1) & 0080) <> 0) = 1) is true then
        jump-to: LABEL 10
LABEL 0f
    if (((read-byte((((temp00 - 1) * 9) + 2ee) + 1) & 0020) <> 0) = 1) is false then
        jump-to: LABEL 19
LABEL 10
    if (read-byte((((temp00 - 1) * 9) + 2ee) + 6) <> 0) is false then
        jump-to: LABEL 19
LABEL 11
    if (((read-byte((((temp00 - 1) * 9) + 2ee) + 1) & 0010) <> 0) = 1) is true then
        jump-to: LABEL 13
LABEL 12
    if (((read-byte((((temp00 - 1) * 9) + 2ee) + 1) & 0008) <> 0) = 1) is false then
        jump-to: LABEL 19
LABEL 13
    if (((read-byte((((temp00 - 1) * 9) + 2ee) + 1) & 0020) <> 0) = 1) is false then
        jump-to: LABEL 15
LABEL 14
    push-SP: 01
    jump-to: LABEL 18
LABEL 15
    if (((read-byte((((temp00 - 1) * 9) + 2ee) + 1) & 0080) <> 0) = 1) is false then
        jump-to: LABEL 17
LABEL 16
    push-SP: 01
    jump-to: LABEL 18
LABEL 17
    push-SP: 00
LABEL 18
    temp0b <- pop-SP
    temp0c <- call 6a52 (temp00, temp01, temp0b)
LABEL 19
    temp0d <- temp00
    temp00 <- read-byte((((temp0d - 1) * 9) + 2ee) + 5)
    if (read-byte((((temp0d - 1) * 9) + 2ee) + 5) <> 0) is true then
        jump-to: LABEL 02
LABEL 1a
    return: 1
]]>

        TestBinder(Zork1, &H6A52, expected)
    End Sub

#End Region
#Region "Zork1_8C9A"

#Region "ZCode"
    ' 8ca3:  a0 01 c8                JZ              L00 [TRUE] 8cac
    ' 8ca6:  e8 bf 01                PUSH            L00
    ' 8ca9:  8c 00 05                JUMP            8caf
    ' 8cac:  e8 bf 57                PUSH            G47
    ' 8caf:  e9 7f 02                PULL            L01
    ' 8cb2:  a0 52 71                JZ              G42 [FALSE] 8ce4
    ' 8cb5:  b2 ...                  PRINT           "It is pitch black."
    ' 8cc2:  a0 51 59                JZ              G41 [FALSE] 8cdc
    ' 8cc5:  b2 ...                  PRINT           " You are likely to be eaten by a grue."
    ' 8cdc:  bb                      NEW_LINE
    ' 8cdd:  e0 3f 28 68 00          CALL            50d0 -> -(SP)
    ' 8ce2:  9b 00                   RET             #00
    ' 8ce4:  4a 10 03 c8             TEST_ATTR       G00,#03 [TRUE] 8cee
    ' 8ce8:  4b 10 03                SET_ATTR        G00,#03
    ' 8ceb:  0d 02 01                STORE           L01,#01
    ' 8cee:  4a 10 05 45             TEST_ATTR       G00,#05 [FALSE] 8cf5
    ' 8cf2:  4c 10 03                CLEAR_ATTR      G00,#03
    ' 8cf5:  46 10 52 53             JIN             G00,#52 [FALSE] 8d0a
    ' 8cf9:  aa 10                   PRINT_OBJ       G00
    ' 8cfb:  a3 7f 04                GET_PARENT      G6f -> L03
    ' 8cfe:  4a 04 1b 49             TEST_ATTR       L03,#1b [FALSE] 8d09
    ' 8d02:  b2 ...                  PRINT           ", in the "
    ' 8d07:  aa 04                   PRINT_OBJ       L03
    ' 8d09:  bb                      NEW_LINE        
    ' 8d0a:  a0 01 45                JZ              L00 [FALSE] 8d10
    ' 8d0d:  a0 56 41                JZ              G46 [FALSE] RTRUE
    ' 8d10:  a3 7f 04                GET_PARENT      G6f -> L03
    ' 8d13:  a0 02 ce                JZ              L01 [TRUE] 8d22
    ' 8d16:  51 10 11 00             GET_PROP        G00,#11 -> -(SP)
    ' 8d1a:  e0 9f 00 03 00          CALL            (SP)+ (#03) -> -(SP)
    ' 8d1f:  a0 00 41                JZ              (SP)+ [FALSE] RTRUE
    ' 8d22:  a0 02 cf                JZ              L01 [TRUE] 8d32
    ' 8d25:  51 10 0b 03             GET_PROP        G00,#0b -> L02
    ' 8d29:  a0 03 c8                JZ              L02 [TRUE] 8d32
    ' 8d2c:  ad 03                   PRINT_PADDR     L02
    ' 8d2e:  bb                      NEW_LINE        
    ' 8d2f:  8c 00 0b                JUMP            8d3b
    ' 8d32:  51 10 11 00             GET_PROP        G00,#11 -> -(SP)
    ' 8d36:  e0 9f 00 04 00          CALL            (SP)+ (#04) -> -(SP)
    ' 8d3b:  61 10 04 c1             JE              G00,L03 [TRUE] RTRUE
    ' 8d3f:  4a 04 1b 41             TEST_ATTR       L03,#1b [FALSE] RTRUE
    ' 8d43:  51 04 11 00             GET_PROP        L03,#11 -> -(SP)
    ' 8d47:  e0 9f 00 03 00          CALL            (SP)+ (#03) -> -(SP)
    ' 8d4c:  b0                      RTRUE
#End Region

    <Fact>
    Sub Zork1_8C9A()
        Dim expected =
<![CDATA[
# temps: 49

LABEL 00
    temp00 <- L00
LABEL 01
    if (temp00 = 0) is true then
        jump-to: LABEL 03
LABEL 02
    push-SP: temp00
    jump-to: LABEL 04
LABEL 03
    temp01 <- read-word(22ff)
    push-SP: temp01
LABEL 04
    temp02 <- pop-SP
    temp03 <- temp02
    temp04 <- read-word(22f5)
    if (temp04 = 0) is false then
        jump-to: LABEL 08
LABEL 05
    print: "It is pitch black."
    temp05 <- read-word(22f3)
    if (temp05 = 0) is false then
        jump-to: LABEL 07
LABEL 06
    print: " You are likely to be eaten by a grue."
LABEL 07
    print: "\n"
    push-SP: call 50d0 ()
    return: 00
LABEL 08
    temp06 <- read-word(2271)
    if (((read-byte(((temp06 - 1) * 9) + 2ee) & 0010) <> 0) = 1) is true then
        jump-to: LABEL 0a
LABEL 09
    temp07 <- read-word(2271)
    temp08 <- (((temp07 - 1) * 9) + 2ee)
    write-byte(temp08) <- byte(read-byte(temp08) | 0010)
    temp03 <- 01
LABEL 0a
    temp09 <- read-word(2271)
    if (((read-byte(((temp09 - 1) * 9) + 2ee) & 0004) <> 0) = 1) is false then
        jump-to: LABEL 0c
LABEL 0b
    temp0a <- read-word(2271)
    temp0b <- (((temp0a - 1) * 9) + 2ee)
    write-byte(temp0b) <- byte(read-byte(temp0b) & not 0010)
LABEL 0c
    temp0c <- read-word(2271)
    if (read-byte((((temp0c - 1) * 9) + 2ee) + 4) = 52) is false then
        jump-to: LABEL 10
LABEL 0d
    temp0d <- read-word(2271)
    temp0e <- read-word((((temp0d - 1) * 9) + 2ee) + 7)
    print: read-text(temp0e + 1, read-byte(temp0e))
    temp0f <- read-word(234f)
    temp10 <- read-byte((((temp0f - 1) * 9) + 2ee) + 4)
    if (((read-byte((((temp10 - 1) * 9) + 2ee) + 3) & 0010) <> 0) = 1) is false then
        jump-to: LABEL 0f
LABEL 0e
    print: ", in the "
    temp11 <- read-word((((temp10 - 1) * 9) + 2ee) + 7)
    print: read-text(temp11 + 1, read-byte(temp11))
LABEL 0f
    print: "\n"
LABEL 10
    if (temp00 = 0) is false then
        jump-to: LABEL 12
LABEL 11
    temp12 <- read-word(22fd)
    if (temp12 = 0) is false then
        return: 1
LABEL 12
    temp13 <- read-word(234f)
    temp10 <- read-byte((((temp13 - 1) * 9) + 2ee) + 4)
    if (temp03 = 0) is true then
        jump-to: LABEL 21
LABEL 13
    temp14 <- read-word(2271)
    temp15 <- read-word((((temp14 - 1) * 9) + 2ee) + 7)
    temp16 <- uint16((temp15 + 1) + (read-byte(temp15) * 2))
    temp17 <- 0
LABEL 14
    temp18 <- read-byte(temp16)
    if ((temp18 & 1f) <= 11) is false then
        jump-to: LABEL 16
LABEL 15
    temp17 <- 1
    jump-to: LABEL 17
LABEL 16
    temp19 <- read-byte(temp16)
    temp16 <- uint16((temp16 + 1) + ((temp19 >> 5) + 1))
LABEL 17
    if (temp17 = 0) is true then
        jump-to: LABEL 14
    if ((temp18 & 1f) = 11) is false then
        jump-to: LABEL 1c
LABEL 18
    temp16 <- (temp16 + 1)
    if ((temp18 & e0) = 0) is false then
        jump-to: LABEL 1a
LABEL 19
    temp1a <- read-byte(temp16)
    jump-to: LABEL 1b
LABEL 1a
    temp1a <- read-word(temp16)
LABEL 1b
    jump-to: LABEL 1d
LABEL 1c
    temp1a <- uint16(read-word(2d0))
LABEL 1d
    if (temp1a = 0) is false then
        jump-to: LABEL 1f
LABEL 1e
    push-SP: 0
    jump-to: LABEL 20
LABEL 1f
    push-SP: call (temp1a * 2) (03)
LABEL 20
    temp1b <- pop-SP
    if (temp1b = 0) is false then
        return: 1
LABEL 21
    if (temp03 = 0) is true then
        jump-to: LABEL 2e
LABEL 22
    temp1c <- read-word(2271)
    temp1d <- read-word((((temp1c - 1) * 9) + 2ee) + 7)
    temp1e <- uint16((temp1d + 1) + (read-byte(temp1d) * 2))
    temp1f <- 0
LABEL 23
    temp20 <- read-byte(temp1e)
    if ((temp20 & 1f) <= 0b) is false then
        jump-to: LABEL 25
LABEL 24
    temp1f <- 1
    jump-to: LABEL 26
LABEL 25
    temp21 <- read-byte(temp1e)
    temp1e <- uint16((temp1e + 1) + ((temp21 >> 5) + 1))
LABEL 26
    if (temp1f = 0) is true then
        jump-to: LABEL 23
    if ((temp20 & 1f) = 0b) is false then
        jump-to: LABEL 2b
LABEL 27
    temp1e <- (temp1e + 1)
    if ((temp20 & e0) = 0) is false then
        jump-to: LABEL 29
LABEL 28
    temp22 <- read-byte(temp1e)
    jump-to: LABEL 2a
LABEL 29
    temp22 <- read-word(temp1e)
LABEL 2a
    jump-to: LABEL 2c
LABEL 2b
    temp22 <- uint16(read-word(2c4))
LABEL 2c
    if (temp22 = 0) is true then
        jump-to: LABEL 2e
LABEL 2d
    print: read-text(temp22 * 2)
    print: "\n"
    jump-to: LABEL 3b
LABEL 2e
    temp23 <- read-word(2271)
    temp24 <- read-word((((temp23 - 1) * 9) + 2ee) + 7)
    temp25 <- uint16((temp24 + 1) + (read-byte(temp24) * 2))
    temp26 <- 0
LABEL 2f
    temp27 <- read-byte(temp25)
    if ((temp27 & 1f) <= 11) is false then
        jump-to: LABEL 31
LABEL 30
    temp26 <- 1
    jump-to: LABEL 32
LABEL 31
    temp28 <- read-byte(temp25)
    temp25 <- uint16((temp25 + 1) + ((temp28 >> 5) + 1))
LABEL 32
    if (temp26 = 0) is true then
        jump-to: LABEL 2f
    if ((temp27 & 1f) = 11) is false then
        jump-to: LABEL 37
LABEL 33
    temp25 <- (temp25 + 1)
    if ((temp27 & e0) = 0) is false then
        jump-to: LABEL 35
LABEL 34
    temp29 <- read-byte(temp25)
    jump-to: LABEL 36
LABEL 35
    temp29 <- read-word(temp25)
LABEL 36
    jump-to: LABEL 38
LABEL 37
    temp29 <- uint16(read-word(2d0))
LABEL 38
    if (temp29 = 0) is false then
        jump-to: LABEL 3a
LABEL 39
    push-SP: 0
    jump-to: LABEL 3b
LABEL 3a
    push-SP: call (temp29 * 2) (04)
LABEL 3b
    temp2a <- read-word(2271)
    if (temp2a = temp10) is true then
        return: 1
LABEL 3c
    if (((read-byte((((temp10 - 1) * 9) + 2ee) + 3) & 0010) <> 0) = 1) is false then
        return: 1
LABEL 3d
    temp2b <- read-word((((temp10 - 1) * 9) + 2ee) + 7)
    temp2c <- uint16((temp2b + 1) + (read-byte(temp2b) * 2))
    temp2d <- 0
LABEL 3e
    temp2e <- read-byte(temp2c)
    if ((temp2e & 1f) <= 11) is false then
        jump-to: LABEL 40
LABEL 3f
    temp2d <- 1
    jump-to: LABEL 41
LABEL 40
    temp2f <- read-byte(temp2c)
    temp2c <- uint16((temp2c + 1) + ((temp2f >> 5) + 1))
LABEL 41
    if (temp2d = 0) is true then
        jump-to: LABEL 3e
    if ((temp2e & 1f) = 11) is false then
        jump-to: LABEL 46
LABEL 42
    temp2c <- (temp2c + 1)
    if ((temp2e & e0) = 0) is false then
        jump-to: LABEL 44
LABEL 43
    temp30 <- read-byte(temp2c)
    jump-to: LABEL 45
LABEL 44
    temp30 <- read-word(temp2c)
LABEL 45
    jump-to: LABEL 47
LABEL 46
    temp30 <- uint16(read-word(2d0))
LABEL 47
    if (temp30 = 0) is false then
        jump-to: LABEL 49
LABEL 48
    push-SP: 0
    jump-to: LABEL 4a
LABEL 49
    push-SP: call (temp30 * 2) (03)
LABEL 4a
    return: 1
]]>

        TestBinder(Zork1, &H8C9A, expected)
    End Sub

#End Region
#Region "Zork1_101E0"

#Region "ZCode"
    '101eb:  93 72 01                GET_PARENT      "thief" -> L00
    '101ee:  0a 72 07 c8             TEST_ATTR       "thief",#07 [TRUE] 101f8
    '101f2:  e8 7f 01                PUSH            #01
    '101f5:  8c 00 05                JUMP            101fb
    '101f8:  e8 7f 00                PUSH            #00
    '101fb:  2d 03 00                STORE           L02,(SP)+
    '101fe:  a0 03 c5                JZ              L02 [TRUE] 10204
    '10201:  93 72 01                GET_PARENT      "thief" -> L00
    '10204:  41 01 be 5a             JE              L00,#be [FALSE] 10220
    '10208:  61 01 10 d6             JE              L00,G00 [TRUE] 10220
    '1020c:  a0 03 ca                JZ              L02 [TRUE] 10217
    '1020f:  e0 3f 69 69 00          CALL            d2d2 -> -(SP)
    '10214:  0d 03 00                STORE           L02,#00
    '10217:  e0 1f 69 77 be 00       CALL            d2ee (#be) -> -(SP)
    '1021d:  8c 00 56                JUMP            10274
    '10220:  61 01 10 5e             JE              L00,G00 [FALSE] 10240
    '10224:  4a 01 14 da             TEST_ATTR       L00,#14 [TRUE] 10240
    '10228:  26 d9 10 d6             JIN             "troll",G00 [TRUE] 10240
    '1022c:  e0 2f 67 47 03 00       CALL            ce8e (L02) -> -(SP)
    '10232:  a0 00 41                JZ              (SP)+ [FALSE] RTRUE
    '10235:  0a 72 07 00 3c          TEST_ATTR       "thief",#07 [FALSE] 10274
    '1023a:  0d 03 00                STORE           L02,#00
    '1023d:  8c 00 36                JUMP            10274
    '10240:  26 72 01 4c             JIN             "thief",L00 [FALSE] 1024e
    '10244:  0a 72 07 c8             TEST_ATTR       "thief",#07 [TRUE] 1024e
    '10248:  0b 72 07                SET_ATTR        "thief",#07
    '1024b:  0d 03 00                STORE           L02,#00
    '1024e:  4a 01 03 64             TEST_ATTR       L00,#03 [FALSE] 10274
    '10252:  e0 25 81 dc 01 72 4b 00 CALL            103b8 (L00,#72,#4b) -> -(SP)
    '1025a:  4a 01 05 4f             TEST_ATTR       L00,#05 [FALSE] 1026b
    '1025e:  4a 10 05 4b             TEST_ATTR       G00,#05 [FALSE] 1026b
    '10262:  e0 2f 69 96 01 00       CALL            d32c (L00) -> -(SP)
    '10268:  8c 00 08                JUMP            10271
    '1026b:  e0 2f 81 a6 01 00       CALL            1034c (L00) -> -(SP)
    '10271:  2d 05 00                STORE           L04,(SP)+
    '10274:  a0 04 48                JZ              L03 [FALSE] 1027d
    '10277:  e8 7f 01                PUSH            #01
    '1027a:  8c 00 05                JUMP            10280
    '1027d:  e8 7f 00                PUSH            #00
    '10280:  2d 04 00                STORE           L03,(SP)+
    '10283:  a0 04 f1                JZ              L03 [TRUE] 102b5
    '10286:  a0 03 6e                JZ              L02 [FALSE] 102b5
    '10289:  e0 3f 81 9e 00          CALL            1033c -> -(SP)
    '1028e:  a0 01 c9                JZ              L00 [TRUE] 10298
    '10291:  a1 01 01 45             GET_SIBLING     L00 -> L00 [FALSE] 10298
    '10295:  8c 00 06                JUMP            1029c
    '10298:  92 52 01 c2             GET_CHILD       #52 -> L00 [TRUE] 1029c
    '1029c:  4a 01 09 bf ef          TEST_ATTR       L00,#09 [TRUE] 1028e
    '102a1:  4a 01 06 3f ea          TEST_ATTR       L00,#06 [FALSE] 1028e
    '102a6:  2e 72 01                INSERT_OBJ      "thief",L00
    '102a9:  0c 72 02                CLEAR_ATTR      "thief",#02
    '102ac:  0b 72 07                SET_ATTR        "thief",#07
    '102af:  0d 2f 00                STORE           G1f,#00
    '102b2:  8c ff 3b                JUMP            101ee
    '102b5:  41 01 be ca             JE              L00,#be [TRUE] 102c1
    '102b9:  e0 2f 81 62 01 00       CALL            102c4 (L00) -> -(SP)
    '102bf:  ab 05                   RET             L04
    '102c1:  ab 05                   RET             L04
#End Region

    <Fact>
    Sub Zork1_101E0()
        Dim expected =
<![CDATA[
# temps: 18

LABEL 00
    temp00 <- L03
    temp01 <- L04
LABEL 01
    temp02 <- read-byte(6eb)
LABEL 02
    if (((read-byte(6e7) & 0001) <> 0) = 1) is true then
        jump-to: LABEL 04
LABEL 03
    push-SP: 01
    jump-to: LABEL 05
LABEL 04
    push-SP: 00
LABEL 05
    temp03 <- pop-SP
    temp04 <- temp03
    if (temp03 = 0) is true then
        jump-to: LABEL 07
LABEL 06
    temp02 <- read-byte(6eb)
LABEL 07
    if (temp02 = be) is false then
        jump-to: LABEL 0c
LABEL 08
    temp05 <- read-word(2271)
    if (temp02 = temp05) is true then
        jump-to: LABEL 0c
LABEL 09
    if (temp03 = 0) is true then
        jump-to: LABEL 0b
LABEL 0a
    push-SP: call d2d2 ()
    temp04 <- 00
LABEL 0b
    push-SP: call d2ee (be)
    jump-to: LABEL 1b
LABEL 0c
    temp06 <- read-word(2271)
    if (temp02 = temp06) is false then
        jump-to: LABEL 12
LABEL 0d
    if (((read-byte((((temp02 - 1) * 9) + 2ee) + 2) & 0008) <> 0) = 1) is true then
        jump-to: LABEL 12
LABEL 0e
    temp07 <- read-word(2271)
    if (read-byte(a8a) = temp07) is true then
        jump-to: LABEL 12
LABEL 0f
    temp08 <- call ce8e (temp03)
    if (temp08 = 0) is false then
        return: 1
LABEL 10
    if (((read-byte(6e7) & 0001) <> 0) = 1) is false then
        jump-to: LABEL 1b
LABEL 11
    temp04 <- 00
    jump-to: LABEL 1b
LABEL 12
    if (read-byte(6eb) = temp02) is false then
        jump-to: LABEL 15
LABEL 13
    if (((read-byte(6e7) & 0001) <> 0) = 1) is true then
        jump-to: LABEL 15
LABEL 14
    write-byte(6e7) <- byte(read-byte(6e7) | 0001)
    temp04 <- 00
LABEL 15
    if (((read-byte(((temp02 - 1) * 9) + 2ee) & 0010) <> 0) = 1) is false then
        jump-to: LABEL 1b
LABEL 16
    push-SP: call 103b8 (temp02, 72, 4b)
    if (((read-byte(((temp02 - 1) * 9) + 2ee) & 0004) <> 0) = 1) is false then
        jump-to: LABEL 19
LABEL 17
    temp09 <- read-word(2271)
    if (((read-byte(((temp09 - 1) * 9) + 2ee) & 0004) <> 0) = 1) is false then
        jump-to: LABEL 19
LABEL 18
    push-SP: call d32c (temp02)
    jump-to: LABEL 1a
LABEL 19
    push-SP: call 1034c (temp02)
LABEL 1a
    temp0a <- pop-SP
    temp01 <- temp0a
LABEL 1b
    if (temp00 = 0) is false then
        jump-to: LABEL 1d
LABEL 1c
    push-SP: 01
    jump-to: LABEL 1e
LABEL 1d
    push-SP: 00
LABEL 1e
    temp0b <- pop-SP
    temp00 <- temp0b
    if (temp0b = 0) is true then
        jump-to: LABEL 37
LABEL 1f
    if (temp04 = 0) is false then
        jump-to: LABEL 37
LABEL 20
    push-SP: call 1033c ()
LABEL 21
    if (temp02 = 0) is true then
        jump-to: LABEL 24
LABEL 22
    temp0c <- temp02
    temp02 <- read-byte((((temp0c - 1) * 9) + 2ee) + 5)
    if (read-byte((((temp0c - 1) * 9) + 2ee) + 5) <> 0) is false then
        jump-to: LABEL 24
LABEL 23
    jump-to: LABEL 25
LABEL 24
    temp02 <- read-byte(5cd)
    if (read-byte(5cd) <> 0) is true then
        jump-to: LABEL 25
LABEL 25
    if (((read-byte((((temp02 - 1) * 9) + 2ee) + 1) & 0040) <> 0) = 1) is true then
        jump-to: LABEL 21
LABEL 26
    if (((read-byte(((temp02 - 1) * 9) + 2ee) & 0002) <> 0) = 1) is false then
        jump-to: LABEL 21
LABEL 27
    temp0d <- temp02
    temp0e <- 0
    if (read-byte(6eb) = 0) is false then
        jump-to: LABEL 29
LABEL 28
    temp0f <- 0
    jump-to: LABEL 2a
LABEL 29
    temp0f <- read-byte((((read-byte(6eb) - 1) * 9) + 2ee) + 6)
LABEL 2a
    if (temp0f <> 72) is false then
        jump-to: LABEL 30
LABEL 2b
    temp10 <- temp0f
LABEL 2c
    temp11 <- read-byte((((temp10 - 1) * 9) + 2ee) + 5)
    if (temp11 = 72) is false then
        jump-to: LABEL 2e
LABEL 2d
    temp0e <- temp10
    temp10 <- 0
    jump-to: LABEL 2f
LABEL 2e
    temp10 <- temp11
LABEL 2f
    if (temp10 <> 0) is true then
        jump-to: LABEL 2c
LABEL 30
    if (temp0e <> 0) is false then
        jump-to: LABEL 32
LABEL 31
    write-byte((((temp0e - 1) * 9) + 2ee) + 5) <- read-byte(6ec)
LABEL 32
    if (temp0f = 72) is false then
        jump-to: LABEL 34
LABEL 33
    write-byte((((read-byte(6eb) - 1) * 9) + 2ee) + 6) <- read-byte(6ec)
LABEL 34
    write-byte(6eb) <- 0
    write-byte(6ec) <- 0
    if (temp02 <> 0) is false then
        jump-to: LABEL 36
LABEL 35
    write-byte(6eb) <- temp0d
    write-byte(6ec) <- read-byte((((temp0d - 1) * 9) + 2ee) + 6)
    write-byte((((temp0d - 1) * 9) + 2ee) + 6) <- 72
LABEL 36
    write-byte(6e7) <- byte(read-byte(6e7) & not 0020)
    write-byte(6e7) <- byte(read-byte(6e7) | 0001)
    write-word(22af) <- 00
    jump-to: LABEL 02
LABEL 37
    if (temp02 = be) is true then
        jump-to: LABEL 39
LABEL 38
    push-SP: call 102c4 (temp02)
    return: temp01
LABEL 39
    return: temp01
]]>

        TestBinder(Zork1, &H101E0, expected)
    End Sub

#End Region

End Module
