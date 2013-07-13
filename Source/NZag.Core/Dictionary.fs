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

    let private tokenizeWord textBuffer parseBuffer start length dictionary flag (memory: Memory) =
        let mutable address = parseBuffer

        let tokenMax = memory.ReadByte(address)
        address <- address + 1
        let tokenCount = memory.ReadByte(address)

        if tokenCount < tokenMax then
            memory.WriteByte(address, tokenCount + 1uy)
            address <- parseBuffer + 1

        let wordChars = Array.zeroCreate length
        for i = 0 to length - 1 do
            wordChars.[i] <- char (memory.ReadByte(textBuffer + start + i))

        let word = String.fromCharArray wordChars

        let wordAddress = uint16 (memory |> lookupWord word dictionary)

        if wordAddress <> 0us || not flag then
            address <- address + int (tokenCount * 4uy)

            memory.WriteWord(address, wordAddress)
            memory.WriteByte(address + 2, byte length)
            memory.WriteByte(address + 3, byte start)

    let tokenizeLine textBuffer parseBuffer dictionary flag (memory: Memory) =
        // User standard dictionary if none is provided.
        let dictionary = if dictionary = 0 then int (memory |> Header.readDictionaryAddress)
                         else dictionary

        // Set number of parse tokens to zero.
        memory.WriteByte(parseBuffer + 1, 0uy)

        let mutable address1 = textBuffer
        let mutable address2 = 0

        let length =
            if memory.Version >= 5 then
                address1 <- address1 + 1
                int (memory.ReadByte(address1))
            else
                0

        let mutable zc = 0uy
        let mutable stop = false
        while not stop do
            // Get next ZSCII character
            address1 <- address1 + 1

            if memory.Version >= 5 && address1 = textBuffer + length + 2 then
                zc <- 0uy
            else
                zc <- memory.ReadByte(address1)

            // Check for separator
            let mutable separatorAddress = dictionary
            let mutable separatorCount = memory.ReadByte(separatorAddress)
            separatorAddress <- separatorAddress + 1

            let mutable stopSeparatorSearch = false
            while not stopSeparatorSearch do
                let separator = memory.ReadByte(separatorAddress)
                separatorAddress <- separatorAddress + 1

                if zc = separator then
                    stopSeparatorSearch <- true
                else
                    separatorCount <- separatorCount - 1uy
                    if separatorCount = 0uy then
                        stopSeparatorSearch <- true

            // This could be the start or end of a word
            if separatorCount = 0uy && zc <> 0x20uy && zc <> 0uy then
                if address2 = 0 then
                    address2 <- address1
            else if address2 <> 0 then
                memory |> tokenizeWord textBuffer (address2 - textBuffer) (address1 - address2) parseBuffer dictionary flag
                address2 <- 0

            // Translate separator (which is a word in its own right)
            if separatorCount <> 0uy then
                memory |> tokenizeWord textBuffer (address1 - textBuffer) 1 parseBuffer dictionary flag

            if zc = 0uy then
                stop <- true

