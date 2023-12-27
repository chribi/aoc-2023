using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> line) {
    var map = line.ToArray();
    var width = map[0].Length;
    var height = map.Length;
    var startPos = (-1, -1);
    for (var row = 0; row < height; row++) {
        for (var col = 0; col < width; col++) {
            if (map[row][col] == 'S') {
                startPos = (row, col);
                break;
            }
        }
        if (startPos != (-1, -1)) break;
    }

    return CountReachable(map, 64, startPos);
}

int CountReachable(string[] map, int totalSteps, (int, int) startPos) {
    var width = map[0].Length;
    var height = map.Length;
    var steps = new int[height, width];
    for (var row = 0; row < height; row++) {
        for (var col = 0; col < width; col++) {
            steps[row, col] = (row, col) == startPos ? 0 : -1;
            if (map[row][col] == '#')
                steps[row, col] = -2;
        }
    }

    for (var s = 0; s < totalSteps; s++) {
        for (var row = 0; row < height; row++) {
            for (var col = 0; col < width; col++) {
                if (steps[row, col] != -1) continue;
                if ( (row > 0 && steps[row - 1, col] == s)
                        || (row < height - 1 && steps[row + 1, col] == s)
                        || (col > 0 && steps[row, col - 1] == s)
                        || (col < width - 1 && steps[row, col + 1] == s)
                   ) {
                    steps[row, col] = s + 1;
                }
            }
        }
    }

    var reachable = 0;
    var parity = totalSteps % 2;
    for (var row = 0; row < height; row++) {
        for (var col = 0; col < width; col++) {
            if (steps[row, col] >= 0 && steps[row, col] % 2 == parity)
                reachable++;
        }
    }
    return reachable;
}

if (args.Length == 0) {
    EnableLogging = true;
} else {
    Utils.AocMain(args, SolvePart1);
}
