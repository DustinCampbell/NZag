namespace NZag.Core

open System
open NZag.Reflection
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

    let checksum =
        let size = min (memory |> Header.readFileSize) memory.Size
        let mutable result = 0us
        for i = 0x40 to size - 1 do
            result <- result + uint16 (memory.ReadByte(i))
        result

    // Set header bits...
    do
        if memory.Version >= 4 then
            memory.WriteByte(0x1e, 6uy) // target
            memory.WriteByte(0x1f, (byte 'A')) // version

        memory.WriteByte(0x32, 1uy) // standard revision major version
        memory.WriteByte(0x33, 0uy) // standard revision minor version

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
        |> Async.StartAsPlainTask

    member x.Randomize seed =
        (x :> IMachine).Randomize(seed)

    member x.RegisterScreen newScreen =
        (x :> IMachine).RegisterScreen(newScreen)

    interface IMachine with

        member y.RegisterScreen(newScreen) =
            screen <- newScreen
            // TODO: Write screen header values
            outputStreams.RegisterScreenStream(newScreen)

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
        member y.ReadZTextOfLength(address, length) =
            textReader.ReadString(RawAddress(address), length)

        member y.WriteOutputChar(ch) =
            let work = (outputStreams :> IOutputStream).WriteCharAsync(ch)
            Async.RunSynchronously(work |> Async.AwaitTask)
        member y.WriteOutputText(s) =
            let work = (outputStreams :> IOutputStream).WriteTextAsync(s)
            Async.RunSynchronously(work |> Async.AwaitTask)

        member y.Randomize(seed) =
            random <- if seed = 0s then new Random(int DateTime.Now.Ticks)
                      else new Random(int +seed)
        member y.NextRandomNumber(range) =
            let minValue = 1us
            let maxValue = max minValue (uint16 (range - 1s))
            uint16 (random.Next(int minValue, int maxValue))

        member y.ReadInputChar() =
            let readCharTask = screen.ReadCharAsync()
            let ch = readCharTask.Result

            ch

        member y.ReadInputText(textBuffer, parseBuffer) =
            let dictionaryAddress = memory |> Header.readDictionaryAddress |> (fun a -> a.IntValue)
            let maxChars = int (memory.ReadByte(textBuffer))

            let readTextTask = screen.ReadTextAsync(maxChars)
            let text = readTextTask.Result

            // Write text to textBuffer
            let mutable address = textBuffer + 1

            if memory.Version >= 5 then
                memory.WriteByte(address, byte text.Length)
                address <- address + 1

            for i = 0 to text.Length - 1 do
                memory.WriteByte(address + i, byte text.[i])

            memory.WriteByte(address + text.Length, 0uy)

            // Tokenize command and write result to parseBuffer
            if parseBuffer > 0 then
                let tokens = memory |> Dictionary.tokenizeCommand text dictionaryAddress

                let maxWords = memory.ReadByte(parseBuffer)
                let parsedWords = min maxWords (byte tokens.Length)

                let mutable address = parseBuffer + 1
                memory.WriteByte(address, parsedWords)
                address <- address + 1

                for token in tokens do
                    let entryAddress = memory |> Dictionary.lookupWord token.Text dictionaryAddress
                    memory.WriteWord(address, uint16 entryAddress)
                    address <- address + 2

                    memory.WriteByte(address, byte token.Length)
                    address <- address + 1

                    memory.WriteByte(address, byte (token.Start + 1))
                    address <- address + 1

            0

        member y.Verify() =
            checksum = (memory |> Header.readChecksum)


