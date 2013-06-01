namespace NZag.Core

open System.Threading.Tasks
open NZag.Utilities

type IInputStream =
    abstract member ReadCharAsync : unit -> Task<char>
    abstract member ReadTextAsync : maxChars:int -> Task<string>

type IOutputStream =
    abstract member WriteCharAsync : ch:char -> Task
    abstract member WriteTextAsync : text:string -> Task

type IScreen =
    inherit IInputStream
    inherit IOutputStream

module NullInstances =

    let InputStream =
        { new IInputStream with
            member x.ReadCharAsync() = async { return char 0 } |> Async.StartAsTask
            member x.ReadTextAsync _ = async { return "" } |> Async.StartAsTask }

    let OutputStream =
        { new IOutputStream with
            member x.WriteCharAsync _ = async { return () } |> Async.StartAsPlainTask
            member x.WriteTextAsync _ = async { return () } |> Async.StartAsPlainTask }

    let Screen =
        { new IScreen with
            member x.ReadCharAsync() = async { return char 0 } |> Async.StartAsTask
            member x.ReadTextAsync _ = async { return "" } |> Async.StartAsTask

            member x.WriteCharAsync _ = async { return () } |> Async.StartAsPlainTask
            member x.WriteTextAsync _ = async { return () } |> Async.StartAsPlainTask }

type MemoryOutputStream(memory: Memory, address: Address) =

    let mutable count = 0us

    let writeAddress() = address + (2 + int count)
    let writeCount() = memory.WriteWord(address, count)

    do writeCount()

    let charToByte ch =
        if ch = '\n' then 13uy else byte ch

    interface IOutputStream with
        member x.WriteCharAsync ch =
            async {
                memory.WriteByte(writeAddress(), charToByte ch)
                count <- count + 1us
                writeCount()
            }
            |> Async.StartAsPlainTask

        member x.WriteTextAsync s =
            async {
                let bytes = s |> String.toCharArray |> Array.map charToByte
                memory.WriteBytes(writeAddress(), bytes)
                count <- count + (uint16 bytes.Length)
                writeCount()
            }
            |> Async.StartAsPlainTask

type OutputStreamCollection(memory: Memory) =
    let memoryStreams = Stack.create()

    let mutable screenStream = NullInstances.OutputStream
    let mutable screenActive = true

    let mutable transcriptStream = NullInstances.OutputStream
    let mutable transcriptActive = true

    member x.RegisterScreenStream(stream: IOutputStream) =
        screenStream <- stream
    member x.RegisterTranscriptStream(stream: IOutputStream) =
        transcriptStream <- stream

    member x.SelectScreenStream() =
        screenActive <- true
    member x.DeselectScreenStream() =
        screenActive <- false

    member x.SelectTranscriptStream() =
        transcriptActive <- true

        // set transcript flag
        let flags2 = memory.ReadWord(0x10)
        let flags2' = flags2 ||| 0x01us
        memory.WriteWord(0x10, flags2')

    member x.DeselectTranscriptStream() =
        transcriptActive <- false

        // set transcript flag
        let flags2 = memory.ReadWord(0x10)
        let flags2' = flags2 &&& ~~~0x01us
        memory.WriteWord(0x10, flags2')

    member x.SelectMemoryStream(address: Address) =
        if memoryStreams.Count = 16 then
            failruntime "Cannot create more than 16 memory output streams."

        memoryStreams |> Stack.push (new MemoryOutputStream(memory, address) :> IOutputStream)

    interface IOutputStream with

        // If a member stream is active, we don't output to other streams

        member x.WriteCharAsync ch =
            if memoryStreams |> Stack.isEmpty then
                if screenActive then
                    screenStream.WriteCharAsync(ch)
                elif transcriptActive then
                    transcriptStream.WriteCharAsync(ch)
                else
                    async { return () } |> Async.StartAsPlainTask
            else
                memoryStreams |> Stack.pop |> (fun stream -> stream.WriteCharAsync(ch))

        member x.WriteTextAsync s =
            if memoryStreams |> Stack.isEmpty then
                if screenActive then
                    screenStream.WriteTextAsync(s)
                elif transcriptActive then
                    transcriptStream.WriteTextAsync(s)
                else
                    async { return () } |> Async.StartAsPlainTask
            else
                memoryStreams |> Stack.pop |> (fun stream -> stream.WriteTextAsync(s))
