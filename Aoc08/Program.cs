using System.Text.RegularExpressions;
using LibAoc;
using static LibAoc.LogUtils;

(string Path, Dictionary<string, (string, string)> Nodes) ReadInput(IEnumerable<string> lines) {
    var path = lines.First();
    var nodes = new Dictionary<string, (string, string)>();

    var r = new Regex(@"(...) = \((...), (...)\)");
    foreach (var line in lines.Skip(2)) {
        var m = r.Match(line);
        var node = m.Groups[1].Value;
        var left = m.Groups[2].Value;
        var right = m.Groups[3].Value;
        nodes.Add(node, (left, right));
    }
    return (path, nodes);
}
long SolvePart1(IEnumerable<string> lines) {
    var (path, nodes) = ReadInput(lines);

    var steps = 0;
    var i = 0;
    var current = "AAA";
    while (current != "ZZZ") {
        current = (path[i] == 'L') ? nodes[current].Item1 : nodes[current].Item2;
        steps++;
        i++;
        if (i >= path.Length) i = 0;
        Log(steps, current);
    }

    return steps;
}

long SolvePart2(IEnumerable<string> lines) {
    var (path, nodes) = ReadInput(lines);

    // hack: only works because input is cyclic in a way that allows it
    var startNodes = nodes.Keys.Where(k => k[2] == 'A');
    var pathLengths = new List<long>();

    foreach (var node in startNodes) {
        var current = node;
        var steps = 0;
        var i = 0;
        while (current[2] != 'Z') {
            current = (path[i] == 'L') ? nodes[current].Item1 : nodes[current].Item2;
            steps++;
            i++;
            if (i >= path.Length) i = 0;
        }
        pathLengths.Add(steps);
        Log(node, steps, current, path.Length, i);
    }
    return MathUtils.Lcm(pathLengths);
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
