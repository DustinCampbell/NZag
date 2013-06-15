namespace NZag.Core

open NZag.Utilities
open BoundNodeConstruction
open BoundNodeVisitors

module Optimization =

    let private updateTree f tree =
        let updater = new BoundTreeUpdater(tree)
        updater.Update f
        updater.GetTree()


    let cleanupLabels tree =
        // There are two goals here:
        //
        // * First, we want to reorder the labels so that the label numbers
        //   are in ascending order.
        //
        // * Second, we want to remove any redundant labels where a label
        //   immediately follows another one. This ensures that a sensible
        //   control flow graph can be created.

        let nextLabelIndex = ref 0
        let labels = Dictionary.create()
        let lastLabel = ref None

        // First, collect the existing labels and create new ones
        tree |> walkTree
            (fun s ->
                match s with
                | LabelStmt(l) ->
                    match !lastLabel with
                    | Some(newLabel) -> 
                        labels |> Dictionary.add l newLabel
                    | None ->
                        let newLabel = !nextLabelIndex
                        labels |> Dictionary.add l newLabel
                        incr nextLabelIndex
                        lastLabel := Some(newLabel)
                | _ ->
                    lastLabel := None)
            (fun e -> ())

        // Next, rewrite the tree, replacing the old labels with new ones
        let createdLabels = ref Set.empty

        tree |> updateTree (fun s updater ->
            match s with
            | LabelStmt(l) ->
                // Be careful not to add duplicate labels
                let l' = labels.[l]
                if not (!createdLabels |> Set.contains l') then
                    LabelStmt(l') |> updater.AddStatement
                    createdLabels := !createdLabels |> Set.add l'
            | JumpStmt(l) ->
                JumpStmt(labels.[l]) |> updater.AddStatement
            | BranchStmt(c,e,JumpStmt(l)) ->
                BranchStmt(c,e,JumpStmt(labels.[l])) |> updater.AddStatement
            | s ->
                updater.AddStatement(s))

    let cleanupTemps tree =
        let nextTempIndex = ref 0
        let temps = Dictionary.create()

        let getOrAddTemp t =
            match temps |> Dictionary.tryFind t with
            | Some(t) -> t
            | None    -> let newTemp = !nextTempIndex
                         temps |> Dictionary.add t newTemp
                         incr nextTempIndex
                         newTemp

        // Rewrite the tree's statements, replacing old temps with new ones
        let rewriteTemps s =
            s |> rewriteStatement
                (fun s -> 
                    match s with
                    | WriteTempStmt(t,e) ->
                        let newTemp = getOrAddTemp t
                        WriteTempStmt(newTemp,e)
                    | s -> s)
                (fun e ->
                    match e with
                    | TempExpr(t) ->
                        let newTemp = getOrAddTemp t
                        TempExpr(newTemp)
                    | e -> e)

        let newStatements = tree.Statements |> List.map rewriteTemps

        { Statements = newStatements; TempCount = !nextTempIndex; LabelCount = tree.LabelCount; Routine = tree.Routine }

    let private optimize_PropagateConstants tree =
        let graph = Graphs.buildControlFlowGraph tree
        let dataFlowAnalysis = Graphs.analyzeDataFlow graph

        let block = ref None
        let index = ref 0

        let getBlock() =
            match !block with
            | Some(b) -> b
            | None -> failcompile "No block set"

        let setBlock id =
            block := Some(dataFlowAnalysis.Graph.Blocks |> Array.find (fun b -> b.ID = id))

        let getDefinitions t =
            let b = getBlock()
            let index = !index
            let ins = Graphs.computeIns(dataFlowAnalysis, b, index)

            let arr = new ResizeArray<_>(ins.Length)
            for d in ins.AllSet do
                let definition = dataFlowAnalysis.Definitions.[d]
                if definition.Temp = t then
                    arr.Add(definition)

            arr.ToArray()

        tree |> updateTree (fun s updater ->
            let s' =
                s |> rewriteStatement
                    (fun s ->
                        match s with
                        | LabelStmt(l) ->
                            index := 0
                            setBlock(l)
                            incr index
                            s
                        | s ->
                            incr index
                            s)
                    (fun e ->
                        match e with
                        | TempExpr(t) ->
                            let definitions = getDefinitions t

                            // If this temp only has a single definition of a constant expression
                            // at this point, use the constant.
                            if definitions.Length = 1 then
                                let definition = definitions.[0]

                                match definition.Value with
                                | ConstantExpr(_) as c ->
                                    // We can propagate any constant
                                    c
                                | TempExpr(_) as t ->
                                    // For temps, we are a bit more conservative. We only
                                    // propagate if the definition has a single usage within the current block.

                                    let usageCount = definition.GetBlockUsageCount(getBlock().ID)
                                    if usageCount = 1 then t
                                    else e
                                | _ -> e
                            else
                                e
                        | e -> e)

            updater.AddStatement(s'))

    let private optimize_FoldConstants tree =
        let int16_arithmetic l r = function
            | BinaryOperationKind.Add         -> Some(wordConst (uint16 ((int16 l) + (int16 r))))
            | BinaryOperationKind.Subtract    -> Some(wordConst (uint16 ((int16 l) - (int16 r))))
            | BinaryOperationKind.Multiply    -> Some(wordConst (uint16 ((int16 l) * (int16 r))))
            | BinaryOperationKind.Divide      -> Some(wordConst (uint16 ((int16 l) / (int16 r))))
            | BinaryOperationKind.Remainder   -> Some(wordConst (uint16 ((int16 l) % (int16 r))))
            | _ -> None

        let int16_logical l r = function
            | BinaryOperationKind.Equal       -> if l = r then Some(one) else Some(zero)
            | BinaryOperationKind.NotEqual    -> if l <> r then Some(one) else Some(zero)
            | BinaryOperationKind.LessThan    -> if (int16 l) < (int16 r) then Some(one) else Some(zero)
            | BinaryOperationKind.GreaterThan -> if (int16 l) > (int16 r) then Some(one) else Some(zero)
            | BinaryOperationKind.AtLeast     -> if (int16 l) >= (int16 r) then Some(one) else Some(zero)
            | BinaryOperationKind.AtMost      -> if (int16 l) <= (int16 r) then Some(one) else Some(zero)
            | _ -> None

        let int32_arithmetic l r = function
            | BinaryOperationKind.Add         -> Some(int32Const (l + r))
            | BinaryOperationKind.Subtract    -> Some(int32Const (l - r))
            | BinaryOperationKind.Multiply    -> Some(int32Const (l * r))
            | BinaryOperationKind.Divide      -> Some(int32Const (l / r))
            | BinaryOperationKind.Remainder   -> Some(int32Const (l % r))
            | _ -> None

        let int32_logical l r = function
            | BinaryOperationKind.Equal       -> if l = r then Some(one) else Some(zero)
            | BinaryOperationKind.NotEqual    -> if l <> r then Some(one) else Some(zero)
            | BinaryOperationKind.LessThan    -> if l < r then Some(one) else Some(zero)
            | BinaryOperationKind.GreaterThan -> if l > r then Some(one) else Some(zero)
            | BinaryOperationKind.AtLeast     -> if l >= r then Some(one) else Some(zero)
            | BinaryOperationKind.AtMost      -> if l <= r then Some(one) else Some(zero)
            | _ -> None

        let bitwise l r = function
            | BinaryOperationKind.Or          -> Some(wordConst (uint16 l ||| uint16 r))
            | BinaryOperationKind.And         -> Some(wordConst (uint16 l &&& uint16 r))
            | BinaryOperationKind.ShiftLeft   -> Some(wordConst (uint16 l <<< r))
            | BinaryOperationKind.ShiftRight  -> Some(wordConst (uint16 l >>> r))
            | _ -> None

        tree |> updateTree (fun s updater ->
            let s' =
                s |> rewriteStatement
                    (fun s -> s)
                    (fun e ->
                        match e with
                        | BinaryOperationExpr(k,l,r) ->
                            match l,r with
                            | ConstantExpr(Int32Value v1), ConstantExpr(Int32Value v2) ->
                                match k with
                                | IsArithmetic ->
                                    match int32_arithmetic v1 v2 k with
                                    | Some(res) -> res
                                    | None -> e
                                | IsLogical ->
                                    match int32_logical v1 v2 k with
                                    | Some(res) -> res
                                    | None -> e
                                | IsBitwise ->
                                    match bitwise v1 v2 k with
                                    | Some(res) -> res
                                    | None -> e
                                | _ -> e

                            | ToInt16(ConstantExpr(Int32Value v1)), ToInt16(ConstantExpr(Int32Value v2)) ->
                                match k with
                                | IsArithmetic ->
                                    match int16_arithmetic v1 v2 k with
                                    | Some(res) -> res
                                    | None -> e
                                | IsLogical ->
                                    match int16_logical v1 v2 k with
                                    | Some(res) -> res
                                    | None -> e
                                | IsBitwise ->
                                    match bitwise v1 v2 k with
                                    | Some(res) -> res
                                    | None -> e
                                | _ -> e

                            | l, ConstantExpr(Zero) ->
                                match k with
                                | BinaryOperationKind.Add -> l
                                | BinaryOperationKind.Multiply -> zero
                                | BinaryOperationKind.Subtract -> l
                                | _ -> e

                            | l, ConstantExpr(One) ->
                                match k with
                                | BinaryOperationKind.Multiply -> l
                                | _ -> e

                            | ConstantExpr(Zero), r ->
                                match k with
                                | BinaryOperationKind.Add -> r
                                | BinaryOperationKind.Divide -> zero
                                | BinaryOperationKind.Multiply -> zero
                                | _ -> e

                            | ConstantExpr(One), r ->
                                match k with
                                | BinaryOperationKind.Multiply -> r
                                | _ -> e

                            | _ -> e
                        | e -> e)

            updater.AddStatement(s'))

    let private optimize_RemoveUnusedTemps tree =
        let graph = Graphs.buildControlFlowGraph tree
        let dataFlowAnalysis = Graphs.analyzeDataFlow graph

        let block = ref None
        let index = ref 0

        let setBlock id =
            block := Some(dataFlowAnalysis.Graph.Blocks |> Array.find (fun b -> b.ID = id))

        let hasUsages t =
            match !block with
            | Some(b) ->
                let defs =
                    dataFlowAnalysis.DefinitionsByTemp.[t]
                    |> Array.filter (fun d ->
                        let def = dataFlowAnalysis.Definitions.[d]
                        def.BlockID = b.ID && def.StatementIndex = !index
                    )

                match defs with
                | [|d|] -> dataFlowAnalysis.Definitions.[d].UsageCount > 0
                | _ -> failcompile "Expected a single definition"

            | None ->
                failcompile "Couldn't find block"

        let hasSideEffects v =
            let result = ref false

            v |> walkExpression
                (fun e ->
                    match e with
                    | StackPopExpr
                    | CallExpr(_,_)
                    | ReadInputCharExpr
                    | ReadInputTextExpr(_,_)
                    | ReadTimedInputCharExpr(_,_)
                    | ReadTimedInputTextExpr(_,_,_,_) ->
                        result := true
                    | e -> ())

            !result

        tree |> updateTree (fun s updater ->
            let s' = match s with
                     | LabelStmt(l) ->
                         index := 0
                         setBlock(l)
                         Some(s)
                     | WriteTempStmt(t,v) ->
                         if hasSideEffects v || hasUsages t then Some(s)
                         else None
                     | s ->
                         Some(s)

            incr index

            match s' with
            | Some(s') -> updater.AddStatement(s')
            | None -> ())

    let private optimize_InlineStackPushAndPop tree =
        // We want to inline sequences of pushes and pops to reduce VM stack usage.
        // To do this, we watch for StackPushStmts and inline the expression in
        // specific cases.

        let currentStackPush = ref None

        tree |> updateTree (fun s updater ->
            match !currentStackPush with
            | Some(StackPushStmt(e) as lastStackPush) ->
                let updatedStmt =
                    match s with
                    | ReturnStmt(StackPopExpr) -> Some(ReturnStmt(e))
                    | BranchStmt(c,StackPopExpr,s) -> Some(BranchStmt(c,e,s))
                    | WriteTempStmt(i,StackPopExpr) -> Some(WriteTempStmt(i,e))
                    | WriteLocalStmt(StackPopExpr,v) -> Some(WriteLocalStmt(e,v))
                    | WriteLocalStmt(ConstantExpr(_) as i,StackPopExpr) -> Some(WriteLocalStmt(i,e))
                    | WriteLocalStmt(TempExpr(_) as i,StackPopExpr) -> Some(WriteLocalStmt(i,e))
                    | WriteGlobalStmt(StackPopExpr,v) -> Some(WriteGlobalStmt(e,v))
                    | WriteGlobalStmt(ConstantExpr(_) as i,StackPopExpr) -> Some(WriteGlobalStmt(i,e))
                    | WriteGlobalStmt(TempExpr(_) as i,StackPopExpr) -> Some(WriteGlobalStmt(i,e))
                    | StackPushStmt(StackPopExpr) -> Some(StackPushStmt(e))
                    | StackUpdateStmt(StackPopExpr) -> Some(StackUpdateStmt(e))
                    | WriteComputedVarStmt(StackPopExpr,v) -> Some(WriteComputedVarStmt(e,v))
                    | WriteComputedVarStmt(ConstantExpr(_) as i, StackPopExpr) -> Some(WriteComputedVarStmt(i,e))
                    | WriteComputedVarStmt(TempExpr(_) as i, StackPopExpr) -> Some(WriteComputedVarStmt(i,e))
                    | WriteMemoryByteStmt(StackPopExpr,v) -> Some(WriteMemoryByteStmt(e,v))
                    | WriteMemoryByteStmt(ConstantExpr(_) as a, StackPopExpr) -> Some(WriteMemoryByteStmt(a,e))
                    | WriteMemoryByteStmt(TempExpr(_) as a,StackPopExpr) -> Some(WriteMemoryByteStmt(a,e))
                    | WriteMemoryWordStmt(StackPopExpr,v) -> Some(WriteMemoryWordStmt(e,v))
                    | WriteMemoryWordStmt(ConstantExpr(_) as a,StackPopExpr) -> Some(WriteMemoryWordStmt(a,e))
                    | WriteMemoryWordStmt(TempExpr(_) as a,StackPopExpr) -> Some(WriteMemoryWordStmt(a,e))
                    | DiscardValueStmt(StackPopExpr) -> Some(DiscardValueStmt(e))
                    | PrintCharStmt(StackPopExpr) -> Some(PrintCharStmt(e))
                    | PrintTextStmt(StackPopExpr) -> Some(PrintTextStmt(e))
                    | SetRandomNumberSeedStmt(StackPopExpr) -> Some(SetRandomNumberSeedStmt(e))
                    | DebugOutputStmt(StackPopExpr, list) -> Some(DebugOutputStmt(e, list))
                    | _ -> None

                match updatedStmt with
                | Some(StackPushStmt(_)) ->
                    // Special case: If we're inlining into another StackPushStmt,
                    // we don't add it to the updated bound tree. Instead, we'll
                    // just track it and check the next statement.
                    currentStackPush := updatedStmt
                | Some(updatedStmt) ->
                    // Add our updated statement and stop tracking the last stack push.
                    currentStackPush := None
                    updater.AddStatement(updatedStmt)
                | None ->
                    // No inlining could occur. In this, case just add everything
                    // to the updated bound tree.
                    currentStackPush := None
                    updater.AddStatement(lastStackPush)
                    updater.AddStatement(s)

            | None ->
                match s with
                | StackPushStmt(_) ->
                    // We found a StackPushStmt, go ahead and track it but don't
                    // add it to the updated bound tree yet.
                    currentStackPush := Some(s)
                | _ ->
                    // Nothing to do.
                    updater.AddStatement(s)

            | Some(_) -> failcompile "Unexpected")

    let private optimize_EliminateDeadBranches tree =
        tree |> updateTree (fun s updater ->
            let s' =
                match s with
                | BranchStmt(b,e,j) ->
                    match e with
                    | ConstantExpr(One)  -> if b then Some(j) else None
                    | ConstantExpr(Zero) -> if b then None else Some(j)
                    | _ -> Some(s)
                | s ->
                    Some(s)

            match s' with
            | Some(s') -> updater.AddStatement(s')
            | None -> ())

    let private optimize_EliminateDeadBlocks tree =
        let graph = Graphs.buildControlFlowGraph tree

        let block = ref None

        let setBlock id =
            block := Some(graph.Blocks |> Array.find (fun b -> b.ID = id))

        let hasPredecessors() =
            match !block with
            | Some(b) -> b.Predecessors.Length > 0
            | None -> failcompile "Couldn't find block"

        tree |> updateTree (fun s updater ->
            match s with
            | LabelStmt(l) ->
                setBlock(l)
            | s -> ()

            if hasPredecessors() then
                updater.AddStatement(s))

    let private optimize_EliminateRedundantLabels tree =
        let graph = Graphs.buildControlFlowGraph tree

        let labelsToRemove = ref Set.empty

        for i = 0 to tree.Statements.Length - 2 do
            let s1 = tree.Statements.[i]
            let s2 = tree.Statements.[i+1]

            match s1, s2 with
            | BranchStmt(_,_,_), LabelStmt(_)
            | ReturnStmt(_), LabelStmt(_)
            | QuitStmt(_), LabelStmt(_) -> ()

            | JumpStmt(j), LabelStmt(l) ->
                if j = l then
                    let b = graph.Blocks |> Array.find (fun b -> b.ID = l)
                    if b.Predecessors.Length = 1 then
                        labelsToRemove := !labelsToRemove |> Set.add l
            | _, LabelStmt(l) ->
                let b = graph.Blocks |> Array.find (fun b -> b.ID = l)
                if b.Predecessors.Length = 1 then
                    labelsToRemove := !labelsToRemove |> Set.add l
            | _ -> ()

        tree |> updateTree (fun s updater ->
            match s with
            | LabelStmt(l)
            | JumpStmt(l) ->
                if not (!labelsToRemove |> Set.contains l) then
                    updater.AddStatement(s)
            | s -> updater.AddStatement(s))

    let optimize tree =
        let optimized = tree |> fixedpoint (fun tree ->
            tree
            |> optimize_PropagateConstants
            |> optimize_FoldConstants
            |> optimize_RemoveUnusedTemps
            |> optimize_InlineStackPushAndPop
            |> optimize_EliminateDeadBranches
            |> optimize_EliminateDeadBlocks
            |> optimize_EliminateRedundantLabels)

        optimized
        |> cleanupLabels
        |> cleanupTemps
