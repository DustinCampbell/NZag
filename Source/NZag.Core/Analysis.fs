namespace NZag.Core

open NZag.Utilities
open System.Collections.Generic
open BoundNodeVisitors

module Graphs =

    type Node = int
    type Edges = Node list
    type Context<'T> = (Node * 'T * Edges * Edges)
    type Graph<'T> = Context<'T> list

    [<Literal>]
    let Entry = -1
    [<Literal>]
    let Exit = -2

    type Builder<'T> =
      { AddNode : Node -> unit;
        AddEdge : Node -> Node -> unit;
        AddEdgeToNext : Node -> unit;
        GetData : Node -> 'T option
        UpdateData : Node -> 'T option -> unit }

    let buildGraph f (finalizeData : 'T option -> 'U) : Graph<'U> =

        let map = Dictionary.create()

        let get (id : Node) =
            map |> Dictionary.find id

        let next (id : Node) =
            let nextId = id + 1
            if map |> Dictionary.contains nextId then nextId
            else Exit

        let addNode (id : Node) =
            let pred = SortedSet.create()
            let succ = SortedSet.create()

            map |> Dictionary.add id (pred, succ, None)

        let addEdge (id1 : Node) (id2 : Node) =
            let (_,succ,_) = get id1
            let (pred,_,_) = get id2

            succ |> SortedSet.add id2
            pred |> SortedSet.add id1

        let addEdgeToNext (id : Node) =
            addEdge id (next id)

        let getData (id : Node) =
            let (_,_,data) = get id
            data

        let updateData (id : Node) (data : 'T option) =
            let (pred,succ,_) = get id
            map.[id] <- (pred,succ,data)

        addNode Entry
        addNode Exit

        let builder =
          { AddNode = addNode;
            AddEdge = addEdge;
            AddEdgeToNext = addEdgeToNext;
            GetData = getData;
            UpdateData = updateData }

        f builder

        addEdgeToNext Entry

        map
            |> Dictionary.toList
            |> List.map (fun (id,(pred,succ,data)) -> (id, data |> finalizeData, pred |> SortedSet.toList, succ |> SortedSet.toList))

    let buildControlFlowGraph (tree : BoundTree) =

        buildGraph
            (fun builder ->
                // First, add all nodes to the map.
                do tree |> walkTree
                    (fun s -> match s with | LabelStmt(label) -> builder.AddNode label | _ -> ())
                    (fun e -> ())

                // Next, add all data (i.e. statements) and edges.
                let currentId = ref Entry
                let lastStatement = ref None

                do tree |> walkTree
                    (fun s ->
                        let mutable id = !currentId

                        match s with
                        | LabelStmt(label) ->
                            match !lastStatement with
                            | Some(JumpStmt(_))
                            | Some(ReturnStmt(_)) -> ()
                            | _ -> builder.AddEdge id label

                            id <- label
                            currentId := label
                        | JumpStmt(label) ->
                            builder.AddEdge id label
                        | BranchStmt(_,_,_) ->
                            builder.AddEdgeToNext id
                        | ReturnStmt(_) ->
                            builder.AddEdge id Exit
                        | _ -> ()

                        let slist =
                            match builder.GetData id with
                            | Some(slist) -> slist
                            | None -> ResizeArray.create()

                        slist |> ResizeArray.add s
                        builder.UpdateData id (Some(slist))

                        lastStatement := Some(s))
                    (fun e -> ()))
            (fun data ->
                match data with
                | Some(slist) -> slist |> List.ofSeq
                | None -> List.empty)

