using LibAoc;
using static LibAoc.LogUtils;

long SolvePart1(IEnumerable<string> input) {
    var lines = input.ToArray();
    var durations = ParseUtils.GetNumbers(lines[0]);
    var records = ParseUtils.GetNumbers(lines[1]);
    long total = 1;
    for (var i = 0; i < durations.Count; i++) {
        total *= Solve(durations[i], records[i]);
    }
    return total;
}

long SolvePart2(IEnumerable<string> input) {
    return SolvePart1(input.Select(l => l.Replace(" ", "")));
}

long Solve(long d, long r) {
    var max = (d + 1) / 2;

    var low = 1L;
    var high = max;

    while (high - low > 1) {
        var m = (high + low) / 2;
        if (m * (d - m) > r) {
            high = m;
        } else {
            low = m;
        }
    }

    return (max - high) * 2 + 1 - d % 2;
}


if (args.Length == 0) {
    EnableLogging = true;
    var solveCases = new ((long, long), long)[] {
        ((7, 9), 4),
        ((15, 40), 8),
        ((30, 200), 9),
    };
    TestUtils.Test("Solve", t => Solve(t.Item1, t.Item2), solveCases);
} else {
    // Utils.AocMain(args, Utils.SolveLineByLine(SolvePart1Line));
    Utils.AocMain(args, SolvePart1);
    Utils.AocMain(args, SolvePart2);
}
