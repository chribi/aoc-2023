using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> line) {
    var map = line.ToArray();
    var distances = Djikstra(map);

    var info = GetOptimalPath(distances, map.Length - 1, map[0].Length - 1);

    return info.Distance;
}

Info GetOptimalPath(Info[,,] distances, int row, int col) {
    var best = distances[row, col, 0];
    for (var s = 1; s < 12; s++) {
        if (distances[row, col, s].Distance < best.Distance) {
            best = distances[row, col, s];
        }
    }
    return best;
}

// We use Djikstra, but nodes are positions + last steps used to enter
Info[,,] Djikstra(string[] map) {
    var rows = map.Length;
    var cols = map[0].Length;
    var dists = InitializeNodes(rows, cols);
    var queue = new List<(int, Node)>();

    UpdateDistance(dists, queue, new Node(0, 1, LastSteps.R), Cost(map[0][1]), new Node(0, 0, LastSteps.D));
    UpdateDistance(dists, queue, new Node(1, 0, LastSteps.D), Cost(map[1][0]), new Node(0, 0, LastSteps.R));

    var currentNode = Dequeue(queue);
    while (currentNode.Row >= 0) {
        var info = GetInfo(dists, currentNode);
        Log($"Visiting {currentNode.Row}, {currentNode.Column} {currentNode.Steps}, Distance {info.Distance}");

        if (currentNode.Row == rows - 1 && currentNode.Column == cols - 1) break;

        info.Final = true;

        foreach (var neighbor in GetNeighbors(currentNode, rows, cols)) {
            var newDistance = info.Distance + Cost(map[neighbor.Row][neighbor.Column]);
            UpdateDistance(dists, queue, neighbor, newDistance, currentNode);
        }

        Log($"\tQueue size: {queue.Count}");
        currentNode = Dequeue(queue);
    }

    return dists;
}

void UpdateDistance(Info[,,] dists, List<(int, Node)> queue, Node node, int newDistance, Node pred) {
    var info = GetInfo(dists, node);
    if (info.Final) return;
    if (info.Distance > newDistance) {
        Log($"\tUpdate {node.Row}, {node.Column} {node.Steps}: {info.Distance} > {newDistance}");
        info.Distance = newDistance;
        info.Predecessor = pred;
        Update(queue, node, newDistance);
    }
}

Node Dequeue(List<(int, Node)> queue) {
    if (!queue.Any()) return new Node(-1, -1, LastSteps.R);
    var (_, best) = queue[0];
    queue.RemoveAt(0);
    return best;
}

void Update(List<(int, Node)> queue, Node node, int distance) {
    var i = queue.FindIndex(entry => entry.Item2 == node);
    if (i >= 0) queue.RemoveAt(i);
    var j = queue.FindIndex(entry => entry.Item1 > distance);
    if (j >= 0) {
        queue.Insert(j, (distance, node));
    } else {
        queue.Add((distance, node));
    }
}

Info GetInfo(Info[,,] dists, Node node) {
    return dists[node.Row, node.Column, (int)node.Steps];
}

IEnumerable<Node> GetNeighbors(Node node, int rows, int cols) {
    var r = node.Row;
    var c = node.Column;
    var s = node.Steps;
    var lastDirection = SingleStep(s);

    if (lastDirection == LastSteps.R || lastDirection == LastSteps.L) {
        if (r + 1 < rows)
            yield return new Node(r + 1, c, LastSteps.D);
        if (r - 1 >= 0)
            yield return new Node(r - 1, c, LastSteps.U);
    }

    if (lastDirection == LastSteps.U || lastDirection == LastSteps.D) {
        if (c + 1 < cols)
            yield return new Node(r, c + 1, LastSteps.R);
        if (c - 1 >= 0)
            yield return new Node(r, c - 1, LastSteps.L);
    }

    if ((s == LastSteps.D || s == LastSteps.DD) && r + 1 < rows)
            yield return new Node(r + 1, c, OneMore(s));

    if ((s == LastSteps.U || s == LastSteps.UU) && r - 1 >= 0)
            yield return new Node(r - 1, c, OneMore(s));

    if ((s == LastSteps.R || s == LastSteps.RR) && c + 1 < cols)
            yield return new Node(r, c + 1, OneMore(s));

    if ((s == LastSteps.L || s == LastSteps.LL) && c - 1 >= 0)
            yield return new Node(r, c - 1, OneMore(s));

    yield return new Node(1,1, LastSteps.R);
}

Info[,,] InitializeNodes(int rows, int cols) {
    var dists = new Info[rows, cols, 12];
    for (var i = 0; i < rows; i++)
        for (var j = 0; j < cols; j++)
            for (var s = 0; s < 12; s++) {
                dists[i, j, s] = new Info();
            }
    return dists;
}

int Cost(char mapChar) {
    return (int)(mapChar - '0');
}

LastSteps SingleStep(LastSteps steps) {
    return (LastSteps)(3 * ((int)steps / 3));
}

LastSteps OneMore(LastSteps steps) {
    return (LastSteps)((int)steps + 1);
}

if (args.Length == 0) {
    EnableLogging = true;
    TestUtils.TestCase("SingleStep", SingleStep, LastSteps.R, LastSteps.R);
    TestUtils.TestCase("SingleStep", SingleStep, LastSteps.D, LastSteps.D);
    TestUtils.TestCase("SingleStep", SingleStep, LastSteps.U, LastSteps.U);
    TestUtils.TestCase("SingleStep", SingleStep, LastSteps.L, LastSteps.L);
    TestUtils.TestCase("SingleStep", SingleStep, LastSteps.RRR, LastSteps.R);
    TestUtils.TestCase("SingleStep", SingleStep, LastSteps.DDD, LastSteps.D);
    TestUtils.TestCase("SingleStep", SingleStep, LastSteps.UUU, LastSteps.U);
    TestUtils.TestCase("SingleStep", SingleStep, LastSteps.LLL, LastSteps.L);
} else {
    Utils.AocMain(args, SolvePart1);
}

class Info {
    public Info() { }
    public int Distance { get; set; } = int.MaxValue;
    public Node Predecessor { get; set; } = new Node(-1, -1, LastSteps.R);
    public bool Final { get; set; } = false;
}

record struct Node(int Row, int Column, LastSteps Steps);
enum LastSteps { R, RR, RRR, D, DD, DDD, L, LL, LLL, U, UU, UUU };
