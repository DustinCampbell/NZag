namespace NZag.Core

open NZag.Utilities
open BoundNodeVisitors

type BasicBlock =
  { Label : int
    Statements : list<Statement>
    Successors : list<int>
    Predecessors : list<int> }

type BasicBlockBuilder (label : int, graph : ControlFlowGraph) =

    let statements = new ResizeArray<_>()
    let successors = new ResizeArray<_>()
    let predecessors = new ResizeArray<_>()

    member x.AddStatement(statement : Statement) =
        statements.Add(statement)

    member x.AddSuccessor(label : int) =
        successors.Add(label)

    member x.AddPredecessor(label : int) =
        predecessors.Add(label)

    member x.GetBasicBlock() =
      { Label = label;
        Statements = statements |> List.ofSeq;
        Successors = successors |> List.ofSeq;
        Predecessors = predecessors |> List.ofSeq }

and ControlFlowGraph (tree : BoundTree) as this =

    let builderMap =
        let map = Dictionary.create()
        let current = ref None

        let newBuilder l =
            let builder = new BasicBlockBuilder(l, this)
            map.Add(l, builder)
            current := Some(builder)
            builder

        let getCurrentBuilder() =
            (!current).Value

        tree |> walkTree
            (fun s ->
                match s with
                | LabelStmt(l) ->
                    let builder = newBuilder l
                    builder.AddStatement(s)
                | s ->
                    let builder = getCurrentBuilder()
                    builder.AddStatement(s))
            (fun e -> ())

        map
