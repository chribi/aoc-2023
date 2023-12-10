using LibAoc;
using static LibAoc.LogUtils;

int SolvePart2(IEnumerable<string> lines) {
    var map = lines.ToArray();
    var startRow = Array.FindIndex(map, row => row.Contains('S'));
    var startCol = map[startRow].IndexOf('S');

    var loop = BuildLoop(map, startRow, startCol);
    var ioMap = FillInsideOutside(map, loop);
    LogMap(ioMap);
    return ioMap.Select(row => row.Count(c => c == '#')).Sum();
}

void LogMap(string[] map) {
    if (!LogUtils.EnableLogging) return;
    foreach (var line in map) Console.WriteLine(line);
}

string[] FillInsideOutside(string[] map, List<Node> loop) {
    var fill = new char[map.Length, map[0].Length];
    foreach (var node in loop)
        fill[node.Row, node.Column] = 'X';
    for (var row = 0; row < map.Length; row++) {
        var inside = false;
        var onLoop = false;
        var enterDir = Direction.East;
        for (var col = 0; col < map[0].Length; col++) {
            if (fill[row, col] != 'X') {
                fill[row, col] = inside ? '#' : ' ';
                continue;
            }

            // on loop
            var loopPipe = Get(map, new Node(row, col));
            if (loopPipe == '|')
            {
                inside = !inside;
                continue;
            }
            if (loopPipe == '-')
                continue;

            // corner handling: track North/South of corner when entering loop tiles
            if (onLoop) {
                onLoop = false;
                var (_, exitDir) = PipeDirections(loopPipe);
                if (exitDir != enterDir)
                    inside = !inside;
            } else {
                onLoop = true;
                (_, enterDir) = PipeDirections(loopPipe);
            }
        }
    }
    return ToStringArray(fill);
}

string[] ToStringArray(char[,] chars) {
    var result = new string[chars.GetLength(0)];
    var b = new char[chars.GetLength(1)];
    for (var row = 0; row < result.Length; row++) {
        for (var col = 0; col < b.Length; col++) {
            b[col] = chars[row, col];
        }
        result[row] = new string(b);
    }
    return result;
}

int SolvePart1(IEnumerable<string> lines) {
    var map = lines.ToArray();
    var startRow = Array.FindIndex(map, row => row.Contains('S'));
    var startCol = map[startRow].IndexOf('S');

    var loop = BuildLoop(map, startRow, startCol);
    return loop.Count / 2;
}

List<Node> BuildLoop(string[] map, int startRow, int startCol) {
    var loop = new List<Node>();
    var startNode = new Node(startRow, startCol);
    loop.Add(startNode);
    // from looking at input: loop starts East and South => we start right
    var moveDir = Direction.East;
    var currentNode = Move(startNode, moveDir);
    while (currentNode != startNode) {
        loop.Add(currentNode);
        var pipe = Get(map, currentNode);
        moveDir = NextDirection(pipe, moveDir);
        currentNode = Move(currentNode, moveDir);
    }

    return loop;
}

char Get(string[] map, Node n) {
    return map[n.Row][n.Column];
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

(Direction, Direction) PipeDirections(char pipe) {
    switch (pipe) {
        case '|': return (Direction.North, Direction.South);
        case '-':return (Direction.East, Direction.West);
        case 'L':return (Direction.East, Direction.North);
        case 'J':return (Direction.West, Direction.North);
        case '7':return (Direction.West, Direction.South);
        case 'F':return (Direction.East, Direction.South);
        case 'S':return (Direction.East, Direction.South);
        default: throw new ArgumentException("Not a pipe");
    }
}

Direction Opposite(Direction direction) {
    switch(direction) {
        case Direction.North: return Direction.South;
        case Direction.East:  return Direction.West;
        case Direction.South: return Direction.North;
        case Direction.West : return Direction.East;
        default: throw new ArgumentException();
    }
}

Direction NextDirection(char pipe, Direction lastDirection) {
    var cameFrom = Opposite(lastDirection);
    var (dirA, dirB) = PipeDirections(pipe);
    if (cameFrom == dirA) return dirB;
    if (cameFrom == dirB) return dirA;
    throw new ArgumentException("Invalid directions");
}

if (args.Length == 0) {
    EnableLogging = true;
} else {
    Utils.AocMain(args, SolvePart1);
    Utils.AocMain(args, SolvePart2);
}

enum Direction { North, East, South, West }
record struct Node(int Row, int Column);
