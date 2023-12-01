using LibAoc;

int SolvePart1Line(string line) {
    var digits = line.ToCharArray()
        .Where(c => char.IsDigit(c))
        .Select(c => c - '0')
        .ToList();
    return 10 * digits[0] + digits.Last();
}

string ReplaceDigitWords(string line) {
    var result = line
        .Replace("one", "one1one")
        .Replace("two", "two2two")
        .Replace("three", "three3three")
        .Replace("four", "four4four")
        .Replace("five", "five5five")
        .Replace("six", "six6six")
        .Replace("seven", "seven7seven")
        .Replace("eight", "eight8eight")
        .Replace("nine", "nine9nine");
    return result;
}

int SolvePart2Line(string line) => SolvePart1Line(ReplaceDigitWords(line));

if (args.Length == 0) {
    var part1Cases = new (string, int)[] {
        ("1", 11),
        ("ab1cd", 11),
        ("ab1cd2", 12),
        ("ab1cd2ef3gh", 13)
    };
    TestUtils.Test("SolvePart1Line", SolvePart1Line, part1Cases);

    var part2Cases = new (string, int)[] {
        ("one2", 12),
        ("1two", 12),
        ("twone", 21)
    };
    TestUtils.Test("SolvePart2Line", SolvePart2Line, part2Cases);
} else {
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart1Line));
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart2Line));
}
