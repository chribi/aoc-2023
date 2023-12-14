using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> lines) {
    var map = lines.Select(l => l.ToCharArray()).ToArray();

    RollNorth(map);

    return SumRockWeight(map);
}

int SolvePart2(IEnumerable<string> lines) {
    var map = lines.Select(l => l.ToCharArray()).ToArray();

    var hashes = new Dictionary<long, int>();
    var remaining = 0;
    var total = 1_000_000_000;
    for (var n = 1; n <= total; n++) {
        RollNorth(map);
        RollWest(map);
        RollSouth(map);
        RollEast(map);

        // cycle detection
        var hash = RockHash(map);
        if (hashes.TryGetValue(hash, out var hashN)) {
            var delta = hashN - n;
            remaining = (total - n) % delta;
            break;
        } else {
            hashes.Add(hash, n);
        }
    }

    for (var k = 0; k < remaining; k++) {
        RollNorth(map);
        RollWest(map);
        RollSouth(map);
        RollEast(map);
    }

    return SumRockWeight(map);
}

// Simple but good enough hash value over rock positions for cycle detection
long RockHash(char[][] map) {
    var hash = 0;
    for (var i = 0; i < map.Length; i++) {
        for (var j = 0; j < map[0].Length; j++) {
            if (map[i][j] == 'O')
                hash += 11 * i + 13 * j + 17 * i * j;
        }
    }
    return hash;
}

int SumRockWeight(char[][] map) {
    var height = map.Length;
    var weight = 0;
    for (var i = 0; i < map.Length; i++) {
        for (var j = 0; j < map[0].Length; j++) {
            if (map[i][j] == 'O')
                weight += height - i;
        }
    }

    return weight;
}

void RollNorth(char[][] map) {
    for (var j = 0; j < map[0].Length; j++) {
        var roundCount = 0;
        var lastSquare = 0;
        for (var i = 0; i < map.Length; i++) {
            if (map[i][j] == 'O') roundCount++;
            if (map[i][j] == '#') {
                for (var i2 = lastSquare; i2 < i; i2++) {
                    map[i2][j] = i2 - lastSquare < roundCount ? 'O' : '.';
                }
                lastSquare = i + 1;
                roundCount = 0;
            }

        }
        for (var i2 = lastSquare; i2 < map.Length; i2++) {
            map[i2][j] = i2 - lastSquare < roundCount ? 'O' : '.';
        }
    }
}

void RollSouth(char[][] map) {
    var width = map[0].Length;
    var height = map.Length;
    RollDirection(map, width, height,
            (i, j) => (height - i - 1, j));
}

void RollEast(char[][] map) {
    var width = map.Length;
    var height = map[0].Length;
    RollDirection(map, width, height,
            (i, j) => (j, height - i - 1));
}

void RollWest(char[][] map) {
    var width = map.Length;
    var height = map[0].Length;
    RollDirection(map, width, height,
            (i, j) => (j, i));
}

void RollDirection(char[][] map, int width, int height, Func<int, int, (int, int)> transform) {
    for (var j = 0; j < width; j++) {
        var roundCount = 0;
        var lastSquare = 0;
        for (var i = 0; i < height; i++) {
            var (ii, jj) = transform(i, j);
            if (map[ii][jj] == 'O') roundCount++;
            if (map[ii][jj] == '#') {
                for (var i2 = lastSquare; i2 < i; i2++) {
                    var (ii2, jj2) = transform(i2, j);
                    map[ii2][jj2] = i2 - lastSquare < roundCount ? 'O' : '.';
                }
                lastSquare = i + 1;
                roundCount = 0;
            }

        }
        for (var i2 = lastSquare; i2 < map.Length; i2++) {
            var (ii2, jj2) = transform(i2, j);
            map[ii2][jj2] = i2 - lastSquare < roundCount ? 'O' : '.';
        }
    }
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
