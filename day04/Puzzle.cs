using Delta = (int X, int Y);

internal class Puzzle(string rawInput) : AocPuzzle<string[], int>(rawInput)
{
    private static readonly Delta[] _deltas = [(1, -1), (1, 0), (1, 1), (0, 1)];

    private bool HasWord(int x, int y, Delta delta, ReadOnlySpan<char> word)
    {
        if ((delta.X > 0 && x > _input[0].Length - word.Length) || (delta.Y > 0 && y > _input.Length - word.Length) || (delta.Y < 0 && y < word.Length - 1))
            return false;

        foreach (char letter in word)
        {
            if (_input[y][x] != letter)
                return false;

            x += delta.X;
            y += delta.Y;
        }

        return true;
    }

    protected override string[] ParseInput(string rawInput) => rawInput.Split('\n');

    protected override int RunPartOne()
    {
        int result = 0;
        for (int y = 0; y < _input.Length; ++y)
        {
            for (int x = 0; x < _input[0].Length; ++x)
            {
                foreach (var delta in _deltas)
                {
                    if (HasWord(x, y, delta, "XMAS") || HasWord(x, y, delta, "SAMX"))
                        ++result;
                }
            }
        }
        return result;
    }

    protected override int RunPartTwo()
    {
        int result = 0;
        for (int y = 0, maxY = _input.Length - 3; y <= maxY; ++y)
        {
            for (int x = 0, maxX = _input[0].Length - 3; x <= maxX; ++x)
            {
                if ((HasWord(x, y, _deltas[2], "MAS") || HasWord(x, y, _deltas[2], "SAM")) && (HasWord(x, y + 2, _deltas[0], "MAS") || HasWord(x, y + 2, _deltas[0], "SAM")))
                    ++result;
            }
        }
        return result;
    }
}
