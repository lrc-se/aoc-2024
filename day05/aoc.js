import { parseInput, runPartOne, runPartTwo } from "./puzzle";

const runPuzzle = { part1: runPartOne, part2: runPartTwo }[process.env.part];
if (runPuzzle) {
  (async () => console.log("Result:", runPuzzle(parseInput((await Bun.file(process.argv[2]).text()).trimEnd()))))();
} else {
  console.log(`Unknown part: '${process.env.part ?? ""}'`);
}
