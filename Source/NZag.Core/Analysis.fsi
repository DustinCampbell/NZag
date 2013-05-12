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
      { Statements : Statement list }

    [<CompiledNameAttribute("BuildControlFlowGraph")>]
    val buildControlFlowGraph : tree:BoundTree -> Graph<ControlFlowData>

    type Definition =
      { Temp : int
        BlockID : int
        StatementIndex : int
        Value : Expression }

    type StatementFlowInfo =
      { Statement : Statement
        InDefinitions : Set<Definition>
        OutDefinitions : Set<Definition> }

    type DefinitionData =
      { Statements : StatementFlowInfo list
        InDefinitions : Set<Definition>
        OutDefinitions : Set<Definition> }

    type ReachingDefinitions =
      { Definitions : Map<int, Set<Definition>>
        Usages : Map<Definition, int>
        Graph : Graph<DefinitionData> }

    [<CompiledNameAttribute("ComputeReachingDefinitions")>]
    val computeReachingDefinitions : graph:Graph<ControlFlowData> -> ReachingDefinitions
