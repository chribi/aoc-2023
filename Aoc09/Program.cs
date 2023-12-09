using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1Line(string line) {
    var numbers = ParseUtils.GetNumbers(line);
    return Extrapolate(numbers);
}

int SolvePart2Line(string line) {
    var numbers = ParseUtils.GetNumbers(line);
    numbers.Reverse();
    return Extrapolate(numbers);
}

int Extrapolate(List<long> numbers) {
    var deltas = new List<List<long>>();
    deltas.Add(numbers);

    while (deltas.Last().Any(n => n != 0)) {
        deltas.Add(Deltas(deltas.Last()));
    }
    for (int i = deltas.Count - 2; i >= 0 ; i--) {
        deltas[i].Add(deltas[i].Last() + deltas[i + 1].Last());
    }

    return (int)deltas[0].Last();
}

List<long> Deltas(List<long> numbers) {
    var result = new List<long>();
    for (var i = 1; i < numbers.Count; i++) {
        result.Add(numbers[i] - numbers[i - 1]);
    }
    return result;
}

if (args.Length == 0) {
    EnableLogging = true;
    var part1Cases = new (string, int)[] {
        ("0 3 6 9 12 15", 18),
        ("1 3 6 10 15 21", 28),
        ("10 13 16 21 30 45", 68),
    };
    TestUtils.Test("SolvePart1Line", SolvePart1Line, part1Cases);
    var part2Cases = new (string, int)[] {
        ("0 3 6 9 12 15", -3),
        ("1 3 6 10 15 21", 0),
        ("10 13 16 21 30 45", 5),
    };
    TestUtils.Test("SolvePart2Line", SolvePart2Line, part2Cases);
} else {
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart1Line));
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart2Line));
}
