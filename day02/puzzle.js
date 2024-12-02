function isSafe(report) {
  const sign = Math.sign(report[1] - report[0]);
  for (let i = 1; i < report.length; ++i) {
    const diff = report[i] - report[i - 1];
    if (Math.sign(diff) !== sign || Math.abs(diff) < 1 || Math.abs(diff) > 3) {
      return false;
    }
  }
  return true;
}

export function parseInput(rawInput) {
  return rawInput.split("\n").map(line => line.split(" ").map(Number));
}

export function runPartOne(input) {
  return input.reduce((count, report) => count + Number(isSafe(report)), 0);
}

export function runPartTwo(input) {
  let result = 0;
  for (const report of input) {
    for (let i = 0; i < report.length; ++i) {
      if (isSafe([...report.slice(0, i), ...report.slice(i + 1)])) {
        ++result;
        break;
      }
    }
  }
  return result;
}
