type Input = number[];
type Result = number;

export function parseInput(rawInput: string): Input {
  return rawInput.split("\n").map(Number);
}

export function runPartOne(input: Input): Result {
  return input.reduce((prev, cur) => prev + cur);
}

export function runPartTwo(input: Input): Result {
  return input.reduce((prev, cur) => prev * cur);
}
