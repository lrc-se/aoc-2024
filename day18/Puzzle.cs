using Coord = (int X, int Y);

internal record Input(Coord[] Bytes, Coord Exit);

internal class Puzzle(string rawInput) : AocPuzzle<Input, string>(rawInput)
{
    private static readonly Coord[] _directions = [(1, 0), (0, 1), (-1, 0), (0, -1)];

    private static int GetShortestPath(HashSet<Coord> bytes, Coord end)
    {
        int curLength = 0;
        int minLength = int.MaxValue;
        Coord curPos = (0, 0);
        Dictionary<Coord, int> visited = [];
        Dictionary<Coord, int> unvisited = new() { [curPos] = 0 };

        while (true)
        {
            foreach (var direction in _directions)
            {
                Coord nextPos = (curPos.X + direction.X, curPos.Y + direction.Y);
                if (nextPos.X < 0 || nextPos.X > end.X || nextPos.Y < 0 || nextPos.Y > end.Y || bytes.Contains(nextPos) || visited.ContainsKey(nextPos))
                    continue;

                int nextLength = curLength + 1;
                if (nextLength < minLength && (!unvisited.TryGetValue(nextPos, out int prevLength) || nextLength < prevLength))
                    unvisited[nextPos] = nextLength;
            }

            visited[curPos] = curLength;
            if (curPos == end)
                minLength = curLength;

            unvisited.Remove(curPos);
            if (unvisited.Count == 0)
                break;

            int curMin = int.MaxValue;
            foreach (var (pos, length) in unvisited)
            {
                if (length < curMin)
                {
                    curMin = length;
                    curPos = pos;
                }
            }

            curLength = curMin;
        }

        return minLength;
    }

    private static Coord CreateCoord(string line)
    {
        var parts = line.Split(',');
        return (int.Parse(parts[0]), int.Parse(parts[1]));
    }

    protected override Input ParseInput(string rawInput)
    {
        Coord[] coords = [..rawInput.Split('\n').Select(CreateCoord)];
        return new(coords, (coords.Max(c => c.X), coords.Max(c => c.Y)));
    }

    protected override string RunPartOne() => GetShortestPath([.._input.Bytes[..(_input.Bytes.Length > 1024 ? 1024 : 12)]], _input.Exit).ToString();

    protected override string RunPartTwo()
    {
        int i = _input.Bytes.Length - 1;
        HashSet<Coord> coords = [.._input.Bytes];
        while (true)
        {
            int length = GetShortestPath(coords, _input.Exit);
            if (length != int.MaxValue)
                return $"{_input.Bytes[i].X},{_input.Bytes[i].Y}";

            coords.Remove(_input.Bytes[--i]);
        }
    }
}
