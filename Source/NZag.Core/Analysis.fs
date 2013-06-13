namespace NZag.Core

open NZag.Utilities
open BoundNodeVisitors

module Graphs =

    let Entry = -1
    let Exit = -2

    type Block<'T> =
      { ID : int
        Data : 'T
        Predecessors : int[]
        Successors : int[] }

        member x.IsEntry = x.ID = Entry
        member x.IsExit = x.ID = Exit

    type Graph<'T> =
      { Tree : BoundTree
        Blocks : Block<'T>[] }

    type Builder<'T> =
      { AddNode : int -> unit;
        AddEdge : int -> int -> unit;
        GetData : int -> 'T option
        UpdateData : int -> 'T option -> unit }

    let computeBlocks (buildBlocks : Builder<'T> -> unit) (finalizeData : 'T option -> 'U) : Block<'U>[] =

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
            Predecessors = pred |> SortedSet.toArray
            Successors = succ |> SortedSet.toArray }

        map
            |> Dictionary.toArray
            |> Array.map createBlock

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

    type Definition(id: int, temp: int, blockId: int, statementIndex: int, value: Expression) =

        let mutable usageCount = 0

        member x.AddUsage() =
            usageCount <- usageCount + 1

        member x.ID = id
        member x.Temp = temp
        member x.BlockID = blockId
        member x.StatementIndex = statementIndex
        member x.Value = value
        member x.UsageCount = usageCount

        override x.ToString() =
            sprintf "Definition = %d; Temp = %d" x.ID x.Temp

    type DataFlowBlockInfo(definitionIds: int[], statements: Statement[], ins: IBitSet, outs: IBitSet) =

        member x.DefinitionIds = definitionIds
        member x.Statements = statements
        member x.Ins = ins
        member x.Outs = outs

    type DataFlowAnalysis =
      { Definitions : Definition[]
        DefinitionsByTemp : int[][]
        Graph : Graph<DataFlowBlockInfo> }

    [<CompiledNameAttribute("ComputeIns")>]
    let computeIns(dataFlowAnalysis: DataFlowAnalysis, block: Block<DataFlowBlockInfo>, statement: Statement) =
        let blockId = block.ID
        let blockDefinitionIds = block.Data.DefinitionIds
        let blockStatements = block.Data.Statements
        let ins = block.Data.Ins

        let definitions = dataFlowAnalysis.Definitions
        let outs = BitSet.create definitions.Length
        outs.UnionWith(ins)

        let mutable stop = false
        let mutable index = 0

        while not stop do
            if statement = blockStatements.[index] then
                stop <- true
            else
                let definitionId = blockDefinitionIds.[index]
                if definitionId >= 0 then
                    let definition = definitions.[definitionId]

                    outs.RemoveWhere(fun defId -> definitions.[defId].Temp = definition.Temp)
                    outs.Add(definitionId)

                index <- index + 1
                if index = blockDefinitionIds.Length then
                    stop <- true

        outs

    [<CompiledNameAttribute("ComputeOuts")>]
    let computeOuts(dataFlowAnalysis: DataFlowAnalysis, block: Block<DataFlowBlockInfo>, statement: Statement) =
        let blockId = block.ID
        let blockDefinitionIds = block.Data.DefinitionIds
        let blockStatements = block.Data.Statements
        let ins = block.Data.Ins

        let definitions = dataFlowAnalysis.Definitions
        let outs = BitSet.create definitions.Length
        outs.UnionWith(ins)

        let mutable stop = false
        let mutable index = 0

        while not stop do
            let definitionId = blockDefinitionIds.[index]
            if definitionId >= 0 then
                let definition = definitions.[definitionId]

                outs.RemoveWhere(fun defId -> definitions.[defId].Temp = definition.Temp)
                outs.Add(definitionId)

            index <- index + 1
            if index = blockDefinitionIds.Length || statement = blockStatements.[index] then
                stop <- true

        outs

    [<CompiledNameAttribute("AnalyzeDataFlow")>]
    let analyzeDataFlow (graph : Graph<ControlFlowData>) =

        let tempCount = graph.Tree.TempCount
        let blocks = graph.Blocks
        let blockCount = blocks.Length
        let definitions = new ResizeArray<_>(tempCount)
        let definitionsByTemp = ResizeArray.init tempCount (fun _ -> ResizeArray.create())

        let definitionsByBlock = new ResizeArray<_>(blockCount)
        let insByBlock = new ResizeArray<_>(blockCount)
        let outsByBlock = new ResizeArray<_>(blockCount)

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

        // First, collect all of the definitions
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

                lastBlockIdSeen <- blockId

                let statements = block.Data.Statements
                let count = statements.Length

                let blockDefinitions = new ResizeArray<_>(count)
                definitionsByBlock.Add(blockDefinitions)

                for i = 0 to count - 1 do
                    let statement = statements.[i]

                    match statement with
                    | WriteTempStmt(temp,value) ->
                        // We've found a defintion, go ahead and add it to
                        // the various collections
                        let definitionId = definitions.Count
                        let definition = new Definition(definitionId, temp, blockId, i, value)

                        definitions.Add(definition)
                        definitionsByTemp.[temp].Add(definitionId)
                        blockDefinitions.Add(definitionId)
                    | _ ->
                        // If there's no definition at this statement, add -1 to
                        // indicate "no definition"
                        blockDefinitions.Add(-1)

        let definitionCount = definitions.Count
        let createBitSet() = BitSet.create definitionCount

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
                        insByBlock.Add(createBitSet())
                        outsByBlock.Add(createBitSet())

                lastBlockIdSeen <- blockId

                let statements = block.Data.Statements
                let count = statements.Length

                insByBlock.Add(createBitSet())
                outsByBlock.Add(createBitSet())

        let computeBlockIns (block: Block<ControlFlowData>) =
            let ins = createBitSet()

            for predecessorId in block.Predecessors do
                let outs = getOutsByBlock predecessorId
                ins.UnionWith(outs)

            ins

        let computeBlockOuts (block : Block<ControlFlowData>) =
            let blockId = block.ID
            let blockDefinitions = getDefinitionsByBlock blockId

            let ins = computeBlockIns block
            setInsByBlock blockId ins

            let outs = createBitSet()
            outs.UnionWith(ins)

            for i = 0 to blockDefinitions.Count - 1 do
                let definitionId = blockDefinitions.[i]
                if definitionId >= 0 then
                    let definition = definitions.[definitionId]

                    outs.RemoveWhere(fun defId -> definitions.[defId].Temp = definition.Temp)
                    outs.Add(definitionId)

            outs

        let stop = ref false
        while not (!stop) do
            stop := true

            graph.Blocks
            |> Seq.skip 1
            |> Seq.iter (fun b ->
                let outs = getOutsByBlock b.ID
                let newOuts = computeBlockOuts b
                if not (outs.Equals(newOuts)) then
                    setOutsByBlock b.ID newOuts
                    stop := false)

        // Everything's computed. Now we can go ahead and locate usages for each definition
        for i = 0 to blockCount - 1 do
            let block = graph.Blocks.[i]
            let blockId = block.ID
            let blockDefinitionIds = getDefinitionsByBlock blockId
            let blockStatements = block.Data.Statements
            let statementCount = blockStatements.Length

            let ins = (getInsByBlock blockId).Clone()

            for j = 0 to statementCount - 1 do

                blockStatements.[j] |> walkStatement
                    (fun s -> ())
                    (fun e ->
                        match e with
                        | TempExpr(temp) ->
                            for i in ins.AllSet do
                                let def = definitions.[i]
                                if def.Temp = temp then
                                    def.AddUsage()

                        | e -> ())

                let definitionId = blockDefinitionIds.[j]
                if definitionId >= 0 then
                    let definition = definitions.[definitionId]

                    ins.RemoveWhere(fun defId -> definitions.[defId].Temp = definition.Temp)
                    ins.Add(definitionId)

        let blocks =
            graph.Blocks
            |> Array.map (fun b ->

                let definitionIds = getDefinitionsByBlock b.ID |> ResizeArray.toArray
                let statements = b.Data.Statements
                let ins = getInsByBlock b.ID
                let outs = getOutsByBlock b.ID

                {
                    ID = b.ID
                    Data = new DataFlowBlockInfo(definitionIds, statements, ins, outs)
                    Predecessors = b.Predecessors
                    Successors = b.Successors
                }
             )

        { Definitions = definitions |> ResizeArray.toArray
          DefinitionsByTemp =
            definitionsByTemp
            |> ResizeArray.toArray
            |> Array.map (ResizeArray.toArray)
          Graph =
            { Tree = graph.Tree;
              Blocks = blocks } }
