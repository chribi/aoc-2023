using LibAoc;
using static LibAoc.LogUtils;

long SolvePart1Line(string line) {
    var row = line.Split(' ')[0];
    var nums = ParseUtils.GetNumbers(line);
    return Solve(row, nums);
}

long SolvePart2Line(string line) {
    Log(line);
    var row = line.Split(' ')[0];
    var nums = ParseUtils.GetNumbers(line);
    var unfoldedRow = $"{row}?{row}?{row}?{row}?{row}";
    var unfoldedNums = Enumerable.Repeat(nums, 5).SelectMany(n => n).ToList();
    return Solve(unfoldedRow, unfoldedNums);
}

long Solve(string row, List<long> groups) {
    // partialSolutions[i, j] = Solve(row[^i..], groups[^j..]);
    var partialSolutions = new long[row.Length + 1, groups.Count + 1];

    // Initial cases
    // Solve("", []) = 1
    partialSolutions[0, 0] = 1;
    // Solve("", [x, ...]) = 0
    for (var j = 1; j < groups.Count; j++) {
        partialSolutions[0, j] = 0;
    }
    var i = 1;
    // Solve(xxx, []) = xxx.All(c => c != '#') ? 1 : 0;
    for (; i < row.Length; i++) {
        if (row[^i] == '#') break;
        Log($"Empty groups, tail {i} => 1");
        partialSolutions[i, 0] = 1;
    }

    for (var l = i; l < row.Length; l++) {
        Log($"Empty groups, tail {l} => 0");
        partialSolutions[l, 0] = 0;
    }

    // "recursive" cases
    for (i = 1; i <= row.Length; i++) {
        var c = row[^i];
        for (var j = 1; j <= groups.Count; j++) {

            var solutions = 0L;
            if (c == '.' || c == '?') {
                solutions += partialSolutions[i - 1, j];
            }

            if (c == '#' || c == '?') {
                var n = groups[^j];

                if (CanStartGroup(row, i, n)) {
                    var iNext = Math.Max(i - n - 1, 0);
                    solutions += partialSolutions[iNext, j - 1];
                }
            }
            Log(row, string.Join(",", groups), "@", i, j, c, "=>", solutions);
            partialSolutions[i, j] = solutions;
        }
    }

    return partialSolutions[row.Length, groups.Count];
}

// i indexes from the end
bool CanStartGroup(string row, int i, long groupSize) {
    if (i < groupSize) return false;
    for (var k = 0; k < groupSize; k++) {
        if (row[^(i - k)] == '.') return false;
    }
    if (i == groupSize) return true;
    return row[^(i - (int)groupSize)] != '#';
}

if (args.Length == 0) {
    EnableLogging = true;
    var part1Cases = new (string, long)[] {
        ("???.### 1,1,3", 1),
        (".??..??...?##. 1,1,3", 4),
        ("?#?#?#?#?#?#?#? 1,3,1,6", 1),
        ("????.#...#... 4,1,1", 1),
        ("????.######..#####. 1,6,5", 4),
        ("?###???????? 3,2,1", 10),
    };
    var part2Cases = new (string, long)[] {
        ("???.### 1,1,3", 1),
        (".??..??...?##. 1,1,3", 16384),
        ("?#?#?#?#?#?#?#? 1,3,1,6", 1),
        ("????.#...#... 4,1,1", 16),
        ("????.######..#####. 1,6,5", 2500),
        ("?###???????? 3,2,1", 506250),
    };
    TestUtils.Test("SolvePart1Line", SolvePart1Line, part1Cases);
    TestUtils.Test("SolvePart2Line", SolvePart2Line, part2Cases);
} else {
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart1Line));
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart2Line));
}
