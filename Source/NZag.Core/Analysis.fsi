namespace NZag.Core

module Graphs =

    val Entry : int
    val Exit : int

    type Block<'T> =
      { ID : int
        Data : 'T
        Predecessors : int list
        Successors : int list }

        member IsEntry : bool
        member IsExit : bool

    type Graph<'T> =
      { Tree : BoundTree
        Blocks : Block<'T> list }

    [<CompiledNameAttribute("BuildControlFlowGraph")>]
    val buildControlFlowGraph : tree:BoundTree -> Graph<Statement list>
