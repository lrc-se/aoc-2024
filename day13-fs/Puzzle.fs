module Puzzle

open System.Text.RegularExpressions

type Offset = { X: int64; Y: int64 }
type ClawMachine = { ButtonA: Offset; ButtonB: Offset; Prize: Offset }
type private Pushes = { ButtonA: int64; ButtonB: int64 }

let private buttonRegex = Regex(@"X\+(\d+), Y\+(\d+)")
let private prizeRegex = Regex(@"X=(\d+), Y=(\d+)")

let private createMachine (lines: string[]): ClawMachine =
    let buttonA = buttonRegex.Match(lines[0])
    let buttonB = buttonRegex.Match(lines[1])
    let prize = prizeRegex.Match(lines[2])
    { ButtonA = { X = int64 buttonA.Groups[1].Value; Y = int64 buttonA.Groups[2].Value }
      ButtonB = { X = int64 buttonB.Groups[1].Value; Y = int64 buttonB.Groups[2].Value }
      Prize = { X = int64 prize.Groups[1].Value; Y = int64 prize.Groups[2].Value } }

let private getPushes (machine: ClawMachine) =
    let buttonA = float (machine.Prize.Y * machine.ButtonB.X - machine.Prize.X * machine.ButtonB.Y) / float (machine.ButtonA.Y * machine.ButtonB.X - machine.ButtonB.Y * machine.ButtonA.X)
    if buttonA > floor buttonA then
        None
    else
        let buttonB = float (machine.Prize.X - int64 buttonA * machine.ButtonA.X) / float machine.ButtonB.X
        if buttonB > floor buttonB then
            None
        else
            Some({ ButtonA = int64 buttonA; ButtonB = int64 buttonB })

let private getTokens (pushes: Pushes list) = pushes |> List.sumBy (fun push -> push.ButtonA * 3L + push.ButtonB)

let parseInput (rawInput: string) =
    let rec loop (sections: string list) (machines: ClawMachine list) =
        match sections with
        | [] -> machines
        | head :: tail -> loop tail ((head.Split('\n') |> createMachine) :: machines)

    loop (rawInput.Split("\n\n") |> Array.toList) []

let runPartOne (input: ClawMachine list) =
    input
    |> List.map getPushes
    |> List.choose (function
        | Some(push) when push.ButtonA <= 100 && push.ButtonB <= 100 -> Some(push)
        | _ -> None)
    |> getTokens

let runPartTwo (input: ClawMachine list) =
    input
    |> List.map (fun machine -> getPushes { machine with Prize = { X = machine.Prize.X + 10000000000000L; Y = machine.Prize.Y + 10000000000000L } })
    |> List.choose id
    |> getTokens
