// For more information see https://aka.ms/fsharp-console-apps
open System
open System.Diagnostics
open System.Numerics
open System.Runtime.Intrinsics

printfn "Advent of code 2024 Day 1"

[<Literal>]
let inputPath = "data-d1.txt"

let inline dataLineToTuple (line: string) =
    let parts =
        line.Split(' ')
        |> Array.filter (fun ff -> ff.Length > 0)
        |> Array.map (fun ss -> ss.Trim())

    (int parts.[0], int parts.[1])

let inline dataLineToTuple1 (line: string) = (int line[0..4], int line[8..])

let inline arraySum (array:^T[]) : ^T = 
    let mutable state = Vector< ^T>.Zero
    let count = Vector< ^T>.Count
        
    let mutable i = 0
    while i <= array.Length-count do
        state <-  state + Vector< ^T>(array,i)
        i <- i + count

    let mutable result = Unchecked.defaultof< ^T>
    i <- array.Length-array.Length%count
    while i < array.Length do
        result <- result + array.[i]
        i <- i + 1

    i <- 0
    while i < count do
        result <- result + state.[i]
        i <- i + 1
    result

let inline arraySumV512 (array:^T[]) : ^T = 
    let mutable state = Vector512< ^T>.Zero
    let count = Vector512< ^T>.Count
        
    let mutable i = 0
    while i <= array.Length-count do
        state <-  state + Vector512.Create (array,i)
        i <- i + count

    let mutable result = Unchecked.defaultof< ^T>
    i <- array.Length-array.Length%count
    while i < array.Length do
        result <- result + array.[i]
        i <- i + 1

    i <- 0
    while i < count do
        result <- result + state.[i]
        i <- i + 1
    result

let inline arrayMap (vf : ^T Vector -> ^U Vector) (sf : ^T -> ^U) (array : ^T[]) : ^U[] =
    let count = Vector< ^T>.Count
    if count <> Vector< ^U>.Count then invalidArg "array" "Output type must have the same width as input type."    
    
    let result = Array.zeroCreate array.Length
    
    let mutable i = 0
    while i <= array.Length-count do        
        (vf (Vector< ^T>(array,i ))).CopyTo(result,i)   
        i <- i + count
    
    i <- array.Length-array.Length%count
    while i < result.Length do
        result.[i] <- sf array.[i]
        i <- i + 1

    result

let allocated = GC.GetAllocatedBytesForCurrentThread()
let stopWatch = Stopwatch.StartNew()
let data =
    IO.File.ReadAllLines(inputPath)
    |> Array.map (fun x -> dataLineToTuple x)
    |> Array.unzip
    ||> (fun xx yy -> (Array.sort xx, Array.sort yy))

let inline magic_number_arr lhs rhs =
    (lhs, rhs) ||> Array.zip |> Array.map (fun (l, r) -> abs (l - r)) |> Array.sum

let magic_number_V512 (lhs:^T[]) (rhs:^T[]) : ^T =
    let count = Vector512< ^T>.Count
    let mutable state = Vector512< ^T>.Zero

    let inline cm (vl:Vector512< ^T>) (vr:Vector512< ^T>) : Vector512< ^T> = 
        Vector512.Abs (vl - vr)

    let mutable i = 0
    while i <= lhs.Length-count do
        let v1 = Vector512.Create(lhs,i)
        let v2 = Vector512.Create(rhs,i)
        state <- state + (cm v1 v2)
        i <- i + count

    let mutable result = Unchecked.defaultof< ^T>
    i <- lhs.Length-lhs.Length%count
    while i < lhs.Length do
        result <- result + abs(lhs.[i] - rhs.[i])
        i <- i + 1

    i <- 0
    while i < count do
        result <- result + state.[i]
        i <- i + 1

    result

let magic_number_V256 (lhs:^T array) (rhs:^T array) : ^T =
    let count = Vector256< ^T>.Count
    let mutable state = Vector256< ^T>.Zero

    let inline cm (vl:Vector256< ^T>) (vr:Vector256< ^T>) : Vector256< ^T> = 
        Vector256.Abs (vl - vr)

    let mutable i = 0
    while i <= lhs.Length-count do
        let v1 = Vector256.Create(lhs ,i)
        let v2 = Vector256.Create(rhs,i)
        state <- state + (cm v1 v2)
        i <- i + count

    let mutable result = Unchecked.defaultof< ^T>
    i <- lhs.Length-lhs.Length%count
    while i < lhs.Length do
        result <- result + abs(lhs.[i] - rhs.[i])
        i <- i + 1

    i <- 0
    while i < count do
        result <- result + state.[i]
        i <- i + 1

    result

let magic_number lhs rhs =
    magic_number_arr lhs rhs

let magic_number_1 lhs rhs =
    magic_number_V256 lhs rhs

let magic_number_2 lhs rhs =
    magic_number_V512 lhs rhs

let similarity_score lhs rhs =

    let count arr x =
        arr |> Array.filter (fun y -> y = x) |> Array.length

    lhs |> Array.map (fun x -> (count rhs x) * x) |> arraySumV512

printfn $"Part 1: {(data ||> magic_number)}"
printfn $"Part 2: {(data ||> similarity_score)}"
stopWatch.Stop()
printfn $"Day 1:"
printfn $"    Execution time: {stopWatch.Elapsed.TotalMilliseconds} ms"
printfn $"    Memory used: {(GC.GetAllocatedBytesForCurrentThread() - allocated)} B"
