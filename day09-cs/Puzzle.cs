internal record Input(int[] Values, int Size);
internal readonly record struct DiskBlock(int Offset, int Length);
internal readonly record struct DiskFile(int Id, int Offset, int Length);
internal record DiskMap(int[] Data, DiskFile[] Files);

internal class Puzzle(string rawInput) : AocPuzzle<Input, long>(rawInput)
{
    private const int Empty = -1;

    private DiskMap GetDiskMap()
    {
        int id = 0;
        int offset = 0;
        var data = new int[_input.Size].AsSpan();
        List<DiskFile> files = [];
        for (int i = 1; i < _input.Values.Length; i += 2)
        {
            DiskFile file = new(id, offset, _input.Values[i - 1]);
            files.Add(file);
            data[offset..(offset + file.Length)].Fill(id++);
            offset += file.Length;
            data[offset..(offset + _input.Values[i])].Fill(Empty);
            offset += _input.Values[i];
        }
        if (_input.Values.Length % 2 > 0)
        {
            DiskFile file = new(id, offset, _input.Values[^1]);
            files.Add(file);
            data[offset..(offset + file.Length)].Fill(id);
        }
        return new([..data], [..files]);
    }

    private static void Defragment(DiskMap diskMap)
    {
        var data = diskMap.Data.AsSpan();
        int freeOffset = data.IndexOf(Empty);
        var file = diskMap.Files[^1];
        int fileOffset = file.Offset + file.Length - 1;
        while (fileOffset > freeOffset)
        {
            data[freeOffset] = data[fileOffset];
            data[fileOffset] = Empty;
            if (fileOffset == file.Offset)
            {
                if (file.Id == 0)
                    break;

                file = diskMap.Files[file.Id - 1];
                fileOffset = file.Offset + file.Length - 1;
            }
            else
                --fileOffset;

            freeOffset += data[(freeOffset + 1)..].IndexOf(Empty) + 1;
        }
    }

    private static void DefragmentWhole(DiskMap diskMap)
    {
        var data = diskMap.Data.AsSpan();
        int freeOffset = data.IndexOf(Empty);
        List<DiskBlock> freeBlocks = [];
        while (true)
        {
            int freeLength = data[(freeOffset + 1)..].IndexOfAnyExcept(Empty) + 1;
            freeBlocks.Add(new(freeOffset, freeLength));
            int nextOffset = data[(freeOffset + freeLength + 1)..].IndexOf(Empty);
            if (nextOffset == -1)
                break;

            freeOffset += freeLength + nextOffset + 1;
        }
        for (int i = diskMap.Files.Length - 1; i >= 0; --i)
        {
            var file = diskMap.Files[i];
            for (int j = 0; j < freeBlocks.Count; ++j)
            {
                var freeBlock = freeBlocks[j];
                if (freeBlock.Offset < file.Offset && freeBlock.Length >= file.Length)
                {
                    data[freeBlock.Offset..(freeBlock.Offset + file.Length)].Fill(file.Id);
                    data[file.Offset..(file.Offset + file.Length)].Fill(Empty);
                    freeBlocks[j] = new(freeBlock.Offset + file.Length, freeBlock.Length - file.Length);
                    break;
                }
            }
        }
    }

    private static long GetChecksum(int[] data) => data.Index().Sum(i => i.Item != Empty ? (long)i.Item * i.Index : 0);

    protected override Input ParseInput(string rawInput)
    {
        var values = new int[rawInput.Length];
        int size = 0;
        for (int i = 0; i < rawInput.Length; ++i)
        {
            size += values[i] = rawInput[i] - 48;
        }
        return new(values, size);
    }

    protected override long RunPartOne()
    {
        var diskMap = GetDiskMap();
        Defragment(diskMap);
        return GetChecksum(diskMap.Data);
    }

    protected override long RunPartTwo()
    {
        var diskMap = GetDiskMap();
        DefragmentWhole(diskMap);
        return GetChecksum(diskMap.Data);
    }
}
