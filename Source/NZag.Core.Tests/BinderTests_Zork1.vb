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
    print: "a "
    temp00 <- L00
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
# temps: 5

LABEL 00
    temp00 <- read-word(22e9)
    if (temp00 = 0) is true then
        jump-to: LABEL 06
LABEL 01
    if (int16(64) > 0) is false then
        jump-to: LABEL 03
LABEL 02
    push-SP: random(int16(64))
    jump-to: LABEL 04
LABEL 03
    randomize(int16(64))
    push-SP: 0
LABEL 04
    temp01 <- L00
    temp02 <- pop-SP
    if (int16(temp01) > int16(temp02)) is true then
        return: 1
LABEL 05
    return: 0
LABEL 06
    if (int16(012c) > 0) is false then
        jump-to: LABEL 08
LABEL 07
    push-SP: random(int16(012c))
    jump-to: LABEL 09
LABEL 08
    randomize(int16(012c))
    push-SP: 0
LABEL 09
    temp03 <- L00
    temp04 <- pop-SP
    if (int16(temp03) > int16(temp04)) is true then
        return: 1
LABEL 0a
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
# temps: 4

LABEL 00
    temp00 <- L00
    temp01 <- read-word(temp00)
    if (int16(temp01) > 0) is false then
        jump-to: LABEL 02
LABEL 01
    push-SP: random(int16(temp01))
    jump-to: LABEL 03
LABEL 02
    randomize(int16(temp01))
    push-SP: 0
LABEL 03
    temp02 <- L00
    temp03 <- pop-SP
    return: read-word(temp02 + (temp03 * 2))
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
# temps: 24

LABEL 00
    temp00 <- L00
    L01 <- read-word(temp00)
    temp01 <- L00
    L02 <- read-word(temp01 + 2)
    temp02 <- L01
    L01 <- (int16(temp02) - int16(1))
    temp03 <- L00
    L00 <- (int16(temp03) + int16(02))
    temp04 <- L02
    push-SP: (int16(temp04) * int16(02))
    temp05 <- L00
    temp06 <- pop-SP
    L05 <- (int16(temp05) + int16(temp06))
    temp07 <- L01
    temp08 <- L02
    temp09 <- (int16(temp07) - int16(temp08))
    if (int16(temp09) > 0) is false then
        jump-to: LABEL 02
LABEL 01
    L03 <- random(int16(temp09))
    jump-to: LABEL 03
LABEL 02
    randomize(int16(temp09))
    L03 <- 0
LABEL 03
    temp0a <- L05
    temp0b <- L03
    L04 <- read-word(temp0a + (temp0b * 2))
    temp0c <- L05
    push-SP: read-word(temp0c + 2)
    temp0d <- L05
    temp0e <- L03
    temp0f <- pop-SP
    write-word(temp0d + (temp0e * 2)) <- temp0f
    temp10 <- L05
    temp11 <- L04
    write-word(temp10 + 2) <- temp11
    temp12 <- L02
    L02 <- (int16(temp12) + int16(1))
    temp13 <- L02
    temp14 <- L01
    if (temp13 = temp14) is false then
        jump-to: LABEL 05
LABEL 04
    L02 <- 00
LABEL 05
    temp15 <- L00
    temp16 <- L02
    write-word(temp15) <- temp16
    temp17 <- L04
    return: temp17
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
    if (temp00 = 01) is false then
        return: 0
LABEL 01
    temp01 <- read-word(2361)
    if (temp01 = 45) is false then
        return: 0
LABEL 02
    temp02 <- read-word(235d)
    if (temp02 = 0) is false then
        return: 0
LABEL 03
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
# temps: 104

LABEL 00
    L03 <- 00
    L04 <- 00
    L07 <- 01
    write-word(236f) <- call 5880 ()
    temp00 <- read-word(236f)
    if (temp00 = 0) is true then
        jump-to: LABEL 58
LABEL 01
    temp01 <- read-word(231b)
    temp02 <- read-word(2313)
    L00 <- read-word(temp01 + (temp02 * 2))
    temp03 <- read-word(231d)
    temp04 <- read-word(2313)
    L01 <- read-word(temp03 + (temp04 * 2))
    temp05 <- L01
    if (temp05 = 0) is false then
        jump-to: LABEL 03
LABEL 02
    temp06 <- L01
    push-SP: temp06
    jump-to: LABEL 0b
LABEL 03
    temp07 <- L01
    if (int16(temp07) > int16(01)) is false then
        jump-to: LABEL 08
LABEL 04
    temp08 <- read-word(231d)
    L05 <- temp08
    temp09 <- L00
    if (temp09 = 0) is false then
        jump-to: LABEL 06
LABEL 05
    L04 <- 00
    jump-to: LABEL 07
LABEL 06
    temp0a <- read-word(231b)
    L04 <- read-word(temp0a + 2)
LABEL 07
    temp0b <- L01
    push-SP: temp0b
    jump-to: LABEL 0b
LABEL 08
    temp0c <- L00
    if (int16(temp0c) > int16(01)) is false then
        jump-to: LABEL 0a
LABEL 09
    L07 <- 00
    temp0d <- read-word(231b)
    L05 <- temp0d
    temp0e <- read-word(231d)
    L04 <- read-word(temp0e + 2)
    temp0f <- L00
    push-SP: temp0f
    jump-to: LABEL 0b
LABEL 0a
    push-SP: 01
LABEL 0b
    temp10 <- pop-SP
    L02 <- temp10
    temp11 <- L04
    if (temp11 = 0) is false then
        jump-to: LABEL 0e
LABEL 0c
    temp12 <- L00
    if (temp12 = 01) is false then
        jump-to: LABEL 0e
LABEL 0d
    temp13 <- read-word(231b)
    L04 <- read-word(temp13 + 2)
LABEL 0e
    temp14 <- read-word(2361)
    if (temp14 = 89) is false then
        jump-to: LABEL 10
LABEL 0f
    temp15 <- read-word(2361)
    temp16 <- read-word(235d)
    L06 <- call 577c (temp15, temp16)
    jump-to: LABEL 44
LABEL 10
    temp17 <- L02
    if (temp17 = 0) is false then
        jump-to: LABEL 16
LABEL 11
    temp18 <- read-word(2357)
    temp19 <- read-byte(temp18)
    temp1a <- (temp19 & 03)
    if (temp1a = 0) is false then
        jump-to: LABEL 13
LABEL 12
    temp1b <- read-word(2361)
    L06 <- call 577c (temp1b)
    write-word(235d) <- 00
    jump-to: LABEL 44
LABEL 13
    temp1c <- read-word(22f5)
    if (temp1c = 0) is false then
        jump-to: LABEL 15
LABEL 14
    print: "It's too dark to see."
    print: "\n"
    jump-to: LABEL 44
LABEL 15
    print: "It's not clear what you're referring to."
    print: "\n"
    L06 <- 00
    jump-to: LABEL 44
LABEL 16
    write-word(2365) <- 00
    write-word(2367) <- 00
    temp1d <- L02
    if (int16(temp1d) > int16(01)) is false then
        jump-to: LABEL 18
LABEL 17
    write-word(2367) <- 01
LABEL 18
    L09 <- 00
LABEL 19
    temp1e <- L02
    temp1f <- L03
    temp20 <- (int16(temp1f) + int16(1))
    L03 <- temp20
    if (int16(temp20) > int16(temp1e)) is false then
        jump-to: LABEL 25
LABEL 1a
    temp21 <- read-word(2365)
    if (int16(temp21) > int16(00)) is false then
        jump-to: LABEL 23
LABEL 1b
    print: "The "
    temp22 <- read-word(2365)
    temp23 <- L02
    if (temp22 = temp23) is true then
        jump-to: LABEL 1d
LABEL 1c
    print: "other "
LABEL 1d
    print: "object"
    temp24 <- read-word(2365)
    if (temp24 = 01) is true then
        jump-to: LABEL 1f
LABEL 1e
    print: "s"
LABEL 1f
    print: " that you mentioned "
    temp25 <- read-word(2365)
    if (temp25 = 01) is true then
        jump-to: LABEL 21
LABEL 20
    print: "are"
    jump-to: LABEL 22
LABEL 21
    print: "is"
LABEL 22
    print: "n't here."
    print: "\n"
    jump-to: LABEL 44
LABEL 23
    temp26 <- L09
    if (temp26 = 0) is false then
        jump-to: LABEL 44
LABEL 24
    print: "There's nothing here you can take."
    print: "\n"
    jump-to: LABEL 44
LABEL 25
    temp27 <- L07
    if (temp27 = 0) is true then
        jump-to: LABEL 27
LABEL 26
    temp28 <- read-word(231d)
    temp29 <- L03
    L08 <- read-word(temp28 + (temp29 * 2))
    jump-to: LABEL 28
LABEL 27
    temp2a <- read-word(231b)
    temp2b <- L03
    L08 <- read-word(temp2a + (temp2b * 2))
LABEL 28
    temp2c <- L07
    if (temp2c = 0) is true then
        jump-to: LABEL 2a
LABEL 29
    temp2d <- L08
    push-SP: temp2d
    jump-to: LABEL 2b
LABEL 2a
    temp2e <- L04
    push-SP: temp2e
LABEL 2b
    temp2f <- pop-SP
    write-word(235d) <- temp2f
    temp30 <- L07
    if (temp30 = 0) is true then
        jump-to: LABEL 2d
LABEL 2c
    temp31 <- L04
    push-SP: temp31
    jump-to: LABEL 2e
LABEL 2d
    temp32 <- L08
    push-SP: temp32
LABEL 2e
    temp33 <- pop-SP
    write-word(235f) <- temp33
    temp34 <- L02
    if (int16(temp34) > int16(01)) is true then
        jump-to: LABEL 30
LABEL 2f
    temp35 <- read-word(2339)
    temp36 <- read-word(temp35 + c)
    temp37 <- read-word(temp36)
    if (temp37 = 3b7c) is false then
        jump-to: LABEL 42
LABEL 30
    temp38 <- L08
    if (temp38 = 0b) is false then
        jump-to: LABEL 32
LABEL 31
    temp39 <- read-word(2365)
    write-word(2365) <- (int16(temp39) + int16(1))
    jump-to: LABEL 19
LABEL 32
    temp3a <- read-word(2361)
    if (temp3a = 5d) is false then
        jump-to: LABEL 37
LABEL 33
    temp3b <- read-word(235f)
    if (temp3b = 0) is true then
        jump-to: LABEL 37
LABEL 34
    temp3c <- read-word(2339)
    temp3d <- read-word(temp3c + c)
    temp3e <- read-word(temp3d)
    if (temp3e = 3b7c) is false then
        jump-to: LABEL 37
LABEL 35
    temp3f <- read-word(235d)
    temp40 <- read-word(235f)
    if (read-byte((((temp3f - 1) * 9) + 2ee) + 4) = temp40) is true then
        jump-to: LABEL 37
LABEL 36
    jump-to: LABEL 19
LABEL 37
    temp41 <- read-word(2311)
    if (temp41 = 01) is false then
        jump-to: LABEL 3e
LABEL 38
    temp42 <- read-word(2361)
    if (temp42 = 5d) is false then
        jump-to: LABEL 3e
LABEL 39
    temp43 <- L08
    temp44 <- read-byte((((temp43 - 1) * 9) + 2ee) + 4)
    temp45 <- read-word(234f)
    temp46 <- read-word(2271)
    if ((temp44 = temp45) | (temp44 = temp46)) is true then
        jump-to: LABEL 3b
LABEL 3a
    temp47 <- L08
    temp48 <- read-byte((((temp47 - 1) * 9) + 2ee) + 4)
    if (((read-byte((((temp48 - 1) * 9) + 2ee) + 1) & 0020) <> 0) = 1) is false then
        jump-to: LABEL 19
LABEL 3b
    temp49 <- L08
    if (((read-byte((((temp49 - 1) * 9) + 2ee) + 2) & 0040) <> 0) = 1) is true then
        jump-to: LABEL 3e
LABEL 3c
    temp4a <- L08
    if (((read-byte((((temp4a - 1) * 9) + 2ee) + 1) & 0004) <> 0) = 1) is true then
        jump-to: LABEL 3e
LABEL 3d
    jump-to: LABEL 19
LABEL 3e
    temp4b <- L08
    if (temp4b = 0c) is false then
        jump-to: LABEL 40
LABEL 3f
    temp4c <- read-word(2347)
    temp4d <- read-word((((temp4c - 1) * 9) + 2ee) + 7)
    print: read-text(temp4d + 1, read-byte(temp4d))
    jump-to: LABEL 41
LABEL 40
    temp4e <- L08
    temp4f <- read-word((((temp4e - 1) * 9) + 2ee) + 7)
    print: read-text(temp4f + 1, read-byte(temp4f))
LABEL 41
    print: ": "
LABEL 42
    L09 <- 01
    temp50 <- read-word(2361)
    temp51 <- read-word(235d)
    temp52 <- read-word(235f)
    L06 <- call 577c (temp50, temp51, temp52)
    temp53 <- L06
    if (temp53 = 02) is false then
        jump-to: LABEL 19
LABEL 43
    jump-to: LABEL 44
LABEL 44
    temp54 <- L06
    if (temp54 = 02) is true then
        jump-to: LABEL 52
LABEL 45
    temp55 <- read-word(234f)
    temp56 <- read-byte((((temp55 - 1) * 9) + 2ee) + 4)
    temp57 <- read-word((((temp56 - 1) * 9) + 2ee) + 7)
    temp58 <- uint16((temp57 + 1) + (read-byte(temp57) * 2))
    temp59 <- 0
LABEL 46
    temp5a <- read-byte(temp58)
    if ((temp5a & 1f) <= 11) is false then
        jump-to: LABEL 48
LABEL 47
    temp59 <- 1
    jump-to: LABEL 49
LABEL 48
    temp5b <- read-byte(temp58)
    temp58 <- uint16((temp58 + 1) + ((temp5b >> 5) + 1))
LABEL 49
    if (temp59 = 0) is true then
        jump-to: LABEL 46
    if ((temp5a & 1f) = 11) is false then
        jump-to: LABEL 4e
LABEL 4a
    temp58 <- (temp58 + 1)
    if ((temp5a & e0) = 0) is false then
        jump-to: LABEL 4c
LABEL 4b
    temp5c <- read-byte(temp58)
    jump-to: LABEL 4d
LABEL 4c
    temp5c <- read-word(temp58)
LABEL 4d
    jump-to: LABEL 4f
LABEL 4e
    temp5c <- uint16(read-word(2d0))
LABEL 4f
    if (temp5c = 0) is false then
        jump-to: LABEL 51
LABEL 50
    L06 <- 0
    jump-to: LABEL 52
LABEL 51
    L06 <- call (temp5c * 2) (06)
LABEL 52
    temp5d <- read-word(2361)
    if (((temp5d = 08) | (temp5d = 89)) | (temp5d = 0f)) is true then
        jump-to: LABEL 56
LABEL 53
    temp5e <- read-word(2361)
    if (((temp5e = 0c) | (temp5e = 09)) | (temp5e = 07)) is false then
        jump-to: LABEL 55
LABEL 54
    jump-to: LABEL 56
LABEL 55
    temp5f <- read-word(2361)
    write-word(236d) <- temp5f
    temp60 <- read-word(235d)
    write-word(236b) <- temp60
    temp61 <- read-word(235f)
    write-word(2369) <- temp61
LABEL 56
    temp62 <- L06
    if (temp62 = 02) is false then
        jump-to: LABEL 59
LABEL 57
    write-word(2349) <- 00
    jump-to: LABEL 59
LABEL 58
    write-word(2349) <- 00
LABEL 59
    temp63 <- read-word(236f)
    if (temp63 = 0) is true then
        jump-to: LABEL 00
LABEL 5a
    temp64 <- read-word(2361)
    if (((temp64 = 02) | (temp64 = 01)) | (temp64 = 6f)) is true then
        jump-to: LABEL 00
LABEL 5b
    temp65 <- read-word(2361)
    if (((temp65 = 0c) | (temp65 = 08)) | (temp65 = 00)) is true then
        jump-to: LABEL 00
LABEL 5c
    temp66 <- read-word(2361)
    if (((temp66 = 09) | (temp66 = 06)) | (temp66 = 05)) is true then
        jump-to: LABEL 00
LABEL 5d
    temp67 <- read-word(2361)
    if (((temp67 = 07) | (temp67 = 0b)) | (temp67 = 0a)) is false then
        jump-to: LABEL 5f
LABEL 5e
    jump-to: LABEL 00
LABEL 5f
    L06 <- call 54c4 ()
    jump-to: LABEL 00
]]>

        TestBinder(Zork1, &H552A, expected)
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
# temps: 26

LABEL 00
    temp00 <- L00
    L00 <- read-byte((((temp00 - 1) * 9) + 2ee) + 6)
    if (read-byte((((temp00 - 1) * 9) + 2ee) + 6) <> 0) is false then
        return: 0
LABEL 01
    temp01 <- L02
    if (temp01 = 02) is true then
        jump-to: LABEL 0c
LABEL 02
    temp02 <- L00
    temp03 <- read-word((((temp02 - 1) * 9) + 2ee) + 7)
    temp04 <- uint16((temp03 + 1) + (read-byte(temp03) * 2))
    temp05 <- 0
LABEL 03
    temp06 <- read-byte(temp04)
    if ((temp06 & 1f) <= 12) is false then
        jump-to: LABEL 05
LABEL 04
    temp05 <- 1
    jump-to: LABEL 06
LABEL 05
    temp07 <- read-byte(temp04)
    temp04 <- uint16((temp04 + 1) + ((temp07 >> 5) + 1))
LABEL 06
    if (temp05 = 0) is true then
        jump-to: LABEL 03
    if ((temp06 & 1f) = 12) is false then
        jump-to: LABEL 08
LABEL 07
    push-SP: (temp04 + 1)
    jump-to: LABEL 09
LABEL 08
    push-SP: 0
LABEL 09
    temp08 <- pop-SP
    if (temp08 = 0) is true then
        jump-to: LABEL 0c
LABEL 0a
    temp09 <- L00
    temp0a <- L01
    temp0b <- call 6d1c (temp09, temp0a)
    if (temp0b = 0) is true then
        jump-to: LABEL 0c
LABEL 0b
    temp0c <- L00
    temp0d <- L01
    push-SP: call 6aba (temp0c, temp0d)
LABEL 0c
    temp0e <- L02
    if (temp0e = 00) is false then
        jump-to: LABEL 0f
LABEL 0d
    temp0f <- L00
    if (((read-byte((((temp0f - 1) * 9) + 2ee) + 1) & 0080) <> 0) = 1) is true then
        jump-to: LABEL 0f
LABEL 0e
    temp10 <- L00
    if (((read-byte((((temp10 - 1) * 9) + 2ee) + 1) & 0020) <> 0) = 1) is false then
        jump-to: LABEL 18
LABEL 0f
    temp11 <- L00
    L04 <- read-byte((((temp11 - 1) * 9) + 2ee) + 6)
    if (read-byte((((temp11 - 1) * 9) + 2ee) + 6) <> 0) is false then
        jump-to: LABEL 18
LABEL 10
    temp12 <- L00
    if (((read-byte((((temp12 - 1) * 9) + 2ee) + 1) & 0010) <> 0) = 1) is true then
        jump-to: LABEL 12
LABEL 11
    temp13 <- L00
    if (((read-byte((((temp13 - 1) * 9) + 2ee) + 1) & 0008) <> 0) = 1) is false then
        jump-to: LABEL 18
LABEL 12
    temp14 <- L00
    if (((read-byte((((temp14 - 1) * 9) + 2ee) + 1) & 0020) <> 0) = 1) is false then
        jump-to: LABEL 14
LABEL 13
    push-SP: 01
    jump-to: LABEL 17
LABEL 14
    temp15 <- L00
    if (((read-byte((((temp15 - 1) * 9) + 2ee) + 1) & 0080) <> 0) = 1) is false then
        jump-to: LABEL 16
LABEL 15
    push-SP: 01
    jump-to: LABEL 17
LABEL 16
    push-SP: 00
LABEL 17
    temp16 <- L00
    temp17 <- L01
    temp18 <- pop-SP
    L03 <- call 6a52 (temp16, temp17, temp18)
LABEL 18
    temp19 <- L00
    L00 <- read-byte((((temp19 - 1) * 9) + 2ee) + 5)
    if (read-byte((((temp19 - 1) * 9) + 2ee) + 5) <> 0) is true then
        jump-to: LABEL 01
LABEL 19
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
# temps: 58

LABEL 00
    temp00 <- L00
    if (temp00 = 0) is true then
        jump-to: LABEL 02
LABEL 01
    temp01 <- L00
    push-SP: temp01
    jump-to: LABEL 03
LABEL 02
    temp02 <- read-word(22ff)
    push-SP: temp02
LABEL 03
    temp03 <- pop-SP
    L01 <- temp03
    temp04 <- read-word(22f5)
    if (temp04 = 0) is false then
        jump-to: LABEL 07
LABEL 04
    print: "It is pitch black."
    temp05 <- read-word(22f3)
    if (temp05 = 0) is false then
        jump-to: LABEL 06
LABEL 05
    print: " You are likely to be eaten by a grue."
LABEL 06
    print: "\n"
    push-SP: call 50d0 ()
    return: 00
LABEL 07
    temp06 <- read-word(2271)
    if (((read-byte(((temp06 - 1) * 9) + 2ee) & 0010) <> 0) = 1) is true then
        jump-to: LABEL 09
LABEL 08
    temp07 <- read-word(2271)
    temp08 <- (((temp07 - 1) * 9) + 2ee)
    write-byte(temp08) <- byte(read-byte(temp08) | 0010)
    L01 <- 01
LABEL 09
    temp09 <- read-word(2271)
    if (((read-byte(((temp09 - 1) * 9) + 2ee) & 0004) <> 0) = 1) is false then
        jump-to: LABEL 0b
LABEL 0a
    temp0a <- read-word(2271)
    temp0b <- (((temp0a - 1) * 9) + 2ee)
    write-byte(temp0b) <- byte(read-byte(temp0b) & not 0010)
LABEL 0b
    temp0c <- read-word(2271)
    if (read-byte((((temp0c - 1) * 9) + 2ee) + 4) = 52) is false then
        jump-to: LABEL 0f
LABEL 0c
    temp0d <- read-word(2271)
    temp0e <- read-word((((temp0d - 1) * 9) + 2ee) + 7)
    print: read-text(temp0e + 1, read-byte(temp0e))
    temp0f <- read-word(234f)
    L03 <- read-byte((((temp0f - 1) * 9) + 2ee) + 4)
    temp10 <- L03
    if (((read-byte((((temp10 - 1) * 9) + 2ee) + 3) & 0010) <> 0) = 1) is false then
        jump-to: LABEL 0e
LABEL 0d
    print: ", in the "
    temp11 <- L03
    temp12 <- read-word((((temp11 - 1) * 9) + 2ee) + 7)
    print: read-text(temp12 + 1, read-byte(temp12))
LABEL 0e
    print: "\n"
LABEL 0f
    temp13 <- L00
    if (temp13 = 0) is false then
        jump-to: LABEL 11
LABEL 10
    temp14 <- read-word(22fd)
    if (temp14 = 0) is false then
        return: 1
LABEL 11
    temp15 <- read-word(234f)
    L03 <- read-byte((((temp15 - 1) * 9) + 2ee) + 4)
    temp16 <- L01
    if (temp16 = 0) is true then
        jump-to: LABEL 20
LABEL 12
    temp17 <- read-word(2271)
    temp18 <- read-word((((temp17 - 1) * 9) + 2ee) + 7)
    temp19 <- uint16((temp18 + 1) + (read-byte(temp18) * 2))
    temp1a <- 0
LABEL 13
    temp1b <- read-byte(temp19)
    if ((temp1b & 1f) <= 11) is false then
        jump-to: LABEL 15
LABEL 14
    temp1a <- 1
    jump-to: LABEL 16
LABEL 15
    temp1c <- read-byte(temp19)
    temp19 <- uint16((temp19 + 1) + ((temp1c >> 5) + 1))
LABEL 16
    if (temp1a = 0) is true then
        jump-to: LABEL 13
    if ((temp1b & 1f) = 11) is false then
        jump-to: LABEL 1b
LABEL 17
    temp19 <- (temp19 + 1)
    if ((temp1b & e0) = 0) is false then
        jump-to: LABEL 19
LABEL 18
    temp1d <- read-byte(temp19)
    jump-to: LABEL 1a
LABEL 19
    temp1d <- read-word(temp19)
LABEL 1a
    jump-to: LABEL 1c
LABEL 1b
    temp1d <- uint16(read-word(2d0))
LABEL 1c
    if (temp1d = 0) is false then
        jump-to: LABEL 1e
LABEL 1d
    push-SP: 0
    jump-to: LABEL 1f
LABEL 1e
    push-SP: call (temp1d * 2) (03)
LABEL 1f
    temp1e <- pop-SP
    if (temp1e = 0) is false then
        return: 1
LABEL 20
    temp1f <- L01
    if (temp1f = 0) is true then
        jump-to: LABEL 2d
LABEL 21
    temp20 <- read-word(2271)
    temp21 <- read-word((((temp20 - 1) * 9) + 2ee) + 7)
    temp22 <- uint16((temp21 + 1) + (read-byte(temp21) * 2))
    temp23 <- 0
LABEL 22
    temp24 <- read-byte(temp22)
    if ((temp24 & 1f) <= 0b) is false then
        jump-to: LABEL 24
LABEL 23
    temp23 <- 1
    jump-to: LABEL 25
LABEL 24
    temp25 <- read-byte(temp22)
    temp22 <- uint16((temp22 + 1) + ((temp25 >> 5) + 1))
LABEL 25
    if (temp23 = 0) is true then
        jump-to: LABEL 22
    if ((temp24 & 1f) = 0b) is false then
        jump-to: LABEL 2a
LABEL 26
    temp22 <- (temp22 + 1)
    if ((temp24 & e0) = 0) is false then
        jump-to: LABEL 28
LABEL 27
    temp26 <- read-byte(temp22)
    jump-to: LABEL 29
LABEL 28
    temp26 <- read-word(temp22)
LABEL 29
    jump-to: LABEL 2b
LABEL 2a
    temp26 <- uint16(read-word(2c4))
LABEL 2b
    L02 <- temp26
    temp27 <- L02
    if (temp27 = 0) is true then
        jump-to: LABEL 2d
LABEL 2c
    temp28 <- L02
    print: read-text(temp28 * 2)
    print: "\n"
    jump-to: LABEL 3a
LABEL 2d
    temp29 <- read-word(2271)
    temp2a <- read-word((((temp29 - 1) * 9) + 2ee) + 7)
    temp2b <- uint16((temp2a + 1) + (read-byte(temp2a) * 2))
    temp2c <- 0
LABEL 2e
    temp2d <- read-byte(temp2b)
    if ((temp2d & 1f) <= 11) is false then
        jump-to: LABEL 30
LABEL 2f
    temp2c <- 1
    jump-to: LABEL 31
LABEL 30
    temp2e <- read-byte(temp2b)
    temp2b <- uint16((temp2b + 1) + ((temp2e >> 5) + 1))
LABEL 31
    if (temp2c = 0) is true then
        jump-to: LABEL 2e
    if ((temp2d & 1f) = 11) is false then
        jump-to: LABEL 36
LABEL 32
    temp2b <- (temp2b + 1)
    if ((temp2d & e0) = 0) is false then
        jump-to: LABEL 34
LABEL 33
    temp2f <- read-byte(temp2b)
    jump-to: LABEL 35
LABEL 34
    temp2f <- read-word(temp2b)
LABEL 35
    jump-to: LABEL 37
LABEL 36
    temp2f <- uint16(read-word(2d0))
LABEL 37
    if (temp2f = 0) is false then
        jump-to: LABEL 39
LABEL 38
    push-SP: 0
    jump-to: LABEL 3a
LABEL 39
    push-SP: call (temp2f * 2) (04)
LABEL 3a
    temp30 <- read-word(2271)
    temp31 <- L03
    if (temp30 = temp31) is true then
        return: 1
LABEL 3b
    temp32 <- L03
    if (((read-byte((((temp32 - 1) * 9) + 2ee) + 3) & 0010) <> 0) = 1) is false then
        return: 1
LABEL 3c
    temp33 <- L03
    temp34 <- read-word((((temp33 - 1) * 9) + 2ee) + 7)
    temp35 <- uint16((temp34 + 1) + (read-byte(temp34) * 2))
    temp36 <- 0
LABEL 3d
    temp37 <- read-byte(temp35)
    if ((temp37 & 1f) <= 11) is false then
        jump-to: LABEL 3f
LABEL 3e
    temp36 <- 1
    jump-to: LABEL 40
LABEL 3f
    temp38 <- read-byte(temp35)
    temp35 <- uint16((temp35 + 1) + ((temp38 >> 5) + 1))
LABEL 40
    if (temp36 = 0) is true then
        jump-to: LABEL 3d
    if ((temp37 & 1f) = 11) is false then
        jump-to: LABEL 45
LABEL 41
    temp35 <- (temp35 + 1)
    if ((temp37 & e0) = 0) is false then
        jump-to: LABEL 43
LABEL 42
    temp39 <- read-byte(temp35)
    jump-to: LABEL 44
LABEL 43
    temp39 <- read-word(temp35)
LABEL 44
    jump-to: LABEL 46
LABEL 45
    temp39 <- uint16(read-word(2d0))
LABEL 46
    if (temp39 = 0) is false then
        jump-to: LABEL 48
LABEL 47
    push-SP: 0
    jump-to: LABEL 49
LABEL 48
    push-SP: call (temp39 * 2) (03)
LABEL 49
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
# temps: 37

LABEL 00
    L00 <- read-byte(6eb)
LABEL 01
    if (((read-byte(6e7) & 0001) <> 0) = 1) is true then
        jump-to: LABEL 03
LABEL 02
    push-SP: 01
    jump-to: LABEL 04
LABEL 03
    push-SP: 00
LABEL 04
    temp00 <- pop-SP
    L02 <- temp00
    temp01 <- L02
    if (temp01 = 0) is true then
        jump-to: LABEL 06
LABEL 05
    L00 <- read-byte(6eb)
LABEL 06
    temp02 <- L00
    if (temp02 = be) is false then
        jump-to: LABEL 0b
LABEL 07
    temp03 <- L00
    temp04 <- read-word(2271)
    if (temp03 = temp04) is true then
        jump-to: LABEL 0b
LABEL 08
    temp05 <- L02
    if (temp05 = 0) is true then
        jump-to: LABEL 0a
LABEL 09
    push-SP: call d2d2 ()
    L02 <- 00
LABEL 0a
    push-SP: call d2ee (be)
    jump-to: LABEL 1a
LABEL 0b
    temp06 <- L00
    temp07 <- read-word(2271)
    if (temp06 = temp07) is false then
        jump-to: LABEL 11
LABEL 0c
    temp08 <- L00
    if (((read-byte((((temp08 - 1) * 9) + 2ee) + 2) & 0008) <> 0) = 1) is true then
        jump-to: LABEL 11
LABEL 0d
    temp09 <- read-word(2271)
    if (read-byte(a8a) = temp09) is true then
        jump-to: LABEL 11
LABEL 0e
    temp0a <- L02
    temp0b <- call ce8e (temp0a)
    if (temp0b = 0) is false then
        return: 1
LABEL 0f
    if (((read-byte(6e7) & 0001) <> 0) = 1) is false then
        jump-to: LABEL 1a
LABEL 10
    L02 <- 00
    jump-to: LABEL 1a
LABEL 11
    temp0c <- L00
    if (read-byte(6eb) = temp0c) is false then
        jump-to: LABEL 14
LABEL 12
    if (((read-byte(6e7) & 0001) <> 0) = 1) is true then
        jump-to: LABEL 14
LABEL 13
    write-byte(6e7) <- byte(read-byte(6e7) | 0001)
    L02 <- 00
LABEL 14
    temp0d <- L00
    if (((read-byte(((temp0d - 1) * 9) + 2ee) & 0010) <> 0) = 1) is false then
        jump-to: LABEL 1a
LABEL 15
    temp0e <- L00
    push-SP: call 103b8 (temp0e, 72, 4b)
    temp0f <- L00
    if (((read-byte(((temp0f - 1) * 9) + 2ee) & 0004) <> 0) = 1) is false then
        jump-to: LABEL 18
LABEL 16
    temp10 <- read-word(2271)
    if (((read-byte(((temp10 - 1) * 9) + 2ee) & 0004) <> 0) = 1) is false then
        jump-to: LABEL 18
LABEL 17
    temp11 <- L00
    push-SP: call d32c (temp11)
    jump-to: LABEL 19
LABEL 18
    temp12 <- L00
    push-SP: call 1034c (temp12)
LABEL 19
    temp13 <- pop-SP
    L04 <- temp13
LABEL 1a
    temp14 <- L03
    if (temp14 = 0) is false then
        jump-to: LABEL 1c
LABEL 1b
    push-SP: 01
    jump-to: LABEL 1d
LABEL 1c
    push-SP: 00
LABEL 1d
    temp15 <- pop-SP
    L03 <- temp15
    temp16 <- L03
    if (temp16 = 0) is true then
        jump-to: LABEL 36
LABEL 1e
    temp17 <- L02
    if (temp17 = 0) is false then
        jump-to: LABEL 36
LABEL 1f
    push-SP: call 1033c ()
LABEL 20
    temp18 <- L00
    if (temp18 = 0) is true then
        jump-to: LABEL 23
LABEL 21
    temp19 <- L00
    L00 <- read-byte((((temp19 - 1) * 9) + 2ee) + 5)
    if (read-byte((((temp19 - 1) * 9) + 2ee) + 5) <> 0) is false then
        jump-to: LABEL 23
LABEL 22
    jump-to: LABEL 24
LABEL 23
    L00 <- read-byte(5cd)
    if (read-byte(5cd) <> 0) is true then
        jump-to: LABEL 24
LABEL 24
    temp1a <- L00
    if (((read-byte((((temp1a - 1) * 9) + 2ee) + 1) & 0040) <> 0) = 1) is true then
        jump-to: LABEL 20
LABEL 25
    temp1b <- L00
    if (((read-byte(((temp1b - 1) * 9) + 2ee) & 0002) <> 0) = 1) is false then
        jump-to: LABEL 20
LABEL 26
    temp1c <- L00
    temp1d <- 0
    if (read-byte(6eb) = 0) is false then
        jump-to: LABEL 28
LABEL 27
    temp1e <- 0
    jump-to: LABEL 29
LABEL 28
    temp1e <- read-byte((((read-byte(6eb) - 1) * 9) + 2ee) + 6)
LABEL 29
    if (temp1e <> 72) is false then
        jump-to: LABEL 2f
LABEL 2a
    temp1f <- temp1e
LABEL 2b
    temp20 <- read-byte((((temp1f - 1) * 9) + 2ee) + 5)
    if (temp20 = 72) is false then
        jump-to: LABEL 2d
LABEL 2c
    temp1d <- temp1f
    temp1f <- 0
    jump-to: LABEL 2e
LABEL 2d
    temp1f <- temp20
LABEL 2e
    if (temp1f <> 0) is true then
        jump-to: LABEL 2b
LABEL 2f
    if (temp1d <> 0) is false then
        jump-to: LABEL 31
LABEL 30
    write-byte((((temp1d - 1) * 9) + 2ee) + 5) <- read-byte(6ec)
LABEL 31
    if (temp1e = 72) is false then
        jump-to: LABEL 33
LABEL 32
    write-byte((((read-byte(6eb) - 1) * 9) + 2ee) + 6) <- read-byte(6ec)
LABEL 33
    write-byte(6eb) <- 0
    write-byte(6ec) <- 0
    if (temp1c <> 0) is false then
        jump-to: LABEL 35
LABEL 34
    write-byte(6eb) <- temp1c
    write-byte(6ec) <- read-byte((((temp1c - 1) * 9) + 2ee) + 6)
    write-byte((((temp1c - 1) * 9) + 2ee) + 6) <- 72
LABEL 35
    write-byte(6e7) <- byte(read-byte(6e7) & not 0020)
    write-byte(6e7) <- byte(read-byte(6e7) | 0001)
    write-word(22af) <- 00
    jump-to: LABEL 01
LABEL 36
    temp21 <- L00
    if (temp21 = be) is true then
        jump-to: LABEL 38
LABEL 37
    temp22 <- L00
    push-SP: call 102c4 (temp22)
    temp23 <- L04
    return: temp23
LABEL 38
    temp24 <- L04
    return: temp24
]]>

        TestBinder(Zork1, &H101E0, expected)
    End Sub

#End Region

End Module
