namespace NZag.Utilities

open System

type IBitSet =
    abstract member Add : bit:int -> unit
    abstract member Remove : bit:int -> unit
    abstract member RemoveWhere : predicate:Func<int,bool,bool> -> unit
    abstract member Clear : unit -> unit

    abstract member Contains : bit:int -> bool

    abstract member OrWith : other:IBitSet -> unit

    abstract member Item : index:int -> bool with get, set

    abstract member Length : int

module BitSet =

    let inline private validateBit bit length =
        if bit < 0 || bit >= length then
            argOutOfRange "bit" "bit must be in the range 0 to %d." length

    let inline private validateBitSetLength (bitSet: IBitSet) length =
        if length <> bitSet.Length then
            argOutOfRange "other" "Bit sets must have the same length. Expected %d but was %d" length bitSet.Length

    let inline private mask32 bit = 1ul <<< bit
    let inline private mask64 bit = 1UL <<< bit

    type private IBitSet32 =
        abstract member UnderlyingValue : uint32

    type private BitSet32(length: int) =
        [<Literal>]
        let zero = 0ul

        let mutable value = zero

        let add bit =
            validateBit bit length
            value <- value ||| (mask32 bit)

        let remove bit =
            validateBit bit length
            value <- value &&& ~~~(mask32 bit)

        let clear() =
            value <- zero

        let contains bit =
            validateBit bit length
            (value &&& (mask32 bit)) <> zero

        let removeWhere (predicate: Func<int,bool,bool>) =
            for i = 0 to length - 1 do
                if predicate.Invoke(i, contains i) then
                    remove i

        let orWith other =
            validateBitSetLength other length
            value <- value ||| (box other :?> IBitSet32).UnderlyingValue

        interface IBitSet32 with
            member x.UnderlyingValue = value

        interface IBitSet with
            member x.Add bit = add bit
            member x.Remove bit = remove bit
            member x.RemoveWhere predicate = removeWhere predicate
            member x.Clear() = clear()
            member x.Contains bit = contains bit
            member x.OrWith other = orWith other

            member x.Item
                with get index = contains index
                 and set index value =
                    if value then add index
                    else remove index

            member x.Length = length

    type private IBitSet64 =
        abstract member UnderlyingValue : uint64

    type private BitSet64(length: int) =
        [<Literal>]
        let zero = 0UL

        let mutable value = zero

        let add bit =
            validateBit bit length
            value <- value ||| (mask64 bit)

        let remove bit =
            validateBit bit length
            value <- value &&& ~~~(mask64 bit)

        let clear() =
            value <- zero

        let contains bit =
            validateBit bit length
            (value &&& (mask64 bit)) <> zero

        let removeWhere (predicate: Func<int,bool,bool>) =
            for i = 0 to length - 1 do
                if predicate.Invoke(i, contains i) then
                    remove i

        let orWith other =
            validateBitSetLength other length
            value <- value ||| (box other :?> IBitSet64).UnderlyingValue

        interface IBitSet64 with
            member x.UnderlyingValue = value

        interface IBitSet with
            member x.Add bit = add bit
            member x.Remove bit = remove bit
            member x.RemoveWhere predicate = removeWhere predicate
            member x.Clear() = clear()
            member x.Contains bit = contains bit
            member x.OrWith other = orWith other

            member x.Item
                with get index = contains index
                 and set index value =
                    if value then add index
                    else remove index

            member x.Length = length

    type private IBitSetN =
        abstract member UnderlyingValue : uint64[]

    type private BitSetN(length: int) =
        [<Literal>]
        let zero = 0UL
        [<Literal>]
        let resolution = 64

        let byteCount = length / resolution
        let value = Array.zeroCreate byteCount

        let add bit =
            validateBit bit length
            let index = bit / resolution
            let bit = bit % resolution
            value.[index] <- value.[index] ||| (mask64 bit)

        let remove bit =
            validateBit bit length
            let index = bit / resolution
            let bit = bit % resolution
            value.[index] <- value.[index] &&& ~~~(mask64 bit)

        let clear() =
            Array.clear value

        let contains bit =
            validateBit bit length
            let index = bit / resolution
            let bit = bit % resolution
            (value.[index] &&& (mask64 bit)) <> zero

        let removeWhere (predicate: Func<int,bool,bool>) =
            for i = 0 to length - 1 do
                if predicate.Invoke(i, contains i) then
                    remove i

        let orWith other =
            validateBitSetLength other length
            let otherValue = (box other :?> IBitSetN).UnderlyingValue
            for i = 0 to byteCount - 1 do
                value.[i] <- value.[i] ||| otherValue.[i]

        interface IBitSetN with
            member x.UnderlyingValue = value

        interface IBitSet with
            member x.Add bit = add bit
            member x.Remove bit = remove bit
            member x.RemoveWhere predicate = removeWhere predicate
            member x.Clear() = clear()
            member x.Contains bit = contains bit
            member x.OrWith other = orWith other

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
