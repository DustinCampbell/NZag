namespace NZag.Core

open NZag.Utilities

module Dictionary =

    type CommandToken =
      { Start : int
        Length : int
        Text : string }

    let tokenizeCommand (commandText: string) (dictionaryAddress: int) (memory: Memory) =
        let reader = memory.CreateMemoryReader(dictionaryAddress)
        let wordSeparatorCount = int (reader.NextByte())
        let wordSeparators = reader.NextBytes(wordSeparatorCount) |> Array.map char

        let tokens = ResizeArray.create()

        let mutable start = -1
        let mutable i = 0

        while i < commandText.Length do
            let ch = commandText.[i]

            if start < 0 then
                if ch <> ' ' then
                    start <- i

                if wordSeparators |> Array.exists ((=) ch) then
                    tokens.Add({Start = i; Length = 1; Text = ch.ToString()})
                    start <- -1

            elif ch = ' ' then
                let length = i - start
                tokens.Add({Start = start; Length = length; Text = commandText.Substring(start, length)})
                start <- -1

            elif wordSeparators |> Array.exists ((=) ch) then
                let length = i - start
                tokens.Add({Start = start; Length = length; Text = commandText.Substring(start, length)})
                tokens.Add({Start = i; Length = 1; Text = ch.ToString()})
                start <- -1

            i <- i + 1

        if start >= 0 then
            let length = commandText.Length - start
            tokens.Add({Start = start; Length = length; Text = commandText.Substring(start, length)})

        tokens |> ResizeArray.toArray

    let lookupWord (word: string) (dictionaryAddress: int) (memory: Memory) =
        let alphabetTable = new AlphabetTable(memory)
        let version = alphabetTable.Version
        let resolution = if version <= 3 then 2 else 3

        let word =
            if word.Length > (resolution * 3) then
                word.Substring(0, resolution * 3)
            else
                word

        let encoded = word |> ZText.encodeZText alphabetTable

        let reader = memory.CreateMemoryReader(dictionaryAddress)
        let wordSeparatorCount = int (reader.NextByte())
        reader.SkipBytes(wordSeparatorCount)

        let entryLength = reader.NextByte()

        let sorted, entryCount =
            let w = reader.NextWord()
            if (int16 w) < 0s then
                (false, uint16 -(int16 w))
            else
                (true, w)

        let entryAddressBase = reader.Address.IntValue

        let mutable lower = 0
        let mutable upper = int entryCount - 1
        let mutable result = None

        while result.IsNone && lower <= upper do
            let entryNumber =
                if sorted then (lower + upper) / 2
                else lower

            let entryAddress = entryAddressBase + (entryNumber * int entryLength)

            let mutable entryCursor = entryAddress
            let mutable i = 0
            let mutable stop = false

            while not stop && i < resolution do
                let entry = memory.ReadWord(entryCursor)
                if encoded.[i] <> entry then
                    if sorted then
                        if encoded.[i] > entry then
                            lower <- entryNumber + 1
                        else
                            upper <- entryNumber - 1
                    else
                        lower <- lower + 1

                    stop <- true
                else
                    entryCursor <- entryCursor + 2
                    i <- i + 1

            if not stop then
                result <- Some(entryAddress)

        match result with
        | Some(entryAddress) -> entryAddress
        | None -> 0

