using System.Collections.Frozen;

internal class Warehouse(char[][] map, Position robotPosition, char boxTile = 'O')
{
    protected readonly char[][] _map = map;
    protected Position _robotPosition = robotPosition;

    public virtual void AttemptMove(Position move)
    {
        Position nextPos = (_robotPosition.X + move.X, _robotPosition.Y + move.Y);
        Position lastPos = nextPos;
        while (true)
        {
            char tile = _map[lastPos.Y][lastPos.X];
            if (tile == '#')
                return;
            else if (tile == '.')
                break;

            lastPos.X += move.X;
            lastPos.Y += move.Y;
        }

        if (lastPos != nextPos)
            _map[lastPos.Y][lastPos.X] = 'O';

        _map[_robotPosition.Y][_robotPosition.X] = '.';
        _map[nextPos.Y][nextPos.X] = '@';
        _robotPosition = nextPos;
    }

    public int GetCoordinateSum()
    {
        int sum = 0;
        for (int y = 0; y < _map.Length; ++y)
        {
            var row = _map[y].AsSpan();
            for (int x = 0; x < row.Length; ++x)
            {
                if (row[x] == boxTile)
                    sum += 100 * y + x;
            }
        }
        return sum;
    }
}

internal class WideWarehouse(char[][] map, Position robotPosition)
    : Warehouse([..map.Select(row => row.SelectMany(tile => _expansions[tile]).ToArray())], (robotPosition.X * 2, robotPosition.Y), '[')
{
    private static readonly FrozenDictionary<char, char[]> _expansions = new Dictionary<char, char[]>
    {
        ['#'] = ['#', '#'],
        ['O'] = ['[', ']'],
        ['.'] = ['.', '.'],
        ['@'] = ['@', '.']
    }.ToFrozenDictionary();

    public override void AttemptMove(Position move)
    {
        Position nextPos = (_robotPosition.X + move.X, _robotPosition.Y + move.Y);
        List<Position> curPositions = [nextPos];
        List<Position[]> boxPositions = [];
        HashSet<Position> curBoxPositions = [];
        while (true)
        {
            bool isClear = true;
            foreach (Position pos in curPositions)
            {
                char tile = _map[pos.Y][pos.X];
                if (tile == '#')
                    return;
                else if (tile == '[')
                {
                    isClear = false;
                    curBoxPositions.Add(pos);
                }
                else if (tile == ']')
                {
                    isClear = false;
                    curBoxPositions.Add((pos.X - 1, pos.Y));
                }
            }
            if (isClear)
                break;

            curPositions.Clear();
            foreach (Position boxPos in curBoxPositions)
            {
                curPositions.Add((boxPos.X + (move.X == 1 ? 2 : move.X), boxPos.Y + move.Y));
                if (move.Y != 0)
                    curPositions.Add((boxPos.X + move.X + 1, boxPos.Y + move.Y));
            }
            boxPositions.Add([..curBoxPositions]);
            curBoxPositions.Clear();
        }

        for (int i = boxPositions.Count - 1; i >= 0; --i)
        {
            foreach (Position boxPos in boxPositions[i])
            {
                _map[boxPos.Y][boxPos.X] = '.';
                _map[boxPos.Y][boxPos.X + 1] = '.';
                _map[boxPos.Y + move.Y][boxPos.X + move.X] = '[';
                _map[boxPos.Y + move.Y][boxPos.X + move.X + 1] = ']';
            }
        }

        _map[_robotPosition.Y][_robotPosition.X] = '.';
        _map[nextPos.Y][nextPos.X] = '@';
        _robotPosition = nextPos;
    }
}
