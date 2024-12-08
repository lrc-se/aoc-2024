internal abstract class AocPuzzle<TInput, TResult>
{
    protected AocPuzzle(string rawInput) => _input = ParseInput(rawInput.TrimEnd());

    public string Run() => Environment.GetEnvironmentVariable("part") switch
    {
        "part1" => $"Part one result: {RunPartOne()}",
        "part2" => $"Part two result: {RunPartTwo()}",
        var part => $"Unknown part: '{part}'"
    };

    protected abstract TInput ParseInput(string rawInput);
    protected abstract TResult RunPartOne();
    protected abstract TResult RunPartTwo();

    protected TInput _input;
}
