namespace NZag.Utilities

open System
open System.Collections.Generic
open System.IO
open System.Text

[<AutoOpen>]
module Exceptions =

    let invalidOperation message =
        Printf.ksprintf (fun s -> raise <| InvalidOperationException(s)) message

    let argumentOutOfRange paramName message =
        Printf.ksprintf (fun s -> raise <| ArgumentOutOfRangeException(paramName, s)) message

    let argumentNull paramName message =
        Printf.ksprintf (fun s -> raise <| ArgumentNullException(paramName, s)) message

[<RequireQualifiedAccess>]
module String =

    let toCharArray (s : string) =
        s.ToCharArray()

[<RequireQualifiedAccess>]
module StringBuilder =

    let create() =
        new StringBuilder()

    let appendChar (ch : char) (builder : StringBuilder) =
        builder.Append(ch) |> ignore

    let appendFormat format (builder : StringBuilder) =
        Printf.ksprintf (fun s -> builder.Append(s) |> ignore) format

    let appendString (s : string) (builder : StringBuilder) =
        builder.Append(s) |> ignore

    let appendLineBreak (builder : StringBuilder) =
        builder.AppendLine() |> ignore

[<RequireQualifiedAccess>]
module Enumerable =

    let getEnumerator (e : seq<_>) =
        e.GetEnumerator()

[<RequireQualifiedAccess>]
module Enumerator =

    let next (e : IEnumerator<_>) =
        if e.MoveNext() then Some(e.Current)
        else None

[<RequireQualifiedAccess>]
module Dictionary =

    let create() =
        new Dictionary<_,_>() :> IDictionary<_,_>

    let tryGetValue key (d : IDictionary<_,_>) =
        match d.TryGetValue(key) with
        | (true, v) -> Some(v)
        | (false,_) -> None

[<AutoOpen>]
module Functions =

    let memoize f =
        let map = Dictionary.create()
        fun k ->
            match map |> Dictionary.tryGetValue k with
            | Some(v) -> v
            | None -> let v = f k
                      map.[k] <- v
                      v

[<AutoOpen>]
module Extensions =

    type IEnumerator<'a> with
        member x.Next() =
            Enumerator.next x

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

