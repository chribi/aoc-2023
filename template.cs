using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1Line(string line) {
    return 0;
}

if (args.Length == 0) {
    EnableLogging = true;
    var part1Cases = new (string, int)[] {
        ("1", 1),
    };
    TestUtils.Test("SolvePart1Line", SolvePart1Line, part1Cases);
} else {
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart1Line));
}
