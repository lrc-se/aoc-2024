global using Position = (int X, int Y);
using System.Collections.Frozen;

internal record Input(char[][] Map, Position RobotPosition, char[] Moves);

internal class Puzzle(string rawInput) : AocPuzzle<Input, int>(rawInput)
{
    private static readonly FrozenDictionary<char, Position> _moves = new Dictionary<char, Position>
    {
        ['^'] = (0, -1),
        ['>'] = (1, 0),
        ['v'] = (0, 1),
        ['<'] = (-1, 0)
    }.ToFrozenDictionary();

    private int RunPuzzle(Warehouse warehouse)
    {
        foreach (char move in _input.Moves)
        {
            warehouse.AttemptMove(_moves[move]);
        }
        return warehouse.GetCoordinateSum();
    }

    protected override Input ParseInput(string rawInput)
    {
        var sections = rawInput.Split("\n\n");
        var map = sections[0].Split('\n').Select(line => line.ToCharArray()).ToArray();
        for (int y = 0; y < map.Length; ++y)
        {
            int x = map[y].AsSpan().IndexOf('@');
            if (x >= 0)
                return new(map, (x, y), sections[1].Replace("\n", "").ToCharArray());
        }
        throw new System.Diagnostics.UnreachableException();
    }

    protected override int RunPartOne() => RunPuzzle(new Warehouse(_input.Map, _input.RobotPosition));

    protected override int RunPartTwo() => RunPuzzle(new WideWarehouse(_input.Map, _input.RobotPosition));
}
