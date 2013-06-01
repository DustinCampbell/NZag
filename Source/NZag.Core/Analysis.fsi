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

    type ControlFlowData =
      { Statements : Statement[] }

    [<CompiledNameAttribute("BuildControlFlowGraph")>]
    val buildControlFlowGraph : tree:BoundTree -> Graph<ControlFlowData>

    type Definition =
      { Temp : int
        BlockID : int
        StatementIndex : int
        Value : Expression }

    type StatementFlowInfo =
      { Statement : Statement
        InDefinitions : int[]
        OutDefinitions : int[] }

    type DefinitionData =
      { Statements : StatementFlowInfo[]
        InDefinitions : int[]
        OutDefinitions : int[] }

    type ReachingDefinitions =
      { Definitions : Definition[]
        DefinitionsByTemp : int[][]
        Usages : int[]
        Graph : Graph<DefinitionData> }

    [<CompiledNameAttribute("ComputeReachingDefinitions")>]
    val computeReachingDefinitions : graph:Graph<ControlFlowData> -> ReachingDefinitions
