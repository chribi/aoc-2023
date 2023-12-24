using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> lines) {
    var stones = lines.Select(ReadLine).ToArray();
    var count = 0;
    for (var i = 0; i < stones.Length; i++)
        for (var j = i + 1; j < stones.Length; j++) {
            var intersection = IntersectXY(stones[i], stones[j]);
            if (intersection == null) continue;
            var (x, y) = intersection.Value;
            Log(x,y , stones[i], stones[j]);
            if (2e14 <= x && x <= 4e14 && 2e14 <= y && y <= 4e14)
                count++;
        }
    return count;
}

(double, double)? IntersectXY(Hailstone a, Hailstone b) {
    var d = Det(a.Vx, b.Vx, a.Vy, b.Vy);
    if (Math.Abs(d) < 1e-8) return null;
    var t1 = Det(b.X - a.X, b.Vx, b.Y - a.Y, b.Vy) / d;
    var t2 = Det(b.X - a.X, a.Vx, b.Y - a.Y, a.Vy) / d;
    if (t1 <= 0 || t2 <= 0) return null;
    return (a.X + t1 * a.Vx, a.Y + t1 * a.Vy);
}

double Det(double a, double b, double c, double d)
    => a * d - b * c;

Hailstone ReadLine(string line) {
    var nums = ParseUtils.GetNumbers(line);
    return new(nums[0], nums[1], nums[2], nums[3], nums[4], nums[5]);
}

if (args.Length == 0) {
    EnableLogging = true;
} else {
    Utils.AocMain(args, SolvePart1);
}

record struct Hailstone(long X, long Y, long Z,
        long Vx, long Vy, long Vz);
