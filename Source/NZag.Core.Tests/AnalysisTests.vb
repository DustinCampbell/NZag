Imports Node = System.Int32
Imports Edges = Microsoft.FSharp.Collections.FSharpList(Of Integer)
Imports Context = System.Tuple(Of Integer, Microsoft.FSharp.Collections.FSharpList(Of NZag.Core.Statement), Microsoft.FSharp.Collections.FSharpList(Of Integer), Microsoft.FSharp.Collections.FSharpList(Of Integer))
Imports Graph = Microsoft.FSharp.Collections.FSharpList(Of System.Tuple(Of Integer, Microsoft.FSharp.Collections.FSharpList(Of NZag.Core.Statement), Microsoft.FSharp.Collections.FSharpList(Of Integer), Microsoft.FSharp.Collections.FSharpList(Of Integer)))

Public Module AnalysisTests

    <Fact>
    Sub Zork1_4E3B()
        ' 4e3b:  b2 ...                  PRINT           "a "
        ' 4e3e:  aa 01                   PRINT_OBJ       L00
        ' 4e40:  b0                      RTRUE

        Dim expected =
            Graph(
                Context(Graphs.Entry, NoEdges, Edges(0)),
                Context(Graphs.Exit, Edges(0), NoEdges),
                Context(0, Edges(Graphs.Entry), Edges(Graphs.Exit)))

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
            Graph(
                Context(Graphs.Entry, NoEdges, Edges(0)),
                Context(Graphs.Exit, Edges(4, 5, 9, 10), NoEdges),
                Context(0, Edges(Graphs.Entry), Edges(1, 6)),
                Context(1, Edges(0), Edges(2, 3)),
                Context(2, Edges(1), Edges(4)),
                Context(3, Edges(1), Edges(4)),
                Context(4, Edges(2, 3), Edges(Graphs.Exit, 5)),
                Context(5, Edges(4), Edges(Graphs.Exit)),
                Context(6, Edges(0), Edges(7, 8)),
                Context(7, Edges(6), Edges(9)),
                Context(8, Edges(6), Edges(9)),
                Context(9, Edges(7, 8), Edges(Graphs.Exit, 10)),
                Context(10, Edges(9), Edges(Graphs.Exit)))

        Test(Zork1, &H4E42, expected)
    End Sub

    Private Function Graph(ParamArray actions() As Action(Of Context)) As Action(Of Graph)
        Return Sub(g)
                   Assert.Equal(actions.Length, g.Length)

                   For i = 0 To actions.Length - 1
                       actions(i)(g(i))
                   Next
               End Sub
    End Function

    Private Function Context(id As Node, predecessors As Action(Of Edges), successors As Action(Of Edges)) As Action(Of Context)
        Return Sub(c)
                   Assert.Equal(id, c.Item1)
                   predecessors(c.Item3)
                   successors(c.Item4)
               End Sub
    End Function

    Private Function Edges(ParamArray nodes() As Node) As Action(Of Edges)
        Return Sub(e)
                   Assert.Equal(nodes.Length, e.Length)

                   For i = 0 To nodes.Length - 1
                       Assert.Equal(nodes(i), e(i))
                   Next
               End Sub
    End Function

    Private ReadOnly NoEdges As Action(Of Edges) =
        Sub(e)
            Assert.Equal(0, e.Length)
        End Sub

    Private Sub Test(gameName As String, address As Integer, expected As Action(Of Graph))
        Dim memory = GameMemory(gameName)
        Dim reader = New RoutineReader(memory)

        Dim a = RawAddress(address)
        Dim r = reader.ReadRoutine(a)

        Assert.Equal(a, r.Address)

        Dim binder = New RoutineBinder(memory)
        Dim tree = binder.BindRoutine(r)
        Dim graph = Graphs.buildControlFlowGraph(tree)

        expected(graph)
    End Sub

End Module
