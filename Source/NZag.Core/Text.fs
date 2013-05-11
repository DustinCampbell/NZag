﻿namespace NZag.Core

open System.Collections.Generic
open NZag.Utilities

type ZChar = byte
type ZCharEnumerator = IEnumerator<ZChar>

type AlphabetTable (memory : Memory) =

    let A0    = "      abcdefghijklmnopqrstuvwxyz" |> String.toCharArray
    let A1    = "      ABCDEFGHIJKLMNOPQRSTUVWXYZ" |> String.toCharArray
    let A2_v1 = "       0123456789.,!?_#'\"/\\<-:()" |> String.toCharArray
    let A2    = "       \n0123456789.,!?_#'\"/\\-:()" |> String.toCharArray

    let readCustomTable address =
        let byteToChar b =
            let ch = char b
            if ch = '^' then '\n' else ch

        let readAlphabet address length =
            // Every "alphabet" is 32 characters long.
            let buffer = Array.zeroCreate 32
            memory.Read buffer (32 - length) length address
            buffer |> Array.map byteToChar

        // The last "alphabet" is 25 characters long to account for the fact that A2/C6
        // isn't a printable character, though A0/C6 and A1/C6 are.
        [| readAlphabet address 26;
           readAlphabet (address + 26) 26;
           readAlphabet (address + 53) 25 |]

    let alphabets =
        match memory.Version with
        | 1 -> [|A0; A1; A2_v1|]
        | 2 | 3 | 4 -> [|A0; A1; A2|]
        | 5 | 6 | 7 | 8 ->
            let customTableAddress = memory |> Header.readAlphabetTableAddress
            if customTableAddress.IsZero then [|A0; A1; A2|]
            else readCustomTable customTableAddress
        | _ -> failcompilef "Unexpected version number: %d" memory.Version

    let baseAlphabet = ref 0
    let currentAlphabet = ref 0

    member x.Reset() =
        baseAlphabet := 0
        currentAlphabet := 0

    member x.Shift() =
        currentAlphabet := (!baseAlphabet + 1) % 3
    member x.DoubleShift() =
        currentAlphabet := (!baseAlphabet + 2) % 3
    member x.ShiftLock() =
        baseAlphabet := (!baseAlphabet + 1) % 3
        currentAlphabet := !baseAlphabet
    member x.DoubleShiftLock() =
        baseAlphabet := (!baseAlphabet + 2) % 3
        currentAlphabet := !baseAlphabet

    member x.GetChar (zchar : ZChar) =
        let result = alphabets.[!currentAlphabet].[int zchar]
        currentAlphabet := !baseAlphabet
        result

    member x.CurrentAlphabet = !currentAlphabet

type ICharProcessor =
    abstract member Reset : unit -> unit
    abstract member TryProcessNext : System.Text.StringBuilder -> ZCharEnumerator -> bool

[<RequireQualifiedAccess>]
module private ZText =

    let readZChars (reader : IMemoryReader) : ZCharEnumerator =
        seq {
            let stop = ref false
            while not (!stop) do
                let zword = reader.NextWord()
                let last = (zword &&& 0x8000us) = 0x8000us
                yield byte ((zword >>> 10) &&& 0x1fus)
                yield byte ((zword >>> 5) &&& 0x1fus)
                yield byte (zword &&& 0x1fus)
                stop := last || reader.AtEndOfMemory }
        |> Enumerable.getEnumerator

    let skipZChars (reader : IMemoryReader) =
        let mutable stop = false
        while not stop && not reader.AtEndOfMemory do
            let zword = reader.NextWord()
            stop <- (zword &&& 0x8000us) = 0x8000us

    let readString (reader : IMemoryReader) (charProcessor : ICharProcessor) =
        let builder = StringBuilder.create()
        let zcharEnum = readZChars reader

        charProcessor.Reset()
        while charProcessor.TryProcessNext builder zcharEnum do ()

        builder.ToString()

type CharProcessor (memory : Memory, ?abbreviationReader : AbbreviationReader) =

    let alphabetTable = new AlphabetTable(memory)

    let appendMultibyteZsciiChar (zcharEnum : ZCharEnumerator) builder =
        // If this is character 6 in A2, it's a multibyte ZSCII character
        // Note that it can be legal for the stream to end in the middle of a 
        // multi-byte ZSCII character (i.e. in the dictionary table). In that case,
        // the value is discared or an exception is thrown if that behavior
        // isn't allowed.

        // The alphabet table must be reset to ensure that the next zcode
        // after the multi-byte ZSCII character uses the correct alphabet.
        alphabetTable.Reset()

        // The next two characters make up a 10-bit ZSCII character
        match zcharEnum.Next(), zcharEnum.Next() with
        | Some(zc1), Some(zc2) ->
            let zscii = ((uint16 zc1 &&& 0x1fus) <<< 5) ||| uint16 zc2
            builder |> StringBuilder.appendChar (char zscii)
        | _ ->
            // Incomplete multi-byte characters are not allowed in abbreviations.
            if abbreviationReader.IsSome then
                failcompile "Encountered illegal incomplete multi-byte ZSCII character"

    let appendAbbreviation (zcharEnum : ZCharEnumerator) offset builder =
        if abbreviationReader.IsNone then
            failcompile "Encounted ZSCII code for an illegal abbreviation."

        match zcharEnum.Next() with
        | Some(code) ->
            let index = (32 * (offset - 1)) + int code
            let abbreviation = abbreviationReader.Value.GetAbbreviation(index)
            builder |> StringBuilder.appendString abbreviation
        | None -> ()

    let processChar_v1 builder zcharEnum (zchar : ZChar) =
        match zchar with
        | 0uy -> builder|> StringBuilder.appendChar(' ')
        | 1uy -> builder|> StringBuilder.appendChar('\n')
        | 2uy -> alphabetTable.Shift()
        | 3uy -> alphabetTable.DoubleShift()
        | 4uy -> alphabetTable.ShiftLock()
        | 5uy -> alphabetTable.DoubleShiftLock()
        | 6uy when alphabetTable.CurrentAlphabet = 2 -> builder |> appendMultibyteZsciiChar zcharEnum
        | zc -> if zc <= 31uy then builder |> StringBuilder.appendChar (alphabetTable.GetChar(zc))
                else failcompilef "Unexpected ZSCII character value: %d. Legal values are from 0 to 31." zc

    let processChar_v2 builder zcharEnum (zchar : ZChar) =
        match zchar with
        | 0uy -> builder|> StringBuilder.appendChar(' ')
        | 1uy -> builder |> appendAbbreviation zcharEnum 1
        | 2uy -> alphabetTable.Shift()
        | 3uy -> alphabetTable.DoubleShift()
        | 4uy -> alphabetTable.ShiftLock()
        | 5uy -> alphabetTable.DoubleShiftLock()
        | 6uy when alphabetTable.CurrentAlphabet = 2 -> builder |> appendMultibyteZsciiChar zcharEnum
        | zc -> if zc <= 31uy then builder |> StringBuilder.appendChar (alphabetTable.GetChar(zc))
                else failcompilef "Unexpected ZSCII character value: %d. Legal values are from 0 to 31." zc

    let processChar_v3 builder zcharEnum (zchar : ZChar) =
        match zchar with
        | 0uy -> builder|> StringBuilder.appendChar(' ')
        | 1uy -> builder |> appendAbbreviation zcharEnum 1
        | 2uy -> builder |> appendAbbreviation zcharEnum 2
        | 3uy -> builder |> appendAbbreviation zcharEnum 3
        | 4uy -> alphabetTable.Shift()
        | 5uy -> alphabetTable.DoubleShift()
        | 6uy when alphabetTable.CurrentAlphabet = 2 -> builder |> appendMultibyteZsciiChar zcharEnum
        | zc -> if zc <= 31uy then builder |> StringBuilder.appendChar (alphabetTable.GetChar(zc))
                else failcompilef "Unexpected ZSCII character value: %d. Legal values are from 0 to 31." zc

    let processChar =
        match memory.Version with
        | 1 -> processChar_v1
        | 2 -> processChar_v2
        | 3 | 4 | 5 | 6 | 7 | 8 -> processChar_v3
        | _ -> failcompilef "Unexpected version number: %d" memory.Version

    interface ICharProcessor with
        member x.Reset() =
            alphabetTable.Reset()

        member x.TryProcessNext builder zcharEnum =
            match zcharEnum.Next() with
            | Some(zc) -> zc |> processChar builder zcharEnum 
                          true
            | None     -> false

and AbbreviationReader (memory : Memory) =

    let charProcessor = new CharProcessor(memory)
    let baseAddress = memory |> Header.readAbbreviationTableAddress

    let readAbbreviationAddress index = 
        baseAddress + (index * 2) |> memory.ReadWord |> WordAddress

    member x.GetAbbreviation index =
        let reader = readAbbreviationAddress index |> memory.CreateMemoryReader
        ZText.readString reader charProcessor

type ZTextReader (memory : Memory) =

    let charProcessor = new CharProcessor(memory, new AbbreviationReader(memory))

    member x.ReadString (reader : IMemoryReader) =
        if reader.Memory <> memory then
            failcompile "Expected IMemoryReader from same memory"

        ZText.readString reader charProcessor

    member x.ReadString address =
        let reader = address |> memory.CreateMemoryReader
        x.ReadString reader
