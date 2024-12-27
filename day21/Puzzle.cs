global using Position = (int X, int Y);
global using ButtonPair = (char Start, char End);
using System.Collections.Frozen;

internal class Puzzle(string rawInput) : AocPuzzle<string[], long>(rawInput)
{
    private static readonly FrozenDictionary<Position, char> _numPad = new Dictionary<Position, char>
    {
        [(0, 0)] = '7',
        [(1, 0)] = '8',
        [(2, 0)] = '9',
        [(0, 1)] = '4',
        [(1, 1)] = '5',
        [(2, 1)] = '6',
        [(0, 2)] = '1',
        [(1, 2)] = '2',
        [(2, 2)] = '3',
        [(1, 3)] = '0',
        [(2, 3)] = 'A'
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<Position, char> _dirPad = new Dictionary<Position, char>
    {
        [(1, 0)] = '^',
        [(2, 0)] = 'A',
        [(0, 1)] = '<',
        [(1, 1)] = 'v',
        [(2, 1)] = '>'
    }.ToFrozenDictionary();

    private static readonly FrozenDictionary<Position, char> _directions = new Dictionary<Position, char>
    {
        [(0, -1)] = '^',
        [(-1, 0)] = '<',
        [(0, 1)] = 'v',
        [(1, 0)] = '>'
    }.ToFrozenDictionary();

    private static Dictionary<ButtonPair, char[][]> GetPaths(FrozenDictionary<Position, char> pad)
    {
        List<char[]> GetButtonCombos(Position start, Position end, Position delta, char[] buttons)
        {
            if (start == end)
                return [buttons];

            List<char[]> curButtons = [];

            Position nextPos1 = (start.X + delta.X, start.Y);
            if (nextPos1.X != end.X + delta.X && pad.ContainsKey(nextPos1))
                curButtons.AddRange(GetButtonCombos(nextPos1, end, delta, [..buttons, _directions[(delta.X, 0)]]));

            Position nextPos2 = (start.X, start.Y + delta.Y);
            if (nextPos2.Y != end.Y + delta.Y && pad.ContainsKey(nextPos2))
                curButtons.AddRange(GetButtonCombos(nextPos2, end, delta, [..buttons, _directions[(0, delta.Y)]]));

            return curButtons;
        }

        Dictionary<ButtonPair, char[][]> paths = [];
        foreach (var (pos1, button1) in pad)
        {
            foreach (var (pos2, button2) in pad)
            {
                if (pos1 != pos2)
                    paths[(button1, button2)] = [..GetButtonCombos(pos1, pos2, (Math.Sign(pos2.X - pos1.X), Math.Sign(pos2.Y - pos1.Y)), [])];
            }
        }
        return paths;
    }

    private static FrozenDictionary<ButtonPair, char[]> GetDirPadPaths()
    {
        var allPaths = GetPaths(_dirPad);
        Dictionary<ButtonPair, char[]> paths = [];
        foreach (var (buttons, curPaths) in allPaths)
        {
            int minLength = int.MaxValue;
            foreach (var curPath in curPaths)
            {
                int curLength = 0;
                for (int i = 1; i < curPath.Length; ++i)
                {
                    if (allPaths.TryGetValue((curPath[i - 1], curPath[i]), out var nextPaths))
                        curLength += nextPaths.Min(path => path.Sum(button => allPaths[('A', button)].Min(path2 => path2.Length)));
                }
                if (curLength < minLength)
                {
                    minLength = curLength;
                    paths[buttons] = curPath;
                }
            }
        }
        return paths.ToFrozenDictionary();
    }

    private static FrozenDictionary<ButtonPair, char[]> GetNumPadPaths(FrozenDictionary<ButtonPair, char[]> dirPadPaths)
    {
        var allPaths = GetPaths(_numPad);
        Dictionary<ButtonPair, char[]> paths = [];
        foreach (var (buttons, curPaths) in allPaths)
        {
            int minLength = int.MaxValue;
            foreach (var curPath in curPaths)
            {
                int curLength = 0;
                for (int i = 1; i < curPath.Length; ++i)
                {
                    if (dirPadPaths.TryGetValue((curPath[i - 1], curPath[i]), out var nextPath))
                        curLength += nextPath.Sum(button => dirPadPaths[('A', button)].Length);
                }
                if (curLength < minLength)
                {
                    minLength = curLength;
                    paths[buttons] = curPath;
                }
            }
        }
        return paths.ToFrozenDictionary();
    }

    private long RunPuzzle(int robotCount)
    {
        var dirPadPaths = GetDirPadPaths();
        var numPadPaths = GetNumPadPaths(dirPadPaths);
        long result = 0;
        foreach (var code in _input)
        {
            var robots = new Robot[robotCount + 1];
            Robot? nextRobot = null;
            for (int i = robots.Length - 1; i > 0; --i)
            {
                robots[i] = nextRobot = new Robot(dirPadPaths, nextRobot);
            }
            robots[0] = new Robot(numPadPaths, nextRobot);
            foreach (char button in code)
            {
                robots[0].MoveTo(button);
            }
            result += robots[^1].Pushes * int.Parse(code[..^1]);
        }
        return result;
    }

    protected override string[] ParseInput(string rawInput) => rawInput.Split('\n');

    protected override long RunPartOne() => RunPuzzle(2);

    protected override long RunPartTwo() => RunPuzzle(25);
}
