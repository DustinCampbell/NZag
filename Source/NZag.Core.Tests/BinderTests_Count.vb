Imports Xunit

Public Class BinderTests_Count

#Region "Count_24AC (debugging)"

#Region "ZCode"
    '24ad:  10 00 21 1b             LOADB           #00,#21 -> G0b
    '24b1:  10 00 20 1c             LOADB           #00,#20 -> G0c
    '24b5:  55 1b 01 00             SUB             G0b,#01 -> -(SP)
    '24b9:  e2 9b 21 00 00          STOREB          G11,#00,(SP)+
    '24be:  e2 97 21 01 3c          STOREB          G11,#01,#3c
    '24c3:  0d 01 02                STORE           L00,#02
    '24c6:  55 1b 02 00             SUB             G0b,#02 -> -(SP)
    '24ca:  62 01 00 00 0c          JL              L00,(SP)+ [FALSE] 24d9
    '24cf:  e2 a7 21 01 2d          STOREB          G11,L00,#2d
    '24d4:  95 01                   INC             L00
    '24d6:  8c ff ef                JUMP            24c6
    '24d9:  e2 a7 21 01 3e          STOREB          G11,L00,#3e
    '24de:  b0                      RTRUE           
#End Region

    <Fact>
    Sub Count_24AC()
        Dim expected =
        <![CDATA[
# temps: 12

LABEL 00
    DEBUG: "24ad: loadb {0:x2} {1:x2}" (00, 21)
    write-word(e91) <- read-byte(21)
    DEBUG: "24b1: loadb {0:x2} {1:x2}" (00, 20)
    write-word(e93) <- read-byte(20)
    temp00 <- read-word(e91)
    DEBUG: "24b5: sub G0b={0:x} {1:x2}" (temp00, 01)
    push-SP: (int16(temp00) - int16(01))
    temp01 <- read-word(e9d)
    temp02 <- pop-SP
    DEBUG: "24b9: storeb G11={0:x} {1:x2} SP={2:x}" (temp01, 00, temp02)
    write-byte(temp01) <- temp02
    temp03 <- read-word(e9d)
    DEBUG: "24be: storeb G11={0:x} {1:x2} {2:x2}" (temp03, 01, 3c)
    write-byte(temp03 + 01) <- 3c
    DEBUG: "24c3: store {0:x2} {1:x2}" (01, 02)
    temp04 <- 02
LABEL 01
    temp05 <- read-word(e91)
    DEBUG: "24c6: sub G0b={0:x} {1:x2}" (temp05, 02)
    push-SP: (int16(temp05) - int16(02))
    temp06 <- temp04
    temp07 <- pop-SP
    DEBUG: "24ca: jl L00={0:x} SP={1:x}" (temp06, temp07)
    if (int16(temp06) < int16(temp07)) is false then
        jump-to: LABEL 03
LABEL 02
    temp08 <- read-word(e9d)
    temp09 <- temp04
    DEBUG: "24cf: storeb G11={0:x} L00={1:x} {2:x2}" (temp08, temp09, 2d)
    write-byte(temp08 + temp09) <- 2d
    DEBUG: "24d4: inc {0:x2}" (01)
    temp04 <- (int16(temp04) + int16(1))
    DEBUG: "24d6: jump {0:x4}" (ffef)
    jump-to: LABEL 01
LABEL 03
    temp0a <- read-word(e9d)
    temp0b <- temp04
    DEBUG: "24d9: storeb G11={0:x} L00={1:x} {2:x2}" (temp0a, temp0b, 3e)
    write-byte(temp0a + temp0b) <- 3e
    DEBUG: "24de: rtrue" ()
    return: 1
]]>

        TestBinder(Count, &H24AC, expected, debugging:=True)
    End Sub

#End Region

End Class
