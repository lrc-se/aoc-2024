using Coord = (int X, int Y);

internal record Garden(char[][] Regions, int Width, int Height);

internal class Puzzle(string rawInput) : AocPuzzle<Garden, int>(rawInput)
{
    private (int Size, int Circumference, int Sides) ProcessRegion(Coord coord)
    {
        int circumference = 0;
        HashSet<Coord> region = [];
        HashSet<Coord> boundary = [];
        char plant = _input.Regions[coord.Y][coord.X];
        var coords = new Queue<Coord>([coord]);
        while (coords.Count > 0)
        {
            var pos = coords.Dequeue();
            if (region.Contains(pos))
                continue;

            if (pos.X >= 0 && pos.X < _input.Width && pos.Y >= 0 && pos.Y < _input.Height && _input.Regions[pos.Y][pos.X] == plant)
            {
                region.Add(pos);
                _input.Regions[pos.Y][pos.X] = '.';
                coords.Enqueue((pos.X, pos.Y - 1));
                coords.Enqueue((pos.X + 1, pos.Y));
                coords.Enqueue((pos.X, pos.Y + 1));
                coords.Enqueue((pos.X - 1, pos.Y));
            }
            else
            {
                boundary.Add(pos);
                ++circumference;
            }
        }

        var perRow = boundary.GroupBy(pos => pos.Y).OrderBy(g => g.Key).ToDictionary(g => g.Key, g => g.Select(pos => pos.X).Order().ToArray());
        var perCol = boundary.GroupBy(pos => pos.X).OrderBy(g => g.Key).ToDictionary(g => g.Key, g => g.Select(pos => pos.Y).Order().ToArray());
        int sides = FindEdges(perRow, region, args => (args.SubPos, args.MainPos + args.Offset));
        sides += FindEdges(perCol, region, args => (args.MainPos + args.Offset, args.SubPos));

        return (region.Count, circumference, sides);
    }

    private static int FindEdges(Dictionary<int, int[]> group, HashSet<Coord> region, Func<(int MainPos, int SubPos, int Offset), Coord> getAdjacentPosition)
    {
        int sides = 0;
        int offset = -1;
        do
        {
            foreach (var (mainPos, subPositions) in group)
            {
                int start = -1;
                for (int i = 0; i < subPositions.Length; ++i)
                {
                    if (region.Contains(getAdjacentPosition((mainPos, subPositions[i], offset))))
                    {
                        start = i;
                        break;
                    }
                }
                if (start == -1)
                    continue;

                ++sides;
                int lastPos = subPositions[start];
                for (int i = start + 1; i < subPositions.Length; ++i)
                {
                    int subPos = subPositions[i];
                    if (region.Contains(getAdjacentPosition((mainPos, subPos, offset))))
                    {
                        if (subPos != lastPos + 1)
                            ++sides;

                        lastPos = subPos;
                    }
                }
            }

            offset = -offset;
        } while (offset == 1);

        return sides;
    }

    private bool TryFindNextPlot(Coord coord, out Coord nextCoord)
    {
        for (int x = coord.X; x < _input.Width; ++x)
        {
            if (_input.Regions[coord.Y][x] != '.')
            {
                nextCoord = (x, coord.Y);
                return true;
            }
        }
        for (int y = coord.Y + 1; y < _input.Height; ++y)
        {
            for (int x = 0; x < _input.Width; ++x)
            {
                if (_input.Regions[y][x] != '.')
                {
                    nextCoord = (x, y);
                    return true;
                }
            }
        }
        nextCoord = default;
        return false;
    }

    protected override Garden ParseInput(string rawInput)
    {
        var lines = rawInput.Split('\n');
        return new([..lines.Select(line => line.ToCharArray())], lines[0].Length, lines.Length);
    }

    protected override int RunPartOne()
    {
        int result = 0;
        Coord coord = (0, 0);
        do
        {
            var (size, circumference, _) = ProcessRegion(coord);
            result += size * circumference;
        } while (TryFindNextPlot(coord, out coord));
        return result;
    }

    protected override int RunPartTwo()
    {
        int result = 0;
        Coord coord = (0, 0);
        do
        {
            var (size, _, sides) = ProcessRegion(coord);
            result += size * sides;
        } while (TryFindNextPlot(coord, out coord));
        return result;
    }
}
