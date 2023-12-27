using LibAoc;
using static LibAoc.LogUtils;

long SolvePart1(IEnumerable<string> lines) {
    var map = lines.ToArray();
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
    Log(startPos);

    return CountReachable(map, 64, startPos);
}

/* Use some facts for the input of part 2:
 * - Start is in the center
 * - There is a clear path straight up/down/left/right from the center
 * - The border is also clear
 * - The map is square
 *
 * This means that for N = k*W + d steps, where W is the width of the map
 * - There are 2*k*(k-1) + 1 copies of the map we can reach completely,
 *   these split into k*k "odd" copies whee we need an odd number of steps to
 *   reach and k*k - 2*k + 1 "even" copies, where we need an even number of steps to reach
 * - There are k copies of the map where we can reach the top right corner
 *   in k*W + 1 steps, having then d - 1 steps to reach tiles on that copy of the map (same for other corners)
 * - There are k - 1 map copies where we reach the top right corner in
 *   (k - 1) * W + 1 steps, having W + d - 1 steps remaining
 *   (same for other corners)
 * - For each direction, when walking straight up/left/right/down, we reach
 *   a map copy after (k-1)*W + d + 1 steps in the middle of the
 *   bottom/right/left/top edge (as W = 2*d + 1 for the given input),
 *   leaving W-1 steps to explore that copy.
 *
 * As we use these assumptions, this will not work for the sample input.
 */
long SolvePart2(IEnumerable<string> lines) {
    var map = lines.ToArray();
    var w = map.Length;
    var n = 26501365L;
    var d = w / 2;
    var k = n / w;
    Log("n", n, "k", k, "d", d);
    var completeEven = CountReachable(map, w, (d, d));
    var totalEven = (k * k - 2 * k + 1) * completeEven;
    Log("even", completeEven, totalEven);
    var completeOdd = CountReachable(map, w + 1, (d, d));
    var totalOdd = k * k * completeOdd;
    Log("odd", completeOdd, totalOdd);
    var topLeft = CountReachable(map, d - 1, (0, 0));
    var topRight = CountReachable(map, d - 1, (0, w - 1));
    var bottomLeft = CountReachable(map, d - 1, (w - 1, 0));
    var bottomRight = CountReachable(map, d - 1, (w - 1, w - 1));
    Log("topLeft", topLeft);
    Log("topRight", topRight);
    Log("bottomLeft", bottomLeft);
    Log("bottomRight", bottomRight);
    var totalFromCorner = k * (topLeft + topRight + bottomLeft + bottomRight);
    Log("totalFromCorner", totalFromCorner);
    var topLeftLong = CountReachable(map, w + d - 1, (0, 0));
    var topRightLong = CountReachable(map, w + d - 1, (0, w - 1));
    var bottomLeftLong = CountReachable(map, w + d - 1, (w - 1, 0));
    var bottomRightLong = CountReachable(map, w + d - 1, (w - 1, w - 1));
    Log("topLeftLong", topLeftLong);
    Log("topRightLong", topRightLong);
    Log("bottomLeftLong", bottomLeftLong);
    Log("bottomRightLong", bottomRightLong);
    var totalFromCornerLong = (k - 1) * (topLeftLong + topRightLong
            + bottomLeftLong + bottomRightLong);
    Log("totalFromCornerLong", totalFromCornerLong);
    var top = CountReachable(map, w - 1, (0, d));
    var bottom = CountReachable(map, w - 1, (w - 1, d));
    var left = CountReachable(map, w - 1, (d, 0));
    var right = CountReachable(map, w - 1, (d, w - 1));

    Log("top", top);
    Log("bottom", bottom);
    Log("left", left);
    Log("right", right);

    return totalOdd + totalEven + totalFromCorner + totalFromCornerLong
        + top + bottom + left + right;
}

long CountReachable(string[] map, int totalSteps, (int, int) startPos) {
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

    var reachable = 0L;
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
    Utils.AocMain(args, SolvePart2);
}
