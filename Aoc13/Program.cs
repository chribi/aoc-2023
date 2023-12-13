using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> lines) => SolvePatterns(lines, SolvePattern);
int SolvePart2(IEnumerable<string> lines) => SolvePatterns(lines, SolveSmudgedPattern);

int SolvePatterns(IEnumerable<string> lines, Func<List<string>, int> solvePattern) {
    return Utils.SplitAtEmptyLines(lines)
        .Select(solvePattern)
        .Sum();
}

int SolvePattern(List<string> pattern) {
    for (var row = 1; row < pattern.Count; row++) {
        if (ReflectsAtRow(pattern, row))
            return row * 100;
    }
    for (var col = 1; col < pattern[0].Length; col++) {
        if (ReflectsAtCol(pattern, col))
            return col;
    }

    throw new Exception($"Bad pattern: {string.Join("\n", pattern)}");
}

int SolveSmudgedPattern(List<string> pattern) {
    for (var row = 1; row < pattern.Count; row++) {
        if (ReflectsSmudgedAtRow(pattern, row))
            return row * 100;
    }
    for (var col = 1; col < pattern[0].Length; col++) {
        if (ReflectsSmudgedAtCol(pattern, col))
            return col;
    }

    throw new Exception($"Bad pattern: {string.Join("\n", pattern)}");
}

bool ReflectsAtRow(List<string> pattern, int row) {
    for (var (left, right) = (row - 1, row);
        left >= 0 && right < pattern.Count;
        left--, right++) {
        if (pattern[left] != pattern[right]) return false;
    }
    return true;
}

bool ReflectsAtCol(List<string> pattern, int row) {
    for (var (left, right) = (row - 1, row);
        left >= 0 && right < pattern[0].Length;
        left--, right++) {
        if (Utils.GetColStr(pattern, left) != Utils.GetColStr(pattern, right)) return false;
    }
    return true;
}

bool ReflectsSmudgedAtRow(List<string> pattern, int row) {
    var errors = 0;
    for (var (left, right) = (row - 1, row);
        left >= 0 && right < pattern.Count;
        left--, right++) {
        errors += CountErrors(pattern[left], pattern[right]);
        if (errors > 1) return false;
    }
    return errors == 1;
}

bool ReflectsSmudgedAtCol(List<string> pattern, int col) {
    var errors = 0;
    for (var (left, right) = (col - 1, col);
        left >= 0 && right < pattern[0].Length;
        left--, right++) {
        errors += CountErrors(Utils.GetColStr(pattern, left), Utils.GetColStr(pattern, right));
        if (errors > 1) return false;
    }
    return errors == 1;
}

int CountErrors(string a, string b) {
    var errors = 0;
    for (var i = 0; i < a.Length; i++) {
        if (a[i] != b[i]) errors++;
    }
    return errors;
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
