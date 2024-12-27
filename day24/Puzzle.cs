using System.Diagnostics;
using GatePair = (Gate First, Gate Second);

internal record Input(Dictionary<string, bool> Wires, Gate[] Gates, int MaxPosition);

internal class Puzzle(string rawInput) : AocPuzzle<Input, string>(rawInput)
{
    private static long RunSystem(Dictionary<string, bool> wires, List<Gate> gates)
    {
        bool hasStalled;
        do
        {
            hasStalled = true;
            for (int i = 0; i < gates.Count; ++i)
            {
                var gate = gates[i];
                if (wires.TryGetValue(gate.Input1, out bool input1) && wires.TryGetValue(gate.Input2, out bool input2))
                {
                    wires[gate.Output] = gate.Execute(input1, input2);
                    gates.RemoveAt(i--);
                    hasStalled = false;
                }
            }
        } while (!hasStalled && gates.Count > 0);
        return hasStalled ? -1 : GetValueFor(wires, 'z');
    }

    private static long GetValueFor(Dictionary<string, bool> wires, char wireStart)
        => wires.Where(w => w.Value && w.Key.StartsWith(wireStart)).Sum(w => (long)Math.Pow(2, int.Parse(w.Key[1..])));

    private static void SwapGateOutputs(GatePair pair) => (pair.First.Output, pair.Second.Output) = (pair.Second.Output, pair.First.Output);

    private bool IsValid(int position, bool full, bool lowerBits)
    {
        (bool x, bool y)[] combos = full ? [(false, true), (true, true)] : [(false, true)];
        Dictionary<string, bool> wires = [];
        List<Gate> gates = [];
        foreach (var (x, y) in combos)
        {
            for (int i = 0; i < position; ++i)
            {
                wires[$"x{i:0#}"] = lowerBits;
                wires[$"y{i:0#}"] = lowerBits;
            }
            wires[$"x{position:0#}"] = x;
            wires[$"y{position:0#}"] = y;
            for (int i = position + 1; i <= _input.MaxPosition; ++i)
            {
                wires[$"x{i:0#}"] = false;
                wires[$"y{i:0#}"] = false;
            }
            gates.AddRange(_input.Gates);
            long value = RunSystem(wires, gates);
            if (value == -1 || value != GetValueFor(wires, 'x') + GetValueFor(wires, 'y'))
                return false;

            wires.Clear();
            gates.Clear();
        }
        return true;
    }

    private HashSet<GatePair> GetSwappableGates(int position)
    {
        var lastGate = _input.Gates.First(g => g.Output == $"z{position:0#}");
        List<Gate> gates = [lastGate];
        var nextWires = new Queue<string>([lastGate.Input1, lastGate.Input2]);
        do
        {
            string wire = nextWires.Dequeue();
            if (!wire.StartsWith('x') && !wire.StartsWith('y'))
            {
                var nextGate = _input.Gates.First(g => g.Output == wire);
                gates.Add(nextGate);
                nextWires.Enqueue(nextGate.Input1);
                nextWires.Enqueue(nextGate.Input2);
            }
        } while (nextWires.Count > 0);

        HashSet<GatePair> pairs = [];
        foreach (var gate1 in gates)
        {
            foreach (var gate2 in _input.Gates)
            {
                if (gate1 == gate2 || pairs.Contains((gate2, gate1)))
                    continue;

                GatePair pair = (gate1, gate2);
                SwapGateOutputs(pair);
                if (IsValid(position, true, false))
                    pairs.Add(pair);

                SwapGateOutputs(pair);
            }
        }
        return pairs;
    }

    private string[] GetSwappedWires(List<HashSet<GatePair>> swappableGates)
    {
        foreach (var pair1 in swappableGates[0])
        {
            SwapGateOutputs(pair1);
            foreach (var pair2 in swappableGates[1])
            {
                SwapGateOutputs(pair2);
                foreach (var pair3 in swappableGates[2])
                {
                    SwapGateOutputs(pair3);
                    foreach (var pair4 in swappableGates[3])
                    {
                        SwapGateOutputs(pair4);
                        bool isValid = true;
                        for (int i = 0; i <= _input.MaxPosition; ++i)
                        {
                            if (!IsValid(i, true, true))
                            {
                                isValid = false;
                                break;
                            }
                        }
                        if (isValid)
                            return [pair1.First.Output, pair1.Second.Output, pair2.First.Output, pair2.Second.Output, pair3.First.Output, pair3.Second.Output, pair4.First.Output, pair4.Second.Output];

                        SwapGateOutputs(pair4);
                    }
                    SwapGateOutputs(pair3);
                }
                SwapGateOutputs(pair2);
            }
            SwapGateOutputs(pair1);
        }
        throw new UnreachableException();
    }

    protected override Input ParseInput(string rawInput)
    {
        var sections = rawInput.Split("\n\n");
        Dictionary<string, bool> wires = [];
        foreach (string line in sections[0].Split('\n'))
        {
            var parts = line.Split(": ");
            wires[parts[0]] = parts[1] == "1";
        }
        List<Gate> gates = [];
        foreach (string line in sections[1].Split('\n'))
        {
            var parts = line.Split(" -> ");
            var parts2 = parts[0].Split(' ');
            gates.Add(parts2[1] switch
            {
                "AND" => new AndGate(parts2[0], parts2[2], parts[1]),
                "OR" => new OrGate(parts2[0], parts2[2], parts[1]),
                "XOR" => new XorGate(parts2[0], parts2[2], parts[1]),
                _ => throw new UnreachableException()
            });
        }
        return new(wires, [..gates], wires.Count / 2 - 1);
    }

    protected override string RunPartOne() => RunSystem(_input.Wires, [.._input.Gates]).ToString();

    protected override string RunPartTwo()
    {
        List<HashSet<GatePair>> swappableGates = [];
        for (int i = 0; i <= _input.MaxPosition; ++i)
        {
            if (!IsValid(i, false, false))
                swappableGates.Add(GetSwappableGates(i));
        }

        return string.Join(",", GetSwappedWires(swappableGates).Order());
    }
}
