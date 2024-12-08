using Coord = (int X, int Y);

internal record Map(Coord[][] Antennas, int Width, int Height);

internal class Puzzle(string rawInput) : AocPuzzle<Map, int>(rawInput)
{
    private bool IsWithinMap(Coord coord) => coord.X >= 0 && coord.X < _input.Width && coord.Y >= 0 && coord.Y < _input.Height;

    protected override Map ParseInput(string rawInput)
    {
        Dictionary<char, List<Coord>> antennas = [];
        var lines = rawInput.Split('\n');
        (int width, int height) = (lines[0].Length, lines.Length);
        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                char freq = lines[y][x];
                if (freq != '.')
                {
                    if (!antennas.TryGetValue(freq, out var coords))
                        antennas[freq] = coords = [];

                    coords.Add((x, y));
                }
            }
        }
        return new([..antennas.Values.Select(coords => coords.ToArray())], width, height);
    }

    protected override int RunPartOne()
    {
        HashSet<Coord> antinodes = [];
        foreach (var coords in _input.Antennas)
        {
            for (int i = 0; i < coords.Length - 1; ++i)
            {
                for (int j = i + 1; j < coords.Length; ++j)
                {
                    Coord delta = (coords[i].X - coords[j].X, coords[i].Y - coords[j].Y);

                    Coord antinode = (coords[i].X + delta.X, coords[i].Y + delta.Y);
                    if (IsWithinMap(antinode))
                        antinodes.Add(antinode);

                    antinode = (coords[j].X - delta.X, coords[j].Y - delta.Y);
                    if (IsWithinMap(antinode))
                        antinodes.Add(antinode);
                }
            }
        }
        return antinodes.Count;
    }

    protected override int RunPartTwo()
    {
        HashSet<Coord> antinodes = [];
        foreach (var coords in _input.Antennas)
        {
            for (int i = 0; i < coords.Length - 1; ++i)
            {
                for (int j = i + 1; j < coords.Length; ++j)
                {
                    Coord delta = (coords[i].X - coords[j].X, coords[i].Y - coords[j].Y);

                    Coord antinode = coords[i];
                    while (IsWithinMap(antinode))
                    {
                        antinodes.Add(antinode);
                        antinode.X += delta.X;
                        antinode.Y += delta.Y;
                    }

                    antinode = coords[j];
                    while (IsWithinMap(antinode))
                    {
                        antinodes.Add(antinode);
                        antinode.X -= delta.X;
                        antinode.Y -= delta.Y;
                    }
                }
            }
        }
        return antinodes.Count;
    }
}
