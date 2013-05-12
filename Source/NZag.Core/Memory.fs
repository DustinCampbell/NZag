namespace NZag.Core

open System
open System.IO
open NZag.Utilities

type Address =
    /// A byte address specifies a byte in memory in the range 0 up to the last byte of static memory.
    | ByteAddress of uint16

    /// A word address specifies an even address in the bottom 128K of memory (by giving the address
    /// divided by 2). (Word addresses are used only in the abbreviations table.)
    | WordAddress of uint16

    /// A routine address is a packed address that specifies where a routine begins in high memory.
    | RoutineAddress of uint16

    /// A string address is a packed address that specifies where a string begins in high memory.
    | StringAddress of uint16

    /// A raw, translated address anywhere in memory
    | RawAddress of int

    override x.ToString() =
        match x with
        | ByteAddress(a)    -> sprintf "byte: %04x" a
        | WordAddress(a)    -> sprintf "word: %04x" a
        | RoutineAddress(a) -> sprintf "routine: %04x" a
        | StringAddress(a)  -> sprintf "string: %04x" a
        | RawAddress(a)     -> sprintf "raw: %08x" a

    member x.IsZero =
        match x with
        | ByteAddress(a)    -> a = 0us
        | WordAddress(a)    -> a = 0us
        | RoutineAddress(a) -> a = 0us
        | StringAddress(a)  -> a = 0us
        | RawAddress(a)     -> a = 0

    member x.IntValue =
        match x with
        | ByteAddress(a)    -> int a
        | WordAddress(a)    -> int a
        | RoutineAddress(a) -> int a
        | StringAddress(a)  -> int a
        | RawAddress(a)     -> a

    static member (+) (x, y) =
        let add_uint16 x y =
            let result = (int x) + y
            if result < 0 || result > int UInt16.MaxValue then
                failwith "Operation caused address overflow"
            uint16 result

        let add_int32 x y =
            let result = int64 x + int64 y
            if result < 0L || result > int64 Int32.MaxValue then
                failwith "Operation caused address overflow"
            int result

        match x with
        | ByteAddress(a)    -> ByteAddress(add_uint16 a y)
        | WordAddress(a)    -> WordAddress(add_uint16 a y)
        | RoutineAddress(a) -> RoutineAddress(add_uint16 a y)
        | StringAddress(a)  -> StringAddress(add_uint16 a y)
        | RawAddress(a)     -> RawAddress(add_int32 a y)

    static member (-) (x, y) =
        let subtract_uint16 x y =
            let result = (int x) - y
            if result < 0 || result > int UInt16.MaxValue then
                failwith "Operation caused address overflow"
            uint16 result

        let subtract_int32 x y =
            let result = int64 x - int64 y
            if result < 0L || result > int64 Int32.MaxValue then
                failwith "Operation caused address overflow"
            int result

        match x with
        | ByteAddress(a)    -> ByteAddress(subtract_uint16 a y)
        | WordAddress(a)    -> WordAddress(subtract_uint16 a y)
        | RoutineAddress(a) -> RoutineAddress(subtract_uint16 a y)
        | StringAddress(a)  -> StringAddress(subtract_uint16 a y)
        | RawAddress(a)     -> RawAddress(subtract_int32 a y)

type IMemoryReader =
    /// Read the next byte without incrementing the address
    abstract member PeekByte : unit -> byte
    /// Read the next word without incrementing the address
    abstract member PeekWord : unit -> uint16
    /// Read the next dword without incrementing the address
    abstract member PeekDWord : unit -> uint32

    /// Read the next byte
    abstract member NextByte : unit -> byte
    /// Read the next word
    abstract member NextWord : unit -> uint16
    /// Read the next dword
    abstract member NextDWord : unit -> uint32

    /// Read a byte array of the given count
    abstract member NextBytes : int -> byte[]
    /// Read a word array of the given count
    abstract member NextWords : int -> uint16[]

    /// Increment the address by a given number of positive bytes
    abstract member SkipBytes : int -> unit

    /// The current address to read from
    abstract member Address : Address
    /// Determines whether the address of this reader is at or past the end of the memory.
    abstract member AtEndOfMemory : bool

    /// The memory this reader was created from
    abstract member Memory : Memory

and Memory private (stream : Stream) =

    // We split memory into 64k chunks to avoid creating very large arrays.
    [<Literal>]
    let ChunkSize = 0x10000

    let version =
        do stream.Seek(0L, SeekOrigin.Begin) |> ignore

        match stream.NextByte() with
        | Some(b) -> if b >= 1uy && b <= 8uy then int b
                     else failwithf "Invalid version number: %d" b

        | None    -> failwith "Could not read version"

    let size =
        do stream.Seek(0x1AL, SeekOrigin.Begin) |> ignore

        let packedSize =
            match stream.NextWord() with
            | Some(w) -> int w
            | None    -> failwith "Could not read size"

        match version with
        | 1 | 2 | 3 -> packedSize * 2
        | 4 | 5     -> packedSize * 4
        | 6 | 7 | 8 -> packedSize * 8
        | v -> failwithf "Invalid version number: %d" v

    let packedMultiplier =
        match version with
        | 1 | 2 | 3 -> 2
        | 4 | 5 | 6 | 7 -> 4
        | 8 -> 8
        | v -> failwithf "Invalid version number: %d" v

    let routinesOffset =
        do stream.Seek(0x28L, SeekOrigin.Begin) |> ignore

        match stream.NextWord() with
        | Some(w) -> int w * 8
        | None    -> failwith "Could not read routines offset"

    let stringsOffset =
        do stream.Seek(0x2AL, SeekOrigin.Begin) |> ignore

        match stream.NextWord() with
        | Some(w) -> int w * 8
        | None    -> failwith "Could not read static strings offset"

    let translate address =
        match address with
        | ByteAddress(a)    -> int a
        | WordAddress(a)    -> int a * 2
        | RoutineAddress(a) -> (int a * packedMultiplier) + routinesOffset
        | StringAddress(a)  -> (int a * packedMultiplier) + stringsOffset
        | RawAddress(a)     -> a

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

    let readWord address =
        // We take a faster path if the entire word can be read from the current chunk
        if address >= currentChunkStart && address < currentChunkEnd - 2 then
            let chunk = currentChunk
            let chunkAddress = address - currentChunkStart

            ((uint16 chunk.[chunkAddress]) <<< 8) |||
             (uint16 chunk.[chunkAddress+1])
        else
            let b1 = readByte  address
            let b2 = readByte (address+1)

            (uint16 b1 <<< 8) ||| uint16 b2

    member x.Read (buffer : byte[]) offset count address =
        if buffer = null then
            argNull "buffer" "buffer is null"
        if offset < 0 then
            argOutOfRange "offset" "offset is less than zero"
        if count < 0 then
            argOutOfRange "count" "count is less than zero"
        if offset + count > buffer.Length then
            argOutOfRange "count" "count is larger than buffer size"
        if count > size then
            argOutOfRange "count" "count is larger than the Memory size"

        let address' = translate address
        if address' > size - count then
            argOutOfRange "address" "Expected address to be in range 0 to %d" (size - count)

        let mutable readSoFar = offset

        while readSoFar < count do
            let chunkIndex = (address' + readSoFar) / ChunkSize
            let chunk = chunks.[chunkIndex]
            let chunkStart = chunkIndex * ChunkSize
            let chunkEnd = chunkStart + ChunkSize
            let chunkOffset = (address' + readSoFar) - chunkStart
            let amountToRead = min (count - readSoFar) (chunkEnd - (chunkStart + chunkOffset))

            Array.blit chunk chunkOffset buffer readSoFar amountToRead
            readSoFar <- readSoFar + amountToRead

    member x.ReadByte address =
        if address > size - 1 then
            argOutOfRange "address" "Expected address to be in range 0 to %d" (size - 1)

        readByte address

    member x.ReadByte address =
        x.ReadByte(translate address)

    member x.ReadBytes address count =
        if count > size then
            argOutOfRange "count" "count is larger than the Memory size"

        let address' = translate address
        if address' > size - count then
            argOutOfRange "address" "Expected address to be in range 0 to %d" (size - count)

        let buffer = Array.zeroCreate count
        let mutable readSoFar = 0

        while readSoFar < count do
            let chunkIndex = (address' + readSoFar) / ChunkSize
            let chunk = chunks.[chunkIndex]
            let chunkStart = chunkIndex * ChunkSize
            let chunkEnd = chunkStart + ChunkSize
            let offset = (address' + readSoFar) - chunkStart
            let amountToRead = min (count - readSoFar) (chunkEnd - (chunkStart + offset))

            Array.blit chunk offset buffer readSoFar amountToRead
            readSoFar <- readSoFar + amountToRead

        buffer

    member x.ReadWord address =
        if address > size - 2 then
            argOutOfRange "address" "Expected address to be in range 0 to %d" (size - 2)

        readWord address

    member x.ReadWord address =
        x.ReadWord(translate address)

    member x.ReadWords address count =
        if (count * 2) > size then
            argOutOfRange "count" "count is larger than the Memory size"

        let address' = translate address
        if address' > size - (count * 2) then
            argOutOfRange "address" "Expected address to be in range 0 to % d" (size - count)

        let buffer = Array.zeroCreate count
        for i = 0 to count - 1 do
            buffer.[i] <- readWord (address' + (i * 2))

        buffer

    member x.ReadDWord address =
        if address > size - 4 then
            argOutOfRange "address" "Expected address to be in range 0 to %d" (size - 4)

        // We take a faster path if the entire dword can be read from the current chunk
        if address >= currentChunkStart && address < currentChunkEnd - 4 then
            let chunk = currentChunk
            let chunkAddress = address - currentChunkStart

            ((uint32 chunk.[chunkAddress]) <<< 24) |||
            ((uint32 chunk.[chunkAddress+1]) <<< 16) |||
            ((uint32 chunk.[chunkAddress+2]) <<< 8) |||
             (uint32 chunk.[chunkAddress+3])
        else
            let b1 = readByte  address
            let b2 = readByte (address+1)
            let b3 = readByte (address+2)
            let b4 = readByte (address+3)

            (uint32 b1 <<< 24) ||| (uint32 b1 <<< 16) ||| (uint32 b1 <<< 8) ||| uint32 b4

    member x.ReadDWord address =
        x.ReadDWord(translate address)

    member x.WriteByte(address, value) =
        if address > size - 1 then
            argOutOfRange "address" "Expected address to be in range 0 to %d" (size - 1)

        writeByte address value

    member x.WriteByte(address, value) =
        x.WriteByte(translate address, value)

    member x.WriteWord(address, value: uint16) =
        if address > size - 2 then
            argOutOfRange "address" "Expected address to be in range 0 to %d" (size - 2)

        // We take a faster path if the entire word can be written to the current chunk
        if address >= currentChunkStart && address < currentChunkEnd - 2 then
            let chunk = currentChunk
            let chunkAddress = address - currentChunkStart

            chunk.[chunkAddress]    <- byte (value >>> 8)
            chunk.[chunkAddress+1]  <- byte (value &&& 0xffus)
        else
            byte (value >>> 8)      |> writeByte  address
            byte (value &&& 0xffus) |> writeByte (address+1)

    member x.WriteWord(address, value: uint16) =
        x.WriteWord(translate address, value)

    member x.WriteDWord(address, value: uint32) =
        if address > size - 4 then
            argOutOfRange "address" "Expected argument to be in range 0 to %d" (size - 4)

        // We take a faster path if the entire dword can be written to the current chunk
        if address >= currentChunkStart && address < currentChunkEnd - 4 then
            let chunk = currentChunk
            let chunkAddress = address - currentChunkStart

            chunk.[chunkAddress]   <- byte (value >>> 24)
            chunk.[chunkAddress+1] <- byte (value >>> 16)
            chunk.[chunkAddress+2] <- byte (value >>> 8)
            chunk.[chunkAddress+3] <- byte (value &&& 0xffu)
        else
            byte (value >>> 24)    |> writeByte  address
            byte (value >>> 16)    |> writeByte (address+1)
            byte (value >>> 8)     |> writeByte (address+2)
            byte (value &&& 0xffu) |> writeByte (address+3)

    member x.WriteDWord(address, value: uint32) =
        x.WriteDWord(translate address, value)

    member x.Size = size
    member x.Version = version

    member x.CreateMemoryReader address =
        let readerAddress = ref (translate address)
        let readerChunk = ref None
        let readerChunkOffset = ref 0

        let readPastEndOfMemory() =
            failwith "Attempted to read past end of memory."

        let getChunk() =
            match !readerChunk with
            | Some(chunk) -> chunk
            | None -> readPastEndOfMemory()

        let selectChunk() =
            let readerAddress' = !readerAddress
            let chunkIndex = readerAddress' / ChunkSize
            readerChunkOffset := readerAddress' % ChunkSize
            readerChunk := if chunkIndex < chunks.Length then Some(chunks.[chunkIndex]) else None

        let increment count =
            readerAddress := !readerAddress + count
            let readerChunkOffset' = !readerChunkOffset + count
            if readerChunkOffset' < ChunkSize then
                readerChunkOffset := readerChunkOffset'
            else
                selectChunk()

        let readNextByte() =
            match !readerChunk with
            | Some(chunk) ->
                let result = chunk.[!readerChunkOffset]
                increment 1
                result
            | None ->
                readPastEndOfMemory()

        let readNextWord() =
            match !readerChunk with
            | Some(chunk) ->
                // We take a faster path if the entire word can be written to the current chunk
                let readerChunkOffset' = !readerChunkOffset
                if readerChunkOffset' <= ChunkSize - 2 then
                    let chunk = getChunk()
                    let result = ((uint16 chunk.[readerChunkOffset']) <<< 8) |||
                                  (uint16 chunk.[readerChunkOffset'+1])

                    increment 2
                    result
                else
                    let b1 = readNextByte()
                    let b2 = readNextByte()

                    (uint16 b1 <<< 8) ||| uint16 b2
            | None ->
                readPastEndOfMemory()

        let peek f =
            let oldReaderAddress = !readerAddress
            let oldReaderChunk = !readerChunk
            let oldReaderChunkOffset = !readerChunkOffset

            let result = f()

            readerAddress := oldReaderAddress
            readerChunk := oldReaderChunk
            readerChunkOffset := oldReaderChunkOffset

            result

        selectChunk()

        { new IMemoryReader with
            member y.PeekByte() =
                peek (fun () -> y.NextByte())
            member y.PeekWord() =
                peek (fun () -> y.NextWord())
            member y.PeekDWord() =
                peek (fun () -> y.NextDWord())

            member y.NextByte() =
                if !readerAddress > size - 1 then
                    readPastEndOfMemory()

                readNextByte()

            member y.NextWord() =
                if !readerAddress > size - 2 then
                    readPastEndOfMemory()

                readNextWord()

            member y.NextDWord() =
                if !readerAddress > size - 4 then
                    readPastEndOfMemory()

                // We take a faster path if the entire dword can be written to the current chunk
                let readerChunkOffset' = !readerChunkOffset
                if readerChunkOffset' <= ChunkSize - 4 then
                    let chunk = getChunk()
                    let result = ((uint32 chunk.[readerChunkOffset']) <<< 24) |||
                                 ((uint32 chunk.[readerChunkOffset'+1]) <<< 16) |||
                                 ((uint32 chunk.[readerChunkOffset'+2]) <<< 8) |||
                                  (uint32 chunk.[readerChunkOffset'+3])

                    increment 4
                    result
                else
                    let b1 = readNextByte()
                    let b2 = readNextByte()
                    let b3 = readNextByte()
                    let b4 = readNextByte()

                    (uint32 b1 <<< 24) ||| (uint32 b2 <<< 16) ||| (uint32 b3 <<< 8) ||| uint32 b4

            member y.NextBytes count =
                if !readerAddress > size - count then
                    readPastEndOfMemory()

                let buffer = Array.zeroCreate count
                for i = 0 to count - 1 do
                    buffer.[i] <- readNextByte()

                buffer

            member y.NextWords count =
                if !readerAddress > size - (count * 2) then
                    readPastEndOfMemory()

                let buffer = Array.zeroCreate count
                for i = 0 to count - 1 do
                    buffer.[i] <- readNextWord()

                buffer

            member y.SkipBytes count =
                if count < 0 then
                    argOutOfRange "count" "count was less than 0."
                if count > 0 then
                    increment count

            member y.Address = RawAddress(!readerAddress)
            member y.AtEndOfMemory = !readerAddress >= size

            member y.Memory = x
        }

    static member CreateFrom(stream : Stream) =
        let position = stream.Position
        let memory = new Memory(stream)
        stream.Position <- position

        memory

module Header =

    let private offset_InitialPC = ByteAddress(0x06us)
    let private offset_GlobalVariableTableAddress = ByteAddress(0x06us)
    let private offset_AbbreviationTableAddress = ByteAddress(0x18us)
    let private offset_RoutinesOffset = ByteAddress(0x28us)
    let private offset_StringsOffset = ByteAddress(0x2aus)
    let private offset_AlphabetTableAddress = ByteAddress(0x34us)

    let readMainRoutineAddress (memory: Memory) =
        let initialPC = offset_InitialPC |> memory.ReadWord
        if memory.Version < 6 then
            ByteAddress(initialPC - 1us)
        else
            RoutineAddress(initialPC)

    let readGlobalVariableTableAddress (memory: Memory) =
        offset_GlobalVariableTableAddress |> memory.ReadWord |> ByteAddress

    let readAbbreviationTableAddress (memory: Memory) =
        offset_AbbreviationTableAddress |> memory.ReadWord |> ByteAddress

    let readRoutinesOffset (memory: Memory) =
        offset_RoutinesOffset |> memory.ReadWord |> ByteAddress

    let readStringOffset (memory: Memory) =
        offset_StringsOffset |> memory.ReadWord |> ByteAddress

    let readAlphabetTableAddress (memory: Memory) =
        offset_AlphabetTableAddress |> memory.ReadWord |> ByteAddress
