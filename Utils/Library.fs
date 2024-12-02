namespace Utils

open System 

module Say =
    let mutable private verbosity_ = 1

    let verbosity () = 
        verbosity_ 

    let setVerbosity (v: int) = 
        verbosity_ <- v


    let print (msg: string) = 
        if verbosity_ > 0 then
            Console.Write(msg)

    let printColor (color: ConsoleColor) (msg: string) =
        if verbosity_ > 0 then
            let oldColor = Console.ForegroundColor
            Console.ForegroundColor <- color
            Console.Write(msg)
            Console.ForegroundColor <- oldColor

    let printYellow (msg: string) = printColor ConsoleColor.Yellow msg

    let printRed (msg: string) = printColor ConsoleColor.Red msg

    let printGreen (msg: string) = printColor ConsoleColor.Green msg

    let printBlue (msg: string) = printColor ConsoleColor.Blue msg

module Data = 

    let lineToIntArrary (line: string) = 
        line.Split(' ')
        |> Array.filter (fun ff -> ff.Length > 0)
        |> Array.map (fun ss -> ss.Trim())
        |> Array.map int

