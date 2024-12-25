module Puzzle

type private Heights = int list
type Input = { Locks: Heights list; Keys: Heights list }

let private createObject (lines: string[]) =
    let startRow, rowDelta =
        match lines[0][0] with
        | '#' -> 1, 1
        | _ -> 5, -1

    let rec loopCols col (heights: Heights) =
        let rec loopRows row height =
            match lines[row][col] with
            | '#' -> loopRows (row + rowDelta) (height + 1)
            | _ -> height

        match col with
        | value when value < 5 -> loopCols (col + 1) ((loopRows startRow 0) :: heights)
        | _ -> heights

    loopCols 0 []

let parseInput (rawInput: string) =
    let rec loop (sections: string[] list) (locks: Heights list) (keys: Heights list) =
        match sections with
        | [] -> locks, keys
        | section :: nextSections ->
            match section[0][0] with
            | '#' -> loop nextSections (createObject section :: locks) keys
            | _ -> loop nextSections locks (createObject section :: keys)

    let sections =
        rawInput.Split("\n\n")
        |> Array.map _.Split('\n')
        |> Array.toList

    let locks, keys = loop sections [] []
    { Locks = locks; Keys = keys }

let runPartOne (input: Input) =
    List.allPairs input.Locks input.Keys
    |> List.filter (fun pair ->
        List.zip (fst pair) (snd pair)
        |> List.forall (fun cols -> (fst cols) + (snd cols) <= 5))
    |> List.length

let runPartTwo _ = 2024
