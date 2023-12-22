using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> lines) {
    var bricks = lines.Select(ReadBrick).OrderBy(Height).ToArray();
    // supports[i] = List of bricks supporting brick i
    Drop(bricks, out var supports);
    var canBeDisintegrated = new bool[bricks.Length];
    for (var i = 0; i < bricks.Length; i++) canBeDisintegrated[i] = true;
    foreach (var support in supports) {
        if (support.Count == 1) {
            canBeDisintegrated[support[0]] = false;
        }
    }

    return canBeDisintegrated.Count(x => x);
}

int SolvePart2(IEnumerable<string> lines) {
    var bricks = lines.Select(ReadBrick).OrderBy(Height).ToArray();
    Drop(bricks, out var supports);
    var supportedBy = SupportedBy(supports);

    var totalDropCount = 0;
    for (var i = 0; i < bricks.Length; i++) {
        totalDropCount += CalculateDropCount(i, supports, supportedBy);
    }

    return totalDropCount;
}

int CalculateDropCount(int i, List<int>[] supports, List<int>[] supportedBy) {
    var removed = new List<int>();
    var removeQueue = new Queue<int>();
    removeQueue.Enqueue(i);
    while (removeQueue.Any()) {
        var j = removeQueue.Dequeue();
        removed.Add(j);
        foreach (var s in supportedBy[j]) {
            if (supports[s].All(k => removed.Contains(k))) {
                removeQueue.Enqueue(s);
            }
        }
    }

    return removed.Count - 1;
}

// supportedBy[i] = bricks supported by brick i
List<int>[] SupportedBy(List<int>[] supports) {
    var result = new List<int>[supports.Length];
    for (var i = 0; i < result.Length; i++)
        result[i] = new List<int>();

    for (var j = 0; j < supports.Length; j++) {
        var support = supports[j];
        foreach (var i in support) {
            result[i].Add(j);
        }
    }
    return result;
}

void Drop(Brick[] bricks, out List<int>[] supports) {
    supports = new List<int>[bricks.Length];

    for (var i = 0; i < supports.Length; i++) {
        var brick = bricks[i];

        var supportBricks = Enumerable.Range(0, i)
            .Where(j => IntersectXY(brick, bricks[j]))
            .OrderByDescending(j => Top(bricks[j]))
            .ToList();
        if (supportBricks.Any()) {
            var supportTop = Top(bricks[supportBricks[0]]);
            var trueSupports = supportBricks.TakeWhile(
                    j => Top(bricks[j]) == supportTop).ToList();
            supports[i] = trueSupports;
            bricks[i] = DropBrick(brick, supportTop);
        } else {
            // drop to ground
            supports[i] = new List<int>();
            bricks[i] = DropBrick(brick, 0);
        }
    }
}

Brick DropBrick(Brick b, int supportHeight) {
    var newHeight = supportHeight + 1;
    var zLength = b.Z1 - b.Z0;
    return b with {
        Z0 = newHeight,
        Z1 = newHeight + zLength
    };
}

bool IntersectXY(Brick a, Brick b) {
    return IntersectAxis(a.X0, a.X1, b.X0, b.X1)
        && IntersectAxis(a.Y0, a.Y1, b.Y0, b.Y1);
}

bool IntersectAxis(int a0, int a1, int b0, int b1) {
    return (a0 <= b0 && b0 <= a1)
        || (b0 <= a0 && a0 <= b1);
}


Brick ReadBrick(string line) {
    var numbers = ParseUtils.GetNumbers(line);
    var xmin = (int) Math.Min(numbers[0], numbers[3]);
    var xmax = (int) Math.Max(numbers[0], numbers[3]);
    var ymin = (int) Math.Min(numbers[1], numbers[4]);
    var ymax = (int) Math.Max(numbers[1], numbers[4]);
    var zmin = (int) Math.Min(numbers[2], numbers[5]);
    var zmax = (int) Math.Max(numbers[2], numbers[5]);
    return new(xmin, ymin, zmin, xmax, ymax, zmax);
}

int Height(Brick b) => b.Z0;
int Top(Brick b) => b.Z1;

if (args.Length == 0) {
    EnableLogging = true;
} else {
    Utils.AocMain(args, SolvePart1);
    Utils.AocMain(args, SolvePart2);
}

record struct Brick(int X0, int Y0, int Z0, int X1, int Y1, int Z1);
