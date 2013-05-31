﻿namespace NZag.Core

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

        member x.IsNone =
            x.ID = -1

        static member None =
          { ID = -1
            Temp = 0
            BlockID = 0
            StatementIndex = 0
            Value = ConstantExpr(Int32(0)) }

    type NewStatementFlowInfo =
      { Statement : Statement
        InDefinitions : NewDefinition[]
        OutDefinitions : NewDefinition[] }

        static member None =
          { Statement = LabelStmt(-1)
            InDefinitions = Array.empty
            OutDefinitions = Array.empty }

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
            | [] | _::[] | _::_::[] -> failcompile "Expected at least three nodes in graph (entry, exit and some basic block(s))"
            | h::t -> h,t

        // First, find every defintion.
        let definitions, definitionsByBlock, statementInfosByBlock =
            let allDefs = new ResizeArray<_>(graph.Tree.TempCount)
            let defsByBlock = new ResizeArray<_>(graph.Blocks.Length)
            let stmtInfosByBlock = new ResizeArray<_>(graph.Blocks.Length)

            for b in graph.Blocks do
                let stmts = b.Data.Statements
                let count = stmts.Length

                let defs = new ResizeArray<_>(count)
                defsByBlock.Add(defs)

                let stmtInfos = new ResizeArray<_>(count)
                stmtInfosByBlock.Add(stmtInfos)

                for i = 0 to count - 1 do
                    stmtInfos.Add(NewStatementFlowInfo.None)

                    match stmts.[i] with
                    | WriteTempStmt(t,e) ->
                        let def = { ID = allDefs.Count; Temp = t; BlockID = b.ID; StatementIndex = i; Value = e }
                        allDefs.Add(def)
                        defs.Add(def)
                    | _ ->
                        defs.Add(NewDefinition.None)

            allDefs, defsByBlock, stmtInfosByBlock

        let definitionCount = definitions.Count
        let bitVectorLength = definitionCount / 32

        let emptyBitVector() : uint32[] =
            Array.zeroCreate bitVectorLength

        let add (def: int) (vector: uint32[]) =
            let index = def / bitVectorLength
            let bit = def % bitVectorLength
            vector.[index] <- vector.[index] ||| (1u <<< bit)

        let remove (def: int) (vector: uint32[]) =
            let index = def / bitVectorLength
            let bit = def % bitVectorLength
            vector.[index] <- vector.[index] &&& ~~~(1u <<< bit)

        let hasDefinition (def: int) (vector: uint32[]) =
            let index = def / bitVectorLength
            let bit = def % bitVectorLength
            (vector.[index] ||| (1u <<< bit)) <> 0u

        let unionWith (source: uint32[]) (target: uint32[]) =
            for i = 0 to bitVectorLength - 1 do
                target.[i] <- target.[i] ||| source.[i]

        let differenceOf (source: uint32[]) (target: uint32[]) =
            for i = 0 to bitVectorLength - 1 do
                target.[i] <- target.[i] &&& ~~~source.[i]

        let equals (vector1: uint32[]) (vector2: uint32[]) =
            Array.forall2 (=) vector1 vector2

        let clone (vector: uint32[]) =
            Array.copy vector

        let definitionsFromVector (vector: uint32[]) =
            let len = vector.Length
            let arr = Array.zeroCreate len
            for i = 0 to len - 1 do
                arr.[i] <- if vector |> hasDefinition i then definitions.[i] else NewDefinition.None
            arr

        let getIndexForBlockID id (arr: ResizeArray<_>) =
            match id with
            | -2 -> 0
            | -1 -> arr.Count - 1
            | id -> id + 1

        let insByBlock =
            let arr = new ResizeArray<_>()
            for b in graph.Blocks do
                arr.Add(emptyBitVector())
            arr

        let outsByBlock =
            let arr = new ResizeArray<_>()
            for b in graph.Blocks do
                arr.Add(emptyBitVector())
            arr

        let getBlockData id (arr: ResizeArray<_>) =
            let index = arr |> getIndexForBlockID id
            arr.[index]

        let setBlockData id value (arr: ResizeArray<_>) =
            let index = arr |> getIndexForBlockID id
            arr.[index] <- value

        let getDefinitions id =
            definitionsByBlock |> getBlockData id

        let getStatementInfos id =
            statementInfosByBlock |> getBlockData id

        let getIns id =
            insByBlock |> getBlockData id

        let setIns id ins =
            insByBlock |> setBlockData id ins

        let getOuts id =
            outsByBlock |> getBlockData id

        let setOuts id outs =
            outsByBlock |> setBlockData id outs

        let computeIns b =
            let vector = emptyBitVector()
            for p in b.Predecessors do
                vector |> unionWith (getOuts p)
            vector

        let computeOuts (b: Block<ControlFlowData>) =
            let mutable outs = computeIns b
            setIns b.ID outs

            // TODO: Start here! To improve performance, we'll use bit vectors to represent the
            // ins and outs for each block.

            let defs = getDefinitions b.ID
            let stmtInfos = getStatementInfos b.ID

            let stmts = b.Data.Statements
            let count = stmts.Length
            
            for i = 0 to count - 1 do
                let s = stmts.[i]
                let ins = outs
                
                let def = defs.[i]
                if not def.IsNone then
                    // TODO: Remove any reaching definitions for the same temp as this one.
                    outs |> add def.ID

                // Create statement info
                let info : NewStatementFlowInfo =
                  { Statement = s
                    InDefinitions = definitionsFromVector ins
                    OutDefinitions = definitionsFromVector outs }

                stmtInfos.[i] <- info

                // TODO: Compute usages

            outs

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
            let ins = computeIns b
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


