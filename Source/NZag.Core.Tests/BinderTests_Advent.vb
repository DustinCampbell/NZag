Public Module BinderTests_Advent

#Region "Advent_9524"

#Region "ZCode"
    '9525:  41 59 01 77             je              g49 #01 ~955e
    '9529:  0d 06 00                store           local5 #00
    '952c:  42 06 08 6c             jl              local5 #08 ~955a
    '9530:  42 06 00 c6             jl              local5 #00 9538
    '9534:  42 06 10 d2             jl              local5 #10 9548
    '9538:  fa 19 5f 5a 72 1d 06 0f 01 04 
    '                              call_vn2        169c8 #1d local5 #0f #01 #04
    '9542:  e8 7f 00                push            #00
    '9545:  8c 00 08                jump            954e
    '9548:  cf 2f 3e d7 06 00       loadw           #3ed7 local5 -> sp
    '954e:  f9 2a 5c d0 01 06 00    call_vn         17340 local0 local5 sp
    '9555:  95 06                   inc             local5
    '9557:  8c ff d4                jump            952c
    '955a:  0d 59 00                store           g49 #00
    '955d:  b0                      rtrue           
    '955e:  41 97 01 50             je              g87 #01 ~9570
    '9562:  0d 97 00                store           g87 #00
    '9565:  f9 03 24 92 41 b7 42 32 call_vn         9248 #41b7 #4232
    '956d:  8c 00 0a                jump            9578
    '9570:  f9 03 24 a6 41 b7 42 32 call_vn         9298 #41b7 #4232
    '9578:  0d 4e 01                store           g3e #01
    '957b:  02 01 00 c6             jl              #01 #00 9583
    '957f:  02 01 41 cf             jl              #01 #41 9590
    '9583:  fa 15 5f 5a 72 1c 01 40 04 0e 
    '                              call_vn2        169c8 #1c #01 #40 #04 #0e
    '958d:  8c 00 08                jump            9596
    '9590:  d0 1f 42 32 01 91       loadb           #4232 #01 -> g81
    '9596:  0d 90 01                store           g80 #01
    '9599:  8f 1a 9c                call_1n         6a70
    '959c:  f9 03 24 92 41 b7 42 32 call_vn         9248 #41b7 #4232
    '95a4:  8f 57 dc                call_1n         15f70
    '95a7:  02 01 00 c6             jl              #01 #00 95af
    '95ab:  02 01 41 cf             jl              #01 #41 95bc
    '95af:  fa 15 5f 5a 72 1c 01 40 04 0e 
    '                              call_vn2        169c8 #1c #01 #40 #04 #0e
    '95b9:  8c 00 08                jump            95c2
    '95bc:  d0 1f 42 32 01 91       loadb           #4232 #01 -> g81
    '95c2:  0d 08 00                store           local7 #00
    '95c5:  0d 93 01                store           g83 #01
    '95c8:  2d 4f 26                store           g3f g16
    '95cb:  d9 2f 28 86 26 50       call_2s         a218 g16 -> g40
    '95d1:  0d 94 00                store           g84 #00
    '95d4:  0d 85 00                store           g75 #00
    '95d7:  cd 4f 69 ff ff          store           g59 #ffff
    '95dc:  2d 90 93                store           g80 g83
    '95df:  88 33 a6 92             call_1s         ce98 -> g82
    '95e3:  c1 8f 92 ff ff 48       je              g82 #ffff ~95ef
    '95e9:  0d 5d 01                store           g4d #01
    '95ec:  8c 08 bd                jump            9eaa
    '95ef:  c1 83 92 58 30 4f 03 47 je              g82 "g" "again" ~95fc
    '95f7:  cd 4f 92 4f 03          store           g82 "again"
    '95fc:  c1 8f 92 4f 03 00 87    je              g82 "again" ~9688
    '9603:  61 4f 26 cc             je              g3f g16 9611
    '9607:  f9 07 4e a2 10 07 14    call_vn         13a88 #1007 #14
    '960e:  8c ff 61                jump            9570
    '9611:  02 01 00 c6             jl              #01 #00 9619
    '9615:  02 01 7b d2             jl              #01 #7b 9629
    '9619:  fa 15 5f 5a 72 1c 01 7a 00 11 
    '                              call_vn2        169c8 #1c #01 #7a #00 #11
    '9623:  e8 7f 00                push            #00
    '9626:  8c 00 08                jump            962f
    '9629:  d0 1f 43 2f 01 00       loadb           #432f #01 -> sp
    '962f:  a0 00 4c                jz              sp ~963c
    '9632:  f9 07 4e a2 10 07 15    call_vn         13a88 #1007 #15
    '9639:  8c ff 36                jump            9570
    '963c:  0d 06 00                store           local5 #00
    '963f:  42 06 78 00 43          jl              local5 #78 ~9685
    '9644:  42 06 00 c6             jl              local5 #00 964c
    '9648:  42 06 7b d2             jl              local5 #7b 965c
    '964c:  fa 19 5f 5a 72 1c 06 7a 00 11 
    '                              call_vn2        169c8 #1c local5 #7a #00 #11
    '9656:  e8 7f 00                push            #00
    '9659:  8c 00 08                jump            9662
    '965c:  d0 2f 43 2f 06 00       loadb           #432f local5 -> sp
    '9662:  42 06 00 c6             jl              local5 #00 966a
    '9666:  42 06 7b d2             jl              local5 #7b 967a
    '966a:  fa 19 5f 5a 72 1e 06 7a 00 0d 
    '                              call_vn2        169c8 #1e local5 #7a #00 #0d
    '9674:  a0 00 42                jz              sp ~9677
    '9677:  8c 00 08                jump            9680
    '967a:  e2 2b 41 b7 06 00       storeb          #41b7 local5 sp
    '9680:  95 06                   inc             local5
    '9682:  8c ff bc                jump            963f
    '9685:  8c fe f2                jump            9578
    '9688:  c1 8f 92 4f 03 80 4b    je              g82 "again" 96d8
    '968f:  0d 06 00                store           local5 #00
    '9692:  42 06 78 00 43          jl              local5 #78 ~96d8
    '9697:  42 06 00 c6             jl              local5 #00 969f
    '969b:  42 06 7b d2             jl              local5 #7b 96af
    '969f:  fa 19 5f 5a 72 1c 06 7a 00 0d 
    '                              call_vn2        169c8 #1c local5 #7a #00 #0d
    '96a9:  e8 7f 00                push            #00
    '96ac:  8c 00 08                jump            96b5
    '96af:  d0 2f 41 b7 06 00       loadb           #41b7 local5 -> sp
    '96b5:  42 06 00 c6             jl              local5 #00 96bd
    '96b9:  42 06 7b d2             jl              local5 #7b 96cd
    '96bd:  fa 19 5f 5a 72 1e 06 7a 00 11 
    '                              call_vn2        169c8 #1e local5 #7a #00 #11
    '96c7:  a0 00 42                jz              sp ~96ca
    '96ca:  8c 00 08                jump            96d3
    '96cd:  e2 2b 43 2f 06 00       storeb          #432f local5 sp
    '96d3:  95 06                   inc             local5
    '96d5:  8c ff bc                jump            9692
    '96d8:  a0 94 00 7b             jz              g84 ~9755
    '96dc:  2d 07 93                store           local6 g83
    '96df:  e0 27 38 11 4f 1c 06    call_vs         e044 g3f #1c -> local5
    '96e6:  c1 97 06 00 01 fa       je              local5 #00 #01 9724
    '96ec:  e0 2b 3c 1b 06 9a 00    call_vs         f06c local5 g8a -> sp
    '96f3:  42 00 00 e8             jl              sp #00 971d
    '96f7:  e0 2b 3c 1b 06 9c 00    call_vs         f06c local5 g8c -> sp
    '96fe:  42 00 00 5d             jl              sp #00 ~971d
    '9702:  75 06 9a 00             sub             local5 g8a -> sp
    '9706:  e9 7f ff                pull            gef
    '9709:  2d fe 9b                store           gee g8b
    '970c:  a0 fe 49                jz              gee ~9716
    '970f:  f9 1f 5a 72 14          call_vn         169c8 #14
    '9714:  95 fe                   inc             gee
    '9716:  78 ff fe 00             mod             gef gee -> sp
    '971a:  a0 00 c9                jz              sp 9724
    '971d:  2d 94 07                store           g84 local6
    '9720:  35 00 06 06             sub             #00 local5 -> local5
    '9724:  41 06 01 58             je              local5 #01 ~973e
    '9728:  f9 26 5c d0 01 00 42    call_vn         17340 local0 #00 g32
    '972f:  f9 26 5c d0 01 01 45    call_vn         17340 local0 #01 g35
    '9736:  f9 26 5c d0 01 02 46    call_vn         17340 local0 #02 g36
    '973d:  b0                      rtrue           
    '973e:  a0 06 cc                jz              local5 974b
    '9741:  2d 92 06                store           g82 local5
    '9744:  96 90                   dec             g80
    '9746:  96 93                   dec             g83
    '9748:  8c 00 09                jump            9752
    '974b:  2d 90 93                store           g80 g83
    '974e:  88 33 8d 92             call_1s         ce34 -> g82
    '9752:  8c 00 05                jump            9758
    '9755:  0d 94 00                store           g84 #00
    '9758:  a0 92 d1                jz              g82 976a
    '975b:  e0 27 5c a1 92 06 00    call_vs         17284 g82 #06 -> sp
    '9762:  49 00 01 00             and             sp #01 -> sp
    '9766:  a0 00 01 20             jz              sp ~9888
    '976a:  2d 90 93                store           g80 g83
    '976d:  0d 75 00                store           g65 #00
    '9770:  0d 6e 00                store           g5e #00
    '9773:  e0 15 2a e9 06 00 00 0a call_vs         aba4 #06 #00 #00 -> local9
    '977b:  c1 8f 0a 27 10 45       je              local9 #2710 ~9784
    '9781:  8c fd f6                jump            9578
    '9784:  a0 0a dd                jz              local9 97a2
    '9787:  f9 25 5c d0 01 00 1b    call_vn         17340 local0 #00 #1b
    '978e:  0d 69 1b                store           g59 #1b
    '9791:  f9 25 5c d0 01 01 01    call_vn         17340 local0 #01 #01
    '9798:  f9 26 5c d0 01 02 0a    call_vn         17340 local0 #02 local9
    '979f:  8c 09 ce                jump            a16e
    '97a2:  61 4f 26 67             je              g3f g16 ~97cb
    '97a6:  0d 07 02                store           local6 #02
    '97a9:  63 07 91 d4             jg              local6 g81 97bf
    '97ad:  88 33 8d 06             call_1s         ce34 -> local5
    '97b1:  c1 8f 06 53 32 45       je              local5 "comma," ~97ba
    '97b7:  8c 00 19                jump            97d1
    '97ba:  95 07                   inc             local6
    '97bc:  8c ff ec                jump            97a9
    '97bf:  d9 2f 57 0b 92 92       call_2s         15c2c g82 -> g82
    '97c5:  a0 92 c5                jz              g82 97cb
    '97c8:  8c 00 bf                jump            9888
    '97cb:  0d 5d 0c                store           g4d #0c
    '97ce:  8c 06 db                jump            9eaa
    '97d1:  55 90 01 07             sub             g80 #01 -> local6
    '97d5:  41 07 01 4c             je              local6 #01 ~97e3
    '97d9:  f9 07 4e a2 10 07 16    call_vn         13a88 #1007 #16
    '97e0:  8c fd 8f                jump            9570
    '97e3:  0d 90 01                store           g80 #01
    '97e6:  0d 70 01                store           g60 #01
    '97e9:  0d 84 01                store           g74 #01
    '97ec:  e0 29 2a e9 26 50 06 0a call_vs         aba4 g16 g40 #06 -> local9
    '97f4:  0d 84 00                store           g74 #00
    '97f7:  c1 8f 0a 27 10 45       je              local9 #2710 ~9800
    '97fd:  8c fd 7a                jump            9578
    '9800:  a0 0a 4c                jz              local9 ~980d
    '9803:  f9 07 4e a2 10 07 17    call_vn         13a88 #1007 #17
    '980a:  8c fd 65                jump            9570
    '980d:  23 01 0a c8             jg              #01 local9 9817
    '9811:  c3 8f 0a 01 14 4b       jg              local9 #0114 ~9820
    '9817:  f9 1b 5a 72 03 0a       call_vn         169c8 #03 local9
    '981d:  8c 00 06                jump            9824
    '9820:  4a 0a 00 e4             test_attr       local9 animate 9846
    '9824:  23 01 0a c8             jg              #01 local9 982e
    '9828:  c3 8f 0a 01 14 4b       jg              local9 #0114 ~9837
    '982e:  f9 1b 5a 72 03 0a       call_vn         169c8 #03 local9
    '9834:  8c 00 06                jump            983b
    '9837:  4a 0a 16 cd             test_attr       local9 talkable 9846
    '983b:  f9 06 4e a2 10 07 18 0a call_vn         13a88 #1007 #18 local9
    '9843:  8c fd 2c                jump            9570
    '9846:  61 90 07 cc             je              g80 local6 9854
    '984a:  f9 07 4e a2 10 07 19    call_vn         13a88 #1007 #19
    '9851:  8c fd 1e                jump            9570
    '9854:  da 2f 35 11 0a          call_2n         d444 local9
    '9859:  54 07 01 93             add             local6 #01 -> g83
    '985d:  61 0a 26 5d             je              local9 g16 ~987c
    '9861:  2d 90 93                store           g80 g83
    '9864:  88 33 a6 00             call_1s         ce98 -> sp
    '9868:  c1 80 00 4f 03 58 30 4f 03 4c 
    '                              je              sp "again" "g" "again" ~987c
    '9872:  f9 07 4e a2 10 07 14    call_vn         13a88 #1007 #14
    '9879:  8c fc f6                jump            9570
    '987c:  2d 4f 0a                store           g3f local9
    '987f:  d9 2f 28 86 0a 50       call_2s         a218 local9 -> g40
    '9885:  8c fd 56                jump            95dc
    '9888:  e0 27 5c a1 92 06 00    call_vs         17284 g82 #06 -> sp
    '988f:  49 00 02 00             and             sp #02 -> sp
    '9893:  57 00 02 51             div             sp #02 -> g41
    '9897:  41 51 01 4f             je              g41 #01 ~98a8
    '989b:  61 4f 26 cb             je              g3f g16 98a8
    '989f:  0d 5d 0c                store           g4d #0c
    '98a2:  0d 51 00                store           g41 #00
    '98a5:  8c 06 04                jump            9eaa
    '98a8:  e0 27 5c a1 92 07 00    call_vs         17284 g82 #07 -> sp
    '98af:  35 ff 00 06             sub             #ff sp -> local5
    '98b3:  e0 17 5c a9 0e 00 00    call_vs         172a4 #0e #00 -> sp
    '98ba:  e0 2b 5c a9 00 06 02    call_vs         172a4 sp local5 -> local1
    '98c1:  e0 27 5c a1 02 00 00    call_vs         17284 local1 #00 -> sp
    '98c8:  55 00 01 04             sub             sp #01 -> local3
    '98cc:  cd 4f 80 ff ff          store           g70 #ffff
    '98d1:  cd 4f 81 ff ff          store           g71 #ffff
    '98d6:  0d 5d 01                store           g4d #01
    '98d9:  0d 5e 01                store           g4e #01
    '98dc:  0d 52 00                store           g42 #00
    '98df:  54 02 01 05             add             local1 #01 -> local4
    '98e3:  0d 03 00                store           local2 #00
    '98e6:  63 03 04 85 c1          jg              local2 local3 9eaa
    '98eb:  0d 06 00                store           local5 #00
    '98ee:  42 06 20 00 58          jl              local5 #20 ~9949
    '98f3:  42 06 00 c6             jl              local5 #00 98fb
    '98f7:  42 06 20 cf             jl              local5 #20 9908
    '98fb:  fa 19 5f 5a 72 1f 06 1f 01 09 
    '                              call_vn2        169c8 #1f local5 #1f #01 #09
    '9905:  8c 00 08                jump            990e
    '9908:  e1 27 3f f7 06 0f       storew          #3ff7 local5 #0f
    '990e:  42 06 00 c6             jl              local5 #00 9916
    '9912:  42 06 20 cf             jl              local5 #20 9923
    '9916:  fa 19 5f 5a 72 1f 06 1f 01 07 
    '                              call_vn2        169c8 #1f local5 #1f #01 #07
    '9920:  8c 00 08                jump            9929
    '9923:  e1 27 3f 77 06 01       storew          #3f77 local5 #01
    '9929:  42 06 00 c6             jl              local5 #00 9931
    '992d:  42 06 20 cf             jl              local5 #20 993e
    '9931:  fa 19 5f 5a 72 1f 06 1f 01 08 
    '                              call_vn2        169c8 #1f local5 #1f #01 #08
    '993b:  8c 00 08                jump            9944
    '993e:  e1 27 3f b7 06 0f       storew          #3fb7 local5 #0f
    '9944:  95 06                   inc             local5
    '9946:  8c ff a7                jump            98ee
    '9949:  d9 2f 24 4d 05 05       call_2s         9134 local4 -> local4
    '994f:  0d 5b 00                store           g4b #00
    '9952:  0d 66 00                store           g56 #00
    '9955:  0d 61 00                store           g51 #00
    '9958:  0d 62 00                store           g52 #00
    '995b:  0d 54 00                store           g44 #00
    '995e:  0d 55 00                store           g45 #00
    '9961:  02 00 00 c6             jl              #00 #00 9969
    '9965:  02 00 40 cf             jl              #00 #40 9976
    '9969:  fa 15 5f 5a 72 1f 00 3f 01 03 
    '                              call_vn2        169c8 #1f #00 #3f #01 #03
    '9973:  8c 00 08                jump            997c
    '9976:  e1 17 3e 57 00 00       storew          #3e57 #00 #00
    '997c:  0d 74 00                store           g64 #00
    '997f:  0d 5c 01                store           g4c #01
    '9982:  54 93 01 90             add             g83 #01 -> g80
    '9986:  cd 4f 6b ff ff          store           g5b #ffff
    '998b:  0d 75 00                store           g65 #00
    '998e:  0d 06 00                store           local5 #00
    '9991:  0d 0b 00                store           local10 #00
    '9994:  0d 5f 00                store           g4f #00
    '9997:  42 5f 00 c6             jl              g4f #00 999f
    '999b:  42 5f 20 d2             jl              g4f #20 99af
    '999f:  fa 19 5f 5a 72 1d 5f 1f 01 09 
    '                              call_vn2        169c8 #1d g4f #1f #01 #09
    '99a9:  e8 7f 00                push            #00
    '99ac:  8c 00 08                jump            99b5
    '99af:  cf 2f 3f f7 5f 00       loadw           #3ff7 g4f -> sp
    '99b5:  41 00 0f 81 73          je              sp #0f 9b2b
    '99ba:  0d 85 00                store           g75 #00
    '99bd:  42 5f 00 c6             jl              g4f #00 99c5
    '99c1:  42 5f 20 d2             jl              g4f #20 99d5
    '99c5:  fa 19 5f 5a 72 1d 5f 1f 01 07 
    '                              call_vn2        169c8 #1d g4f #1f #01 #07
    '99cf:  e8 7f 00                push            #00
    '99d2:  8c 00 08                jump            99db
    '99d5:  cf 2f 3f 77 5f 00       loadw           #3f77 g4f -> sp
    '99db:  41 00 02 c4             je              sp #02 99e1
    '99df:  95 06                   inc             local5
    '99e1:  42 5f 00 c6             jl              g4f #00 99e9
    '99e5:  42 5f 20 d2             jl              g4f #20 99f9
    '99e9:  fa 19 5f 5a 72 1d 5f 1f 01 07 
    '                              call_vn2        169c8 #1d g4f #1f #01 #07
    '99f3:  e8 7f 00                push            #00
    '99f6:  8c 00 08                jump            99ff
    '99f9:  cf 2f 3f 77 5f 00       loadw           #3f77 g4f -> sp
    '99ff:  41 00 01 01 24          je              sp #01 ~9b26
    '9a04:  42 5f 00 c6             jl              g4f #00 9a0c
    '9a08:  42 5f 20 d2             jl              g4f #20 9a1c
    '9a0c:  fa 19 5f 5a 72 1d 5f 1f 01 08 
    '                              call_vn2        169c8 #1d g4f #1f #01 #08
    '9a16:  e8 7f 00                push            #00
    '9a19:  8c 00 08                jump            9a22
    '9a1c:  cf 2f 3f b7 5f 00       loadw           #3fb7 g4f -> sp
    '9a22:  41 00 02 45             je              sp #02 ~9a29
    '9a26:  0d 0b 01                store           local10 #01
    '9a29:  42 5f 00 c6             jl              g4f #00 9a31
    '9a2d:  42 5f 20 d2             jl              g4f #20 9a41
    '9a31:  fa 19 5f 5a 72 1d 5f 1f 01 08 
    '                              call_vn2        169c8 #1d g4f #1f #01 #08
    '9a3b:  e8 7f 00                push            #00
    '9a3e:  8c 00 08                jump            9a47
    '9a41:  cf 2f 3f b7 5f 00       loadw           #3fb7 g4f -> sp
    '9a47:  c1 97 00 04 05 00 da    je              sp #04 #05 ~9b26
    '9a4e:  41 06 01 00 d5          je              local5 #01 ~9b26
    '9a53:  95 5f                   inc             g4f
    '9a55:  42 5f 00 c6             jl              g4f #00 9a5d
    '9a59:  42 5f 20 d2             jl              g4f #20 9a6d
    '9a5d:  fa 19 5f 5a 72 1d 5f 1f 01 07 
    '                              call_vn2        169c8 #1d g4f #1f #01 #07
    '9a67:  e8 7f 00                push            #00
    '9a6a:  8c 00 08                jump            9a73
    '9a6d:  cf 2f 3f 77 5f 00       loadw           #3f77 g4f -> sp
    '9a73:  41 00 02 00 ad          je              sp #02 ~9b23
    '9a78:  42 5f 00 c6             jl              g4f #00 9a80
    '9a7c:  42 5f 20 d2             jl              g4f #20 9a90
    '9a80:  fa 19 5f 5a 72 1d 5f 1f 01 07 
    '                              call_vn2        169c8 #1d g4f #1f #01 #07
    '9a8a:  e8 7f 00                push            #00
    '9a8d:  8c 00 08                jump            9a96
    '9a90:  cf 2f 3f 77 5f 00       loadw           #3f77 g4f -> sp
    '9a96:  41 00 02 47             je              sp #02 ~9a9f
    '9a9a:  95 5f                   inc             g4f
    '9a9c:  8c ff db                jump            9a78
    '9a9f:  42 5f 00 c6             jl              g4f #00 9aa7
    '9aa3:  42 5f 20 d2             jl              g4f #20 9ab7
    '9aa7:  fa 19 5f 5a 72 1d 5f 1f 01 07 
    '                              call_vn2        169c8 #1d g4f #1f #01 #07
    '9ab1:  e8 7f 00                push            #00
    '9ab4:  8c 00 08                jump            9abd
    '9ab7:  cf 2f 3f 77 5f 00       loadw           #3f77 g4f -> sp
    '9abd:  41 00 01 00 63          je              sp #01 ~9b23
    '9ac2:  42 5f 00 c6             jl              g4f #00 9aca
    '9ac6:  42 5f 20 d2             jl              g4f #20 9ada
    '9aca:  fa 19 5f 5a 72 1d 5f 1f 01 08 
    '                              call_vn2        169c8 #1d g4f #1f #01 #08
    '9ad4:  e8 7f 00                push            #00
    '9ad7:  8c 00 08                jump            9ae0
    '9ada:  cf 2f 3f b7 5f 00       loadw           #3fb7 g4f -> sp
    '9ae0:  a0 00 00 41             jz              sp ~9b23
    '9ae4:  62 90 91 00 3c          jl              g80 g81 ~9b23
    '9ae9:  88 33 8d 0a             call_1s         ce34 -> local9
    '9aed:  a0 0a f2                jz              local9 9b20
    '9af0:  e0 27 5c a1 0a 06 00    call_vs         17284 local9 #06 -> sp
    '9af7:  49 00 08 00             and             sp #08 -> sp
    '9afb:  a0 00 e4                jz              sp 9b20
    '9afe:  88 28 c3 0a             call_1s         a30c -> local9
    '9b02:  a0 0a c5                jz              local9 9b08
    '9b05:  2d 5c 0a                store           g4c local9
    '9b08:  e0 29 2a e9 50 4f 00 0a call_vs         aba4 g40 g3f #00 -> local9
    '9b10:  c1 8f 0a 27 10 45       je              local9 #2710 ~9b19
    '9b16:  8c fa 61                jump            9578
    '9b19:  42 0a 02 c5             jl              local9 #02 9b20
    '9b1d:  2d 6b 0a                store           g5b local9
    '9b20:  8c ff c3                jump            9ae4
    '9b23:  8c 00 07                jump            9b2b
    '9b26:  95 5f                   inc             g4f
    '9b28:  8c fe 6e                jump            9997
    '9b2b:  0d 7e 00                store           g6e #00
    '9b2e:  a0 0b cd                jz              local10 9b3c
    '9b31:  41 65 01 49             je              g55 #01 ~9b3c
    '9b35:  41 69 4e 45             je              g59 #4e ~9b3c
    '9b39:  0d 7e 01                store           g6e #01
    '9b3c:  0d 5b 00                store           g4b #00
    '9b3f:  0d 66 00                store           g56 #00
    '9b42:  0d 61 00                store           g51 #00
    '9b45:  0d 62 00                store           g52 #00
    '9b48:  0d 54 00                store           g44 #00
    '9b4b:  0d 55 00                store           g45 #00
    '9b4e:  02 00 00 c6             jl              #00 #00 9b56
    '9b52:  02 00 40 cf             jl              #00 #40 9b63
    '9b56:  fa 15 5f 5a 72 1f 00 3f 01 03 
    '                              call_vn2        169c8 #1f #00 #3f #01 #03
    '9b60:  8c 00 08                jump            9b69
    '9b63:  e1 17 3e 57 00 00       storew          #3e57 #00 #00
    '9b69:  0d 5c 01                store           g4c #01
    '9b6c:  54 93 01 90             add             g83 #01 -> g80
    '9b70:  0d 5f 01                store           g4f #01
    '9b73:  42 5f 00 c6             jl              g4f #00 9b7b
    '9b77:  42 5f 20 cf             jl              g4f #20 9b88
    '9b7b:  fa 19 5f 5a 72 1f 5f 1f 01 05 
    '                              call_vn2        169c8 #1f g4f #1f #01 #05
    '9b85:  8c 00 09                jump            9b8f
    '9b88:  e1 23 3e f7 5f ff ff    storew          #3ef7 g4f #ffff
    '9b8f:  0d 85 00                store           g75 #00
    '9b92:  55 5f 01 00             sub             g4f #01 -> sp
    '9b96:  e9 7f fe                pull            gee
    '9b99:  e8 bf fe                push            gee
    '9b9c:  42 fe 00 c6             jl              gee #00 9ba4
    '9ba0:  42 fe 20 d2             jl              gee #20 9bb4
    '9ba4:  fa 19 5f 5a 72 1d fe 1f 01 09 
    '                              call_vn2        169c8 #1d gee #1f #01 #09
    '9bae:  a0 00 42                jz              sp ~9bb1
    '9bb1:  8c 00 08                jump            9bba
    '9bb4:  cf 2f 3f f7 00 09       loadw           #3ff7 sp -> local8
    '9bba:  42 5f 00 c6             jl              g4f #00 9bc2
    '9bbe:  42 5f 20 cf             jl              g4f #20 9bcf
    '9bc2:  fa 19 5f 5a 72 1d 5f 1f 01 09 
    '                              call_vn2        169c8 #1d g4f #1f #01 #09
    '9bcc:  8c 00 08                jump            9bd5
    '9bcf:  cf 2f 3f f7 5f 70       loadw           #3ff7 g4f -> g60
    '9bd5:  41 09 0f 80 e4          je              local8 #0f 9cbc
    '9bda:  0d 84 00                store           g74 #00
    '9bdd:  0d 4e 01                store           g3e #01
    '9be0:  da 2f 24 44 09          call_2n         9110 local8
    '9be5:  41 69 5d 6d             je              g59 #5d ~9c14
    '9be9:  41 6c 01 69             je              g5c #01 ~9c14
    '9bed:  41 6d 09 65             je              g5d #09 ~9c14
    '9bf1:  02 02 00 c6             jl              #02 #00 9bf9
    '9bf5:  02 02 10 cf             jl              #02 #10 9c06
    '9bf9:  fa 15 5f 5a 72 1d 02 0f 01 02 
    '                              call_vn2        169c8 #1d #02 #0f #01 #02
    '9c03:  8c 00 08                jump            9c0c
    '9c06:  cf 1f 3e 37 02 0a       loadw           #3e37 #02 -> local9
    '9c0c:  96 90                   dec             g80
    '9c0e:  2d 07 90                store           local6 g80
    '9c11:  8c fb fb                jump            980d
    '9c14:  55 5f 01 00             sub             g4f #01 -> sp
    '9c18:  ec 2a bf 29 80 6c 6d 00 09 0a 
    '                              call_vs2        a600 g5c g5d sp local8 -> local9
    '9c22:  c2 8f 0a ff 38 52       jl              local9 #ff38 ~9c38
    '9c28:  d4 8f 0a 01 00 00       add             local9 #0100 -> sp
    '9c2e:  e0 1b 29 80 01 00 0a    call_vs         a600 #01 sp -> local9
    '9c35:  8c ff ec                jump            9c22
    '9c38:  0d 84 00                store           g74 #00
    '9c3b:  a0 0a 56                jz              local9 ~9c52
    '9c3e:  41 6c 02 cc             je              g5c #02 9c4c
    '9c42:  41 6c 01 46             je              g5c #01 ~9c4a
    '9c46:  41 6d 09 c4             je              g5d #09 9c4c
    '9c4a:  96 65                   dec             g55
    '9c4c:  0d 0a 01                store           local9 #01
    '9c4f:  8c 00 5a                jump            9caa
    '9c52:  42 0a 00 48             jl              local9 #00 ~9c5c
    '9c56:  0d 0a 00                store           local9 #00
    '9c59:  8c 00 50                jump            9caa
    '9c5c:  c1 8f 0a 27 10 80 49    je              local9 #2710 9caa
    '9c63:  41 0a 01 53             je              local9 #01 ~9c78
    '9c67:  a0 62 48                jz              g52 ~9c70
    '9c6a:  2d 63 56                store           g53 g46
    '9c6d:  8c 00 05                jump            9c73
    '9c70:  2d 64 56                store           g54 g46
    '9c73:  95 62                   inc             g52
    '9c75:  0d 0a 01                store           local9 #01
    '9c78:  41 0a 02 45             je              local9 #02 ~9c7f
    '9c7c:  0d 0a 00                store           local9 #00
    '9c7f:  54 61 02 00             add             g51 #02 -> sp
    '9c83:  f9 2a 5c d0 01 00 0a    call_vn         17340 local0 sp local9
    '9c8a:  95 61                   inc             g51
    '9c8c:  42 5f 00 c6             jl              g4f #00 9c94
    '9c90:  42 5f 20 cf             jl              g4f #20 9ca1
    '9c94:  fa 19 5f 5a 72 1f 5f 1f 01 05 
    '                              call_vn2        169c8 #1f g4f #1f #01 #05
    '9c9e:  8c 00 08                jump            9ca7
    '9ca1:  e1 2b 3e f7 5f 0a       storew          #3ef7 g4f local9
    '9ca7:  0d 0a 01                store           local9 #01
    '9caa:  c1 8f 0a 27 10 45       je              local9 #2710 ~9cb3
    '9cb0:  8c f8 c7                jump            9578
    '9cb3:  a0 0a 45                jz              local9 ~9cb9
    '9cb6:  8c 01 d1                jump            9e88
    '9cb9:  8c 01 c9                jump            9e83
    '9cbc:  63 90 91 80 72          jg              g80 g81 9d31
    '9cc1:  88 33 8d 0a             call_1s         ce34 -> local9
    '9cc5:  c1 80 0a 66 64 66 64 66 64 c8 
    '                              je              local9 "then" "then" "then" 9cd5
    '9ccf:  c1 8f 0a 53 32 4c       je              local9 "comma," ~9cdf
    '9cd5:  0d 97 01                store           g87 #01
    '9cd8:  55 90 01 98             sub             g80 #01 -> g88
    '9cdc:  8c 00 54                jump            9d31
    '9cdf:  0d 0b 00                store           local10 #00
    '9ce2:  42 0b 20 00 43          jl              local10 #20 ~9d28
    '9ce7:  42 0b 00 c6             jl              local10 #00 9cef
    '9ceb:  42 0b 20 d2             jl              local10 #20 9cff
    '9cef:  fa 19 5f 5a 72 1d 0b 1f 01 05 
    '                              call_vn2        169c8 #1d local10 #1f #01 #05
    '9cf9:  e8 7f 00                push            #00
    '9cfc:  8c 00 08                jump            9d05
    '9cff:  cf 2f 3e f7 0b 00       loadw           #3ef7 local10 -> sp
    '9d05:  42 0b 00 c6             jl              local10 #00 9d0d
    '9d09:  42 0b 20 d2             jl              local10 #20 9d1d
    '9d0d:  fa 19 5f 5a 72 1f 0b 1f 01 06 
    '                              call_vn2        169c8 #1f local10 #1f #01 #06
    '9d17:  a0 00 42                jz              sp ~9d1a
    '9d1a:  8c 00 08                jump            9d23
    '9d1d:  e1 2b 3f 37 0b 00       storew          #3f37 local10 sp
    '9d23:  95 0b                   inc             local10
    '9d25:  8c ff bc                jump            9ce2
    '9d28:  2d 60 5f                store           g50 g4f
    '9d2b:  0d 5c 02                store           g4c #02
    '9d2e:  8c 01 59                jump            9e88
    '9d31:  42 61 01 e9             jl              g51 #01 9d5c
    '9d35:  e0 27 5c a9 01 02 00    call_vs         172a4 local0 #02 -> sp
    '9d3c:  a0 00 5f                jz              sp ~9d5c
    '9d3f:  e0 27 5c a9 01 03 00    call_vs         172a4 local0 #03 -> sp
    '9d46:  d9 2f 2e a9 00 0a       call_2s         baa4 sp -> local9
    '9d4c:  a0 0a cf                jz              local9 9d5c
    '9d4f:  2d 5c 0a                store           g4c local9
    '9d52:  f9 26 5c d0 01 00 69    call_vn         17340 local0 #00 g59
    '9d59:  8c 01 2e                jump            9e88
    '9d5c:  42 61 02 e2             jl              g51 #02 9d80
    '9d60:  e0 27 5c a9 01 03 00    call_vs         172a4 local0 #03 -> sp
    '9d67:  a0 00 58                jz              sp ~9d80
    '9d6a:  e0 27 5c a9 01 02 00    call_vs         172a4 local0 #02 -> sp
    '9d71:  d9 2f 2e a9 00 0a       call_2s         baa4 sp -> local9
    '9d77:  a0 0a c8                jz              local9 9d80
    '9d7a:  2d 5c 0a                store           g4c local9
    '9d7d:  8c 01 0a                jump            9e88
    '9d80:  41 7e 02 53             je              g6e #02 ~9d95
    '9d84:  e0 27 5c a9 01 02 00    call_vs         172a4 local0 #02 -> sp
    '9d8b:  61 00 4f 48             je              sp g3f ~9d95
    '9d8f:  0d 5d 11                store           g4d #11
    '9d92:  8c 01 17                jump            9eaa
    '9d95:  0d 95 00                store           g85 #00
    '9d98:  a0 66 cf                jz              g56 9da8
    '9d9b:  b2 ...                  print           "("
    '9d9e:  da 2f 30 9a 66          call_2n         c268 g56
    '9da3:  b2 ...                  print           ")^"
    '9da8:  f9 26 5c d0 01 00 69    call_vn         17340 local0 #00 g59
    '9daf:  f9 26 5c d0 01 01 61    call_vn         17340 local0 #01 g51
    '9db6:  a0 6a ef                jz              g5a 9de6
    '9db9:  41 61 02 6b             je              g51 #02 ~9de6
    '9dbd:  e0 27 5c a9 01 02 06    call_vs         172a4 local0 #02 -> local5
    '9dc4:  e0 27 5c a9 01 03 00    call_vs         172a4 local0 #03 -> sp
    '9dcb:  f9 26 5c d0 01 02 00    call_vn         17340 local0 #02 sp
    '9dd2:  f9 26 5c d0 01 03 06    call_vn         17340 local0 #03 local5
    '9dd9:  41 62 02 4b             je              g52 #02 ~9de6
    '9ddd:  2d 06 63                store           local5 g53
    '9de0:  2d 63 64                store           g53 g54
    '9de3:  2d 64 06                store           g54 local5
    '9de6:  43 61 00 59             jg              g51 #00 ~9e01
    '9dea:  e0 27 5c a9 01 02 00    call_vs         172a4 local0 #02 -> sp
    '9df1:  42 00 02 ce             jl              sp #02 9e01
    '9df5:  e0 27 5c a9 01 02 00    call_vs         172a4 local0 #02 -> sp
    '9dfc:  da 2f 35 11 00          call_2n         d444 sp
    '9e01:  a0 5b 80 75             jz              g4b 9e78
    '9e05:  61 4f 26 00 70          je              g3f g16 ~9e78
    '9e0a:  0d 42 4e                store           g32 #4e
    '9e0d:  e0 27 38 11 5b 49 06    call_vs         e044 g4b #49 -> local5
    '9e14:  43 06 02 48             jg              local5 #02 ~9e1e
    '9e18:  0d 5d 06                store           g4d #06
    '9e1b:  8c 00 8e                jump            9eaa
    '9e1e:  42 06 02 00 57          jl              local5 #02 ~9e78
    '9e23:  41 06 01 ca             je              local5 #01 9e2f
    '9e27:  f9 06 4e a2 10 07 1a 5b call_vn         13a88 #1007 #1a g4b
    '9e2f:  0d 59 01                store           g49 #01
    '9e32:  0d 06 00                store           local5 #00
    '9e35:  42 06 08 6c             jl              local5 #08 ~9e63
    '9e39:  e0 2b 5c a9 01 06 00    call_vs         172a4 local0 local5 -> sp
    '9e40:  42 06 00 c6             jl              local5 #00 9e48
    '9e44:  42 06 10 d2             jl              local5 #10 9e58
    '9e48:  fa 19 5f 5a 72 1f 06 0f 01 04 
    '                              call_vn2        169c8 #1f local5 #0f #01 #04
    '9e52:  a0 00 42                jz              sp ~9e55
    '9e55:  8c 00 08                jump            9e5e
    '9e58:  e1 2b 3e d7 06 00       storew          #3ed7 local5 sp
    '9e5e:  95 06                   inc             local5
    '9e60:  8c ff d4                jump            9e35
    '9e63:  f9 25 5c d0 01 00 4e    call_vn         17340 local0 #00 #4e
    '9e6a:  f9 25 5c d0 01 01 01    call_vn         17340 local0 #01 #01
    '9e71:  f9 26 5c d0 01 02 5b    call_vn         17340 local0 #02 g4b
    '9e78:  41 97 01 48             je              g87 #01 ~9e82
    '9e7c:  2d 90 98                store           g80 g88
    '9e7f:  8c 02 ee                jump            a16e
    '9e82:  b0                      rtrue           
    '9e83:  95 5f                   inc             g4f
    '9e85:  8c fc ed                jump            9b73
    '9e88:  63 5c 5d 45             jg              g4c g4d ~9e8f
    '9e8c:  2d 5d 5c                store           g4d g4c
    '9e8f:  41 5c 12 c9             je              g4c #12 9e9a
    '9e93:  63 5c 5e 45             jg              g4c g4e ~9e9a
    '9e97:  2d 5e 5c                store           g4e g4c
    '9e9a:  41 7e 02 49             je              g6e #02 ~9ea5
    '9e9e:  41 5c 11 45             je              g4c #11 ~9ea5
    '9ea2:  8c 00 07                jump            9eaa
    '9ea5:  95 03                   inc             local2
    '9ea7:  8c fa 3e                jump            98e6
    '9eaa:  2d 5c 5d                store           g4c g4d
    '9ead:  61 4f 26 80 50          je              g3f g16 9f00
    '9eb2:  a0 94 c8                jz              g84 9ebb
    '9eb5:  2d 93 94                store           g83 g84
    '9eb8:  8c f7 1b                jump            95d4
    '9ebb:  2d 90 93                store           g80 g83
    '9ebe:  88 33 8d 54             call_1s         ce34 -> g44
    '9ec2:  c1 8f 54 53 32 48       je              g44 "comma," ~9ece
    '9ec8:  88 33 8d 54             call_1s         ce34 -> g44
    '9ecc:  95 93                   inc             g83
    '9ece:  d9 2f 33 cc 93 55       call_2s         cf30 g83 -> g45
    '9ed4:  f9 24 5c d0 01 00 10 09 call_vn         17340 local0 #00 #1009
    '9edc:  f9 25 5c d0 01 01 02    call_vn         17340 local0 #01 #02
    '9ee3:  f9 25 5c d0 01 02 01    call_vn         17340 local0 #02 #01
    '9eea:  2d 63 54                store           g53 g44
    '9eed:  f9 26 5c d0 01 03 4f    call_vn         17340 local0 #03 g3f
    '9ef4:  2d 57 93                store           g47 g83
    '9ef7:  75 91 57 00             sub             g81 g47 -> sp
    '9efb:  54 00 01 58             add             sp #01 -> g48
    '9eff:  b0                      rtrue           
    '9f00:  d9 2f 57 e5 5c 00       call_2s         15f94 g4c -> sp
    '9f06:  a0 00 c5                jz              sp 9f0c
    '9f09:  8c f6 66                jump            9570
    '9f0c:  2d 80 82                store           g70 g72
    '9f0f:  2d 81 83                store           g71 g73
    '9f12:  41 5c 01 4c             je              g4c #01 ~9f20
    '9f16:  f9 07 4e a2 10 07 1b    call_vn         13a88 #1007 #1b
    '9f1d:  0d 95 01                store           g85 #01
    '9f20:  41 5c 02 00 61          je              g4c #02 ~9f84
    '9f25:  f9 07 4e a2 10 07 1c    call_vn         13a88 #1007 #1c
    '9f2c:  0d 0b 00                store           local10 #00
    '9f2f:  42 0b 20 00 43          jl              local10 #20 ~9f75
    '9f34:  42 0b 00 c6             jl              local10 #00 9f3c
    '9f38:  42 0b 20 d2             jl              local10 #20 9f4c
    '9f3c:  fa 19 5f 5a 72 1d 0b 1f 01 06 
    '                              call_vn2        169c8 #1d local10 #1f #01 #06
    '9f46:  e8 7f 00                push            #00
    '9f49:  8c 00 08                jump            9f52
    '9f4c:  cf 2f 3f 37 0b 00       loadw           #3f37 local10 -> sp
    '9f52:  42 0b 00 c6             jl              local10 #00 9f5a
    '9f56:  42 0b 20 d2             jl              local10 #20 9f6a
    '9f5a:  fa 19 5f 5a 72 1f 0b 1f 01 05 
    '                              call_vn2        169c8 #1f local10 #1f #01 #05
    '9f64:  a0 00 42                jz              sp ~9f67
    '9f67:  8c 00 08                jump            9f70
    '9f6a:  e1 2b 3e f7 0b 00       storew          #3ef7 local10 sp
    '9f70:  95 0b                   inc             local10
    '9f72:  8c ff bc                jump            9f2f
    '9f75:  2d 5f 60                store           g4f g50
    '9f78:  da 1f 30 9a 00          call_2n         c268 #00
    '9f7d:  f9 07 4e a2 10 07 38    call_vn         13a88 #1007 #38
    '9f84:  41 5c 03 49             je              g4c #03 ~9f8f
    '9f88:  f9 07 4e a2 10 07 1d    call_vn         13a88 #1007 #1d
    '9f8f:  41 5c 04 4c             je              g4c #04 ~9f9d
    '9f93:  f9 07 4e a2 10 07 1e    call_vn         13a88 #1007 #1e
    '9f9a:  2d 95 96                store           g85 g86
    '9f9d:  41 5c 05 49             je              g4c #05 ~9fa8
    '9fa1:  f9 07 4e a2 10 07 1f    call_vn         13a88 #1007 #1f
    '9fa8:  41 5c 06 4c             je              g4c #06 ~9fb6
    '9fac:  f9 07 4e a2 10 07 20    call_vn         13a88 #1007 #20
    '9fb3:  2d 95 96                store           g85 g86
    '9fb6:  41 5c 07 49             je              g4c #07 ~9fc1
    '9fba:  f9 07 4e a2 10 07 21    call_vn         13a88 #1007 #21
    '9fc1:  41 5c 08 49             je              g4c #08 ~9fcc
    '9fc5:  f9 07 4e a2 10 07 22    call_vn         13a88 #1007 #22
    '9fcc:  41 5c 09 49             je              g4c #09 ~9fd7
    '9fd0:  f9 07 4e a2 10 07 23    call_vn         13a88 #1007 #23
    '9fd7:  41 5c 0a 49             je              g4c #0a ~9fe2
    '9fdb:  f9 07 4e a2 10 07 24    call_vn         13a88 #1007 #24
    '9fe2:  41 5c 0b 49             je              g4c #0b ~9fed
    '9fe6:  f9 07 4e a2 10 07 25    call_vn         13a88 #1007 #25
    '9fed:  41 5c 0c 49             je              g4c #0c ~9ff8
    '9ff1:  f9 07 4e a2 10 07 26    call_vn         13a88 #1007 #26
    '9ff8:  41 5c 0d 49             je              g4c #0d ~a003
    '9ffc:  f9 07 4e a2 10 07 27    call_vn         13a88 #1007 #27
    'a003:  41 5c 0e 59             je              g4c #0e ~a01e
    'a007:  c1 8f 81 ff ff 4c       je              g71 #ffff ~a017
    'a00d:  f9 07 4e a2 10 07 23    call_vn         13a88 #1007 #23
    'a014:  8c 00 09                jump            a01e
    'a017:  f9 07 4e a2 10 07 28    call_vn         13a88 #1007 #28
    'a01e:  41 5c 0f 49             je              g4c #0f ~a029
    'a022:  f9 07 4e a2 10 07 29    call_vn         13a88 #1007 #29
    'a029:  41 5c 10 4a             je              g4c #10 ~a035
    'a02d:  f9 06 4e a2 10 07 2a 73 call_vn         13a88 #1007 #2a g63
    'a035:  41 5c 11 01 1d          je              g4c #11 ~a155
    'a03a:  e0 27 5c a9 01 00 00    call_vs         172a4 local0 #00 -> sp
    'a041:  41 00 38 00 f1          je              sp #38 ~a135
    'a046:  e0 27 5c a9 01 03 00    call_vs         172a4 local0 #03 -> sp
    'a04d:  e0 27 5a 0b 00 02 00    call_vs         1682c sp #02 -> sp
    'a054:  a0 00 80 df             jz              sp a135
    'a058:  e0 27 5c a9 01 03 45    call_vs         172a4 local0 #03 -> g35
    'a05f:  23 01 45 c8             jg              #01 g35 a069
    'a063:  c3 8f 45 01 14 4b       jg              g35 #0114 ~a072
    'a069:  f9 1b 5a 72 03 45       call_vn         169c8 #03 g35
    'a06f:  8c 00 10                jump            a080
    'a072:  4a 45 00 4c             test_attr       g35 animate ~a080
    'a076:  f9 16 4e a2 4e 06 45    call_vn         13a88 #4e #06 g35
    'a07d:  8c 00 b7                jump            a135
    'a080:  23 01 45 c8             jg              #01 g35 a08a
    'a084:  c3 8f 45 01 14 4b       jg              g35 #0114 ~a093
    'a08a:  f9 1b 5a 72 03 45       call_vn         169c8 #03 g35
    'a090:  8c 00 06                jump            a097
    'a093:  4a 45 04 e3             test_attr       g35 container a0b8
    'a097:  23 01 45 c8             jg              #01 g35 a0a1
    'a09b:  c3 8f 45 01 14 4b       jg              g35 #0114 ~a0aa
    'a0a1:  f9 1b 5a 72 03 45       call_vn         169c8 #03 g35
    'a0a7:  8c 00 06                jump            a0ae
    'a0aa:  4a 45 14 cc             test_attr       g35 supporter a0b8
    'a0ae:  f9 16 4e a2 1c 02 45    call_vn         13a88 #1c #02 g35
    'a0b5:  8c 00 7f                jump            a135
    'a0b8:  23 01 45 c8             jg              #01 g35 a0c2
    'a0bc:  c3 8f 45 01 14 4b       jg              g35 #0114 ~a0cb
    'a0c2:  f9 1b 5a 72 03 45       call_vn         169c8 #03 g35
    'a0c8:  8c 00 27                jump            a0f0
    'a0cb:  4a 45 04 63             test_attr       g35 container ~a0f0
    'a0cf:  23 01 45 c8             jg              #01 g35 a0d9
    'a0d3:  c3 8f 45 01 14 4b       jg              g35 #0114 ~a0e2
    'a0d9:  f9 1b 5a 72 03 45       call_vn         169c8 #03 g35
    'a0df:  8c 00 06                jump            a0e6
    'a0e2:  4a 45 0e cc             test_attr       g35 open a0f0
    'a0e6:  f9 16 4e a2 4e 09 45    call_vn         13a88 #4e #09 g35
    'a0ed:  8c 00 47                jump            a135
    'a0f0:  23 05 45 cc             jg              #05 g35 a0fe
    'a0f4:  c3 8f 45 01 14 c6       jg              g35 #0114 a0fe
    'a0fa:  46 45 01 4e             jin             g35 "Class" ~a10a
    'a0fe:  f9 1b 5a 72 09 45       call_vn         169c8 #09 g35
    'a104:  0d fe 02                store           gee #02
    'a107:  8c 00 05                jump            a10d
    'a10a:  2d fe 45                store           gee g35
    'a10d:  0d ff 00                store           gef #00
    'a110:  a2 fe 00 49             get_child       gee -> sp ~a11b
    'a114:  95 ff                   inc             gef
    'a116:  a1 00 00 bf fb          get_sibling     sp -> sp a114
    'a11b:  e9 7f fe                pull            gee
    'a11e:  e8 bf ff                push            gef
    'a121:  a0 00 4c                jz              sp ~a12e
    'a124:  f9 16 4e a2 40 06 45    call_vn         13a88 #40 #06 g35
    'a12b:  8c 00 09                jump            a135
    'a12e:  f9 25 5c d0 01 00 00    call_vn         17340 local0 #00 #00
    'a135:  e0 27 5c a9 01 00 00    call_vs         172a4 local0 #00 -> sp
    'a13c:  41 00 38 d7             je              sp #38 a155
    'a140:  41 72 64 4c             je              g62 #64 ~a14e
    'a144:  f9 07 4e a2 10 07 2b    call_vn         13a88 #1007 #2b
    'a14b:  8c 00 09                jump            a155
    'a14e:  f9 07 4e a2 10 07 2c    call_vn         13a88 #1007 #2c
    'a155:  41 5c 12 54             je              g4c #12 ~a16b
    'a159:  0d 87 03                store           g77 #03
    'a15c:  a8 86 00                call_1s         g76 -> sp
    'a15f:  c1 8f 00 ff ff 48       je              sp #ffff ~a16b
    'a165:  2d 5d 5e                store           g4d g4e
    'a168:  8c fd 41                jump            9eaa
    'a16b:  8c f4 04                jump            9570
    'a16e:  63 90 91 c1             jg              g80 g81 rtrue
    'a172:  88 33 8d 06             call_1s         ce34 -> local5
    'a176:  c1 80 06 66 64 66 64 66 64 c9 
    '                              je              local5 "then" "then" "then" a187
    'a180:  c1 8f 06 53 32 00 8b    je              local5 "comma," ~a210
    'a187:  63 90 91 46             jg              g80 g81 ~a18f
    'a18b:  0d 97 00                store           g87 #00
    'a18e:  b0                      rtrue           
    'a18f:  d9 2f 33 b2 93 06       call_2s         cec8 g83 -> local5
    'a195:  d9 2f 33 b2 90 07       call_2s         cec8 g80 -> local6
    'a19b:  62 06 07 4e             jl              local5 local6 ~a1ab
    'a19f:  f9 25 5c b2 06 00 20    call_vn         172c8 local5 #00 #20
    'a1a6:  95 06                   inc             local5
    'a1a8:  8c ff f2                jump            a19b
    'a1ab:  88 33 8d 06             call_1s         ce34 -> local5
    'a1af:  c1 80 06 4f 03 58 30 4f 03 00 4c 
    '                              je              local5 "again" "g" "again" ~a204
    'a1ba:  55 90 02 00             sub             g80 #02 -> sp
    'a1be:  d9 2f 33 b2 00 00       call_2s         cec8 sp -> sp
    'a1c4:  d5 8f 00 41 b7 06       sub             sp #41b7 -> local5
    'a1ca:  63 90 91 48             jg              g80 g81 ~a1d4
    'a1ce:  0d 07 77                store           local6 #77
    'a1d1:  8c 00 0e                jump            a1e0
    'a1d4:  d9 2f 33 b2 90 00       call_2s         cec8 g80 -> sp
    'a1da:  d5 8f 00 41 b7 07       sub             sp #41b7 -> local6
    'a1e0:  62 06 07 62             jl              local5 local6 ~a204
    'a1e4:  42 06 00 c6             jl              local5 #00 a1ec
    'a1e8:  42 06 7b cf             jl              local5 #7b a1f9
    'a1ec:  fa 19 5f 5a 72 1e 06 7a 00 11 
    '                              call_vn2        169c8 #1e local5 #7a #00 #11
    'a1f6:  8c 00 08                jump            a1ff
    'a1f9:  e2 27 43 2f 06 20       storeb          #432f local5 #20
    'a1ff:  95 06                   inc             local5
    'a201:  8c ff de                jump            a1e0
    'a204:  f9 03 24 92 41 b7 42 32 call_vn         9248 #41b7 #4232
    'a20c:  0d 97 01                store           g87 #01
    'a20f:  b0                      rtrue           
    'a210:  0d 5d 02                store           g4d #02
    'a213:  8c fc 96                jump            9eaa
#End Region

    <Fact>
    Sub Advent_9524()
        Dim expected =
<![CDATA[
# temps: 657

LABEL 00
    temp00 <- read-word(3c7b)
    temp01 <- (temp00 = 01)
    if (temp01) is false then
        jump-to: LABEL 09
LABEL 01
    L05 <- 00
LABEL 02
    temp02 <- L05
    if (int16(temp02) < int16(08)) is false then
        jump-to: LABEL 08
LABEL 03
    temp03 <- L05
    if (int16(temp03) < int16(00)) is true then
        jump-to: LABEL 05
LABEL 04
    temp04 <- L05
    if (int16(temp04) < int16(10)) is true then
        jump-to: LABEL 06
LABEL 05
    temp05 <- L05
    discard: call 169c8 (1d, temp05, 0f, 01, 04)
    push-SP: 00
    jump-to: LABEL 07
LABEL 06
    temp06 <- L05
    push-SP: read-word(3ed7 + (temp06 * 2))
LABEL 07
    temp07 <- L00
    temp08 <- L05
    temp09 <- pop-SP
    discard: call 17340 (temp07, temp08, temp09)
    temp0a <- L05
    L05 <- (int16(temp0a) + int16(1))
    jump-to: LABEL 02
LABEL 08
    write-word(3c7b) <- 00
    return: 1
LABEL 09
    temp0b <- read-word(3cf7)
    temp0c <- (temp0b = 01)
    if (temp0c) is false then
        jump-to: LABEL 0b
LABEL 0a
    write-word(3cf7) <- 00
    discard: call 9248 (41b7, 4232)
    jump-to: LABEL 0c
LABEL 0b
    discard: call 9298 (41b7, 4232)
LABEL 0c
    write-word(3c65) <- 01
    write-word(3ceb) <- read-byte(4233)
    write-word(3ce9) <- 01
    discard: call 6a70 ()
    discard: call 9248 (41b7, 4232)
    discard: call 15f70 ()
    write-word(3ceb) <- read-byte(4233)
    L07 <- 00
    write-word(3cef) <- 01
    temp0d <- read-word(3c15)
    write-word(3c67) <- temp0d
    temp0e <- read-word(3c15)
    write-word(3c69) <- call a218 (temp0e)
    write-word(3cf1) <- 00
LABEL 0d
    write-word(3cd3) <- 00
    write-word(3c9b) <- ffff
LABEL 0e
    temp0f <- read-word(3cef)
    write-word(3ce9) <- temp0f
    write-word(3ced) <- call ce98 ()
    temp10 <- read-word(3ced)
    temp11 <- (temp10 = ffff)
    if (temp11) is false then
        jump-to: LABEL 10
LABEL 0f
    write-word(3c83) <- 01
    jump-to: LABEL 12c
LABEL 10
    temp12 <- read-word(3ced)
    temp13 <- (temp12 = 5830)
    temp14 <- (temp12 = 4f03)
    if (temp13 | temp14) is false then
        jump-to: LABEL 12
LABEL 11
    write-word(3ced) <- 4f03
LABEL 12
    temp15 <- read-word(3ced)
    temp16 <- (temp15 = 4f03)
    if (temp16) is false then
        jump-to: LABEL 24
LABEL 13
    temp17 <- read-word(3c67)
    temp18 <- read-word(3c15)
    temp19 <- (temp17 = temp18)
    if (temp19) is true then
        jump-to: LABEL 15
LABEL 14
    discard: call 13a88 (1007, 14)
    jump-to: LABEL 0b
LABEL 15
    temp1a <- read-byte(4330)
    if (temp1a = 0) is false then
        jump-to: LABEL 17
LABEL 16
    discard: call 13a88 (1007, 15)
    jump-to: LABEL 0b
LABEL 17
    L05 <- 00
LABEL 18
    temp1b <- L05
    if (int16(temp1b) < int16(78)) is false then
        jump-to: LABEL 23
LABEL 19
    temp1c <- L05
    if (int16(temp1c) < int16(00)) is true then
        jump-to: LABEL 1b
LABEL 1a
    temp1d <- L05
    if (int16(temp1d) < int16(7b)) is true then
        jump-to: LABEL 1c
LABEL 1b
    temp1e <- L05
    discard: call 169c8 (1c, temp1e, 7a, 00, 11)
    push-SP: 00
    jump-to: LABEL 1d
LABEL 1c
    temp1f <- L05
    push-SP: read-byte(432f + temp1f)
LABEL 1d
    temp20 <- L05
    if (int16(temp20) < int16(00)) is true then
        jump-to: LABEL 1f
LABEL 1e
    temp21 <- L05
    if (int16(temp21) < int16(7b)) is true then
        jump-to: LABEL 21
LABEL 1f
    temp22 <- L05
    discard: call 169c8 (1e, temp22, 7a, 00, 0d)
    temp23 <- pop-SP
    if (temp23 = 0) is false then
        jump-to: LABEL 20
LABEL 20
    jump-to: LABEL 22
LABEL 21
    temp24 <- L05
    temp25 <- pop-SP
    write-byte(41b7 + temp24) <- temp25
LABEL 22
    temp26 <- L05
    L05 <- (int16(temp26) + int16(1))
    jump-to: LABEL 18
LABEL 23
    jump-to: LABEL 0c
LABEL 24
    temp27 <- read-word(3ced)
    temp28 <- (temp27 = 4f03)
    if (temp28) is true then
        jump-to: LABEL 31
LABEL 25
    L05 <- 00
LABEL 26
    temp29 <- L05
    if (int16(temp29) < int16(78)) is false then
        jump-to: LABEL 31
LABEL 27
    temp2a <- L05
    if (int16(temp2a) < int16(00)) is true then
        jump-to: LABEL 29
LABEL 28
    temp2b <- L05
    if (int16(temp2b) < int16(7b)) is true then
        jump-to: LABEL 2a
LABEL 29
    temp2c <- L05
    discard: call 169c8 (1c, temp2c, 7a, 00, 0d)
    push-SP: 00
    jump-to: LABEL 2b
LABEL 2a
    temp2d <- L05
    push-SP: read-byte(41b7 + temp2d)
LABEL 2b
    temp2e <- L05
    if (int16(temp2e) < int16(00)) is true then
        jump-to: LABEL 2d
LABEL 2c
    temp2f <- L05
    if (int16(temp2f) < int16(7b)) is true then
        jump-to: LABEL 2f
LABEL 2d
    temp30 <- L05
    discard: call 169c8 (1e, temp30, 7a, 00, 11)
    temp31 <- pop-SP
    if (temp31 = 0) is false then
        jump-to: LABEL 2e
LABEL 2e
    jump-to: LABEL 30
LABEL 2f
    temp32 <- L05
    temp33 <- pop-SP
    write-byte(432f + temp32) <- temp33
LABEL 30
    temp34 <- L05
    L05 <- (int16(temp34) + int16(1))
    jump-to: LABEL 26
LABEL 31
    temp35 <- read-word(3cf1)
    if (temp35 = 0) is false then
        jump-to: LABEL 3f
LABEL 32
    temp36 <- read-word(3cef)
    L06 <- temp36
    temp37 <- read-word(3c67)
    L05 <- call e044 (temp37, 1c)
    temp38 <- L05
    temp39 <- (temp38 = 00)
    temp3a <- (temp38 = 01)
    if (temp39 | temp3a) is true then
        jump-to: LABEL 39
LABEL 33
    temp3b <- L05
    temp3c <- read-word(3cfd)
    temp3d <- call f06c (temp3b, temp3c)
    if (int16(temp3d) < int16(00)) is true then
        jump-to: LABEL 38
LABEL 34
    temp3e <- L05
    temp3f <- read-word(3d01)
    temp40 <- call f06c (temp3e, temp3f)
    if (int16(temp40) < int16(00)) is false then
        jump-to: LABEL 38
LABEL 35
    temp41 <- L05
    temp42 <- read-word(3cfd)
    temp43 <- (int16(temp41) - int16(temp42))
    write-word(3dc7) <- temp43
    temp44 <- read-word(3cff)
    write-word(3dc5) <- temp44
    temp45 <- read-word(3dc5)
    if (temp45 = 0) is false then
        jump-to: LABEL 37
LABEL 36
    discard: call 169c8 (14)
    temp46 <- read-word(3dc5)
    write-word(3dc5) <- (int16(temp46) + int16(1))
LABEL 37
    temp47 <- read-word(3dc7)
    temp48 <- read-word(3dc5)
    temp49 <- (int16(temp47) % int16(temp48))
    if (temp49 = 0) is true then
        jump-to: LABEL 39
LABEL 38
    temp4a <- L06
    write-word(3cf1) <- temp4a
    temp4b <- L05
    L05 <- (int16(00) - int16(temp4b))
LABEL 39
    temp4c <- L05
    temp4d <- (temp4c = 01)
    if (temp4d) is false then
        jump-to: LABEL 3b
LABEL 3a
    temp4e <- L00
    temp4f <- read-word(3c4d)
    discard: call 17340 (temp4e, 00, temp4f)
    temp50 <- L00
    temp51 <- read-word(3c53)
    discard: call 17340 (temp50, 01, temp51)
    temp52 <- L00
    temp53 <- read-word(3c55)
    discard: call 17340 (temp52, 02, temp53)
    return: 1
LABEL 3b
    temp54 <- L05
    if (temp54 = 0) is true then
        jump-to: LABEL 3d
LABEL 3c
    temp55 <- L05
    write-word(3ced) <- temp55
    temp56 <- read-word(3ce9)
    write-word(3ce9) <- (int16(temp56) - int16(1))
    temp57 <- read-word(3cef)
    write-word(3cef) <- (int16(temp57) - int16(1))
    jump-to: LABEL 3e
LABEL 3d
    temp58 <- read-word(3cef)
    write-word(3ce9) <- temp58
    write-word(3ced) <- call ce34 ()
LABEL 3e
    jump-to: LABEL 40
LABEL 3f
    write-word(3cf1) <- 00
LABEL 40
    temp59 <- read-word(3ced)
    if (temp59 = 0) is true then
        jump-to: LABEL 42
LABEL 41
    temp5a <- read-word(3ced)
    temp5b <- call 17284 (temp5a, 06)
    temp5c <- (temp5b & 01)
    if (temp5c = 0) is false then
        jump-to: LABEL 64
LABEL 42
    temp5d <- read-word(3cef)
    write-word(3ce9) <- temp5d
    write-word(3cb3) <- 00
    write-word(3ca5) <- 00
    L09 <- call aba4 (06, 00, 00)
    temp5e <- L09
    temp5f <- (temp5e = 2710)
    if (temp5f) is false then
        jump-to: LABEL 44
LABEL 43
    jump-to: LABEL 0c
LABEL 44
    temp60 <- L09
    if (temp60 = 0) is true then
        jump-to: LABEL 46
LABEL 45
    temp61 <- L00
    discard: call 17340 (temp61, 00, 1b)
    write-word(3c9b) <- 1b
    temp62 <- L00
    discard: call 17340 (temp62, 01, 01)
    temp63 <- L00
    temp64 <- L09
    discard: call 17340 (temp63, 02, temp64)
    jump-to: LABEL 191
LABEL 46
    temp65 <- read-word(3c67)
    temp66 <- read-word(3c15)
    temp67 <- (temp65 = temp66)
    if (temp67) is false then
        jump-to: LABEL 4e
LABEL 47
    L06 <- 02
LABEL 48
    temp68 <- L06
    temp69 <- read-word(3ceb)
    if (int16(temp68) > int16(temp69)) is true then
        jump-to: LABEL 4c
LABEL 49
    L05 <- call ce34 ()
    temp6a <- L05
    temp6b <- (temp6a = 5332)
    if (temp6b) is false then
        jump-to: LABEL 4b
LABEL 4a
    jump-to: LABEL 4f
LABEL 4b
    temp6c <- L06
    L06 <- (int16(temp6c) + int16(1))
    jump-to: LABEL 48
LABEL 4c
    temp6d <- read-word(3ced)
    write-word(3ced) <- call 15c2c (temp6d)
    temp6e <- read-word(3ced)
    if (temp6e = 0) is true then
        jump-to: LABEL 4e
LABEL 4d
    jump-to: LABEL 64
LABEL 4e
    write-word(3c83) <- 0c
    jump-to: LABEL 12c
LABEL 4f
    temp6f <- read-word(3ce9)
    L06 <- (int16(temp6f) - int16(01))
    temp70 <- L06
    temp71 <- (temp70 = 01)
    if (temp71) is false then
        jump-to: LABEL 51
LABEL 50
    discard: call 13a88 (1007, 16)
    jump-to: LABEL 0b
LABEL 51
    write-word(3ce9) <- 01
    write-word(3ca9) <- 01
    write-word(3cd1) <- 01
    temp72 <- read-word(3c15)
    temp73 <- read-word(3c69)
    L09 <- call aba4 (temp72, temp73, 06)
    write-word(3cd1) <- 00
    temp74 <- L09
    temp75 <- (temp74 = 2710)
    if (temp75) is false then
        jump-to: LABEL 53
LABEL 52
    jump-to: LABEL 0c
LABEL 53
    temp76 <- L09
    if (temp76 = 0) is false then
        jump-to: LABEL 55
LABEL 54
    discard: call 13a88 (1007, 17)
    jump-to: LABEL 0b
LABEL 55
    temp77 <- L09
    if (int16(01) > int16(temp77)) is true then
        jump-to: LABEL 57
LABEL 56
    temp78 <- L09
    if (int16(temp78) > int16(0114)) is false then
        jump-to: LABEL 58
LABEL 57
    temp79 <- L09
    discard: call 169c8 (03, temp79)
    jump-to: LABEL 59
LABEL 58
    temp7a <- L09
    temp7b <- (temp7a - 1)
    temp7c <- (temp7b * e)
    temp7d <- (temp7c + 188)
    temp7e <- read-byte(temp7d)
    temp7f <- (temp7e & 0080)
    temp80 <- (temp7f <> 0)
    if (temp80 = 1) is true then
        jump-to: LABEL 5e
LABEL 59
    temp81 <- L09
    if (int16(01) > int16(temp81)) is true then
        jump-to: LABEL 5b
LABEL 5a
    temp82 <- L09
    if (int16(temp82) > int16(0114)) is false then
        jump-to: LABEL 5c
LABEL 5b
    temp83 <- L09
    discard: call 169c8 (03, temp83)
    jump-to: LABEL 5d
LABEL 5c
    temp84 <- L09
    temp85 <- (temp84 - 1)
    temp86 <- (temp85 * e)
    temp87 <- (temp86 + 188)
    temp88 <- (temp87 + 2)
    temp89 <- read-byte(temp88)
    temp8a <- (temp89 & 0002)
    temp8b <- (temp8a <> 0)
    if (temp8b = 1) is true then
        jump-to: LABEL 5e
LABEL 5d
    temp8c <- L09
    discard: call 13a88 (1007, 18, temp8c)
    jump-to: LABEL 0b
LABEL 5e
    temp8d <- read-word(3ce9)
    temp8e <- L06
    temp8f <- (temp8d = temp8e)
    if (temp8f) is true then
        jump-to: LABEL 60
LABEL 5f
    discard: call 13a88 (1007, 19)
    jump-to: LABEL 0b
LABEL 60
    temp90 <- L09
    discard: call d444 (temp90)
    temp91 <- L06
    write-word(3cef) <- (int16(temp91) + int16(01))
    temp92 <- L09
    temp93 <- read-word(3c15)
    temp94 <- (temp92 = temp93)
    if (temp94) is false then
        jump-to: LABEL 63
LABEL 61
    temp95 <- read-word(3cef)
    write-word(3ce9) <- temp95
    temp96 <- call ce98 ()
    temp97 <- (temp96 = 4f03)
    temp98 <- (temp96 = 5830)
    temp99 <- (temp96 = 4f03)
    if ((temp97 | temp98) | temp99) is false then
        jump-to: LABEL 63
LABEL 62
    discard: call 13a88 (1007, 14)
    jump-to: LABEL 0b
LABEL 63
    temp9a <- L09
    write-word(3c67) <- temp9a
    temp9b <- L09
    write-word(3c69) <- call a218 (temp9b)
    jump-to: LABEL 0e
LABEL 64
    temp9c <- read-word(3ced)
    temp9d <- call 17284 (temp9c, 06)
    temp9e <- (temp9d & 02)
    write-word(3c6b) <- (int16(temp9e) / int16(02))
    temp9f <- read-word(3c6b)
    tempa0 <- (temp9f = 01)
    if (tempa0) is false then
        jump-to: LABEL 67
LABEL 65
    tempa1 <- read-word(3c67)
    tempa2 <- read-word(3c15)
    tempa3 <- (tempa1 = tempa2)
    if (tempa3) is true then
        jump-to: LABEL 67
LABEL 66
    write-word(3c83) <- 0c
    write-word(3c6b) <- 00
    jump-to: LABEL 12c
LABEL 67
    tempa4 <- read-word(3ced)
    tempa5 <- call 17284 (tempa4, 07)
    L05 <- (int16(ff) - int16(tempa5))
    tempa6 <- call 172a4 (0e, 00)
    tempa7 <- L05
    L01 <- call 172a4 (tempa6, tempa7)
    tempa8 <- L01
    tempa9 <- call 17284 (tempa8, 00)
    L03 <- (int16(tempa9) - int16(01))
    write-word(3cc9) <- ffff
    write-word(3ccb) <- ffff
    write-word(3c83) <- 01
    write-word(3c85) <- 01
    write-word(3c6d) <- 00
    tempaa <- L01
    L04 <- (int16(tempaa) + int16(01))
    L02 <- 00
LABEL 68
    tempab <- L02
    tempac <- L03
    if (int16(tempab) > int16(tempac)) is true then
        jump-to: LABEL 12c
LABEL 69
    L05 <- 00
LABEL 6a
    tempad <- L05
    if (int16(tempad) < int16(20)) is false then
        jump-to: LABEL 78
LABEL 6b
    tempae <- L05
    if (int16(tempae) < int16(00)) is true then
        jump-to: LABEL 6d
LABEL 6c
    tempaf <- L05
    if (int16(tempaf) < int16(20)) is true then
        jump-to: LABEL 6e
LABEL 6d
    tempb0 <- L05
    discard: call 169c8 (1f, tempb0, 1f, 01, 09)
    jump-to: LABEL 6f
LABEL 6e
    tempb1 <- L05
    write-word(3ff7 + (tempb1 * 2)) <- 0f
LABEL 6f
    tempb2 <- L05
    if (int16(tempb2) < int16(00)) is true then
        jump-to: LABEL 71
LABEL 70
    tempb3 <- L05
    if (int16(tempb3) < int16(20)) is true then
        jump-to: LABEL 72
LABEL 71
    tempb4 <- L05
    discard: call 169c8 (1f, tempb4, 1f, 01, 07)
    jump-to: LABEL 73
LABEL 72
    tempb5 <- L05
    write-word(3f77 + (tempb5 * 2)) <- 01
LABEL 73
    tempb6 <- L05
    if (int16(tempb6) < int16(00)) is true then
        jump-to: LABEL 75
LABEL 74
    tempb7 <- L05
    if (int16(tempb7) < int16(20)) is true then
        jump-to: LABEL 76
LABEL 75
    tempb8 <- L05
    discard: call 169c8 (1f, tempb8, 1f, 01, 08)
    jump-to: LABEL 77
LABEL 76
    tempb9 <- L05
    write-word(3fb7 + (tempb9 * 2)) <- 0f
LABEL 77
    tempba <- L05
    L05 <- (int16(tempba) + int16(1))
    jump-to: LABEL 6a
LABEL 78
    tempbb <- L04
    L04 <- call 9134 (tempbb)
    write-word(3c7f) <- 00
    write-word(3c95) <- 00
    write-word(3c8b) <- 00
    write-word(3c8d) <- 00
    write-word(3c71) <- 00
    write-word(3c73) <- 00
    write-word(3e57) <- 00
    write-word(3cb1) <- 00
    write-word(3c81) <- 01
    tempbc <- read-word(3cef)
    write-word(3ce9) <- (int16(tempbc) + int16(01))
    write-word(3c9f) <- ffff
    write-word(3cb3) <- 00
    L05 <- 00
    L0a <- 00
    write-word(3c87) <- 00
LABEL 79
    tempbd <- read-word(3c87)
    if (int16(tempbd) < int16(00)) is true then
        jump-to: LABEL 7b
LABEL 7a
    tempbe <- read-word(3c87)
    if (int16(tempbe) < int16(20)) is true then
        jump-to: LABEL 7c
LABEL 7b
    tempbf <- read-word(3c87)
    discard: call 169c8 (1d, tempbf, 1f, 01, 09)
    push-SP: 00
    jump-to: LABEL 7d
LABEL 7c
    tempc0 <- read-word(3c87)
    push-SP: read-word(3ff7 + (tempc0 * 2))
LABEL 7d
    tempc1 <- pop-SP
    tempc2 <- (tempc1 = 0f)
    if (tempc2) is true then
        jump-to: LABEL b6
LABEL 7e
    write-word(3cd3) <- 00
    tempc3 <- read-word(3c87)
    if (int16(tempc3) < int16(00)) is true then
        jump-to: LABEL 80
LABEL 7f
    tempc4 <- read-word(3c87)
    if (int16(tempc4) < int16(20)) is true then
        jump-to: LABEL 81
LABEL 80
    tempc5 <- read-word(3c87)
    discard: call 169c8 (1d, tempc5, 1f, 01, 07)
    push-SP: 00
    jump-to: LABEL 82
LABEL 81
    tempc6 <- read-word(3c87)
    push-SP: read-word(3f77 + (tempc6 * 2))
LABEL 82
    tempc7 <- pop-SP
    tempc8 <- (tempc7 = 02)
    if (tempc8) is true then
        jump-to: LABEL 84
LABEL 83
    tempc9 <- L05
    L05 <- (int16(tempc9) + int16(1))
LABEL 84
    tempca <- read-word(3c87)
    if (int16(tempca) < int16(00)) is true then
        jump-to: LABEL 86
LABEL 85
    tempcb <- read-word(3c87)
    if (int16(tempcb) < int16(20)) is true then
        jump-to: LABEL 87
LABEL 86
    tempcc <- read-word(3c87)
    discard: call 169c8 (1d, tempcc, 1f, 01, 07)
    push-SP: 00
    jump-to: LABEL 88
LABEL 87
    tempcd <- read-word(3c87)
    push-SP: read-word(3f77 + (tempcd * 2))
LABEL 88
    tempce <- pop-SP
    tempcf <- (tempce = 01)
    if (tempcf) is false then
        jump-to: LABEL b5
LABEL 89
    tempd0 <- read-word(3c87)
    if (int16(tempd0) < int16(00)) is true then
        jump-to: LABEL 8b
LABEL 8a
    tempd1 <- read-word(3c87)
    if (int16(tempd1) < int16(20)) is true then
        jump-to: LABEL 8c
LABEL 8b
    tempd2 <- read-word(3c87)
    discard: call 169c8 (1d, tempd2, 1f, 01, 08)
    push-SP: 00
    jump-to: LABEL 8d
LABEL 8c
    tempd3 <- read-word(3c87)
    push-SP: read-word(3fb7 + (tempd3 * 2))
LABEL 8d
    tempd4 <- pop-SP
    tempd5 <- (tempd4 = 02)
    if (tempd5) is false then
        jump-to: LABEL 8f
LABEL 8e
    L0a <- 01
LABEL 8f
    tempd6 <- read-word(3c87)
    if (int16(tempd6) < int16(00)) is true then
        jump-to: LABEL 91
LABEL 90
    tempd7 <- read-word(3c87)
    if (int16(tempd7) < int16(20)) is true then
        jump-to: LABEL 92
LABEL 91
    tempd8 <- read-word(3c87)
    discard: call 169c8 (1d, tempd8, 1f, 01, 08)
    push-SP: 00
    jump-to: LABEL 93
LABEL 92
    tempd9 <- read-word(3c87)
    push-SP: read-word(3fb7 + (tempd9 * 2))
LABEL 93
    tempda <- pop-SP
    tempdb <- (tempda = 04)
    tempdc <- (tempda = 05)
    if (tempdb | tempdc) is false then
        jump-to: LABEL b5
LABEL 94
    tempdd <- L05
    tempde <- (tempdd = 01)
    if (tempde) is false then
        jump-to: LABEL b5
LABEL 95
    tempdf <- read-word(3c87)
    write-word(3c87) <- (int16(tempdf) + int16(1))
    tempe0 <- read-word(3c87)
    if (int16(tempe0) < int16(00)) is true then
        jump-to: LABEL 97
LABEL 96
    tempe1 <- read-word(3c87)
    if (int16(tempe1) < int16(20)) is true then
        jump-to: LABEL 98
LABEL 97
    tempe2 <- read-word(3c87)
    discard: call 169c8 (1d, tempe2, 1f, 01, 07)
    push-SP: 00
    jump-to: LABEL 99
LABEL 98
    tempe3 <- read-word(3c87)
    push-SP: read-word(3f77 + (tempe3 * 2))
LABEL 99
    tempe4 <- pop-SP
    tempe5 <- (tempe4 = 02)
    if (tempe5) is false then
        jump-to: LABEL b4
LABEL 9a
    tempe6 <- read-word(3c87)
    if (int16(tempe6) < int16(00)) is true then
        jump-to: LABEL 9c
LABEL 9b
    tempe7 <- read-word(3c87)
    if (int16(tempe7) < int16(20)) is true then
        jump-to: LABEL 9d
LABEL 9c
    tempe8 <- read-word(3c87)
    discard: call 169c8 (1d, tempe8, 1f, 01, 07)
    push-SP: 00
    jump-to: LABEL 9e
LABEL 9d
    tempe9 <- read-word(3c87)
    push-SP: read-word(3f77 + (tempe9 * 2))
LABEL 9e
    tempea <- pop-SP
    tempeb <- (tempea = 02)
    if (tempeb) is false then
        jump-to: LABEL a0
LABEL 9f
    tempec <- read-word(3c87)
    write-word(3c87) <- (int16(tempec) + int16(1))
    jump-to: LABEL 9a
LABEL a0
    temped <- read-word(3c87)
    if (int16(temped) < int16(00)) is true then
        jump-to: LABEL a2
LABEL a1
    tempee <- read-word(3c87)
    if (int16(tempee) < int16(20)) is true then
        jump-to: LABEL a3
LABEL a2
    tempef <- read-word(3c87)
    discard: call 169c8 (1d, tempef, 1f, 01, 07)
    push-SP: 00
    jump-to: LABEL a4
LABEL a3
    tempf0 <- read-word(3c87)
    push-SP: read-word(3f77 + (tempf0 * 2))
LABEL a4
    tempf1 <- pop-SP
    tempf2 <- (tempf1 = 01)
    if (tempf2) is false then
        jump-to: LABEL b4
LABEL a5
    tempf3 <- read-word(3c87)
    if (int16(tempf3) < int16(00)) is true then
        jump-to: LABEL a7
LABEL a6
    tempf4 <- read-word(3c87)
    if (int16(tempf4) < int16(20)) is true then
        jump-to: LABEL a8
LABEL a7
    tempf5 <- read-word(3c87)
    discard: call 169c8 (1d, tempf5, 1f, 01, 08)
    push-SP: 00
    jump-to: LABEL a9
LABEL a8
    tempf6 <- read-word(3c87)
    push-SP: read-word(3fb7 + (tempf6 * 2))
LABEL a9
    tempf7 <- pop-SP
    if (tempf7 = 0) is false then
        jump-to: LABEL b4
LABEL aa
    tempf8 <- read-word(3ce9)
    tempf9 <- read-word(3ceb)
    if (int16(tempf8) < int16(tempf9)) is false then
        jump-to: LABEL b4
LABEL ab
    L09 <- call ce34 ()
    tempfa <- L09
    if (tempfa = 0) is true then
        jump-to: LABEL b3
LABEL ac
    tempfb <- L09
    tempfc <- call 17284 (tempfb, 06)
    tempfd <- (tempfc & 08)
    if (tempfd = 0) is true then
        jump-to: LABEL b3
LABEL ad
    L09 <- call a30c ()
    tempfe <- L09
    if (tempfe = 0) is true then
        jump-to: LABEL af
LABEL ae
    tempff <- L09
    write-word(3c81) <- tempff
LABEL af
    temp100 <- read-word(3c69)
    temp101 <- read-word(3c67)
    L09 <- call aba4 (temp100, temp101, 00)
    temp102 <- L09
    temp103 <- (temp102 = 2710)
    if (temp103) is false then
        jump-to: LABEL b1
LABEL b0
    jump-to: LABEL 0c
LABEL b1
    temp104 <- L09
    if (int16(temp104) < int16(02)) is true then
        jump-to: LABEL b3
LABEL b2
    temp105 <- L09
    write-word(3c9f) <- temp105
LABEL b3
    jump-to: LABEL aa
LABEL b4
    jump-to: LABEL b6
LABEL b5
    temp106 <- read-word(3c87)
    write-word(3c87) <- (int16(temp106) + int16(1))
    jump-to: LABEL 79
LABEL b6
    write-word(3cc5) <- 00
    temp107 <- L0a
    if (temp107 = 0) is true then
        jump-to: LABEL ba
LABEL b7
    temp108 <- read-word(3c93)
    temp109 <- (temp108 = 01)
    if (temp109) is false then
        jump-to: LABEL ba
LABEL b8
    temp10a <- read-word(3c9b)
    temp10b <- (temp10a = 4e)
    if (temp10b) is false then
        jump-to: LABEL ba
LABEL b9
    write-word(3cc5) <- 01
LABEL ba
    write-word(3c7f) <- 00
    write-word(3c95) <- 00
    write-word(3c8b) <- 00
    write-word(3c8d) <- 00
    write-word(3c71) <- 00
    write-word(3c73) <- 00
    write-word(3e57) <- 00
    write-word(3c81) <- 01
    temp10c <- read-word(3cef)
    write-word(3ce9) <- (int16(temp10c) + int16(01))
    write-word(3c87) <- 01
LABEL bb
    temp10d <- read-word(3c87)
    if (int16(temp10d) < int16(00)) is true then
        jump-to: LABEL bd
LABEL bc
    temp10e <- read-word(3c87)
    if (int16(temp10e) < int16(20)) is true then
        jump-to: LABEL be
LABEL bd
    temp10f <- read-word(3c87)
    discard: call 169c8 (1f, temp10f, 1f, 01, 05)
    jump-to: LABEL bf
LABEL be
    temp110 <- read-word(3c87)
    write-word(3ef7 + (temp110 * 2)) <- ffff
LABEL bf
    write-word(3cd3) <- 00
    temp111 <- read-word(3c87)
    temp112 <- (int16(temp111) - int16(01))
    write-word(3dc5) <- temp112
    temp113 <- read-word(3dc5)
    push-SP: temp113
    temp114 <- read-word(3dc5)
    if (int16(temp114) < int16(00)) is true then
        jump-to: LABEL c1
LABEL c0
    temp115 <- read-word(3dc5)
    if (int16(temp115) < int16(20)) is true then
        jump-to: LABEL c3
LABEL c1
    temp116 <- read-word(3dc5)
    discard: call 169c8 (1d, temp116, 1f, 01, 09)
    temp117 <- pop-SP
    if (temp117 = 0) is false then
        jump-to: LABEL c2
LABEL c2
    jump-to: LABEL c4
LABEL c3
    temp118 <- pop-SP
    L08 <- read-word(3ff7 + (temp118 * 2))
LABEL c4
    temp119 <- read-word(3c87)
    if (int16(temp119) < int16(00)) is true then
        jump-to: LABEL c6
LABEL c5
    temp11a <- read-word(3c87)
    if (int16(temp11a) < int16(20)) is true then
        jump-to: LABEL c7
LABEL c6
    temp11b <- read-word(3c87)
    discard: call 169c8 (1d, temp11b, 1f, 01, 09)
    jump-to: LABEL c8
LABEL c7
    temp11c <- read-word(3c87)
    write-word(3ca9) <- read-word(3ff7 + (temp11c * 2))
LABEL c8
    temp11d <- L08
    temp11e <- (temp11d = 0f)
    if (temp11e) is true then
        jump-to: LABEL ea
LABEL c9
    write-word(3cd1) <- 00
    write-word(3c65) <- 01
    temp11f <- L08
    discard: call 9110 (temp11f)
    temp120 <- read-word(3c9b)
    temp121 <- (temp120 = 5d)
    if (temp121) is false then
        jump-to: LABEL cd
LABEL ca
    temp122 <- read-word(3ca1)
    temp123 <- (temp122 = 01)
    if (temp123) is false then
        jump-to: LABEL cd
LABEL cb
    temp124 <- read-word(3ca3)
    temp125 <- (temp124 = 09)
    if (temp125) is false then
        jump-to: LABEL cd
LABEL cc
    L09 <- read-word(3e3b)
    temp126 <- read-word(3ce9)
    write-word(3ce9) <- (int16(temp126) - int16(1))
    temp127 <- read-word(3ce9)
    L06 <- temp127
    jump-to: LABEL 55
LABEL cd
    temp128 <- read-word(3c87)
    push-SP: (int16(temp128) - int16(01))
    temp129 <- read-word(3ca1)
    temp12a <- read-word(3ca3)
    temp12b <- pop-SP
    temp12c <- L08
    L09 <- call a600 (temp129, temp12a, temp12b, temp12c)
LABEL ce
    temp12d <- L09
    if (int16(temp12d) < int16(ff38)) is false then
        jump-to: LABEL d0
LABEL cf
    temp12e <- L09
    temp12f <- (int16(temp12e) + int16(0100))
    L09 <- call a600 (01, temp12f)
    jump-to: LABEL ce
LABEL d0
    write-word(3cd1) <- 00
    temp130 <- L09
    if (temp130 = 0) is false then
        jump-to: LABEL d6
LABEL d1
    temp131 <- read-word(3ca1)
    temp132 <- (temp131 = 02)
    if (temp132) is true then
        jump-to: LABEL d5
LABEL d2
    temp133 <- read-word(3ca1)
    temp134 <- (temp133 = 01)
    if (temp134) is false then
        jump-to: LABEL d4
LABEL d3
    temp135 <- read-word(3ca3)
    temp136 <- (temp135 = 09)
    if (temp136) is true then
        jump-to: LABEL d5
LABEL d4
    temp137 <- read-word(3c93)
    write-word(3c93) <- (int16(temp137) - int16(1))
LABEL d5
    L09 <- 01
    jump-to: LABEL e5
LABEL d6
    temp138 <- L09
    if (int16(temp138) < int16(00)) is false then
        jump-to: LABEL d8
LABEL d7
    L09 <- 00
    jump-to: LABEL e5
LABEL d8
    temp139 <- L09
    temp13a <- (temp139 = 2710)
    if (temp13a) is true then
        jump-to: LABEL e5
LABEL d9
    temp13b <- L09
    temp13c <- (temp13b = 01)
    if (temp13c) is false then
        jump-to: LABEL de
LABEL da
    temp13d <- read-word(3c8d)
    if (temp13d = 0) is false then
        jump-to: LABEL dc
LABEL db
    temp13e <- read-word(3c75)
    write-word(3c8f) <- temp13e
    jump-to: LABEL dd
LABEL dc
    temp13f <- read-word(3c75)
    write-word(3c91) <- temp13f
LABEL dd
    temp140 <- read-word(3c8d)
    write-word(3c8d) <- (int16(temp140) + int16(1))
    L09 <- 01
LABEL de
    temp141 <- L09
    temp142 <- (temp141 = 02)
    if (temp142) is false then
        jump-to: LABEL e0
LABEL df
    L09 <- 00
LABEL e0
    temp143 <- read-word(3c8b)
    push-SP: (int16(temp143) + int16(02))
    temp144 <- L00
    temp145 <- pop-SP
    temp146 <- L09
    discard: call 17340 (temp144, temp145, temp146)
    temp147 <- read-word(3c8b)
    write-word(3c8b) <- (int16(temp147) + int16(1))
    temp148 <- read-word(3c87)
    if (int16(temp148) < int16(00)) is true then
        jump-to: LABEL e2
LABEL e1
    temp149 <- read-word(3c87)
    if (int16(temp149) < int16(20)) is true then
        jump-to: LABEL e3
LABEL e2
    temp14a <- read-word(3c87)
    discard: call 169c8 (1f, temp14a, 1f, 01, 05)
    jump-to: LABEL e4
LABEL e3
    temp14b <- read-word(3c87)
    temp14c <- L09
    write-word(3ef7 + (temp14b * 2)) <- temp14c
LABEL e4
    L09 <- 01
LABEL e5
    temp14d <- L09
    temp14e <- (temp14d = 2710)
    if (temp14e) is false then
        jump-to: LABEL e7
LABEL e6
    jump-to: LABEL 0c
LABEL e7
    temp14f <- L09
    if (temp14f = 0) is false then
        jump-to: LABEL e9
LABEL e8
    jump-to: LABEL 123
LABEL e9
    jump-to: LABEL 122
LABEL ea
    temp150 <- read-word(3ce9)
    temp151 <- read-word(3ceb)
    if (int16(temp150) > int16(temp151)) is true then
        jump-to: LABEL fb
LABEL eb
    L09 <- call ce34 ()
    temp152 <- L09
    temp153 <- (temp152 = 6664)
    temp154 <- (temp152 = 6664)
    temp155 <- (temp152 = 6664)
    if ((temp153 | temp154) | temp155) is true then
        jump-to: LABEL ed
LABEL ec
    temp156 <- L09
    temp157 <- (temp156 = 5332)
    if (temp157) is false then
        jump-to: LABEL ee
LABEL ed
    write-word(3cf7) <- 01
    temp158 <- read-word(3ce9)
    write-word(3cf9) <- (int16(temp158) - int16(01))
    jump-to: LABEL fb
LABEL ee
    L0a <- 00
LABEL ef
    temp159 <- L0a
    if (int16(temp159) < int16(20)) is false then
        jump-to: LABEL fa
LABEL f0
    temp15a <- L0a
    if (int16(temp15a) < int16(00)) is true then
        jump-to: LABEL f2
LABEL f1
    temp15b <- L0a
    if (int16(temp15b) < int16(20)) is true then
        jump-to: LABEL f3
LABEL f2
    temp15c <- L0a
    discard: call 169c8 (1d, temp15c, 1f, 01, 05)
    push-SP: 00
    jump-to: LABEL f4
LABEL f3
    temp15d <- L0a
    push-SP: read-word(3ef7 + (temp15d * 2))
LABEL f4
    temp15e <- L0a
    if (int16(temp15e) < int16(00)) is true then
        jump-to: LABEL f6
LABEL f5
    temp15f <- L0a
    if (int16(temp15f) < int16(20)) is true then
        jump-to: LABEL f8
LABEL f6
    temp160 <- L0a
    discard: call 169c8 (1f, temp160, 1f, 01, 06)
    temp161 <- pop-SP
    if (temp161 = 0) is false then
        jump-to: LABEL f7
LABEL f7
    jump-to: LABEL f9
LABEL f8
    temp162 <- L0a
    temp163 <- pop-SP
    write-word(3f37 + (temp162 * 2)) <- temp163
LABEL f9
    temp164 <- L0a
    L0a <- (int16(temp164) + int16(1))
    jump-to: LABEL ef
LABEL fa
    temp165 <- read-word(3c87)
    write-word(3c89) <- temp165
    write-word(3c81) <- 02
    jump-to: LABEL 123
LABEL fb
    temp166 <- read-word(3c8b)
    if (int16(temp166) < int16(01)) is true then
        jump-to: LABEL ff
LABEL fc
    temp167 <- L00
    temp168 <- call 172a4 (temp167, 02)
    if (temp168 = 0) is false then
        jump-to: LABEL ff
LABEL fd
    temp169 <- L00
    temp16a <- call 172a4 (temp169, 03)
    L09 <- call baa4 (temp16a)
    temp16b <- L09
    if (temp16b = 0) is true then
        jump-to: LABEL ff
LABEL fe
    temp16c <- L09
    write-word(3c81) <- temp16c
    temp16d <- L00
    temp16e <- read-word(3c9b)
    discard: call 17340 (temp16d, 00, temp16e)
    jump-to: LABEL 123
LABEL ff
    temp16f <- read-word(3c8b)
    if (int16(temp16f) < int16(02)) is true then
        jump-to: LABEL 103
LABEL 100
    temp170 <- L00
    temp171 <- call 172a4 (temp170, 03)
    if (temp171 = 0) is false then
        jump-to: LABEL 103
LABEL 101
    temp172 <- L00
    temp173 <- call 172a4 (temp172, 02)
    L09 <- call baa4 (temp173)
    temp174 <- L09
    if (temp174 = 0) is true then
        jump-to: LABEL 103
LABEL 102
    temp175 <- L09
    write-word(3c81) <- temp175
    jump-to: LABEL 123
LABEL 103
    temp176 <- read-word(3cc5)
    temp177 <- (temp176 = 02)
    if (temp177) is false then
        jump-to: LABEL 106
LABEL 104
    temp178 <- L00
    temp179 <- call 172a4 (temp178, 02)
    temp17a <- read-word(3c67)
    temp17b <- (temp179 = temp17a)
    if (temp17b) is false then
        jump-to: LABEL 106
LABEL 105
    write-word(3c83) <- 11
    jump-to: LABEL 12c
LABEL 106
    write-word(3cf3) <- 00
    temp17c <- read-word(3c95)
    if (temp17c = 0) is true then
        jump-to: LABEL 108
LABEL 107
    print: "("
    temp17d <- read-word(3c95)
    discard: call c268 (temp17d)
    print: ")\n"
LABEL 108
    temp17e <- L00
    temp17f <- read-word(3c9b)
    discard: call 17340 (temp17e, 00, temp17f)
    temp180 <- L00
    temp181 <- read-word(3c8b)
    discard: call 17340 (temp180, 01, temp181)
    temp182 <- read-word(3c9d)
    if (temp182 = 0) is true then
        jump-to: LABEL 10c
LABEL 109
    temp183 <- read-word(3c8b)
    temp184 <- (temp183 = 02)
    if (temp184) is false then
        jump-to: LABEL 10c
LABEL 10a
    temp185 <- L00
    L05 <- call 172a4 (temp185, 02)
    temp186 <- L00
    push-SP: call 172a4 (temp186, 03)
    temp187 <- L00
    temp188 <- pop-SP
    discard: call 17340 (temp187, 02, temp188)
    temp189 <- L00
    temp18a <- L05
    discard: call 17340 (temp189, 03, temp18a)
    temp18b <- read-word(3c8d)
    temp18c <- (temp18b = 02)
    if (temp18c) is false then
        jump-to: LABEL 10c
LABEL 10b
    temp18d <- read-word(3c8f)
    L05 <- temp18d
    temp18e <- read-word(3c91)
    write-word(3c8f) <- temp18e
    temp18f <- L05
    write-word(3c91) <- temp18f
LABEL 10c
    temp190 <- read-word(3c8b)
    if (int16(temp190) > int16(00)) is false then
        jump-to: LABEL 10f
LABEL 10d
    temp191 <- L00
    temp192 <- call 172a4 (temp191, 02)
    if (int16(temp192) < int16(02)) is true then
        jump-to: LABEL 10f
LABEL 10e
    temp193 <- L00
    temp194 <- call 172a4 (temp193, 02)
    discard: call d444 (temp194)
LABEL 10f
    temp195 <- read-word(3c7f)
    if (temp195 = 0) is true then
        jump-to: LABEL 11f
LABEL 110
    temp196 <- read-word(3c67)
    temp197 <- read-word(3c15)
    temp198 <- (temp196 = temp197)
    if (temp198) is false then
        jump-to: LABEL 11f
LABEL 111
    write-word(3c4d) <- 4e
    temp199 <- read-word(3c7f)
    L05 <- call e044 (temp199, 49)
    temp19a <- L05
    if (int16(temp19a) > int16(02)) is false then
        jump-to: LABEL 113
LABEL 112
    write-word(3c83) <- 06
    jump-to: LABEL 12c
LABEL 113
    temp19b <- L05
    if (int16(temp19b) < int16(02)) is false then
        jump-to: LABEL 11f
LABEL 114
    temp19c <- L05
    temp19d <- (temp19c = 01)
    if (temp19d) is true then
        jump-to: LABEL 116
LABEL 115
    temp19e <- read-word(3c7f)
    discard: call 13a88 (1007, 1a, temp19e)
LABEL 116
    write-word(3c7b) <- 01
    L05 <- 00
LABEL 117
    temp19f <- L05
    if (int16(temp19f) < int16(08)) is false then
        jump-to: LABEL 11e
LABEL 118
    temp1a0 <- L00
    temp1a1 <- L05
    push-SP: call 172a4 (temp1a0, temp1a1)
    temp1a2 <- L05
    if (int16(temp1a2) < int16(00)) is true then
        jump-to: LABEL 11a
LABEL 119
    temp1a3 <- L05
    if (int16(temp1a3) < int16(10)) is true then
        jump-to: LABEL 11c
LABEL 11a
    temp1a4 <- L05
    discard: call 169c8 (1f, temp1a4, 0f, 01, 04)
    temp1a5 <- pop-SP
    if (temp1a5 = 0) is false then
        jump-to: LABEL 11b
LABEL 11b
    jump-to: LABEL 11d
LABEL 11c
    temp1a6 <- L05
    temp1a7 <- pop-SP
    write-word(3ed7 + (temp1a6 * 2)) <- temp1a7
LABEL 11d
    temp1a8 <- L05
    L05 <- (int16(temp1a8) + int16(1))
    jump-to: LABEL 117
LABEL 11e
    temp1a9 <- L00
    discard: call 17340 (temp1a9, 00, 4e)
    temp1aa <- L00
    discard: call 17340 (temp1aa, 01, 01)
    temp1ab <- L00
    temp1ac <- read-word(3c7f)
    discard: call 17340 (temp1ab, 02, temp1ac)
LABEL 11f
    temp1ad <- read-word(3cf7)
    temp1ae <- (temp1ad = 01)
    if (temp1ae) is false then
        jump-to: LABEL 121
LABEL 120
    temp1af <- read-word(3cf9)
    write-word(3ce9) <- temp1af
    jump-to: LABEL 191
LABEL 121
    return: 1
LABEL 122
    temp1b0 <- read-word(3c87)
    write-word(3c87) <- (int16(temp1b0) + int16(1))
    jump-to: LABEL bb
LABEL 123
    temp1b1 <- read-word(3c81)
    temp1b2 <- read-word(3c83)
    if (int16(temp1b1) > int16(temp1b2)) is false then
        jump-to: LABEL 125
LABEL 124
    temp1b3 <- read-word(3c81)
    write-word(3c83) <- temp1b3
LABEL 125
    temp1b4 <- read-word(3c81)
    temp1b5 <- (temp1b4 = 12)
    if (temp1b5) is true then
        jump-to: LABEL 128
LABEL 126
    temp1b6 <- read-word(3c81)
    temp1b7 <- read-word(3c85)
    if (int16(temp1b6) > int16(temp1b7)) is false then
        jump-to: LABEL 128
LABEL 127
    temp1b8 <- read-word(3c81)
    write-word(3c85) <- temp1b8
LABEL 128
    temp1b9 <- read-word(3cc5)
    temp1ba <- (temp1b9 = 02)
    if (temp1ba) is false then
        jump-to: LABEL 12b
LABEL 129
    temp1bb <- read-word(3c81)
    temp1bc <- (temp1bb = 11)
    if (temp1bc) is false then
        jump-to: LABEL 12b
LABEL 12a
    jump-to: LABEL 12c
LABEL 12b
    temp1bd <- L02
    L02 <- (int16(temp1bd) + int16(1))
    jump-to: LABEL 68
LABEL 12c
    temp1be <- read-word(3c83)
    write-word(3c81) <- temp1be
    temp1bf <- read-word(3c67)
    temp1c0 <- read-word(3c15)
    temp1c1 <- (temp1bf = temp1c0)
    if (temp1c1) is true then
        jump-to: LABEL 132
LABEL 12d
    temp1c2 <- read-word(3cf1)
    if (temp1c2 = 0) is true then
        jump-to: LABEL 12f
LABEL 12e
    temp1c3 <- read-word(3cf1)
    write-word(3cef) <- temp1c3
    jump-to: LABEL 0d
LABEL 12f
    temp1c4 <- read-word(3cef)
    write-word(3ce9) <- temp1c4
    write-word(3c71) <- call ce34 ()
    temp1c5 <- read-word(3c71)
    temp1c6 <- (temp1c5 = 5332)
    if (temp1c6) is false then
        jump-to: LABEL 131
LABEL 130
    write-word(3c71) <- call ce34 ()
    temp1c7 <- read-word(3cef)
    write-word(3cef) <- (int16(temp1c7) + int16(1))
LABEL 131
    temp1c8 <- read-word(3cef)
    write-word(3c73) <- call cf30 (temp1c8)
    temp1c9 <- L00
    discard: call 17340 (temp1c9, 00, 1009)
    temp1ca <- L00
    discard: call 17340 (temp1ca, 01, 02)
    temp1cb <- L00
    discard: call 17340 (temp1cb, 02, 01)
    temp1cc <- read-word(3c71)
    write-word(3c8f) <- temp1cc
    temp1cd <- L00
    temp1ce <- read-word(3c67)
    discard: call 17340 (temp1cd, 03, temp1ce)
    temp1cf <- read-word(3cef)
    write-word(3c77) <- temp1cf
    temp1d0 <- read-word(3ceb)
    temp1d1 <- read-word(3c77)
    temp1d2 <- (int16(temp1d0) - int16(temp1d1))
    write-word(3c79) <- (int16(temp1d2) + int16(01))
    return: 1
LABEL 132
    temp1d3 <- read-word(3c81)
    temp1d4 <- call 15f94 (temp1d3)
    if (temp1d4 = 0) is true then
        jump-to: LABEL 134
LABEL 133
    jump-to: LABEL 0b
LABEL 134
    temp1d5 <- read-word(3ccd)
    write-word(3cc9) <- temp1d5
    temp1d6 <- read-word(3ccf)
    write-word(3ccb) <- temp1d6
    temp1d7 <- read-word(3c81)
    temp1d8 <- (temp1d7 = 01)
    if (temp1d8) is false then
        jump-to: LABEL 136
LABEL 135
    discard: call 13a88 (1007, 1b)
    write-word(3cf3) <- 01
LABEL 136
    temp1d9 <- read-word(3c81)
    temp1da <- (temp1d9 = 02)
    if (temp1da) is false then
        jump-to: LABEL 144
LABEL 137
    discard: call 13a88 (1007, 1c)
    L0a <- 00
LABEL 138
    temp1db <- L0a
    if (int16(temp1db) < int16(20)) is false then
        jump-to: LABEL 143
LABEL 139
    temp1dc <- L0a
    if (int16(temp1dc) < int16(00)) is true then
        jump-to: LABEL 13b
LABEL 13a
    temp1dd <- L0a
    if (int16(temp1dd) < int16(20)) is true then
        jump-to: LABEL 13c
LABEL 13b
    temp1de <- L0a
    discard: call 169c8 (1d, temp1de, 1f, 01, 06)
    push-SP: 00
    jump-to: LABEL 13d
LABEL 13c
    temp1df <- L0a
    push-SP: read-word(3f37 + (temp1df * 2))
LABEL 13d
    temp1e0 <- L0a
    if (int16(temp1e0) < int16(00)) is true then
        jump-to: LABEL 13f
LABEL 13e
    temp1e1 <- L0a
    if (int16(temp1e1) < int16(20)) is true then
        jump-to: LABEL 141
LABEL 13f
    temp1e2 <- L0a
    discard: call 169c8 (1f, temp1e2, 1f, 01, 05)
    temp1e3 <- pop-SP
    if (temp1e3 = 0) is false then
        jump-to: LABEL 140
LABEL 140
    jump-to: LABEL 142
LABEL 141
    temp1e4 <- L0a
    temp1e5 <- pop-SP
    write-word(3ef7 + (temp1e4 * 2)) <- temp1e5
LABEL 142
    temp1e6 <- L0a
    L0a <- (int16(temp1e6) + int16(1))
    jump-to: LABEL 138
LABEL 143
    temp1e7 <- read-word(3c89)
    write-word(3c87) <- temp1e7
    discard: call c268 (00)
    discard: call 13a88 (1007, 38)
LABEL 144
    temp1e8 <- read-word(3c81)
    temp1e9 <- (temp1e8 = 03)
    if (temp1e9) is false then
        jump-to: LABEL 146
LABEL 145
    discard: call 13a88 (1007, 1d)
LABEL 146
    temp1ea <- read-word(3c81)
    temp1eb <- (temp1ea = 04)
    if (temp1eb) is false then
        jump-to: LABEL 148
LABEL 147
    discard: call 13a88 (1007, 1e)
    temp1ec <- read-word(3cf5)
    write-word(3cf3) <- temp1ec
LABEL 148
    temp1ed <- read-word(3c81)
    temp1ee <- (temp1ed = 05)
    if (temp1ee) is false then
        jump-to: LABEL 14a
LABEL 149
    discard: call 13a88 (1007, 1f)
LABEL 14a
    temp1ef <- read-word(3c81)
    temp1f0 <- (temp1ef = 06)
    if (temp1f0) is false then
        jump-to: LABEL 14c
LABEL 14b
    discard: call 13a88 (1007, 20)
    temp1f1 <- read-word(3cf5)
    write-word(3cf3) <- temp1f1
LABEL 14c
    temp1f2 <- read-word(3c81)
    temp1f3 <- (temp1f2 = 07)
    if (temp1f3) is false then
        jump-to: LABEL 14e
LABEL 14d
    discard: call 13a88 (1007, 21)
LABEL 14e
    temp1f4 <- read-word(3c81)
    temp1f5 <- (temp1f4 = 08)
    if (temp1f5) is false then
        jump-to: LABEL 150
LABEL 14f
    discard: call 13a88 (1007, 22)
LABEL 150
    temp1f6 <- read-word(3c81)
    temp1f7 <- (temp1f6 = 09)
    if (temp1f7) is false then
        jump-to: LABEL 152
LABEL 151
    discard: call 13a88 (1007, 23)
LABEL 152
    temp1f8 <- read-word(3c81)
    temp1f9 <- (temp1f8 = 0a)
    if (temp1f9) is false then
        jump-to: LABEL 154
LABEL 153
    discard: call 13a88 (1007, 24)
LABEL 154
    temp1fa <- read-word(3c81)
    temp1fb <- (temp1fa = 0b)
    if (temp1fb) is false then
        jump-to: LABEL 156
LABEL 155
    discard: call 13a88 (1007, 25)
LABEL 156
    temp1fc <- read-word(3c81)
    temp1fd <- (temp1fc = 0c)
    if (temp1fd) is false then
        jump-to: LABEL 158
LABEL 157
    discard: call 13a88 (1007, 26)
LABEL 158
    temp1fe <- read-word(3c81)
    temp1ff <- (temp1fe = 0d)
    if (temp1ff) is false then
        jump-to: LABEL 15a
LABEL 159
    discard: call 13a88 (1007, 27)
LABEL 15a
    temp200 <- read-word(3c81)
    temp201 <- (temp200 = 0e)
    if (temp201) is false then
        jump-to: LABEL 15e
LABEL 15b
    temp202 <- read-word(3ccb)
    temp203 <- (temp202 = ffff)
    if (temp203) is false then
        jump-to: LABEL 15d
LABEL 15c
    discard: call 13a88 (1007, 23)
    jump-to: LABEL 15e
LABEL 15d
    discard: call 13a88 (1007, 28)
LABEL 15e
    temp204 <- read-word(3c81)
    temp205 <- (temp204 = 0f)
    if (temp205) is false then
        jump-to: LABEL 160
LABEL 15f
    discard: call 13a88 (1007, 29)
LABEL 160
    temp206 <- read-word(3c81)
    temp207 <- (temp206 = 10)
    if (temp207) is false then
        jump-to: LABEL 162
LABEL 161
    temp208 <- read-word(3caf)
    discard: call 13a88 (1007, 2a, temp208)
LABEL 162
    temp209 <- read-word(3c81)
    temp20a <- (temp209 = 11)
    if (temp20a) is false then
        jump-to: LABEL 18a
LABEL 163
    temp20b <- L00
    temp20c <- call 172a4 (temp20b, 00)
    temp20d <- (temp20c = 38)
    if (temp20d) is false then
        jump-to: LABEL 186
LABEL 164
    temp20e <- L00
    temp20f <- call 172a4 (temp20e, 03)
    temp210 <- call 1682c (temp20f, 02)
    if (temp210 = 0) is true then
        jump-to: LABEL 186
LABEL 165
    temp211 <- L00
    write-word(3c53) <- call 172a4 (temp211, 03)
    temp212 <- read-word(3c53)
    if (int16(01) > int16(temp212)) is true then
        jump-to: LABEL 167
LABEL 166
    temp213 <- read-word(3c53)
    if (int16(temp213) > int16(0114)) is false then
        jump-to: LABEL 168
LABEL 167
    temp214 <- read-word(3c53)
    discard: call 169c8 (03, temp214)
    jump-to: LABEL 16a
LABEL 168
    temp215 <- read-word(3c53)
    temp216 <- (temp215 - 1)
    temp217 <- (temp216 * e)
    temp218 <- (temp217 + 188)
    temp219 <- read-byte(temp218)
    temp21a <- (temp219 & 0080)
    temp21b <- (temp21a <> 0)
    if (temp21b = 1) is false then
        jump-to: LABEL 16a
LABEL 169
    temp21c <- read-word(3c53)
    discard: call 13a88 (4e, 06, temp21c)
    jump-to: LABEL 186
LABEL 16a
    temp21d <- read-word(3c53)
    if (int16(01) > int16(temp21d)) is true then
        jump-to: LABEL 16c
LABEL 16b
    temp21e <- read-word(3c53)
    if (int16(temp21e) > int16(0114)) is false then
        jump-to: LABEL 16d
LABEL 16c
    temp21f <- read-word(3c53)
    discard: call 169c8 (03, temp21f)
    jump-to: LABEL 16e
LABEL 16d
    temp220 <- read-word(3c53)
    temp221 <- (temp220 - 1)
    temp222 <- (temp221 * e)
    temp223 <- (temp222 + 188)
    temp224 <- read-byte(temp223)
    temp225 <- (temp224 & 0008)
    temp226 <- (temp225 <> 0)
    if (temp226 = 1) is true then
        jump-to: LABEL 173
LABEL 16e
    temp227 <- read-word(3c53)
    if (int16(01) > int16(temp227)) is true then
        jump-to: LABEL 170
LABEL 16f
    temp228 <- read-word(3c53)
    if (int16(temp228) > int16(0114)) is false then
        jump-to: LABEL 171
LABEL 170
    temp229 <- read-word(3c53)
    discard: call 169c8 (03, temp229)
    jump-to: LABEL 172
LABEL 171
    temp22a <- read-word(3c53)
    temp22b <- (temp22a - 1)
    temp22c <- (temp22b * e)
    temp22d <- (temp22c + 188)
    temp22e <- (temp22d + 2)
    temp22f <- read-byte(temp22e)
    temp230 <- (temp22f & 0008)
    temp231 <- (temp230 <> 0)
    if (temp231 = 1) is true then
        jump-to: LABEL 173
LABEL 172
    temp232 <- read-word(3c53)
    discard: call 13a88 (1c, 02, temp232)
    jump-to: LABEL 186
LABEL 173
    temp233 <- read-word(3c53)
    if (int16(01) > int16(temp233)) is true then
        jump-to: LABEL 175
LABEL 174
    temp234 <- read-word(3c53)
    if (int16(temp234) > int16(0114)) is false then
        jump-to: LABEL 176
LABEL 175
    temp235 <- read-word(3c53)
    discard: call 169c8 (03, temp235)
    jump-to: LABEL 17c
LABEL 176
    temp236 <- read-word(3c53)
    temp237 <- (temp236 - 1)
    temp238 <- (temp237 * e)
    temp239 <- (temp238 + 188)
    temp23a <- read-byte(temp239)
    temp23b <- (temp23a & 0008)
    temp23c <- (temp23b <> 0)
    if (temp23c = 1) is false then
        jump-to: LABEL 17c
LABEL 177
    temp23d <- read-word(3c53)
    if (int16(01) > int16(temp23d)) is true then
        jump-to: LABEL 179
LABEL 178
    temp23e <- read-word(3c53)
    if (int16(temp23e) > int16(0114)) is false then
        jump-to: LABEL 17a
LABEL 179
    temp23f <- read-word(3c53)
    discard: call 169c8 (03, temp23f)
    jump-to: LABEL 17b
LABEL 17a
    temp240 <- read-word(3c53)
    temp241 <- (temp240 - 1)
    temp242 <- (temp241 * e)
    temp243 <- (temp242 + 188)
    temp244 <- (temp243 + 1)
    temp245 <- read-byte(temp244)
    temp246 <- (temp245 & 0002)
    temp247 <- (temp246 <> 0)
    if (temp247 = 1) is true then
        jump-to: LABEL 17c
LABEL 17b
    temp248 <- read-word(3c53)
    discard: call 13a88 (4e, 09, temp248)
    jump-to: LABEL 186
LABEL 17c
    temp249 <- read-word(3c53)
    if (int16(05) > int16(temp249)) is true then
        jump-to: LABEL 17f
LABEL 17d
    temp24a <- read-word(3c53)
    if (int16(temp24a) > int16(0114)) is true then
        jump-to: LABEL 17f
LABEL 17e
    temp24b <- read-word(3c53)
    temp24c <- (temp24b - 1)
    temp24d <- (temp24c * e)
    temp24e <- (temp24d + 188)
    temp24f <- (temp24e + 6)
    temp250 <- read-word(temp24f)
    if (temp250 = 01) is false then
        jump-to: LABEL 180
LABEL 17f
    temp251 <- read-word(3c53)
    discard: call 169c8 (09, temp251)
    write-word(3dc5) <- 02
    jump-to: LABEL 181
LABEL 180
    temp252 <- read-word(3c53)
    write-word(3dc5) <- temp252
LABEL 181
    write-word(3dc7) <- 00
    temp253 <- read-word(3dc5)
    temp254 <- (temp253 - 1)
    temp255 <- (temp254 * e)
    temp256 <- (temp255 + 188)
    temp257 <- (temp256 + a)
    temp258 <- read-word(temp257)
    push-SP: temp258
    if (temp258 <> 0) is false then
        jump-to: LABEL 183
LABEL 182
    temp259 <- read-word(3dc7)
    write-word(3dc7) <- (int16(temp259) + int16(1))
    temp25a <- pop-SP
    temp25b <- (temp25a - 1)
    temp25c <- (temp25b * e)
    temp25d <- (temp25c + 188)
    temp25e <- (temp25d + 8)
    temp25f <- read-word(temp25e)
    push-SP: temp25f
    if (temp25f <> 0) is true then
        jump-to: LABEL 182
LABEL 183
    temp260 <- pop-SP
    write-word(3dc5) <- temp260
    temp261 <- read-word(3dc7)
    if (temp261 = 0) is false then
        jump-to: LABEL 185
LABEL 184
    temp262 <- read-word(3c53)
    discard: call 13a88 (40, 06, temp262)
    jump-to: LABEL 186
LABEL 185
    temp263 <- L00
    discard: call 17340 (temp263, 00, 00)
LABEL 186
    temp264 <- L00
    temp265 <- call 172a4 (temp264, 00)
    temp266 <- (temp265 = 38)
    if (temp266) is true then
        jump-to: LABEL 18a
LABEL 187
    temp267 <- read-word(3cad)
    temp268 <- (temp267 = 64)
    if (temp268) is false then
        jump-to: LABEL 189
LABEL 188
    discard: call 13a88 (1007, 2b)
    jump-to: LABEL 18a
LABEL 189
    discard: call 13a88 (1007, 2c)
LABEL 18a
    temp269 <- read-word(3c81)
    temp26a <- (temp269 = 12)
    if (temp26a) is false then
        jump-to: LABEL 190
LABEL 18b
    write-word(3cd7) <- 03
    temp26b <- read-word(3cd5)
    if (temp26b = 0) is false then
        jump-to: LABEL 18d
LABEL 18c
    push-SP: 0
    jump-to: LABEL 18e
LABEL 18d
    push-SP: call (temp26b * 4) ()
LABEL 18e
    temp26c <- pop-SP
    temp26d <- (temp26c = ffff)
    if (temp26d) is false then
        jump-to: LABEL 190
LABEL 18f
    temp26e <- read-word(3c85)
    write-word(3c83) <- temp26e
    jump-to: LABEL 12c
LABEL 190
    jump-to: LABEL 0b
LABEL 191
    temp26f <- read-word(3ce9)
    temp270 <- read-word(3ceb)
    if (int16(temp26f) > int16(temp270)) is true then
        return: 1
LABEL 192
    L05 <- call ce34 ()
    temp271 <- L05
    temp272 <- (temp271 = 6664)
    temp273 <- (temp271 = 6664)
    temp274 <- (temp271 = 6664)
    if ((temp272 | temp273) | temp274) is true then
        jump-to: LABEL 194
LABEL 193
    temp275 <- L05
    temp276 <- (temp275 = 5332)
    if (temp276) is false then
        jump-to: LABEL 1a4
LABEL 194
    temp277 <- read-word(3ce9)
    temp278 <- read-word(3ceb)
    if (int16(temp277) > int16(temp278)) is false then
        jump-to: LABEL 196
LABEL 195
    write-word(3cf7) <- 00
    return: 1
LABEL 196
    temp279 <- read-word(3cef)
    L05 <- call cec8 (temp279)
    temp27a <- read-word(3ce9)
    L06 <- call cec8 (temp27a)
LABEL 197
    temp27b <- L05
    temp27c <- L06
    if (int16(temp27b) < int16(temp27c)) is false then
        jump-to: LABEL 199
LABEL 198
    temp27d <- L05
    discard: call 172c8 (temp27d, 00, 20)
    temp27e <- L05
    L05 <- (int16(temp27e) + int16(1))
    jump-to: LABEL 197
LABEL 199
    L05 <- call ce34 ()
    temp27f <- L05
    temp280 <- (temp27f = 4f03)
    temp281 <- (temp27f = 5830)
    temp282 <- (temp27f = 4f03)
    if ((temp280 | temp281) | temp282) is false then
        jump-to: LABEL 1a3
LABEL 19a
    temp283 <- read-word(3ce9)
    temp284 <- (int16(temp283) - int16(02))
    temp285 <- call cec8 (temp284)
    L05 <- (int16(temp285) - int16(41b7))
    temp286 <- read-word(3ce9)
    temp287 <- read-word(3ceb)
    if (int16(temp286) > int16(temp287)) is false then
        jump-to: LABEL 19c
LABEL 19b
    L06 <- 77
    jump-to: LABEL 19d
LABEL 19c
    temp288 <- read-word(3ce9)
    temp289 <- call cec8 (temp288)
    L06 <- (int16(temp289) - int16(41b7))
LABEL 19d
    temp28a <- L05
    temp28b <- L06
    if (int16(temp28a) < int16(temp28b)) is false then
        jump-to: LABEL 1a3
LABEL 19e
    temp28c <- L05
    if (int16(temp28c) < int16(00)) is true then
        jump-to: LABEL 1a0
LABEL 19f
    temp28d <- L05
    if (int16(temp28d) < int16(7b)) is true then
        jump-to: LABEL 1a1
LABEL 1a0
    temp28e <- L05
    discard: call 169c8 (1e, temp28e, 7a, 00, 11)
    jump-to: LABEL 1a2
LABEL 1a1
    temp28f <- L05
    write-byte(432f + temp28f) <- 20
LABEL 1a2
    temp290 <- L05
    L05 <- (int16(temp290) + int16(1))
    jump-to: LABEL 19d
LABEL 1a3
    discard: call 9248 (41b7, 4232)
    write-word(3cf7) <- 01
    return: 1
LABEL 1a4
    write-word(3c83) <- 02
    jump-to: LABEL 12c
]]>

        TestBinder(Advent, &H9524, expected)
    End Sub

#End Region

End Module
