namespace NZag.Core

open NZag.Utilities

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

        member GetBlockUsageCount : blockId:int -> int

        member ID : int
        member Temp : int
        member BlockID : int
        member StatementIndex : int
        member Value : Expression
        member UsageCount : int

    type DataFlowBlockInfo =

        new : int[] * Statement[] * IBitSet * IBitSet -> DataFlowBlockInfo

        member DefinitionIds : int[]
        member Statements : Statement[]
        member Ins : IBitSet
        member Outs : IBitSet

    type DataFlowAnalysis =
      { Definitions : Definition[]
        DefinitionsByTemp : int[][]
        Graph : Graph<DataFlowBlockInfo> }

    [<CompiledNameAttribute("ComputeIns")>]
    val computeIns : dataFlowAnalysis:DataFlowAnalysis * block:Block<DataFlowBlockInfo> * statementIndex:int -> IBitSet

    [<CompiledNameAttribute("ComputeOuts")>]
    val computeOuts : dataFlowAnalysis:DataFlowAnalysis * block:Block<DataFlowBlockInfo> * statementIndex:int -> IBitSet

    [<CompiledNameAttribute("AnalyzeDataFlow")>]
    val analyzeDataFlow : graph:Graph<ControlFlowData> -> DataFlowAnalysis
