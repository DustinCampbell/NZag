namespace NZag.Core

open NZag.Utilities

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

        | _ -> Exceptions.invalidOperation "Unexpected version number: %d" memory.Version
