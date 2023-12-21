using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> line) {
    var map = line.ToArray();
    var width = map[0].Length;
    var height = map.Length;
    var steps = new int[height, width];
    for (var row = 0; row < height; row++) {
        for (var col = 0; col < width; col++) {
            steps[row, col] = map[row][col] == 'S' ? 0 : -1;
        }
    }

    var totalSteps = 64;

    for (var s = 0; s < totalSteps; s++) {
        for (var row = 0; row < height; row++) {
            for (var col = 0; col < width; col++) {
                if (map[row][col] == '#') continue;
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
    for (var row = 0; row < height; row++) {
        for (var col = 0; col < width; col++) {
            if (steps[row, col] == totalSteps)
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
