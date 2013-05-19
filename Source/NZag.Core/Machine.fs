namespace NZag.Core

open System
open NZag.Utilities

type Machine (memory: Memory, debugging: bool) as this =

    let mainRoutineAddress = memory |> Header.readMainRoutineAddress
    let zfuncMap = Dictionary.create()
    let localArrayPool = Stack.create()

    let getOrCreateLocalArray() =
        if localArrayPool |> Stack.isEmpty then Array.zeroCreate 15
        else localArrayPool |> Stack.pop

    let releaseLocalArray arr =
        arr |> Array.clear
        localArrayPool |> Stack.push arr

    let textReader = new ZTextReader(memory)

    let outputStreams = new OutputStreamCollection(memory)
    let mutable screen = NullInstances.Screen

    let mutable random = new Random()

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

            CodeGenerator.Compile(memory, routine, this, builder, callSites, debugging)

            let zfunc = dynamicMethod.CreateDelegate(typeof<ZFunc>, this) :?> ZFunc

            { Routine = routine
              ZFunc = zfunc
              CallSites = callSites.ToArray() }

        memoize compileAux

    let getRoutine =
        let reader = new RoutineReader(memory)

        memoize (fun (address: Address) -> reader.ReadRoutine(address))

    let getCallSite =
        memoize (fun address -> new ZFuncCallSite(this, RawAddress(address) |> getRoutine))

    member x.Memory = memory

    member x.RunAsync() =
        async {
            let reader = new RoutineReader(memory)
            let callSite = getCallSite mainRoutineAddress.IntValue
            let stack = Array.zeroCreate 1024
            callSite.Invoke0(memory, stack, 0) |> ignore
        }

    member x.Run() =
        Async.RunSynchronously(x.RunAsync())

    member x.Randomize seed =
        (x :> IMachine).Randomize(seed)

    member x.RegisterScreen newScreen =
        screen <- newScreen
        // TODO: Write screen header values
        outputStreams.RegisterScreenStream(newScreen)

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

        member y.ReadZText(address) =
            textReader.ReadString(RawAddress(address))

        member y.WriteOutputChar(ch) =
            let work = (outputStreams :> IOutputStream).WriteCharAsync(ch)
            Async.RunSynchronously(work |> awaitTask)
        member y.WriteOutputText(s) =
            let work = (outputStreams :> IOutputStream).WriteTextAsync(s)
            Async.RunSynchronously(work |> awaitTask)

        member y.Randomize(seed) =
            random <- if seed = 0s then new Random(int DateTime.Now.Ticks)
                      else new Random(int +seed)
        member y.NextRandomNumber(range) =
            let minValue = 1us
            let maxValue = max minValue (uint16 (range - 1s))
            uint16 (random.Next(int minValue, int maxValue))


