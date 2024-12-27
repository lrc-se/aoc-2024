module Puzzle

type Equation = { TestValue: int64; Numbers: int64 list }
type private Operation = int64 -> int64 -> int64

let private createEquation (line: string) =
    let parts = line.Split(": ")
    { TestValue = int64 parts[0]; Numbers = parts[1].Split(' ') |> Array.map int64 |> Array.toList }

let private canBeTrue (operations: Operation[]) (equation: Equation) =
    let rec loop (numbers: int64 list) (value: int64) =
        match numbers with
        | [] -> value = equation.TestValue
        | curValue :: nextValues -> operations |> Array.exists (fun operation -> loop nextValues (operation value curValue))

    loop equation.Numbers.Tail equation.Numbers.Head

let private calibrationResult (operations: Operation[]) (equations: Equation[]) =
    equations
    |> Array.filter (canBeTrue operations)
    |> Array.sumBy _.TestValue

let private concat number1 number2 = int64 $"{number1}{number2}"

let parseInput (rawInput: string) = rawInput.Split('\n') |> Array.map createEquation

let runPartOne (input: Equation[]) = input |> calibrationResult [| (+); (*) |]

let runPartTwo (input: Equation[]) = input |> calibrationResult [| (+); (*); concat |]
