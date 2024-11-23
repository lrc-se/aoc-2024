open System
open Puzzle

let runner =
    match Environment.GetEnvironmentVariable("part") with
    | "part1" -> Ok(runPartOne)
    | "part2" -> Ok(runPartTwo)
    | part -> Error(part)

match runner with
| Ok(runPuzzle) ->
    printfn "Result: %s" (
        IO.File.ReadAllText(Environment.GetCommandLineArgs()[1]).TrimEnd()
        |> parseInput
        |> runPuzzle
        |> string
    )
| Error(part) -> printfn "Unknown part: '%s'" part
