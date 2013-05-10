namespace NZag.Core

open NZag.Utilities

type Machine (memory : Memory) as this =

    let zfuncMap = Dictionary.create()

    let localArrayPool = Stack.create()

    let getOrCreateLocalArray() =
        if localArrayPool |> Stack.isEmpty then Array.zeroCreate 15
        else localArrayPool |> Stack.pop

    let releaseLocalArray arr =
        arr |> Array.clear
        localArrayPool |> Stack.push arr

    let compile (routine: Routine) =
        let dynamicMethod =
            new System.Reflection.Emit.DynamicMethod(
                name = sprintf "%4x_%d_locals" routine.Address.IntValue routine.Locals.Length,
                returnType = typeof<uint16>,
                parameterTypes = [|typeof<Machine>; typeof<uint16>; typeof<int>|],
                owner = typeof<Machine>,
                skipVisibility = true)

        let generator = dynamicMethod.GetILGenerator()
        let builder = new ILBuilder(generator)

        CodeGenerator.Compile(memory, routine, builder)

        dynamicMethod.CreateDelegate(typeof<ZFunc>, this) :?> ZFunc

    member x.Memory = memory

    interface IMachine with

        member y.GetInitialLocalArray(routine) =
            let result = getOrCreateLocalArray()

            if memory.Version < 5 then
                routine.Locals
                |> List.iteri (fun i v -> if v > 0us then result.[i] <- v)

            result

        member y.ReleaseLocalArray(locals) =
            releaseLocalArray locals

        member y.GetOrCompileZFunc(routine) =
            match zfuncMap |> Dictionary.tryFind routine.Address with
            | Some(f) -> f
            | None    -> let f = compile routine
                         zfuncMap |> Dictionary.add routine.Address f
                         f
