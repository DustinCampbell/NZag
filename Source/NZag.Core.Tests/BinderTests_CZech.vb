Imports Xunit

Public Class BinderTests_CZech

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
# temps: 4

LABEL 00
    temp00 <- L00
    temp01 <- L01
    temp02 <- L02
    if (temp01 = temp00) is true then
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
    print: read-text(temp02 * 4)
    print: ")"
LABEL 03
    discard: call 8ec ()
    print: " Expected "
    print: number-to-text(int16(temp01))
    print: "; got "
    print: number-to-text(int16(temp00))
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
# temps: 4

LABEL 00
    temp00 <- L00
    print: "Jumps"
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
    temp03 <- call a8c (00)
    discard: call 7e4 (temp03, 00, 0b1f)
    temp03 <- call a8c (01)
    discard: call 7e4 (temp03, 01, 0b21)
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
# temps: 19

LABEL 00
    temp00 <- L00
    print: "Variables"
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
    discard: call 7e4 (temp03, 08, 0b23)
    temp04 <- pop-SP
    write-word(4fa) <- temp04
    temp05 <- read-word(4fa)
    discard: call 7e4 (temp05, 09, 0b26)
    print: "store"
    discard: call 7e4 (05, 05)
    print: "load"
    discard: call 7e4 (06, 06)
    print: "dec"
    discard: call cc0 (05, 04)
    discard: call cc0 (00, ffff)
    discard: call cc0 (fff8, fff7)
    discard: call cc0 (8000, 7fff)
    push-SP: 01
    push-SP: 0a
    temp06 <- peek-SP
    update-SP: (int16(temp06) - int16(1))
    temp07 <- pop-SP
    discard: call 7e4 (temp07, 09, 0b29)
    temp08 <- pop-SP
    discard: call 7e4 (temp08, 01, 0b2a)
    write-word(4f0) <- 03
    write-word(4f0) <- (int16(read-word(4f0)) - int16(1))
    temp09 <- read-word(4f0)
    discard: call 7e4 (temp09, 02, 0b2b)
    print: "inc"
    discard: call cb0 (05, 06)
    discard: call cb0 (ffff, 00)
    discard: call cb0 (fff8, fff9)
    discard: call cb0 (7fff, 8000)
    push-SP: 01
    push-SP: 0a
    temp0a <- peek-SP
    update-SP: (int16(temp0a) + int16(1))
    temp0b <- pop-SP
    discard: call 7e4 (temp0b, 0b, 0b2d)
    temp0c <- pop-SP
    discard: call 7e4 (temp0c, 01, 0b2e)
    write-word(4f0) <- 03
    write-word(4f0) <- (int16(read-word(4f0)) + int16(1))
    temp0d <- read-word(4f0)
    discard: call 7e4 (temp0d, 04, 0b2f)
    print: "\n    dec_chk"
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    push-SP: 01
    push-SP: 0a
    temp0e <- peek-SP
    temp0f <- (int16(temp0e) - int16(1))
    update-SP: temp0f
    if (int16(temp0f) < int16(05)) is true then
        jump-to: LABEL 04
LABEL 03
    discard: call 8e0 ()
    temp10 <- pop-SP
    discard: call 7e4 (temp10, 09, 0b31)
    temp11 <- pop-SP
    discard: call 7e4 (temp11, 01, 0b33)
    jump-to: LABEL 05
LABEL 04
    print: "\nbad ["
    temp12 <- read-word(4f2)
    print: number-to-text(int16(temp12))
    print: "]\n"
    discard: call 8ec ()
LABEL 05
    print: "inc_chk"
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    discard: call 8e0 ()
    return: 1
]]>

        TestBinder(CZech, &HAA4, expected)
    End Sub

#End Region
#Region "CZech_1150"

#Region "ZCode"
    '1151:  b2 ...                  print           "^^^"
    '1156:  b2 ...                  print           "Print opcodes"
    '1161:  a0 01 ca                jz              local0 116c
    '1164:  b2 ...                  print           " skipped"
    '116b:  b1                      rfalse          
    '116c:  b2 ...                  print           " ["
    '1171:  54 11 01 00             add             g01 #01 -> sp
    '1175:  e6 bf 00                print_num       sp
    '1178:  b2 ...                  print           "]: "
    '117f:  b2 ...                  print           "Tests should look like... '[Test] opcode (stuff): stuff'"
    '11b0:  b2 ...                  print           "^print_num (0, 1, -1, 32767,-32768, -1): "
    '11df:  8f 02 36                call_1n         8d8
    '11e2:  e6 7f 00                print_num       #00
    '11e5:  b2 ...                  print           ", "
    '11e8:  0d 02 01                store           local1 #01
    '11eb:  e6 bf 02                print_num       local1
    '11ee:  b2 ...                  print           ", "
    '11f1:  8f 02 36                call_1n         8d8
    '11f4:  e6 3f ff ff             print_num       #ffff
    '11f8:  b2 ...                  print           ", "
    '11fb:  8f 02 36                call_1n         8d8
    '11fe:  e6 3f 7f ff             print_num       #7fff
    '1202:  b2 ...                  print           ", "
    '1205:  8f 02 36                call_1n         8d8
    '1208:  e6 3f 80 00             print_num       #8000
    '120c:  b2 ...                  print           ", "
    '120f:  8f 02 36                call_1n         8d8
    '1212:  cd 4f 02 ff ff          store           local1 #ffff
    '1217:  e6 bf 02                print_num       local1
    '121a:  8f 02 36                call_1n         8d8
    '121d:  b2 ...                  print           "^["
    '1222:  54 11 01 00             add             g01 #01 -> sp
    '1226:  e6 bf 00                print_num       sp
    '1229:  b2 ...                  print           "] "
    '122e:  b2 ...                  print           "print_char (abcd): "
    '123f:  e5 7f 61                print_char      'a'
    '1242:  8f 02 36                call_1n         8d8
    '1245:  e5 7f 62                print_char      'b'
    '1248:  8f 02 36                call_1n         8d8
    '124b:  0d 02 63                store           local1 #63
    '124e:  e5 bf 02                print_char      local1
    '1251:  8f 02 36                call_1n         8d8
    '1254:  e8 7f 64                push            #64
    '1257:  8f 02 36                call_1n         8d8
    '125a:  e5 bf 00                print_char      sp
    '125d:  b2 ...                  print           "^["
    '1262:  54 11 01 00             add             g01 #01 -> sp
    '1266:  e6 bf 00                print_num       sp
    '1269:  b2 ...                  print           "] "
    '126e:  b2 ...                  print           "new_line:^"
    '1279:  8f 02 36                call_1n         8d8
    '127c:  bb                      new_line        
    '127d:  b2 ...                  print           "There should be an empty line above this line.^"
    '12a0:  88 05 a4 02             call_1s         1690 -> local1
    '12a4:  8f 02 36                call_1n         8d8
    '12a7:  f9 27 01 f9 02 01       call_vn         7e4 local1 #01
    '12ad:  b2 ...                  print           "^print_addr (Hello.): "
    '12c2:  8f 02 36                call_1n         8d8
    '12c5:  e1 13 06 d0 00 11 aa    storew          #06d0 #00 #11aa
    '12cc:  e1 13 06 d0 01 46 34    storew          #06d0 #01 #4634
    '12d3:  e1 13 06 d0 02 16 45    storew          #06d0 #02 #1645
    '12da:  e1 13 06 d0 03 9c a5    storew          #06d0 #03 #9ca5
    '12e1:  87 06 d0                print_addr      #06d0
    '12e4:  b2 ...                  print           "^print_paddr (A long string that Inform will put in high memory):^"
    '1317:  8f 02 36                call_1n         8d8
    '131a:  8d 0b 52                print_paddr     s029
    '131d:  b2 ...                  print           "^Abbreviations (I love 'xyz"
    '1334:  b2 ...                  print           "zy' [two times]): "
    '1347:  8f 02 36                call_1n         8d8
    '134a:  cd 4f 02 0b 5b          store           local1 s030
    '134f:  ad 02                   print_paddr     local1
    '1351:  8f 02 36                call_1n         8d8
    '1354:  b2 ...                  print           " I love 'xyzzy'^"
    '1361:  b2 ...                  print           "^["
    '1366:  54 11 01 00             add             g01 #01 -> sp
    '136a:  e6 bf 00                print_num       sp
    '136d:  b2 ...                  print           "] "
    '1372:  b2 ...                  print           "print_obj (Test Object #1Test Object #2): "
    '1397:  8f 02 36                call_1n         8d8
    '139a:  8a 00 05                print_obj       "Test Object #1"
    '139d:  8f 02 36                call_1n         8d8
    '13a0:  8a 00 06                print_obj       "Test Object #2"
    '13a3:  b0                      rtrue           
#End Region

    <Fact>
    Sub CZech_1150()
        Dim expected =
        <![CDATA[
# temps: 13

LABEL 00
    temp00 <- L00
    print: "\n\n\n"
    print: "Print opcodes"
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
    print: "Tests should look like... '[Test] opcode (stuff): stuff'"
    print: "\nprint_num (0, 1, -1, 32767,-32768, -1): "
    discard: call 8d8 ()
    print: number-to-text(int16(00))
    print: ", "
    print: number-to-text(int16(01))
    print: ", "
    discard: call 8d8 ()
    print: number-to-text(int16(ffff))
    print: ", "
    discard: call 8d8 ()
    print: number-to-text(int16(7fff))
    print: ", "
    discard: call 8d8 ()
    print: number-to-text(int16(8000))
    print: ", "
    discard: call 8d8 ()
    print: number-to-text(int16(ffff))
    discard: call 8d8 ()
    print: "\n["
    temp03 <- read-word(4f2)
    temp04 <- (int16(temp03) + int16(01))
    print: number-to-text(int16(temp04))
    print: "] "
    print: "print_char (abcd): "
    print: 61
    discard: call 8d8 ()
    print: 62
    discard: call 8d8 ()
    print: 63
    discard: call 8d8 ()
    push-SP: 64
    discard: call 8d8 ()
    temp05 <- pop-SP
    print: temp05
    print: "\n["
    temp06 <- read-word(4f2)
    temp07 <- (int16(temp06) + int16(01))
    print: number-to-text(int16(temp07))
    print: "] "
    print: "new_line:\n"
    discard: call 8d8 ()
    print: "\n"
    print: "There should be an empty line above this line.\n"
    temp08 <- call 1690 ()
    discard: call 8d8 ()
    discard: call 7e4 (temp08, 01)
    print: "\nprint_addr (Hello.): "
    discard: call 8d8 ()
    write-word(6d0) <- 11aa
    write-word(6d2) <- 4634
    write-word(6d4) <- 1645
    write-word(6d6) <- 9ca5
    print: read-text(06d0)
    print: "\nprint_paddr (A long string that Inform will put in high memory):\n"
    discard: call 8d8 ()
    print: read-text(2d48)
    print: "\nAbbreviations (I love 'xyz"
    print: "zy' [two times]): "
    discard: call 8d8 ()
    print: read-text(2d6c)
    discard: call 8d8 ()
    print: " I love 'xyzzy'\n"
    print: "\n["
    temp09 <- read-word(4f2)
    temp0a <- (int16(temp09) + int16(01))
    print: number-to-text(int16(temp0a))
    print: "] "
    print: "print_obj (Test Object #1Test Object #2): "
    discard: call 8d8 ()
    temp0b <- read-word(1d0)
    print: read-text(temp0b + 1, read-byte(temp0b))
    discard: call 8d8 ()
    temp0c <- read-word(1de)
    print: read-text(temp0c + 1, read-byte(temp0c))
    return: 1
]]>

        TestBinder(CZech, &H1150, expected)
    End Sub

#End Region
#Region "CZech_13A4"

#Region "ZCode"
    '13a5:  b2 ...                  print           "Subroutines"
    '13ae:  a0 01 ca                jz              local0 13b9
    '13b1:  b2 ...                  print           " skipped"
    '13b8:  b1                      rfalse          
    '13b9:  b2 ...                  print           " ["
    '13be:  54 11 01 00             add             g01 #01 -> sp
    '13c2:  e6 bf 00                print_num       sp
    '13c5:  b2 ...                  print           "]: "
    '13cc:  0d 02 00                store           local1 #00
    '13cf:  0d 15 00                store           g05 #00
    '13d2:  b2 ...                  print           "call_1s"
    '13d9:  0d 15 02                store           g05 #02
    '13dc:  88 05 90 02             call_1s         1640 -> local1
    '13e0:  f9 27 01 f9 15 03       call_vn         7e4 g05 #03
    '13e6:  b2 ...                  print           "call_2s"
    '13ed:  d9 1f 05 92 06 02       call_2s         1648 #06 -> local1
    '13f3:  f9 27 01 f9 02 05       call_vn         7e4 local1 #05
    '13f9:  b2 ...                  print           "call_vs2"
    '1402:  0d 02 00                store           local1 #00
    '1405:  ec 15 55 05 95 01 02 03 04 05 06 07 02 
    '                              call_vs2        1654 #01 #02 #03 #04 #05 #06 #07 -> local1
    '1412:  f9 27 01 f9 15 09       call_vn         7e4 g05 #09
    '1418:  f9 27 01 f9 02 05       call_vn         7e4 local1 #05
    '141e:  b2 ...                  print           "call_vs"
    '1425:  0d 02 00                store           local1 #00
    '1428:  e0 15 05 9c 01 02 03 02 call_vs         1670 #01 #02 #03 -> local1
    '1430:  f9 27 01 f9 02 05       call_vn         7e4 local1 #05
    '1436:  f9 27 01 f9 15 07       call_vn         7e4 g05 #07
    '143c:  b2 ...                  print           "ret"
    '143f:  f9 27 01 f9 02 05       call_vn         7e4 local1 #05
    '1445:  b2 ...                  print           "^    call_1n"
    '1450:  8f 05 80                call_1n         1600
    '1453:  f9 27 01 f9 15 01       call_vn         7e4 g05 #01
    '1459:  b2 ...                  print           "call_2n"
    '1460:  da 1f 05 82 06          call_2n         1608 #06
    '1465:  f9 27 01 f9 15 05       call_vn         7e4 g05 #05
    '146b:  b2 ...                  print           "call_vn"
    '1472:  f9 15 05 85 01 02 03    call_vn         1614 #01 #02 #03
    '1479:  f9 27 01 f9 15 0a       call_vn         7e4 g05 #0a
    '147f:  b2 ...                  print           "call_vn2"
    '1488:  fa 15 55 05 89 01 02 03 04 05 06 07 
    '                              call_vn2        1624 #01 #02 #03 #04 #05 #06 #07
    '1494:  f9 27 01 f9 15 0b       call_vn         7e4 g05 #0b
    '149a:  b2 ...                  print           "^    "
    '149f:  b2 ...                  print           "rtrue"
    '14a4:  0d 02 02                store           local1 #02
    '14a7:  88 05 a2 02             call_1s         1688 -> local1
    '14ab:  f9 27 01 f9 02 01       call_vn         7e4 local1 #01
    '14b1:  0d 02 02                store           local1 #02
    '14b4:  b2 ...                  print           "rfalse"
    '14b9:  88 05 a3 02             call_1s         168c -> local1
    '14bd:  f9 27 01 f9 02 00       call_vn         7e4 local1 #00
    '14c3:  88 05 ac 02             call_1s         16b0 -> local1
    '14c7:  f9 24 01 f9 02 05 0b 5e call_vn         7e4 local1 #05 s031
    '14cf:  b2 ...                  print           "^    Computed call"
    '14de:  0d 02 01                store           local1 #01
    '14e1:  cd 4f 03 05 b0          store           local2 #05b0
    '14e6:  a8 03 02                call_1s         local2 -> local1
    '14e9:  f9 27 01 f9 02 05       call_vn         7e4 local1 #05
    '14ef:  e8 7f 01                push            #01
    '14f2:  e8 3f 05 b1             push            #05b1
    '14f6:  a8 00 02                call_1s         sp -> local1
    '14f9:  f9 27 01 f9 02 06       call_vn         7e4 local1 #06
    '14ff:  e9 7f 02                pull            local1
    '1502:  f9 27 01 f9 02 01       call_vn         7e4 local1 #01
    '1508:  b2 ...                  print           "^    check_arg_count"
    '1519:  0d 10 00                store           g00 #00
    '151c:  8f 05 5d                call_1n         1574
    '151f:  0d 10 01                store           g00 #01
    '1522:  da 1f 05 5d 01          call_2n         1574 #01
    '1527:  0d 10 02                store           g00 #02
    '152a:  f9 17 05 5d 02 01       call_vn         1574 #02 #01
    '1530:  0d 10 03                store           g00 #03
    '1533:  f9 15 05 5d 03 02 01    call_vn         1574 #03 #02 #01
    '153a:  0d 10 04                store           g00 #04
    '153d:  fa 15 7f 05 5d 04 03 02 01 
    '                              call_vn2        1574 #04 #03 #02 #01
    '1546:  0d 10 05                store           g00 #05
    '1549:  fa 15 5f 05 5d 05 04 03 02 01 
    '                              call_vn2        1574 #05 #04 #03 #02 #01
    '1553:  0d 10 06                store           g00 #06
    '1556:  fa 15 57 05 5d 06 05 04 03 02 01 
    '                              call_vn2        1574 #06 #05 #04 #03 #02 #01
    '1561:  0d 10 07                store           g00 #07
    '1564:  fa 15 55 05 5d 07 06 05 04 03 02 01 
    '                              call_vn2        1574 #07 #06 #05 #04 #03 #02 #01
    '1570:  b0                      rtrue           
#End Region

    <Fact>
    Sub CZech_13A4()
        Dim expected =
        <![CDATA[
# temps: 13

LABEL 00
    temp00 <- L00
    print: "Subroutines"
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
    write-word(4fa) <- 00
    print: "call_1s"
    write-word(4fa) <- 02
    temp03 <- call 1640 ()
    temp04 <- read-word(4fa)
    discard: call 7e4 (temp04, 03)
    print: "call_2s"
    temp03 <- call 1648 (06)
    discard: call 7e4 (temp03, 05)
    print: "call_vs2"
    temp03 <- call 1654 (01, 02, 03, 04, 05, 06, 07)
    temp05 <- read-word(4fa)
    discard: call 7e4 (temp05, 09)
    discard: call 7e4 (temp03, 05)
    print: "call_vs"
    temp03 <- call 1670 (01, 02, 03)
    discard: call 7e4 (temp03, 05)
    temp06 <- read-word(4fa)
    discard: call 7e4 (temp06, 07)
    print: "ret"
    discard: call 7e4 (temp03, 05)
    print: "\n    call_1n"
    discard: call 1600 ()
    temp07 <- read-word(4fa)
    discard: call 7e4 (temp07, 01)
    print: "call_2n"
    discard: call 1608 (06)
    temp08 <- read-word(4fa)
    discard: call 7e4 (temp08, 05)
    print: "call_vn"
    discard: call 1614 (01, 02, 03)
    temp09 <- read-word(4fa)
    discard: call 7e4 (temp09, 0a)
    print: "call_vn2"
    discard: call 1624 (01, 02, 03, 04, 05, 06, 07)
    temp0a <- read-word(4fa)
    discard: call 7e4 (temp0a, 0b)
    print: "\n    "
    print: "rtrue"
    temp03 <- call 1688 ()
    discard: call 7e4 (temp03, 01)
    print: "rfalse"
    temp03 <- call 168c ()
    discard: call 7e4 (temp03, 00)
    temp03 <- call 16b0 ()
    discard: call 7e4 (temp03, 05, 0b5e)
    print: "\n    Computed call"
    temp03 <- call 16c0 ()
    discard: call 7e4 (temp03, 05)
    push-SP: 01
    push-SP: 05b1
    temp0b <- pop-SP
    if (temp0b = 0) is false then
        jump-to: LABEL 04
LABEL 03
    temp03 <- 0
    jump-to: LABEL 05
LABEL 04
    temp03 <- call (temp0b * 4) ()
LABEL 05
    discard: call 7e4 (temp03, 06)
    temp0c <- pop-SP
    discard: call 7e4 (temp0c, 01)
    print: "\n    check_arg_count"
    write-word(4f0) <- 00
    discard: call 1574 ()
    write-word(4f0) <- 01
    discard: call 1574 (01)
    write-word(4f0) <- 02
    discard: call 1574 (02, 01)
    write-word(4f0) <- 03
    discard: call 1574 (03, 02, 01)
    write-word(4f0) <- 04
    discard: call 1574 (04, 03, 02, 01)
    write-word(4f0) <- 05
    discard: call 1574 (05, 04, 03, 02, 01)
    write-word(4f0) <- 06
    discard: call 1574 (06, 05, 04, 03, 02, 01)
    write-word(4f0) <- 07
    discard: call 1574 (07, 06, 05, 04, 03, 02, 01)
    return: 1
]]>

        TestBinder(CZech, &H13A4, expected)
    End Sub

#End Region
#Region "CZech_1640"

#Region "ZCode"
    '1641:  0d 15 03                store           g05 #03
    '1644:  9b 05                   ret             #05
#End Region

    <Fact>
    Sub CZech_1640()
        Dim expected =
        <![CDATA[
# temps: 0

LABEL 00
    write-word(4fa) <- 03
    return: 05
]]>

        TestBinder(CZech, &H1640, expected)
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
# temps: 9

LABEL 00
    temp00 <- L00
    temp01 <- L01
    temp02 <- L02
    temp03 <- read-word((((temp00 - 1) * e) + 18c) + c)
    temp04 <- uint16((temp03 + 1) + (read-byte(temp03) * 2))
    if (temp01 <> 0) is false then
        jump-to: LABEL 07
LABEL 01
    temp05 <- read-byte(temp04)
    temp06 <- read-byte(temp04)
    if ((temp06 & 80) <> 80) is false then
        jump-to: LABEL 03
LABEL 02
    temp07 <- (temp06 >> 6)
    jump-to: LABEL 06
LABEL 03
    if ((read-byte(temp04 + 1) & 3f) = 0) is false then
        jump-to: LABEL 05
LABEL 04
    temp07 <- 40
    jump-to: LABEL 06
LABEL 05
    temp07 <- (read-byte(temp04 + 1) & 3f)
LABEL 06
    temp04 <- uint16((temp04 + 1) + (temp07 + 1))
    if ((temp05 & 3f) > temp01) is true then
        jump-to: LABEL 01
LABEL 07
    temp08 <- (read-byte(temp04) & 3f)
    write-word(4fc) <- temp00
    write-word(4fe) <- temp01
    discard: call 884 (temp08, temp02, 0b6b)
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
# temps: 144

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
    jump-to: LABEL 15b
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
    jump-to: LABEL 15b
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
    jump-to: LABEL 15b
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
    jump-to: LABEL 15b
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
    jump-to: LABEL 15b
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
    jump-to: LABEL 15b
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
    jump-to: LABEL 15b
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
    jump-to: LABEL 15b
LABEL 29
    temp15 <- read-word(6ce)
    if (temp15 = 0b) is false then
        jump-to: LABEL 30
LABEL 2a
    temp16 <- L03
    if (temp16 = 0) is false then
        jump-to: LABEL 2c
LABEL 2b
    update-SP: 53
    jump-to: LABEL 2f
LABEL 2c
    if (temp16 < 10) is false then
        jump-to: LABEL 2e
LABEL 2d
    L(temp16 - 1) <- 53
    jump-to: LABEL 2f
LABEL 2e
    write-word(((temp16 - 10) * 2) + 4f0) <- 53
LABEL 2f
    L0a <- 53
    L0b <- 2c
    L08 <- 0b99
    jump-to: LABEL 15b
LABEL 30
    temp17 <- read-word(6ce)
    if (temp17 = 0c) is false then
        jump-to: LABEL 37
LABEL 31
    temp18 <- L03
    if (temp18 = 0) is false then
        jump-to: LABEL 33
LABEL 32
    update-SP: 53
    jump-to: LABEL 36
LABEL 33
    if (temp18 < 10) is false then
        jump-to: LABEL 35
LABEL 34
    L(temp18 - 1) <- 53
    jump-to: LABEL 36
LABEL 35
    write-word(((temp18 - 10) * 2) + 4f0) <- 53
LABEL 36
    L0a <- 53
    L0b <- 2c
    L08 <- 0b9e
    jump-to: LABEL 15b
LABEL 37
    temp19 <- read-word(6ce)
    if (temp19 = 0d) is false then
        jump-to: LABEL 3e
LABEL 38
    temp1a <- L06
    if (temp1a = 0) is false then
        jump-to: LABEL 3a
LABEL 39
    update-SP: 53
    jump-to: LABEL 3d
LABEL 3a
    if (temp1a < 10) is false then
        jump-to: LABEL 3c
LABEL 3b
    L(temp1a - 1) <- 53
    jump-to: LABEL 3d
LABEL 3c
    write-word(((temp1a - 10) * 2) + 4f0) <- 53
LABEL 3d
    L09 <- 53
    L0a <- 2d
    L0b <- 2c
    L08 <- 0ba4
    jump-to: LABEL 15b
LABEL 3e
    temp1b <- read-word(6ce)
    if (temp1b = 0e) is false then
        jump-to: LABEL 45
LABEL 3f
    temp1c <- L06
    if (temp1c = 0) is false then
        jump-to: LABEL 41
LABEL 40
    update-SP: 53
    jump-to: LABEL 44
LABEL 41
    if (temp1c < 10) is false then
        jump-to: LABEL 43
LABEL 42
    L(temp1c - 1) <- 53
    jump-to: LABEL 44
LABEL 43
    write-word(((temp1c - 10) * 2) + 4f0) <- 53
LABEL 44
    L09 <- 53
    L0a <- 2d
    L0b <- 2c
    L08 <- 0ba9
    jump-to: LABEL 15b
LABEL 45
    temp1d <- read-word(6ce)
    if (temp1d = 0f) is false then
        jump-to: LABEL 47
LABEL 46
    temp1e <- pop-SP
    L01 <- temp1e
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0baf
    jump-to: LABEL 15b
LABEL 47
    temp1f <- read-word(6ce)
    if (temp1f = 10) is false then
        jump-to: LABEL 49
LABEL 48
    temp20 <- pop-SP
    update-SP: temp20
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bb2
    jump-to: LABEL 15b
LABEL 49
    temp21 <- read-word(6ce)
    if (temp21 = 11) is false then
        jump-to: LABEL 50
LABEL 4a
    temp22 <- L03
    temp23 <- pop-SP
    if (temp22 = 0) is false then
        jump-to: LABEL 4c
LABEL 4b
    update-SP: temp23
    jump-to: LABEL 4f
LABEL 4c
    if (temp22 < 10) is false then
        jump-to: LABEL 4e
LABEL 4d
    L(temp22 - 1) <- temp23
    jump-to: LABEL 4f
LABEL 4e
    write-word(((temp22 - 10) * 2) + 4f0) <- temp23
LABEL 4f
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bb4
    jump-to: LABEL 15b
LABEL 50
    temp24 <- read-word(6ce)
    if (temp24 = 12) is false then
        jump-to: LABEL 57
LABEL 51
    temp25 <- L06
    temp26 <- pop-SP
    if (temp25 = 0) is false then
        jump-to: LABEL 53
LABEL 52
    update-SP: temp26
    jump-to: LABEL 56
LABEL 53
    if (temp25 < 10) is false then
        jump-to: LABEL 55
LABEL 54
    L(temp25 - 1) <- temp26
    jump-to: LABEL 56
LABEL 55
    write-word(((temp25 - 10) * 2) + 4f0) <- temp26
LABEL 56
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bba
    jump-to: LABEL 15b
LABEL 57
    temp27 <- read-word(6ce)
    if (temp27 = 13) is false then
        jump-to: LABEL 5e
LABEL 58
    temp28 <- L06
    temp29 <- pop-SP
    if (temp28 = 0) is false then
        jump-to: LABEL 5a
LABEL 59
    update-SP: temp29
    jump-to: LABEL 5d
LABEL 5a
    if (temp28 < 10) is false then
        jump-to: LABEL 5c
LABEL 5b
    L(temp28 - 1) <- temp29
    jump-to: LABEL 5d
LABEL 5c
    write-word(((temp28 - 10) * 2) + 4f0) <- temp29
LABEL 5d
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bbf
    jump-to: LABEL 15b
LABEL 5e
    temp2a <- read-word(6ce)
    if (temp2a = 14) is false then
        jump-to: LABEL 60
LABEL 5f
    print: "\n    pull"
    temp2b <- pop-SP
    L01 <- temp2b
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bc5
    jump-to: LABEL 15b
LABEL 60
    temp2c <- read-word(6ce)
    if (temp2c = 15) is false then
        jump-to: LABEL 67
LABEL 61
    temp2d <- L06
    temp2e <- pop-SP
    if (temp2d = 0) is false then
        jump-to: LABEL 63
LABEL 62
    update-SP: temp2e
    jump-to: LABEL 66
LABEL 63
    if (temp2d < 10) is false then
        jump-to: LABEL 65
LABEL 64
    L(temp2d - 1) <- temp2e
    jump-to: LABEL 66
LABEL 65
    write-word(((temp2d - 10) * 2) + 4f0) <- temp2e
LABEL 66
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bc7
    jump-to: LABEL 15b
LABEL 67
    temp2f <- read-word(6ce)
    if (temp2f = 16) is false then
        jump-to: LABEL 6e
LABEL 68
    temp30 <- L06
    temp31 <- pop-SP
    if (temp30 = 0) is false then
        jump-to: LABEL 6a
LABEL 69
    update-SP: temp31
    jump-to: LABEL 6d
LABEL 6a
    if (temp30 < 10) is false then
        jump-to: LABEL 6c
LABEL 6b
    L(temp30 - 1) <- temp31
    jump-to: LABEL 6d
LABEL 6c
    write-word(((temp30 - 10) * 2) + 4f0) <- temp31
LABEL 6d
    L09 <- 2d
    L0a <- 2c
    L0b <- 2b
    L08 <- 0bcb
    jump-to: LABEL 15b
LABEL 6e
    temp32 <- read-word(6ce)
    if (temp32 = 17) is false then
        jump-to: LABEL 70
LABEL 6f
    temp33 <- pop-SP
    update-SP: temp33
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bd0
    jump-to: LABEL 15b
LABEL 70
    temp34 <- read-word(6ce)
    if (temp34 = 18) is false then
        jump-to: LABEL 77
LABEL 71
    temp35 <- L03
    temp36 <- pop-SP
    if (temp35 = 0) is false then
        jump-to: LABEL 73
LABEL 72
    update-SP: temp36
    jump-to: LABEL 76
LABEL 73
    if (temp35 < 10) is false then
        jump-to: LABEL 75
LABEL 74
    L(temp35 - 1) <- temp36
    jump-to: LABEL 76
LABEL 75
    write-word(((temp35 - 10) * 2) + 4f0) <- temp36
LABEL 76
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bd2
    jump-to: LABEL 15b
LABEL 77
    temp37 <- read-word(6ce)
    if (temp37 = 19) is false then
        jump-to: LABEL 7e
LABEL 78
    temp38 <- L03
    temp39 <- pop-SP
    if (temp38 = 0) is false then
        jump-to: LABEL 7a
LABEL 79
    update-SP: temp39
    jump-to: LABEL 7d
LABEL 7a
    if (temp38 < 10) is false then
        jump-to: LABEL 7c
LABEL 7b
    L(temp38 - 1) <- temp39
    jump-to: LABEL 7d
LABEL 7c
    write-word(((temp38 - 10) * 2) + 4f0) <- temp39
LABEL 7d
    L0a <- 2d
    L0b <- 2b
    L08 <- 0bd7
    jump-to: LABEL 15b
LABEL 7e
    temp3a <- read-word(6ce)
    if (temp3a = 1e) is false then
        jump-to: LABEL 80
LABEL 7f
    print: "inc"
    L01 <- (int16(L01) + int16(1))
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bdb
    jump-to: LABEL 15b
LABEL 80
    temp3b <- read-word(6ce)
    if (temp3b = 1f) is false then
        jump-to: LABEL 8c
LABEL 81
    temp3c <- L06
    if (temp3c = 0) is false then
        jump-to: LABEL 83
LABEL 82
    temp3d <- peek-SP
    jump-to: LABEL 86
LABEL 83
    if (temp3c < 10) is false then
        jump-to: LABEL 85
LABEL 84
    temp3d <- L(temp3c - 1)
    jump-to: LABEL 86
LABEL 85
    temp3d <- read-word(((temp3c - 10) * 2) + 4f0)
LABEL 86
    if (temp3c = 0) is false then
        jump-to: LABEL 88
LABEL 87
    update-SP: (int16(temp3d) + int16(1))
    jump-to: LABEL 8b
LABEL 88
    if (temp3c < 10) is false then
        jump-to: LABEL 8a
LABEL 89
    L(temp3c - 1) <- (int16(temp3d) + int16(1))
    jump-to: LABEL 8b
LABEL 8a
    write-word(((temp3c - 10) * 2) + 4f0) <- (int16(temp3d) + int16(1))
LABEL 8b
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bdf
    jump-to: LABEL 15b
LABEL 8c
    temp3e <- read-word(6ce)
    if (temp3e = 20) is false then
        jump-to: LABEL 98
LABEL 8d
    temp3f <- L06
    if (temp3f = 0) is false then
        jump-to: LABEL 8f
LABEL 8e
    temp40 <- peek-SP
    jump-to: LABEL 92
LABEL 8f
    if (temp3f < 10) is false then
        jump-to: LABEL 91
LABEL 90
    temp40 <- L(temp3f - 1)
    jump-to: LABEL 92
LABEL 91
    temp40 <- read-word(((temp3f - 10) * 2) + 4f0)
LABEL 92
    if (temp3f = 0) is false then
        jump-to: LABEL 94
LABEL 93
    update-SP: (int16(temp40) + int16(1))
    jump-to: LABEL 97
LABEL 94
    if (temp3f < 10) is false then
        jump-to: LABEL 96
LABEL 95
    L(temp3f - 1) <- (int16(temp40) + int16(1))
    jump-to: LABEL 97
LABEL 96
    write-word(((temp3f - 10) * 2) + 4f0) <- (int16(temp40) + int16(1))
LABEL 97
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    L08 <- 0be3
    jump-to: LABEL 15b
LABEL 98
    temp41 <- read-word(6ce)
    if (temp41 = 21) is false then
        jump-to: LABEL 9a
LABEL 99
    temp42 <- peek-SP
    update-SP: (int16(temp42) + int16(1))
    L0a <- 2e
    L0b <- 2c
    L08 <- 0be8
    jump-to: LABEL 15b
LABEL 9a
    temp43 <- read-word(6ce)
    if (temp43 = 22) is false then
        jump-to: LABEL a6
LABEL 9b
    temp44 <- L03
    if (temp44 = 0) is false then
        jump-to: LABEL 9d
LABEL 9c
    temp45 <- peek-SP
    jump-to: LABEL a0
LABEL 9d
    if (temp44 < 10) is false then
        jump-to: LABEL 9f
LABEL 9e
    temp45 <- L(temp44 - 1)
    jump-to: LABEL a0
LABEL 9f
    temp45 <- read-word(((temp44 - 10) * 2) + 4f0)
LABEL a0
    if (temp44 = 0) is false then
        jump-to: LABEL a2
LABEL a1
    update-SP: (int16(temp45) + int16(1))
    jump-to: LABEL a5
LABEL a2
    if (temp44 < 10) is false then
        jump-to: LABEL a4
LABEL a3
    L(temp44 - 1) <- (int16(temp45) + int16(1))
    jump-to: LABEL a5
LABEL a4
    write-word(((temp44 - 10) * 2) + 4f0) <- (int16(temp45) + int16(1))
LABEL a5
    L0a <- 2e
    L0b <- 2c
    L08 <- 0be9
    jump-to: LABEL 15b
LABEL a6
    temp46 <- read-word(6ce)
    if (temp46 = 23) is false then
        jump-to: LABEL b2
LABEL a7
    temp47 <- L03
    if (temp47 = 0) is false then
        jump-to: LABEL a9
LABEL a8
    temp48 <- peek-SP
    jump-to: LABEL ac
LABEL a9
    if (temp47 < 10) is false then
        jump-to: LABEL ab
LABEL aa
    temp48 <- L(temp47 - 1)
    jump-to: LABEL ac
LABEL ab
    temp48 <- read-word(((temp47 - 10) * 2) + 4f0)
LABEL ac
    if (temp47 = 0) is false then
        jump-to: LABEL ae
LABEL ad
    update-SP: (int16(temp48) + int16(1))
    jump-to: LABEL b1
LABEL ae
    if (temp47 < 10) is false then
        jump-to: LABEL b0
LABEL af
    L(temp47 - 1) <- (int16(temp48) + int16(1))
    jump-to: LABEL b1
LABEL b0
    write-word(((temp47 - 10) * 2) + 4f0) <- (int16(temp48) + int16(1))
LABEL b1
    L0a <- 2e
    L0b <- 2c
    L08 <- 0bed
    jump-to: LABEL 15b
LABEL b2
    temp49 <- read-word(6ce)
    if (temp49 = 28) is false then
        jump-to: LABEL b4
LABEL b3
    print: "dec"
    L01 <- (int16(L01) - int16(1))
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bf2
    jump-to: LABEL 15b
LABEL b4
    temp4a <- read-word(6ce)
    if (temp4a = 29) is false then
        jump-to: LABEL c0
LABEL b5
    temp4b <- L06
    if (temp4b = 0) is false then
        jump-to: LABEL b7
LABEL b6
    temp4c <- peek-SP
    jump-to: LABEL ba
LABEL b7
    if (temp4b < 10) is false then
        jump-to: LABEL b9
LABEL b8
    temp4c <- L(temp4b - 1)
    jump-to: LABEL ba
LABEL b9
    temp4c <- read-word(((temp4b - 10) * 2) + 4f0)
LABEL ba
    if (temp4b = 0) is false then
        jump-to: LABEL bc
LABEL bb
    update-SP: (int16(temp4c) - int16(1))
    jump-to: LABEL bf
LABEL bc
    if (temp4b < 10) is false then
        jump-to: LABEL be
LABEL bd
    L(temp4b - 1) <- (int16(temp4c) - int16(1))
    jump-to: LABEL bf
LABEL be
    write-word(((temp4b - 10) * 2) + 4f0) <- (int16(temp4c) - int16(1))
LABEL bf
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bf6
    jump-to: LABEL 15b
LABEL c0
    temp4d <- read-word(6ce)
    if (temp4d = 2a) is false then
        jump-to: LABEL cc
LABEL c1
    temp4e <- L06
    if (temp4e = 0) is false then
        jump-to: LABEL c3
LABEL c2
    temp4f <- peek-SP
    jump-to: LABEL c6
LABEL c3
    if (temp4e < 10) is false then
        jump-to: LABEL c5
LABEL c4
    temp4f <- L(temp4e - 1)
    jump-to: LABEL c6
LABEL c5
    temp4f <- read-word(((temp4e - 10) * 2) + 4f0)
LABEL c6
    if (temp4e = 0) is false then
        jump-to: LABEL c8
LABEL c7
    update-SP: (int16(temp4f) - int16(1))
    jump-to: LABEL cb
LABEL c8
    if (temp4e < 10) is false then
        jump-to: LABEL ca
LABEL c9
    L(temp4e - 1) <- (int16(temp4f) - int16(1))
    jump-to: LABEL cb
LABEL ca
    write-word(((temp4e - 10) * 2) + 4f0) <- (int16(temp4f) - int16(1))
LABEL cb
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    L08 <- 0bfa
    jump-to: LABEL 15b
LABEL cc
    temp50 <- read-word(6ce)
    if (temp50 = 2b) is false then
        jump-to: LABEL ce
LABEL cd
    temp51 <- peek-SP
    update-SP: (int16(temp51) - int16(1))
    L0a <- 2c
    L0b <- 2c
    L08 <- 0bff
    jump-to: LABEL 15b
LABEL ce
    temp52 <- read-word(6ce)
    if (temp52 = 2c) is false then
        jump-to: LABEL da
LABEL cf
    temp53 <- L03
    if (temp53 = 0) is false then
        jump-to: LABEL d1
LABEL d0
    temp54 <- peek-SP
    jump-to: LABEL d4
LABEL d1
    if (temp53 < 10) is false then
        jump-to: LABEL d3
LABEL d2
    temp54 <- L(temp53 - 1)
    jump-to: LABEL d4
LABEL d3
    temp54 <- read-word(((temp53 - 10) * 2) + 4f0)
LABEL d4
    if (temp53 = 0) is false then
        jump-to: LABEL d6
LABEL d5
    update-SP: (int16(temp54) - int16(1))
    jump-to: LABEL d9
LABEL d6
    if (temp53 < 10) is false then
        jump-to: LABEL d8
LABEL d7
    L(temp53 - 1) <- (int16(temp54) - int16(1))
    jump-to: LABEL d9
LABEL d8
    write-word(((temp53 - 10) * 2) + 4f0) <- (int16(temp54) - int16(1))
LABEL d9
    L0a <- 2c
    L0b <- 2c
    L08 <- 0c00
    jump-to: LABEL 15b
LABEL da
    temp55 <- read-word(6ce)
    if (temp55 = 2d) is false then
        jump-to: LABEL e6
LABEL db
    temp56 <- L03
    if (temp56 = 0) is false then
        jump-to: LABEL dd
LABEL dc
    temp57 <- peek-SP
    jump-to: LABEL e0
LABEL dd
    if (temp56 < 10) is false then
        jump-to: LABEL df
LABEL de
    temp57 <- L(temp56 - 1)
    jump-to: LABEL e0
LABEL df
    temp57 <- read-word(((temp56 - 10) * 2) + 4f0)
LABEL e0
    if (temp56 = 0) is false then
        jump-to: LABEL e2
LABEL e1
    update-SP: (int16(temp57) - int16(1))
    jump-to: LABEL e5
LABEL e2
    if (temp56 < 10) is false then
        jump-to: LABEL e4
LABEL e3
    L(temp56 - 1) <- (int16(temp57) - int16(1))
    jump-to: LABEL e5
LABEL e4
    write-word(((temp56 - 10) * 2) + 4f0) <- (int16(temp57) - int16(1))
LABEL e5
    L0a <- 2c
    L0b <- 2c
    L08 <- 0c04
    jump-to: LABEL 15b
LABEL e6
    temp58 <- read-word(6ce)
    if (temp58 = 32) is false then
        jump-to: LABEL e9
LABEL e7
    print: "\n    inc_chk"
    L08 <- 0c09
    temp59 <- (int16(L01) + int16(1))
    L01 <- temp59
    if (int16(temp59) > int16(48)) is true then
        jump-to: LABEL 15e
LABEL e8
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 15b
LABEL e9
    temp5a <- read-word(6ce)
    if (temp5a = 33) is false then
        jump-to: LABEL f6
LABEL ea
    L08 <- 0c0e
    temp5b <- L06
    if (temp5b = 0) is false then
        jump-to: LABEL ec
LABEL eb
    temp5c <- peek-SP
    jump-to: LABEL ef
LABEL ec
    if (temp5b < 10) is false then
        jump-to: LABEL ee
LABEL ed
    temp5c <- L(temp5b - 1)
    jump-to: LABEL ef
LABEL ee
    temp5c <- read-word(((temp5b - 10) * 2) + 4f0)
LABEL ef
    temp5d <- (int16(temp5c) + int16(1))
    if (temp5b = 0) is false then
        jump-to: LABEL f1
LABEL f0
    update-SP: temp5d
    jump-to: LABEL f4
LABEL f1
    if (temp5b < 10) is false then
        jump-to: LABEL f3
LABEL f2
    L(temp5b - 1) <- temp5d
    jump-to: LABEL f4
LABEL f3
    write-word(((temp5b - 10) * 2) + 4f0) <- temp5d
LABEL f4
    if (int16(temp5d) > int16(48)) is true then
        jump-to: LABEL 15e
LABEL f5
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 15b
LABEL f6
    temp5e <- read-word(6ce)
    if (temp5e = 34) is false then
        jump-to: LABEL 103
LABEL f7
    L08 <- 0c13
    temp5f <- L06
    if (temp5f = 0) is false then
        jump-to: LABEL f9
LABEL f8
    temp60 <- peek-SP
    jump-to: LABEL fc
LABEL f9
    if (temp5f < 10) is false then
        jump-to: LABEL fb
LABEL fa
    temp60 <- L(temp5f - 1)
    jump-to: LABEL fc
LABEL fb
    temp60 <- read-word(((temp5f - 10) * 2) + 4f0)
LABEL fc
    temp61 <- (int16(temp60) + int16(1))
    if (temp5f = 0) is false then
        jump-to: LABEL fe
LABEL fd
    update-SP: temp61
    jump-to: LABEL 101
LABEL fe
    if (temp5f < 10) is false then
        jump-to: LABEL 100
LABEL ff
    L(temp5f - 1) <- temp61
    jump-to: LABEL 101
LABEL 100
    write-word(((temp5f - 10) * 2) + 4f0) <- temp61
LABEL 101
    if (int16(temp61) > int16(48)) is true then
        jump-to: LABEL 15e
LABEL 102
    L09 <- 48
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 15b
LABEL 103
    temp62 <- read-word(6ce)
    if (temp62 = 35) is false then
        jump-to: LABEL 106
LABEL 104
    L08 <- 0c19
    temp63 <- peek-SP
    temp64 <- (int16(temp63) + int16(1))
    update-SP: temp64
    if (int16(temp64) > int16(2e)) is true then
        jump-to: LABEL 15e
LABEL 105
    L0a <- 2e
    L0b <- 2c
    jump-to: LABEL 15b
LABEL 106
    temp65 <- read-word(6ce)
    if (temp65 = 36) is false then
        jump-to: LABEL 113
LABEL 107
    L08 <- 0c1b
    temp66 <- L03
    if (temp66 = 0) is false then
        jump-to: LABEL 109
LABEL 108
    temp67 <- peek-SP
    jump-to: LABEL 10c
LABEL 109
    if (temp66 < 10) is false then
        jump-to: LABEL 10b
LABEL 10a
    temp67 <- L(temp66 - 1)
    jump-to: LABEL 10c
LABEL 10b
    temp67 <- read-word(((temp66 - 10) * 2) + 4f0)
LABEL 10c
    temp68 <- (int16(temp67) + int16(1))
    if (temp66 = 0) is false then
        jump-to: LABEL 10e
LABEL 10d
    update-SP: temp68
    jump-to: LABEL 111
LABEL 10e
    if (temp66 < 10) is false then
        jump-to: LABEL 110
LABEL 10f
    L(temp66 - 1) <- temp68
    jump-to: LABEL 111
LABEL 110
    write-word(((temp66 - 10) * 2) + 4f0) <- temp68
LABEL 111
    if (int16(temp68) > int16(2e)) is true then
        jump-to: LABEL 15e
LABEL 112
    L0a <- 2e
    L0b <- 2c
    jump-to: LABEL 15b
LABEL 113
    temp69 <- read-word(6ce)
    if (temp69 = 37) is false then
        jump-to: LABEL 120
LABEL 114
    L08 <- 0c20
    temp6a <- L03
    if (temp6a = 0) is false then
        jump-to: LABEL 116
LABEL 115
    temp6b <- peek-SP
    jump-to: LABEL 119
LABEL 116
    if (temp6a < 10) is false then
        jump-to: LABEL 118
LABEL 117
    temp6b <- L(temp6a - 1)
    jump-to: LABEL 119
LABEL 118
    temp6b <- read-word(((temp6a - 10) * 2) + 4f0)
LABEL 119
    temp6c <- (int16(temp6b) + int16(1))
    if (temp6a = 0) is false then
        jump-to: LABEL 11b
LABEL 11a
    update-SP: temp6c
    jump-to: LABEL 11e
LABEL 11b
    if (temp6a < 10) is false then
        jump-to: LABEL 11d
LABEL 11c
    L(temp6a - 1) <- temp6c
    jump-to: LABEL 11e
LABEL 11d
    write-word(((temp6a - 10) * 2) + 4f0) <- temp6c
LABEL 11e
    if (int16(temp6c) > int16(2e)) is true then
        jump-to: LABEL 15e
LABEL 11f
    L0a <- 2e
    L0b <- 2c
    jump-to: LABEL 15b
LABEL 120
    temp6d <- read-word(6ce)
    if (temp6d = 3c) is false then
        jump-to: LABEL 123
LABEL 121
    print: "dec_chk"
    L08 <- 0c26
    temp6e <- (int16(L01) - int16(1))
    L01 <- temp6e
    if (int16(temp6e) < int16(46)) is true then
        jump-to: LABEL 15e
LABEL 122
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 15b
LABEL 123
    temp6f <- read-word(6ce)
    if (temp6f = 3d) is false then
        jump-to: LABEL 130
LABEL 124
    L08 <- 0c2b
    temp70 <- L06
    if (temp70 = 0) is false then
        jump-to: LABEL 126
LABEL 125
    temp71 <- peek-SP
    jump-to: LABEL 129
LABEL 126
    if (temp70 < 10) is false then
        jump-to: LABEL 128
LABEL 127
    temp71 <- L(temp70 - 1)
    jump-to: LABEL 129
LABEL 128
    temp71 <- read-word(((temp70 - 10) * 2) + 4f0)
LABEL 129
    temp72 <- (int16(temp71) - int16(1))
    if (temp70 = 0) is false then
        jump-to: LABEL 12b
LABEL 12a
    update-SP: temp72
    jump-to: LABEL 12e
LABEL 12b
    if (temp70 < 10) is false then
        jump-to: LABEL 12d
LABEL 12c
    L(temp70 - 1) <- temp72
    jump-to: LABEL 12e
LABEL 12d
    write-word(((temp70 - 10) * 2) + 4f0) <- temp72
LABEL 12e
    if (int16(temp72) < int16(46)) is true then
        jump-to: LABEL 15e
LABEL 12f
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 15b
LABEL 130
    temp73 <- read-word(6ce)
    if (temp73 = 3e) is false then
        jump-to: LABEL 13d
LABEL 131
    L08 <- 0c30
    temp74 <- L06
    if (temp74 = 0) is false then
        jump-to: LABEL 133
LABEL 132
    temp75 <- peek-SP
    jump-to: LABEL 136
LABEL 133
    if (temp74 < 10) is false then
        jump-to: LABEL 135
LABEL 134
    temp75 <- L(temp74 - 1)
    jump-to: LABEL 136
LABEL 135
    temp75 <- read-word(((temp74 - 10) * 2) + 4f0)
LABEL 136
    temp76 <- (int16(temp75) - int16(1))
    if (temp74 = 0) is false then
        jump-to: LABEL 138
LABEL 137
    update-SP: temp76
    jump-to: LABEL 13b
LABEL 138
    if (temp74 < 10) is false then
        jump-to: LABEL 13a
LABEL 139
    L(temp74 - 1) <- temp76
    jump-to: LABEL 13b
LABEL 13a
    write-word(((temp74 - 10) * 2) + 4f0) <- temp76
LABEL 13b
    if (int16(temp76) < int16(46)) is true then
        jump-to: LABEL 15e
LABEL 13c
    L09 <- 46
    L0a <- 2d
    L0b <- 2c
    jump-to: LABEL 15b
LABEL 13d
    temp77 <- read-word(6ce)
    if (temp77 = 3f) is false then
        jump-to: LABEL 140
LABEL 13e
    L08 <- 0c36
    temp78 <- peek-SP
    temp79 <- (int16(temp78) - int16(1))
    update-SP: temp79
    if (int16(temp79) < int16(2c)) is true then
        jump-to: LABEL 15e
LABEL 13f
    L0a <- 2c
    L0b <- 2c
    jump-to: LABEL 15b
LABEL 140
    temp7a <- read-word(6ce)
    if (temp7a = 40) is false then
        jump-to: LABEL 14d
LABEL 141
    L08 <- 0c38
    temp7b <- L03
    if (temp7b = 0) is false then
        jump-to: LABEL 143
LABEL 142
    temp7c <- peek-SP
    jump-to: LABEL 146
LABEL 143
    if (temp7b < 10) is false then
        jump-to: LABEL 145
LABEL 144
    temp7c <- L(temp7b - 1)
    jump-to: LABEL 146
LABEL 145
    temp7c <- read-word(((temp7b - 10) * 2) + 4f0)
LABEL 146
    temp7d <- (int16(temp7c) - int16(1))
    if (temp7b = 0) is false then
        jump-to: LABEL 148
LABEL 147
    update-SP: temp7d
    jump-to: LABEL 14b
LABEL 148
    if (temp7b < 10) is false then
        jump-to: LABEL 14a
LABEL 149
    L(temp7b - 1) <- temp7d
    jump-to: LABEL 14b
LABEL 14a
    write-word(((temp7b - 10) * 2) + 4f0) <- temp7d
LABEL 14b
    if (int16(temp7d) < int16(2c)) is true then
        jump-to: LABEL 15e
LABEL 14c
    L0a <- 2c
    L0b <- 2c
    jump-to: LABEL 15b
LABEL 14d
    temp7e <- read-word(6ce)
    if (temp7e = 41) is false then
        jump-to: LABEL 15a
LABEL 14e
    L08 <- 0c3d
    temp7f <- L03
    if (temp7f = 0) is false then
        jump-to: LABEL 150
LABEL 14f
    temp80 <- peek-SP
    jump-to: LABEL 153
LABEL 150
    if (temp7f < 10) is false then
        jump-to: LABEL 152
LABEL 151
    temp80 <- L(temp7f - 1)
    jump-to: LABEL 153
LABEL 152
    temp80 <- read-word(((temp7f - 10) * 2) + 4f0)
LABEL 153
    temp81 <- (int16(temp80) - int16(1))
    if (temp7f = 0) is false then
        jump-to: LABEL 155
LABEL 154
    update-SP: temp81
    jump-to: LABEL 158
LABEL 155
    if (temp7f < 10) is false then
        jump-to: LABEL 157
LABEL 156
    L(temp7f - 1) <- temp81
    jump-to: LABEL 158
LABEL 157
    write-word(((temp7f - 10) * 2) + 4f0) <- temp81
LABEL 158
    if (int16(temp81) < int16(2c)) is true then
        jump-to: LABEL 15e
LABEL 159
    L0a <- 2c
    L0b <- 2c
    jump-to: LABEL 15b
LABEL 15a
    return: 0
LABEL 15b
    temp82 <- L09
    if (temp82 = 03e7) is true then
        jump-to: LABEL 15d
LABEL 15c
    temp83 <- L01
    temp84 <- L09
    temp85 <- L08
    discard: call 7e4 (temp83, temp84, temp85)
LABEL 15d
    temp86 <- pop-SP
    L07 <- temp86
    temp87 <- L07
    temp88 <- L0a
    temp89 <- L08
    discard: call 7e4 (temp87, temp88, temp89)
    temp8a <- pop-SP
    L07 <- temp8a
    temp8b <- L07
    temp8c <- L0b
    temp8d <- L08
    discard: call 7e4 (temp8b, temp8c, temp8d)
    return: 1
LABEL 15e
    temp8e <- L01
    temp8f <- L08
    discard: call 7e4 (temp8e, 7b, temp8f)
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

End Class
