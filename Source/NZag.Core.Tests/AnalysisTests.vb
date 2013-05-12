Imports ControlFlowGraph = NZag.Core.Graphs.Graph(Of NZag.Core.Graphs.ControlFlowData)
Imports ControlFlowBlock = NZag.Core.Graphs.Block(Of NZag.Core.Graphs.ControlFlowData)
Imports DefinitionsGraph = NZag.Core.Graphs.Graph(Of NZag.Core.Graphs.DefinitionData)
Imports DefinitionsBlock = NZag.Core.Graphs.Block(Of NZag.Core.Graphs.DefinitionData)

Public Module AnalysisTests

    <Fact>
    Sub Zork1_4E3B_ControlFlow()
        ' 4e3b:  b2 ...                  PRINT           "a "
        ' 4e3e:  aa 01                   PRINT_OBJ       L00
        ' 4e40:  b0                      RTRUE

        Dim expected =
            ControlFlowGraph(
                ControlFlowBlock(Graphs.Entry, NoPred, Succ(0)),
                ControlFlowBlock(0, Pred(Graphs.Entry), Succ(Graphs.Exit)),
                ControlFlowBlock(Graphs.Exit, Pred(0), NoSucc))

        Test(Zork1, &H4E38, expected)
    End Sub

    <Fact>
    Sub Zork1_4E42_ControlFlow()
        ' 4e45:  a0 4c cb                JZ              G3c [TRUE] 4e51
        ' 4e48:  e7 7f 64 00             RANDOM          #64 -> -(SP)
        ' 4e4c:  63 01 00 c1             JG              L00,(SP)+ [TRUE] RTRUE
        ' 4e50:  b1                      RFALSE
        ' 4e51:  e7 3f 01 2c 00          RANDOM          #012c -> -(SP)
        ' 4e56:  63 01 00 c1             JG              L00,(SP)+ [TRUE] RTRUE
        ' 4e5a:  b1                      RFALSE

        Dim expected =
            ControlFlowGraph(
                ControlFlowBlock(Graphs.Entry, NoPred, Succ(0)),
                ControlFlowBlock(0, Pred(Graphs.Entry), Succ(1, 6)),
                ControlFlowBlock(1, Pred(0), Succ(2, 3)),
                ControlFlowBlock(2, Pred(1), Succ(4)),
                ControlFlowBlock(3, Pred(1), Succ(4)),
                ControlFlowBlock(4, Pred(2, 3), Succ(Graphs.Exit, 5)),
                ControlFlowBlock(5, Pred(4), Succ(Graphs.Exit)),
                ControlFlowBlock(6, Pred(0), Succ(7, 8)),
                ControlFlowBlock(7, Pred(6), Succ(9)),
                ControlFlowBlock(8, Pred(6), Succ(9)),
                ControlFlowBlock(9, Pred(7, 8), Succ(Graphs.Exit, 10)),
                ControlFlowBlock(10, Pred(9), Succ(Graphs.Exit)),
                ControlFlowBlock(Graphs.Exit, Pred(4, 5, 9, 10), NoSucc))

        Test(Zork1, &H4E42, expected)
    End Sub

    <Fact>
    Sub Zork1_4E42_ReachingDefinitions()
        ' 4e45:  a0 4c cb                JZ              G3c [TRUE] 4e51
        ' 4e48:  e7 7f 64 00             RANDOM          #64 -> -(SP)
        ' 4e4c:  63 01 00 c1             JG              L00,(SP)+ [TRUE] RTRUE
        ' 4e50:  b1                      RFALSE
        ' 4e51:  e7 3f 01 2c 00          RANDOM          #012c -> -(SP)
        ' 4e56:  63 01 00 c1             JG              L00,(SP)+ [TRUE] RTRUE
        ' 4e5a:  b1                      RFALSE

        Dim expected =
            DefinitionsGraph(
                DefinitionsBlock(Graphs.Entry,
                                 NoInDefs,
                                 NoOutDefs),
                DefinitionsBlock(0,
                                 NoInDefs,
                                 Outs(0)),
                DefinitionsBlock(1,
                                 Ins(0),
                                 Outs(0, 1)),
                DefinitionsBlock(2,
                                 Ins(0, 1),
                                 Outs(0, 1)),
                DefinitionsBlock(3,
                                 Ins(0, 1),
                                 Outs(0, 1)),
                DefinitionsBlock(4,
                                 Ins(0, 1),
                                 Outs(0, 1, 2, 3, 4, 5)),
                DefinitionsBlock(5,
                                 Ins(0, 1, 2, 3, 4, 5),
                                 Outs(0, 1, 2, 3, 4, 5)),
                DefinitionsBlock(6,
                                 Ins(0),
                                 Outs(0, 6)),
                DefinitionsBlock(7,
                                 Ins(0, 6),
                                 Outs(0, 6)),
                DefinitionsBlock(8,
                                 Ins(0, 6),
                                 Outs(0, 6)),
                DefinitionsBlock(9,
                                 Ins(0, 6),
                                 Outs(0, 6, 7, 8, 9, 10)),
                DefinitionsBlock(10,
                                 Ins(0, 6, 7, 8, 9, 10),
                                 Outs(0, 6, 7, 8, 9, 10)),
                DefinitionsBlock(Graphs.Exit,
                                 Ins(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10),
                                 Outs(0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10)))

        Test(Zork1, &H4E42, expected)
    End Sub

    Private Function ControlFlowGraph(ParamArray actions() As Action(Of ControlFlowBlock)) As Action(Of ControlFlowGraph)
        Return Sub(g)
                   Assert.Equal(actions.Length, g.Blocks.Length)

                   For i = 0 To actions.Length - 1
                       actions(i)(g.Blocks(i))
                   Next
               End Sub
    End Function

    Private Function ControlFlowBlock(id As Integer, ParamArray actions() As Action(Of ControlFlowBlock)) As Action(Of ControlFlowBlock)
        Return Sub(b)
                   Assert.Equal(id, b.ID)

                   For Each action In actions
                       action(b)
                   Next
               End Sub
    End Function

    Private Function Pred(ParamArray ids() As Integer) As Action(Of ControlFlowBlock)
        Return Sub(b)
                   Assert.Equal(ids.Length, b.Predecessors.Length)

                   For i = 0 To ids.Length - 1
                       Assert.Equal(ids(i), b.Predecessors(i))
                   Next
               End Sub
    End Function

    Private Function Succ(ParamArray ids() As Integer) As Action(Of ControlFlowBlock)
        Return Sub(b)
                   Assert.Equal(ids.Length, b.Successors.Length)

                   For i = 0 To ids.Length - 1
                       Assert.Equal(ids(i), b.Successors(i))
                   Next
               End Sub
    End Function

    Private ReadOnly NoPred As Action(Of ControlFlowBlock) =
        Sub(b)
            Assert.Equal(0, b.Predecessors.Length)
        End Sub

    Private ReadOnly NoSucc As Action(Of ControlFlowBlock) =
        Sub(b)
            Assert.Equal(0, b.Successors.Length)
        End Sub

    Private Function DefinitionsGraph(ParamArray actions() As Action(Of DefinitionsBlock)) As Action(Of DefinitionsGraph)
        Return Sub(g)
                   Assert.Equal(actions.Length, g.Blocks.Length)

                   For i = 0 To actions.Length - 1
                       actions(i)(g.Blocks(i))
                   Next
               End Sub
    End Function

    Private Function DefinitionsBlock(id As Integer, ParamArray actions() As Action(Of DefinitionsBlock)) As Action(Of DefinitionsBlock)
        Return Sub(b)
                   Assert.Equal(id, b.ID)

                   For Each action In actions
                       action(b)
                   Next
               End Sub
    End Function

    Private Function Ins(ParamArray ids() As Integer) As Action(Of DefinitionsBlock)
        Return Sub(b)
                   Dim orderedIns = b.Data.InDefinitions.OrderBy(Function(d) d.Temp).ToArray()

                   Assert.Equal(ids.Length, orderedIns.Length)

                   For i = 0 To ids.Length - 1
                       Assert.Equal(ids(i), orderedIns(i).Temp)
                   Next
               End Sub
    End Function

    Private ReadOnly NoInDefs As Action(Of DefinitionsBlock) =
        Sub(b)
            Assert.Equal(0, b.Data.InDefinitions.Count)
        End Sub

    Private Function Outs(ParamArray ids() As Integer) As Action(Of DefinitionsBlock)
        Return Sub(b)
                   Dim orderedOuts = b.Data.OutDefinitions.OrderBy(Function(d) d.Temp).ToArray()

                   Assert.Equal(ids.Length, orderedOuts.Length)

                   For i = 0 To ids.Length - 1
                       Assert.Equal(ids(i), orderedOuts(i).Temp)
                   Next
               End Sub
    End Function

    Private ReadOnly NoOutDefs As Action(Of DefinitionsBlock) =
        Sub(b)
            Assert.Equal(0, b.Data.OutDefinitions.Count)
        End Sub

    Private Sub Test(gameName As String, address As Integer, expected As Action(Of ControlFlowGraph))
        Dim memory = GameMemory(gameName)
        Dim reader = New RoutineReader(memory)

        Dim a = RawAddress(address)
        Dim r = reader.ReadRoutine(a)

        Assert.Equal(a, r.Address)

        Dim binder = New RoutineBinder(memory)
        Dim tree = binder.BindRoutine(r)
        Dim graph = Graphs.BuildControlFlowGraph(tree)

        expected(graph)
    End Sub

    Private Sub Test(gameName As String, address As Integer, expected As Action(Of DefinitionsGraph))
        Dim memory = GameMemory(gameName)
        Dim reader = New RoutineReader(memory)

        Dim a = RawAddress(address)
        Dim r = reader.ReadRoutine(a)

        Assert.Equal(a, r.Address)

        Dim binder = New RoutineBinder(memory)
        Dim tree = binder.BindRoutine(r)
        Dim cfg = Graphs.BuildControlFlowGraph(tree)
        Dim rd = Graphs.ComputeReachingDefinitions(cfg)

        expected(rd.Graph)
    End Sub

End Module
