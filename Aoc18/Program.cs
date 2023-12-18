using System.Text.RegularExpressions;
using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> lines) {
    var instructions = lines.Select(ReadInstruction).ToArray();
    var trenches = BuildTrench(instructions);
    LogTrench(trenches);

    Log("Fill");
    var map = FillTrench(trenches);
    trenches = MapToList(map);
    LogTrench(trenches);
    return trenches.Count;
}

List<Trench> BuildTrench((Dir Dir, int Count, string Color)[] instructions) {
    var x = 0;
    var y = 0;
    var trenches = new List<Trench>();
    trenches.Add(new Trench(x, y, "#000000", instructions[^1].Dir, instructions[0].Dir));
    for (var i = 0; i < instructions.Length; i++) {
        var (dir, count, color) = instructions[i];
        var (dx, dy) = Movement(dir);
        var nextDir = i + 1 < instructions.Length ? instructions[i + 1].Dir : dir;
        for (var k = 1; k <= count; k++) {
            x += dx;
            y += dy;
            trenches.Add(new Trench(x, y, color, dir, k < count ? dir : nextDir));
        }
    }

    return trenches;
}

Trench?[,] FillTrench(List<Trench> trenches) {
    var bounds = GetBoundingBox(trenches);
    var map = new Trench?[bounds.Width, bounds.Height];
    foreach (var t in trenches) {
        map[t.X - bounds.MinX, t.Y - bounds.MinY] = t;
    }

    for (var y = 0; y < bounds.Height; y++) {
        var inside = false;
        var enterDir = Dir.None;
        var exitDir = Dir.None;
        for (var x = 0; x < bounds.Width; x++) {
            if (!map[x, y].HasValue) {
                map[x, y] = inside ? new Trench(x + bounds.MinX, y + bounds.MinY,
                        "#000000", Dir.None, Dir.None)
                    : null;
                continue;
            }

            // on the outer trench
            var t = map[x, y]!.Value;
            if (t.In == Dir.U || t.In == Dir.D) {
                enterDir = t.In;
            }

            if (t.Out == Dir.U || t.Out == Dir.D) {
                exitDir = t.Out;
            }

            if (enterDir != Dir.None && exitDir != Dir.None)
            {
                if (enterDir == exitDir)
                {
                    inside = !inside;
                }
                enterDir = Dir.None;
                exitDir = Dir.None;
            }
        }
    }

    return map;
}

List<Trench> MapToList(Trench?[,] trenchMap) {
    var result = new List<Trench>();
    for (var i = 0; i < trenchMap.GetLength(0); i++) {
        for (var j = 0; j < trenchMap.GetLength(1); j++) {
            if (trenchMap[i, j].HasValue) {
                result.Add(trenchMap[i, j]!.Value);
            }
        }
    }
    return result;
}

(int MinX, int MinY, int Width, int Height) GetBoundingBox(List<Trench> trench) {
    var minX = 0;
    var maxX = 0;
    var minY = 0;
    var maxY = 0;

    foreach (var (x, y, _, _, _) in trench) {
        if (x > maxX) maxX = x;
        if (x < minX) minX = x;
        if (y > maxY) maxY = y;
        if (y < minY) minY = y;
    }

    return (minX, minY, maxX - minX + 1, maxY - minY + 1);
}

void LogTrench(List<Trench> trenches) {
    if (!LogUtils.EnableLogging) return;

    var (minX, minY, width, height) = GetBoundingBox(trenches);
    var map = new char[height, width];
    for (var i = 0; i < height; i++)
        for (var j = 0; j < width; j++)
            map[i, j] = ' ';
    foreach (var trench in trenches) {
        map[trench.Y - minY, trench.X - minX] = trench.In switch {
            Dir.U => '^',
            Dir.D => 'v',
            Dir.L => '<',
            Dir.R => '>',
            _ => '#',
        };
    }

    foreach (var mapLine in Utils.ToStringArray(map)) {
        Console.WriteLine(mapLine);
    }
}

(int, int) Movement(Dir d) {
    return d switch {
        Dir.D => (0, 1),
        Dir.U => (0, -1),
        Dir.R => (1, 0),
        Dir.L => (-1, 0),
        _ => throw new ArgumentException(),
    };
}

(Dir, int, string) ReadInstruction(string instruction) {
    var m = Regex.Match(instruction, @"([RDUL]) (\d+) \((#.{6})\)");
    var dir = Enum.Parse<Dir>(m.Groups[1].Value);
    var count = int.Parse(m.Groups[2].Value);
    var color = m.Groups[3].Value;
    return (dir, count, color);
}

if (args.Length == 0) {
    EnableLogging = true;
    TestUtils.TestCase("ReadInstruction", ReadInstruction, "U 14 (#123456)", (Dir.U, 14, "#123456"));
} else {
    Utils.AocMain(args, SolvePart1);
}

enum Dir { U, R, D, L, None };
record struct Trench(int X, int Y, string Color, Dir In, Dir Out);
