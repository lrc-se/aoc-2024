using System.Collections.Frozen;

internal class Robot(FrozenDictionary<ButtonPair, char[]> padPaths, Robot? nextRobot)
{
    private char _button = 'A';
    private long _pushes;
    private readonly Dictionary<ButtonPair, long[]> _moveCache = [];

    public long Pushes => _pushes;

    protected void IncreasePushes(long[] pushes)
    {
        _pushes += pushes[0];
        nextRobot?.IncreasePushes(pushes[1..]);
    }

    public long[] MoveTo(char button)
    {
        ButtonPair buttons = (_button, button);
        if (_moveCache.TryGetValue(buttons, out var pushes))
        {
            _button = button;
            _pushes += pushes[0];
            nextRobot?.IncreasePushes(pushes[1..]);
            return pushes;
        }

        if (buttons.Start == buttons.End)
        {
            ++_pushes;
            return _moveCache[buttons] = [1, ..(nextRobot?.MoveTo('A') ?? [])];
        }

        var path = padPaths[buttons];
        int newPushes = path.Length + 1;
        _button = button;
        _pushes += newPushes;
        if (nextRobot is null)
            return _moveCache[buttons] = [newPushes];

        List<long[]> nextPushes = [];
        for (int i = 0; i < path.Length; ++i)
        {
            nextPushes.Add(nextRobot.MoveTo(path[i]));
        }
        nextPushes.Add(nextRobot.MoveTo('A'));
        long[] nextPushes2 = [..nextPushes[0]];
        for (int i = 1; i < nextPushes.Count; ++i)
        {
            for (int j = 0; j < nextPushes[0].Length; ++j)
            {
                nextPushes2[j] += nextPushes[i][j];
            }
        }

        return _moveCache[buttons] = [newPushes, ..nextPushes2];
    }
}
