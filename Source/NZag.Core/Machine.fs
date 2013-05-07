namespace NZag.Core

open NZag.Utilities

type Machine (memory : Memory) =

    let functionMap = Dictionary.create()

    let localArrayPool = Stack.create()

    let getOrCreateLocalArray() =
        if localArrayPool |> Stack.isEmpty then Array.zeroCreate 15
        else localArrayPool |> Stack.pop

    let releaseLocalArray arr =
        arr |> Array.clear
        localArrayPool |> Stack.push arr

    member x.Memory = memory

    interface IMachine with

        member x.GetInitialLocalArray routine =
            let result = getOrCreateLocalArray()

            if memory.Version < 5 then
                routine.Locals
                |> List.iteri (fun i v -> if v > 0us then result.[i] <- v)

            result

        member x.ReleaseLocalArray locals =
            releaseLocalArray locals

        member x.GetOrCompileFunction address =
            ()
