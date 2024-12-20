using System.Runtime.InteropServices;
using Position = (int X, int Y);

internal record Racetrack(string[] Map, Position Start, Position End);

internal class Puzzle(string rawInput) : AocPuzzle<Racetrack, int>(rawInput)
{
    private static readonly Position[] _directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];

    private Dictionary<Position, int> GetPath()
    {
        Position pos = _input.Start;
        int time = 0;
        Dictionary<Position, int> positions = new() { [pos] = time };
        do
        {
            foreach (Position direction in _directions)
            {
                Position nextPos = (pos.X + direction.X, pos.Y + direction.Y);
                if (_input.Map[nextPos.Y][nextPos.X] != '#' && !positions.ContainsKey(nextPos))
                {
                    positions.Add(nextPos, ++time);
                    pos = nextPos;
                    break;
                }
            }
        } while (pos != _input.End);
        return positions;
    }

    private Dictionary<int, int> FindCheatSaves()
    {
        var path = GetPath();
        Dictionary<int, int> saves = [];
        foreach (var (pos, time) in path)
        {
            foreach (var direction in _directions)
            {
                Position nextPos1 = (pos.X + direction.X, pos.Y + direction.Y);
                Position nextPos2 = (pos.X + direction.X * 2, pos.Y + direction.Y * 2);
                if (!path.ContainsKey(nextPos1) && path.TryGetValue(nextPos2, out int nextTime) && nextTime > time)
                    ++CollectionsMarshal.GetValueRefOrAddDefault(saves, nextTime - time - 2, out _);
            }
        }
        return saves;
    }

    private Dictionary<int, int> FindCheatSaves2()
    {
        var path = GetPath();
        Dictionary<int, int> saves = [];
        var pathPoints = path.OrderBy(i => i.Value).ToArray().AsSpan();
        for (int i = 0; i < pathPoints.Length - 5; ++i)
        {
            var (startPos, startTime) = pathPoints[i];
            for (int j = i + 4; j < pathPoints.Length; ++j)
            {
                var (endPos, endTime) = pathPoints[j];
                int steps = Math.Abs(endPos.X - startPos.X) + Math.Abs(endPos.Y - startPos.Y);
                if (steps > 20)
                    continue;

                int diff = endTime - startTime - steps;
                if (diff > 0)
                    ++CollectionsMarshal.GetValueRefOrAddDefault(saves, diff, out _);
            }
        }
        return saves;
    }

    private int CountCheats(Dictionary<int, int> cheatSaves)
    {
        int limit = _input.Map.Length > 15 ? 100 : 50;
        return cheatSaves.Sum(i => i.Key >= limit ? i.Value : 0);
    }

    protected override Racetrack ParseInput(string rawInput)
    {
        var map = rawInput.Split('\n');
        (Position start, Position end) = (default, default);
        for (int y = 0; y < map.Length; ++y)
        {
            for (int x = 0; x < map[0].Length; ++x)
            {
                if (map[y][x] == 'S')
                    start = (x, y);

                if (map[y][x] == 'E')
                    end = (x, y);
            }
        }
        return new(map, start, end);
    }

    protected override int RunPartOne() => CountCheats(FindCheatSaves());

    protected override int RunPartTwo() => CountCheats(FindCheatSaves2());
}
