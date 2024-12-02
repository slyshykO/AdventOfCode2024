// For more information see https://aka.ms/fsharp-console-apps
open System
open System.Diagnostics
open System.Numerics.Tensors

printfn "Advent of code 2024 Day 2"

module Say = 
    let mutable level = 1
    let print (msg: string) = 
        if level > 0 then
            Console.Write(msg)

    let printColor (color: ConsoleColor) (msg: string) =
        if level > 0 then
            let oldColor = Console.ForegroundColor
            Console.ForegroundColor <- color
            Console.Write(msg)
            Console.ForegroundColor <- oldColor

    let printYellow (msg: string) = printColor ConsoleColor.Yellow msg

    let printRed (msg: string) = printColor ConsoleColor.Red msg

(*
7 6 4 2 1: Safe because the levels are all decreasing by 1 or 2.
1 2 7 8 9: Unsafe because 2 7 is an increase of 5.
9 7 6 2 1: Unsafe because 6 2 is a decrease of 4.
1 3 2 4 5: Unsafe because 1 3 is increasing but 3 2 is decreasing.
8 6 4 4 1: Unsafe because 4 4 is neither an increase or a decrease.
1 3 6 7 9: Safe because the levels are all increasing by 1, 2, or 3.
*)
let testData = [|
    [|7; 6; 4; 2; 1|]
    [|1; 2; 7; 8; 9|]
    [|9; 7; 6; 2; 1|]
    [|1; 3; 2; 4; 5|]
    [|8; 6; 4; 4; 1|]
    [|1; 3; 6; 7; 9|]
|]


let arrayToString (x: int array) = 
    x |> Array.fold (fun acc x -> $"{acc} {x}") ""

let printArray (x: int array) = 
    if Say.level > 0 then
        Console.Write("[")
        x |> Array.iter (fun x -> Console.Write($"{x} "))
        Console.WriteLine("]")

let problemIndexes (a : int array) : int array = 
    let lst =  new Collections.Generic.List<int>()
    let mutable prevDiff = 0
    for i in 0..a.Length-2 do
        let diff = a.[i] - a.[i+1]
        let isBigDiff = abs(diff) < 1 || abs(diff) > 3 
        let isSignChange = (prevDiff * diff) < 0
        if isBigDiff || isSignChange then
            lst.Add(i)
        prevDiff <- diff
    lst.ToArray()

let printArrayWithProblems(a : int array) (problems: int array) =
    Say.print "["
    for i in 0..a.Length-1 do
        if problems |> Array.contains i then
            Say.printRed($"{a.[i]} ")
        else
            Say.print($"{a.[i]} ")
    Say.print "]"
let canRemoveProblem (a : int array) (problems: int array) : bool = 
    if problems.Length = 0 then 
        true
    else
        problems |> Array.map (fun p -> 
            let remProblem = Array.removeAt p a
            let newProblems = problemIndexes remProblem
            printArrayWithProblems a problems
            Say.print "->"
            printArrayWithProblems remProblem newProblems
            $": {newProblems.Length} " |> Say.print
            newProblems.Length = 0)
            |> Array.filter (fun x -> x ) |> Array.length <> 0

let canRemoveProblem2 (a : int array) (problems: int array): bool = 
    if problems.Length = 0 then 
        true
    else
        [|0..a.Length - 1|] |> Array.map (fun p -> 
            let remProblem = Array.removeAt p a
            let newProblems = problemIndexes remProblem
            printArrayWithProblems a problems
            Say.print "->"
            printArrayWithProblems remProblem newProblems
            $": {newProblems.Length} " |> Say.print
            newProblems.Length = 0)
            |> Array.filter (fun x -> x ) |> Array.length <> 0


testData 
|> Array.map 
 (fun x -> 
    let mutable result = Array.create (x.Length-1) 0
    for i in 0..x.Length-2 do
        result.[i] <- (x.[i] - x.[i+1])
    result)
|> Array.filter 
 (fun x -> 
    x |> Array.filter (fun x -> abs (x) < 1 || abs (x) >3) |> Array.length = 0)
|> Array.filter 
 (fun x -> 
    let sum = x |> Array.sum 
    let abs_sum = x |> Array.map abs |> Array.sum
    printArray x
    $"Abs sum:{abs_sum} Sum:{sum}" |> Say.print
    abs_sum = abs(sum))
|> (fun x -> Say.print($"Test Safe: {x.Length}"))

printfn "\n---\n"

// 2
testData 
|> Array.mapi 
 (fun i x -> 
    let problems = problemIndexes x
    (i,x,problems))
|> Array.filter 
 (fun (i,x,problems) -> 
    $"#{i}| " |> Say.print
    let c =canRemoveProblem2 x problems
    $"| {c}\n" |> Say.print
    c)
|> (fun x -> Say.print($"Test Safe2: {x.Length}"))

// Real data
Say.level <- 0
printf "\n---\n\n"

[<Literal>]
let inputPath = "data-d2.txt"

let inline dataLineToArray (line: string) =
    let parts =
        line.Split(' ')
        |> Array.filter (fun ff -> ff.Length > 0)
        |> Array.map (fun ss -> ss.Trim())
        |> Array.map int

    parts


let allocated = GC.GetAllocatedBytesForCurrentThread()
let stopWatch = Stopwatch.StartNew()

let data = 
    IO.File.ReadAllLines inputPath
    |> Array.map dataLineToArray

data
|> Array.map 
 (fun x -> 
    let mutable result = Array.create (x.Length-1) 0
    for i in 0..x.Length-2 do
        result.[i] <- (x.[i] - x.[i+1])
    result)
|> Array.filter 
 (fun x -> 
    x |> Array.filter (fun x -> abs (x) < 1 || abs (x) >3) |> Array.length = 0)
|> Array.filter 
 (fun x -> 
    let sum = x |> Array.sum 
    let abs_sum = x |> Array.map abs |> Array.sum
    abs_sum = abs(sum))
|> (fun x -> Console.WriteLine($"Real Safe: {x.Length}"))

data 
|> Array.mapi 
 (fun i x -> 
    let problems = problemIndexes x
    (i,x,problems))
|> Array.filter 
 (fun (i,x,problems) -> 
    $"#{i}| " |> Say.print
    let c =canRemoveProblem2 x problems
    $"| {c}\n" |> Say.print
    c)
|> (fun x -> Console.WriteLine($"Restored safe: {x.Length}"))

stopWatch.Stop()
printfn $"Day 2:"
printfn $"    Execution time: {stopWatch.Elapsed.TotalMilliseconds} ms"
printfn $"    Memory used: {(GC.GetAllocatedBytesForCurrentThread() - allocated)} B"
