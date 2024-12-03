type Input = string;
type Result = number;

const re = /mul\((\d+),(\d+)\)/g;
const re2 = /mul\((\d+),(\d+)\)|do\(\)|don't\(\)/g;

export function parseInput(rawInput: string): Input {
  return rawInput;
}

export function runPartOne(input: Input): Result {
  return [...input.matchAll(re)].reduce((sum, match) => sum + Number(match[1]) * Number(match[2]), 0);
}

export function runPartTwo(input: Input): Result {
  let result = 0;
  let enabled = true;
  for (const match of input.matchAll(re2)) {
    if (match[0] === "do()") {
      enabled = true;
    } else if (match[0] === "don't()") {
      enabled = false;
    } else if (enabled) {
      result += Number(match[1]) * Number(match[2]);
    }
  }
  return result;
}
