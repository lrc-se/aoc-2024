module Puzzle

type Equation = { testValue: int64; numbers: int64 list }
type private Operation = int64 -> int64 -> int64

let private createEquation (line: string) =
    let parts = line.Split(": ")
    { testValue = int64 parts[0]; numbers = parts[1].Split(' ') |> Array.map int64 |> Array.toList }

let private canBeTrue (operations: Operation list) (equation: Equation) =
    let rec loop (numbers: int64 list) (curValue: int64) =
        if numbers.IsEmpty then
            curValue = equation.testValue
        else
            operations |> List.exists (fun operation -> loop numbers.Tail (operation curValue numbers.Head))

    loop equation.numbers.Tail equation.numbers.Head

let private calibrationResult (operations: Operation list) (equations: Equation[]) =
    equations
    |> Array.filter (canBeTrue operations)
    |> Array.sumBy _.testValue

let private concat number1 number2 = int64 $"{number1}{number2}"

let parseInput (rawInput: string) = rawInput.Split('\n') |> Array.map createEquation

let runPartOne (input: Equation[]) = input |> calibrationResult [(+); (*)]

let runPartTwo (input: Equation[]) = input |> calibrationResult [(+); (*); concat]
