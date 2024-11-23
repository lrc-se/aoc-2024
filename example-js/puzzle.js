export function parseInput(rawInput) {
  return rawInput.split("\n").map(Number);
}

export function runPartOne(input) {
  return input.reduce((prev, cur) => prev + cur);
}

export function runPartTwo(input) {
  return input.reduce((prev, cur) => prev * cur);
}
