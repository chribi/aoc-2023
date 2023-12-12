using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1Line(string line) {
    var row = line.Split(' ')[0];
    var nums = ParseUtils.GetNumbers(line);
    return Solve(row, nums);
}

int Solve(string row, List<long> numBroken) {
    return SolveRec(row.ToCharArray(), numBroken, 0);
}

int SolveRec(char[] row, List<long> numBroken, int pos) {
    if (pos >= row.Length) return CheckFinal(row, numBroken) ? 1 : 0;
    if (row[pos] != '?') return SolveRec(row, numBroken, pos + 1);
    row[pos] = '#';
    var options = 0;
    if (Check(row, numBroken, pos + 1))
        options = SolveRec(row, numBroken, pos + 1);
    row[pos] = '.';
    if (Check(row, numBroken, pos + 1))
        options += SolveRec(row, numBroken, pos + 1);
    row[pos] = '?';
    return options;
}

bool Check(char[] row, List<long> numBroken, int checkUntil) {
    var actualBroken = GetBroken(row, checkUntil, out var lastComplete);
    if (actualBroken.Count > numBroken.Count) return false;
    for (var i = 0; i < actualBroken.Count - (lastComplete ? 0 : 1); i++) {
        if (actualBroken[i] != numBroken[i]) return false;
    }
    if (!lastComplete) {
        var last = actualBroken.Count - 1;
        if (actualBroken[last] > numBroken[last]) return false;
    }
    return true;
}

bool CheckFinal(char[] row, List<long> numBroken) {
    var actualBroken = GetBroken(row, row.Length, out _);
    if (actualBroken.Count != numBroken.Count) return false;
    for (var i = 0; i < actualBroken.Count; i++) {
        if (actualBroken[i] != numBroken[i]) return false;
    }
    return true;
}

List<long> GetBroken(char[] row, int untilPos, out bool lastComplete) {
    var result = new List<long>();
    lastComplete = true;
    var current = 0;
    for (var i = 0; i < untilPos; i++) {
        if (row[i] == '.') {
            if (current > 0)
                result.Add(current);
            lastComplete = true;
            current = 0;
        } else {
            current++;
            lastComplete = false;
        }
    }
    if (!lastComplete)
        result.Add(current);

    return result;
}


if (args.Length == 0) {
    EnableLogging = true;
    var part1Cases = new (string, int)[] {
        ("???.### 1,1,3", 1),
        (".??..??...?##. 1,1,3", 4),
        ("?#?#?#?#?#?#?#? 1,3,1,6", 1),
        ("????.#...#... 4,1,1", 1),
        ("????.######..#####. 1,6,5", 4),
        ("?###???????? 3,2,1", 10),
    };
    TestUtils.Test("SolvePart1Line", SolvePart1Line, part1Cases);
} else {
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart1Line));
}
