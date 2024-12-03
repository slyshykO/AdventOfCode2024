// For more information see https://aka.ms/fsharp-console-apps
open System
open System.Diagnostics
open System.Text.RegularExpressions 
open Utils

Say.printGreen("\n\n\nAdvent of code 2024 Day 3\n")

let testString = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))"
let testAnswer = 161 // (2*4 + 5*5 + 11*8 + 8*5)

let regex = new Regex(@"mul\((\d+),(\d+)\)")

let testMatches = regex.Matches(testString)
for ``match`` in testMatches do
    let a = int ``match``.Groups.[1].Value
    let b = int ``match``.Groups.[2].Value
    Say.print($"{a,3}*{b,3}\n")

let testResult = testMatches |> Seq.map (fun ``match`` -> int ``match``.Groups.[1].Value * int ``match``.Groups.[2].Value) |> Seq.sum
Say.print($"Test result: {testResult} (expected {testAnswer}). ")
if testResult <> testAnswer then 
    Say.printRed("Fail\n")
else
    Say.printGreen("Ok\n")

let testString2 = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))"
let testAnswer2 = 48
//let regex2 = new Regex(@"(don\'t\(\).*?|do\(\).*?)?(mul\((\d+),(\d+)\))")
let regex2 = new Regex(@"do\(\)|don\'t\(\)|mul\((\d+),(\d+)\)")
let testMatches2 = regex2.Matches(testString2)
Say.print("--\n")
for ``match`` in testMatches2 do
    ``match``.Groups |> Seq.iteri (fun i m -> Say.print($"#{i}:`{m.Value,3}` ") )
    Say.print"\n"

testMatches2 |> Seq.fold (fun acc m -> 
    let b = fst acc
    let sum = snd acc
    let newSum = 
        if (m.Groups.[0].Value.StartsWith("mul(") && b) then
            let add0 = int m.Groups.[1].Value
            let add1 = int m.Groups.[2].Value
            sum + add0 * add1
        else
            sum

    let newB = 
        if m.Groups.[0].Value.StartsWith("don't()") then 
            false 
        elif m.Groups.[0].Value.StartsWith("do()") then
            true 
        else 
            b
    (newB, newSum)
    
) (true, 0) 
|> fun (_, result) -> Say.print($"Test result 2: {result} (expected {testAnswer2}). ")



// Real data
Say.setVerbosity 0
printf "\n---\n\n"
let allocated = GC.GetAllocatedBytesForCurrentThread()
let stopWatch = Stopwatch.StartNew()

let input = System.IO.File.ReadAllText("input.txt")
let matches = regex.Matches(input)
let result = matches |> Seq.map (fun ``match`` -> int ``match``.Groups.[1].Value * int ``match``.Groups.[2].Value) |> Seq.sum
Say.printBlueForce($"Result Part one: {result}\n")

let matches2 = regex2.Matches(input)
let result2 = 
    matches2 |> Seq.fold (fun acc m -> 
    let b = fst acc
    let sum = snd acc
    let newSum = 
        if (m.Groups.[0].Value.StartsWith("mul(") && b) then
            let add0 = int m.Groups.[1].Value
            let add1 = int m.Groups.[2].Value
            sum + add0 * add1
        else
            sum

    let newB = 
        if m.Groups.[0].Value.StartsWith("don't()") then 
            false 
        elif m.Groups.[0].Value.StartsWith("do()") then
            true 
        else 
            b
    (newB, newSum)
    
    ) (true, 0)
    |> fun (_, result) -> result
Say.printBlueForce($"Result Part two: {result2}\n")


stopWatch.Stop()
printfn $"\n\nDay 2:"
printfn $"    Execution time: {stopWatch.Elapsed.TotalMilliseconds} ms"
printfn $"    Memory used: {(GC.GetAllocatedBytesForCurrentThread() - allocated)} B"