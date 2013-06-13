Public Module BinderTests_CZech

#Region "CZech_7DC"

#Region "ZCode"
    ' 7dd:  e0 3f 09 12 ff          call_vs         2448 -> gef
    ' 7e2:  ba                      quit  
#End Region

    <Fact>
    Sub CZech_7DC()
        Dim expected =
<![CDATA[
# temps: 0

LABEL 00
    write-word(6ce) <- call 2448 ()
    quit
]]>

        TestBinder(CZech, &H7DC, expected)
    End Sub

#End Region
#Region "CZech_7E4"

#Region "ZCode"
    ' 7e5:  61 02 01 80 46          je              local1 local0 82e
    ' 7ea:  b2 ...                  print           "^^ERROR ["
    ' 7f9:  e6 bf 11                print_num       g01
    ' 7fc:  b2 ...                  print           "] "
    ' 801:  ff 7f 03 4a             check_arg_count #03 ~80d
    ' 805:  b2 ...                  print           "("
    ' 808:  ad 03                   print_paddr     local2
    ' 80a:  b2 ...                  print           ")"
    ' 80d:  8f 02 3b                call_1n         8ec
    ' 810:  b2 ...                  print           " Expected "
    ' 819:  e6 bf 02                print_num       local1
    ' 81c:  b2 ...                  print           "; got "
    ' 823:  e6 bf 01                print_num       local0
    ' 826:  b2 ...                  print           "^^"
    ' 82b:  8c 00 05                jump            831
    ' 82e:  8f 02 38                call_1n         8e0
    ' 831:  b0                      rtrue
#End Region

    <Fact>
    Sub CZech_7E4()
        Dim expected =
<![CDATA[
# temps: 6

LABEL 00
    temp00 <- L01
    temp01 <- L00
    if (temp00 = temp01) is true then
        jump-to: LABEL 04
LABEL 01
    print: "\n\nERROR ["
    temp02 <- read-word(4f2)
    print: number-to-text(int16(temp02))
    print: "] "
    if (03 <= arg-count) is false then
        jump-to: LABEL 03
LABEL 02
    print: "("
    temp03 <- L02
    print: read-text(temp03 * 4)
    print: ")"
LABEL 03
    discard: call 8ec ()
    print: " Expected "
    temp04 <- L01
    print: number-to-text(int16(temp04))
    print: "; got "
    temp05 <- L00
    print: number-to-text(int16(temp05))
    print: "\n\n"
    jump-to: LABEL 05
LABEL 04
    discard: call 8e0 ()
LABEL 05
    return: 1
]]>

        TestBinder(CZech, &H7E4, expected)
    End Sub

#End Region
#Region "CZech_8F4"

#Region "ZCode"
    ' 8f5:  b2 ...                  print           "Jumps"
    ' 8fa:  a0 01 ca                jz              local0 905
    ' 8fd:  b2 ...                  print           " skipped"
    ' 904:  b1                      rfalse
    ' 905:  b2 ...                  print           " ["
    ' 90a:  54 11 01 00             add             g01 #01 -> sp
    ' 90e:  e6 bf 00                print_num       sp
    ' 911:  b2 ...                  print           "]: "
    ' 918:  b2 ...                  print           "jump"
    ' 91d:  8c 00 08                jump            926
    ' 920:  b2 ...                  print           "bad!"
    ' 925:  ba                      quit
    ' 926:  8f 02 38                call_1n         8e0
    ' 929:  b2 ...                  print           "je"
    ' 92c:  01 05 05 01 29          je              #05 #05 ~a58
    ' 931:  8f 02 38                call_1n         8e0
    ' 934:  c1 4f 05 ff fb 81 1f    je              #05 #fffb a58
    ' 93b:  8f 02 38                call_1n         8e0
    ' 93e:  c1 1f ff fb 05 81 15    je              #fffb #05 a58
    ' 945:  8f 02 38                call_1n         8e0
    ' 948:  c1 0f ff fb ff fb 01 0a je              #fffb #fffb ~a58
    ' 950:  8f 02 38                call_1n         8e0
    ' 953:  c1 0f 7f ff 80 00 80 ff je              #7fff #8000 a58
    ' 95b:  8f 02 38                call_1n         8e0
    ' 95e:  c1 0f 80 00 80 00 00 f4 je              #8000 #8000 ~a58
    ' 966:  8f 02 38                call_1n         8e0
    ' 969:  c1 57 05 04 05 00 ea    je              #05 #04 #05 ~a58
    ' 970:  8f 02 38                call_1n         8e0
    ' 973:  c1 55 05 04 03 05 00 df je              #05 #04 #03 #05 ~a58
    ' 97b:  8f 02 38                call_1n         8e0
    ' 97e:  c1 55 05 04 05 03 00 d4 je              #05 #04 #05 #03 ~a58
    ' 986:  8f 02 38                call_1n         8e0
    ' 989:  c1 55 05 04 03 02 80 c9 je              #05 #04 #03 #02 a58
    ' 991:  8f 02 38                call_1n         8e0
    ' 994:  b2 ...                  print           "jg"
    ' 997:  03 05 05 80 be          jg              #05 #05 a58
    ' 99c:  8f 02 38                call_1n         8e0
    ' 99f:  03 01 00 00 b6          jg              #01 #00 ~a58
    ' 9a4:  8f 02 38                call_1n         8e0
    ' 9a7:  03 00 01 80 ae          jg              #00 #01 a58
    ' 9ac:  8f 02 38                call_1n         8e0
    ' 9af:  c3 0f ff ff ff fe 00 a3 jg              #ffff #fffe ~a58
    ' 9b7:  8f 02 38                call_1n         8e0
    ' 9ba:  c3 0f ff fe ff ff 80 98 jg              #fffe #ffff a58
    ' 9c2:  8f 02 38                call_1n         8e0
    ' 9c5:  c3 4f 01 ff ff 00 8e    jg              #01 #ffff ~a58
    ' 9cc:  8f 02 38                call_1n         8e0
    ' 9cf:  c3 1f ff ff 01 80 84    jg              #ffff #01 a58
    ' 9d6:  8f 02 38                call_1n         8e0
    ' 9d9:  b2 ...                  print           "jl"
    ' 9dc:  02 05 05 80 79          jl              #05 #05 a58
    ' 9e1:  8f 02 38                call_1n         8e0
    ' 9e4:  02 01 00 80 71          jl              #01 #00 a58
    ' 9e9:  8f 02 38                call_1n         8e0
    ' 9ec:  02 00 01 00 69          jl              #00 #01 ~a58
    ' 9f1:  8f 02 38                call_1n         8e0
    ' 9f4:  c2 0f ff ff ff fe 80 5e jl              #ffff #fffe a58
    ' 9fc:  8f 02 38                call_1n         8e0
    ' 9ff:  c2 0f ff fe ff ff 00 53 jl              #fffe #ffff ~a58
    ' a07:  8f 02 38                call_1n         8e0
    ' a0a:  c2 4f 01 ff ff 80 49    jl              #01 #ffff a58
    ' a11:  8f 02 38                call_1n         8e0
    ' a14:  c2 1f ff ff 01 00 3f    jl              #ffff #01 ~a58
    ' a1b:  8f 02 38                call_1n         8e0
    ' a1e:  b2 ...                  print           "jz"
    ' a21:  90 00 76                jz              #00 ~a58
    ' a24:  8f 02 38                call_1n         8e0
    ' a27:  90 01 f0                jz              #01 a58
    ' a2a:  8f 02 38                call_1n         8e0
    ' a2d:  80 ff fc e9             jz              #fffc a58
    ' a31:  8f 02 38                call_1n         8e0
    ' a34:  b2 ...                  print           "offsets"
    ' a3b:  d9 1f 02 a3 00 02       call_2s         a8c #00 -> local1
    ' a41:  f9 24 01 f9 02 00 0b 1f call_vn         7e4 local1 #00 s002
    ' a49:  d9 1f 02 a3 01 02       call_2s         a8c #01 -> local1
    ' a4f:  f9 24 01 f9 02 01 0b 21 call_vn         7e4 local1 #01 s003
    ' a57:  b0 rtrue
    ' a58:  b2 ...                  print           "^bad ["
    ' a61:  e6 bf 11                print_num       g01
    ' a64:  b2 ...                  print           "]!^"
    ' a6b:  b2 ...                  print           "Quitting tests because jumps don't work!"
    ' a8a:  ba                      quit
#End Region

    <Fact>
    Sub CZech_8F4()
        Dim expected =
<![CDATA[
# temps: 5

LABEL 00
    print: "Jumps"
    temp00 <- L00
    if (temp00 = 0) is true then
        jump-to: LABEL 02
LABEL 01
    print: " skipped"
    return: 0
LABEL 02
    print: " ["
    temp01 <- read-word(4f2)
    temp02 <- (int16(temp01) + int16(01))
    print: number-to-text(int16(temp02))
    print: "]: "
    print: "jump"
    jump-to: LABEL 03
    print: "bad!"
    quit
LABEL 03
    discard: call 8e0 ()
    print: "je"
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    print: "jg"
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    print: "jl"
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    print: "jz"
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    print: "offsets"
    L01 <- call a8c (00)
    temp03 <- L01
    discard: call 7e4 (temp03, 00, 0b1f)
    L01 <- call a8c (01)
    temp04 <- L01
    discard: call 7e4 (temp04, 01, 0b21)
    return: 1
]]>

        TestBinder(CZech, &H8F4, expected)
    End Sub

#End Region
#Region "CZech_AA4"

#Region "ZCode"
    ' aa5:  b2 ...                  print           "Variables"
    ' aae:  a0 01 ca                jz              local0 ab9
    ' ab1:  b2 ...                  print           " skipped"
    ' ab8:  b1                      rfalse
    ' ab9:  b2 ...                  print           " ["
    ' abe:  54 11 01 00             add             g01 #01 -> sp
    ' ac2:  e6 bf 00                print_num       sp
    ' ac5:  b2 ...                  print           "]: "
    ' acc:  b2 ...                  print           "push/pull"
    ' ad5:  e8 7f 09                push            #09
    ' ad8:  e8 7f 08                push            #08
    ' adb:  e9 7f 02                pull            local1
    ' ade:  f9 24 01 f9 02 08 0b 23 call_vn         7e4 local1 #08 s004
    ' ae6:  e9 7f 15                pull            g05
    ' ae9:  f9 24 01 f9 15 09 0b 26 call_vn         7e4 g05 #09 s005
    ' af1:  b2 ...                  print           "store"
    ' af6:  0d 02 05                store           local1 #05
    ' af9:  f9 27 01 f9 02 05       call_vn         7e4 local1 #05
    ' aff:  b2 ...                  print           "load"
    ' b04:  0d 03 05                store           local2 #05
    ' b07:  0d 02 06                store           local1 #06
    ' b0a:  9e 02 00                load            local1 -> sp
    ' b0d:  e9 7f 03                pull            local2
    ' b10:  f9 2b 01 f9 02 03       call_vn         7e4 local1 local2
    ' b16:  b2 ...                  print           "dec"
    ' b19:  f9 17 03 30 05 04       call_vn         cc0 #05 #04
    ' b1f:  f9 13 03 30 00 ff ff    call_vn         cc0 #00 #ffff
    ' b26:  f9 03 03 30 ff f8 ff f7 call_vn         cc0 #fff8 #fff7
    ' b2e:  f9 03 03 30 80 00 7f ff call_vn         cc0 #8000 #7fff
    ' b36:  e8 7f 01                push            #01
    ' b39:  e8 7f 0a                push            #0a
    ' b3c:  96 00                   dec             sp
    ' b3e:  e9 7f 03                pull            local2
    ' b41:  f9 24 01 f9 03 09 0b 29 call_vn         7e4 local2 #09 s006
    ' b49:  e9 7f 03                pull            local2
    ' b4c:  f9 24 01 f9 03 01 0b 2a call_vn         7e4 local2 #01 s007
    ' b54:  0d 10 03                store           g00 #03
    ' b57:  96 10                   dec             g00
    ' b59:  f9 24 01 f9 10 02 0b 2b call_vn         7e4 g00 #02 s008
    ' b61:  b2 ...                  print           "inc"
    ' b64:  f9 17 03 2c 05 06       call_vn         cb0 #05 #06
    ' b6a:  f9 07 03 2c ff ff 00    call_vn         cb0 #ffff #00
    ' b71:  f9 03 03 2c ff f8 ff f9 call_vn         cb0 #fff8 #fff9
    ' b79:  f9 03 03 2c 7f ff 80 00 call_vn         cb0 #7fff #8000
    ' b81:  e8 7f 01                push            #01
    ' b84:  e8 7f 0a                push            #0a
    ' b87:  95 00                   inc             sp
    ' b89:  e9 7f 03                pull            local2
    ' b8c:  f9 24 01 f9 03 0b 0b 2d call_vn         7e4 local2 #0b s009
    ' b94:  e9 7f 03                pull            local2
    ' b97:  f9 24 01 f9 03 01 0b 2e call_vn         7e4 local2 #01 s010
    ' b9f:  0d 10 03                store           g00 #03
    ' ba2:  95 10                   inc             g00
    ' ba4:  f9 24 01 f9 10 04 0b 2f call_vn         7e4 g00 #04 s011
    ' bac:  b2 ...                  print           "^    dec_chk"
    ' bb7:  0d 03 03                store           local2 #03
    ' bba:  c4 4f 03 03 e8 00 69    dec_chk         local2 #03e8 ~c28
    ' bc1:  8f 02 38                call_1n         8e0
    ' bc4:  04 03 01 80 61          dec_chk         local2 #01 c28
    ' bc9:  8f 02 38                call_1n         8e0
    ' bcc:  04 03 01 00 59          dec_chk         local2 #01 ~c28
    ' bd1:  8f 02 38                call_1n         8e0
    ' bd4:  04 03 00 00 51          dec_chk         local2 #00 ~c28
    ' bd9:  8f 02 38                call_1n         8e0
    ' bdc:  c4 4f 03 ff fe 80 47    dec_chk         local2 #fffe c28
    ' be3:  8f 02 38                call_1n         8e0
    ' be6:  c4 4f 03 ff fe 00 3d    dec_chk         local2 #fffe ~c28
    ' bed:  8f 02 38                call_1n         8e0
    ' bf0:  c4 4f 03 03 e8 74       dec_chk         local2 #03e8 ~c28
    ' bf6:  8f 02 38                call_1n         8e0
    ' bf9:  c4 4f 03 fe 0c eb       dec_chk         local2 #fe0c c28
    ' bff:  8f 02 38                call_1n         8e0
    ' c02:  e8 7f 01                push            #01
    ' c05:  e8 7f 0a                push            #0a
    ' c08:  04 00 05 de             dec_chk         sp #05 c28
    ' c0c:  8f 02 38                call_1n         8e0
    ' c0f:  e9 7f 03                pull            local2
    ' c12:  f9 24 01 f9 03 09 0b 31 call_vn         7e4 local2 #09 s012
    ' c1a:  e9 7f 03                pull            local2
    ' c1d:  f9 24 01 f9 03 01 0b 33 call_vn         7e4 local2 #01 s013
    ' c25:  8c 00 16                jump            c3c
    ' c28:  b2 ...                  print           "^bad ["
    ' c31:  e6 bf 11                print_num       g01
    ' c34:  b2 ...                  print           "]^"
    ' c39:  8f 02 3b                call_1n         8ec
    ' c3c:  b2 ...                  print           "inc_chk"
    ' c43:  cd 4f 03 ff fa          store           local2 #fffa
    ' c48:  c5 4f 03 fe 0c 00 49    inc_chk         local2 #fe0c ~c96
    ' c4f:  8f 02 38                call_1n         8e0
    ' c52:  c5 4f 03 03 e8 80 3f    inc_chk         local2 #03e8 c96
    ' c59:  8f 02 38                call_1n         8e0
    ' c5c:  c5 4f 03 ff fd f6       inc_chk         local2 #fffd c96
    ' c62:  8f 02 38                call_1n         8e0
    ' c65:  c5 4f 03 ff fd 6d       inc_chk         local2 #fffd ~c96
    ' c6b:  8f 02 38                call_1n         8e0
    ' c6e:  05 03 00 e6             inc_chk         local2 #00 c96
    ' c72:  8f 02 38                call_1n         8e0
    ' c75:  05 03 01 df             inc_chk         local2 #01 c96
    ' c79:  8f 02 38                call_1n         8e0
    ' c7c:  05 03 01 d8             inc_chk         local2 #01 c96
    ' c80:  8f 02 38                call_1n         8e0
    ' c83:  05 03 01 51             inc_chk         local2 #01 ~c96
    ' c87:  8f 02 38                call_1n         8e0
    ' c8a:  c5 4f 03 03 e8 c8       inc_chk         local2 #03e8 c96
    ' c90:  8f 02 38                call_1n         8e0
    ' c93:  8c 00 18                jump            cac
    ' c96:  b2 ...                  print           "^bad ["
    ' c9f:  e6 bf 11                print_num       g01
    ' ca2:  b2 ...                  print           "]!^"
    ' ca9:  8f 02 3b                call_1n         8ec
    ' cac:  b0                      rtrue
#End Region

    <Fact>
    Sub CZech_AA4()
        Dim expected =
<![CDATA[
# temps: 67

LABEL 00
    print: "Variables"
    temp00 <- L00
    if (temp00 = 0) is true then
        jump-to: LABEL 02
LABEL 01
    print: " skipped"
    return: 0
LABEL 02
    print: " ["
    temp01 <- read-word(4f2)
    temp02 <- (int16(temp01) + int16(01))
    print: number-to-text(int16(temp02))
    print: "]: "
    print: "push/pull"
    push-SP: 09
    push-SP: 08
    temp03 <- pop-SP
    L01 <- temp03
    temp04 <- L01
    discard: call 7e4 (temp04, 08, 0b23)
    temp05 <- pop-SP
    write-word(4fa) <- temp05
    temp06 <- read-word(4fa)
    discard: call 7e4 (temp06, 09, 0b26)
    print: "store"
    L01 <- 05
    temp07 <- L01
    discard: call 7e4 (temp07, 05)
    print: "load"
    L02 <- 05
    L01 <- 06
    temp08 <- L01
    L02 <- temp08
    temp09 <- L01
    temp0a <- L02
    discard: call 7e4 (temp09, temp0a)
    print: "dec"
    discard: call cc0 (05, 04)
    discard: call cc0 (00, ffff)
    discard: call cc0 (fff8, fff7)
    discard: call cc0 (8000, 7fff)
    push-SP: 01
    push-SP: 0a
    temp0b <- peek-SP
    update-SP: (int16(temp0b) - int16(1))
    temp0c <- pop-SP
    L02 <- temp0c
    temp0d <- L02
    discard: call 7e4 (temp0d, 09, 0b29)
    temp0e <- pop-SP
    L02 <- temp0e
    temp0f <- L02
    discard: call 7e4 (temp0f, 01, 0b2a)
    write-word(4f0) <- 03
    temp10 <- read-word(4f0)
    write-word(4f0) <- (int16(temp10) - int16(1))
    temp11 <- read-word(4f0)
    discard: call 7e4 (temp11, 02, 0b2b)
    print: "inc"
    discard: call cb0 (05, 06)
    discard: call cb0 (ffff, 00)
    discard: call cb0 (fff8, fff9)
    discard: call cb0 (7fff, 8000)
    push-SP: 01
    push-SP: 0a
    temp12 <- peek-SP
    update-SP: (int16(temp12) + int16(1))
    temp13 <- pop-SP
    L02 <- temp13
    temp14 <- L02
    discard: call 7e4 (temp14, 0b, 0b2d)
    temp15 <- pop-SP
    L02 <- temp15
    temp16 <- L02
    discard: call 7e4 (temp16, 01, 0b2e)
    write-word(4f0) <- 03
    temp17 <- read-word(4f0)
    write-word(4f0) <- (int16(temp17) + int16(1))
    temp18 <- read-word(4f0)
    discard: call 7e4 (temp18, 04, 0b2f)
    print: "\n    dec_chk"
    L02 <- 03
    temp19 <- L02
    temp1a <- (int16(temp19) - int16(1))
    L02 <- temp1a
    if (int16(temp1a) < int16(03e8)) is false then
        jump-to: LABEL 0c
LABEL 03
    discard: call 8e0 ()
    temp1b <- L02
    temp1c <- (int16(temp1b) - int16(1))
    L02 <- temp1c
    if (int16(temp1c) < int16(01)) is true then
        jump-to: LABEL 0c
LABEL 04
    discard: call 8e0 ()
    temp1d <- L02
    temp1e <- (int16(temp1d) - int16(1))
    L02 <- temp1e
    if (int16(temp1e) < int16(01)) is false then
        jump-to: LABEL 0c
LABEL 05
    discard: call 8e0 ()
    temp1f <- L02
    temp20 <- (int16(temp1f) - int16(1))
    L02 <- temp20
    if (int16(temp20) < int16(00)) is false then
        jump-to: LABEL 0c
LABEL 06
    discard: call 8e0 ()
    temp21 <- L02
    temp22 <- (int16(temp21) - int16(1))
    L02 <- temp22
    if (int16(temp22) < int16(fffe)) is true then
        jump-to: LABEL 0c
LABEL 07
    discard: call 8e0 ()
    temp23 <- L02
    temp24 <- (int16(temp23) - int16(1))
    L02 <- temp24
    if (int16(temp24) < int16(fffe)) is false then
        jump-to: LABEL 0c
LABEL 08
    discard: call 8e0 ()
    temp25 <- L02
    temp26 <- (int16(temp25) - int16(1))
    L02 <- temp26
    if (int16(temp26) < int16(03e8)) is false then
        jump-to: LABEL 0c
LABEL 09
    discard: call 8e0 ()
    temp27 <- L02
    temp28 <- (int16(temp27) - int16(1))
    L02 <- temp28
    if (int16(temp28) < int16(fe0c)) is true then
        jump-to: LABEL 0c
LABEL 0a
    discard: call 8e0 ()
    push-SP: 01
    push-SP: 0a
    temp29 <- peek-SP
    temp2a <- (int16(temp29) - int16(1))
    update-SP: temp2a
    if (int16(temp2a) < int16(05)) is true then
        jump-to: LABEL 0c
LABEL 0b
    discard: call 8e0 ()
    temp2b <- pop-SP
    L02 <- temp2b
    temp2c <- L02
    discard: call 7e4 (temp2c, 09, 0b31)
    temp2d <- pop-SP
    L02 <- temp2d
    temp2e <- L02
    discard: call 7e4 (temp2e, 01, 0b33)
    jump-to: LABEL 0d
LABEL 0c
    print: "\nbad ["
    temp2f <- read-word(4f2)
    print: number-to-text(int16(temp2f))
    print: "]\n"
    discard: call 8ec ()
LABEL 0d
    print: "inc_chk"
    L02 <- fffa
    temp30 <- L02
    temp31 <- (int16(temp30) + int16(1))
    L02 <- temp31
    if (int16(temp31) > int16(fe0c)) is false then
        jump-to: LABEL 17
LABEL 0e
    discard: call 8e0 ()
    temp32 <- L02
    temp33 <- (int16(temp32) + int16(1))
    L02 <- temp33
    if (int16(temp33) > int16(03e8)) is true then
        jump-to: LABEL 17
LABEL 0f
    discard: call 8e0 ()
    temp34 <- L02
    temp35 <- (int16(temp34) + int16(1))
    L02 <- temp35
    if (int16(temp35) > int16(fffd)) is true then
        jump-to: LABEL 17
LABEL 10
    discard: call 8e0 ()
    temp36 <- L02
    temp37 <- (int16(temp36) + int16(1))
    L02 <- temp37
    if (int16(temp37) > int16(fffd)) is false then
        jump-to: LABEL 17
LABEL 11
    discard: call 8e0 ()
    temp38 <- L02
    temp39 <- (int16(temp38) + int16(1))
    L02 <- temp39
    if (int16(temp39) > int16(00)) is true then
        jump-to: LABEL 17
LABEL 12
    discard: call 8e0 ()
    temp3a <- L02
    temp3b <- (int16(temp3a) + int16(1))
    L02 <- temp3b
    if (int16(temp3b) > int16(01)) is true then
        jump-to: LABEL 17
LABEL 13
    discard: call 8e0 ()
    temp3c <- L02
    temp3d <- (int16(temp3c) + int16(1))
    L02 <- temp3d
    if (int16(temp3d) > int16(01)) is true then
        jump-to: LABEL 17
LABEL 14
    discard: call 8e0 ()
    temp3e <- L02
    temp3f <- (int16(temp3e) + int16(1))
    L02 <- temp3f
    if (int16(temp3f) > int16(01)) is false then
        jump-to: LABEL 17
LABEL 15
    discard: call 8e0 ()
    temp40 <- L02
    temp41 <- (int16(temp40) + int16(1))
    L02 <- temp41
    if (int16(temp41) > int16(03e8)) is true then
        jump-to: LABEL 17
LABEL 16
    discard: call 8e0 ()
    jump-to: LABEL 18
LABEL 17
    print: "\nbad ["
    temp42 <- read-word(4f2)
    print: number-to-text(int16(temp42))
    print: "]!\n"
    discard: call 8ec ()
LABEL 18
    return: 1
]]>

        TestBinder(CZech, &HAA4, expected)
    End Sub

#End Region
#Region "CZech_1AC8"

#Region "ZCode"
    ' 1ac9:  73 01 02 04             get_next_prop   local0 local1 -> local3
    ' 1acd:  2d 16 01                store           g06 local0
    ' 1ad0:  2d 17 02                store           g07 local1
    ' 1ad3:  f9 28 02 21 04 03 0b 6b call_vn         884 local3 local2 s035
    ' 1adb:  b0                      rtrue
#End Region

    <Fact>
    Sub CZech_1AC8()
        Dim expected =
<![CDATA[
# temps: 11

LABEL 00
    temp00 <- L00
    temp01 <- L01
    temp02 <- read-word((((temp00 - 1) * e) + 18c) + c)
    temp03 <- uint16((temp02 + 1) + (read-byte(temp02) * 2))
    if (temp01 <> 0) is false then
        jump-to: LABEL 07
LABEL 01
    temp04 <- read-byte(temp03)
    temp05 <- read-byte(temp03)
    if ((temp05 & 80) <> 80) is false then
        jump-to: LABEL 03
LABEL 02
    temp06 <- (temp05 >> 6)
    jump-to: LABEL 06
LABEL 03
    if ((read-byte(temp03 + 1) & 3f) = 0) is false then
        jump-to: LABEL 05
LABEL 04
    temp06 <- 40
    jump-to: LABEL 06
LABEL 05
    temp06 <- (read-byte(temp03 + 1) & 3f)
LABEL 06
    temp03 <- uint16((temp03 + 1) + (temp06 + 1))
    if ((temp04 & 3f) > temp01) is true then
        jump-to: LABEL 01
LABEL 07
    L03 <- (read-byte(temp03) & 3f)
    temp07 <- L00
    write-word(4fc) <- temp07
    temp08 <- L01
    write-word(4fe) <- temp08
    temp09 <- L03
    temp0a <- L02
    discard: call 884 (temp09, temp0a, 0b6b)
    return: 1
]]>

        TestBinder(CZech, &H1AC8, expected)
    End Sub

#End Region
#Region "CZech_1B34"

#Region "ZCode"
    ' 1b35:  0d 03 33                store           local2 #33
    ' 1b38:  0d 15 3d                store           g05 #3d
    ' 1b3b:  0d 02 47                store           local1 #47
    ' 1b3e:  0d 04 00                store           local3 #00
    ' 1b41:  0d 07 02                store           local6 #02
    ' 1b44:  0d 05 03                store           local4 #03
    ' 1b47:  0d 06 15                store           local5 #15
    ' 1b4a:  cd 4f 0a 03 e7          store           local9 #03e7
    ' 1b4f:  e8 7f 29                push            #29
    ' 1b52:  e8 7f 2a                push            #2a
    ' 1b55:  e8 7f 2b                push            #2b
    ' 1b58:  e8 7f 2c                push            #2c
    ' 1b5b:  e8 7f 2d                push            #2d
    ' 1b5e:  2d ff 01                store           gef local0
    ' 1b61:  41 ff 00 5b             je              gef #00 ~1b7e
    ' 1b65:  b2 ...                  print           "load"
    ' 1b6a:  9e 00 02                load            sp -> local1
    ' 1b6d:  0d 0a 2d                store           local9 #2d
    ' 1b70:  0d 0b 2d                store           local10 #2d
    ' 1b73:  0d 0c 2c                store           local11 #2c
    ' 1b76:  cd 4f 09 0b 6d          store           local8 s037
    ' 1b7b:  8c 04 75                jump            1ff1
    ' 1b7e:  41 ff 01 56             je              gef #01 ~1b96
    ' 1b82:  ae 04 02                load            [local3] -> local1
    ' 1b85:  0d 0a 2d                store           local9 #2d
    ' 1b88:  0d 0b 2d                store           local10 #2d
    ' 1b8b:  0d 0c 2c                store           local11 #2c
    ' 1b8e:  cd 4f 09 0b 71          store           local8 s038
    ' 1b93:  8c 04 5d                jump            1ff1
    ' 1b96:  41 ff 02 59             je              gef #02 ~1bb1
    ' 1b9a:  e8 bf 05                push            local4
    ' 1b9d:  ae 00 02                load            [sp] -> local1
    ' 1ba0:  0d 0a 33                store           local9 #33
    ' 1ba3:  0d 0b 2d                store           local10 #2d
    ' 1ba6:  0d 0c 2c                store           local11 #2c
    ' 1ba9:  cd 4f 09 0b 77          store           local8 s039
    ' 1bae:  8c 04 42                jump            1ff1
    ' 1bb1:  41 ff 03 59             je              gef #03 ~1bcc
    ' 1bb5:  e8 bf 04                push            local3
    ' 1bb8:  ae 00 02                load            [sp] -> local1
    ' 1bbb:  0d 0a 2d                store           local9 #2d
    ' 1bbe:  0d 0b 2d                store           local10 #2d
    ' 1bc1:  0d 0c 2c                store           local11 #2c
    ' 1bc4:  cd 4f 09 0b 7e          store           local8 s040
    ' 1bc9:  8c 04 27                jump            1ff1
    ' 1bcc:  41 ff 04 53             je              gef #04 ~1be1
    ' 1bd0:  9e 00 00                load            sp -> sp
    ' 1bd3:  0d 0b 2d                store           local10 #2d
    ' 1bd6:  0d 0c 2d                store           local11 #2d
    ' 1bd9:  cd 4f 09 0b 85          store           local8 s041
    ' 1bde:  8c 04 12                jump            1ff1
    ' 1be1:  41 ff 05 56             je              gef #05 ~1bf9
    ' 1be5:  e8 bf 05                push            local4
    ' 1be8:  ae 00 00                load            [sp] -> sp
    ' 1beb:  0d 0b 33                store           local10 #33
    ' 1bee:  0d 0c 2d                store           local11 #2d
    ' 1bf1:  cd 4f 09 0b 88          store           local8 s042
    ' 1bf6:  8c 03 fa                jump            1ff1
    ' 1bf9:  41 ff 06 56             je              gef #06 ~1c11
    ' 1bfd:  e8 bf 04                push            local3
    ' 1c00:  ae 00 00                load            [sp] -> sp
    ' 1c03:  0d 0b 2d                store           local10 #2d
    ' 1c06:  0d 0c 2d                store           local11 #2d
    ' 1c09:  cd 4f 09 0b 8f          store           local8 s043
    ' 1c0e:  8c 03 e2                jump            1ff1
    ' 1c11:  41 ff 0a 58             je              gef #0a ~1c2b
    ' 1c15:  b2 ...                  print           "store"
    ' 1c1a:  0d 00 53                store           sp #53
    ' 1c1d:  0d 0b 53                store           local10 #53
    ' 1c20:  0d 0c 2c                store           local11 #2c
    ' 1c23:  cd 4f 09 0b 96          store           local8 s044
    ' 1c28:  8c 03 c8                jump            1ff1
    ' 1c2b:  41 ff 0b 53             je              gef #0b ~1c40
    ' 1c2f:  4d 04 53                store           [local3] #53
    ' 1c32:  0d 0b 53                store           local10 #53
    ' 1c35:  0d 0c 2c                store           local11 #2c
    ' 1c38:  cd 4f 09 0b 99          store           local8 s045
    ' 1c3d:  8c 03 b3                jump            1ff1
    ' 1c40:  41 ff 0c 56             je              gef #0c ~1c58
    ' 1c44:  e8 bf 04                push            local3
    ' 1c47:  4d 00 53                store           [sp] #53
    ' 1c4a:  0d 0b 53                store           local10 #53
    ' 1c4d:  0d 0c 2c                store           local11 #2c
    ' 1c50:  cd 4f 09 0b 9e          store           local8 s046
    ' 1c55:  8c 03 9b                jump            1ff1
    ' 1c58:  41 ff 0d 56             je              gef #0d ~1c70
    ' 1c5c:  4d 07 53                store           [local6] #53
    ' 1c5f:  0d 0a 53                store           local9 #53
    ' 1c62:  0d 0b 2d                store           local10 #2d
    ' 1c65:  0d 0c 2c                store           local11 #2c
    ' 1c68:  cd 4f 09 0b a4          store           local8 s047
    ' 1c6d:  8c 03 83                jump            1ff1
    ' 1c70:  41 ff 0e 59             je              gef #0e ~1c8b
    ' 1c74:  e8 bf 07                push            local6
    ' 1c77:  4d 00 53                store           [sp] #53
    ' 1c7a:  0d 0a 53                store           local9 #53
    ' 1c7d:  0d 0b 2d                store           local10 #2d
    ' 1c80:  0d 0c 2c                store           local11 #2c
    ' 1c83:  cd 4f 09 0b a9          store           local8 s048
    ' 1c88:  8c 03 68                jump            1ff1
    ' 1c8b:  41 ff 0f 56             je              gef #0f ~1ca3
    ' 1c8f:  2d 02 00                store           local1 sp
    ' 1c92:  0d 0a 2d                store           local9 #2d
    ' 1c95:  0d 0b 2c                store           local10 #2c
    ' 1c98:  0d 0c 2b                store           local11 #2b
    ' 1c9b:  cd 4f 09 0b af          store           local8 s049
    ' 1ca0:  8c 03 50                jump            1ff1
    ' 1ca3:  41 ff 10 53             je              gef #10 ~1cb8
    ' 1ca7:  2d 00 00                store           sp sp
    ' 1caa:  0d 0b 2d                store           local10 #2d
    ' 1cad:  0d 0c 2b                store           local11 #2b
    ' 1cb0:  cd 4f 09 0b b2          store           local8 s050
    ' 1cb5:  8c 03 3b                jump            1ff1
    ' 1cb8:  41 ff 11 56             je              gef #11 ~1cd0
    ' 1cbc:  e8 bf 04                push            local3
    ' 1cbf:  6d 00 00                store           [sp] sp
    ' 1cc2:  0d 0b 2d                store           local10 #2d
    ' 1cc5:  0d 0c 2b                store           local11 #2b
    ' 1cc8:  cd 4f 09 0b b4          store           local8 s051
    ' 1ccd:  8c 03 23                jump            1ff1
    ' 1cd0:  41 ff 12 56             je              gef #12 ~1ce8
    ' 1cd4:  6d 07 00                store           [local6] sp
    ' 1cd7:  0d 0a 2d                store           local9 #2d
    ' 1cda:  0d 0b 2c                store           local10 #2c
    ' 1cdd:  0d 0c 2b                store           local11 #2b
    ' 1ce0:  cd 4f 09 0b ba          store           local8 s052
    ' 1ce5:  8c 03 0b                jump            1ff1
    ' 1ce8:  41 ff 13 59             je              gef #13 ~1d03
    ' 1cec:  e8 bf 07                push            local6
    ' 1cef:  6d 00 00                store           [sp] sp
    ' 1cf2:  0d 0a 2d                store           local9 #2d
    ' 1cf5:  0d 0b 2c                store           local10 #2c
    ' 1cf8:  0d 0c 2b                store           local11 #2b
    ' 1cfb:  cd 4f 09 0b bf          store           local8 s053
    ' 1d00:  8c 02 f0                jump            1ff1
    ' 1d03:  41 ff 14 5f             je              gef #14 ~1d24
    ' 1d07:  b2 ...                  print           "^    pull"
    ' 1d10:  e9 7f 02                pull            local1
    ' 1d13:  0d 0a 2d                store           local9 #2d
    ' 1d16:  0d 0b 2c                store           local10 #2c
    ' 1d19:  0d 0c 2b                store           local11 #2b
    ' 1d1c:  cd 4f 09 0b c5          store           local8 s054
    ' 1d21:  8c 02 cf                jump            1ff1
    ' 1d24:  41 ff 15 56             je              gef #15 ~1d3c
    ' 1d28:  e9 bf 07                pull            [local6]
    ' 1d2b:  0d 0a 2d                store           local9 #2d
    ' 1d2e:  0d 0b 2c                store           local10 #2c
    ' 1d31:  0d 0c 2b                store           local11 #2b
    ' 1d34:  cd 4f 09 0b c7          store           local8 s055
    ' 1d39:  8c 02 b7                jump            1ff1
    ' 1d3c:  41 ff 16 59             je              gef #16 ~1d57
    ' 1d40:  e8 bf 07                push            local6
    ' 1d43:  e9 bf 00                pull            [sp]
    ' 1d46:  0d 0a 2d                store           local9 #2d
    ' 1d49:  0d 0b 2c                store           local10 #2c
    ' 1d4c:  0d 0c 2b                store           local11 #2b
    ' 1d4f:  cd 4f 09 0b cb          store           local8 s056
    ' 1d54:  8c 02 9c                jump            1ff1
    ' 1d57:  41 ff 17 53             je              gef #17 ~1d6c
    ' 1d5b:  e9 7f 00                pull            sp
    ' 1d5e:  0d 0b 2d                store           local10 #2d
    ' 1d61:  0d 0c 2b                store           local11 #2b
    ' 1d64:  cd 4f 09 0b d0          store           local8 s057
    ' 1d69:  8c 02 87                jump            1ff1
    ' 1d6c:  41 ff 18 56             je              gef #18 ~1d84
    ' 1d70:  e8 bf 04                push            local3
    ' 1d73:  e9 bf 00                pull            [sp]
    ' 1d76:  0d 0b 2d                store           local10 #2d
    ' 1d79:  0d 0c 2b                store           local11 #2b
    ' 1d7c:  cd 4f 09 0b d2          store           local8 s058
    ' 1d81:  8c 02 6f                jump            1ff1
    ' 1d84:  41 ff 19 53             je              gef #19 ~1d99
    ' 1d88:  e9 bf 04                pull            [local3]
    ' 1d8b:  0d 0b 2d                store           local10 #2d
    ' 1d8e:  0d 0c 2b                store           local11 #2b
    ' 1d91:  cd 4f 09 0b d7          store           local8 s059
    ' 1d96:  8c 02 5a                jump            1ff1
    ' 1d99:  41 ff 1e 58             je              gef #1e ~1db3
    ' 1d9d:  b2 ...                  print           "inc"
    ' 1da0:  95 02                   inc             local1
    ' 1da2:  0d 0a 48                store           local9 #48
    ' 1da5:  0d 0b 2d                store           local10 #2d
    ' 1da8:  0d 0c 2c                store           local11 #2c
    ' 1dab:  cd 4f 09 0b db          store           local8 s060
    ' 1db0:  8c 02 40                jump            1ff1
    ' 1db3:  41 ff 1f 55             je              gef #1f ~1dca
    ' 1db7:  a5 07                   inc             [local6]
    ' 1db9:  0d 0a 48                store           local9 #48
    ' 1dbc:  0d 0b 2d                store           local10 #2d
    ' 1dbf:  0d 0c 2c                store           local11 #2c
    ' 1dc2:  cd 4f 09 0b df          store           local8 s061
    ' 1dc7:  8c 02 29                jump            1ff1
    ' 1dca:  41 ff 20 58             je              gef #20 ~1de4
    ' 1dce:  e8 bf 07                push            local6
    ' 1dd1:  a5 00                   inc             [sp]
    ' 1dd3:  0d 0a 48                store           local9 #48
    ' 1dd6:  0d 0b 2d                store           local10 #2d
    ' 1dd9:  0d 0c 2c                store           local11 #2c
    ' 1ddc:  cd 4f 09 0b e3          store           local8 s062
    ' 1de1:  8c 02 0f                jump            1ff1
    ' 1de4:  41 ff 21 52             je              gef #21 ~1df8
    ' 1de8:  95 00                   inc             sp
    ' 1dea:  0d 0b 2e                store           local10 #2e
    ' 1ded:  0d 0c 2c                store           local11 #2c
    ' 1df0:  cd 4f 09 0b e8          store           local8 s063
    ' 1df5:  8c 01 fb                jump            1ff1
    ' 1df8:  41 ff 22 52             je              gef #22 ~1e0c
    ' 1dfc:  a5 04                   inc             [local3]
    ' 1dfe:  0d 0b 2e                store           local10 #2e
    ' 1e01:  0d 0c 2c                store           local11 #2c
    ' 1e04:  cd 4f 09 0b e9          store           local8 s064
    ' 1e09:  8c 01 e7                jump            1ff1
    ' 1e0c:  41 ff 23 55             je              gef #23 ~1e23
    ' 1e10:  e8 bf 04                push            local3
    ' 1e13:  a5 00                   inc             [sp]
    ' 1e15:  0d 0b 2e                store           local10 #2e
    ' 1e18:  0d 0c 2c                store           local11 #2c
    ' 1e1b:  cd 4f 09 0b ed          store           local8 s065
    ' 1e20:  8c 01 d0                jump            1ff1
    ' 1e23:  41 ff 28 58             je              gef #28 ~1e3d
    ' 1e27:  b2 ...                  print           "dec"
    ' 1e2a:  96 02                   dec             local1
    ' 1e2c:  0d 0a 46                store           local9 #46
    ' 1e2f:  0d 0b 2d                store           local10 #2d
    ' 1e32:  0d 0c 2c                store           local11 #2c
    ' 1e35:  cd 4f 09 0b f2          store           local8 s066
    ' 1e3a:  8c 01 b6                jump            1ff1
    ' 1e3d:  41 ff 29 55             je              gef #29 ~1e54
    ' 1e41:  a6 07                   dec             [local6]
    ' 1e43:  0d 0a 46                store           local9 #46
    ' 1e46:  0d 0b 2d                store           local10 #2d
    ' 1e49:  0d 0c 2c                store           local11 #2c
    ' 1e4c:  cd 4f 09 0b f6          store           local8 s067
    ' 1e51:  8c 01 9f                jump            1ff1
    ' 1e54:  41 ff 2a 58             je              gef #2a ~1e6e
    ' 1e58:  e8 bf 07                push            local6
    ' 1e5b:  a6 00                   dec             [sp]
    ' 1e5d:  0d 0a 46                store           local9 #46
    ' 1e60:  0d 0b 2d                store           local10 #2d
    ' 1e63:  0d 0c 2c                store           local11 #2c
    ' 1e66:  cd 4f 09 0b fa          store           local8 s068
    ' 1e6b:  8c 01 85                jump            1ff1
    ' 1e6e:  41 ff 2b 52             je              gef #2b ~1e82
    ' 1e72:  96 00                   dec             sp
    ' 1e74:  0d 0b 2c                store           local10 #2c
    ' 1e77:  0d 0c 2c                store           local11 #2c
    ' 1e7a:  cd 4f 09 0b ff          store           local8 s069
    ' 1e7f:  8c 01 71                jump            1ff1
    ' 1e82:  41 ff 2c 52             je              gef #2c ~1e96
    ' 1e86:  a6 04                   dec             [local3]
    ' 1e88:  0d 0b 2c                store           local10 #2c
    ' 1e8b:  0d 0c 2c                store           local11 #2c
    ' 1e8e:  cd 4f 09 0c 00          store           local8 s070
    ' 1e93:  8c 01 5d                jump            1ff1
    ' 1e96:  41 ff 2d 55             je              gef #2d ~1ead
    ' 1e9a:  e8 bf 04                push            local3
    ' 1e9d:  a6 00                   dec             [sp]
    ' 1e9f:  0d 0b 2c                store           local10 #2c
    ' 1ea2:  0d 0c 2c                store           local11 #2c
    ' 1ea5:  cd 4f 09 0c 04          store           local8 s071
    ' 1eaa:  8c 01 46                jump            1ff1
    ' 1ead:  41 ff 32 63             je              gef #32 ~1ed2
    ' 1eb1:  b2 ...                  print           "^    inc_chk"
    ' 1ebc:  cd 4f 09 0c 09          store           local8 s072
    ' 1ec1:  05 02 48 81 4f          inc_chk         local1 #48 2013
    ' 1ec6:  0d 0a 48                store           local9 #48
    ' 1ec9:  0d 0b 2d                store           local10 #2d
    ' 1ecc:  0d 0c 2c                store           local11 #2c
    ' 1ecf:  8c 01 21                jump            1ff1
    ' 1ed2:  41 ff 33 58             je              gef #33 ~1eec
    ' 1ed6:  cd 4f 09 0c 0e          store           local8 s073
    ' 1edb:  45 07 48 81 35          inc_chk         [local6] #48 2013
    ' 1ee0:  0d 0a 48                store           local9 #48
    ' 1ee3:  0d 0b 2d                store           local10 #2d
    ' 1ee6:  0d 0c 2c                store           local11 #2c
    ' 1ee9:  8c 01 07                jump            1ff1
    ' 1eec:  41 ff 34 5b             je              gef #34 ~1f09
    ' 1ef0:  cd 4f 09 0c 13          store           local8 s074
    ' 1ef5:  e8 bf 07                push            local6
    ' 1ef8:  45 00 48 81 18          inc_chk         [sp] #48 2013
    ' 1efd:  0d 0a 48                store           local9 #48
    ' 1f00:  0d 0b 2d                store           local10 #2d
    ' 1f03:  0d 0c 2c                store           local11 #2c
    ' 1f06:  8c 00 ea                jump            1ff1
    ' 1f09:  41 ff 35 55             je              gef #35 ~1f20
    ' 1f0d:  cd 4f 09 0c 19          store           local8 s075
    ' 1f12:  05 00 2e 80 fe          inc_chk         sp #2e 2013
    ' 1f17:  0d 0b 2e                store           local10 #2e
    ' 1f1a:  0d 0c 2c                store           local11 #2c
    ' 1f1d:  8c 00 d3                jump            1ff1
    ' 1f20:  41 ff 36 55             je              gef #36 ~1f37
    ' 1f24:  cd 4f 09 0c 1b          store           local8 s076
    ' 1f29:  45 04 2e 80 e7          inc_chk         [local3] #2e 2013
    ' 1f2e:  0d 0b 2e                store           local10 #2e
    ' 1f31:  0d 0c 2c                store           local11 #2c
    ' 1f34:  8c 00 bc                jump            1ff1
    ' 1f37:  41 ff 37 58             je              gef #37 ~1f51
    ' 1f3b:  cd 4f 09 0c 20          store           local8 s077
    ' 1f40:  e8 bf 04                push            local3
    ' 1f43:  45 00 2e 80 cd          inc_chk         [sp] #2e 2013
    ' 1f48:  0d 0b 2e                store           local10 #2e
    ' 1f4b:  0d 0c 2c                store           local11 #2c
    ' 1f4e:  8c 00 a2                jump            1ff1
    ' 1f51:  41 ff 3c 5f             je              gef #3c ~1f72
    ' 1f55:  b2 ...                  print           "dec_chk"
    ' 1f5c:  cd 4f 09 0c 26          store           local8 s078
    ' 1f61:  04 02 46 80 af          dec_chk         local1 #46 2013
    ' 1f66:  0d 0a 46                store           local9 #46
    ' 1f69:  0d 0b 2d                store           local10 #2d
    ' 1f6c:  0d 0c 2c                store           local11 #2c
    ' 1f6f:  8c 00 81                jump            1ff1
    ' 1f72:  41 ff 3d 58             je              gef #3d ~1f8c
    ' 1f76:  cd 4f 09 0c 2b          store           local8 s079
    ' 1f7b:  44 07 46 80 95          dec_chk         [local6] #46 2013
    ' 1f80:  0d 0a 46                store           local9 #46
    ' 1f83:  0d 0b 2d                store           local10 #2d
    ' 1f86:  0d 0c 2c                store           local11 #2c
    ' 1f89:  8c 00 67                jump            1ff1
    ' 1f8c:  41 ff 3e 5b             je              gef #3e ~1fa9
    ' 1f90:  cd 4f 09 0c 30          store           local8 s080
    ' 1f95:  e8 bf 07                push            local6
    ' 1f98:  44 00 46 80 78          dec_chk         [sp] #46 2013
    ' 1f9d:  0d 0a 46                store           local9 #46
    ' 1fa0:  0d 0b 2d                store           local10 #2d
    ' 1fa3:  0d 0c 2c                store           local11 #2c
    ' 1fa6:  8c 00 4a                jump            1ff1
    ' 1fa9:  41 ff 3f 55             je              gef #3f ~1fc0
    ' 1fad:  cd 4f 09 0c 36          store           local8 s081
    ' 1fb2:  04 00 2c 80 5e          dec_chk         sp #2c 2013
    ' 1fb7:  0d 0b 2c                store           local10 #2c
    ' 1fba:  0d 0c 2c                store           local11 #2c
    ' 1fbd:  8c 00 33                jump            1ff1
    ' 1fc0:  41 ff 40 55             je              gef #40 ~1fd7
    ' 1fc4:  cd 4f 09 0c 38          store           local8 s082
    ' 1fc9:  44 04 2c 80 47          dec_chk         [local3] #2c 2013
    ' 1fce:  0d 0b 2c                store           local10 #2c
    ' 1fd1:  0d 0c 2c                store           local11 #2c
    ' 1fd4:  8c 00 1c                jump            1ff1
    ' 1fd7:  41 ff 41 57             je              gef #41 ~1ff0
    ' 1fdb:  cd 4f 09 0c 3d          store           local8 s083
    ' 1fe0:  e8 bf 04                push            local3
    ' 1fe3:  44 00 2c ee             dec_chk         [sp] #2c 2013
    ' 1fe7:  0d 0b 2c                store           local10 #2c
    ' 1fea:  0d 0c 2c                store           local11 #2c
    ' 1fed:  8c 00 03                jump            1ff1
    ' 1ff0:  b1                      rfalse          
    ' 1ff1:  c1 8f 0a 03 e7 c9       je              local9 #03e7 1ffe
    ' 1ff7:  f9 2a 01 f9 02 0a 09    call_vn         7e4 local1 local9 local8
    ' 1ffe:  e9 7f 08                pull            local7
    ' 2001:  f9 2a 01 f9 08 0b 09    call_vn         7e4 local7 local10 local8
    ' 2008:  e9 7f 08                pull            local7
    ' 200b:  f9 2a 01 f9 08 0c 09    call_vn         7e4 local7 local11 local8
    ' 2012:  b0                      rtrue
    ' 2013:  f9 26 01 f9 02 7b 09    call_vn         7e4 local1 #7b local8
    ' 201a:  b1                      rfalse
#End Region

    <Fact>
    Sub CZech_1B34()
        Dim expected =
<![CDATA[
# temps: 148

LABEL 00
    L02 <- 33
    write-word(4fa) <- 3d
    L01 <- 47
    L03 <- 00
    L06 <- 02
    L04 <- 03
    L05 <- 15
    L09 <- 03e7
    push-SP: 29
    push-SP: 2a
    push-SP: 2b
    push-SP: 2c
    push-SP: 2d
    temp00 <- L00
    write-word(6ce) <- temp00
    temp01 <- read-word(6ce)
    if (temp01 = 00) is false then
        jump-to: LABEL 02
LABEL 01
    print: "load"
    temp02 <- peek-SP
    L01 <- temp02
    L09 <- 2d
    L0a <- 2d
    L0b <- 2c
    L08 <- 0b6d
    jump-to: LABEL 187
LABEL 02
    temp03 <- read-word(6ce)
    if (temp03 = 01) is false then
        jump-to: LABEL 09
LABEL 03
    temp04 <- L03
    if (temp04 = 0) is false then
        jump-to: LABEL 05
LABEL 04
    temp05 <- peek-SP
    jump-to: LABEL 08
LABEL 05
    if (temp04 < 10) is false then
        jump-to: LABEL 07
LABEL 06
    temp05 <- L(temp04 - 1)
    jump-to: LABEL 08
LABEL 07
    temp05 <- read-word(((temp04 - 10) * 2) + 4f0)
LABEL 08
    L01 <- temp05
    L09 <- 2d
    L0a <- 2d
    L0b <- 2c
    L08 <- 0b71
    jump-to: LABEL 187
LABEL 09
    temp06 <- read-word(6ce)
    if (temp06 = 02) is false then
        jump-to: LABEL 10
LABEL 0a
    temp07 <- L04
    if (temp07 = 0) is false then
        jump-to: LABEL 0c
LABEL 0b
    temp08 <- peek-SP
    jump-to: LABEL 0f
LABEL 0c
    if (temp07 < 10) is false then
        jump-to: LABEL 0e
LABEL 0d
    temp08 <- L(temp07 - 1)
    jump-to: LABEL 0f
LABEL 0e
    temp08 <- read-word(((temp07 - 10) * 2) + 4f0)
LABEL 0f
    L01 <- temp08
    L09 <- 33
    L0a <- 2d
    L0b <- 2c
    L08 <- 0b77
    jump-to: LABEL 187
LABEL 10
    temp09 <- read-word(6ce)
    if (temp09 = 03) is false then
        jump-to: LABEL 17
LABEL 11
    temp0a <- L03
    if (temp0a = 0) is false then
        jump-to: LABEL 13
LABEL 12
    temp0b <- peek-SP
    jump-to: LABEL 16
LABEL 13
    if (temp0a < 10) is false then
        jump-to: LABEL 15
LABEL 14
    temp0b <- L(temp0a - 1)
    jump-to: LABEL 16
LABEL 15
    temp0b <- read-word(((temp0a - 10) * 2) + 4f0)
LABEL 16
    L01 <- temp0b
    L09 <- 2d
    L0a <- 2d
    L0b <- 2c
    L08 <- 0b7e
    jump-to: LABEL 187
LABEL 17
    temp0c <- read-word(6ce)
    if (temp0c = 04) is false then
        jump-to: LABEL 19
LABEL 18
    temp0d <- peek-SP
    push-SP: temp0d
    L0a <- 2d
    L0b <- 2d
    L08 <- 0b85
    jump-to: LABEL 187
LABEL 19
    temp0e <- read-word(6ce)
    if (temp0e = 05) is false then
        jump-to: LABEL 20
LABEL 1a
    temp0f <- L04
    if (temp0f = 0) is false then
        jump-to: LABEL 1c
LABEL 1b
    temp10 <- peek-SP
    jump-to: LABEL 1f
LABEL 1c
    if (temp0f < 10) is false then
        jump-to: LABEL 1e
LABEL 1d
    temp10 <- L(temp0f - 1)
    jump-to: LABEL 1f
LABEL 1e
    temp10 <- read-word(((temp0f - 10) * 2) + 4f0)
LABEL 1f
    push-SP: temp10
    L0a <- 33
    L0b <- 2d
    L08 <- 0b88
    jump-to: LABEL 187
LABEL 20
    temp11 <- read-word(6ce)
    if (temp11 = 06) is false then
        jump-to: LABEL 27
LABEL 21
    temp12 <- L03
    if (temp12 = 0) is false then
        jump-to: LABEL 23
LABEL 22
    temp13 <- peek-SP
    jump-to: LABEL 26
LABEL 23
    if (temp12 < 10) is false then
        jump-to: LABEL 25
LABEL 24
    temp13 <- L(temp12 - 1)
    jump-to: LABEL 26
LABEL 25
    temp13 <- read-word(((temp12 - 10) * 2) + 4f0)
LABEL 26
    push-SP: temp13
    L0a <- 2d
    L0b <- 2d
    L08 <- 0b8f
    jump-to: LABEL 187
LABEL 27
    temp14 <- read-word(6ce)
    if (temp14 = 0a) is false then
        jump-to: LABEL 29
LABEL 28
    print: "store"
    update-SP: 53
    L0a <- 53
    L0b <- 2c
    L08 <- 0b96
    jump-to: LABEL 187
LABEL 29
    temp15 <- read-word(6ce)
    if (temp15 = 0b) is false then
        jump-to: LABEL 34
LABEL 2a
    temp16 <- L03
    if (temp16 = 0) is false then
        jump-to: LABEL 2c
LABEL 2b
    jump-to: LABEL 2e
LABEL 2c
    if (temp16 < 10) is false then
        jump-to: LABEL 2e
LABEL 2d
    jump-to: LABEL 2e
LABEL 2e
    if (temp16 = 0) is false then
        jump-to: LABEL 30
LABEL 2f
    update-SP: 53
    jump-to: LABEL 33
LABEL 30
    if (temp16 < 10) is false then
        jump-to: LABEL 32
LABEL 31
    L(temp16 - 1) <- 53
    jump-to: LABEL 33
LABEL 32
    write-word(((temp16 - 10) * 2) + 4f0) <- 53
LABEL 33
    L0a <- 53
    L0b <- 2c
    L08 <- 0b99
    jump-to: LABEL 187
LABEL 34
    temp17 <- read-word(6ce)
    if (temp17 = 0c) is false then
        jump-to: LABEL 3f
LABEL 35
    temp18 <- L03
    if (temp18 = 0) is false then
        jump-to: LABEL 37
LABEL 36
    jump-to: LABEL 39
LABEL 37
    if (temp18 < 10) is false then
        jump-to: LABEL 39
LABEL 38
    jump-to: LABEL 39
LABEL 39
    if (temp18 = 0) is false then
        jump-to: LABEL 3b
LABEL 3a
    update-SP: 53
    jump-to: LABEL 3e
LABEL 3b
    if (temp18 < 10) is false then
        jump-to: LABEL 3d
LABEL 3c
    L(temp18 - 1) <- 53
    jump-to: LABEL 3e
LABEL 3d
    write-word(((temp18 - 10) * 2) + 4f0) <- 53
LABEL 3e
    L0a <- 53
    L0b <- 2c
    L08 <- 0b9e
    jump-to: LABEL 187
LABEL 3f
    temp19 <- read-word(6ce)
    if (temp19 = 0d) is false then
        jump-to: LABEL 4a
LABEL 40
    temp1a <- L06
    if (temp1a = 0) is false then
        jump-to: LABEL 42
LABEL 41
    jump-to: LABEL 44
LABEL 42
    if (temp1a < 10) is false then
        jump-to: LABEL 44
LABEL 43
    jump-to: LABEL 44
LABEL 44
    if (temp1a = 0) is false then
        jump-to: LABEL 46
LABEL 45
    update-SP: 53
    jump-to: LABEL 49
LABEL 46
    if (temp1a < 10) is false then
        jump-to: LABEL 48
LABEL 47
    L(temp1a - 1) <- 53
    jump-to: LABEL 49
LABEL 48
    write-word(((temp1a - 10) * 2) + 4f0) <- 53
LABEL 49
    L09 <- 53
    L0a <- 2d
    L0b <- 2c
    L08 <- 0ba4
    jump-to: LABEL 187
LABEL 4a
    temp1b <- read-word(6ce)
    if (temp1b = 0e) is false then
        jump-to: LABEL 55
LABEL 4b
    temp1c <- L06
    if (temp1c = 0) is false then
        jump-to: LABEL 4d
LABEL 4c
    jump-to: LABEL 4f
LABEL 4d
    if (temp1c < 10) is false then
        jump-to: LABEL 4f
LABEL 4e
    jump-to: LABEL 4f
LABEL 4f
    if (temp1c = 0) is false then
        jump-to: LABEL 51
LABEL 50
    update-SP: 53
    jump-to: LABEL 54
LABEL 51
    if (temp1c < 10) is false then
        jump-to: LABEL 53
LABEL 52
    L(temp1c - 1) <- 53
    jump-to: LABEL 54
LABEL 53
    write-word(((temp1c - 10) * 2) + 4f0) <- 53
LABEL 54
    L09 <- 53
    L0a <- 2d
    L0b <- 2c
    L08 <- 0ba9
    jump-to: LABEL 187
LABEL 55
    temp1d <- read-word(6ce)
    if (temp1d = 0f) is false then
        jump-to: LABEL 57
LABEL 56
    temp1e <- pop-SP
    L01 <- temp1e
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0baf
    jump-to: LABEL 187
LABEL 57
    temp1f <- read-word(6ce)
    if (temp1f = 10) is false then
        jump-to: LABEL 59
LABEL 58
    temp20 <- pop-SP
    update-SP: temp20
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bb2
    jump-to: LABEL 187
LABEL 59
    temp21 <- read-word(6ce)
    if (temp21 = 11) is false then
        jump-to: LABEL 64
LABEL 5a
    temp22 <- L03
    temp23 <- pop-SP
    if (temp22 = 0) is false then
        jump-to: LABEL 5c
LABEL 5b
    jump-to: LABEL 5e
LABEL 5c
    if (temp22 < 10) is false then
        jump-to: LABEL 5e
LABEL 5d
    jump-to: LABEL 5e
LABEL 5e
    if (temp22 = 0) is false then
        jump-to: LABEL 60
LABEL 5f
    update-SP: temp23
    jump-to: LABEL 63
LABEL 60
    if (temp22 < 10) is false then
        jump-to: LABEL 62
LABEL 61
    L(temp22 - 1) <- temp23
    jump-to: LABEL 63
LABEL 62
    write-word(((temp22 - 10) * 2) + 4f0) <- temp23
LABEL 63
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bb4
    jump-to: LABEL 187
LABEL 64
    temp24 <- read-word(6ce)
    if (temp24 = 12) is false then
        jump-to: LABEL 6f
LABEL 65
    temp25 <- L06
    temp26 <- pop-SP
    if (temp25 = 0) is false then
        jump-to: LABEL 67
LABEL 66
    jump-to: LABEL 69
LABEL 67
    if (temp25 < 10) is false then
        jump-to: LABEL 69
LABEL 68
    jump-to: LABEL 69
LABEL 69
    if (temp25 = 0) is false then
        jump-to: LABEL 6b
LABEL 6a
    update-SP: temp26
    jump-to: LABEL 6e
LABEL 6b
    if (temp25 < 10) is false then
        jump-to: LABEL 6d
LABEL 6c
    L(temp25 - 1) <- temp26
    jump-to: LABEL 6e
LABEL 6d
    write-word(((temp25 - 10) * 2) + 4f0) <- temp26
LABEL 6e
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bba
    jump-to: LABEL 187
LABEL 6f
    temp27 <- read-word(6ce)
    if (temp27 = 13) is false then
        jump-to: LABEL 7a
LABEL 70
    temp28 <- L06
    temp29 <- pop-SP
    if (temp28 = 0) is false then
        jump-to: LABEL 72
LABEL 71
    jump-to: LABEL 74
LABEL 72
    if (temp28 < 10) is false then
        jump-to: LABEL 74
LABEL 73
    jump-to: LABEL 74
LABEL 74
    if (temp28 = 0) is false then
        jump-to: LABEL 76
LABEL 75
    update-SP: temp29
    jump-to: LABEL 79
LABEL 76
    if (temp28 < 10) is false then
        jump-to: LABEL 78
LABEL 77
    L(temp28 - 1) <- temp29
    jump-to: LABEL 79
LABEL 78
    write-word(((temp28 - 10) * 2) + 4f0) <- temp29
LABEL 79
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bbf
    jump-to: LABEL 187
LABEL 7a
    temp2a <- read-word(6ce)
    if (temp2a = 14) is false then
        jump-to: LABEL 7c
LABEL 7b
    print: "\n    pull"
    temp2b <- pop-SP
    L01 <- temp2b
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bc5
    jump-to: LABEL 187
LABEL 7c
    temp2c <- read-word(6ce)
    if (temp2c = 15) is false then
        jump-to: LABEL 87
LABEL 7d
    temp2d <- L06
    temp2e <- pop-SP
    if (temp2d = 0) is false then
        jump-to: LABEL 7f
LABEL 7e
    jump-to: LABEL 81
LABEL 7f
    if (temp2d < 10) is false then
        jump-to: LABEL 81
LABEL 80
    jump-to: LABEL 81
LABEL 81
    if (temp2d = 0) is false then
        jump-to: LABEL 83
LABEL 82
    update-SP: temp2e
    jump-to: LABEL 86
LABEL 83
    if (temp2d < 10) is false then
        jump-to: LABEL 85
LABEL 84
    L(temp2d - 1) <- temp2e
    jump-to: LABEL 86
LABEL 85
    write-word(((temp2d - 10) * 2) + 4f0) <- temp2e
LABEL 86
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bc7
    jump-to: LABEL 187
LABEL 87
    temp2f <- read-word(6ce)
    if (temp2f = 16) is false then
        jump-to: LABEL 92
LABEL 88
    temp30 <- L06
    temp31 <- pop-SP
    if (temp30 = 0) is false then
        jump-to: LABEL 8a
LABEL 89
    jump-to: LABEL 8c
LABEL 8a
    if (temp30 < 10) is false then
        jump-to: LABEL 8c
LABEL 8b
    jump-to: LABEL 8c
LABEL 8c
    if (temp30 = 0) is false then
        jump-to: LABEL 8e
LABEL 8d
    update-SP: temp31
    jump-to: LABEL 91
LABEL 8e
    if (temp30 < 10) is false then
        jump-to: LABEL 90
LABEL 8f
    L(temp30 - 1) <- temp31
    jump-to: LABEL 91
LABEL 90
    write-word(((temp30 - 10) * 2) + 4f0) <- temp31
LABEL 91
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bcb
    jump-to: LABEL 187
LABEL 92
    temp32 <- read-word(6ce)
    if (temp32 = 17) is false then
        jump-to: LABEL 94
LABEL 93
    temp33 <- pop-SP
    update-SP: temp33
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bd0
    jump-to: LABEL 187
LABEL 94
    temp34 <- read-word(6ce)
    if (temp34 = 18) is false then
        jump-to: LABEL 9f
LABEL 95
    temp35 <- L03
    temp36 <- pop-SP
    if (temp35 = 0) is false then
        jump-to: LABEL 97
LABEL 96
    jump-to: LABEL 99
LABEL 97
    if (temp35 < 10) is false then
        jump-to: LABEL 99
LABEL 98
    jump-to: LABEL 99
LABEL 99
    if (temp35 = 0) is false then
        jump-to: LABEL 9b
LABEL 9a
    update-SP: temp36
    jump-to: LABEL 9e
LABEL 9b
    if (temp35 < 10) is false then
        jump-to: LABEL 9d
LABEL 9c
    L(temp35 - 1) <- temp36
    jump-to: LABEL 9e
LABEL 9d
    write-word(((temp35 - 10) * 2) + 4f0) <- temp36
LABEL 9e
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bd2
    jump-to: LABEL 187
LABEL 9f
    temp37 <- read-word(6ce)
    if (temp37 = 19) is false then
        jump-to: LABEL aa
LABEL a0
    temp38 <- L03
    temp39 <- pop-SP
    if (temp38 = 0) is false then
        jump-to: LABEL a2
LABEL a1
    jump-to: LABEL a4
LABEL a2
    if (temp38 < 10) is false then
        jump-to: LABEL a4
LABEL a3
    jump-to: LABEL a4
LABEL a4
    if (temp38 = 0) is false then
        jump-to: LABEL a6
LABEL a5
    update-SP: temp39
    jump-to: LABEL a9
LABEL a6
    if (temp38 < 10) is false then
        jump-to: LABEL a8
LABEL a7
    L(temp38 - 1) <- temp39
    jump-to: LABEL a9
LABEL a8
    write-word(((temp38 - 10) * 2) + 4f0) <- temp39
LABEL a9
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bd7
    jump-to: LABEL 187
LABEL aa
    temp3a <- read-word(6ce)
    if (temp3a = 1e) is false then
        jump-to: LABEL ac
LABEL ab
    print: "inc"
    temp3b <- L01
    L01 <- (int16(temp3b) + int16(1))
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bdb
    jump-to: LABEL 187
LABEL ac
    temp3c <- read-word(6ce)
    if (temp3c = 1f) is false then
        jump-to: LABEL b8
LABEL ad
    temp3d <- L06
    if (temp3d = 0) is false then
        jump-to: LABEL af
LABEL ae
    temp3e <- peek-SP
    jump-to: LABEL b2
LABEL af
    if (temp3d < 10) is false then
        jump-to: LABEL b1
LABEL b0
    temp3e <- L(temp3d - 1)
    jump-to: LABEL b2
LABEL b1
    temp3e <- read-word(((temp3d - 10) * 2) + 4f0)
LABEL b2
    if (temp3d = 0) is false then
        jump-to: LABEL b4
LABEL b3
    update-SP: (int16(temp3e) + int16(1))
    jump-to: LABEL b7
LABEL b4
    if (temp3d < 10) is false then
        jump-to: LABEL b6
LABEL b5
    L(temp3d - 1) <- (int16(temp3e) + int16(1))
    jump-to: LABEL b7
LABEL b6
    write-word(((temp3d - 10) * 2) + 4f0) <- (int16(temp3e) + int16(1))
LABEL b7
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bdf
    jump-to: LABEL 187
LABEL b8
    temp3f <- read-word(6ce)
    if (temp3f = 20) is false then
        jump-to: LABEL c4
LABEL b9
    temp40 <- L06
    if (temp40 = 0) is false then
        jump-to: LABEL bb
LABEL ba
    temp41 <- peek-SP
    jump-to: LABEL be
LABEL bb
    if (temp40 < 10) is false then
        jump-to: LABEL bd
LABEL bc
    temp41 <- L(temp40 - 1)
    jump-to: LABEL be
LABEL bd
    temp41 <- read-word(((temp40 - 10) * 2) + 4f0)
LABEL be
    if (temp40 = 0) is false then
        jump-to: LABEL c0
LABEL bf
    update-SP: (int16(temp41) + int16(1))
    jump-to: LABEL c3
LABEL c0
    if (temp40 < 10) is false then
        jump-to: LABEL c2
LABEL c1
    L(temp40 - 1) <- (int16(temp41) + int16(1))
    jump-to: LABEL c3
LABEL c2
    write-word(((temp40 - 10) * 2) + 4f0) <- (int16(temp41) + int16(1))
LABEL c3
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    L08 <- 0be3
    jump-to: LABEL 187
LABEL c4
    temp42 <- read-word(6ce)
    if (temp42 = 21) is false then
        jump-to: LABEL c6
LABEL c5
    temp43 <- peek-SP
    update-SP: (int16(temp43) + int16(1))
    L0a <- 2e
    L0b <- 2c
    L08 <- 0be8
    jump-to: LABEL 187
LABEL c6
    temp44 <- read-word(6ce)
    if (temp44 = 22) is false then
        jump-to: LABEL d2
LABEL c7
    temp45 <- L03
    if (temp45 = 0) is false then
        jump-to: LABEL c9
LABEL c8
    temp46 <- peek-SP
    jump-to: LABEL cc
LABEL c9
    if (temp45 < 10) is false then
        jump-to: LABEL cb
LABEL ca
    temp46 <- L(temp45 - 1)
    jump-to: LABEL cc
LABEL cb
    temp46 <- read-word(((temp45 - 10) * 2) + 4f0)
LABEL cc
    if (temp45 = 0) is false then
        jump-to: LABEL ce
LABEL cd
    update-SP: (int16(temp46) + int16(1))
    jump-to: LABEL d1
LABEL ce
    if (temp45 < 10) is false then
        jump-to: LABEL d0
LABEL cf
    L(temp45 - 1) <- (int16(temp46) + int16(1))
    jump-to: LABEL d1
LABEL d0
    write-word(((temp45 - 10) * 2) + 4f0) <- (int16(temp46) + int16(1))
LABEL d1
    L0a <- 2e
    L0b <- 2c
    L08 <- 0be9
    jump-to: LABEL 187
LABEL d2
    temp47 <- read-word(6ce)
    if (temp47 = 23) is false then
        jump-to: LABEL de
LABEL d3
    temp48 <- L03
    if (temp48 = 0) is false then
        jump-to: LABEL d5
LABEL d4
    temp49 <- peek-SP
    jump-to: LABEL d8
LABEL d5
    if (temp48 < 10) is false then
        jump-to: LABEL d7
LABEL d6
    temp49 <- L(temp48 - 1)
    jump-to: LABEL d8
LABEL d7
    temp49 <- read-word(((temp48 - 10) * 2) + 4f0)
LABEL d8
    if (temp48 = 0) is false then
        jump-to: LABEL da
LABEL d9
    update-SP: (int16(temp49) + int16(1))
    jump-to: LABEL dd
LABEL da
    if (temp48 < 10) is false then
        jump-to: LABEL dc
LABEL db
    L(temp48 - 1) <- (int16(temp49) + int16(1))
    jump-to: LABEL dd
LABEL dc
    write-word(((temp48 - 10) * 2) + 4f0) <- (int16(temp49) + int16(1))
LABEL dd
    L0a <- 2e
    L0b <- 2c
    L08 <- 0bed
    jump-to: LABEL 187
LABEL de
    temp4a <- read-word(6ce)
    if (temp4a = 28) is false then
        jump-to: LABEL e0
LABEL df
    print: "dec"
    temp4b <- L01
    L01 <- (int16(temp4b) - int16(1))
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bf2
    jump-to: LABEL 187
LABEL e0
    temp4c <- read-word(6ce)
    if (temp4c = 29) is false then
        jump-to: LABEL ec
LABEL e1
    temp4d <- L06
    if (temp4d = 0) is false then
        jump-to: LABEL e3
LABEL e2
    temp4e <- peek-SP
    jump-to: LABEL e6
LABEL e3
    if (temp4d < 10) is false then
        jump-to: LABEL e5
LABEL e4
    temp4e <- L(temp4d - 1)
    jump-to: LABEL e6
LABEL e5
    temp4e <- read-word(((temp4d - 10) * 2) + 4f0)
LABEL e6
    if (temp4d = 0) is false then
        jump-to: LABEL e8
LABEL e7
    update-SP: (int16(temp4e) - int16(1))
    jump-to: LABEL eb
LABEL e8
    if (temp4d < 10) is false then
        jump-to: LABEL ea
LABEL e9
    L(temp4d - 1) <- (int16(temp4e) - int16(1))
    jump-to: LABEL eb
LABEL ea
    write-word(((temp4d - 10) * 2) + 4f0) <- (int16(temp4e) - int16(1))
LABEL eb
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bf6
    jump-to: LABEL 187
LABEL ec
    temp4f <- read-word(6ce)
    if (temp4f = 2a) is false then
        jump-to: LABEL f8
LABEL ed
    temp50 <- L06
    if (temp50 = 0) is false then
        jump-to: LABEL ef
LABEL ee
    temp51 <- peek-SP
    jump-to: LABEL f2
LABEL ef
    if (temp50 < 10) is false then
        jump-to: LABEL f1
LABEL f0
    temp51 <- L(temp50 - 1)
    jump-to: LABEL f2
LABEL f1
    temp51 <- read-word(((temp50 - 10) * 2) + 4f0)
LABEL f2
    if (temp50 = 0) is false then
        jump-to: LABEL f4
LABEL f3
    update-SP: (int16(temp51) - int16(1))
    jump-to: LABEL f7
LABEL f4
    if (temp50 < 10) is false then
        jump-to: LABEL f6
LABEL f5
    L(temp50 - 1) <- (int16(temp51) - int16(1))
    jump-to: LABEL f7
LABEL f6
    write-word(((temp50 - 10) * 2) + 4f0) <- (int16(temp51) - int16(1))
LABEL f7
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bfa
    jump-to: LABEL 187
LABEL f8
    temp52 <- read-word(6ce)
    if (temp52 = 2b) is false then
        jump-to: LABEL fa
LABEL f9
    temp53 <- peek-SP
    update-SP: (int16(temp53) - int16(1))
    L0a <- 2c
    L0b <- 2c
    L08 <- 0bff
    jump-to: LABEL 187
LABEL fa
    temp54 <- read-word(6ce)
    if (temp54 = 2c) is false then
        jump-to: LABEL 106
LABEL fb
    temp55 <- L03
    if (temp55 = 0) is false then
        jump-to: LABEL fd
LABEL fc
    temp56 <- peek-SP
    jump-to: LABEL 100
LABEL fd
    if (temp55 < 10) is false then
        jump-to: LABEL ff
LABEL fe
    temp56 <- L(temp55 - 1)
    jump-to: LABEL 100
LABEL ff
    temp56 <- read-word(((temp55 - 10) * 2) + 4f0)
LABEL 100
    if (temp55 = 0) is false then
        jump-to: LABEL 102
LABEL 101
    update-SP: (int16(temp56) - int16(1))
    jump-to: LABEL 105
LABEL 102
    if (temp55 < 10) is false then
        jump-to: LABEL 104
LABEL 103
    L(temp55 - 1) <- (int16(temp56) - int16(1))
    jump-to: LABEL 105
LABEL 104
    write-word(((temp55 - 10) * 2) + 4f0) <- (int16(temp56) - int16(1))
LABEL 105
    L0a <- 2c
    L0b <- 2c
    L08 <- 0c00
    jump-to: LABEL 187
LABEL 106
    temp57 <- read-word(6ce)
    if (temp57 = 2d) is false then
        jump-to: LABEL 112
LABEL 107
    temp58 <- L03
    if (temp58 = 0) is false then
        jump-to: LABEL 109
LABEL 108
    temp59 <- peek-SP
    jump-to: LABEL 10c
LABEL 109
    if (temp58 < 10) is false then
        jump-to: LABEL 10b
LABEL 10a
    temp59 <- L(temp58 - 1)
    jump-to: LABEL 10c
LABEL 10b
    temp59 <- read-word(((temp58 - 10) * 2) + 4f0)
LABEL 10c
    if (temp58 = 0) is false then
        jump-to: LABEL 10e
LABEL 10d
    update-SP: (int16(temp59) - int16(1))
    jump-to: LABEL 111
LABEL 10e
    if (temp58 < 10) is false then
        jump-to: LABEL 110
LABEL 10f
    L(temp58 - 1) <- (int16(temp59) - int16(1))
    jump-to: LABEL 111
LABEL 110
    write-word(((temp58 - 10) * 2) + 4f0) <- (int16(temp59) - int16(1))
LABEL 111
    L0a <- 2c
    L0b <- 2c
    L08 <- 0c04
    jump-to: LABEL 187
LABEL 112
    temp5a <- read-word(6ce)
    if (temp5a = 32) is false then
        jump-to: LABEL 115
LABEL 113
    print: "\n    inc_chk"
    L08 <- 0c09
    temp5b <- L01
    temp5c <- (int16(temp5b) + int16(1))
    L01 <- temp5c
    if (int16(temp5c) > int16(48)) is true then
        jump-to: LABEL 18a
LABEL 114
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 115
    temp5d <- read-word(6ce)
    if (temp5d = 33) is false then
        jump-to: LABEL 122
LABEL 116
    L08 <- 0c0e
    temp5e <- L06
    if (temp5e = 0) is false then
        jump-to: LABEL 118
LABEL 117
    temp5f <- peek-SP
    jump-to: LABEL 11b
LABEL 118
    if (temp5e < 10) is false then
        jump-to: LABEL 11a
LABEL 119
    temp5f <- L(temp5e - 1)
    jump-to: LABEL 11b
LABEL 11a
    temp5f <- read-word(((temp5e - 10) * 2) + 4f0)
LABEL 11b
    temp60 <- (int16(temp5f) + int16(1))
    if (temp5e = 0) is false then
        jump-to: LABEL 11d
LABEL 11c
    update-SP: temp60
    jump-to: LABEL 120
LABEL 11d
    if (temp5e < 10) is false then
        jump-to: LABEL 11f
LABEL 11e
    L(temp5e - 1) <- temp60
    jump-to: LABEL 120
LABEL 11f
    write-word(((temp5e - 10) * 2) + 4f0) <- temp60
LABEL 120
    if (int16(temp60) > int16(48)) is true then
        jump-to: LABEL 18a
LABEL 121
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 122
    temp61 <- read-word(6ce)
    if (temp61 = 34) is false then
        jump-to: LABEL 12f
LABEL 123
    L08 <- 0c13
    temp62 <- L06
    if (temp62 = 0) is false then
        jump-to: LABEL 125
LABEL 124
    temp63 <- peek-SP
    jump-to: LABEL 128
LABEL 125
    if (temp62 < 10) is false then
        jump-to: LABEL 127
LABEL 126
    temp63 <- L(temp62 - 1)
    jump-to: LABEL 128
LABEL 127
    temp63 <- read-word(((temp62 - 10) * 2) + 4f0)
LABEL 128
    temp64 <- (int16(temp63) + int16(1))
    if (temp62 = 0) is false then
        jump-to: LABEL 12a
LABEL 129
    update-SP: temp64
    jump-to: LABEL 12d
LABEL 12a
    if (temp62 < 10) is false then
        jump-to: LABEL 12c
LABEL 12b
    L(temp62 - 1) <- temp64
    jump-to: LABEL 12d
LABEL 12c
    write-word(((temp62 - 10) * 2) + 4f0) <- temp64
LABEL 12d
    if (int16(temp64) > int16(48)) is true then
        jump-to: LABEL 18a
LABEL 12e
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 12f
    temp65 <- read-word(6ce)
    if (temp65 = 35) is false then
        jump-to: LABEL 132
LABEL 130
    L08 <- 0c19
    temp66 <- peek-SP
    temp67 <- (int16(temp66) + int16(1))
    update-SP: temp67
    if (int16(temp67) > int16(2e)) is true then
        jump-to: LABEL 18a
LABEL 131
    L0a <- 2e
    L0b <- 2c
    jump-to: LABEL 187
LABEL 132
    temp68 <- read-word(6ce)
    if (temp68 = 36) is false then
        jump-to: LABEL 13f
LABEL 133
    L08 <- 0c1b
    temp69 <- L03
    if (temp69 = 0) is false then
        jump-to: LABEL 135
LABEL 134
    temp6a <- peek-SP
    jump-to: LABEL 138
LABEL 135
    if (temp69 < 10) is false then
        jump-to: LABEL 137
LABEL 136
    temp6a <- L(temp69 - 1)
    jump-to: LABEL 138
LABEL 137
    temp6a <- read-word(((temp69 - 10) * 2) + 4f0)
LABEL 138
    temp6b <- (int16(temp6a) + int16(1))
    if (temp69 = 0) is false then
        jump-to: LABEL 13a
LABEL 139
    update-SP: temp6b
    jump-to: LABEL 13d
LABEL 13a
    if (temp69 < 10) is false then
        jump-to: LABEL 13c
LABEL 13b
    L(temp69 - 1) <- temp6b
    jump-to: LABEL 13d
LABEL 13c
    write-word(((temp69 - 10) * 2) + 4f0) <- temp6b
LABEL 13d
    if (int16(temp6b) > int16(2e)) is true then
        jump-to: LABEL 18a
LABEL 13e
    L0a <- 2e
    L0b <- 2c
    jump-to: LABEL 187
LABEL 13f
    temp6c <- read-word(6ce)
    if (temp6c = 37) is false then
        jump-to: LABEL 14c
LABEL 140
    L08 <- 0c20
    temp6d <- L03
    if (temp6d = 0) is false then
        jump-to: LABEL 142
LABEL 141
    temp6e <- peek-SP
    jump-to: LABEL 145
LABEL 142
    if (temp6d < 10) is false then
        jump-to: LABEL 144
LABEL 143
    temp6e <- L(temp6d - 1)
    jump-to: LABEL 145
LABEL 144
    temp6e <- read-word(((temp6d - 10) * 2) + 4f0)
LABEL 145
    temp6f <- (int16(temp6e) + int16(1))
    if (temp6d = 0) is false then
        jump-to: LABEL 147
LABEL 146
    update-SP: temp6f
    jump-to: LABEL 14a
LABEL 147
    if (temp6d < 10) is false then
        jump-to: LABEL 149
LABEL 148
    L(temp6d - 1) <- temp6f
    jump-to: LABEL 14a
LABEL 149
    write-word(((temp6d - 10) * 2) + 4f0) <- temp6f
LABEL 14a
    if (int16(temp6f) > int16(2e)) is true then
        jump-to: LABEL 18a
LABEL 14b
    L0a <- 2e
    L0b <- 2c
    jump-to: LABEL 187
LABEL 14c
    temp70 <- read-word(6ce)
    if (temp70 = 3c) is false then
        jump-to: LABEL 14f
LABEL 14d
    print: "dec_chk"
    L08 <- 0c26
    temp71 <- L01
    temp72 <- (int16(temp71) - int16(1))
    L01 <- temp72
    if (int16(temp72) < int16(46)) is true then
        jump-to: LABEL 18a
LABEL 14e
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 14f
    temp73 <- read-word(6ce)
    if (temp73 = 3d) is false then
        jump-to: LABEL 15c
LABEL 150
    L08 <- 0c2b
    temp74 <- L06
    if (temp74 = 0) is false then
        jump-to: LABEL 152
LABEL 151
    temp75 <- peek-SP
    jump-to: LABEL 155
LABEL 152
    if (temp74 < 10) is false then
        jump-to: LABEL 154
LABEL 153
    temp75 <- L(temp74 - 1)
    jump-to: LABEL 155
LABEL 154
    temp75 <- read-word(((temp74 - 10) * 2) + 4f0)
LABEL 155
    temp76 <- (int16(temp75) - int16(1))
    if (temp74 = 0) is false then
        jump-to: LABEL 157
LABEL 156
    update-SP: temp76
    jump-to: LABEL 15a
LABEL 157
    if (temp74 < 10) is false then
        jump-to: LABEL 159
LABEL 158
    L(temp74 - 1) <- temp76
    jump-to: LABEL 15a
LABEL 159
    write-word(((temp74 - 10) * 2) + 4f0) <- temp76
LABEL 15a
    if (int16(temp76) < int16(46)) is true then
        jump-to: LABEL 18a
LABEL 15b
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 15c
    temp77 <- read-word(6ce)
    if (temp77 = 3e) is false then
        jump-to: LABEL 169
LABEL 15d
    L08 <- 0c30
    temp78 <- L06
    if (temp78 = 0) is false then
        jump-to: LABEL 15f
LABEL 15e
    temp79 <- peek-SP
    jump-to: LABEL 162
LABEL 15f
    if (temp78 < 10) is false then
        jump-to: LABEL 161
LABEL 160
    temp79 <- L(temp78 - 1)
    jump-to: LABEL 162
LABEL 161
    temp79 <- read-word(((temp78 - 10) * 2) + 4f0)
LABEL 162
    temp7a <- (int16(temp79) - int16(1))
    if (temp78 = 0) is false then
        jump-to: LABEL 164
LABEL 163
    update-SP: temp7a
    jump-to: LABEL 167
LABEL 164
    if (temp78 < 10) is false then
        jump-to: LABEL 166
LABEL 165
    L(temp78 - 1) <- temp7a
    jump-to: LABEL 167
LABEL 166
    write-word(((temp78 - 10) * 2) + 4f0) <- temp7a
LABEL 167
    if (int16(temp7a) < int16(46)) is true then
        jump-to: LABEL 18a
LABEL 168
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 169
    temp7b <- read-word(6ce)
    if (temp7b = 3f) is false then
        jump-to: LABEL 16c
LABEL 16a
    L08 <- 0c36
    temp7c <- peek-SP
    temp7d <- (int16(temp7c) - int16(1))
    update-SP: temp7d
    if (int16(temp7d) < int16(2c)) is true then
        jump-to: LABEL 18a
LABEL 16b
    L0a <- 2c
    L0b <- 2c
    jump-to: LABEL 187
LABEL 16c
    temp7e <- read-word(6ce)
    if (temp7e = 40) is false then
        jump-to: LABEL 179
LABEL 16d
    L08 <- 0c38
    temp7f <- L03
    if (temp7f = 0) is false then
        jump-to: LABEL 16f
LABEL 16e
    temp80 <- peek-SP
    jump-to: LABEL 172
LABEL 16f
    if (temp7f < 10) is false then
        jump-to: LABEL 171
LABEL 170
    temp80 <- L(temp7f - 1)
    jump-to: LABEL 172
LABEL 171
    temp80 <- read-word(((temp7f - 10) * 2) + 4f0)
LABEL 172
    temp81 <- (int16(temp80) - int16(1))
    if (temp7f = 0) is false then
        jump-to: LABEL 174
LABEL 173
    update-SP: temp81
    jump-to: LABEL 177
LABEL 174
    if (temp7f < 10) is false then
        jump-to: LABEL 176
LABEL 175
    L(temp7f - 1) <- temp81
    jump-to: LABEL 177
LABEL 176
    write-word(((temp7f - 10) * 2) + 4f0) <- temp81
LABEL 177
    if (int16(temp81) < int16(2c)) is true then
        jump-to: LABEL 18a
LABEL 178
    L0a <- 2c
    L0b <- 2c
    jump-to: LABEL 187
LABEL 179
    temp82 <- read-word(6ce)
    if (temp82 = 41) is false then
        jump-to: LABEL 186
LABEL 17a
    L08 <- 0c3d
    temp83 <- L03
    if (temp83 = 0) is false then
        jump-to: LABEL 17c
LABEL 17b
    temp84 <- peek-SP
    jump-to: LABEL 17f
LABEL 17c
    if (temp83 < 10) is false then
        jump-to: LABEL 17e
LABEL 17d
    temp84 <- L(temp83 - 1)
    jump-to: LABEL 17f
LABEL 17e
    temp84 <- read-word(((temp83 - 10) * 2) + 4f0)
LABEL 17f
    temp85 <- (int16(temp84) - int16(1))
    if (temp83 = 0) is false then
        jump-to: LABEL 181
LABEL 180
    update-SP: temp85
    jump-to: LABEL 184
LABEL 181
    if (temp83 < 10) is false then
        jump-to: LABEL 183
LABEL 182
    L(temp83 - 1) <- temp85
    jump-to: LABEL 184
LABEL 183
    write-word(((temp83 - 10) * 2) + 4f0) <- temp85
LABEL 184
    if (int16(temp85) < int16(2c)) is true then
        jump-to: LABEL 18a
LABEL 185
    L0a <- 2c
    L0b <- 2c
    jump-to: LABEL 187
LABEL 186
    return: 0
LABEL 187
    temp86 <- L09
    if (temp86 = 03e7) is true then
        jump-to: LABEL 189
LABEL 188
    temp87 <- L01
    temp88 <- L09
    temp89 <- L08
    discard: call 7e4 (temp87, temp88, temp89)
LABEL 189
    temp8a <- pop-SP
    L07 <- temp8a
    temp8b <- L07
    temp8c <- L0a
    temp8d <- L08
    discard: call 7e4 (temp8b, temp8c, temp8d)
    temp8e <- pop-SP
    L07 <- temp8e
    temp8f <- L07
    temp90 <- L0b
    temp91 <- L08
    discard: call 7e4 (temp8f, temp90, temp91)
    return: 1
LABEL 18a
    temp92 <- L01
    temp93 <- L08
    discard: call 7e4 (temp92, 7b, temp93)
    return: 0
]]>

        TestBinder(CZech, &H1B34, expected)
    End Sub

#End Region
#Region "CZech_2448"

#Region "ZCode"
    ' 2449:  0d 11 00                store           g01 #00
    ' 244c:  0d 12 00                store           g02 #00
    ' 244f:  0d 13 00                store           g03 #00
    ' 2452:  0d 14 00                store           g04 #00
    ' 2455:  b2 ...                  print           "CZECH: the Comprehensive Z-machine Emulation CHecker, version "
    ' 2488:  8d 0b 1e                print_paddr     s001
    ' 248b:  b2 ...                  print           "^Test numbers appear in [brackets].^"
    ' 24ac:  8f 02 36                call_1n         8d8
    ' 24af:  b2 ...                  print           "^print works or you wouldn't be seeing this.^^"
    ' 24d2:  da 1f 02 3d 00          call_2n         8f4 #00
    ' 24d7:  b2 ...                  print           "^"
    ' 24da:  da 1f 02 a9 00          call_2n         aa4 #00
    ' 24df:  b2 ...                  print           "^"
    ' 24e2:  da 1f 03 34 00          call_2n         cd0 #00
    ' 24e7:  b2 ...                  print           "^"
    ' 24ea:  da 1f 03 b8 00          call_2n         ee0 #00
    ' 24ef:  b2 ...                  print           "^"
    ' 24f2:  da 1f 04 23 00          call_2n         108c #00
    ' 24f7:  b2 ...                  print           "^"
    ' 24fa:  da 1f 04 e9 00          call_2n         13a4 #00
    ' 24ff:  b2 ...                  print           "^"
    ' 2502:  da 1f 05 b2 00          call_2n         16c8 #00
    ' 2507:  b2 ...                  print           "^"
    ' 250a:  da 1f 06 bd 00          call_2n         1af4 #00
    ' 250f:  b2 ...                  print           "^"
    ' 2512:  da 1f 08 07 00          call_2n         201c #00
    ' 2517:  b2 ...                  print           "^"
    ' 251a:  da 1f 08 31 00          call_2n         20c4 #00
    ' 251f:  b2 ...                  print           "^"
    ' 2522:  da 1f 04 54 00          call_2n         1150 #00
    ' 2527:  b2 ...                  print           "^"
    ' 252a:  b2 ...                  print           "^^Performed "
    ' 2535:  e6 bf 11                print_num       g01
    ' 2538:  b2 ...                  print           " tests.^"
    ' 2541:  b2 ...                  print           "Passed: "
    ' 254a:  e6 bf 12                print_num       g02
    ' 254d:  b2 ...                  print           ", Failed: "
    ' 2558:  e6 bf 13                print_num       g03
    ' 255b:  b2 ...                  print           ", Print tests: "
    ' 2568:  e6 bf 14                print_num       g04
    ' 256b:  b2 ...                  print           "^"
    ' 256e:  74 12 13 00             add             g02 g03 -> sp
    ' 2572:  74 00 14 00             add             sp g04 -> sp
    ' 2576:  61 00 11 80 42          je              sp g01 25bb
    ' 257b:  b2 ...                  print           "^ERROR - Total number of tests should equal"
    ' 259e:  b2 ...                  print           " passed + failed + print tests!^^"
    ' 25bb:  b2 ...                  print           "Didn't crash: hooray!^"
    ' 25ce:  b2 ...                  print           "Last test: quit!^"
    ' 25dd:  ba                      quit            
#End Region

    <Fact>
    Sub CZech_2448()
        Dim expected =
<![CDATA[
# temps: 10

LABEL 00
    write-word(4f2) <- 00
    write-word(4f4) <- 00
    write-word(4f6) <- 00
    write-word(4f8) <- 00
    print: "CZECH: the Comprehensive Z-machine Emulation CHecker, version "
    print: read-text(2c78)
    print: "\nTest numbers appear in [brackets].\n"
    discard: call 8d8 ()
    print: "\nprint works or you wouldn't be seeing this.\n\n"
    discard: call 8f4 (00)
    print: "\n"
    discard: call aa4 (00)
    print: "\n"
    discard: call cd0 (00)
    print: "\n"
    discard: call ee0 (00)
    print: "\n"
    discard: call 108c (00)
    print: "\n"
    discard: call 13a4 (00)
    print: "\n"
    discard: call 16c8 (00)
    print: "\n"
    discard: call 1af4 (00)
    print: "\n"
    discard: call 201c (00)
    print: "\n"
    discard: call 20c4 (00)
    print: "\n"
    discard: call 1150 (00)
    print: "\n"
    print: "\n\nPerformed "
    temp00 <- read-word(4f2)
    print: number-to-text(int16(temp00))
    print: " tests.\n"
    print: "Passed: "
    temp01 <- read-word(4f4)
    print: number-to-text(int16(temp01))
    print: ", Failed: "
    temp02 <- read-word(4f6)
    print: number-to-text(int16(temp02))
    print: ", Print tests: "
    temp03 <- read-word(4f8)
    print: number-to-text(int16(temp03))
    print: "\n"
    temp04 <- read-word(4f4)
    temp05 <- read-word(4f6)
    temp06 <- (int16(temp04) + int16(temp05))
    temp07 <- read-word(4f8)
    temp08 <- (int16(temp06) + int16(temp07))
    temp09 <- read-word(4f2)
    if (temp08 = temp09) is true then
        jump-to: LABEL 02
LABEL 01
    print: "\nERROR - Total number of tests should equal"
    print: " passed + failed + print tests!\n\n"
LABEL 02
    print: "Didn't crash: hooray!\n"
    print: "Last test: quit!\n"
    quit
]]>

        TestBinder(CZech, &H2448, expected)
    End Sub

#End Region

End Module
