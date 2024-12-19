function getTowelIndex(towels) {
  const index = {};
  for (const towel of towels) {
    let list = index[towel[0]];
    if (!list) {
      index[towel[0]] = list = [];
    }
    list.push(towel);
  }
  return index;
}

function isPossible(design, towelIndex) {
  for (const towel of towelIndex[design[0]] || []) {
    if (design.startsWith(towel) && (towel.length === design.length || isPossible(design.substring(towel.length), towelIndex))) {
      return true;
    }
  }
  return false;
}

function countPossible(design, towelIndex, count, cache) {
  const cached = cache[design];
  if (cached !== undefined) {
    return count + cached;
  }
  let curCount = 0;
  for (const towel of towelIndex[design[0]] || []) {
    if (design.startsWith(towel)) {
      curCount += (towel.length === design.length ? 1 : countPossible(design.substring(towel.length), towelIndex, count, cache));
    }
  }
  return cache[design] = count + curCount;
}

export function parseInput(rawInput) {
  const lines = rawInput.split("\n");
  return { towels: lines[0].split(", "), designs: lines.slice(2) };
}

export function runPartOne(input) {
  const index = getTowelIndex(input.towels);
  return input.designs.filter(design => isPossible(design, index)).length;
}

export function runPartTwo(input) {
  const index = getTowelIndex(input.towels);
  const cache = {};
  return input.designs.map(design => countPossible(design, index, 0, cache)).reduce((sum, count) => sum + count, 0);
}
