namespace NZag.Core

open System
open NZag.Reflection
open NZag.Utilities

type IProfiler =
    abstract member RoutineCompiled : routine:Routine * compileTime:TimeSpan * ilByteSize:int * optimized:bool -> unit

    abstract member EnterRoutine : routine:Routine -> unit
    abstract member ExitRoutine : routine:Routine -> unit

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

    let mutable profiler = None

    let registerProfiler (p: IProfiler) =
        profiler <- Some(p)

    let compiledRoutineMap = Dictionary.create()

    let compileRoutine (routine: Routine) optimize =
        let watch = Measurement.start()

        let methodName =
            if optimize then
                sprintf "%4x_%d_locals_optimized" routine.Address.IntValue routine.Locals.Length
            else
                sprintf "%4x_%d_locals" routine.Address.IntValue routine.Locals.Length

        let dynamicMethod =
            new System.Reflection.Emit.DynamicMethod(
                name = methodName,
                returnType = typeof<uint16>,
                parameterTypes = TypeArray.Seven<Machine, Memory, uint16[], uint16[], int, ZFuncInvoker[], int>(),
                owner = typeof<Machine>,
                skipVisibility = true)

        let generator = dynamicMethod.GetILGenerator()
        let builder = new ILBuilder(generator)
        let invokers = ResizeArray.create()

        CodeGenerator.Compile(memory, routine, this, builder, invokers, optimize, debugging)

        let zfunc = dynamicMethod.CreateDelegate(typeof<ZFunc>, this) :?> ZFunc

        let compileTime = watch |> Measurement.stop

        let result = {
            Routine = routine
            ZFunc = zfunc
            Optimized = optimize
            Invokers = invokers.ToArray()
            CompileTime = compileTime
        }

        match profiler with
        | Some(p) -> p.RoutineCompiled(routine, compileTime, generator.ILOffset, optimize)
        | None -> ()

        result

    let compile (routine: Routine) optimize =
        let res = compileRoutine routine optimize
        compiledRoutineMap.[routine.Address] <- res
        res

    let getCompiledRoutine (routine: Routine) optimize =
        match compiledRoutineMap.TryGetValue(routine.Address) with
        | (true, res) -> res
        | (false, _) -> compile routine optimize

    let getRoutine =
        let reader = new RoutineReader(memory)

        memoize (fun (address: Address) -> reader.ReadRoutine(address))

    let invokerMap = Dictionary.create()

    let getInvoker address =
        match invokerMap.TryGetValue(address) with
        | (true, res) -> res
        | (false, _) ->
            let routine = RawAddress(address) |> getRoutine
            let res = new ZFuncInvoker(this, routine)
            invokerMap.[address] <- res
            res

    member x.Memory = memory

    member x.RunAsync() =
        async {
            let reader = new RoutineReader(memory)
            let invoker = getInvoker mainRoutineAddress.IntValue
            let stack = Array.zeroCreate 1024
            invoker.Invoke0(memory, stack, 0) |> ignore
        }
        |> Async.StartAsPlainTask

    member x.Randomize seed =
        (x :> IMachine).Randomize(seed)

    member x.RegisterProfiler profiler =
        registerProfiler profiler

    member x.RegisterScreen newScreen =
        screen <- newScreen

        if memory.Version >= 4 then
            memory |> Header.writeScreenHeightInLines screen.ScreenHeightInLines
            memory |> Header.writeScreenWidthInColumns screen.ScreenWidthInColumns

        if memory.Version >= 5 then
            memory |> Header.writeScreenHeightInUnits screen.ScreenHeightInUnits
            memory |> Header.writeScreenWidthInUnits screen.ScreenWidthInUnits
            memory |> Header.writeFontHeightInUnits screen.FontHeightInUnits
            memory |> Header.writeFontWidthInUnits screen.FontWidthInUnits

        outputStreams.RegisterScreenStream(newScreen)

    member x.ForceFixedWidthFont() =
        (memory.ReadWord(0x10) &&& 0x0002us) = 0x0002us

    member x.IsScoreGame() =
        if memory.Version < 3 then
            true
        else
            (memory.ReadByte(0x01) &&& 0x01uy) = 0x00uy

    member x.ReadGlobalVariable index =
        let globalVariableTableAddress = memory |> Header.readGlobalVariableTableAddress |> (fun a -> a.IntValue)
        let globalVariableAddress = globalVariableTableAddress + (index * 2)
        memory.ReadWord(globalVariableAddress)

    member x.ReadObjectShortName objectNumber =
        let objectTableAddress = memory |> Header.readObjectTableAddress |> (fun a -> a.IntValue)
        let propertyCount = if memory.Version <= 3 then 31 else 63
        let propertyDefaultsTableSize = propertyCount * 2
        let objectEntriesAddress = objectTableAddress + propertyDefaultsTableSize
        let objectEntrySize = if memory.Version <= 3 then 9 else 14
        let objectAddress = objectEntriesAddress + ((objectNumber - 1) * objectEntrySize)
        let propertyTableAddressOffset = if memory.Version <= 3 then 7 else 12
        let propertyTableAddress = int (memory.ReadWord(objectAddress + propertyTableAddressOffset))
        let length = int (memory.ReadByte(propertyTableAddress))
        textReader.ReadString(RawAddress(propertyTableAddress + 1), length)

    interface IMachine with

        member m.Debugging = debugging
        member m.Profiling = profiler.IsSome

        member m.EnterRoutine routine =
            match profiler with
            | Some(p) -> p.EnterRoutine(routine)
            | None -> ()

        member m.ExitRoutine routine =
            match profiler with
            | Some(p) -> p.ExitRoutine(routine)
            | None -> ()


        member y.GetInitialLocalArray(routine) =
            let result = getOrCreateLocalArray()

            if memory.Version < 5 then
                routine.Locals
                |> List.iteri (fun i v -> if v > 0us then result.[i] <- v)

            result

        member y.ReleaseLocalArray(locals) =
            releaseLocalArray locals

        member y.Compile(routine, optimize) =
            compile routine optimize
        member y.GetInvoker(address) =
            getInvoker address

        member y.Verify() =
            checksum = (memory |> Header.readChecksum)

        member y.Randomize(seed) =
            random <- if seed = 0s then new Random(int DateTime.Now.Ticks)
                      else new Random(int +seed)
        member y.NextRandomNumber(range) =
            let minValue = 1us
            let maxValue = max minValue (uint16 (range - 1s))
            uint16 (random.Next(int minValue, int maxValue))

        member y.ReadZText(address) =
            textReader.ReadString(RawAddress(address))
        member y.ReadZTextOfLength(address, length) =
            textReader.ReadString(RawAddress(address), length)

        member y.ReadInputChar() =
            let readCharTask = screen.ReadCharAsync()
            let ch = readCharTask.Result

            ch

        member y.ReadInputText(textBuffer, parseBuffer) =
            if memory.Version <= 3 then
                screen.ShowStatusAsync().Wait()

            let dictionaryAddress = memory |> Header.readDictionaryAddress |> (fun a -> a.IntValue)
            let maxChars = int (memory.ReadByte(textBuffer))

            let readTextTask = screen.ReadTextAsync(maxChars)
            let text = readTextTask.Result

            let text = text.ToLower()

            if debugging then
                System.Diagnostics.Debug.WriteLine(sprintf "TEXT INPUT: %s" text)

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

        member y.ReadTimedInputChar(time, routine) =
            // TODO: Support timed input
            let readCharTask = screen.ReadCharAsync()
            let ch = readCharTask.Result

            ch

        member y.SelectOutputStream number =
            match number with
            |  0s -> ()
            |  1s -> outputStreams.SelectScreenStream()
            | -1s -> outputStreams.DeselectScreenStream()
            |  2s -> outputStreams.SelectTranscriptStream()
            | -2s -> outputStreams.DeselectTranscriptStream()
            |  3s -> failruntime "Unexpected: An address must be supplied when selecting a memory stream"
            | -3s -> outputStreams.DeselectMemoryStream()
            | -4s | 4s -> failruntime "Stream 4 is not supported"
            |  _ -> failruntimef "Invalid stream number %d" number

        member y.SelectOutputStream(number, table) =
            match number with
            |  0s -> ()
            |  1s -> outputStreams.SelectScreenStream()
            | -1s -> outputStreams.DeselectScreenStream()
            |  2s -> outputStreams.SelectTranscriptStream()
            | -2s -> outputStreams.DeselectTranscriptStream()
            |  3s -> outputStreams.SelectMemoryStream(ByteAddress(table))
            | -3s -> outputStreams.DeselectMemoryStream()
            | -4s | 4s -> failruntime "Stream 4 is not supported"
            |  _ -> failruntimef "Invalid stream number %d" number

        member y.WriteOutputChar(ch) =
            let work = (outputStreams :> IOutputStream).WriteCharAsync(ch)
            Async.RunSynchronously(work |> Async.AwaitTask)
        member y.WriteOutputText(s) =
            let work = (outputStreams :> IOutputStream).WriteTextAsync(s)
            Async.RunSynchronously(work |> Async.AwaitTask)

        member y.SetWindow(window) =
            screen.SetWindowAsync(int window).Wait()
        member y.ClearWindow(window) =
            if window >= 0s then screen.ClearAsync(int window).Wait()
            elif window = -1s then screen.ClearAllAsync(true).Wait()
            elif window = -2s then screen.ClearAllAsync(false).Wait()
        member y.SplitWindow(lines) =
            if lines = 0s then
                screen.UnsplitAsync().Wait()
            else
                screen.SplitAsync(int lines).Wait()

        member y.SetCursor(line, column) =
            screen.SetCursorAsync(line, column).Wait()

        member y.SetTextStyle(style) =
            screen.SetTextStyleAsync(style).Wait()

