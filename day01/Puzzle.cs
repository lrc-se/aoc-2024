internal class Puzzle(string rawInput) : AocPuzzle<(int[] Left, int[] Right), int>(rawInput)
{
    protected override (int[], int[]) ParseInput(string rawInput)
    {
        var lines = rawInput.Split('\n');
        var (left, right) = (new int[lines.Length], new int[lines.Length]);
        for (int i = 0; i < lines.Length; ++i)
        {
            var parts = lines[i].Split("   ");
            left[i] = int.Parse(parts[0]);
            right[i] = int.Parse(parts[1]);
        }
        return (left, right);
    }

    protected override int RunPartOne()
        => _input.Left.OrderBy(id => id)
            .Zip(_input.Right.OrderBy(id => id))
            .Sum(pair => Math.Abs(pair.First - pair.Second));

    protected override int RunPartTwo()
    {
        Dictionary<int, int> counts = [];
        foreach (int id in _input.Right)
        {
            ++System.Runtime.InteropServices.CollectionsMarshal.GetValueRefOrAddDefault(counts, id, out _);
        }
        return _input.Left.Sum(id => id * counts.GetValueOrDefault(id));
    }
}
