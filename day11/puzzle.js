function getStoneCounts(numbers) {
  const counts = {};
  for (const number of numbers) {
    counts[number] = (counts[number] ?? 0) + 1;
  }
  return counts;
}

function blink(stoneCounts, blinkCount) {
  for (let i = 0; i < blinkCount; ++i) {
    for (const [number, count] of Object.entries(stoneCounts)) {
      if (count === 0) {
        continue;
      } else if (number === "0") {
        stoneCounts["0"] -= count;
        stoneCounts["1"] = (stoneCounts["1"] ?? 0) + count;
      } else if (number.length % 2 === 0) {
        const half = number.length >> 1;
        const firstStone = String(+number.substring(0, half));
        const secondStone = String(+number.substring(half));
        stoneCounts[number] -= count;
        stoneCounts[firstStone] = (stoneCounts[firstStone] ?? 0) + count;
        stoneCounts[secondStone] = (stoneCounts[secondStone] ?? 0) + count;
      } else {
        const stone = String(+number * 2024);
        stoneCounts[number] -= count;
        stoneCounts[stone] = (stoneCounts[stone] ?? 0) + count;
      }
    }
  }
  return Object.values(stoneCounts).reduce((sum, count) => sum + count, 0);
}

export function parseInput(rawInput) {
  return rawInput.split(" ");
}

export function runPartOne(input) {
  return blink(getStoneCounts(input), 25);
}

export function runPartTwo(input) {
  return blink(getStoneCounts(input), 75);
}
