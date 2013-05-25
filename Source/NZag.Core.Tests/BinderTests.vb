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
    write-word(06ce) <- call 2448 ()
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
    temp03 <- read-word(04f2)
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
    discard: call 08ec ()
    print: " Expected "
    temp05 <- L01
    print: number-to-text(int16(temp05))
    print: "; got "
    temp06 <- L00
    print: number-to-text(int16(temp06))
    print: "\n\n"
    jump-to: LABEL 05
LABEL 04
    discard: call 08e0 ()
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
# temps: 6

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
    temp01 <- read-word(04f2)
    push-SP: (int16(temp01) + int16(01))
    temp02 <- pop-SP
    print: number-to-text(int16(temp02))
    print: "]: "
    print: "jump"
    jump-to: LABEL 03
    print: "bad!"
    quit
LABEL 03
    discard: call 08e0 ()
    print: "je"
    if (1) is false then
        jump-to: LABEL 1f
LABEL 04
    discard: call 08e0 ()
    if (0) is true then
        jump-to: LABEL 1f
LABEL 05
    discard: call 08e0 ()
    if (0) is true then
        jump-to: LABEL 1f
LABEL 06
    discard: call 08e0 ()
    if (1) is false then
        jump-to: LABEL 1f
LABEL 07
    discard: call 08e0 ()
    if (0) is true then
        jump-to: LABEL 1f
LABEL 08
    discard: call 08e0 ()
    if (1) is false then
        jump-to: LABEL 1f
LABEL 09
    discard: call 08e0 ()
    if (0001) is false then
        jump-to: LABEL 1f
LABEL 0a
    discard: call 08e0 ()
    if (0001) is false then
        jump-to: LABEL 1f
LABEL 0b
    discard: call 08e0 ()
    if (0001) is false then
        jump-to: LABEL 1f
LABEL 0c
    discard: call 08e0 ()
    if (0000) is true then
        jump-to: LABEL 1f
LABEL 0d
    discard: call 08e0 ()
    print: "jg"
    if (int16(05) > int16(05)) is true then
        jump-to: LABEL 1f
LABEL 0e
    discard: call 08e0 ()
    if (int16(01) > int16(00)) is false then
        jump-to: LABEL 1f
LABEL 0f
    discard: call 08e0 ()
    if (int16(00) > int16(01)) is true then
        jump-to: LABEL 1f
LABEL 10
    discard: call 08e0 ()
    if (int16(ffff) > int16(fffe)) is false then
        jump-to: LABEL 1f
LABEL 11
    discard: call 08e0 ()
    if (int16(fffe) > int16(ffff)) is true then
        jump-to: LABEL 1f
LABEL 12
    discard: call 08e0 ()
    if (int16(01) > int16(ffff)) is false then
        jump-to: LABEL 1f
LABEL 13
    discard: call 08e0 ()
    if (int16(ffff) > int16(01)) is true then
        jump-to: LABEL 1f
LABEL 14
    discard: call 08e0 ()
    print: "jl"
    if (int16(05) < int16(05)) is true then
        jump-to: LABEL 1f
LABEL 15
    discard: call 08e0 ()
    if (int16(01) < int16(00)) is true then
        jump-to: LABEL 1f
LABEL 16
    discard: call 08e0 ()
    if (int16(00) < int16(01)) is false then
        jump-to: LABEL 1f
LABEL 17
    discard: call 08e0 ()
    if (int16(ffff) < int16(fffe)) is true then
        jump-to: LABEL 1f
LABEL 18
    discard: call 08e0 ()
    if (int16(fffe) < int16(ffff)) is false then
        jump-to: LABEL 1f
LABEL 19
    discard: call 08e0 ()
    if (int16(01) < int16(ffff)) is true then
        jump-to: LABEL 1f
LABEL 1a
    discard: call 08e0 ()
    if (int16(ffff) < int16(01)) is false then
        jump-to: LABEL 1f
LABEL 1b
    discard: call 08e0 ()
    print: "jz"
    if (1) is false then
        jump-to: LABEL 1f
LABEL 1c
    discard: call 08e0 ()
    if (0) is true then
        jump-to: LABEL 1f
LABEL 1d
    discard: call 08e0 ()
    if (0) is true then
        jump-to: LABEL 1f
LABEL 1e
    discard: call 08e0 ()
    print: "offsets"
    L01 <- call 0a8c (00)
    temp03 <- L01
    discard: call 07e4 (temp03, 00, 0b1f)
    L01 <- call 0a8c (01)
    temp04 <- L01
    discard: call 07e4 (temp04, 01, 0b21)
    return: 1
LABEL 1f
    print: "\nbad ["
    temp05 <- read-word(04f2)
    print: number-to-text(int16(temp05))
    print: "]!\n"
    print: "Quitting tests because jumps don't work!"
    quit
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
# temps: 60

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
    temp01 <- read-word(04f2)
    push-SP: (int16(temp01) + int16(01))
    temp02 <- pop-SP
    print: number-to-text(int16(temp02))
    print: "]: "
    print: "push/pull"
    push-SP: 09
    push-SP: 08
    L01 <- pop-SP
    temp03 <- L01
    discard: call 07e4 (temp03, 08, 0b23)
    write-word(04fa) <- pop-SP
    temp04 <- read-word(04fa)
    discard: call 07e4 (temp04, 09, 0b26)
    print: "store"
    L01 <- 05
    temp05 <- L01
    discard: call 07e4 (temp05, 05)
    print: "load"
    L02 <- 05
    L01 <- 06
    temp06 <- L01
    temp07 <- temp06
    push-SP: temp07
    L02 <- pop-SP
    temp08 <- L01
    temp09 <- L02
    discard: call 07e4 (temp08, temp09)
    print: "dec"
    discard: call 0cc0 (05, 04)
    discard: call 0cc0 (00, ffff)
    discard: call 0cc0 (fff8, fff7)
    discard: call 0cc0 (8000, 7fff)
    push-SP: 01
    push-SP: 0a
    temp0a <- peek-SP
    update-SP: (int16(temp0a) - int16(1))
    L02 <- pop-SP
    temp0b <- L02
    discard: call 07e4 (temp0b, 09, 0b29)
    L02 <- pop-SP
    temp0c <- L02
    discard: call 07e4 (temp0c, 01, 0b2a)
    write-word(04f0) <- 03
    temp0d <- read-word(04f0)
    write-word(04f0) <- (int16(temp0d) - int16(1))
    temp0e <- read-word(04f0)
    discard: call 07e4 (temp0e, 02, 0b2b)
    print: "inc"
    discard: call 0cb0 (05, 06)
    discard: call 0cb0 (ffff, 00)
    discard: call 0cb0 (fff8, fff9)
    discard: call 0cb0 (7fff, 8000)
    push-SP: 01
    push-SP: 0a
    temp0f <- peek-SP
    update-SP: (int16(temp0f) + int16(1))
    L02 <- pop-SP
    temp10 <- L02
    discard: call 07e4 (temp10, 0b, 0b2d)
    L02 <- pop-SP
    temp11 <- L02
    discard: call 07e4 (temp11, 01, 0b2e)
    write-word(04f0) <- 03
    temp12 <- read-word(04f0)
    write-word(04f0) <- (int16(temp12) + int16(1))
    temp13 <- read-word(04f0)
    discard: call 07e4 (temp13, 04, 0b2f)
    print: "\n    dec_chk"
    L02 <- 03
    temp14 <- L02
    temp15 <- (int16(temp14) - int16(1))
    L02 <- temp15
    if (int16(temp15) < int16(03e8)) is false then
        jump-to: LABEL 0c
LABEL 03
    discard: call 08e0 ()
    temp16 <- L02
    temp17 <- (int16(temp16) - int16(1))
    L02 <- temp17
    if (int16(temp17) < int16(01)) is true then
        jump-to: LABEL 0c
LABEL 04
    discard: call 08e0 ()
    temp18 <- L02
    temp19 <- (int16(temp18) - int16(1))
    L02 <- temp19
    if (int16(temp19) < int16(01)) is false then
        jump-to: LABEL 0c
LABEL 05
    discard: call 08e0 ()
    temp1a <- L02
    temp1b <- (int16(temp1a) - int16(1))
    L02 <- temp1b
    if (int16(temp1b) < int16(00)) is false then
        jump-to: LABEL 0c
LABEL 06
    discard: call 08e0 ()
    temp1c <- L02
    temp1d <- (int16(temp1c) - int16(1))
    L02 <- temp1d
    if (int16(temp1d) < int16(fffe)) is true then
        jump-to: LABEL 0c
LABEL 07
    discard: call 08e0 ()
    temp1e <- L02
    temp1f <- (int16(temp1e) - int16(1))
    L02 <- temp1f
    if (int16(temp1f) < int16(fffe)) is false then
        jump-to: LABEL 0c
LABEL 08
    discard: call 08e0 ()
    temp20 <- L02
    temp21 <- (int16(temp20) - int16(1))
    L02 <- temp21
    if (int16(temp21) < int16(03e8)) is false then
        jump-to: LABEL 0c
LABEL 09
    discard: call 08e0 ()
    temp22 <- L02
    temp23 <- (int16(temp22) - int16(1))
    L02 <- temp23
    if (int16(temp23) < int16(fe0c)) is true then
        jump-to: LABEL 0c
LABEL 0a
    discard: call 08e0 ()
    push-SP: 01
    push-SP: 0a
    temp24 <- peek-SP
    temp25 <- (int16(temp24) - int16(1))
    update-SP: temp25
    if (int16(temp25) < int16(05)) is true then
        jump-to: LABEL 0c
LABEL 0b
    discard: call 08e0 ()
    L02 <- pop-SP
    temp26 <- L02
    discard: call 07e4 (temp26, 09, 0b31)
    L02 <- pop-SP
    temp27 <- L02
    discard: call 07e4 (temp27, 01, 0b33)
    jump-to: LABEL 0d
LABEL 0c
    print: "\nbad ["
    temp28 <- read-word(04f2)
    print: number-to-text(int16(temp28))
    print: "]\n"
    discard: call 08ec ()
LABEL 0d
    print: "inc_chk"
    L02 <- fffa
    temp29 <- L02
    temp2a <- (int16(temp29) + int16(1))
    L02 <- temp2a
    if (int16(temp2a) > int16(fe0c)) is false then
        jump-to: LABEL 17
LABEL 0e
    discard: call 08e0 ()
    temp2b <- L02
    temp2c <- (int16(temp2b) + int16(1))
    L02 <- temp2c
    if (int16(temp2c) > int16(03e8)) is true then
        jump-to: LABEL 17
LABEL 0f
    discard: call 08e0 ()
    temp2d <- L02
    temp2e <- (int16(temp2d) + int16(1))
    L02 <- temp2e
    if (int16(temp2e) > int16(fffd)) is true then
        jump-to: LABEL 17
LABEL 10
    discard: call 08e0 ()
    temp2f <- L02
    temp30 <- (int16(temp2f) + int16(1))
    L02 <- temp30
    if (int16(temp30) > int16(fffd)) is false then
        jump-to: LABEL 17
LABEL 11
    discard: call 08e0 ()
    temp31 <- L02
    temp32 <- (int16(temp31) + int16(1))
    L02 <- temp32
    if (int16(temp32) > int16(00)) is true then
        jump-to: LABEL 17
LABEL 12
    discard: call 08e0 ()
    temp33 <- L02
    temp34 <- (int16(temp33) + int16(1))
    L02 <- temp34
    if (int16(temp34) > int16(01)) is true then
        jump-to: LABEL 17
LABEL 13
    discard: call 08e0 ()
    temp35 <- L02
    temp36 <- (int16(temp35) + int16(1))
    L02 <- temp36
    if (int16(temp36) > int16(01)) is true then
        jump-to: LABEL 17
LABEL 14
    discard: call 08e0 ()
    temp37 <- L02
    temp38 <- (int16(temp37) + int16(1))
    L02 <- temp38
    if (int16(temp38) > int16(01)) is false then
        jump-to: LABEL 17
LABEL 15
    discard: call 08e0 ()
    temp39 <- L02
    temp3a <- (int16(temp39) + int16(1))
    L02 <- temp3a
    if (int16(temp3a) > int16(03e8)) is true then
        jump-to: LABEL 17
LABEL 16
    discard: call 08e0 ()
    jump-to: LABEL 18
LABEL 17
    print: "\nbad ["
    temp3b <- read-word(04f2)
    print: number-to-text(int16(temp3b))
    print: "]!\n"
    discard: call 08ec ()
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
    temp04 <- (temp03 + 018c)
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
    write-word(04fc) <- temp12
    temp13 <- L01
    write-word(04fe) <- temp13
    temp14 <- L03
    temp15 <- L02
    discard: call 0884 (temp14, temp15, 0b6b)
    return: 1
]]>

        Test(CZech, &H1AC8, expected)
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
    write-word(04f2) <- 00
    write-word(04f4) <- 00
    write-word(04f6) <- 00
    write-word(04f8) <- 00
    print: "CZECH: the Comprehensive Z-machine Emulation CHecker, version "
    print: read-text(2c78)
    print: "\nTest numbers appear in [brackets].\n"
    discard: call 08d8 ()
    print: "\nprint works or you wouldn't be seeing this.\n\n"
    discard: call 08f4 (00)
    print: "\n"
    discard: call 0aa4 (00)
    print: "\n"
    discard: call 0cd0 (00)
    print: "\n"
    discard: call 0ee0 (00)
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
    temp00 <- read-word(04f2)
    print: number-to-text(int16(temp00))
    print: " tests.\n"
    print: "Passed: "
    temp01 <- read-word(04f4)
    print: number-to-text(int16(temp01))
    print: ", Failed: "
    temp02 <- read-word(04f6)
    print: number-to-text(int16(temp02))
    print: ", Print tests: "
    temp03 <- read-word(04f8)
    print: number-to-text(int16(temp03))
    print: "\n"
    temp04 <- read-word(04f4)
    temp05 <- read-word(04f6)
    push-SP: (int16(temp04) + int16(temp05))
    temp06 <- pop-SP
    temp07 <- read-word(04f8)
    push-SP: (int16(temp06) + int16(temp07))
    temp08 <- pop-SP
    temp09 <- read-word(04f2)
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
    temp03 <- (temp02 + 02ee)
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
    push-SP: read-word(temp00)
    temp01 <- pop-SP
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
    push-SP: read-word(temp02 + (temp03 * 2))
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
# temps: 25

LABEL 00
    temp00 <- L00
    L01 <- read-word(temp00)
    temp01 <- L00
    L02 <- read-word(temp01 + 0002)
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
    push-SP: (int16(temp07) - int16(temp08))
    temp09 <- pop-SP
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
    push-SP: read-word(temp0c + 0002)
    temp0d <- L05
    temp0e <- L03
    temp0f <- pop-SP
    write-word(temp0d + (temp0e * 2)) <- temp0f
    temp10 <- L05
    temp11 <- L04
    write-word(temp10 + 0002) <- temp11
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
    temp01 <- read-byte(0904)
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
    push-SP: call 0666 (983b)
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
# temps: 99

LABEL 00
    push-SP: call 5472 (8010, ffff)
    temp00 <- pop-SP
    write-word(temp00) <- 01
    push-SP: call 5472 (807c, ffff)
    push-SP: call 5472 (80f0, ffff)
    temp01 <- pop-SP
    write-word(temp01) <- 01
    push-SP: call 5472 (6f6a, 28)
    push-SP: call 5472 (6f55, c8)
    temp02 <- read-word(0868)
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
    write-word(temp0d + 0002) <- temp0e
    temp0f <- read-word(2291)
    push-SP: (int16(temp0f) + int16(04))
    temp10 <- read-word(2285)
    temp11 <- pop-SP
    write-word(temp10 + 0004) <- temp11
    temp12 <- read-word(228d)
    push-SP: (int16(temp12) + int16(02))
    temp13 <- read-word(2283)
    temp14 <- pop-SP
    write-word(temp13 + 0004) <- temp14
    temp15 <- read-word(228d)
    push-SP: (int16(temp15) + int16(04))
    temp16 <- read-word(2283)
    temp17 <- pop-SP
    write-word(temp16 + 0006) <- temp17
    temp18 <- read-word(228b)
    push-SP: (int16(temp18) + int16(02))
    temp19 <- read-word(2281)
    temp1a <- pop-SP
    write-word(temp19 + 0002) <- temp1a
    temp1b <- read-word(2289)
    push-SP: (int16(temp1b) + int16(02))
    temp1c <- read-word(2281)
    temp1d <- pop-SP
    write-word(temp1c + 0006) <- temp1d
    write-word(2271) <- b4
    push-SP: call 9530 (a0)
    temp1e <- read-word(2271)
    temp1f <- (temp1e - 1)
    temp20 <- (temp1f * 9)
    temp21 <- (temp20 + 02ee)
    temp22 <- temp21
    temp23 <- temp22
    temp24 <- read-byte(temp23)
    temp25 <- (temp24 & 0010)
    temp26 <- (temp25 <> 0)
    if (temp26 = 1) is true then
        jump-to: LABEL 0b
LABEL 0a
    push-SP: call 6ee0 ()
    print: "\n"
LABEL 0b
    write-word(22f5) <- 01
    write-word(234f) <- 04
    temp27 <- read-word(234f)
    write-word(2371) <- temp27
    temp28 <- read-word(234f)
    temp29 <- read-word(2271)
    temp2a <- 0
    temp2b <- (temp28 - 1)
    temp2c <- (temp2b * 9)
    temp2d <- (temp2c + 02ee)
    temp2e <- (temp2d + 5)
    temp2f <- read-byte(temp2e)
    temp30 <- (temp28 - 1)
    temp31 <- (temp30 * 9)
    temp32 <- (temp31 + 02ee)
    temp33 <- (temp32 + 4)
    temp34 <- read-byte(temp33)
    if (temp34 = 0) is false then
        jump-to: LABEL 0d
LABEL 0c
    temp35 <- 0
    jump-to: LABEL 0e
LABEL 0d
    temp36 <- (temp34 - 1)
    temp37 <- (temp36 * 9)
    temp38 <- (temp37 + 02ee)
    temp39 <- (temp38 + 6)
    temp3a <- read-byte(temp39)
    temp35 <- temp3a
LABEL 0e
    if (temp35 <> temp28) is false then
        jump-to: LABEL 14
LABEL 0f
    temp3b <- temp35
LABEL 10
    temp3c <- (temp3b - 1)
    temp3d <- (temp3c * 9)
    temp3e <- (temp3d + 02ee)
    temp3f <- (temp3e + 5)
    temp40 <- read-byte(temp3f)
    temp41 <- temp40
    if (temp41 = temp28) is false then
        jump-to: LABEL 12
LABEL 11
    temp2a <- temp3b
    temp3b <- 0
    jump-to: LABEL 13
LABEL 12
    temp3b <- temp41
LABEL 13
    if (temp3b <> 0) is true then
        jump-to: LABEL 10
LABEL 14
    if (temp2a <> 0) is false then
        jump-to: LABEL 16
LABEL 15
    temp42 <- (temp2a - 1)
    temp43 <- (temp42 * 9)
    temp44 <- (temp43 + 02ee)
    temp45 <- (temp44 + 5)
    write-byte(temp45) <- temp2f
LABEL 16
    if (temp35 = temp28) is false then
        jump-to: LABEL 18
LABEL 17
    temp46 <- (temp34 - 1)
    temp47 <- (temp46 * 9)
    temp48 <- (temp47 + 02ee)
    temp49 <- (temp48 + 6)
    write-byte(temp49) <- temp2f
LABEL 18
    temp4a <- (temp28 - 1)
    temp4b <- (temp4a * 9)
    temp4c <- (temp4b + 02ee)
    temp4d <- (temp4c + 4)
    write-byte(temp4d) <- 0
    temp4e <- (temp28 - 1)
    temp4f <- (temp4e * 9)
    temp50 <- (temp4f + 02ee)
    temp51 <- (temp50 + 5)
    write-byte(temp51) <- 0
    if (temp29 <> 0) is false then
        jump-to: LABEL 1a
LABEL 19
    temp52 <- (temp28 - 1)
    temp53 <- (temp52 * 9)
    temp54 <- (temp53 + 02ee)
    temp55 <- (temp54 + 4)
    write-byte(temp55) <- temp29
    temp56 <- (temp29 - 1)
    temp57 <- (temp56 * 9)
    temp58 <- (temp57 + 02ee)
    temp59 <- (temp58 + 6)
    temp5a <- read-byte(temp59)
    temp5b <- (temp28 - 1)
    temp5c <- (temp5b * 9)
    temp5d <- (temp5c + 02ee)
    temp5e <- (temp5d + 5)
    write-byte(temp5e) <- temp5a
    temp5f <- (temp29 - 1)
    temp60 <- (temp5f * 9)
    temp61 <- (temp60 + 02ee)
    temp62 <- (temp61 + 6)
    write-byte(temp62) <- temp28
LABEL 1a
    push-SP: call 7e04 ()
    push-SP: call 552a ()
    jump-to: LABEL 00]]>

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
# temps: 206

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
    L04 <- read-word(temp0a + 0002)
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
    L04 <- read-word(temp0e + 0002)
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
    L04 <- read-word(temp14 + 0002)
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
    push-SP: read-byte(temp1a)
    temp1b <- pop-SP
    push-SP: (temp1b & 03)
    temp1c <- pop-SP
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
    push-SP: read-word(temp3a + 000c)
    temp3b <- pop-SP
    push-SP: read-word(temp3b)
    temp3c <- pop-SP
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
    push-SP: read-word(temp44 + 000c)
    temp45 <- pop-SP
    push-SP: read-word(temp45)
    temp46 <- pop-SP
    temp47 <- (temp46 = 3b7c)
    if (temp47) is false then
        jump-to: LABEL 37
LABEL 35
    temp48 <- read-word(235d)
    temp49 <- read-word(235f)
    temp4a <- (temp48 - 1)
    temp4b <- (temp4a * 9)
    temp4c <- (temp4b + 02ee)
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
    temp56 <- (temp55 + 02ee)
    temp57 <- (temp56 + 4)
    temp58 <- read-byte(temp57)
    push-SP: temp58
    temp59 <- pop-SP
    temp5a <- read-word(234f)
    temp5b <- read-word(2271)
    temp5c <- (temp59 = temp5a)
    temp5d <- (temp59 = temp5b)
    if (temp5c | temp5d) is true then
        jump-to: LABEL 3b
LABEL 3a
    temp5e <- L08
    temp5f <- (temp5e - 1)
    temp60 <- (temp5f * 9)
    temp61 <- (temp60 + 02ee)
    temp62 <- (temp61 + 4)
    temp63 <- read-byte(temp62)
    push-SP: temp63
    temp64 <- pop-SP
    temp65 <- (temp64 - 1)
    temp66 <- (temp65 * 9)
    temp67 <- (temp66 + 02ee)
    temp68 <- (temp67 + 0001)
    temp69 <- temp68
    temp6a <- read-byte(temp69)
    temp6b <- (temp6a & 0020)
    temp6c <- (temp6b <> 0)
    if (temp6c = 1) is false then
        jump-to: LABEL 19
LABEL 3b
    temp6d <- L08
    temp6e <- (temp6d - 1)
    temp6f <- (temp6e * 9)
    temp70 <- (temp6f + 02ee)
    temp71 <- (temp70 + 0002)
    temp72 <- temp71
    temp73 <- read-byte(temp72)
    temp74 <- (temp73 & 0040)
    temp75 <- (temp74 <> 0)
    if (temp75 = 1) is true then
        jump-to: LABEL 3e
LABEL 3c
    temp76 <- L08
    temp77 <- (temp76 - 1)
    temp78 <- (temp77 * 9)
    temp79 <- (temp78 + 02ee)
    temp7a <- (temp79 + 0001)
    temp7b <- temp7a
    temp7c <- read-byte(temp7b)
    temp7d <- (temp7c & 0004)
    temp7e <- (temp7d <> 0)
    if (temp7e = 1) is true then
        jump-to: LABEL 3e
LABEL 3d
    jump-to: LABEL 19
LABEL 3e
    temp7f <- L08
    temp80 <- (temp7f = 0c)
    if (temp80) is false then
        jump-to: LABEL 40
LABEL 3f
    temp81 <- read-word(2347)
    temp82 <- (temp81 - 1)
    temp83 <- (temp82 * 9)
    temp84 <- (temp83 + 02ee)
    temp85 <- (temp84 + 7)
    temp86 <- read-word(temp85)
    temp87 <- read-byte(temp86)
    temp88 <- (temp86 + 1)
    print: read-text(temp88, temp87)
    jump-to: LABEL 41
LABEL 40
    temp89 <- L08
    temp8a <- (temp89 - 1)
    temp8b <- (temp8a * 9)
    temp8c <- (temp8b + 02ee)
    temp8d <- (temp8c + 7)
    temp8e <- read-word(temp8d)
    temp8f <- read-byte(temp8e)
    temp90 <- (temp8e + 1)
    print: read-text(temp90, temp8f)
LABEL 41
    print: ": "
LABEL 42
    L09 <- 01
    temp91 <- read-word(2361)
    temp92 <- read-word(235d)
    temp93 <- read-word(235f)
    L06 <- call 577c (temp91, temp92, temp93)
    temp94 <- L06
    temp95 <- (temp94 = 02)
    if (temp95) is false then
        jump-to: LABEL 19
LABEL 43
    jump-to: LABEL 44
LABEL 44
    temp96 <- L06
    temp97 <- (temp96 = 02)
    if (temp97) is true then
        jump-to: LABEL 52
LABEL 45
    temp98 <- read-word(234f)
    temp99 <- (temp98 - 1)
    temp9a <- (temp99 * 9)
    temp9b <- (temp9a + 02ee)
    temp9c <- (temp9b + 4)
    temp9d <- read-byte(temp9c)
    push-SP: temp9d
    temp9e <- pop-SP
    temp9f <- (temp9e - 1)
    tempa0 <- (temp9f * 9)
    tempa1 <- (tempa0 + 02ee)
    tempa2 <- (tempa1 + 7)
    tempa3 <- read-word(tempa2)
    tempa4 <- read-byte(tempa3)
    tempa5 <- (tempa3 + 1)
    tempa6 <- (tempa4 * 2)
    tempa7 <- (tempa5 + tempa6)
    tempa8 <- uint16(tempa7)
    tempa9 <- 0
LABEL 46
    tempaa <- read-byte(tempa8)
    if ((tempaa & 1f) <= 11) is false then
        jump-to: LABEL 48
LABEL 47
    tempa9 <- 1
    jump-to: LABEL 49
LABEL 48
    tempab <- read-byte(tempa8)
    tempac <- ((tempa8 + 1) + ((tempab >> 5) + 1))
    tempa8 <- uint16(tempac)
LABEL 49
    if (tempa9 = 0) is true then
        jump-to: LABEL 46
    if ((tempaa & 1f) = 11) is false then
        jump-to: LABEL 4e
LABEL 4a
    tempa8 <- (tempa8 + 1)
    if ((tempaa & e0) = 0) is false then
        jump-to: LABEL 4c
LABEL 4b
    tempad <- read-byte(tempa8)
    jump-to: LABEL 4d
LABEL 4c
    tempad <- read-word(tempa8)
LABEL 4d
    jump-to: LABEL 4f
LABEL 4e
    tempae <- read-word(02d0)
    tempad <- uint16(tempae)
LABEL 4f
    push-SP: tempad
    tempaf <- pop-SP
    if (tempaf = 0) is false then
        jump-to: LABEL 51
LABEL 50
    L06 <- 0
    jump-to: LABEL 52
LABEL 51
    L06 <- call (tempaf * 2) (06)
LABEL 52
    tempb0 <- read-word(2361)
    tempb1 <- (tempb0 = 08)
    tempb2 <- (tempb0 = 89)
    tempb3 <- (tempb0 = 0f)
    if ((tempb1 | tempb2) | tempb3) is true then
        jump-to: LABEL 56
LABEL 53
    tempb4 <- read-word(2361)
    tempb5 <- (tempb4 = 0c)
    tempb6 <- (tempb4 = 09)
    tempb7 <- (tempb4 = 07)
    if ((tempb5 | tempb6) | tempb7) is false then
        jump-to: LABEL 55
LABEL 54
    jump-to: LABEL 56
LABEL 55
    tempb8 <- read-word(2361)
    write-word(236d) <- tempb8
    tempb9 <- read-word(235d)
    write-word(236b) <- tempb9
    tempba <- read-word(235f)
    write-word(2369) <- tempba
LABEL 56
    tempbb <- L06
    tempbc <- (tempbb = 02)
    if (tempbc) is false then
        jump-to: LABEL 59
LABEL 57
    write-word(2349) <- 00
    jump-to: LABEL 59
LABEL 58
    write-word(2349) <- 00
LABEL 59
    tempbd <- read-word(236f)
    if (tempbd = 0) is true then
        jump-to: LABEL 00
LABEL 5a
    tempbe <- read-word(2361)
    tempbf <- (tempbe = 02)
    tempc0 <- (tempbe = 01)
    tempc1 <- (tempbe = 6f)
    if ((tempbf | tempc0) | tempc1) is true then
        jump-to: LABEL 00
LABEL 5b
    tempc2 <- read-word(2361)
    tempc3 <- (tempc2 = 0c)
    tempc4 <- (tempc2 = 08)
    tempc5 <- (tempc2 = 00)
    if ((tempc3 | tempc4) | tempc5) is true then
        jump-to: LABEL 00
LABEL 5c
    tempc6 <- read-word(2361)
    tempc7 <- (tempc6 = 09)
    tempc8 <- (tempc6 = 06)
    tempc9 <- (tempc6 = 05)
    if ((tempc7 | tempc8) | tempc9) is true then
        jump-to: LABEL 00
LABEL 5d
    tempca <- read-word(2361)
    tempcb <- (tempca = 07)
    tempcc <- (tempca = 0b)
    tempcd <- (tempca = 0a)
    if ((tempcb | tempcc) | tempcd) is false then
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
# temps: 170

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
    L01 <- pop-SP
    temp03 <- read-word(22f5)
    if (temp03 = 0) is false then
        jump-to: LABEL 07
LABEL 04
    print: "It is pitch black."
    temp04 <- read-word(22f3)
    if (temp04 = 0) is false then
        jump-to: LABEL 06
LABEL 05
    print: " You are likely to be eaten by a grue."
LABEL 06
    print: "\n"
    push-SP: call 50d0 ()
    return: 00
LABEL 07
    temp05 <- read-word(2271)
    temp06 <- (temp05 - 1)
    temp07 <- (temp06 * 9)
    temp08 <- (temp07 + 02ee)
    temp09 <- temp08
    temp0a <- temp09
    temp0b <- read-byte(temp0a)
    temp0c <- (temp0b & 0010)
    temp0d <- (temp0c <> 0)
    if (temp0d = 1) is true then
        jump-to: LABEL 09
LABEL 08
    temp0e <- read-word(2271)
    temp0f <- (temp0e - 1)
    temp10 <- (temp0f * 9)
    temp11 <- (temp10 + 02ee)
    temp12 <- temp11
    temp13 <- temp12
    temp14 <- read-byte(temp13)
    write-byte(temp13) <- byte(temp14 | 0010)
    L01 <- 01
LABEL 09
    temp15 <- read-word(2271)
    temp16 <- (temp15 - 1)
    temp17 <- (temp16 * 9)
    temp18 <- (temp17 + 02ee)
    temp19 <- temp18
    temp1a <- temp19
    temp1b <- read-byte(temp1a)
    temp1c <- (temp1b & 0004)
    temp1d <- (temp1c <> 0)
    if (temp1d = 1) is false then
        jump-to: LABEL 0b
LABEL 0a
    temp1e <- read-word(2271)
    temp1f <- (temp1e - 1)
    temp20 <- (temp1f * 9)
    temp21 <- (temp20 + 02ee)
    temp22 <- temp21
    temp23 <- temp22
    temp24 <- read-byte(temp23)
    write-byte(temp23) <- byte(temp24 & not 0010)
LABEL 0b
    temp25 <- read-word(2271)
    temp26 <- (temp25 - 1)
    temp27 <- (temp26 * 9)
    temp28 <- (temp27 + 02ee)
    temp29 <- (temp28 + 4)
    temp2a <- read-byte(temp29)
    if (temp2a = 52) is false then
        jump-to: LABEL 0f
LABEL 0c
    temp2b <- read-word(2271)
    temp2c <- (temp2b - 1)
    temp2d <- (temp2c * 9)
    temp2e <- (temp2d + 02ee)
    temp2f <- (temp2e + 7)
    temp30 <- read-word(temp2f)
    temp31 <- read-byte(temp30)
    temp32 <- (temp30 + 1)
    print: read-text(temp32, temp31)
    temp33 <- read-word(234f)
    temp34 <- (temp33 - 1)
    temp35 <- (temp34 * 9)
    temp36 <- (temp35 + 02ee)
    temp37 <- (temp36 + 4)
    temp38 <- read-byte(temp37)
    L03 <- temp38
    temp39 <- L03
    temp3a <- (temp39 - 1)
    temp3b <- (temp3a * 9)
    temp3c <- (temp3b + 02ee)
    temp3d <- (temp3c + 0003)
    temp3e <- temp3d
    temp3f <- read-byte(temp3e)
    temp40 <- (temp3f & 0010)
    temp41 <- (temp40 <> 0)
    if (temp41 = 1) is false then
        jump-to: LABEL 0e
LABEL 0d
    print: ", in the "
    temp42 <- L03
    temp43 <- (temp42 - 1)
    temp44 <- (temp43 * 9)
    temp45 <- (temp44 + 02ee)
    temp46 <- (temp45 + 7)
    temp47 <- read-word(temp46)
    temp48 <- read-byte(temp47)
    temp49 <- (temp47 + 1)
    print: read-text(temp49, temp48)
LABEL 0e
    print: "\n"
LABEL 0f
    temp4a <- L00
    if (temp4a = 0) is false then
        jump-to: LABEL 11
LABEL 10
    temp4b <- read-word(22fd)
    if (temp4b = 0) is false then
        return: 1
LABEL 11
    temp4c <- read-word(234f)
    temp4d <- (temp4c - 1)
    temp4e <- (temp4d * 9)
    temp4f <- (temp4e + 02ee)
    temp50 <- (temp4f + 4)
    temp51 <- read-byte(temp50)
    L03 <- temp51
    temp52 <- L01
    if (temp52 = 0) is true then
        jump-to: LABEL 20
LABEL 12
    temp53 <- read-word(2271)
    temp54 <- (temp53 - 1)
    temp55 <- (temp54 * 9)
    temp56 <- (temp55 + 02ee)
    temp57 <- (temp56 + 7)
    temp58 <- read-word(temp57)
    temp59 <- read-byte(temp58)
    temp5a <- (temp58 + 1)
    temp5b <- (temp59 * 2)
    temp5c <- (temp5a + temp5b)
    temp5d <- uint16(temp5c)
    temp5e <- 0
LABEL 13
    temp5f <- read-byte(temp5d)
    if ((temp5f & 1f) <= 11) is false then
        jump-to: LABEL 15
LABEL 14
    temp5e <- 1
    jump-to: LABEL 16
LABEL 15
    temp60 <- read-byte(temp5d)
    temp61 <- ((temp5d + 1) + ((temp60 >> 5) + 1))
    temp5d <- uint16(temp61)
LABEL 16
    if (temp5e = 0) is true then
        jump-to: LABEL 13
    if ((temp5f & 1f) = 11) is false then
        jump-to: LABEL 1b
LABEL 17
    temp5d <- (temp5d + 1)
    if ((temp5f & e0) = 0) is false then
        jump-to: LABEL 19
LABEL 18
    temp62 <- read-byte(temp5d)
    jump-to: LABEL 1a
LABEL 19
    temp62 <- read-word(temp5d)
LABEL 1a
    jump-to: LABEL 1c
LABEL 1b
    temp63 <- read-word(02d0)
    temp62 <- uint16(temp63)
LABEL 1c
    push-SP: temp62
    temp64 <- pop-SP
    if (temp64 = 0) is false then
        jump-to: LABEL 1e
LABEL 1d
    push-SP: 0
    jump-to: LABEL 1f
LABEL 1e
    push-SP: call (temp64 * 2) (03)
LABEL 1f
    temp65 <- pop-SP
    if (temp65 = 0) is false then
        return: 1
LABEL 20
    temp66 <- L01
    if (temp66 = 0) is true then
        jump-to: LABEL 2d
LABEL 21
    temp67 <- read-word(2271)
    temp68 <- (temp67 - 1)
    temp69 <- (temp68 * 9)
    temp6a <- (temp69 + 02ee)
    temp6b <- (temp6a + 7)
    temp6c <- read-word(temp6b)
    temp6d <- read-byte(temp6c)
    temp6e <- (temp6c + 1)
    temp6f <- (temp6d * 2)
    temp70 <- (temp6e + temp6f)
    temp71 <- uint16(temp70)
    temp72 <- 0
LABEL 22
    temp73 <- read-byte(temp71)
    if ((temp73 & 1f) <= 0b) is false then
        jump-to: LABEL 24
LABEL 23
    temp72 <- 1
    jump-to: LABEL 25
LABEL 24
    temp74 <- read-byte(temp71)
    temp75 <- ((temp71 + 1) + ((temp74 >> 5) + 1))
    temp71 <- uint16(temp75)
LABEL 25
    if (temp72 = 0) is true then
        jump-to: LABEL 22
    if ((temp73 & 1f) = 0b) is false then
        jump-to: LABEL 2a
LABEL 26
    temp71 <- (temp71 + 1)
    if ((temp73 & e0) = 0) is false then
        jump-to: LABEL 28
LABEL 27
    temp76 <- read-byte(temp71)
    jump-to: LABEL 29
LABEL 28
    temp76 <- read-word(temp71)
LABEL 29
    jump-to: LABEL 2b
LABEL 2a
    temp77 <- read-word(02c4)
    temp76 <- uint16(temp77)
LABEL 2b
    L02 <- temp76
    temp78 <- L02
    if (temp78 = 0) is true then
        jump-to: LABEL 2d
LABEL 2c
    temp79 <- L02
    print: read-text(temp79 * 2)
    print: "\n"
    jump-to: LABEL 3a
LABEL 2d
    temp7a <- read-word(2271)
    temp7b <- (temp7a - 1)
    temp7c <- (temp7b * 9)
    temp7d <- (temp7c + 02ee)
    temp7e <- (temp7d + 7)
    temp7f <- read-word(temp7e)
    temp80 <- read-byte(temp7f)
    temp81 <- (temp7f + 1)
    temp82 <- (temp80 * 2)
    temp83 <- (temp81 + temp82)
    temp84 <- uint16(temp83)
    temp85 <- 0
LABEL 2e
    temp86 <- read-byte(temp84)
    if ((temp86 & 1f) <= 11) is false then
        jump-to: LABEL 30
LABEL 2f
    temp85 <- 1
    jump-to: LABEL 31
LABEL 30
    temp87 <- read-byte(temp84)
    temp88 <- ((temp84 + 1) + ((temp87 >> 5) + 1))
    temp84 <- uint16(temp88)
LABEL 31
    if (temp85 = 0) is true then
        jump-to: LABEL 2e
    if ((temp86 & 1f) = 11) is false then
        jump-to: LABEL 36
LABEL 32
    temp84 <- (temp84 + 1)
    if ((temp86 & e0) = 0) is false then
        jump-to: LABEL 34
LABEL 33
    temp89 <- read-byte(temp84)
    jump-to: LABEL 35
LABEL 34
    temp89 <- read-word(temp84)
LABEL 35
    jump-to: LABEL 37
LABEL 36
    temp8a <- read-word(02d0)
    temp89 <- uint16(temp8a)
LABEL 37
    push-SP: temp89
    temp8b <- pop-SP
    if (temp8b = 0) is false then
        jump-to: LABEL 39
LABEL 38
    push-SP: 0
    jump-to: LABEL 3a
LABEL 39
    push-SP: call (temp8b * 2) (04)
LABEL 3a
    temp8c <- read-word(2271)
    temp8d <- L03
    temp8e <- (temp8c = temp8d)
    if (temp8e) is true then
        return: 1
LABEL 3b
    temp8f <- L03
    temp90 <- (temp8f - 1)
    temp91 <- (temp90 * 9)
    temp92 <- (temp91 + 02ee)
    temp93 <- (temp92 + 0003)
    temp94 <- temp93
    temp95 <- read-byte(temp94)
    temp96 <- (temp95 & 0010)
    temp97 <- (temp96 <> 0)
    if (temp97 = 1) is false then
        return: 1
LABEL 3c
    temp98 <- L03
    temp99 <- (temp98 - 1)
    temp9a <- (temp99 * 9)
    temp9b <- (temp9a + 02ee)
    temp9c <- (temp9b + 7)
    temp9d <- read-word(temp9c)
    temp9e <- read-byte(temp9d)
    temp9f <- (temp9d + 1)
    tempa0 <- (temp9e * 2)
    tempa1 <- (temp9f + tempa0)
    tempa2 <- uint16(tempa1)
    tempa3 <- 0
LABEL 3d
    tempa4 <- read-byte(tempa2)
    if ((tempa4 & 1f) <= 11) is false then
        jump-to: LABEL 3f
LABEL 3e
    tempa3 <- 1
    jump-to: LABEL 40
LABEL 3f
    tempa5 <- read-byte(tempa2)
    tempa6 <- ((tempa2 + 1) + ((tempa5 >> 5) + 1))
    tempa2 <- uint16(tempa6)
LABEL 40
    if (tempa3 = 0) is true then
        jump-to: LABEL 3d
    if ((tempa4 & 1f) = 11) is false then
        jump-to: LABEL 45
LABEL 41
    tempa2 <- (tempa2 + 1)
    if ((tempa4 & e0) = 0) is false then
        jump-to: LABEL 43
LABEL 42
    tempa7 <- read-byte(tempa2)
    jump-to: LABEL 44
LABEL 43
    tempa7 <- read-word(tempa2)
LABEL 44
    jump-to: LABEL 46
LABEL 45
    tempa8 <- read-word(02d0)
    tempa7 <- uint16(tempa8)
LABEL 46
    push-SP: tempa7
    tempa9 <- pop-SP
    if (tempa9 = 0) is false then
        jump-to: LABEL 48
LABEL 47
    push-SP: 0
    jump-to: LABEL 49
LABEL 48
    push-SP: call (tempa9 * 2) (03)
LABEL 49
    return: 1]]>

        Test(Zork1, &H8C9A, expected)
    End Sub

    Private Sub Test(gameName As String, address As Integer, expected As XCData)
        Dim memory = GameMemory(gameName)
        Dim reader = New RoutineReader(memory)

        Dim a = RawAddress(address)
        Dim r = reader.ReadRoutine(a)

        Assert.Equal(a, r.Address)

        Dim binder = New RoutineBinder(memory, debugging:=False)
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
