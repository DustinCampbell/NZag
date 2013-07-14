namespace NZag.Core

open NZag.Utilities

module Dictionary =

    let readWordSeparators (dictionaryAddress: int) (memory: Memory) =
        let count = memory.ReadByte (dictionaryAddress)
        memory.ReadBytes (dictionaryAddress + 1) (int count)

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

        let entryAddressBase = reader.Address

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

    let private createTokenizeWord textBuffer parseBuffer dictionaryAddress ignoreUnrecognizedWords (memory: Memory) =
        (fun start length ->
            let mutable address = parseBuffer

            let tokenMax = memory.ReadByte(address)
            address <- address + 1
            let tokenCount = memory.ReadByte(address)

            if tokenCount < tokenMax then
                memory.WriteByte(address, tokenCount + 1uy)
                address <- address + 1

            let wordChars = Array.zeroCreate length
            for i = 0 to length - 1 do
                wordChars.[i] <- char (memory.ReadByte(textBuffer + start + i))

            let word = String.fromCharArray wordChars

            let wordAddress = memory |> lookupWord word dictionaryAddress

            if wordAddress <> 0 || not ignoreUnrecognizedWords then
                address <- address + int (tokenCount * 4uy)

                memory.WriteWord(address, uint16 wordAddress)
                memory.WriteByte(address + 2, byte length)
                memory.WriteByte(address + 3, byte start)
        )

    let tokenize textBuffer parseBuffer dictionaryAddress ignoreUnrecognizedWords (memory: Memory) =
        // Use standard dictionary if none is provided.
        let dictionaryAddress =
            if dictionaryAddress = 0 then int (memory |> Header.readDictionaryAddress)
            else dictionaryAddress

        // Read in the separators
        let wordSeparators = memory |> readWordSeparators dictionaryAddress

        let tokenizeWord = memory |> createTokenizeWord textBuffer parseBuffer dictionaryAddress ignoreUnrecognizedWords

        // Set number of parse tokens to zero.
        memory.WriteByte(parseBuffer + 1, 0uy)

        let mutable startAddress = 0
        let mutable endAddress = textBuffer

        let length =
            if memory.Version >= 5 then
                endAddress <- endAddress + 1
                int (memory.ReadByte(endAddress))
            else
                0

        let mutable zc = 0uy
        let mutable stop = false
        while not stop do
            // Get next ZSCII character
            endAddress <- endAddress + 1

            zc <- if memory.Version >= 5 && endAddress = textBuffer + length + 2 then 0uy
                  else memory.ReadByte(endAddress)

            // Is this a word separator?
            let wordSeparatorIndex = wordSeparators |> Array.tryFindIndex ((=) zc)

            // If it's not a word separator, a space or the end of the text buffer (0), carry on.
            if wordSeparatorIndex.IsNone && zc <> 0x20uy && zc <> 0uy then
                // Is this the start of a word?
                if startAddress = 0 then
                    startAddress <- endAddress
            else if startAddress <> 0 then
                tokenizeWord (startAddress - textBuffer) (endAddress - startAddress)
                startAddress <- 0

            // Translate separator (which is a word in its own right)
            if wordSeparatorIndex.IsSome then
                tokenizeWord (endAddress - textBuffer) 1

            if zc = 0uy then
                stop <- true

