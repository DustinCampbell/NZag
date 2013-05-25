namespace NZag.Utilities

open System
open System.Collections.Generic
open System.IO
open System.Text
open System.Threading.Tasks

[<AutoOpen>]
module Patterns =

    let (|Any|_|) value = Some()
    let (|LessThan|_|) upper value = if value < upper then Some() else None
    let (|AtMost|_|) upper value = if value <= upper then Some() else None
    let (|GreaterThan|_|) lower value = if value > lower then Some() else None
    let (|AtLeast|_|) lower value = if value >= lower then Some() else None
    let (|Is|_|) comparand value = if value = comparand then Some() else None

[<AutoOpen>]
module Exceptions =

    type CompilerException(message: string) =
        inherit Exception(message)

    type RuntimeException(message: string) =
        inherit Exception(message)

    type ZMachineQuitException() =
        inherit Exception()

    type ContractException(message: string) =
        inherit Exception(message)

    let failcompile message =
        raise <| CompilerException(message)

    let failcompilef format =
        Printf.ksprintf (fun s -> failcompile s) format

    let failruntime message =
        raise <| RuntimeException(message)

    let failruntimef format =
        Printf.ksprintf (fun s -> failruntime s) format

    let argNull name format =
        Printf.ksprintf (fun s -> raise <| ArgumentNullException(name, s)) format

    let argOutOfRange name format =
        Printf.ksprintf (fun s -> raise <| ArgumentOutOfRangeException(name, s)) format

[<AutoOpen>]
module Async =

    let awaitTask (task: Task) =
        let continuation (t: Task) : unit =
            match t.IsFaulted with
            | true -> raise t.Exception
            | arg -> ()

        task.ContinueWith(continuation, TaskContinuationOptions.ExecuteSynchronously) |> Async.AwaitTask

    let startAsTask (work: Async<unit>) =
        Task.Factory.StartNew(fun () -> work |> Async.RunSynchronously)

[<RequireQualifiedAccess>]
module String =

    let replace (oldValue : string) (newValue : string) (s : string) =
        s.Replace(oldValue, newValue)

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

module Array =

    let clear arr =
        Array.Clear(arr, 0, arr |> Array.length)

module Collection =

    let length (c : ICollection<_>) = c.Count

    let toArray (c : ICollection<_>) =
        let res = Array.zeroCreate (length c)
        let mutable i = 0
        for v in c do
            res.[i] <- v
            i <- i + 1
        res

module Dictionary =

    let create() = new Dictionary<_,_>() :> IDictionary<_,_>

    let createFrom (keyValues: seq<_*_>) =
        let d = new Dictionary<_,_>()
        for (k,v) in keyValues do
            d.[k] <- v
        d :> IDictionary<_,_>

    let length (d: IDictionary<_,_>) = d.Count

    let contains key (d: IDictionary<_,_>) =
        d.ContainsKey(key)

    let find key (d: IDictionary<_,_>) =
        d.[key]

    let tryFind key (d: IDictionary<_,_>) =
        match d.TryGetValue(key) with
        | (true, v) -> Some(v)
        | (false,_) -> None

    let add k v (d: IDictionary<_,_>) =
        d.Add(k, v)

    let getOrAdd k f (d: IDictionary<_,_>) =
        match d |> tryFind k with
        | Some(v) -> v
        | None ->
            let v = f()
            d.Add(k, v)
            v

    let toList (d: IDictionary<_,_>) =
        let mutable res = []

        let keys = d.Keys |> Collection.toArray
        for i = length d - 1 downto 0 do
            let key = keys.[i]
            res <- (key, d.[key]) :: res
        res

    let toMap (d: IDictionary<_,_>) =
        let mutable map = Map.empty

        for kvp in d do
            map <- map |> Map.add kvp.Key kvp.Value

        map

module SortedList =

    let create() =
        new SortedList<_,_>() :> IDictionary<_,_>

    let createWithCompare compare =
        let comparer = { new IComparer<'T> with
            member this.Compare(x, y) =
                compare x y }

        new SortedList<_,_>(comparer) :> IDictionary<_,_>

module ResizeArray =

    let create() = new ResizeArray<_>()
    let createFrom (e: seq<_>) = new ResizeArray<_>(e)

    let length (arr: ResizeArray<_>) = arr.Count

    let add v (arr: ResizeArray<_>) =
        arr.Add(v) |> ignore

    let toList (arr: ResizeArray<_>) =
        let mutable res = []
        for i = length arr - 1 downto 0 do
            res <- arr.[i] :: res
        res

module HashSet =

    let create() = new HashSet<_>()
    let createFrom (e: seq<_>) = new HashSet<_>(e)

    let unionWith (e: seq<_>) (set: HashSet<_>) =
        set.UnionWith(e)

    let equals (e: seq<_>) (set: HashSet<_>) =
        set.SetEquals(e)

    let add v (set: HashSet<_>) =
        set.Add(v) |> ignore

    let remove v (set: HashSet<_>) =
        set.Remove(v) |> ignore

    let removeWhere f (set: HashSet<_>) =
        set.RemoveWhere(Predicate(f)) |> ignore

    let toList (set: HashSet<_>) =
        set |> List.ofSeq

module SortedSet =

    let create() =
        new SortedSet<_>()
    let length (s : SortedSet<_>) = s.Count

    let add v (s : SortedSet<_>) =
        s.Add(v) |> ignore

    let toList (s : SortedSet<_>) =
        s |> List.ofSeq

module Stack =

    let create() = new Stack<_>()

    let length (s: Stack<_>) = s.Count
    let isEmpty (s: Stack<_>) = s.Count = 0

    let push v (s: Stack<_>) = s.Push(v)
    let pop (s: Stack<_>) = s.Pop()

[<AutoOpen>]
module Functions =

    let memoize f =
        let map = Dictionary.create()
        fun k ->
            match map |> Dictionary.tryFind k with
            | Some(v) -> v
            | None -> let v = f k
                      map.[k] <- v
                      v

    let fixedpoint f v =
        let mutable current = v
        let mutable stop = false

        while not stop do
            let result = f current

            if current <> result then
                current <- result
            else
                stop <- true

        current

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
