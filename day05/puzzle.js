function isCorrect(update, rules) {
  for (let i = 1; i < update.length; ++i) {
    const rule = rules[update[i - 1]];
    if (!rule || !update.slice(i).every(page => rule.has(page))) {
      return false;
    }
  }
  return true;
}

export function parseInput(rawInput) {
  const rules = {};
  const lines = rawInput.split("\n");
  let i = 0;
  do {
    const parts = lines[i].split("|");
    let rule = rules[parts[0]];
    if (!rule) {
      rules[parts[0]] = rule = new Set();
    }
    rule.add(Number(parts[1]));
  } while (lines[++i]);
  return { rules, updates: lines.slice(i + 1).map(line => line.split(",").map(Number)) };
}

export function runPartOne(input) {
  const correctUpdates = input.updates.filter(update => isCorrect(update, input.rules));
  return correctUpdates.reduce((sum, update) => sum + update[update.length >> 1], 0);
}

export function runPartTwo(input) {
  const incorrectUpdates = input.updates.filter(update => !isCorrect(update, input.rules));
  for (const update of incorrectUpdates) {
    update.sort((a, b) => input.rules[a]?.has(b) ? -1 : 1);
  }
  return incorrectUpdates.reduce((sum, update) => sum + update[update.length >> 1], 0);
}
