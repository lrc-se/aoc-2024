using Coord = (int X, int Y);

internal record Input(string[] Map, Coord Start, Coord End);
internal readonly record struct MapPos(Coord Pos, int DirectionIndex);
internal readonly record struct PathPos(Coord Pos, int Score);

internal class Puzzle(string rawInput) : AocPuzzle<Input, int>(rawInput)
{
    private static readonly Coord[] _directions = [(1, 0), (0, 1), (-1, 0), (0, -1)];

    private (int Score, Dictionary<Coord, HashSet<PathPos>> PreviousPositions) GetLowestScore()
    {
        int curScore = 0;
        int minScore = int.MaxValue;
        MapPos curPos = new(_input.Start, 0);
        Dictionary<MapPos, int> visited = [];
        Dictionary<MapPos, int> unvisited = new() { [curPos] = 0 };
        Dictionary<Coord, HashSet<PathPos>> prev = [];

        while (true)
        {
            for (int i = 0; i <= 3; ++i)
            {
                int nextDirectionIndex = (curPos.DirectionIndex + i) % 4;
                var direction = _directions[nextDirectionIndex];
                MapPos nextPos = new((curPos.Pos.X + direction.X, curPos.Pos.Y + direction.Y), nextDirectionIndex);
                if (_input.Map[nextPos.Pos.Y][nextPos.Pos.X] == '#' || visited.ContainsKey(nextPos))
                    continue;

                int nextScore = curScore + (nextDirectionIndex == curPos.DirectionIndex ? 1 : 1001);
                if (nextScore < minScore && (!unvisited.TryGetValue(nextPos, out int prevScore) || nextScore < prevScore))
                {
                    unvisited[nextPos] = nextScore;
                    if (!prev.TryGetValue(nextPos.Pos, out var prevs))
                        prev[nextPos.Pos] = prevs = [];

                    prevs.Add(new(curPos.Pos, nextScore));
                }
            }

            visited[curPos] = curScore;
            if (curPos.Pos == _input.End && curScore < minScore)
                minScore = curScore;

            unvisited.Remove(curPos);
            if (unvisited.Count == 0)
                break;

            int curMin = int.MaxValue;
            foreach (var (pos, score) in unvisited)
            {
                if (score < curMin)
                {
                    curMin = score;
                    curPos = pos;
                }
            }

            curScore = curMin;
        }

        return (minScore, prev);
    }

    protected override Input ParseInput(string rawInput)
    {
        var map = rawInput.Split('\n');
        return new(map, (1, map.Length - 2), (map[0].Length - 2, 1));
    }

    protected override int RunPartOne() => GetLowestScore().Score;

    protected override int RunPartTwo()
    {
        var (score, prev) = GetLowestScore();
        var unvisited = new Queue<PathPos>(prev[_input.End]);
        HashSet<Coord> visited = [_input.End];
        PathPos curPos = new(_input.End, score + 1);
        do
        {
            foreach (var prevPos in prev[curPos.Pos])
            {
                if ((curPos.Score - prevPos.Score) % 1000 == 1 && !visited.Contains(prevPos.Pos))
                {
                    visited.Add(prevPos.Pos);
                    unvisited.Enqueue(prevPos);
                }
            }
        } while (unvisited.TryDequeue(out curPos));

        return visited.Count;
    }
}
