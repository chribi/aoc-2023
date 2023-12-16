using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> line) {
    var map = line.ToArray();
    return CalculateEnergization(map, new Node(0, 0), Direction.East);
}

int SolvePart2(IEnumerable<string> line) {
    var map = line.ToArray();
    var energizations = new List<int>();

    var height = map.Length;
    var width = map[0].Length;
    for (var col = 0; col < width; col++) {
        energizations.Add(CalculateEnergization(map, new Node(0, col), Direction.South));
        energizations.Add(CalculateEnergization(map, new Node(height - 1, col), Direction.North));
    }
    for (var row = 0; row < height; row++) {
        energizations.Add(CalculateEnergization(map, new Node(row, 0), Direction.East));
        energizations.Add(CalculateEnergization(map, new Node(row, height - 1), Direction.West));
    }
    return energizations.Max();
}

int CalculateEnergization(string[] map, Node startPos, Direction startDir) {
    // map of energized positions, each byte encodes whether it was energized
    // by a beam in a certain direction, this allows detecting circles
    var energized = new byte[map.Length, map[0].Length];
    var beams = new List<(Node, Direction)>();

    // to-do-list of beams that need to be handled, can grow when encountering splitters
    beams.Add((startPos, startDir));

    while (beams.Any()) {
        var (pos, dir) = beams.Last();
        beams.RemoveAt(beams.Count - 1);

        while (true) {
            var isDone = MarkBeam(pos, dir, energized);
            if (isDone) break;

            var current = map[pos.Row][pos.Column];

            if (current == '/' || current == '\\')
                dir = Mirror(dir, current);

            if (current == '|' || current == '-') {
                var orthogonal = (dir == Direction.East || dir == Direction.West)
                    == (current == '|');
                if (orthogonal) {
                    // Split of a beam and add it to the todo list, then continue
                    // in the opposite direction
                    var dirSplit = Mirror(dir, '/');
                    var posSplit = Move(pos, dirSplit);
                    beams.Add((posSplit, dirSplit));

                    dir = Mirror(dir, '\\');
                }
            }

            pos = Move(pos, dir);
        }
    }

    var count = 0;
    foreach (var e in energized) {
        if (e != 0) count++;
    }
    Log($"Result after entering at {startPos.Row}, {startPos.Column}, {startDir}: {count}");
    LogEnergizedMap(energized);
    return count;
}

void LogEnergizedMap(byte[,] energized) {
    if (!LogUtils.EnableLogging) return;
    var line = new char[energized.GetLength(1)];
    for (var row = 0; row < energized.GetLength(0); row++) {
        for (var col = 0; col < energized.GetLength(1); col++) {
            line[col] = EnergizedToChar(energized[row, col]);
        }
        Console.WriteLine(new string(line));
    }
}

char EnergizedToChar(byte e) {
    return e switch {
        0 => '.',
        1 => '^',
        2 => '>',
        4 => 'v',
        8 => '<',
        _ => '#',
    };
}

bool MarkBeam(Node pos, Direction dir, byte[,] energized) {
    var (r, c) = pos;

    // out of bounds check
    if (r < 0 || r >= energized.GetLength(0)) return true;
    if (c < 0 || c >= energized.GetLength(1)) return true;

    var existing = energized[r, c];
    var value = (byte)dir;

    // cycle detection
    if ((existing & value) != 0) return true;

    energized[r, c] = (byte)(existing | value);
    return false;
}

if (args.Length == 0) {
    EnableLogging = true;
    // var part1Cases = new (string, int)[] {
    //     ("1", 1),
    // };
    // TestUtils.Test("SolvePart1Line", SolvePart1Line, part1Cases);
} else {
    Utils.AocMain(args, SolvePart1);
    Utils.AocMain(args, SolvePart2);
}

Direction Mirror(Direction dir, char mirror) {
    switch (dir) {
        case Direction.North: return mirror == '/' ? Direction.East : Direction.West;
        case Direction.South: return mirror == '/' ? Direction.West : Direction.East;
        case Direction.East:  return mirror == '/' ? Direction.North : Direction.South;
        case Direction.West:  return mirror == '/' ? Direction.South : Direction.North;
        default: throw new ArgumentException();
    }
}

Node Move(Node from, Direction to) {
    var (r, c) = from;
    switch(to) {
        case Direction.North: return new Node(r - 1, c);
        case Direction.East:  return new Node(r, c + 1);
        case Direction.South: return new Node(r + 1, c);
        case Direction.West : return new Node(r, c - 1);
        default: throw new ArgumentException();
    }
}

enum Direction { North = 1, East = 2, South = 4, West = 8}
record struct Node(int Row, int Column);
