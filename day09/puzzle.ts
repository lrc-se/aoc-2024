interface Input {
  values: number[];
  size: number;
}

interface Block {
  offset: number;
  length: number;
}

interface File extends Block {
  id: number;
}

interface DiskMap {
  data: Int16Array;
  files: File[];
}

const EMPTY = -1;

function getDiskMap(input: Input): DiskMap {
  let id = 0;
  let offset = 0;
  const data = new Int16Array(input.size);
  const files: File[] = [];
  for (let i = 1; i < input.values.length; i += 2) {
    const file: File = { id, offset, length: input.values[i - 1] };
    files.push(file);
    data.fill(id++, offset, offset + file.length);
    offset += file.length;
    data.fill(EMPTY, offset, offset + input.values[i]);
    offset += input.values[i];
  }
  if (input.values.length % 2) {
    const file: File = { id, offset, length: input.values.at(-1)! };
    files.push(file);
    data.fill(id, offset, offset + file.length);
  }
  return { data, files };
}

function defragment(diskMap: DiskMap) {
  let freeOffset = diskMap.data.indexOf(EMPTY);
  let file = diskMap.files.at(-1)!;
  let fileOffset = file.offset + file.length - 1;
  while (fileOffset > freeOffset) {
    diskMap.data[freeOffset] = diskMap.data[fileOffset];
    diskMap.data[fileOffset] = EMPTY;
    if (fileOffset === file.offset) {
      file = diskMap.files[file.id - 1];
      if (!file) {
        break;
      }
      fileOffset = file.offset + file.length - 1;
    } else {
      --fileOffset;
    }
    freeOffset = diskMap.data.indexOf(EMPTY, freeOffset + 1);
  }
}

function defragmentWhole(diskMap: DiskMap) {
  const freeBlocks: Block[] = [];
  let offset = diskMap.data.indexOf(EMPTY);
  while (offset !== -1) {
    const length = diskMap.data.slice(offset + 1).findIndex(value => value !== EMPTY) + 1;
    freeBlocks.push({ offset, length });
    offset = diskMap.data.indexOf(EMPTY, offset + length + 1);
  }
  for (let i = diskMap.files.length - 1; i >= 0; --i) {
    const file = diskMap.files[i];
    for (const freeBlock of freeBlocks) {
      if (freeBlock.offset < file.offset && freeBlock.length >= file.length) {
        diskMap.data.fill(file.id, freeBlock.offset, freeBlock.offset + file.length);
        diskMap.data.fill(EMPTY, file.offset, file.offset + file.length);
        freeBlock.offset += file.length;
        freeBlock.length -= file.length;
        break;
      }
    }
  }
}

function getChecksum(data: DiskMap["data"]) {
  return data.reduce((checksum, value, position) => checksum + (value !== EMPTY ? value * position : 0), 0);
}

export function parseInput(rawInput: string): Input {
  const values = new Array<number>(rawInput.length);
  let size = 0;
  for (let i = 0; i < rawInput.length; ++i) {
    size += values[i] = rawInput.charCodeAt(i) - 48;
  }
  return { values, size };
}

export function runPartOne(input: Input) {
  const diskMap = getDiskMap(input);
  defragment(diskMap);
  return getChecksum(diskMap.data);
}

export function runPartTwo(input: Input) {
  const diskMap = getDiskMap(input);
  defragmentWhole(diskMap);
  return getChecksum(diskMap.data);
}
