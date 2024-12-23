type Network = Record<string, Set<string>>;
type Result = number | string;

export function parseInput(rawInput: string): Network {
  const network: Network = {};
  for (const line of rawInput.split("\n")) {
    const [comp1, comp2] = line.split("-");
    let connections = network[comp1];
    if (!connections) {
      network[comp1] = connections = new Set();
    }
    connections.add(comp2);
    connections = network[comp2];
    if (!connections) {
      network[comp2] = connections = new Set();
    }
    connections.add(comp1);
  }
  return network;
}

export function runPartOne(input: Network): Result {
  const groups = new Set<string>();
  for (const [comp1, connections] of Object.entries(input)) {
    for (const comp2 of connections) {
      for (const comp3 of input[comp2]) {
        if (input[comp3].has(comp1)) {
          const group = [comp1, comp2, comp3];
          if (group.some(comp => comp.startsWith("t"))) {
            groups.add(group.sort().join(""));
          }
        }
      }
    }
  }
  return groups.size;
}

export function runPartTwo(input: Network): Result {
  const largestGroups: Network = {};
  for (const [comp, connections] of Object.entries(input)) {
    let largestGroup = new Set<string>();
    const set = new Set([comp, ...connections]);
    for (const [comp2, connections2] of Object.entries(input)) {
      if (comp !== comp2) {
        const group = set.intersection(new Set([comp2, ...connections2]));
        if (group.size > largestGroup.size) {
          largestGroup = group;
        }
      }
    }
    largestGroups[comp] = largestGroup;
  }
  let largestGroup = new Set<string>();
  for (const connections of Object.values(largestGroups)) {
    if (connections.size > largestGroup.size && connections.values().every(comp => largestGroups[comp].size === connections.size)) {
      largestGroup = connections;
    }
  }
  return [...largestGroup].sort().join(",");
}
