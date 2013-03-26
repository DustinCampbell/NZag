namespace NZag.Core

open System
open System.IO
open NZag.Extensions

type Memory(stream : Stream) =

    // We split memory into 64k chunks to avoid creating very large arrays.
    [<Literal>]
    let ChunkSize = 0x10000

    let version =
        do stream.Seek(0L, SeekOrigin.Begin) |> ignore

        match stream.NextByte() with
        | Some(b) -> if b >= 1uy && b <= 8uy then int b
                     else raise <| InvalidOperationException(sprintf "Invalid version number: %d" b)

        | None    -> raise <| InvalidOperationException("Could not read version")

    let size =
        do stream.Seek(0x1AL, SeekOrigin.Begin) |> ignore

        let packedSize =
            match stream.NextWord() with
            | Some(w) -> int w
            | None    -> raise <| InvalidOperationException("Could not read size")

        match version with
        | 1 | 2 | 3 -> packedSize * 2
        | 4 | 5     -> packedSize * 4
        | 6 | 7 | 8 -> packedSize * 8
        | _ -> raise <| InvalidOperationException()

    let chunks =
        do stream.Seek(0L, SeekOrigin.Begin) |> ignore

        let count =
            let count' = size / ChunkSize
            if size % ChunkSize > 0 then count' + 1
            else count'

        let readChunk() =
            let chunk = Array.zeroCreate ChunkSize

            let mutable bytesReadSoFar = 0
            let mutable stop = false

            while not stop do
                let read = stream.Read(chunk, bytesReadSoFar, ChunkSize - bytesReadSoFar)

                if read <= 0 then
                    stop <- true
                else
                    bytesReadSoFar <- bytesReadSoFar + read
                    if bytesReadSoFar = ChunkSize then
                        stop <- true
                    else
                        // We got less than expected, are we at the end of the stream?
                        match stream.NextByte() with
                        | Some(b) -> // Nope, it's not the end of the stream. Set the byte we
                                     // just read, increment, and carry on.
                                     chunk.[bytesReadSoFar] <- b
                                     bytesReadSoFar <- bytesReadSoFar + 1

                        | None    -> // Yup, this is the end of the stream, so we're done.
                                     stop <- true

            chunk

        Array.init count (fun _ -> readChunk())

    let mutable currentChunk = chunks.[0]
    let mutable currentChunkStart = 0
    let mutable currentChunkEnd = ChunkSize

    let selectChunk address =
        let chunkIndex = address / ChunkSize
        currentChunk <- chunks.[chunkIndex]
        currentChunkStart <- chunkIndex * ChunkSize
        currentChunkEnd <- currentChunkStart + ChunkSize

    let readByte address =
        if address < currentChunkStart || address >= currentChunkEnd then
            selectChunk address

        currentChunk.[address - currentChunkStart]

    let writeByte address value =
        if address < currentChunkStart || address >= currentChunkEnd then
            selectChunk address

        currentChunk.[address - currentChunkStart] <- value

    member x.ReadByte address =
        if address > size - 1 then
            raise <| ArgumentOutOfRangeException("address")

        readByte address

    member x.ReadWord address =
        if address > size - 2 then
            raise <| ArgumentOutOfRangeException("address")

        // We take a faster path if the entire word can be read from the current chunk
        if address >= currentChunkStart && address < currentChunkEnd - 2 then
            let chunk = currentChunk
            let chunkAddress = address - currentChunkStart

            ((uint16 chunk.[address]) <<< 8) |||
             (uint16 chunk.[address+1])
        else
            let b1 = readByte address
            let b2 = readByte (address+1)

            (uint16 b1 <<< 8) ||| uint16 b2

    member x.ReadDWord address =
        if address > size - 4 then
            raise <| ArgumentOutOfRangeException("address")

        // We take a faster path if the entire dword can be read from the current chunk
        if address >= currentChunkStart && address < currentChunkEnd - 4 then
            let chunk = currentChunk
            let chunkAddress = address - currentChunkStart

            ((uint32 chunk.[address]) <<< 24) |||
            ((uint32 chunk.[address+1]) <<< 16) |||
            ((uint32 chunk.[address+2]) <<< 8) |||
             (uint32 chunk.[address+3])
        else
            let b1 = readByte address
            let b2 = readByte (address+1)
            let b3 = readByte (address+2)
            let b4 = readByte (address+3)

            (uint32 b1 <<< 24) ||| (uint32 b1 <<< 16) ||| (uint32 b1 <<< 8) ||| uint32 b4

    member x.WriteByte address value =
        if address > size - 1 then
            raise <| ArgumentOutOfRangeException("address")

        writeByte address value

    member x.WriteWord address (value : uint16) =
        if address > size - 2 then
            raise <| ArgumentOutOfRangeException("address")

        // We take a faster path if the entire word can be written to the current chunk
        if address >= currentChunkStart && address < currentChunkEnd - 2 then
            let chunk = currentChunk
            let chunkAddress = address - currentChunkStart

            chunk.[chunkAddress]   <- byte (value >>> 8)
            chunk.[chunkAddress+1] <- byte (value &&& 0xffus)
        else
            byte (value >>> 8)      |> writeByte address
            byte (value &&& 0xffus) |> writeByte (address+1)

    member x.WriteDWord address (value : uint32) =
        if address > size - 4 then
            raise <| ArgumentOutOfRangeException("address")

        // We take a faster path if the entire dword can be written to the current chunk
        if address >= currentChunkStart && address < currentChunkEnd - 4 then
            let chunk = currentChunk
            let chunkAddress = address - currentChunkStart

            chunk.[chunkAddress]   <- byte (value >>> 24)
            chunk.[chunkAddress+1] <- byte (value >>> 16)
            chunk.[chunkAddress+2] <- byte (value >>> 8)
            chunk.[chunkAddress+3] <- byte (value &&& 0xffu)
        else
            byte (value >>> 24)    |> writeByte address
            byte (value >>> 16)    |> writeByte (address+1)
            byte (value >>> 8)     |> writeByte (address+2)
            byte (value &&& 0xffu) |> writeByte (address+3)

    member x.Size = size
    member x.Version = version

