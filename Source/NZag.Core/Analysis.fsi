namespace NZag.Core

module Graphs =

    val Entry : int
    val Exit : int

    type Block<'T> =
      { ID : int
        Data : 'T
        Predecessors : int[]
        Successors : int[] }

        member IsEntry : bool
        member IsExit : bool

    type Graph<'T> =
      { Tree : BoundTree
        Blocks : Block<'T>[] }

    type ControlFlowData =
      { Statements : Statement[] }

    [<CompiledNameAttribute("BuildControlFlowGraph")>]
    val buildControlFlowGraph : tree:BoundTree -> Graph<ControlFlowData>

    type Definition =

        new : int * int * int * int * Expression -> Definition

        member ID : int
        member Temp : int
        member BlockID : int
        member StatementIndex : int
        member Value : Expression
        member UsageCount : int

    type StatementFlowInfo =
      { Statement : Statement
        InDefinitions : int[] }

    type DataFlowBlockInfo =

        new : int[] * StatementFlowInfo[] * int[] * int[] -> DataFlowBlockInfo

        member DefinitionIds : int[]
        member Statements : StatementFlowInfo[]
        member InDefinitions : int[]
        member OutDefinitions : int[]

    type DataFlowAnalysis =
      { Definitions : Definition[]
        DefinitionsByTemp : int[][]
        Graph : Graph<DataFlowBlockInfo> }

    [<CompiledNameAttribute("AnalyzeDataFlow")>]
    val analyzeDataFlow : graph:Graph<ControlFlowData> -> DataFlowAnalysis
