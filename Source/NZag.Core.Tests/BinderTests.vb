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
    if ((temp0c & 80) <> 80) is false then
        jump-to: LABEL 03
LABEL 02
    temp0d <- (temp0c >> 6)
    jump-to: LABEL 06
LABEL 03
    temp0e <- (temp0b + 1)
    if ((read-byte(temp0e) & 3f) = 0) is false then
        jump-to: LABEL 05
LABEL 04
    temp0d <- 40
    jump-to: LABEL 06
LABEL 05
    temp0d <- (read-byte(temp0e) & 3f)
LABEL 06
    temp0f <- ((temp0b + 1) + (temp0d + 1))
    temp0b <- uint16(temp0f)
    temp10 <- read-byte(temp0b)
    if ((temp10 & 3f) < temp01) is true then
        jump-to: LABEL 07
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
# temps: 30

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
    RUNTIME EXCEPTION: Unsupported opcode: put_prop (v.3) with 3 operands
    temp02 <- read-word(2291)
    push-SP: (int16(temp02) + int16(02))
    temp03 <- read-word(2285)
    temp04 <- pop-SP
    write-word(temp03 + 0002) <- temp04
    temp05 <- read-word(2291)
    push-SP: (int16(temp05) + int16(04))
    temp06 <- read-word(2285)
    temp07 <- pop-SP
    write-word(temp06 + 0004) <- temp07
    temp08 <- read-word(228d)
    push-SP: (int16(temp08) + int16(02))
    temp09 <- read-word(2283)
    temp0a <- pop-SP
    write-word(temp09 + 0004) <- temp0a
    temp0b <- read-word(228d)
    push-SP: (int16(temp0b) + int16(04))
    temp0c <- read-word(2283)
    temp0d <- pop-SP
    write-word(temp0c + 0006) <- temp0d
    temp0e <- read-word(228b)
    push-SP: (int16(temp0e) + int16(02))
    temp0f <- read-word(2281)
    temp10 <- pop-SP
    write-word(temp0f + 0002) <- temp10
    temp11 <- read-word(2289)
    push-SP: (int16(temp11) + int16(02))
    temp12 <- read-word(2281)
    temp13 <- pop-SP
    write-word(temp12 + 0006) <- temp13
    write-word(2271) <- b4
    push-SP: call 9530 (a0)
    temp14 <- read-word(2271)
    temp15 <- (temp14 - 1)
    temp16 <- (temp15 * 9)
    temp17 <- (temp16 + 02ee)
    temp18 <- temp17
    temp19 <- temp18
    temp1a <- read-byte(temp19)
    temp1b <- (temp1a & 0010)
    temp1c <- (temp1b <> 0)
    if (temp1c = 1) is true then
        jump-to: LABEL 02
LABEL 01
    push-SP: call 6ee0 ()
    print: "\n"
LABEL 02
    write-word(22f5) <- 01
    write-word(234f) <- 04
    temp1d <- read-word(234f)
    write-word(2371) <- temp1d
    RUNTIME EXCEPTION: Unsupported opcode: insert_obj (v.3) with 2 operands
    push-SP: call 7e04 ()
    push-SP: call 552a ()
    jump-to: LABEL 00]]>

        Test(Zork1, &H4F04, expected)
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
