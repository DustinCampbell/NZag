namespace NZag.Core

open NZag.Utilities
open BoundNodeVisitors

module Graphs =

    let Entry = -1
    let Exit = -2

    type Block<'T> =
      { ID : int
        Data : 'T
        Predecessors : int list
        Successors : int list }

        member x.IsEntry = x.ID = Entry
        member x.IsExit = x.ID = Exit

    type Graph<'T> =
      { Tree : BoundTree
        Blocks : Block<'T> list }

    type Builder<'T> =
      { AddNode : int -> unit;
        AddEdge : int -> int -> unit;
        AddEdgeToNext : int -> unit;
        GetData : int -> 'T option
        UpdateData : int -> 'T option -> unit }

    let computeBlocks (buildBlocks : Builder<'T> -> unit) (finalizeData : 'T option -> 'U) : Block<'U> list =

        let compareIds x y = 
            if x = y then 0
            elif x = Entry then System.Int32.MinValue
            elif x = Exit then System.Int32.MaxValue
            elif y = Entry then System.Int32.MaxValue
            elif y = Exit then System.Int32.MinValue
            else compare x y

        let map = SortedList.createWithCompare compareIds

        let get id =
            map |> Dictionary.find id

        let next id =
            let nextId = id + 1
            if map |> Dictionary.contains nextId then nextId
            else Exit

        let addNode id =
            let pred = SortedSet.create()
            let succ = SortedSet.create()

            map |> Dictionary.add id (pred, succ, None)

        let addEdge id1 id2 =
            let (_,succ,_) = get id1
            let (pred,_,_) = get id2

            succ |> SortedSet.add id2
            pred |> SortedSet.add id1

        let addEdgeToNext id =
            addEdge id (next id)

        let getData id  =
            let (_,_,data) = get id
            data

        let updateData id (data : 'T option) =
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

        buildBlocks builder

        addEdgeToNext Entry

        let createBlock (id, (pred,succ,data)) =
          { ID = id
            Data = data |> finalizeData
            Predecessors = pred |> SortedSet.toList
            Successors = succ |> SortedSet.toList }

        map
            |> Dictionary.toList
            |> List.map createBlock

    type ControlFlowData =
      { Statements : Statement list }

    [<CompiledNameAttribute("BuildControlFlowGraph")>]
    let buildControlFlowGraph (tree : BoundTree) =

        let blocks =
            computeBlocks
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
                    | Some(slist) -> { Statements = slist |> List.ofSeq }
                    | None -> { Statements = List.empty })

        { Tree = tree; Blocks = blocks }

    type Definition =
      { Temp : int
        Value : Expression }

        override x.ToString() =
            sprintf "Temp = %02x" x.Temp

    type DefinitionData =
      { Statements : Statement list
        Definitions : Definition list }

    [<CompiledNameAttribute("ComputeReachingDefinitions")>]
    let computeReachingDefinitions (graph : Graph<ControlFlowData>) =
        let entry,rest =
            match graph.Blocks with
            | [] | _::[] | _::_::[] -> invalidOperation "Expected at least three nodes in graph (entry, exit and some basic block)"
            | h::t -> h,t

        let outs =
            graph.Blocks
            |> List.fold (fun res b -> res |> Map.add b.ID Set.empty) Map.empty
            |> ref

        let computeIns b =
            b.Predecessors
            |> List.map (fun p -> !outs |> Map.find p)
            |> List.reduce (fun s1 s2 -> Set.union s1 s2)

        let computeOuts (b : Block<ControlFlowData>) =
            let ins = computeIns b
            let generated = ref Set.empty
            let killed = ref Set.empty

            b.Data.Statements
            |> List.iter (fun s ->
                match s with
                | WriteTempStmt(t,e) ->
                    let def = { Temp = t; Value = e }
                    generated := !generated |> Set.filter (fun d -> d.Temp <> t)
                    killed := !killed |> Set.filter (fun d -> d.Temp = t)
                    generated := !generated |> Set.add def
                | _ -> ())

            Set.union !generated (Set.difference ins !killed)

        let stop = ref false
        while not (!stop) do
            stop := true

            rest
            |> List.iter (fun b ->
                let currentOuts = !outs |> Map.find b.ID
                let newOuts = computeOuts b
                if currentOuts <> newOuts then
                    outs := !outs |> Map.add b.ID newOuts
                    stop := false)

        let blocks =
            graph.Blocks
            |> List.map (fun b -> { ID = b.ID
                                    Data = { Statements = b.Data.Statements; Definitions = !outs |> Map.find b.ID |> Set.toList }
                                    Predecessors = b.Predecessors
                                    Successors = b.Successors })

        { Tree = graph.Tree;
          Blocks = blocks }


