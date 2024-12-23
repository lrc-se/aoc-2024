internal class Puzzle(string rawInput) : AocPuzzle<Computer, string>(rawInput)
{
    protected override Computer ParseInput(string rawInput)
    {
        var lines = rawInput.Split('\n');
        return new(
            int.Parse(lines[0].Split(": ")[1]),
            int.Parse(lines[1].Split(": ")[1]),
            int.Parse(lines[2].Split(": ")[1]),
            [..lines[4].Split(": ")[1].Split(",").Select(int.Parse)]);
    }

    protected override string RunPartOne() => string.Join(",", _input.Run());

    protected override string RunPartTwo()
    {
        var computer = new Computer(_input.RegisterA, _input.RegisterB, _input.RegisterC, _input.Program[..^2]);
        computer.Run();
        long firstRegisterA = computer.RegisterA;
        computer.Run();
        int factor = (int)(firstRegisterA / computer.RegisterA);

        var offsets = new Queue<long>([0]);
        for (int i = _input.Program.Length - 1; i > 0; --i)
        {
            for (int j = offsets.Count; j > 0; --j)
            {
                long offset = offsets.Dequeue();
                for (long registerA = offset; registerA < offset + 8; ++registerA)
                {
                    computer.Reset(registerA, _input.RegisterB, _input.RegisterC);
                    if (computer.Run()[0] == _input.Program[i])
                        offsets.Enqueue(registerA * factor);
                }
            }
        }

        for (long registerA = offsets.Dequeue(); ; ++registerA)
        {
            computer.Reset(registerA, _input.RegisterB, _input.RegisterC);
            if (computer.Run()[0] == _input.Program[0])
                return registerA.ToString();
        }
    }
}
