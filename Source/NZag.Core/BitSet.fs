namespace NZag.Utilities

type IBitSet =
    abstract member Add : bit:int -> unit
    abstract member Remove : bit:int -> unit
    abstract member Clear : unit -> unit

    abstract member Contains : bit:int -> bool

    abstract member Item : index:int -> bool with get, set

    abstract member Length : int

module BitSet =

    let inline private mask32 bit =
        if bit < 0 || bit > 31 then argOutOfRange "bit" "bit must be in the range 0 to 31."
        1ul <<< bit

    let inline private mask64 bit =
        if bit < 0 || bit > 63 then argOutOfRange "bit" "bit must be in the range 0 to 63."
        1UL <<< bit

    type private BitSet32(length: int) =
        let mutable value = 0ul

        let add bit = value <- value ||| (mask32 bit)
        let remove bit = value <- value &&& ~~~(mask32 bit)
        let clear() = value <- 0ul
        let contains bit = (value &&& (mask32 bit)) <> 0ul

        interface IBitSet with
            member x.Add bit = add bit
            member x.Remove bit = remove bit
            member x.Clear() = clear()
            member x.Contains bit = contains bit

            member x.Item
                with get index = contains index
                 and set index value =
                    if value then add index
                    else remove index

            member x.Length = length

    type private BitSet64(length: int) =
        let mutable value = 0UL

        let add bit = value <- value ||| (mask64 bit)
        let remove bit = value <- value &&& ~~~(mask64 bit)
        let clear() = value <- 0UL
        let contains bit = (value &&& (mask64 bit)) <> 0UL

        interface IBitSet with
            member x.Add bit = add bit
            member x.Remove bit = remove bit
            member x.Clear() = clear()
            member x.Contains bit = contains bit

            member x.Item
                with get index = contains index
                 and set index value =
                    if value then add index
                    else remove index

            member x.Length = length

    type private BitSetN(length: int) =
        let value = Array.zeroCreate (length / 64)

        let add bit =
            let index = bit / 64
            let bit = bit % 64
            value.[index] <- value.[index] ||| (mask64 bit)
        let remove bit =
            let index = bit / 64
            let bit = bit % 64
            value.[index] <- value.[index] &&& ~~~(mask64 bit)
        let clear() =
            for i = 0 to (length / 64) - 1 do
                value.[i] <- 0UL
        let contains bit =
            let index = bit / 64
            let bit = bit % 64
            (value.[index] &&& (mask64 bit)) <> 0UL

        interface IBitSet with
            member x.Add bit = add bit
            member x.Remove bit = remove bit
            member x.Clear() = clear()
            member x.Contains bit = contains bit

            member x.Item
                with get index = contains index
                 and set index value =
                    if value then add index
                    else remove index

            member x.Length = length

    [<CompiledName("Create")>]
    let create length =
        if length <= 0 then argOutOfRange "length" "length must be greater than zero."

        if length <= 32 then
            new BitSet32(length) :> IBitSet
        elif length <= 64 then
            new BitSet64(length) :> IBitSet
        else
            new BitSetN(length) :> IBitSet
