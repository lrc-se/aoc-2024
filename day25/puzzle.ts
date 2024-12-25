interface Input {
  locks: Uint8Array[];
  keys: Uint8Array[];
}

export function parseInput(rawInput: string): Input {
  const locks: Uint8Array[] = [];
  const keys: Uint8Array[] = [];
  const sections = rawInput.split("\n\n");
  for (const section of sections) {
    const lines = section.split("\n");
    const columns = new Uint8Array(5);
    for (let row = 1; row <= 5; ++row) {
      for (let col = 0; col < 5; ++col) {
        if (lines[row][col] === "#") {
          ++columns[col];
        }
      }
    }
    if (lines[0][0] === "#") {
      locks.push(columns);
    } else {
      keys.push(columns);
    }
  }
  return { locks, keys };
}

export function runPartOne(input: Input) {
  let result = 0;
  for (const lock of input.locks) {
    for (const key of input.keys) {
      let found = true;
      for (let i = 0; i < 5; ++i) {
        if (lock[i] + key[i] > 5) {
          found = false;
          break;
        }
      }
      if (found) {
        ++result;
      }
    }
  }
  return result;
}

export function runPartTwo() {
  return 2024;
}
