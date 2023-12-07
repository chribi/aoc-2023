using LibAoc;
using static LibAoc.LogUtils;

long SolvePart1(IEnumerable<string> line) {
    var ls = line.ToArray();
    var seeds = ParseUtils.GetNumbers(ls[0]);

    for (var i = 2; i < ls.Length; i++) {
        Log(ls[i]);
        i++; // skip mapping header;

        var currentMap = new List<(Interval Source, long Delta)>();
        while (i < ls.Length && ls[i] != "") {
            currentMap.Add(ReadMap(ls[i]));
            i++;
        }

        for (var j = 0; j < seeds.Count; j++) {
            var s = seeds[j];
            var map = currentMap.FirstOrDefault(m => m.Source.Contains(s));
            if (map != default) {
                seeds[j] = s + map.Delta;
            }
        }
    }

    return seeds.Min();
}

long SolvePart2(IEnumerable<string> line) {
    var ls = line.ToArray();
    var seeds = ParseUtils.GetNumbers(ls[0]);
    var ranges = new List<Interval>();
    for (var n = 0; n < seeds.Count; n += 2) {
        ranges.Add(Interval.WithLength(seeds[n], seeds[n+1]));
    }

    for (var i = 2; i < ls.Length; i++) {
        Log("========");
        Log(ls[i]);
        i++; // skip mapping header;
        var mappings = new List<(Interval Source, long Delta)>();
        while (i < ls.Length && ls[i] != "") {
            mappings.Add(ReadMap(ls[i]));
            i++;
        }
        var nextRanges = new List<Interval>();
        foreach(var r in ranges) {
            Log(r);
            var intersections = new List<(Interval Mapped, long Delta)>();
            foreach (var map in mappings) {
                var intersection = map.Source.IntersectWith(r);
                if (intersection == null) continue;
                Log($"\tintersect with {map} => {intersection}");
                intersections.Add((intersection, map.Delta));
            }
            var k = r.Low;
            foreach (var s in intersections.OrderBy(map => map.Mapped.Low)) {
                Log($"\tinter {s}");
                if (s.Mapped.Low > k) {
                    nextRanges.Add(Interval.WithLength(k, s.Mapped.Low - k));
                    Log($"\t\tnext {nextRanges.Last()}");
                }
                nextRanges.Add(Interval.WithLength(s.Mapped.Low + s.Delta, s.Mapped.Length));
                Log($"\t\tnext {nextRanges.Last()}");
                k += s.Mapped.Length;
            }
            if (r.High > k) {
                nextRanges.Add(new Interval(k, r.High));
                Log($"\t\tnext {nextRanges.Last()}");
            }
        }

        ranges = nextRanges;
    }

    var closest = ranges.MinBy(r => r.Low);
    return closest!.Low;
}

(Interval Source, long Delta) ReadMap(string line) {
    var nums = ParseUtils.GetNumbers(line);
    return (Interval.WithLength(nums[1], nums[2]), nums[0] - nums[1]);
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
