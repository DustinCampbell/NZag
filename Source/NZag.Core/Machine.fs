namespace NZag.Core

open NZag.Utilities

type Machine (memory : Memory) as this =

    let mainRoutineAddress = memory |> Header.readMainRoutineAddress
    let zfuncMap = Dictionary.create()
    let localArrayPool = Stack.create()

    let getOrCreateLocalArray() =
        if localArrayPool |> Stack.isEmpty then Array.zeroCreate 15
        else localArrayPool |> Stack.pop

    let releaseLocalArray arr =
        arr |> Array.clear
        localArrayPool |> Stack.push arr

    let compile =
        let compileAux (routine: Routine) =
            let dynamicMethod =
                new System.Reflection.Emit.DynamicMethod(
                    name = sprintf "%4x_%d_locals" routine.Address.IntValue routine.Locals.Length,
                    returnType = typeof<uint16>,
                    parameterTypes = [|typeof<Machine>; typeof<Memory>; typeof<uint16[]>; typeof<uint16[]>; typeof<int>; typeof<ZFuncCallSite[]>; typeof<int>|],
                    owner = typeof<Machine>,
                    skipVisibility = true)

            let generator = dynamicMethod.GetILGenerator()
            let builder = new ILBuilder(generator)
            let callSites = ResizeArray.create()

            CodeGenerator.Compile(memory, routine, this, builder, callSites)

            let zfunc = dynamicMethod.CreateDelegate(typeof<ZFunc>, this) :?> ZFunc

            { Routine = routine
              ZFunc = zfunc
              CallSites = callSites.ToArray() }

        memoize compileAux

    let getRoutine =
        let reader = new RoutineReader(memory)

        memoize (fun (address: Address) -> reader.ReadRoutine(address))

    let getCallSite =
        memoize (fun address -> new ZFuncCallSite(this, getRoutine address))

    member x.Memory = memory

    member x.Run() =
        let reader = new RoutineReader(memory)
        let callSite = getCallSite mainRoutineAddress
        let stack = Array.zeroCreate 1024
        callSite.Invoke0(memory, stack, 0)

    interface IMachine with

        member y.GetInitialLocalArray(routine) =
            let result = getOrCreateLocalArray()

            if memory.Version < 5 then
                routine.Locals
                |> List.iteri (fun i v -> if v > 0us then result.[i] <- v)

            result

        member y.ReleaseLocalArray(locals) =
            releaseLocalArray locals

        member y.Compile(routine) =
            compile routine
        member y.GetCallSite(address) =
            getCallSite address

