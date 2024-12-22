internal class Puzzle(string rawInput) : AocPuzzle<int[], long>(rawInput)
{
    private static long GetSecretNumber(long seed)
    {
        seed = (seed ^ (seed << 6)) % 16777216;
        seed = (seed ^ (seed >> 5)) % 16777216;
        return (seed ^ (seed << 11)) % 16777216;
    }

    private static int GetChangeSequenceHash(int[] sequence)
    {
        int hash = sequence[0];
        for (int i = 1; i < 4; ++i)
        {
            hash = 47 * hash + sequence[i];
        }
        return hash;
    }

    protected override int[] ParseInput(string rawInput) => [..rawInput.Split('\n').Select(int.Parse)];

    protected override long RunPartOne()
    {
        long result = 0;
        foreach (int seed in _input)
        {
            long secretNumber = seed;
            for (int i = 0; i < 2000; ++i)
            {
                secretNumber = GetSecretNumber(secretNumber);
            }
            result += secretNumber;
        }
        return result;
    }

    protected override long RunPartTwo()
    {
        Dictionary<int, int[]> allChangeSequencePrices = [];
        var prices = new int[2000];
        var changes = new int[2000];
        for (int i = 0; i < _input.Length; ++i)
        {
            long secretNumber = _input[i];
            for (int j = 0; j < 2000; ++j)
            {
                secretNumber = GetSecretNumber(secretNumber);
                prices[j] = (int)(secretNumber % 10);
                if (j > 0)
                    changes[j] = prices[j] - prices[j - 1];

                if (j > 3)
                {
                    int key = GetChangeSequenceHash(changes[(j - 3)..(j + 1)]);
                    if (!allChangeSequencePrices.TryGetValue(key, out var changeSequencePrices))
                        allChangeSequencePrices[key] = changeSequencePrices = new int[_input.Length];

                    if (changeSequencePrices[i] == 0)
                        changeSequencePrices[i] = prices[j];
                }
            }
        }

        return allChangeSequencePrices.Values.Max(prices => prices.Sum());
    }
}
