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
      { Statements : Statement[] }

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
                    let previousId = ref None

                    do tree |> walkTree
                        (fun s ->
                            let mutable id = !currentId

                            match s with
                            | LabelStmt(label) ->
                                match !lastStatement with
                                | Some(JumpStmt(_))
                                | Some(ReturnStmt(_))
                                | Some(QuitStmt) -> ()

                                | _ -> builder.AddEdge id label

                                match !previousId with
                                | Some(id) ->
                                    builder.AddEdge id label
                                    previousId := None
                                | None -> ()

                                id <- label
                                currentId := label
                            | JumpStmt(label) ->
                                builder.AddEdge id label
                            | BranchStmt(_,_,_) ->
                                previousId := Some(id)
                            | ReturnStmt(_)
                            | QuitStmt ->
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
                    | Some(slist) -> { Statements = slist |> ResizeArray.toArray }
                    | None -> { Statements = [||] })

        { Tree = tree; Blocks = blocks }

    type NewDefinition =
      { ID : int
        Temp : int
        BlockID : int
        StatementIndex : int
        Value : Expression }

    type Definition =
      { Temp : int
        BlockID : int
        StatementIndex : int
        Value : Expression }

        override x.ToString() =
            sprintf "Definition: Temp = %d; BlockID = %d; StatementIndex = %d" x.Temp x.BlockID x.StatementIndex

    type StatementFlowInfo =
      { Statement : Statement
        InDefinitions : Definition[]
        OutDefinitions : Definition[] }

    type DefinitionData =
      { Statements : StatementFlowInfo list
        InDefinitions : Definition[]
        OutDefinitions : Definition[] }

    type ReachingDefinitions =
      { Definitions : Map<int, Definition[]>
        Usages : Map<Definition, int>
        Graph : Graph<DefinitionData> }

    [<CompiledNameAttribute("ComputeReachingDefinitions")>]
    let computeReachingDefinitions (graph : Graph<ControlFlowData>) =
        let entry,rest =
            match graph.Blocks with
            | [] | _::[] | _::_::[] -> failcompile "Expected at least three nodes in graph (entry, exit and some basic block)"
            | h::t -> h,t

        // First, find every defintion.
        let definitions =
            let arr = ResizeArray.create()
            for b in rest do
                let statements = b.Data.Statements
                let count = statements.Length
                for i = 0 to count - 1 do
                    match statements.[i] with
                    | WriteTempStmt(t,e) ->
                        let id = arr.Count
                        arr |> ResizeArray.add { ID = id; Temp = t; BlockID = b.ID; StatementIndex = i; Value = e }
                    | _ -> ()
            arr

        // Next, produce a map of each temp to its definitions.
        // TODO: Start here!

        let statementsMap = Dictionary.createFrom (graph.Blocks |> List.map (fun b -> b.ID, ResizeArray.create()))
        let insMap = Dictionary.createFrom (graph.Blocks |> List.map (fun b -> b.ID, HashSet.create()))
        let outsMap = Dictionary.createFrom (graph.Blocks |> List.map (fun b -> b.ID, HashSet.create()))
        let tempToDefinitionsMap = Dictionary.create()
        let definitionToUsagesMap = Dictionary.create()

        let computeIns b =
            let res = HashSet.create()

            b.Predecessors
            |> List.map (fun p -> outsMap.[p])
            |> List.iter (fun s -> res |> HashSet.unionWith s)

            res

        let computeOuts (b : Block<ControlFlowData>) =
            let oldIns = insMap.[b.ID]
            let ins = computeIns b
            if not (oldIns |> HashSet.equals ins) then
                insMap.[b.ID] <- ins

            let statements = statementsMap.[b.ID]
            statements.Clear()

            let currentOuts = HashSet.createFrom ins

            b.Data.Statements |> Array.iteri (fun i s ->
                let currentIns = currentOuts |> HashSet.toArray

                // First, get the flow info for this statement
                let info =
                    match s with
                    | WriteTempStmt(t,e) ->
                        let definitionSet =
                            tempToDefinitionsMap
                            |> Dictionary.getOrAdd t (fun () -> HashSet.create())

                        let definition = { Temp = t
                                           BlockID = b.ID
                                           StatementIndex = i
                                           Value = e }

                        definitionSet |> HashSet.add definition

                        currentOuts |> HashSet.removeWhere (fun d -> d.Temp = t)
                        currentOuts |> HashSet.add definition
                    | s -> ()

                    {Statement = s; InDefinitions = currentIns; OutDefinitions = currentOuts |> HashSet.toArray}

                // Next, find any definition usages for this statement
                s |> walkStatement
                    (fun s -> ())
                    (fun e ->
                        match e with
                        | TempExpr(t) ->
                            let defs = currentIns |> Array.filter (fun d -> d.Temp = t)
                            for def in defs do
                                let usages = match definitionToUsagesMap |> Dictionary.tryFind def with
                                             | Some(u) -> u + 1
                                             | None -> 1

                                definitionToUsagesMap.[def] <- usages

                        | e -> ())

                statements.Add(info))

            currentOuts

        let stop = ref false
        while not (!stop) do
            stop := true

            rest
            |> List.iter (fun b ->
                let currentOuts = outsMap |> Dictionary.find b.ID
                let newOuts = computeOuts b
                if not (currentOuts |> HashSet.equals newOuts) then
                    outsMap.[b.ID] <- newOuts
                    stop := false)

        let blocks =
            graph.Blocks
            |> List.map (fun b -> { ID = b.ID
                                    Data = { Statements = statementsMap |> Dictionary.find b.ID |> ResizeArray.toList
                                             InDefinitions = insMap |> Dictionary.find b.ID |> HashSet.toArray
                                             OutDefinitions = outsMap |> Dictionary.find b.ID |> HashSet.toArray }
                                    Predecessors = b.Predecessors
                                    Successors = b.Successors })

        { Definitions =
            tempToDefinitionsMap 
            |> Dictionary.toList
            |> List.map (fun (id,defs) -> id, defs |> HashSet.toArray)
            |> Map.ofList
          Usages =
            definitionToUsagesMap
            |> Dictionary.toMap
          Graph =
            { Tree = graph.Tree;
              Blocks = blocks } }


