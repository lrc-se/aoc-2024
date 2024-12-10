using Position = (int X, int Y);

internal record TrailMap(int[,] Heights, int Width, int Height, Position[] TrailHeads, Position[] Targets);

internal class Puzzle(string rawInput) : AocPuzzle<TrailMap, int>(rawInput)
{
    private static readonly Position[] _directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];

    private bool CanBeNext(Position pos, int height, HashSet<Position> visited)
        => pos.X >= 0 && pos.X < _input.Width && pos.Y >= 0 && pos.Y < _input.Height && _input.Heights[pos.X, pos.Y] == height && !visited.Contains(pos);

    private bool CanReachTarget(Position start, Position target, HashSet<Position> visited)
    {
        if (start == target)
            return true;

        int height = _input.Heights[start.X, start.Y] + 1;
        foreach (var direction in _directions)
        {
            Position pos = (start.X + direction.X, start.Y + direction.Y);
            if (CanBeNext(pos, height, visited) && CanReachTarget(pos, target, [..visited, pos]))
                return true;
        }

        return false;
    }

    private int CountPaths(Position start, Position target, HashSet<Position> visited)
    {
        if (start == target)
            return 1;

        int height = _input.Heights[start.X, start.Y] + 1;
        int count = 0;
        foreach (var direction in _directions)
        {
            Position pos = (start.X + direction.X, start.Y + direction.Y);
            if (!CanBeNext(pos, height, visited))
                continue;

            count += CountPaths(pos, target, [.. visited, pos]);
        }

        return count;
    }

    protected override TrailMap ParseInput(string rawInput)
    {
        var lines = rawInput.Split('\n');
        (int width, int height) = (lines[0].Length, lines.Length);
        var heights = new int[width, height];
        List<Position> trailHeads = [];
        List<Position> targets = [];
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                int trailHeight = lines[y][x] - 48;
                heights[x, y] = trailHeight;
                if (trailHeight == 0)
                    trailHeads.Add((x, y));
                else if (trailHeight == 9)
                    targets.Add((x, y));
            }
        }
        return new(heights, width, height, [..trailHeads], [..targets]);
    }

    protected override int RunPartOne()
    {
        int result = 0;
        foreach (var trailHead in _input.TrailHeads)
        {
            foreach (var target in _input.Targets)
            {
                if (CanReachTarget(trailHead, target, []))
                    ++result;
            }
        }
        return result;
    }

    protected override int RunPartTwo()
    {
        int result = 0;
        foreach (var trailHead in _input.TrailHeads)
        {
            foreach (var target in _input.Targets)
            {
                result += CountPaths(trailHead, target, []);
            }
        }
        return result;
    }
}
