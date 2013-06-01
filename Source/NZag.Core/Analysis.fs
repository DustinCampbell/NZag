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

    type Definition =
      { Temp : int
        BlockID : int
        StatementIndex : int
        Value : Expression }

        override x.ToString() =
            sprintf "Definition: Temp = %d; BlockID = %d; StatementIndex = %d" x.Temp x.BlockID x.StatementIndex

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
    let computeReachingDefinitions (graph : Graph<ControlFlowData>) =
        let entry,rest =
            match graph.Blocks with
            | [] | _::[] | _::_::[] -> failcompile "Expected at least three nodes in graph (entry, exit and some basic block(s))"
            | h::t -> h,t

        let tempCount = graph.Tree.TempCount
        let blockCount = graph.Blocks.Length
        let definitions = new ResizeArray<_>(tempCount)
        let definitionsByTemp = ResizeArray.init tempCount (fun _ -> ResizeArray.create())
        let definitionsByBlock = new ResizeArray<_>(graph.Blocks.Length)
        let insByBlock = new ResizeArray<_>(blockCount)
        let outsByBlock = new ResizeArray<_>(blockCount)
        let statementInfosByBlock = new ResizeArray<_>(blockCount)

        // First, find all of the definitions
        do
            // Note: The use of -3 below may seem arbitrary but it's the first
            // negative number that isn't a valid block ID, since -1 and -2 are
            // reserved for the entry and exit blocks respectively.
            let mutable lastBlockIdSeen = -3

            for block in graph.Blocks do

                // The block IDs should be sequential, but there may be gaps.
                // To avoid the need for a map later, we'll just fill in any gaps
                // in the list of blocks by remembering the last block ID we
                // visited.
                let blockId = block.ID

                if lastBlockIdSeen <> -3 && blockId <> Entry then
                    for i = lastBlockIdSeen + 1 to blockId - 1 do
                        definitionsByBlock.Add(ResizeArray.create())
                        insByBlock.Add(HashSet.create())
                        outsByBlock.Add(HashSet.create())
                        statementInfosByBlock.Add(ResizeArray.create())

                lastBlockIdSeen <- blockId

                let statements = block.Data.Statements
                let count = statements.Length

                let blockDefinitions = new ResizeArray<_>(count)
                definitionsByBlock.Add(blockDefinitions)
                insByBlock.Add(HashSet.create())
                outsByBlock.Add(HashSet.create())
                statementInfosByBlock.Add(ResizeArray.create())

                for i = 0 to count - 1 do
                    match statements.[i] with
                    | WriteTempStmt(temp,value) ->
                        // We've found a defintion, go ahead and add it to
                        // the various collections
                        let definitionId = definitions.Count

                        let definition = {
                            Temp = temp
                            BlockID = blockId
                            StatementIndex = i
                            Value = value
                        }

                        definitions.Add(definition)
                        definitionsByTemp.[temp].Add(definitionId)
                        blockDefinitions.Add(definitionId)
                    | _ ->
                        // If there's no statement at this defintion, add -1 to
                        // indicate "no definition"
                        blockDefinitions.Add(-1)

        let definitionUsages = ResizeArray.init definitions.Count (fun _ -> 0)

        // Because our block arrays contains the entry and exit blocks,
        // we need to translate the block id to get the right index.
        let blockIdToIndex id (arr: ResizeArray<_>) =
            match id with
            | -1 -> 0
            | -2 -> arr.Count - 1
            | id -> id + 1

        let getDataByBlock id (arr: ResizeArray<_>) =
            let index = arr |> blockIdToIndex id
            arr.[index]

        let setDataByBlock id data (arr: ResizeArray<_>) =
            let index = arr |> blockIdToIndex id
            arr.[index] <- data

        let getDefinitionsByBlock id =
            definitionsByBlock |> getDataByBlock id

        let getInsByBlock id =
            insByBlock |> getDataByBlock id

        let setInsByBlock id ins =
            insByBlock |> setDataByBlock id ins

        let getOutsByBlock id =
            outsByBlock |> getDataByBlock id

        let setOutsByBlock id outs =
            outsByBlock |> setDataByBlock id outs

        let getStatementInfosByBlock id =
            statementInfosByBlock |> getDataByBlock id

        let computeIns (block: Block<ControlFlowData>) =
            let res = HashSet.create()

            block.Predecessors
            |> List.map (fun p -> getOutsByBlock p)
            |> List.iter (fun s -> res |> HashSet.unionWith s)

            res

        let computeOuts (block : Block<ControlFlowData>) =
            let blockId = block.ID
            let blockDefinitions = getDefinitionsByBlock blockId

            let ins = computeIns block
            setInsByBlock blockId ins

            let statementInfos = getStatementInfosByBlock blockId
            statementInfos.Clear()

            let currentOuts = HashSet.createFrom ins

            block.Data.Statements |> Array.iteri (fun i s ->
                let currentIns = currentOuts |> HashSet.toArray

                let definitionId = blockDefinitions.[i]
                if definitionId >= 0 then
                    let definition = definitions.[definitionId]

                    currentOuts |> HashSet.removeWhere (fun d -> definitions.[d].Temp = definition.Temp)
                    currentOuts |> HashSet.add definitionId

                let info = {
                    Statement = s
                    InDefinitions = currentIns
                    OutDefinitions = currentOuts |> HashSet.toArray
                }

                // Find any definition usages for this statement
                s |> walkStatement
                    (fun s -> ())
                    (fun e ->
                        match e with
                        | TempExpr(t) ->
                            let defs = currentIns |> Array.filter (fun d -> definitions.[d].Temp = t)
                            for def in defs do
                                definitionUsages.[def] <- definitionUsages.[def] + 1

                        | e -> ())

                statementInfos.Add(info))

            currentOuts

        let stop = ref false
        while not (!stop) do
            stop := true

            rest
            |> List.iter (fun b ->
                let currentOuts = getOutsByBlock b.ID
                let newOuts = computeOuts b
                if not (currentOuts |> HashSet.equals newOuts) then
                    setOutsByBlock b.ID newOuts
                    stop := false)

        let blocks =
            graph.Blocks
            |> List.map (fun b -> { ID = b.ID
                                    Data = { Statements = getStatementInfosByBlock b.ID |> ResizeArray.toArray
                                             InDefinitions = getInsByBlock b.ID |> HashSet.toArray
                                             OutDefinitions = getOutsByBlock b.ID |> HashSet.toArray }
                                    Predecessors = b.Predecessors
                                    Successors = b.Successors })

        { Definitions = definitions |> ResizeArray.toArray
          DefinitionsByTemp =
            definitionsByTemp
            |> ResizeArray.toArray
            |> Array.map (ResizeArray.toArray)
          Usages =
            definitionUsages
            |> ResizeArray.toArray
          Graph =
            { Tree = graph.Tree;
              Blocks = blocks } }


