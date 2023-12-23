using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> lines) => Solve(lines, ignoreSlides: false);
int SolvePart2(IEnumerable<string> lines) => Solve(lines, ignoreSlides: true);

int Solve(IEnumerable<string> lines, bool ignoreSlides) {
    var map = lines.ToArray();
    var graph = BuildGraph(map, ignoreSlides);
    if (EnableLogging) {
        foreach (var kvp in graph) {
            Log(kvp.Key);
            Log("\t" + string.Join("\n\t", kvp.Value));
        }
    }

    var start = graph.Keys.First(point => point.Row == 0);
    var end = graph.Keys.First(point => point.Row == map.Length - 1);
    var maxLength = 0;
    Log("Node Count", graph.Count, "Segment Count", graph.Sum(e => e.Value.Count));

    maxLength = LongestPath(graph, start, end, new List<Point> { start });
    return maxLength;
}


int LongestPath(Dictionary<Point, List<Path>> graph, Point start, Point end, List<Point> ignore) {
    var max = 0;
    foreach (var next in graph[start]) {
        if (next.End == end) return next.Length;
        if (ignore.Contains(next.End)) continue;
        var newIgnore = new List<Point>(ignore);
        newIgnore.Add(next.End);
        var length = next.Length + LongestPath(graph, next.End, end, newIgnore);
        if (length > max) {
            max = length;
        }
    }
    return max;
}

// Build a graph of only the intersection points
Dictionary<Point, List<Path>> BuildGraph(string[] map, bool ignoreSlides = false) {
    var paths = new Dictionary<Point, List<Path>>();
    var start = new Point(0, map[0].ToList().FindIndex(c => c == '.'));
    var exploreQueue = new Queue<Point>();
    exploreQueue.Enqueue(start);

    while (exploreQueue.Any()) {
        var pt = exploreQueue.Dequeue();
        if (paths.ContainsKey(pt)) continue;
        var ptPaths = Explore(pt, map, ignoreSlides);
        paths.Add(pt, ptPaths);
        foreach (var endPoint in ptPaths.Select(path => path.End)) {
            exploreQueue.Enqueue(endPoint);
        }
    }

    return paths;
}

List<Path> Explore(Point start, string[] map, bool ignoreSlides) {
    return Neighbors(start, map)
        .Select((neighbor) => ExploreDirection(start, neighbor, map, ignoreSlides))
        .Where(path => path.HasValue)
        .Select(path => path!.Value)
        .ToList();
}

Path? ExploreDirection(Point start, Point direction, string[] map, bool ignoreSlides) {
    var from = start;
    var current = direction;
    var length = 1;
    while (true) {
        if (!ignoreSlides) {
            // slides don't occur on intersections or bends, so we can check here
            var currentTile = map[current.Row][current.Col];
            if (currentTile != '.') {
                var (dRow, dCol) = Slide(currentTile);
                if (current.Row - from.Row != dRow
                        || current.Col - from.Col != dCol) {
                    return null;
                }
            }
        }

        var next = Neighbors(current, map).Where(nb => nb != from).ToList();
        if (next.Count == 1) {
            from = current;
            current = next[0];
            length++;
        } else {
            return new Path(start, current, length);
        }
    }
}

(int, int) Slide(char c) {
    if (c == '>') return (0, 1);
    if (c == '<') return (0, -1);
    if (c == 'v') return (1, 0);
    if (c == '^') return (-1, 0);
    return (0, 0);
}

IEnumerable<Point> Neighbors(Point p, string[] map) {
    return NeighborPoints(p, map)
        .Where(pt => map[pt.Row][pt.Col] != '#');
}

IEnumerable<Point> NeighborPoints(Point p, string[] map) {
    var (row, col) = p;
    if (row > 0) yield return new(row - 1, col);
    if (row + 1 < map.Length) yield return new(row + 1, col);
    if (col > 0) yield return new(row, col - 1);
    if (col + 1 < map[0].Length) yield return new(row, col + 1);
}

if (args.Length == 0) {
    EnableLogging = true;
} else {
    Utils.AocMain(args, SolvePart1);
    Utils.AocMain(args, SolvePart2);
}

record struct Point(int Row, int Col) {
    public override string ToString() => $"({Row}, {Col})";
}
record struct Path(Point Start, Point End, int Length) {
    public override string ToString() => $"{Start} -[ {Length} ]-> {End}";
}

