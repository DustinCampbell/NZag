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
    write-word(6ce) <- call 2448 ()
    quit
]]>

        Test(CZech, &H7DC, expected)
    End Sub

    <Fact>
    Sub CZech_7E4()
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

        Dim expected =
<![CDATA[
# temps: 7

LABEL 00
    temp00 <- L01
    temp01 <- L00
    temp02 <- (temp00 = temp01)
    if (temp02) is true then
        jump-to: LABEL 04
LABEL 01
    print: "\n\nERROR ["
    temp03 <- read-word(4f2)
    print: number-to-text(int16(temp03))
    print: "] "
    if (03 <= arg-count) is false then
        jump-to: LABEL 03
LABEL 02
    print: "("
    temp04 <- L02
    print: read-text(temp04 * 4)
    print: ")"
LABEL 03
    discard: call 8ec ()
    print: " Expected "
    temp05 <- L01
    print: number-to-text(int16(temp05))
    print: "; got "
    temp06 <- L00
    print: number-to-text(int16(temp06))
    print: "\n\n"
    jump-to: LABEL 05
LABEL 04
    discard: call 8e0 ()
LABEL 05
    return: 1
]]>

        Test(CZech, &H7E4, expected)
    End Sub

    <Fact>
    Sub CZech_8F4()
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

        Test(CZech, &H8F4, expected)
    End Sub

    <Fact>
    Sub CZech_AA4()
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

        Test(CZech, &HAA4, expected)
    End Sub

    <Fact>
    Sub CZech_1AC8()
        ' 1ac9:  73 01 02 04             get_next_prop   local0 local1 -> local3
        ' 1acd:  2d 16 01                store           g06 local0
        ' 1ad0:  2d 17 02                store           g07 local1
        ' 1ad3:  f9 28 02 21 04 03 0b 6b call_vn         884 local3 local2 s035
        ' 1adb:  b0                      rtrue

        Dim expected =
<![CDATA[
# temps: 22

LABEL 00
    temp00 <- L00
    temp01 <- L01
    temp02 <- (temp00 - 1)
    temp03 <- (temp02 * e)
    temp04 <- (temp03 + 18c)
    temp05 <- (temp04 + c)
    temp06 <- read-word(temp05)
    temp07 <- read-byte(temp06)
    temp08 <- (temp06 + 1)
    temp09 <- (temp07 * 2)
    temp0a <- (temp08 + temp09)
    temp0b <- uint16(temp0a)
    if (temp01 <> 0) is false then
        jump-to: LABEL 07
LABEL 01
    temp0c <- read-byte(temp0b)
    temp0d <- read-byte(temp0b)
    if ((temp0d & 80) <> 80) is false then
        jump-to: LABEL 03
LABEL 02
    temp0e <- (temp0d >> 6)
    jump-to: LABEL 06
LABEL 03
    temp0f <- (temp0b + 1)
    if ((read-byte(temp0f) & 3f) = 0) is false then
        jump-to: LABEL 05
LABEL 04
    temp0e <- 40
    jump-to: LABEL 06
LABEL 05
    temp0e <- (read-byte(temp0f) & 3f)
LABEL 06
    temp10 <- ((temp0b + 1) + (temp0e + 1))
    temp0b <- uint16(temp10)
    if ((temp0c & 3f) > temp01) is true then
        jump-to: LABEL 01
LABEL 07
    temp11 <- (read-byte(temp0b) & 3f)
    L03 <- temp11
    temp12 <- L00
    write-word(4fc) <- temp12
    temp13 <- L01
    write-word(4fe) <- temp13
    temp14 <- L03
    temp15 <- L02
    discard: call 884 (temp14, temp15, 0b6b)
    return: 1
]]>

        Test(CZech, &H1AC8, expected)
    End Sub

    <Fact>
    Sub CZech_1B34()
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

        Dim expected =
<![CDATA[
# temps: 388

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
    temp02 <- (temp01 = 00)
    if (temp02) is false then
        jump-to: LABEL 02
LABEL 01
    print: "load"
    temp03 <- peek-SP
    L01 <- temp03
    L09 <- 2d
    L0a <- 2d
    L0b <- 2c
    L08 <- 0b6d
    jump-to: LABEL 187
LABEL 02
    temp04 <- read-word(6ce)
    temp05 <- (temp04 = 01)
    if (temp05) is false then
        jump-to: LABEL 09
LABEL 03
    temp06 <- L03
    if (temp06 = 0) is false then
        jump-to: LABEL 05
LABEL 04
    temp07 <- peek-SP
    jump-to: LABEL 08
LABEL 05
    if (temp06 < 10) is false then
        jump-to: LABEL 07
LABEL 06
    temp08 <- (temp06 - 1)
    temp07 <- L(temp08)
    jump-to: LABEL 08
LABEL 07
    temp09 <- (temp06 - 10)
    temp0a <- (temp09 * 2)
    temp0b <- (temp0a + 4f0)
    temp07 <- read-word(temp0b)
LABEL 08
    L01 <- temp07
    L09 <- 2d
    L0a <- 2d
    L0b <- 2c
    L08 <- 0b71
    jump-to: LABEL 187
LABEL 09
    temp0c <- read-word(6ce)
    temp0d <- (temp0c = 02)
    if (temp0d) is false then
        jump-to: LABEL 10
LABEL 0a
    temp0e <- L04
    if (temp0e = 0) is false then
        jump-to: LABEL 0c
LABEL 0b
    temp0f <- peek-SP
    jump-to: LABEL 0f
LABEL 0c
    if (temp0e < 10) is false then
        jump-to: LABEL 0e
LABEL 0d
    temp10 <- (temp0e - 1)
    temp0f <- L(temp10)
    jump-to: LABEL 0f
LABEL 0e
    temp11 <- (temp0e - 10)
    temp12 <- (temp11 * 2)
    temp13 <- (temp12 + 4f0)
    temp0f <- read-word(temp13)
LABEL 0f
    L01 <- temp0f
    L09 <- 33
    L0a <- 2d
    L0b <- 2c
    L08 <- 0b77
    jump-to: LABEL 187
LABEL 10
    temp14 <- read-word(6ce)
    temp15 <- (temp14 = 03)
    if (temp15) is false then
        jump-to: LABEL 17
LABEL 11
    temp16 <- L03
    if (temp16 = 0) is false then
        jump-to: LABEL 13
LABEL 12
    temp17 <- peek-SP
    jump-to: LABEL 16
LABEL 13
    if (temp16 < 10) is false then
        jump-to: LABEL 15
LABEL 14
    temp18 <- (temp16 - 1)
    temp17 <- L(temp18)
    jump-to: LABEL 16
LABEL 15
    temp19 <- (temp16 - 10)
    temp1a <- (temp19 * 2)
    temp1b <- (temp1a + 4f0)
    temp17 <- read-word(temp1b)
LABEL 16
    L01 <- temp17
    L09 <- 2d
    L0a <- 2d
    L0b <- 2c
    L08 <- 0b7e
    jump-to: LABEL 187
LABEL 17
    temp1c <- read-word(6ce)
    temp1d <- (temp1c = 04)
    if (temp1d) is false then
        jump-to: LABEL 19
LABEL 18
    temp1e <- peek-SP
    push-SP: temp1e
    L0a <- 2d
    L0b <- 2d
    L08 <- 0b85
    jump-to: LABEL 187
LABEL 19
    temp1f <- read-word(6ce)
    temp20 <- (temp1f = 05)
    if (temp20) is false then
        jump-to: LABEL 20
LABEL 1a
    temp21 <- L04
    if (temp21 = 0) is false then
        jump-to: LABEL 1c
LABEL 1b
    temp22 <- peek-SP
    jump-to: LABEL 1f
LABEL 1c
    if (temp21 < 10) is false then
        jump-to: LABEL 1e
LABEL 1d
    temp23 <- (temp21 - 1)
    temp22 <- L(temp23)
    jump-to: LABEL 1f
LABEL 1e
    temp24 <- (temp21 - 10)
    temp25 <- (temp24 * 2)
    temp26 <- (temp25 + 4f0)
    temp22 <- read-word(temp26)
LABEL 1f
    push-SP: temp22
    L0a <- 33
    L0b <- 2d
    L08 <- 0b88
    jump-to: LABEL 187
LABEL 20
    temp27 <- read-word(6ce)
    temp28 <- (temp27 = 06)
    if (temp28) is false then
        jump-to: LABEL 27
LABEL 21
    temp29 <- L03
    if (temp29 = 0) is false then
        jump-to: LABEL 23
LABEL 22
    temp2a <- peek-SP
    jump-to: LABEL 26
LABEL 23
    if (temp29 < 10) is false then
        jump-to: LABEL 25
LABEL 24
    temp2b <- (temp29 - 1)
    temp2a <- L(temp2b)
    jump-to: LABEL 26
LABEL 25
    temp2c <- (temp29 - 10)
    temp2d <- (temp2c * 2)
    temp2e <- (temp2d + 4f0)
    temp2a <- read-word(temp2e)
LABEL 26
    push-SP: temp2a
    L0a <- 2d
    L0b <- 2d
    L08 <- 0b8f
    jump-to: LABEL 187
LABEL 27
    temp2f <- read-word(6ce)
    temp30 <- (temp2f = 0a)
    if (temp30) is false then
        jump-to: LABEL 29
LABEL 28
    print: "store"
    update-SP: 53
    L0a <- 53
    L0b <- 2c
    L08 <- 0b96
    jump-to: LABEL 187
LABEL 29
    temp31 <- read-word(6ce)
    temp32 <- (temp31 = 0b)
    if (temp32) is false then
        jump-to: LABEL 34
LABEL 2a
    temp33 <- L03
    if (temp33 = 0) is false then
        jump-to: LABEL 2c
LABEL 2b
    jump-to: LABEL 2e
LABEL 2c
    if (temp33 < 10) is false then
        jump-to: LABEL 2e
LABEL 2d
    jump-to: LABEL 2e
LABEL 2e
    if (temp33 = 0) is false then
        jump-to: LABEL 30
LABEL 2f
    update-SP: 53
    jump-to: LABEL 33
LABEL 30
    if (temp33 < 10) is false then
        jump-to: LABEL 32
LABEL 31
    temp34 <- (temp33 - 1)
    L(temp34) <- 53
    jump-to: LABEL 33
LABEL 32
    temp35 <- (temp33 - 10)
    temp36 <- (temp35 * 2)
    temp37 <- (temp36 + 4f0)
    write-word(temp37) <- 53
LABEL 33
    L0a <- 53
    L0b <- 2c
    L08 <- 0b99
    jump-to: LABEL 187
LABEL 34
    temp38 <- read-word(6ce)
    temp39 <- (temp38 = 0c)
    if (temp39) is false then
        jump-to: LABEL 3f
LABEL 35
    temp3a <- L03
    if (temp3a = 0) is false then
        jump-to: LABEL 37
LABEL 36
    jump-to: LABEL 39
LABEL 37
    if (temp3a < 10) is false then
        jump-to: LABEL 39
LABEL 38
    jump-to: LABEL 39
LABEL 39
    if (temp3a = 0) is false then
        jump-to: LABEL 3b
LABEL 3a
    update-SP: 53
    jump-to: LABEL 3e
LABEL 3b
    if (temp3a < 10) is false then
        jump-to: LABEL 3d
LABEL 3c
    temp3b <- (temp3a - 1)
    L(temp3b) <- 53
    jump-to: LABEL 3e
LABEL 3d
    temp3c <- (temp3a - 10)
    temp3d <- (temp3c * 2)
    temp3e <- (temp3d + 4f0)
    write-word(temp3e) <- 53
LABEL 3e
    L0a <- 53
    L0b <- 2c
    L08 <- 0b9e
    jump-to: LABEL 187
LABEL 3f
    temp3f <- read-word(6ce)
    temp40 <- (temp3f = 0d)
    if (temp40) is false then
        jump-to: LABEL 4a
LABEL 40
    temp41 <- L06
    if (temp41 = 0) is false then
        jump-to: LABEL 42
LABEL 41
    jump-to: LABEL 44
LABEL 42
    if (temp41 < 10) is false then
        jump-to: LABEL 44
LABEL 43
    jump-to: LABEL 44
LABEL 44
    if (temp41 = 0) is false then
        jump-to: LABEL 46
LABEL 45
    update-SP: 53
    jump-to: LABEL 49
LABEL 46
    if (temp41 < 10) is false then
        jump-to: LABEL 48
LABEL 47
    temp42 <- (temp41 - 1)
    L(temp42) <- 53
    jump-to: LABEL 49
LABEL 48
    temp43 <- (temp41 - 10)
    temp44 <- (temp43 * 2)
    temp45 <- (temp44 + 4f0)
    write-word(temp45) <- 53
LABEL 49
    L09 <- 53
    L0a <- 2d
    L0b <- 2c
    L08 <- 0ba4
    jump-to: LABEL 187
LABEL 4a
    temp46 <- read-word(6ce)
    temp47 <- (temp46 = 0e)
    if (temp47) is false then
        jump-to: LABEL 55
LABEL 4b
    temp48 <- L06
    if (temp48 = 0) is false then
        jump-to: LABEL 4d
LABEL 4c
    jump-to: LABEL 4f
LABEL 4d
    if (temp48 < 10) is false then
        jump-to: LABEL 4f
LABEL 4e
    jump-to: LABEL 4f
LABEL 4f
    if (temp48 = 0) is false then
        jump-to: LABEL 51
LABEL 50
    update-SP: 53
    jump-to: LABEL 54
LABEL 51
    if (temp48 < 10) is false then
        jump-to: LABEL 53
LABEL 52
    temp49 <- (temp48 - 1)
    L(temp49) <- 53
    jump-to: LABEL 54
LABEL 53
    temp4a <- (temp48 - 10)
    temp4b <- (temp4a * 2)
    temp4c <- (temp4b + 4f0)
    write-word(temp4c) <- 53
LABEL 54
    L09 <- 53
    L0a <- 2d
    L0b <- 2c
    L08 <- 0ba9
    jump-to: LABEL 187
LABEL 55
    temp4d <- read-word(6ce)
    temp4e <- (temp4d = 0f)
    if (temp4e) is false then
        jump-to: LABEL 57
LABEL 56
    temp4f <- pop-SP
    L01 <- temp4f
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0baf
    jump-to: LABEL 187
LABEL 57
    temp50 <- read-word(6ce)
    temp51 <- (temp50 = 10)
    if (temp51) is false then
        jump-to: LABEL 59
LABEL 58
    temp52 <- pop-SP
    update-SP: temp52
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bb2
    jump-to: LABEL 187
LABEL 59
    temp53 <- read-word(6ce)
    temp54 <- (temp53 = 11)
    if (temp54) is false then
        jump-to: LABEL 64
LABEL 5a
    temp55 <- L03
    temp56 <- pop-SP
    if (temp55 = 0) is false then
        jump-to: LABEL 5c
LABEL 5b
    jump-to: LABEL 5e
LABEL 5c
    if (temp55 < 10) is false then
        jump-to: LABEL 5e
LABEL 5d
    jump-to: LABEL 5e
LABEL 5e
    if (temp55 = 0) is false then
        jump-to: LABEL 60
LABEL 5f
    update-SP: temp56
    jump-to: LABEL 63
LABEL 60
    if (temp55 < 10) is false then
        jump-to: LABEL 62
LABEL 61
    temp57 <- (temp55 - 1)
    L(temp57) <- temp56
    jump-to: LABEL 63
LABEL 62
    temp58 <- (temp55 - 10)
    temp59 <- (temp58 * 2)
    temp5a <- (temp59 + 4f0)
    write-word(temp5a) <- temp56
LABEL 63
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bb4
    jump-to: LABEL 187
LABEL 64
    temp5b <- read-word(6ce)
    temp5c <- (temp5b = 12)
    if (temp5c) is false then
        jump-to: LABEL 6f
LABEL 65
    temp5d <- L06
    temp5e <- pop-SP
    if (temp5d = 0) is false then
        jump-to: LABEL 67
LABEL 66
    jump-to: LABEL 69
LABEL 67
    if (temp5d < 10) is false then
        jump-to: LABEL 69
LABEL 68
    jump-to: LABEL 69
LABEL 69
    if (temp5d = 0) is false then
        jump-to: LABEL 6b
LABEL 6a
    update-SP: temp5e
    jump-to: LABEL 6e
LABEL 6b
    if (temp5d < 10) is false then
        jump-to: LABEL 6d
LABEL 6c
    temp5f <- (temp5d - 1)
    L(temp5f) <- temp5e
    jump-to: LABEL 6e
LABEL 6d
    temp60 <- (temp5d - 10)
    temp61 <- (temp60 * 2)
    temp62 <- (temp61 + 4f0)
    write-word(temp62) <- temp5e
LABEL 6e
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bba
    jump-to: LABEL 187
LABEL 6f
    temp63 <- read-word(6ce)
    temp64 <- (temp63 = 13)
    if (temp64) is false then
        jump-to: LABEL 7a
LABEL 70
    temp65 <- L06
    temp66 <- pop-SP
    if (temp65 = 0) is false then
        jump-to: LABEL 72
LABEL 71
    jump-to: LABEL 74
LABEL 72
    if (temp65 < 10) is false then
        jump-to: LABEL 74
LABEL 73
    jump-to: LABEL 74
LABEL 74
    if (temp65 = 0) is false then
        jump-to: LABEL 76
LABEL 75
    update-SP: temp66
    jump-to: LABEL 79
LABEL 76
    if (temp65 < 10) is false then
        jump-to: LABEL 78
LABEL 77
    temp67 <- (temp65 - 1)
    L(temp67) <- temp66
    jump-to: LABEL 79
LABEL 78
    temp68 <- (temp65 - 10)
    temp69 <- (temp68 * 2)
    temp6a <- (temp69 + 4f0)
    write-word(temp6a) <- temp66
LABEL 79
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bbf
    jump-to: LABEL 187
LABEL 7a
    temp6b <- read-word(6ce)
    temp6c <- (temp6b = 14)
    if (temp6c) is false then
        jump-to: LABEL 7c
LABEL 7b
    print: "\n    pull"
    temp6d <- pop-SP
    L01 <- temp6d
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bc5
    jump-to: LABEL 187
LABEL 7c
    temp6e <- read-word(6ce)
    temp6f <- (temp6e = 15)
    if (temp6f) is false then
        jump-to: LABEL 87
LABEL 7d
    temp70 <- L06
    temp71 <- pop-SP
    if (temp70 = 0) is false then
        jump-to: LABEL 7f
LABEL 7e
    jump-to: LABEL 81
LABEL 7f
    if (temp70 < 10) is false then
        jump-to: LABEL 81
LABEL 80
    jump-to: LABEL 81
LABEL 81
    if (temp70 = 0) is false then
        jump-to: LABEL 83
LABEL 82
    update-SP: temp71
    jump-to: LABEL 86
LABEL 83
    if (temp70 < 10) is false then
        jump-to: LABEL 85
LABEL 84
    temp72 <- (temp70 - 1)
    L(temp72) <- temp71
    jump-to: LABEL 86
LABEL 85
    temp73 <- (temp70 - 10)
    temp74 <- (temp73 * 2)
    temp75 <- (temp74 + 4f0)
    write-word(temp75) <- temp71
LABEL 86
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bc7
    jump-to: LABEL 187
LABEL 87
    temp76 <- read-word(6ce)
    temp77 <- (temp76 = 16)
    if (temp77) is false then
        jump-to: LABEL 92
LABEL 88
    temp78 <- L06
    temp79 <- pop-SP
    if (temp78 = 0) is false then
        jump-to: LABEL 8a
LABEL 89
    jump-to: LABEL 8c
LABEL 8a
    if (temp78 < 10) is false then
        jump-to: LABEL 8c
LABEL 8b
    jump-to: LABEL 8c
LABEL 8c
    if (temp78 = 0) is false then
        jump-to: LABEL 8e
LABEL 8d
    update-SP: temp79
    jump-to: LABEL 91
LABEL 8e
    if (temp78 < 10) is false then
        jump-to: LABEL 90
LABEL 8f
    temp7a <- (temp78 - 1)
    L(temp7a) <- temp79
    jump-to: LABEL 91
LABEL 90
    temp7b <- (temp78 - 10)
    temp7c <- (temp7b * 2)
    temp7d <- (temp7c + 4f0)
    write-word(temp7d) <- temp79
LABEL 91
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bcb
    jump-to: LABEL 187
LABEL 92
    temp7e <- read-word(6ce)
    temp7f <- (temp7e = 17)
    if (temp7f) is false then
        jump-to: LABEL 94
LABEL 93
    temp80 <- pop-SP
    update-SP: temp80
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bd0
    jump-to: LABEL 187
LABEL 94
    temp81 <- read-word(6ce)
    temp82 <- (temp81 = 18)
    if (temp82) is false then
        jump-to: LABEL 9f
LABEL 95
    temp83 <- L03
    temp84 <- pop-SP
    if (temp83 = 0) is false then
        jump-to: LABEL 97
LABEL 96
    jump-to: LABEL 99
LABEL 97
    if (temp83 < 10) is false then
        jump-to: LABEL 99
LABEL 98
    jump-to: LABEL 99
LABEL 99
    if (temp83 = 0) is false then
        jump-to: LABEL 9b
LABEL 9a
    update-SP: temp84
    jump-to: LABEL 9e
LABEL 9b
    if (temp83 < 10) is false then
        jump-to: LABEL 9d
LABEL 9c
    temp85 <- (temp83 - 1)
    L(temp85) <- temp84
    jump-to: LABEL 9e
LABEL 9d
    temp86 <- (temp83 - 10)
    temp87 <- (temp86 * 2)
    temp88 <- (temp87 + 4f0)
    write-word(temp88) <- temp84
LABEL 9e
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bd2
    jump-to: LABEL 187
LABEL 9f
    temp89 <- read-word(6ce)
    temp8a <- (temp89 = 19)
    if (temp8a) is false then
        jump-to: LABEL aa
LABEL a0
    temp8b <- L03
    temp8c <- pop-SP
    if (temp8b = 0) is false then
        jump-to: LABEL a2
LABEL a1
    jump-to: LABEL a4
LABEL a2
    if (temp8b < 10) is false then
        jump-to: LABEL a4
LABEL a3
    jump-to: LABEL a4
LABEL a4
    if (temp8b = 0) is false then
        jump-to: LABEL a6
LABEL a5
    update-SP: temp8c
    jump-to: LABEL a9
LABEL a6
    if (temp8b < 10) is false then
        jump-to: LABEL a8
LABEL a7
    temp8d <- (temp8b - 1)
    L(temp8d) <- temp8c
    jump-to: LABEL a9
LABEL a8
    temp8e <- (temp8b - 10)
    temp8f <- (temp8e * 2)
    temp90 <- (temp8f + 4f0)
    write-word(temp90) <- temp8c
LABEL a9
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bd7
    jump-to: LABEL 187
LABEL aa
    temp91 <- read-word(6ce)
    temp92 <- (temp91 = 1e)
    if (temp92) is false then
        jump-to: LABEL ac
LABEL ab
    print: "inc"
    temp93 <- L01
    L01 <- (int16(temp93) + int16(1))
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bdb
    jump-to: LABEL 187
LABEL ac
    temp94 <- read-word(6ce)
    temp95 <- (temp94 = 1f)
    if (temp95) is false then
        jump-to: LABEL b8
LABEL ad
    temp96 <- L06
    if (temp96 = 0) is false then
        jump-to: LABEL af
LABEL ae
    temp97 <- peek-SP
    jump-to: LABEL b2
LABEL af
    if (temp96 < 10) is false then
        jump-to: LABEL b1
LABEL b0
    temp98 <- (temp96 - 1)
    temp97 <- L(temp98)
    jump-to: LABEL b2
LABEL b1
    temp99 <- (temp96 - 10)
    temp9a <- (temp99 * 2)
    temp9b <- (temp9a + 4f0)
    temp97 <- read-word(temp9b)
LABEL b2
    if (temp96 = 0) is false then
        jump-to: LABEL b4
LABEL b3
    update-SP: (int16(temp97) + int16(1))
    jump-to: LABEL b7
LABEL b4
    if (temp96 < 10) is false then
        jump-to: LABEL b6
LABEL b5
    temp9c <- (temp96 - 1)
    L(temp9c) <- (int16(temp97) + int16(1))
    jump-to: LABEL b7
LABEL b6
    temp9d <- (temp96 - 10)
    temp9e <- (temp9d * 2)
    temp9f <- (temp9e + 4f0)
    write-word(temp9f) <- (int16(temp97) + int16(1))
LABEL b7
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bdf
    jump-to: LABEL 187
LABEL b8
    tempa0 <- read-word(6ce)
    tempa1 <- (tempa0 = 20)
    if (tempa1) is false then
        jump-to: LABEL c4
LABEL b9
    tempa2 <- L06
    if (tempa2 = 0) is false then
        jump-to: LABEL bb
LABEL ba
    tempa3 <- peek-SP
    jump-to: LABEL be
LABEL bb
    if (tempa2 < 10) is false then
        jump-to: LABEL bd
LABEL bc
    tempa4 <- (tempa2 - 1)
    tempa3 <- L(tempa4)
    jump-to: LABEL be
LABEL bd
    tempa5 <- (tempa2 - 10)
    tempa6 <- (tempa5 * 2)
    tempa7 <- (tempa6 + 4f0)
    tempa3 <- read-word(tempa7)
LABEL be
    if (tempa2 = 0) is false then
        jump-to: LABEL c0
LABEL bf
    update-SP: (int16(tempa3) + int16(1))
    jump-to: LABEL c3
LABEL c0
    if (tempa2 < 10) is false then
        jump-to: LABEL c2
LABEL c1
    tempa8 <- (tempa2 - 1)
    L(tempa8) <- (int16(tempa3) + int16(1))
    jump-to: LABEL c3
LABEL c2
    tempa9 <- (tempa2 - 10)
    tempaa <- (tempa9 * 2)
    tempab <- (tempaa + 4f0)
    write-word(tempab) <- (int16(tempa3) + int16(1))
LABEL c3
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    L08 <- 0be3
    jump-to: LABEL 187
LABEL c4
    tempac <- read-word(6ce)
    tempad <- (tempac = 21)
    if (tempad) is false then
        jump-to: LABEL c6
LABEL c5
    tempae <- peek-SP
    update-SP: (int16(tempae) + int16(1))
    L0a <- 2e
    L0b <- 2c
    L08 <- 0be8
    jump-to: LABEL 187
LABEL c6
    tempaf <- read-word(6ce)
    tempb0 <- (tempaf = 22)
    if (tempb0) is false then
        jump-to: LABEL d2
LABEL c7
    tempb1 <- L03
    if (tempb1 = 0) is false then
        jump-to: LABEL c9
LABEL c8
    tempb2 <- peek-SP
    jump-to: LABEL cc
LABEL c9
    if (tempb1 < 10) is false then
        jump-to: LABEL cb
LABEL ca
    tempb3 <- (tempb1 - 1)
    tempb2 <- L(tempb3)
    jump-to: LABEL cc
LABEL cb
    tempb4 <- (tempb1 - 10)
    tempb5 <- (tempb4 * 2)
    tempb6 <- (tempb5 + 4f0)
    tempb2 <- read-word(tempb6)
LABEL cc
    if (tempb1 = 0) is false then
        jump-to: LABEL ce
LABEL cd
    update-SP: (int16(tempb2) + int16(1))
    jump-to: LABEL d1
LABEL ce
    if (tempb1 < 10) is false then
        jump-to: LABEL d0
LABEL cf
    tempb7 <- (tempb1 - 1)
    L(tempb7) <- (int16(tempb2) + int16(1))
    jump-to: LABEL d1
LABEL d0
    tempb8 <- (tempb1 - 10)
    tempb9 <- (tempb8 * 2)
    tempba <- (tempb9 + 4f0)
    write-word(tempba) <- (int16(tempb2) + int16(1))
LABEL d1
    L0a <- 2e
    L0b <- 2c
    L08 <- 0be9
    jump-to: LABEL 187
LABEL d2
    tempbb <- read-word(6ce)
    tempbc <- (tempbb = 23)
    if (tempbc) is false then
        jump-to: LABEL de
LABEL d3
    tempbd <- L03
    if (tempbd = 0) is false then
        jump-to: LABEL d5
LABEL d4
    tempbe <- peek-SP
    jump-to: LABEL d8
LABEL d5
    if (tempbd < 10) is false then
        jump-to: LABEL d7
LABEL d6
    tempbf <- (tempbd - 1)
    tempbe <- L(tempbf)
    jump-to: LABEL d8
LABEL d7
    tempc0 <- (tempbd - 10)
    tempc1 <- (tempc0 * 2)
    tempc2 <- (tempc1 + 4f0)
    tempbe <- read-word(tempc2)
LABEL d8
    if (tempbd = 0) is false then
        jump-to: LABEL da
LABEL d9
    update-SP: (int16(tempbe) + int16(1))
    jump-to: LABEL dd
LABEL da
    if (tempbd < 10) is false then
        jump-to: LABEL dc
LABEL db
    tempc3 <- (tempbd - 1)
    L(tempc3) <- (int16(tempbe) + int16(1))
    jump-to: LABEL dd
LABEL dc
    tempc4 <- (tempbd - 10)
    tempc5 <- (tempc4 * 2)
    tempc6 <- (tempc5 + 4f0)
    write-word(tempc6) <- (int16(tempbe) + int16(1))
LABEL dd
    L0a <- 2e
    L0b <- 2c
    L08 <- 0bed
    jump-to: LABEL 187
LABEL de
    tempc7 <- read-word(6ce)
    tempc8 <- (tempc7 = 28)
    if (tempc8) is false then
        jump-to: LABEL e0
LABEL df
    print: "dec"
    tempc9 <- L01
    L01 <- (int16(tempc9) - int16(1))
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bf2
    jump-to: LABEL 187
LABEL e0
    tempca <- read-word(6ce)
    tempcb <- (tempca = 29)
    if (tempcb) is false then
        jump-to: LABEL ec
LABEL e1
    tempcc <- L06
    if (tempcc = 0) is false then
        jump-to: LABEL e3
LABEL e2
    tempcd <- peek-SP
    jump-to: LABEL e6
LABEL e3
    if (tempcc < 10) is false then
        jump-to: LABEL e5
LABEL e4
    tempce <- (tempcc - 1)
    tempcd <- L(tempce)
    jump-to: LABEL e6
LABEL e5
    tempcf <- (tempcc - 10)
    tempd0 <- (tempcf * 2)
    tempd1 <- (tempd0 + 4f0)
    tempcd <- read-word(tempd1)
LABEL e6
    if (tempcc = 0) is false then
        jump-to: LABEL e8
LABEL e7
    update-SP: (int16(tempcd) - int16(1))
    jump-to: LABEL eb
LABEL e8
    if (tempcc < 10) is false then
        jump-to: LABEL ea
LABEL e9
    tempd2 <- (tempcc - 1)
    L(tempd2) <- (int16(tempcd) - int16(1))
    jump-to: LABEL eb
LABEL ea
    tempd3 <- (tempcc - 10)
    tempd4 <- (tempd3 * 2)
    tempd5 <- (tempd4 + 4f0)
    write-word(tempd5) <- (int16(tempcd) - int16(1))
LABEL eb
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bf6
    jump-to: LABEL 187
LABEL ec
    tempd6 <- read-word(6ce)
    tempd7 <- (tempd6 = 2a)
    if (tempd7) is false then
        jump-to: LABEL f8
LABEL ed
    tempd8 <- L06
    if (tempd8 = 0) is false then
        jump-to: LABEL ef
LABEL ee
    tempd9 <- peek-SP
    jump-to: LABEL f2
LABEL ef
    if (tempd8 < 10) is false then
        jump-to: LABEL f1
LABEL f0
    tempda <- (tempd8 - 1)
    tempd9 <- L(tempda)
    jump-to: LABEL f2
LABEL f1
    tempdb <- (tempd8 - 10)
    tempdc <- (tempdb * 2)
    tempdd <- (tempdc + 4f0)
    tempd9 <- read-word(tempdd)
LABEL f2
    if (tempd8 = 0) is false then
        jump-to: LABEL f4
LABEL f3
    update-SP: (int16(tempd9) - int16(1))
    jump-to: LABEL f7
LABEL f4
    if (tempd8 < 10) is false then
        jump-to: LABEL f6
LABEL f5
    tempde <- (tempd8 - 1)
    L(tempde) <- (int16(tempd9) - int16(1))
    jump-to: LABEL f7
LABEL f6
    tempdf <- (tempd8 - 10)
    tempe0 <- (tempdf * 2)
    tempe1 <- (tempe0 + 4f0)
    write-word(tempe1) <- (int16(tempd9) - int16(1))
LABEL f7
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bfa
    jump-to: LABEL 187
LABEL f8
    tempe2 <- read-word(6ce)
    tempe3 <- (tempe2 = 2b)
    if (tempe3) is false then
        jump-to: LABEL fa
LABEL f9
    tempe4 <- peek-SP
    update-SP: (int16(tempe4) - int16(1))
    L0a <- 2c
    L0b <- 2c
    L08 <- 0bff
    jump-to: LABEL 187
LABEL fa
    tempe5 <- read-word(6ce)
    tempe6 <- (tempe5 = 2c)
    if (tempe6) is false then
        jump-to: LABEL 106
LABEL fb
    tempe7 <- L03
    if (tempe7 = 0) is false then
        jump-to: LABEL fd
LABEL fc
    tempe8 <- peek-SP
    jump-to: LABEL 100
LABEL fd
    if (tempe7 < 10) is false then
        jump-to: LABEL ff
LABEL fe
    tempe9 <- (tempe7 - 1)
    tempe8 <- L(tempe9)
    jump-to: LABEL 100
LABEL ff
    tempea <- (tempe7 - 10)
    tempeb <- (tempea * 2)
    tempec <- (tempeb + 4f0)
    tempe8 <- read-word(tempec)
LABEL 100
    if (tempe7 = 0) is false then
        jump-to: LABEL 102
LABEL 101
    update-SP: (int16(tempe8) - int16(1))
    jump-to: LABEL 105
LABEL 102
    if (tempe7 < 10) is false then
        jump-to: LABEL 104
LABEL 103
    temped <- (tempe7 - 1)
    L(temped) <- (int16(tempe8) - int16(1))
    jump-to: LABEL 105
LABEL 104
    tempee <- (tempe7 - 10)
    tempef <- (tempee * 2)
    tempf0 <- (tempef + 4f0)
    write-word(tempf0) <- (int16(tempe8) - int16(1))
LABEL 105
    L0a <- 2c
    L0b <- 2c
    L08 <- 0c00
    jump-to: LABEL 187
LABEL 106
    tempf1 <- read-word(6ce)
    tempf2 <- (tempf1 = 2d)
    if (tempf2) is false then
        jump-to: LABEL 112
LABEL 107
    tempf3 <- L03
    if (tempf3 = 0) is false then
        jump-to: LABEL 109
LABEL 108
    tempf4 <- peek-SP
    jump-to: LABEL 10c
LABEL 109
    if (tempf3 < 10) is false then
        jump-to: LABEL 10b
LABEL 10a
    tempf5 <- (tempf3 - 1)
    tempf4 <- L(tempf5)
    jump-to: LABEL 10c
LABEL 10b
    tempf6 <- (tempf3 - 10)
    tempf7 <- (tempf6 * 2)
    tempf8 <- (tempf7 + 4f0)
    tempf4 <- read-word(tempf8)
LABEL 10c
    if (tempf3 = 0) is false then
        jump-to: LABEL 10e
LABEL 10d
    update-SP: (int16(tempf4) - int16(1))
    jump-to: LABEL 111
LABEL 10e
    if (tempf3 < 10) is false then
        jump-to: LABEL 110
LABEL 10f
    tempf9 <- (tempf3 - 1)
    L(tempf9) <- (int16(tempf4) - int16(1))
    jump-to: LABEL 111
LABEL 110
    tempfa <- (tempf3 - 10)
    tempfb <- (tempfa * 2)
    tempfc <- (tempfb + 4f0)
    write-word(tempfc) <- (int16(tempf4) - int16(1))
LABEL 111
    L0a <- 2c
    L0b <- 2c
    L08 <- 0c04
    jump-to: LABEL 187
LABEL 112
    tempfd <- read-word(6ce)
    tempfe <- (tempfd = 32)
    if (tempfe) is false then
        jump-to: LABEL 115
LABEL 113
    print: "\n    inc_chk"
    L08 <- 0c09
    tempff <- L01
    temp100 <- (int16(tempff) + int16(1))
    L01 <- temp100
    if (int16(temp100) > int16(48)) is true then
        jump-to: LABEL 18a
LABEL 114
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 115
    temp101 <- read-word(6ce)
    temp102 <- (temp101 = 33)
    if (temp102) is false then
        jump-to: LABEL 122
LABEL 116
    L08 <- 0c0e
    temp103 <- L06
    if (temp103 = 0) is false then
        jump-to: LABEL 118
LABEL 117
    temp104 <- peek-SP
    jump-to: LABEL 11b
LABEL 118
    if (temp103 < 10) is false then
        jump-to: LABEL 11a
LABEL 119
    temp105 <- (temp103 - 1)
    temp104 <- L(temp105)
    jump-to: LABEL 11b
LABEL 11a
    temp106 <- (temp103 - 10)
    temp107 <- (temp106 * 2)
    temp108 <- (temp107 + 4f0)
    temp104 <- read-word(temp108)
LABEL 11b
    temp109 <- (int16(temp104) + int16(1))
    if (temp103 = 0) is false then
        jump-to: LABEL 11d
LABEL 11c
    update-SP: temp109
    jump-to: LABEL 120
LABEL 11d
    if (temp103 < 10) is false then
        jump-to: LABEL 11f
LABEL 11e
    temp10a <- (temp103 - 1)
    L(temp10a) <- temp109
    jump-to: LABEL 120
LABEL 11f
    temp10b <- (temp103 - 10)
    temp10c <- (temp10b * 2)
    temp10d <- (temp10c + 4f0)
    write-word(temp10d) <- temp109
LABEL 120
    if (int16(temp109) > int16(48)) is true then
        jump-to: LABEL 18a
LABEL 121
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 122
    temp10e <- read-word(6ce)
    temp10f <- (temp10e = 34)
    if (temp10f) is false then
        jump-to: LABEL 12f
LABEL 123
    L08 <- 0c13
    temp110 <- L06
    if (temp110 = 0) is false then
        jump-to: LABEL 125
LABEL 124
    temp111 <- peek-SP
    jump-to: LABEL 128
LABEL 125
    if (temp110 < 10) is false then
        jump-to: LABEL 127
LABEL 126
    temp112 <- (temp110 - 1)
    temp111 <- L(temp112)
    jump-to: LABEL 128
LABEL 127
    temp113 <- (temp110 - 10)
    temp114 <- (temp113 * 2)
    temp115 <- (temp114 + 4f0)
    temp111 <- read-word(temp115)
LABEL 128
    temp116 <- (int16(temp111) + int16(1))
    if (temp110 = 0) is false then
        jump-to: LABEL 12a
LABEL 129
    update-SP: temp116
    jump-to: LABEL 12d
LABEL 12a
    if (temp110 < 10) is false then
        jump-to: LABEL 12c
LABEL 12b
    temp117 <- (temp110 - 1)
    L(temp117) <- temp116
    jump-to: LABEL 12d
LABEL 12c
    temp118 <- (temp110 - 10)
    temp119 <- (temp118 * 2)
    temp11a <- (temp119 + 4f0)
    write-word(temp11a) <- temp116
LABEL 12d
    if (int16(temp116) > int16(48)) is true then
        jump-to: LABEL 18a
LABEL 12e
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 12f
    temp11b <- read-word(6ce)
    temp11c <- (temp11b = 35)
    if (temp11c) is false then
        jump-to: LABEL 132
LABEL 130
    L08 <- 0c19
    temp11d <- peek-SP
    temp11e <- (int16(temp11d) + int16(1))
    update-SP: temp11e
    if (int16(temp11e) > int16(2e)) is true then
        jump-to: LABEL 18a
LABEL 131
    L0a <- 2e
    L0b <- 2c
    jump-to: LABEL 187
LABEL 132
    temp11f <- read-word(6ce)
    temp120 <- (temp11f = 36)
    if (temp120) is false then
        jump-to: LABEL 13f
LABEL 133
    L08 <- 0c1b
    temp121 <- L03
    if (temp121 = 0) is false then
        jump-to: LABEL 135
LABEL 134
    temp122 <- peek-SP
    jump-to: LABEL 138
LABEL 135
    if (temp121 < 10) is false then
        jump-to: LABEL 137
LABEL 136
    temp123 <- (temp121 - 1)
    temp122 <- L(temp123)
    jump-to: LABEL 138
LABEL 137
    temp124 <- (temp121 - 10)
    temp125 <- (temp124 * 2)
    temp126 <- (temp125 + 4f0)
    temp122 <- read-word(temp126)
LABEL 138
    temp127 <- (int16(temp122) + int16(1))
    if (temp121 = 0) is false then
        jump-to: LABEL 13a
LABEL 139
    update-SP: temp127
    jump-to: LABEL 13d
LABEL 13a
    if (temp121 < 10) is false then
        jump-to: LABEL 13c
LABEL 13b
    temp128 <- (temp121 - 1)
    L(temp128) <- temp127
    jump-to: LABEL 13d
LABEL 13c
    temp129 <- (temp121 - 10)
    temp12a <- (temp129 * 2)
    temp12b <- (temp12a + 4f0)
    write-word(temp12b) <- temp127
LABEL 13d
    if (int16(temp127) > int16(2e)) is true then
        jump-to: LABEL 18a
LABEL 13e
    L0a <- 2e
    L0b <- 2c
    jump-to: LABEL 187
LABEL 13f
    temp12c <- read-word(6ce)
    temp12d <- (temp12c = 37)
    if (temp12d) is false then
        jump-to: LABEL 14c
LABEL 140
    L08 <- 0c20
    temp12e <- L03
    if (temp12e = 0) is false then
        jump-to: LABEL 142
LABEL 141
    temp12f <- peek-SP
    jump-to: LABEL 145
LABEL 142
    if (temp12e < 10) is false then
        jump-to: LABEL 144
LABEL 143
    temp130 <- (temp12e - 1)
    temp12f <- L(temp130)
    jump-to: LABEL 145
LABEL 144
    temp131 <- (temp12e - 10)
    temp132 <- (temp131 * 2)
    temp133 <- (temp132 + 4f0)
    temp12f <- read-word(temp133)
LABEL 145
    temp134 <- (int16(temp12f) + int16(1))
    if (temp12e = 0) is false then
        jump-to: LABEL 147
LABEL 146
    update-SP: temp134
    jump-to: LABEL 14a
LABEL 147
    if (temp12e < 10) is false then
        jump-to: LABEL 149
LABEL 148
    temp135 <- (temp12e - 1)
    L(temp135) <- temp134
    jump-to: LABEL 14a
LABEL 149
    temp136 <- (temp12e - 10)
    temp137 <- (temp136 * 2)
    temp138 <- (temp137 + 4f0)
    write-word(temp138) <- temp134
LABEL 14a
    if (int16(temp134) > int16(2e)) is true then
        jump-to: LABEL 18a
LABEL 14b
    L0a <- 2e
    L0b <- 2c
    jump-to: LABEL 187
LABEL 14c
    temp139 <- read-word(6ce)
    temp13a <- (temp139 = 3c)
    if (temp13a) is false then
        jump-to: LABEL 14f
LABEL 14d
    print: "dec_chk"
    L08 <- 0c26
    temp13b <- L01
    temp13c <- (int16(temp13b) - int16(1))
    L01 <- temp13c
    if (int16(temp13c) < int16(46)) is true then
        jump-to: LABEL 18a
LABEL 14e
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 14f
    temp13d <- read-word(6ce)
    temp13e <- (temp13d = 3d)
    if (temp13e) is false then
        jump-to: LABEL 15c
LABEL 150
    L08 <- 0c2b
    temp13f <- L06
    if (temp13f = 0) is false then
        jump-to: LABEL 152
LABEL 151
    temp140 <- peek-SP
    jump-to: LABEL 155
LABEL 152
    if (temp13f < 10) is false then
        jump-to: LABEL 154
LABEL 153
    temp141 <- (temp13f - 1)
    temp140 <- L(temp141)
    jump-to: LABEL 155
LABEL 154
    temp142 <- (temp13f - 10)
    temp143 <- (temp142 * 2)
    temp144 <- (temp143 + 4f0)
    temp140 <- read-word(temp144)
LABEL 155
    temp145 <- (int16(temp140) - int16(1))
    if (temp13f = 0) is false then
        jump-to: LABEL 157
LABEL 156
    update-SP: temp145
    jump-to: LABEL 15a
LABEL 157
    if (temp13f < 10) is false then
        jump-to: LABEL 159
LABEL 158
    temp146 <- (temp13f - 1)
    L(temp146) <- temp145
    jump-to: LABEL 15a
LABEL 159
    temp147 <- (temp13f - 10)
    temp148 <- (temp147 * 2)
    temp149 <- (temp148 + 4f0)
    write-word(temp149) <- temp145
LABEL 15a
    if (int16(temp145) < int16(46)) is true then
        jump-to: LABEL 18a
LABEL 15b
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 15c
    temp14a <- read-word(6ce)
    temp14b <- (temp14a = 3e)
    if (temp14b) is false then
        jump-to: LABEL 169
LABEL 15d
    L08 <- 0c30
    temp14c <- L06
    if (temp14c = 0) is false then
        jump-to: LABEL 15f
LABEL 15e
    temp14d <- peek-SP
    jump-to: LABEL 162
LABEL 15f
    if (temp14c < 10) is false then
        jump-to: LABEL 161
LABEL 160
    temp14e <- (temp14c - 1)
    temp14d <- L(temp14e)
    jump-to: LABEL 162
LABEL 161
    temp14f <- (temp14c - 10)
    temp150 <- (temp14f * 2)
    temp151 <- (temp150 + 4f0)
    temp14d <- read-word(temp151)
LABEL 162
    temp152 <- (int16(temp14d) - int16(1))
    if (temp14c = 0) is false then
        jump-to: LABEL 164
LABEL 163
    update-SP: temp152
    jump-to: LABEL 167
LABEL 164
    if (temp14c < 10) is false then
        jump-to: LABEL 166
LABEL 165
    temp153 <- (temp14c - 1)
    L(temp153) <- temp152
    jump-to: LABEL 167
LABEL 166
    temp154 <- (temp14c - 10)
    temp155 <- (temp154 * 2)
    temp156 <- (temp155 + 4f0)
    write-word(temp156) <- temp152
LABEL 167
    if (int16(temp152) < int16(46)) is true then
        jump-to: LABEL 18a
LABEL 168
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 187
LABEL 169
    temp157 <- read-word(6ce)
    temp158 <- (temp157 = 3f)
    if (temp158) is false then
        jump-to: LABEL 16c
LABEL 16a
    L08 <- 0c36
    temp159 <- peek-SP
    temp15a <- (int16(temp159) - int16(1))
    update-SP: temp15a
    if (int16(temp15a) < int16(2c)) is true then
        jump-to: LABEL 18a
LABEL 16b
    L0a <- 2c
    L0b <- 2c
    jump-to: LABEL 187
LABEL 16c
    temp15b <- read-word(6ce)
    temp15c <- (temp15b = 40)
    if (temp15c) is false then
        jump-to: LABEL 179
LABEL 16d
    L08 <- 0c38
    temp15d <- L03
    if (temp15d = 0) is false then
        jump-to: LABEL 16f
LABEL 16e
    temp15e <- peek-SP
    jump-to: LABEL 172
LABEL 16f
    if (temp15d < 10) is false then
        jump-to: LABEL 171
LABEL 170
    temp15f <- (temp15d - 1)
    temp15e <- L(temp15f)
    jump-to: LABEL 172
LABEL 171
    temp160 <- (temp15d - 10)
    temp161 <- (temp160 * 2)
    temp162 <- (temp161 + 4f0)
    temp15e <- read-word(temp162)
LABEL 172
    temp163 <- (int16(temp15e) - int16(1))
    if (temp15d = 0) is false then
        jump-to: LABEL 174
LABEL 173
    update-SP: temp163
    jump-to: LABEL 177
LABEL 174
    if (temp15d < 10) is false then
        jump-to: LABEL 176
LABEL 175
    temp164 <- (temp15d - 1)
    L(temp164) <- temp163
    jump-to: LABEL 177
LABEL 176
    temp165 <- (temp15d - 10)
    temp166 <- (temp165 * 2)
    temp167 <- (temp166 + 4f0)
    write-word(temp167) <- temp163
LABEL 177
    if (int16(temp163) < int16(2c)) is true then
        jump-to: LABEL 18a
LABEL 178
    L0a <- 2c
    L0b <- 2c
    jump-to: LABEL 187
LABEL 179
    temp168 <- read-word(6ce)
    temp169 <- (temp168 = 41)
    if (temp169) is false then
        jump-to: LABEL 186
LABEL 17a
    L08 <- 0c3d
    temp16a <- L03
    if (temp16a = 0) is false then
        jump-to: LABEL 17c
LABEL 17b
    temp16b <- peek-SP
    jump-to: LABEL 17f
LABEL 17c
    if (temp16a < 10) is false then
        jump-to: LABEL 17e
LABEL 17d
    temp16c <- (temp16a - 1)
    temp16b <- L(temp16c)
    jump-to: LABEL 17f
LABEL 17e
    temp16d <- (temp16a - 10)
    temp16e <- (temp16d * 2)
    temp16f <- (temp16e + 4f0)
    temp16b <- read-word(temp16f)
LABEL 17f
    temp170 <- (int16(temp16b) - int16(1))
    if (temp16a = 0) is false then
        jump-to: LABEL 181
LABEL 180
    update-SP: temp170
    jump-to: LABEL 184
LABEL 181
    if (temp16a < 10) is false then
        jump-to: LABEL 183
LABEL 182
    temp171 <- (temp16a - 1)
    L(temp171) <- temp170
    jump-to: LABEL 184
LABEL 183
    temp172 <- (temp16a - 10)
    temp173 <- (temp172 * 2)
    temp174 <- (temp173 + 4f0)
    write-word(temp174) <- temp170
LABEL 184
    if (int16(temp170) < int16(2c)) is true then
        jump-to: LABEL 18a
LABEL 185
    L0a <- 2c
    L0b <- 2c
    jump-to: LABEL 187
LABEL 186
    return: 0
LABEL 187
    temp175 <- L09
    temp176 <- (temp175 = 03e7)
    if (temp176) is true then
        jump-to: LABEL 189
LABEL 188
    temp177 <- L01
    temp178 <- L09
    temp179 <- L08
    discard: call 7e4 (temp177, temp178, temp179)
LABEL 189
    temp17a <- pop-SP
    L07 <- temp17a
    temp17b <- L07
    temp17c <- L0a
    temp17d <- L08
    discard: call 7e4 (temp17b, temp17c, temp17d)
    temp17e <- pop-SP
    L07 <- temp17e
    temp17f <- L07
    temp180 <- L0b
    temp181 <- L08
    discard: call 7e4 (temp17f, temp180, temp181)
    return: 1
LABEL 18a
    temp182 <- L01
    temp183 <- L08
    discard: call 7e4 (temp182, 7b, temp183)
    return: 0
]]>

        Test(CZech, &H1B34, expected)
    End Sub

    <Fact>
    Sub CZech_2448()
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

        Dim expected =
<![CDATA[
# temps: 11

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
    temp0a <- (temp08 = temp09)
    if (temp0a) is true then
        jump-to: LABEL 02
LABEL 01
    print: "\nERROR - Total number of tests should equal"
    print: " passed + failed + print tests!\n\n"
LABEL 02
    print: "Didn't crash: hooray!\n"
    print: "Last test: quit!\n"
    quit
]]>

        Test(CZech, &H2448, expected)
    End Sub

    <Fact>
    Sub Zork1_4E3B()
        ' 4e3b:  b2 ...                  PRINT           "a "
        ' 4e3e:  aa 01                   PRINT_OBJ       L00
        ' 4e40:  b0                      RTRUE

        Dim expected =
<![CDATA[
# temps: 8

LABEL 00
    print: "a "
    temp00 <- L00
    temp01 <- (temp00 - 1)
    temp02 <- (temp01 * 9)
    temp03 <- (temp02 + 2ee)
    temp04 <- (temp03 + 7)
    temp05 <- read-word(temp04)
    temp06 <- read-byte(temp05)
    temp07 <- (temp05 + 1)
    print: read-text(temp07, temp06)
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
# temps: 25

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
    temp15 <- (temp13 = temp14)
    if (temp15) is false then
        jump-to: LABEL 05
LABEL 04
    L02 <- 00
LABEL 05
    temp16 <- L00
    temp17 <- L02
    write-word(temp16) <- temp17
    temp18 <- L04
    return: temp18
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
    temp00 <- read-word(2361)
    temp01 <- (temp00 = 2b)
    if (temp01) is false then
        return: 0
LABEL 01
    push-SP: call 90ba (a3)
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
# temps: 4

LABEL 00
    temp00 <- read-word(22cd)
    if (temp00 = 0) is true then
        jump-to: LABEL 04
LABEL 01
    temp01 <- read-byte(904)
    temp02 <- (temp01 & 0010)
    temp03 <- (temp02 <> 0)
    if (temp03 = 1) is false then
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
    temp02 <- read-word(2361)
    temp03 <- (temp02 = 45)
    if (temp03) is false then
        return: 0
LABEL 02
    temp04 <- read-word(235d)
    if (temp04 = 0) is false then
        return: 0
LABEL 03
    push-SP: call 10666 (983b)
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
# temps: 96

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
    temp03 <- read-byte(temp02)
    temp04 <- (temp02 + 1)
    temp05 <- (temp03 * 2)
    temp06 <- (temp04 + temp05)
    temp07 <- uint16(temp06)
    temp08 <- 0
LABEL 01
    temp09 <- read-byte(temp07)
    if ((temp09 & 1f) <= 06) is false then
        jump-to: LABEL 03
LABEL 02
    temp08 <- 1
    jump-to: LABEL 04
LABEL 03
    temp0a <- read-byte(temp07)
    temp0b <- ((temp07 + 1) + ((temp0a >> 5) + 1))
    temp07 <- uint16(temp0b)
LABEL 04
    if (temp08 = 0) is true then
        jump-to: LABEL 01
    if ((temp09 & 1f) <> 06) is false then
        jump-to: LABEL 06
LABEL 05
    RUNTIME EXCEPTION: Property not found!
LABEL 06
    temp07 <- (temp07 + 1)
    if ((temp09 & e0) = 0) is false then
        jump-to: LABEL 08
LABEL 07
    write-byte(temp07) <- byte(04)
    jump-to: LABEL 09
LABEL 08
    write-word(temp07) <- 04
LABEL 09
    temp0c <- read-word(2291)
    push-SP: (int16(temp0c) + int16(02))
    temp0d <- read-word(2285)
    temp0e <- pop-SP
    write-word(temp0d + 2) <- temp0e
    temp0f <- read-word(2291)
    push-SP: (int16(temp0f) + int16(04))
    temp10 <- read-word(2285)
    temp11 <- pop-SP
    write-word(temp10 + 4) <- temp11
    temp12 <- read-word(228d)
    push-SP: (int16(temp12) + int16(02))
    temp13 <- read-word(2283)
    temp14 <- pop-SP
    write-word(temp13 + 4) <- temp14
    temp15 <- read-word(228d)
    push-SP: (int16(temp15) + int16(04))
    temp16 <- read-word(2283)
    temp17 <- pop-SP
    write-word(temp16 + 6) <- temp17
    temp18 <- read-word(228b)
    push-SP: (int16(temp18) + int16(02))
    temp19 <- read-word(2281)
    temp1a <- pop-SP
    write-word(temp19 + 2) <- temp1a
    temp1b <- read-word(2289)
    push-SP: (int16(temp1b) + int16(02))
    temp1c <- read-word(2281)
    temp1d <- pop-SP
    write-word(temp1c + 6) <- temp1d
    write-word(2271) <- b4
    push-SP: call 9530 (a0)
    temp1e <- read-word(2271)
    temp1f <- (temp1e - 1)
    temp20 <- (temp1f * 9)
    temp21 <- (temp20 + 2ee)
    temp22 <- read-byte(temp21)
    temp23 <- (temp22 & 0010)
    temp24 <- (temp23 <> 0)
    if (temp24 = 1) is true then
        jump-to: LABEL 0b
LABEL 0a
    push-SP: call 6ee0 ()
    print: "\n"
LABEL 0b
    write-word(22f5) <- 01
    write-word(234f) <- 04
    temp25 <- read-word(234f)
    write-word(2371) <- temp25
    temp26 <- read-word(234f)
    temp27 <- read-word(2271)
    temp28 <- 0
    temp29 <- (temp26 - 1)
    temp2a <- (temp29 * 9)
    temp2b <- (temp2a + 2ee)
    temp2c <- (temp2b + 5)
    temp2d <- read-byte(temp2c)
    temp2e <- (temp26 - 1)
    temp2f <- (temp2e * 9)
    temp30 <- (temp2f + 2ee)
    temp31 <- (temp30 + 4)
    temp32 <- read-byte(temp31)
    if (temp32 = 0) is false then
        jump-to: LABEL 0d
LABEL 0c
    temp33 <- 0
    jump-to: LABEL 0e
LABEL 0d
    temp34 <- (temp32 - 1)
    temp35 <- (temp34 * 9)
    temp36 <- (temp35 + 2ee)
    temp37 <- (temp36 + 6)
    temp38 <- read-byte(temp37)
    temp33 <- temp38
LABEL 0e
    if (temp33 <> temp26) is false then
        jump-to: LABEL 14
LABEL 0f
    temp39 <- temp33
LABEL 10
    temp3a <- (temp39 - 1)
    temp3b <- (temp3a * 9)
    temp3c <- (temp3b + 2ee)
    temp3d <- (temp3c + 5)
    temp3e <- read-byte(temp3d)
    if (temp3e = temp26) is false then
        jump-to: LABEL 12
LABEL 11
    temp28 <- temp39
    temp39 <- 0
    jump-to: LABEL 13
LABEL 12
    temp39 <- temp3e
LABEL 13
    if (temp39 <> 0) is true then
        jump-to: LABEL 10
LABEL 14
    if (temp28 <> 0) is false then
        jump-to: LABEL 16
LABEL 15
    temp3f <- (temp28 - 1)
    temp40 <- (temp3f * 9)
    temp41 <- (temp40 + 2ee)
    temp42 <- (temp41 + 5)
    write-byte(temp42) <- temp2d
LABEL 16
    if (temp33 = temp26) is false then
        jump-to: LABEL 18
LABEL 17
    temp43 <- (temp32 - 1)
    temp44 <- (temp43 * 9)
    temp45 <- (temp44 + 2ee)
    temp46 <- (temp45 + 6)
    write-byte(temp46) <- temp2d
LABEL 18
    temp47 <- (temp26 - 1)
    temp48 <- (temp47 * 9)
    temp49 <- (temp48 + 2ee)
    temp4a <- (temp49 + 4)
    write-byte(temp4a) <- 0
    temp4b <- (temp26 - 1)
    temp4c <- (temp4b * 9)
    temp4d <- (temp4c + 2ee)
    temp4e <- (temp4d + 5)
    write-byte(temp4e) <- 0
    if (temp27 <> 0) is false then
        jump-to: LABEL 1a
LABEL 19
    temp4f <- (temp26 - 1)
    temp50 <- (temp4f * 9)
    temp51 <- (temp50 + 2ee)
    temp52 <- (temp51 + 4)
    write-byte(temp52) <- temp27
    temp53 <- (temp27 - 1)
    temp54 <- (temp53 * 9)
    temp55 <- (temp54 + 2ee)
    temp56 <- (temp55 + 6)
    temp57 <- read-byte(temp56)
    temp58 <- (temp26 - 1)
    temp59 <- (temp58 * 9)
    temp5a <- (temp59 + 2ee)
    temp5b <- (temp5a + 5)
    write-byte(temp5b) <- temp57
    temp5c <- (temp27 - 1)
    temp5d <- (temp5c * 9)
    temp5e <- (temp5d + 2ee)
    temp5f <- (temp5e + 6)
    write-byte(temp5f) <- temp26
LABEL 1a
    push-SP: call 7e04 ()
    push-SP: call 552a ()
    jump-to: LABEL 00
]]>

        Test(Zork1, &H4F04, expected)
    End Sub

    <Fact>
    Sub Zork1_552A()
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

        Dim expected =
<![CDATA[
# temps: 199

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
    temp13 <- (temp12 = 01)
    if (temp13) is false then
        jump-to: LABEL 0e
LABEL 0d
    temp14 <- read-word(231b)
    L04 <- read-word(temp14 + 2)
LABEL 0e
    temp15 <- read-word(2361)
    temp16 <- (temp15 = 89)
    if (temp16) is false then
        jump-to: LABEL 10
LABEL 0f
    temp17 <- read-word(2361)
    temp18 <- read-word(235d)
    L06 <- call 577c (temp17, temp18)
    jump-to: LABEL 44
LABEL 10
    temp19 <- L02
    if (temp19 = 0) is false then
        jump-to: LABEL 16
LABEL 11
    temp1a <- read-word(2357)
    temp1b <- read-byte(temp1a)
    temp1c <- (temp1b & 03)
    if (temp1c = 0) is false then
        jump-to: LABEL 13
LABEL 12
    temp1d <- read-word(2361)
    L06 <- call 577c (temp1d)
    write-word(235d) <- 00
    jump-to: LABEL 44
LABEL 13
    temp1e <- read-word(22f5)
    if (temp1e = 0) is false then
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
    temp1f <- L02
    if (int16(temp1f) > int16(01)) is false then
        jump-to: LABEL 18
LABEL 17
    write-word(2367) <- 01
LABEL 18
    L09 <- 00
LABEL 19
    temp20 <- L02
    temp21 <- L03
    temp22 <- (int16(temp21) + int16(1))
    L03 <- temp22
    if (int16(temp22) > int16(temp20)) is false then
        jump-to: LABEL 25
LABEL 1a
    temp23 <- read-word(2365)
    if (int16(temp23) > int16(00)) is false then
        jump-to: LABEL 23
LABEL 1b
    print: "The "
    temp24 <- read-word(2365)
    temp25 <- L02
    temp26 <- (temp24 = temp25)
    if (temp26) is true then
        jump-to: LABEL 1d
LABEL 1c
    print: "other "
LABEL 1d
    print: "object"
    temp27 <- read-word(2365)
    temp28 <- (temp27 = 01)
    if (temp28) is true then
        jump-to: LABEL 1f
LABEL 1e
    print: "s"
LABEL 1f
    print: " that you mentioned "
    temp29 <- read-word(2365)
    temp2a <- (temp29 = 01)
    if (temp2a) is true then
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
    temp2b <- L09
    if (temp2b = 0) is false then
        jump-to: LABEL 44
LABEL 24
    print: "There's nothing here you can take."
    print: "\n"
    jump-to: LABEL 44
LABEL 25
    temp2c <- L07
    if (temp2c = 0) is true then
        jump-to: LABEL 27
LABEL 26
    temp2d <- read-word(231d)
    temp2e <- L03
    L08 <- read-word(temp2d + (temp2e * 2))
    jump-to: LABEL 28
LABEL 27
    temp2f <- read-word(231b)
    temp30 <- L03
    L08 <- read-word(temp2f + (temp30 * 2))
LABEL 28
    temp31 <- L07
    if (temp31 = 0) is true then
        jump-to: LABEL 2a
LABEL 29
    temp32 <- L08
    push-SP: temp32
    jump-to: LABEL 2b
LABEL 2a
    temp33 <- L04
    push-SP: temp33
LABEL 2b
    temp34 <- pop-SP
    write-word(235d) <- temp34
    temp35 <- L07
    if (temp35 = 0) is true then
        jump-to: LABEL 2d
LABEL 2c
    temp36 <- L04
    push-SP: temp36
    jump-to: LABEL 2e
LABEL 2d
    temp37 <- L08
    push-SP: temp37
LABEL 2e
    temp38 <- pop-SP
    write-word(235f) <- temp38
    temp39 <- L02
    if (int16(temp39) > int16(01)) is true then
        jump-to: LABEL 30
LABEL 2f
    temp3a <- read-word(2339)
    temp3b <- read-word(temp3a + c)
    temp3c <- read-word(temp3b)
    temp3d <- (temp3c = 3b7c)
    if (temp3d) is false then
        jump-to: LABEL 42
LABEL 30
    temp3e <- L08
    temp3f <- (temp3e = 0b)
    if (temp3f) is false then
        jump-to: LABEL 32
LABEL 31
    temp40 <- read-word(2365)
    write-word(2365) <- (int16(temp40) + int16(1))
    jump-to: LABEL 19
LABEL 32
    temp41 <- read-word(2361)
    temp42 <- (temp41 = 5d)
    if (temp42) is false then
        jump-to: LABEL 37
LABEL 33
    temp43 <- read-word(235f)
    if (temp43 = 0) is true then
        jump-to: LABEL 37
LABEL 34
    temp44 <- read-word(2339)
    temp45 <- read-word(temp44 + c)
    temp46 <- read-word(temp45)
    temp47 <- (temp46 = 3b7c)
    if (temp47) is false then
        jump-to: LABEL 37
LABEL 35
    temp48 <- read-word(235d)
    temp49 <- read-word(235f)
    temp4a <- (temp48 - 1)
    temp4b <- (temp4a * 9)
    temp4c <- (temp4b + 2ee)
    temp4d <- (temp4c + 4)
    temp4e <- read-byte(temp4d)
    if (temp4e = temp49) is true then
        jump-to: LABEL 37
LABEL 36
    jump-to: LABEL 19
LABEL 37
    temp4f <- read-word(2311)
    temp50 <- (temp4f = 01)
    if (temp50) is false then
        jump-to: LABEL 3e
LABEL 38
    temp51 <- read-word(2361)
    temp52 <- (temp51 = 5d)
    if (temp52) is false then
        jump-to: LABEL 3e
LABEL 39
    temp53 <- L08
    temp54 <- (temp53 - 1)
    temp55 <- (temp54 * 9)
    temp56 <- (temp55 + 2ee)
    temp57 <- (temp56 + 4)
    temp58 <- read-byte(temp57)
    temp59 <- read-word(234f)
    temp5a <- read-word(2271)
    temp5b <- (temp58 = temp59)
    temp5c <- (temp58 = temp5a)
    if (temp5b | temp5c) is true then
        jump-to: LABEL 3b
LABEL 3a
    temp5d <- L08
    temp5e <- (temp5d - 1)
    temp5f <- (temp5e * 9)
    temp60 <- (temp5f + 2ee)
    temp61 <- (temp60 + 4)
    temp62 <- read-byte(temp61)
    temp63 <- (temp62 - 1)
    temp64 <- (temp63 * 9)
    temp65 <- (temp64 + 2ee)
    temp66 <- (temp65 + 1)
    temp67 <- read-byte(temp66)
    temp68 <- (temp67 & 0020)
    temp69 <- (temp68 <> 0)
    if (temp69 = 1) is false then
        jump-to: LABEL 19
LABEL 3b
    temp6a <- L08
    temp6b <- (temp6a - 1)
    temp6c <- (temp6b * 9)
    temp6d <- (temp6c + 2ee)
    temp6e <- (temp6d + 2)
    temp6f <- read-byte(temp6e)
    temp70 <- (temp6f & 0040)
    temp71 <- (temp70 <> 0)
    if (temp71 = 1) is true then
        jump-to: LABEL 3e
LABEL 3c
    temp72 <- L08
    temp73 <- (temp72 - 1)
    temp74 <- (temp73 * 9)
    temp75 <- (temp74 + 2ee)
    temp76 <- (temp75 + 1)
    temp77 <- read-byte(temp76)
    temp78 <- (temp77 & 0004)
    temp79 <- (temp78 <> 0)
    if (temp79 = 1) is true then
        jump-to: LABEL 3e
LABEL 3d
    jump-to: LABEL 19
LABEL 3e
    temp7a <- L08
    temp7b <- (temp7a = 0c)
    if (temp7b) is false then
        jump-to: LABEL 40
LABEL 3f
    temp7c <- read-word(2347)
    temp7d <- (temp7c - 1)
    temp7e <- (temp7d * 9)
    temp7f <- (temp7e + 2ee)
    temp80 <- (temp7f + 7)
    temp81 <- read-word(temp80)
    temp82 <- read-byte(temp81)
    temp83 <- (temp81 + 1)
    print: read-text(temp83, temp82)
    jump-to: LABEL 41
LABEL 40
    temp84 <- L08
    temp85 <- (temp84 - 1)
    temp86 <- (temp85 * 9)
    temp87 <- (temp86 + 2ee)
    temp88 <- (temp87 + 7)
    temp89 <- read-word(temp88)
    temp8a <- read-byte(temp89)
    temp8b <- (temp89 + 1)
    print: read-text(temp8b, temp8a)
LABEL 41
    print: ": "
LABEL 42
    L09 <- 01
    temp8c <- read-word(2361)
    temp8d <- read-word(235d)
    temp8e <- read-word(235f)
    L06 <- call 577c (temp8c, temp8d, temp8e)
    temp8f <- L06
    temp90 <- (temp8f = 02)
    if (temp90) is false then
        jump-to: LABEL 19
LABEL 43
    jump-to: LABEL 44
LABEL 44
    temp91 <- L06
    temp92 <- (temp91 = 02)
    if (temp92) is true then
        jump-to: LABEL 52
LABEL 45
    temp93 <- read-word(234f)
    temp94 <- (temp93 - 1)
    temp95 <- (temp94 * 9)
    temp96 <- (temp95 + 2ee)
    temp97 <- (temp96 + 4)
    temp98 <- read-byte(temp97)
    temp99 <- (temp98 - 1)
    temp9a <- (temp99 * 9)
    temp9b <- (temp9a + 2ee)
    temp9c <- (temp9b + 7)
    temp9d <- read-word(temp9c)
    temp9e <- read-byte(temp9d)
    temp9f <- (temp9d + 1)
    tempa0 <- (temp9e * 2)
    tempa1 <- (temp9f + tempa0)
    tempa2 <- uint16(tempa1)
    tempa3 <- 0
LABEL 46
    tempa4 <- read-byte(tempa2)
    if ((tempa4 & 1f) <= 11) is false then
        jump-to: LABEL 48
LABEL 47
    tempa3 <- 1
    jump-to: LABEL 49
LABEL 48
    tempa5 <- read-byte(tempa2)
    tempa6 <- ((tempa2 + 1) + ((tempa5 >> 5) + 1))
    tempa2 <- uint16(tempa6)
LABEL 49
    if (tempa3 = 0) is true then
        jump-to: LABEL 46
    if ((tempa4 & 1f) = 11) is false then
        jump-to: LABEL 4e
LABEL 4a
    tempa2 <- (tempa2 + 1)
    if ((tempa4 & e0) = 0) is false then
        jump-to: LABEL 4c
LABEL 4b
    tempa7 <- read-byte(tempa2)
    jump-to: LABEL 4d
LABEL 4c
    tempa7 <- read-word(tempa2)
LABEL 4d
    jump-to: LABEL 4f
LABEL 4e
    tempa8 <- read-word(2d0)
    tempa7 <- uint16(tempa8)
LABEL 4f
    if (tempa7 = 0) is false then
        jump-to: LABEL 51
LABEL 50
    L06 <- 0
    jump-to: LABEL 52
LABEL 51
    L06 <- call (tempa7 * 2) (06)
LABEL 52
    tempa9 <- read-word(2361)
    tempaa <- (tempa9 = 08)
    tempab <- (tempa9 = 89)
    tempac <- (tempa9 = 0f)
    if ((tempaa | tempab) | tempac) is true then
        jump-to: LABEL 56
LABEL 53
    tempad <- read-word(2361)
    tempae <- (tempad = 0c)
    tempaf <- (tempad = 09)
    tempb0 <- (tempad = 07)
    if ((tempae | tempaf) | tempb0) is false then
        jump-to: LABEL 55
LABEL 54
    jump-to: LABEL 56
LABEL 55
    tempb1 <- read-word(2361)
    write-word(236d) <- tempb1
    tempb2 <- read-word(235d)
    write-word(236b) <- tempb2
    tempb3 <- read-word(235f)
    write-word(2369) <- tempb3
LABEL 56
    tempb4 <- L06
    tempb5 <- (tempb4 = 02)
    if (tempb5) is false then
        jump-to: LABEL 59
LABEL 57
    write-word(2349) <- 00
    jump-to: LABEL 59
LABEL 58
    write-word(2349) <- 00
LABEL 59
    tempb6 <- read-word(236f)
    if (tempb6 = 0) is true then
        jump-to: LABEL 00
LABEL 5a
    tempb7 <- read-word(2361)
    tempb8 <- (tempb7 = 02)
    tempb9 <- (tempb7 = 01)
    tempba <- (tempb7 = 6f)
    if ((tempb8 | tempb9) | tempba) is true then
        jump-to: LABEL 00
LABEL 5b
    tempbb <- read-word(2361)
    tempbc <- (tempbb = 0c)
    tempbd <- (tempbb = 08)
    tempbe <- (tempbb = 00)
    if ((tempbc | tempbd) | tempbe) is true then
        jump-to: LABEL 00
LABEL 5c
    tempbf <- read-word(2361)
    tempc0 <- (tempbf = 09)
    tempc1 <- (tempbf = 06)
    tempc2 <- (tempbf = 05)
    if ((tempc0 | tempc1) | tempc2) is true then
        jump-to: LABEL 00
LABEL 5d
    tempc3 <- read-word(2361)
    tempc4 <- (tempc3 = 07)
    tempc5 <- (tempc3 = 0b)
    tempc6 <- (tempc3 = 0a)
    if ((tempc4 | tempc5) | tempc6) is false then
        jump-to: LABEL 5f
LABEL 5e
    jump-to: LABEL 00
LABEL 5f
    L06 <- call 54c4 ()
    jump-to: LABEL 00
]]>

        Test(Zork1, &H552A, expected)
    End Sub

    <Fact>
    Sub Zork1_6A52()
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

        Dim expected =
<![CDATA[
# temps: 94

LABEL 00
    temp00 <- L00
    temp01 <- (temp00 - 1)
    temp02 <- (temp01 * 9)
    temp03 <- (temp02 + 2ee)
    temp04 <- (temp03 + 6)
    temp05 <- read-byte(temp04)
    L00 <- temp05
    if (temp05 <> 0) is false then
        return: 0
LABEL 01
    temp06 <- L02
    temp07 <- (temp06 = 02)
    if (temp07) is true then
        jump-to: LABEL 0c
LABEL 02
    temp08 <- L00
    temp09 <- (temp08 - 1)
    temp0a <- (temp09 * 9)
    temp0b <- (temp0a + 2ee)
    temp0c <- (temp0b + 7)
    temp0d <- read-word(temp0c)
    temp0e <- read-byte(temp0d)
    temp0f <- (temp0d + 1)
    temp10 <- (temp0e * 2)
    temp11 <- (temp0f + temp10)
    temp12 <- uint16(temp11)
    temp13 <- 0
LABEL 03
    temp14 <- read-byte(temp12)
    if ((temp14 & 1f) <= 12) is false then
        jump-to: LABEL 05
LABEL 04
    temp13 <- 1
    jump-to: LABEL 06
LABEL 05
    temp15 <- read-byte(temp12)
    temp16 <- ((temp12 + 1) + ((temp15 >> 5) + 1))
    temp12 <- uint16(temp16)
LABEL 06
    if (temp13 = 0) is true then
        jump-to: LABEL 03
    if ((temp14 & 1f) = 12) is false then
        jump-to: LABEL 08
LABEL 07
    push-SP: (temp12 + 1)
    jump-to: LABEL 09
LABEL 08
    push-SP: 0
LABEL 09
    temp17 <- pop-SP
    if (temp17 = 0) is true then
        jump-to: LABEL 0c
LABEL 0a
    temp18 <- L00
    temp19 <- L01
    temp1a <- call 6d1c (temp18, temp19)
    if (temp1a = 0) is true then
        jump-to: LABEL 0c
LABEL 0b
    temp1b <- L00
    temp1c <- L01
    push-SP: call 6aba (temp1b, temp1c)
LABEL 0c
    temp1d <- L02
    temp1e <- (temp1d = 00)
    if (temp1e) is false then
        jump-to: LABEL 0f
LABEL 0d
    temp1f <- L00
    temp20 <- (temp1f - 1)
    temp21 <- (temp20 * 9)
    temp22 <- (temp21 + 2ee)
    temp23 <- (temp22 + 1)
    temp24 <- read-byte(temp23)
    temp25 <- (temp24 & 0080)
    temp26 <- (temp25 <> 0)
    if (temp26 = 1) is true then
        jump-to: LABEL 0f
LABEL 0e
    temp27 <- L00
    temp28 <- (temp27 - 1)
    temp29 <- (temp28 * 9)
    temp2a <- (temp29 + 2ee)
    temp2b <- (temp2a + 1)
    temp2c <- read-byte(temp2b)
    temp2d <- (temp2c & 0020)
    temp2e <- (temp2d <> 0)
    if (temp2e = 1) is false then
        jump-to: LABEL 18
LABEL 0f
    temp2f <- L00
    temp30 <- (temp2f - 1)
    temp31 <- (temp30 * 9)
    temp32 <- (temp31 + 2ee)
    temp33 <- (temp32 + 6)
    temp34 <- read-byte(temp33)
    L04 <- temp34
    if (temp34 <> 0) is false then
        jump-to: LABEL 18
LABEL 10
    temp35 <- L00
    temp36 <- (temp35 - 1)
    temp37 <- (temp36 * 9)
    temp38 <- (temp37 + 2ee)
    temp39 <- (temp38 + 1)
    temp3a <- read-byte(temp39)
    temp3b <- (temp3a & 0010)
    temp3c <- (temp3b <> 0)
    if (temp3c = 1) is true then
        jump-to: LABEL 12
LABEL 11
    temp3d <- L00
    temp3e <- (temp3d - 1)
    temp3f <- (temp3e * 9)
    temp40 <- (temp3f + 2ee)
    temp41 <- (temp40 + 1)
    temp42 <- read-byte(temp41)
    temp43 <- (temp42 & 0008)
    temp44 <- (temp43 <> 0)
    if (temp44 = 1) is false then
        jump-to: LABEL 18
LABEL 12
    temp45 <- L00
    temp46 <- (temp45 - 1)
    temp47 <- (temp46 * 9)
    temp48 <- (temp47 + 2ee)
    temp49 <- (temp48 + 1)
    temp4a <- read-byte(temp49)
    temp4b <- (temp4a & 0020)
    temp4c <- (temp4b <> 0)
    if (temp4c = 1) is false then
        jump-to: LABEL 14
LABEL 13
    push-SP: 01
    jump-to: LABEL 17
LABEL 14
    temp4d <- L00
    temp4e <- (temp4d - 1)
    temp4f <- (temp4e * 9)
    temp50 <- (temp4f + 2ee)
    temp51 <- (temp50 + 1)
    temp52 <- read-byte(temp51)
    temp53 <- (temp52 & 0080)
    temp54 <- (temp53 <> 0)
    if (temp54 = 1) is false then
        jump-to: LABEL 16
LABEL 15
    push-SP: 01
    jump-to: LABEL 17
LABEL 16
    push-SP: 00
LABEL 17
    temp55 <- L00
    temp56 <- L01
    temp57 <- pop-SP
    L03 <- call 6a52 (temp55, temp56, temp57)
LABEL 18
    temp58 <- L00
    temp59 <- (temp58 - 1)
    temp5a <- (temp59 * 9)
    temp5b <- (temp5a + 2ee)
    temp5c <- (temp5b + 5)
    temp5d <- read-byte(temp5c)
    L00 <- temp5d
    if (temp5d <> 0) is true then
        jump-to: LABEL 01
LABEL 19
    return: 1
]]>

        Test(Zork1, &H6A52, expected)
    End Sub

    <Fact>
    Sub Zork1_8C9A()
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

        Dim expected =
<![CDATA[
# temps: 158

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
    temp07 <- (temp06 - 1)
    temp08 <- (temp07 * 9)
    temp09 <- (temp08 + 2ee)
    temp0a <- read-byte(temp09)
    temp0b <- (temp0a & 0010)
    temp0c <- (temp0b <> 0)
    if (temp0c = 1) is true then
        jump-to: LABEL 09
LABEL 08
    temp0d <- read-word(2271)
    temp0e <- (temp0d - 1)
    temp0f <- (temp0e * 9)
    temp10 <- (temp0f + 2ee)
    temp11 <- read-byte(temp10)
    write-byte(temp10) <- byte(temp11 | 0010)
    L01 <- 01
LABEL 09
    temp12 <- read-word(2271)
    temp13 <- (temp12 - 1)
    temp14 <- (temp13 * 9)
    temp15 <- (temp14 + 2ee)
    temp16 <- read-byte(temp15)
    temp17 <- (temp16 & 0004)
    temp18 <- (temp17 <> 0)
    if (temp18 = 1) is false then
        jump-to: LABEL 0b
LABEL 0a
    temp19 <- read-word(2271)
    temp1a <- (temp19 - 1)
    temp1b <- (temp1a * 9)
    temp1c <- (temp1b + 2ee)
    temp1d <- read-byte(temp1c)
    write-byte(temp1c) <- byte(temp1d & not 0010)
LABEL 0b
    temp1e <- read-word(2271)
    temp1f <- (temp1e - 1)
    temp20 <- (temp1f * 9)
    temp21 <- (temp20 + 2ee)
    temp22 <- (temp21 + 4)
    temp23 <- read-byte(temp22)
    if (temp23 = 52) is false then
        jump-to: LABEL 0f
LABEL 0c
    temp24 <- read-word(2271)
    temp25 <- (temp24 - 1)
    temp26 <- (temp25 * 9)
    temp27 <- (temp26 + 2ee)
    temp28 <- (temp27 + 7)
    temp29 <- read-word(temp28)
    temp2a <- read-byte(temp29)
    temp2b <- (temp29 + 1)
    print: read-text(temp2b, temp2a)
    temp2c <- read-word(234f)
    temp2d <- (temp2c - 1)
    temp2e <- (temp2d * 9)
    temp2f <- (temp2e + 2ee)
    temp30 <- (temp2f + 4)
    temp31 <- read-byte(temp30)
    L03 <- temp31
    temp32 <- L03
    temp33 <- (temp32 - 1)
    temp34 <- (temp33 * 9)
    temp35 <- (temp34 + 2ee)
    temp36 <- (temp35 + 3)
    temp37 <- read-byte(temp36)
    temp38 <- (temp37 & 0010)
    temp39 <- (temp38 <> 0)
    if (temp39 = 1) is false then
        jump-to: LABEL 0e
LABEL 0d
    print: ", in the "
    temp3a <- L03
    temp3b <- (temp3a - 1)
    temp3c <- (temp3b * 9)
    temp3d <- (temp3c + 2ee)
    temp3e <- (temp3d + 7)
    temp3f <- read-word(temp3e)
    temp40 <- read-byte(temp3f)
    temp41 <- (temp3f + 1)
    print: read-text(temp41, temp40)
LABEL 0e
    print: "\n"
LABEL 0f
    temp42 <- L00
    if (temp42 = 0) is false then
        jump-to: LABEL 11
LABEL 10
    temp43 <- read-word(22fd)
    if (temp43 = 0) is false then
        return: 1
LABEL 11
    temp44 <- read-word(234f)
    temp45 <- (temp44 - 1)
    temp46 <- (temp45 * 9)
    temp47 <- (temp46 + 2ee)
    temp48 <- (temp47 + 4)
    temp49 <- read-byte(temp48)
    L03 <- temp49
    temp4a <- L01
    if (temp4a = 0) is true then
        jump-to: LABEL 20
LABEL 12
    temp4b <- read-word(2271)
    temp4c <- (temp4b - 1)
    temp4d <- (temp4c * 9)
    temp4e <- (temp4d + 2ee)
    temp4f <- (temp4e + 7)
    temp50 <- read-word(temp4f)
    temp51 <- read-byte(temp50)
    temp52 <- (temp50 + 1)
    temp53 <- (temp51 * 2)
    temp54 <- (temp52 + temp53)
    temp55 <- uint16(temp54)
    temp56 <- 0
LABEL 13
    temp57 <- read-byte(temp55)
    if ((temp57 & 1f) <= 11) is false then
        jump-to: LABEL 15
LABEL 14
    temp56 <- 1
    jump-to: LABEL 16
LABEL 15
    temp58 <- read-byte(temp55)
    temp59 <- ((temp55 + 1) + ((temp58 >> 5) + 1))
    temp55 <- uint16(temp59)
LABEL 16
    if (temp56 = 0) is true then
        jump-to: LABEL 13
    if ((temp57 & 1f) = 11) is false then
        jump-to: LABEL 1b
LABEL 17
    temp55 <- (temp55 + 1)
    if ((temp57 & e0) = 0) is false then
        jump-to: LABEL 19
LABEL 18
    temp5a <- read-byte(temp55)
    jump-to: LABEL 1a
LABEL 19
    temp5a <- read-word(temp55)
LABEL 1a
    jump-to: LABEL 1c
LABEL 1b
    temp5b <- read-word(2d0)
    temp5a <- uint16(temp5b)
LABEL 1c
    if (temp5a = 0) is false then
        jump-to: LABEL 1e
LABEL 1d
    push-SP: 0
    jump-to: LABEL 1f
LABEL 1e
    push-SP: call (temp5a * 2) (03)
LABEL 1f
    temp5c <- pop-SP
    if (temp5c = 0) is false then
        return: 1
LABEL 20
    temp5d <- L01
    if (temp5d = 0) is true then
        jump-to: LABEL 2d
LABEL 21
    temp5e <- read-word(2271)
    temp5f <- (temp5e - 1)
    temp60 <- (temp5f * 9)
    temp61 <- (temp60 + 2ee)
    temp62 <- (temp61 + 7)
    temp63 <- read-word(temp62)
    temp64 <- read-byte(temp63)
    temp65 <- (temp63 + 1)
    temp66 <- (temp64 * 2)
    temp67 <- (temp65 + temp66)
    temp68 <- uint16(temp67)
    temp69 <- 0
LABEL 22
    temp6a <- read-byte(temp68)
    if ((temp6a & 1f) <= 0b) is false then
        jump-to: LABEL 24
LABEL 23
    temp69 <- 1
    jump-to: LABEL 25
LABEL 24
    temp6b <- read-byte(temp68)
    temp6c <- ((temp68 + 1) + ((temp6b >> 5) + 1))
    temp68 <- uint16(temp6c)
LABEL 25
    if (temp69 = 0) is true then
        jump-to: LABEL 22
    if ((temp6a & 1f) = 0b) is false then
        jump-to: LABEL 2a
LABEL 26
    temp68 <- (temp68 + 1)
    if ((temp6a & e0) = 0) is false then
        jump-to: LABEL 28
LABEL 27
    temp6d <- read-byte(temp68)
    jump-to: LABEL 29
LABEL 28
    temp6d <- read-word(temp68)
LABEL 29
    jump-to: LABEL 2b
LABEL 2a
    temp6e <- read-word(2c4)
    temp6d <- uint16(temp6e)
LABEL 2b
    L02 <- temp6d
    temp6f <- L02
    if (temp6f = 0) is true then
        jump-to: LABEL 2d
LABEL 2c
    temp70 <- L02
    print: read-text(temp70 * 2)
    print: "\n"
    jump-to: LABEL 3a
LABEL 2d
    temp71 <- read-word(2271)
    temp72 <- (temp71 - 1)
    temp73 <- (temp72 * 9)
    temp74 <- (temp73 + 2ee)
    temp75 <- (temp74 + 7)
    temp76 <- read-word(temp75)
    temp77 <- read-byte(temp76)
    temp78 <- (temp76 + 1)
    temp79 <- (temp77 * 2)
    temp7a <- (temp78 + temp79)
    temp7b <- uint16(temp7a)
    temp7c <- 0
LABEL 2e
    temp7d <- read-byte(temp7b)
    if ((temp7d & 1f) <= 11) is false then
        jump-to: LABEL 30
LABEL 2f
    temp7c <- 1
    jump-to: LABEL 31
LABEL 30
    temp7e <- read-byte(temp7b)
    temp7f <- ((temp7b + 1) + ((temp7e >> 5) + 1))
    temp7b <- uint16(temp7f)
LABEL 31
    if (temp7c = 0) is true then
        jump-to: LABEL 2e
    if ((temp7d & 1f) = 11) is false then
        jump-to: LABEL 36
LABEL 32
    temp7b <- (temp7b + 1)
    if ((temp7d & e0) = 0) is false then
        jump-to: LABEL 34
LABEL 33
    temp80 <- read-byte(temp7b)
    jump-to: LABEL 35
LABEL 34
    temp80 <- read-word(temp7b)
LABEL 35
    jump-to: LABEL 37
LABEL 36
    temp81 <- read-word(2d0)
    temp80 <- uint16(temp81)
LABEL 37
    if (temp80 = 0) is false then
        jump-to: LABEL 39
LABEL 38
    push-SP: 0
    jump-to: LABEL 3a
LABEL 39
    push-SP: call (temp80 * 2) (04)
LABEL 3a
    temp82 <- read-word(2271)
    temp83 <- L03
    temp84 <- (temp82 = temp83)
    if (temp84) is true then
        return: 1
LABEL 3b
    temp85 <- L03
    temp86 <- (temp85 - 1)
    temp87 <- (temp86 * 9)
    temp88 <- (temp87 + 2ee)
    temp89 <- (temp88 + 3)
    temp8a <- read-byte(temp89)
    temp8b <- (temp8a & 0010)
    temp8c <- (temp8b <> 0)
    if (temp8c = 1) is false then
        return: 1
LABEL 3c
    temp8d <- L03
    temp8e <- (temp8d - 1)
    temp8f <- (temp8e * 9)
    temp90 <- (temp8f + 2ee)
    temp91 <- (temp90 + 7)
    temp92 <- read-word(temp91)
    temp93 <- read-byte(temp92)
    temp94 <- (temp92 + 1)
    temp95 <- (temp93 * 2)
    temp96 <- (temp94 + temp95)
    temp97 <- uint16(temp96)
    temp98 <- 0
LABEL 3d
    temp99 <- read-byte(temp97)
    if ((temp99 & 1f) <= 11) is false then
        jump-to: LABEL 3f
LABEL 3e
    temp98 <- 1
    jump-to: LABEL 40
LABEL 3f
    temp9a <- read-byte(temp97)
    temp9b <- ((temp97 + 1) + ((temp9a >> 5) + 1))
    temp97 <- uint16(temp9b)
LABEL 40
    if (temp98 = 0) is true then
        jump-to: LABEL 3d
    if ((temp99 & 1f) = 11) is false then
        jump-to: LABEL 45
LABEL 41
    temp97 <- (temp97 + 1)
    if ((temp99 & e0) = 0) is false then
        jump-to: LABEL 43
LABEL 42
    temp9c <- read-byte(temp97)
    jump-to: LABEL 44
LABEL 43
    temp9c <- read-word(temp97)
LABEL 44
    jump-to: LABEL 46
LABEL 45
    temp9d <- read-word(2d0)
    temp9c <- uint16(temp9d)
LABEL 46
    if (temp9c = 0) is false then
        jump-to: LABEL 48
LABEL 47
    push-SP: 0
    jump-to: LABEL 49
LABEL 48
    push-SP: call (temp9c * 2) (03)
LABEL 49
    return: 1
]]>

        Test(Zork1, &H8C9A, expected)
    End Sub

    <Fact>
    Sub Zork1_101E0()
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

        Dim expected =
<![CDATA[
# temps: 129

LABEL 00
    temp00 <- read-byte(6eb)
    L00 <- temp00
LABEL 01
    temp01 <- read-byte(6e7)
    temp02 <- (temp01 & 0001)
    temp03 <- (temp02 <> 0)
    if (temp03 = 1) is true then
        jump-to: LABEL 03
LABEL 02
    push-SP: 01
    jump-to: LABEL 04
LABEL 03
    push-SP: 00
LABEL 04
    temp04 <- pop-SP
    L02 <- temp04
    temp05 <- L02
    if (temp05 = 0) is true then
        jump-to: LABEL 06
LABEL 05
    temp06 <- read-byte(6eb)
    L00 <- temp06
LABEL 06
    temp07 <- L00
    temp08 <- (temp07 = be)
    if (temp08) is false then
        jump-to: LABEL 0b
LABEL 07
    temp09 <- L00
    temp0a <- read-word(2271)
    temp0b <- (temp09 = temp0a)
    if (temp0b) is true then
        jump-to: LABEL 0b
LABEL 08
    temp0c <- L02
    if (temp0c = 0) is true then
        jump-to: LABEL 0a
LABEL 09
    push-SP: call d2d2 ()
    L02 <- 00
LABEL 0a
    push-SP: call d2ee (be)
    jump-to: LABEL 1a
LABEL 0b
    temp0d <- L00
    temp0e <- read-word(2271)
    temp0f <- (temp0d = temp0e)
    if (temp0f) is false then
        jump-to: LABEL 11
LABEL 0c
    temp10 <- L00
    temp11 <- (temp10 - 1)
    temp12 <- (temp11 * 9)
    temp13 <- (temp12 + 2ee)
    temp14 <- (temp13 + 2)
    temp15 <- read-byte(temp14)
    temp16 <- (temp15 & 0008)
    temp17 <- (temp16 <> 0)
    if (temp17 = 1) is true then
        jump-to: LABEL 11
LABEL 0d
    temp18 <- read-word(2271)
    temp19 <- read-byte(a8a)
    if (temp19 = temp18) is true then
        jump-to: LABEL 11
LABEL 0e
    temp1a <- L02
    temp1b <- call ce8e (temp1a)
    if (temp1b = 0) is false then
        return: 1
LABEL 0f
    temp1c <- read-byte(6e7)
    temp1d <- (temp1c & 0001)
    temp1e <- (temp1d <> 0)
    if (temp1e = 1) is false then
        jump-to: LABEL 1a
LABEL 10
    L02 <- 00
    jump-to: LABEL 1a
LABEL 11
    temp1f <- L00
    temp20 <- read-byte(6eb)
    if (temp20 = temp1f) is false then
        jump-to: LABEL 14
LABEL 12
    temp21 <- read-byte(6e7)
    temp22 <- (temp21 & 0001)
    temp23 <- (temp22 <> 0)
    if (temp23 = 1) is true then
        jump-to: LABEL 14
LABEL 13
    temp24 <- read-byte(6e7)
    write-byte(6e7) <- byte(temp24 | 0001)
    L02 <- 00
LABEL 14
    temp25 <- L00
    temp26 <- (temp25 - 1)
    temp27 <- (temp26 * 9)
    temp28 <- (temp27 + 2ee)
    temp29 <- read-byte(temp28)
    temp2a <- (temp29 & 0010)
    temp2b <- (temp2a <> 0)
    if (temp2b = 1) is false then
        jump-to: LABEL 1a
LABEL 15
    temp2c <- L00
    push-SP: call 103b8 (temp2c, 72, 4b)
    temp2d <- L00
    temp2e <- (temp2d - 1)
    temp2f <- (temp2e * 9)
    temp30 <- (temp2f + 2ee)
    temp31 <- read-byte(temp30)
    temp32 <- (temp31 & 0004)
    temp33 <- (temp32 <> 0)
    if (temp33 = 1) is false then
        jump-to: LABEL 18
LABEL 16
    temp34 <- read-word(2271)
    temp35 <- (temp34 - 1)
    temp36 <- (temp35 * 9)
    temp37 <- (temp36 + 2ee)
    temp38 <- read-byte(temp37)
    temp39 <- (temp38 & 0004)
    temp3a <- (temp39 <> 0)
    if (temp3a = 1) is false then
        jump-to: LABEL 18
LABEL 17
    temp3b <- L00
    push-SP: call d32c (temp3b)
    jump-to: LABEL 19
LABEL 18
    temp3c <- L00
    push-SP: call 1034c (temp3c)
LABEL 19
    temp3d <- pop-SP
    L04 <- temp3d
LABEL 1a
    temp3e <- L03
    if (temp3e = 0) is false then
        jump-to: LABEL 1c
LABEL 1b
    push-SP: 01
    jump-to: LABEL 1d
LABEL 1c
    push-SP: 00
LABEL 1d
    temp3f <- pop-SP
    L03 <- temp3f
    temp40 <- L03
    if (temp40 = 0) is true then
        jump-to: LABEL 36
LABEL 1e
    temp41 <- L02
    if (temp41 = 0) is false then
        jump-to: LABEL 36
LABEL 1f
    push-SP: call 1033c ()
LABEL 20
    temp42 <- L00
    if (temp42 = 0) is true then
        jump-to: LABEL 23
LABEL 21
    temp43 <- L00
    temp44 <- (temp43 - 1)
    temp45 <- (temp44 * 9)
    temp46 <- (temp45 + 2ee)
    temp47 <- (temp46 + 5)
    temp48 <- read-byte(temp47)
    L00 <- temp48
    if (temp48 <> 0) is false then
        jump-to: LABEL 23
LABEL 22
    jump-to: LABEL 24
LABEL 23
    temp49 <- read-byte(5cd)
    L00 <- temp49
    if (temp49 <> 0) is true then
        jump-to: LABEL 24
LABEL 24
    temp4a <- L00
    temp4b <- (temp4a - 1)
    temp4c <- (temp4b * 9)
    temp4d <- (temp4c + 2ee)
    temp4e <- (temp4d + 1)
    temp4f <- read-byte(temp4e)
    temp50 <- (temp4f & 0040)
    temp51 <- (temp50 <> 0)
    if (temp51 = 1) is true then
        jump-to: LABEL 20
LABEL 25
    temp52 <- L00
    temp53 <- (temp52 - 1)
    temp54 <- (temp53 * 9)
    temp55 <- (temp54 + 2ee)
    temp56 <- read-byte(temp55)
    temp57 <- (temp56 & 0002)
    temp58 <- (temp57 <> 0)
    if (temp58 = 1) is false then
        jump-to: LABEL 20
LABEL 26
    temp59 <- L00
    temp5a <- 0
    temp5b <- read-byte(6ec)
    temp5c <- read-byte(6eb)
    if (temp5c = 0) is false then
        jump-to: LABEL 28
LABEL 27
    temp5d <- 0
    jump-to: LABEL 29
LABEL 28
    temp5e <- (temp5c - 1)
    temp5f <- (temp5e * 9)
    temp60 <- (temp5f + 2ee)
    temp61 <- (temp60 + 6)
    temp62 <- read-byte(temp61)
    temp5d <- temp62
LABEL 29
    if (temp5d <> 72) is false then
        jump-to: LABEL 2f
LABEL 2a
    temp63 <- temp5d
LABEL 2b
    temp64 <- (temp63 - 1)
    temp65 <- (temp64 * 9)
    temp66 <- (temp65 + 2ee)
    temp67 <- (temp66 + 5)
    temp68 <- read-byte(temp67)
    if (temp68 = 72) is false then
        jump-to: LABEL 2d
LABEL 2c
    temp5a <- temp63
    temp63 <- 0
    jump-to: LABEL 2e
LABEL 2d
    temp63 <- temp68
LABEL 2e
    if (temp63 <> 0) is true then
        jump-to: LABEL 2b
LABEL 2f
    if (temp5a <> 0) is false then
        jump-to: LABEL 31
LABEL 30
    temp69 <- (temp5a - 1)
    temp6a <- (temp69 * 9)
    temp6b <- (temp6a + 2ee)
    temp6c <- (temp6b + 5)
    write-byte(temp6c) <- temp5b
LABEL 31
    if (temp5d = 72) is false then
        jump-to: LABEL 33
LABEL 32
    temp6d <- (temp5c - 1)
    temp6e <- (temp6d * 9)
    temp6f <- (temp6e + 2ee)
    temp70 <- (temp6f + 6)
    write-byte(temp70) <- temp5b
LABEL 33
    write-byte(6eb) <- 0
    write-byte(6ec) <- 0
    if (temp59 <> 0) is false then
        jump-to: LABEL 35
LABEL 34
    write-byte(6eb) <- temp59
    temp71 <- (temp59 - 1)
    temp72 <- (temp71 * 9)
    temp73 <- (temp72 + 2ee)
    temp74 <- (temp73 + 6)
    temp75 <- read-byte(temp74)
    write-byte(6ec) <- temp75
    temp76 <- (temp59 - 1)
    temp77 <- (temp76 * 9)
    temp78 <- (temp77 + 2ee)
    temp79 <- (temp78 + 6)
    write-byte(temp79) <- 72
LABEL 35
    temp7a <- read-byte(6e7)
    write-byte(6e7) <- byte(temp7a & not 0020)
    temp7b <- read-byte(6e7)
    write-byte(6e7) <- byte(temp7b | 0001)
    write-word(22af) <- 00
    jump-to: LABEL 01
LABEL 36
    temp7c <- L00
    temp7d <- (temp7c = be)
    if (temp7d) is true then
        jump-to: LABEL 38
LABEL 37
    temp7e <- L00
    push-SP: call 102c4 (temp7e)
    temp7f <- L04
    return: temp7f
LABEL 38
    temp80 <- L04
    return: temp80
]]>

        Test(Zork1, &H101E0, expected)
    End Sub

    Private Sub Test(gameName As String, address As Integer, expected As XCData, Optional debugging As Boolean = False)
        Dim memory = GameMemory(gameName)
        Dim reader = New RoutineReader(memory)

        Dim a = RawAddress(address)
        Dim r = reader.ReadRoutine(a)

        Assert.Equal(a, r.Address)

        Dim binder = New RoutineBinder(memory, debugging)
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
