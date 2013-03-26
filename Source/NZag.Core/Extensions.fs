namespace NZag.Extensions

open System.IO

[<AutoOpen>]
module Extensions =

    type Stream with
        member x.NextByte() =
            match x.ReadByte() with
            | -1 -> None
            |  b -> Some(byte b)

        member x.NextWord() =
            match x.NextByte(), x.NextByte() with
            | Some(b1), Some(b2) -> Some((uint16 b1 <<< 8) ||| uint16 b2)
            | _ -> None

        member x.ReadBytes length =
            let buffer = Array.zeroCreate length

            let mutable offset = 0
            let mutable remaining = length

            while remaining > 0 do
                let read = x.Read(buffer, offset, remaining)
                if read <= 0 then
                    raise <| EndOfStreamException(sprintf "End of stream reached with %d bytes left to read." remaining)

                remaining <- remaining - read
                offset <- offset + read

            buffer

