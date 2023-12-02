using System.Text.RegularExpressions;
using LibAoc;

Game ReadGame(string line) {
    var m = Regex.Match(line, @"Game (?<id>\d+): (?<rounds>.*)");
    var id = int.Parse(m.Groups["id"].Value);
    var rounds = m.Groups["rounds"].Value.Split(";").Select(ReadRound).ToList();
    return new Game { Id = id, Rounds = rounds };
}

(int Blue, int Red, int Green) ReadRound(string round) {
    var parts = round.Split(",");
    var blue = GetFirstNumber(parts.FirstOrDefault(p => p.Contains("blue")));
    var red = GetFirstNumber(parts.FirstOrDefault(p => p.Contains("red")));
    var green = GetFirstNumber(parts.FirstOrDefault(p => p.Contains("green")));
    return (blue, red, green);
}

int GetFirstNumber(string? input, int fallback = 0) {
    if (input == null) return fallback;
    var number = Regex.Match(input, @"\d+");
    if (!number.Success) return fallback;
    return int.Parse(number.Value);
}

bool IsPossibleGamePart1(Game g) =>
    g.Rounds.All(round => round.Red <= 12 && round.Green <= 13 && round.Blue <= 14);

int SolvePart1Line(string line) {
    var g = ReadGame(line);
    return IsPossibleGamePart1(g) ? g.Id : 0;
}

int SolvePart2Line(string line) {
    var g = ReadGame(line);
    var minRed = g.Rounds.Select(r => r.Red).Max();
    var minGreen = g.Rounds.Select(r => r.Green).Max();
    var minBlue = g.Rounds.Select(r => r.Blue).Max();
    return minRed * minGreen * minBlue;
}

if (args.Length == 0) {
    var part1Cases = new (string, int)[] {
        ("Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green", 1),
        ("Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue", 2),
        ("Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red", 0),
        ("Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red", 0),
        ("Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green", 5)
    };
    TestUtils.Test("SolvePart1Line", SolvePart1Line, part1Cases);
    var part2Cases = new (string, int)[] {
        ("Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green", 48),
        ("Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue", 12),
        ("Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red", 1560),
        ("Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red", 630),
        ("Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green", 36),
        ("Game 5: 1 blue, 3 green; 2 blue, 2 green", 0)
    };
    TestUtils.Test("SolvePart2Line", SolvePart2Line, part2Cases);
} else {
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart1Line));
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart2Line));
}

struct Game {
    public int Id { get; set; }
    public List<(int Blue, int Red, int Green)> Rounds { get; set; }
}
