using System.Text.RegularExpressions;
using LibAoc;
using static LibAoc.LogUtils;

long SolvePart1(IEnumerable<string> lines) {
    var instructions = lines.Select(ReadInstructionPart1);
    return AreaPath(instructions);
}

long SolvePart2(IEnumerable<string> lines) {
    var instructions = lines.Select(ReadInstructionPart2);
    return AreaPath(instructions);
}

// Use Area(Curve) = Integral_Curve x dy
long AreaPath(IEnumerable<(Dir, long)> path) {
    var pathLength = 0L;
    var area = 0L;
    var x = 0L;
    foreach (var (dir, steps) in path) {
        pathLength += steps;
        if (dir == Dir.R) {
            x += steps;
        }
        if (dir == Dir.L) {
            x -= steps;
        }
        if (dir == Dir.U) {
            area += x * steps;
        }
        if (dir == Dir.D) {
            area -= x * steps;
        }
    }

    // pathLength / 2 + 1 == correction for "thickness" of the path
    return Math.Abs(area) + pathLength / 2 + 1;
}

(Dir, long) ReadInstructionPart1(string instruction) {
    var m = Regex.Match(instruction, @"([RDUL]) (\d+) \((#.{6})\)");
    var dir = Enum.Parse<Dir>(m.Groups[1].Value);
    var count = long.Parse(m.Groups[2].Value);
    return (dir, count);
}

(Dir, long) ReadInstructionPart2(string instruction) {
    var m = Regex.Match(instruction, @"([RDUL]) (\d+) \(#([0-9a-f]{5})([0-3])\)");
    var steps = Convert.ToInt64(m.Groups[3].Value, 16);
    var dir = Enum.Parse<Dir>(m.Groups[4].Value);
    return (dir, steps);
}

if (args.Length == 0) {
    EnableLogging = true;
    TestUtils.TestCase("ReadInstruction1", ReadInstructionPart1, "U 14 (#123456)", (Dir.U, 14));
    TestUtils.TestCase("ReadInstruction2", ReadInstructionPart2, "U 16 (#70c710)", (Dir.R, 461937L));
    TestUtils.TestCase("ReadInstruction2", ReadInstructionPart2, "U 16 (#70c711)", (Dir.D, 461937L));
    TestUtils.TestCase("ReadInstruction2", ReadInstructionPart2, "U 16 (#70c712)", (Dir.L, 461937L));
    TestUtils.TestCase("ReadInstruction2", ReadInstructionPart2, "U 16 (#70c713)", (Dir.U, 461937L));
    TestUtils.TestCase("Area", AreaPath, new[] {
            (Dir.R, 2L), (Dir.D, 2L), (Dir.L, 2L), (Dir.U, 2L)
            }, 9L);
} else {
    Utils.AocMain(args, SolvePart1);
    Utils.AocMain(args, SolvePart2);
}

enum Dir { R, D, L, U };
