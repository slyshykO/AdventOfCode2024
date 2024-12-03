open System
open System.Diagnostics
open Utils

let day = 
    Environment.GetCommandLineArgs() 
    |> Array.take 1 
    |> (fun x -> System.IO.Path.GetFileNameWithoutExtension(x.[0]))
    |> (fun x -> x.Replace("-"," "))

Say.printGreen($"\n\n\nAdvent of code 2024 {day}\n\n")

// *** Tests ***


// *** Problem ***
Say.setVerbosity 0
printf "\n---\n\n"
let allocated = GC.GetAllocatedBytesForCurrentThread()
let stopWatch = Stopwatch.StartNew()




stopWatch.Stop()
Say.printForce $"\n\n{day}: \n"
Say.printForce $"    Execution time: {stopWatch.Elapsed.TotalMilliseconds} ms\n"
Say.printForce $"    Memory used: {(GC.GetAllocatedBytesForCurrentThread() - allocated)} B\n"