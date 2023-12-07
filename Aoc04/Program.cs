using LibAoc;

(List<int>, List<int>) ReadScratchcard(string line) {
    var nums = line.Split(":")[1];
    var split = nums.Split("|");
    return (ReadNumbers(split[0]), ReadNumbers(split[1]));
}
List<int> ReadNumbers(string line) {
    return line.Split(" ", StringSplitOptions.RemoveEmptyEntries)
        .Select(n => int.Parse(n)).ToList();
}

int SolvePart1Line(string line) {
    var (winning, mynums) = ReadScratchcard(line);
    var mywinning = mynums.Count(n => winning.Contains(n));
    if (mywinning == 0) return 0;
    return 1 << (mywinning - 1);
}

int SolvePart2(IEnumerable<string> lines) {
    var wins = new List<(int Count, int Wins)>();
    foreach (var line in lines) {
        var (winning, mynums) = ReadScratchcard(line);
        var mywinning = mynums.Count(n => winning.Contains(n));
        wins.Add((1, mywinning));
    }

    var total = 0;
    for (var i = 0; i < wins.Count; i++) {
        total += wins[i].Count;
        for (var j = 1; j <= wins[i].Wins; j++) {
            wins[i + j] = (wins[i + j].Count + wins[i].Count, wins[i + j].Wins);
        }
    }
    return total;
}

if (args.Length == 0) {
    var part1Cases = new (string, int)[] {
        ("Card 1: 41 48 83 86 17 | 83 86  6 31 17  9 48 53", 8),
        ("Card 2: 13 32 20 16 61 | 61 30 68 82 17 32 24 19", 2),
        ("Card 3:  1 21 53 59 44 | 69 82 63 72 16 21 14  1", 2),
        ("Card 4: 41 92 73 84 69 | 59 84 76 51 58  5 54 83", 1),
        ("Card 5: 87 83 26 28 32 | 88 30 70 12 93 22 82 36", 0),
        ("Card 6: 31 18 13 56 72 | 74 77 10 23 35 67 36 11", 0)
    };
    var part2Cases = new (List<string>, int)[] { (part1Cases.Select(tc => tc.Item1).ToList(), 30) };
    TestUtils.Test("SolvePart1Line", SolvePart1Line, part1Cases);
    TestUtils.Test("SolvePart2", SolvePart2, part2Cases);
} else {
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart1Line));
    Utils.AocMain(args, SolvePart2);
}
