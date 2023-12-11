using LibAoc;
using static LibAoc.LogUtils;

long Solve(IEnumerable<string> lines, int expansion) {
    var map = lines.ToArray();
    var galaxies = GetGalaxies(map);
    Expand(galaxies, map, expansion);
    Log(galaxies.Count);

    var dist = 0L;
    for (var i = 0; i < galaxies.Count; i++) {
        var a = galaxies[i];
        for (var j = i + 1; j < galaxies.Count; j++) {
            dist += Dist(a, galaxies[j]);
        }
        Log(i, a, dist);
    }

    return dist;
}

List<Galaxy> GetGalaxies(string[] map) {
    var galaxies = new List<Galaxy>();
    for (var row = 0; row < map.Length; row++) {
        for (var col = 0; col < map[row].Length; col++) {
            if (map[row][col] == '#') {
                galaxies.Add(new Galaxy(row, col));
            }
        }
    }
    return galaxies;
}

void Expand(List<Galaxy> galaxies, string[] map, int expansion) {
    var rows = new List<int>();
    for (var i = 0; i < map.Length; i++)
        if (!map[i].Contains('#'))
            rows.Add(i);

    var cols = new List<int>();
    for (var j = 0; j < map[0].Length; j++)
        if (Enumerable.Range(0, map.Length).All(i => map[i][j] != '#'))
            cols.Add(j);

    for (var i = 0; i < galaxies.Count; i++) {
        var (row, col) = galaxies[i];
        var rowExpand = rows.Count(r => r < row);
        var colExpand = cols.Count(c => c < col);
        galaxies[i] = new Galaxy(row + expansion * rowExpand, col + expansion * colExpand);
    }
}

long Dist(Galaxy a, Galaxy b) {
    return Math.Abs(a.Row - b.Row) + Math.Abs(a.Col - b.Col);
}

if (args.Length == 0) {
    EnableLogging = true;
    // var part1Cases = new (string, int)[] {
    //     ("1", 1),
    // };
    // TestUtils.Test("SolvePart1Line", SolvePart1Line, part1Cases);
} else {
    Utils.AocMain(args, a => Solve(a, 1));
    // subtract one, as we already have the initial row
    Utils.AocMain(args, a => Solve(a, 9));
    Utils.AocMain(args, a => Solve(a, 99));
    Utils.AocMain(args, a => Solve(a, 999999));
}

record struct Galaxy(int Row, int Col);
