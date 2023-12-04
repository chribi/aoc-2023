using System.Text.RegularExpressions;
using LibAoc;

int SolvePart1(IEnumerable<string> input) {
    var lines = input.ToArray();
    var total = 0;
    var numRegex = new Regex(@"\d+");
    for(var row = 0; row < lines.Length; row++) {
        var nums = numRegex.Matches(lines[row]);
        foreach (Match num in nums) {
            if (IsSymbolAdjacent(num, row, lines)) {
                var n = int.Parse(num.Value);
                total += n;
            }
        }
    }
    return total;
}

int SolvePart2(IEnumerable<string> input) {
    var lines = input.ToArray();
    var numRegex = new Regex(@"\d+");
    var gears = new Dictionary<(int, int), List<int>>();

    for(var row = 0; row < lines.Length; row++) {
        var nums = numRegex.Matches(lines[row]);
        foreach (Match num in nums) {
            foreach (var gear in AdjacentGears(num, row, lines)) {
                AddGearNum(gear, int.Parse(num.Value));
            }
        }
    }

    var total = 0;
    foreach (var gearNums in gears.Values) {
        if (gearNums.Count == 2) {
            total += gearNums[0] * gearNums[1];
        }
    }
    return total;

    void AddGearNum((int, int) pos, int num) {
        if (!gears.TryGetValue(pos, out var nums)) {
            nums = new List<int>();
            gears.Add(pos, nums);
        }
        nums.Add(num);
    }
}

bool IsSymbolAdjacent(Match m, int row, string[] input) {
    var left = Math.Max(0, m.Index - 1);
    var right = Math.Min(input[0].Length - 1, m.Index + m.Length);
    var top = Math.Max(0, row - 1);
    var bottom = Math.Min(input.Length - 1, row + 1);
    for (var i = top; i <= bottom; i++)
        for (var j = left; j <= right; j++)
            if (IsSymbol(input[i][j]))
                return true;

    return false;
}

List<(int, int)> AdjacentGears(Match m, int row, string[] input) {
    var left = Math.Max(0, m.Index - 1);
    var right = Math.Min(input[0].Length - 1, m.Index + m.Length);
    var top = Math.Max(0, row - 1);
    var bottom = Math.Min(input.Length - 1, row + 1);

    var result = new List<(int, int)>();

    for (var i = top; i <= bottom; i++)
        for (var j = left; j <= right; j++)
            if ('*' == input[i][j])
                result.Add((i, j));

    return result;
}
bool IsSymbol(char c) => !char.IsDigit(c) && c != '.';

if (args.Length == 0) {
    var input = new string[]
    {
        "467..114.." ,
        "...*......" ,
        "..35..633." ,
        "......#..." ,
        "617*......" ,
        ".....+.58." ,
        "..592....." ,
        "......755." ,
        "...$.*...." ,
        ".664.598.."
    };
    var part1Cases = new (string[], int)[] {
        (input, 4361),
    };
    var part2Cases = new (string[], int)[] {
        (input, 467835),
    };
    TestUtils.Test("SolvePart1", SolvePart1, part1Cases);
    TestUtils.Test("SolvePart2", SolvePart2, part2Cases);
} else {
    Utils.AocMain(args, SolvePart1);
    Utils.AocMain(args, SolvePart2);
}
