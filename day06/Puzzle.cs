using Coord = (int X, int Y);

internal record Input(string[] Map, Coord Start);

internal class Puzzle(string rawInput) : AocPuzzle<Input, int>(rawInput)
{
    private static readonly Coord[] _directions = [(0, -1), (1, 0), (0, 1), (-1, 0)];

    private HashSet<Coord> GetVisitedPositions()
    {
        HashSet<Coord> positions = [];
        int directionIndex = 0;
        Coord direction = _directions[directionIndex];
        Coord position = _input.Start;
        while (true)
        {
            Coord nextPos = (position.X + direction.X, position.Y + direction.Y);
            if (nextPos.X < 0 || nextPos.X == _input.Map[0].Length || nextPos.Y < 0 || nextPos.Y == _input.Map.Length)
                break;

            if (_input.Map[nextPos.Y][nextPos.X] == '#')
            {
                directionIndex = (directionIndex + 1) % _directions.Length;
                direction = _directions[directionIndex];
            }
            else
            {
                position = nextPos;
                positions.Add(position);
            }
        }

        return positions;
    }

    protected override Input ParseInput(string rawInput)
    {
        var map = rawInput.Split('\n');
        for (int row = 0; row < map.Length; ++row)
        {
            int col = map[row].AsSpan().IndexOf('^');
            if (col >= 0)
                return new(map, (col, row));
        }
        throw new System.Diagnostics.UnreachableException();
    }

    protected override int RunPartOne() => GetVisitedPositions().Count;

    protected override int RunPartTwo()
    {
        int result = 0;
        HashSet<(Coord, Coord)> visitedPositions = [];
        var positions = GetVisitedPositions();
        foreach (Coord obstructionPos in positions)
        {
            int directionIndex = 0;
            Coord direction = _directions[directionIndex];
            Coord position = _input.Start;
            while (true)
            {
                Coord nextPos = (position.X + direction.X, position.Y + direction.Y);
                if (nextPos.X < 0 || nextPos.X == _input.Map[0].Length || nextPos.Y < 0 || nextPos.Y == _input.Map.Length)
                    break;

                if (visitedPositions.Contains((nextPos, direction)))
                {
                    ++result;
                    break;
                }

                visitedPositions.Add((position, direction));

                if (_input.Map[nextPos.Y][nextPos.X] == '#' || nextPos == obstructionPos)
                {
                    directionIndex = (directionIndex + 1) % _directions.Length;
                    direction = _directions[directionIndex];
                }
                else
                    position = nextPos;
            }

            visitedPositions.Clear();
        }

        return result;
    }
}
