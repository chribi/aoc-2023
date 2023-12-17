using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(IEnumerable<string> line) {
    var map = line.ToArray();
    var distances = Djikstra(map, GetNeighborsPart1);

    var info = GetOptimalPath(distances, map.Length - 1, map[0].Length - 1);

    return info.Distance;
}

int SolvePart2(IEnumerable<string> line) {
    var map = line.ToArray();
    var distances = Djikstra(map, GetNeighborsPart2);

    var info = GetOptimalPath(distances, map.Length - 1, map[0].Length - 1);

    return info.Distance;
}

Info GetOptimalPath(Info[,,] distances, int row, int col) {
    var best = distances[row, col, 0];
    for (var s = 1; s < 4; s++) {
        if (distances[row, col, s].Distance < best.Distance) {
            best = distances[row, col, s];
        }
    }
    return best;
}

Info[,,] Djikstra(string[] map, Func<Node, int, int, IEnumerable<Node>> neighbors) {
    var rows = map.Length;
    var cols = map[0].Length;
    var dists = InitializeNodes(rows, cols);
    var queue = new List<(int, Node)>();

    var initialNodes = neighbors(new Node(0, 0, Direction.R), rows, cols)
        .Concat(neighbors(new Node(0, 0, Direction.D), rows, cols));

    var start = new Node(0, 0, Direction.R);
    foreach (var initial in initialNodes) {
        UpdateDistance(dists, queue, initial, Dist(map, start, initial), start);
    }

    var currentNode = Dequeue(queue);
    while (currentNode.Row >= 0) {
        var info = GetInfo(dists, currentNode);
        Log($"Visiting {currentNode.Row}, {currentNode.Column} {currentNode.LastDirection}, Distance {info.Distance}");

        if (currentNode.Row == rows - 1 && currentNode.Column == cols - 1) break;

        info.Final = true;

        foreach (var neighbor in neighbors(currentNode, rows, cols)) {
            var newDistance = info.Distance + Dist(map, currentNode, neighbor);
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
        Log($"\tUpdate {node.Row}, {node.Column} {node.LastDirection}: {info.Distance} > {newDistance}");
        info.Distance = newDistance;
        info.Predecessor = pred;
        Update(queue, node, newDistance);
    }
}

int Dist(string[] map, Node a, Node b) {
    var dr = b.Row == a.Row ? 0 : b.Row > a.Row ? 1 : -1;
    var dc = b.Column == a.Column ? 0 : b.Column > a.Column ? 1 : -1;
    var iMax = Math.Max(Math.Abs(b.Row - a.Row), Math.Abs(b.Column - a.Column));
    var dist = 0;
    for (var i = 1; i <= iMax; i++) {
        dist += Cost(map[a.Row + i * dr][a.Column + i * dc]);
    }
    return dist;
}

Node Dequeue(List<(int, Node)> queue) {
    if (!queue.Any()) return new Node(-1, -1, Direction.R);
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
    return dists[node.Row, node.Column, (int)node.LastDirection];
}

IEnumerable<Node> GetNeighborsPart1(Node node, int rows, int cols) =>
    GetNeighbors(node, rows, cols, 1, 3);

IEnumerable<Node> GetNeighborsPart2(Node node, int rows, int cols) =>
    GetNeighbors(node, rows, cols, 4, 10);

IEnumerable<Node> GetNeighbors(Node node, int rows, int cols, int minSteps, int maxSteps) {
    var r = node.Row;
    var c = node.Column;

    if (node.LastDirection == Direction.R || node.LastDirection == Direction.L) {
        for (var k = minSteps; k <= maxSteps; k++) {
            if (r + k < rows)
                yield return new Node(r + k, c, Direction.D);
            if (r - k >= 0)
                yield return new Node(r - k, c, Direction.U);
        }
    }

    if (node.LastDirection == Direction.U || node.LastDirection == Direction.D) {
        for (var k = minSteps; k <= maxSteps; k++) {
            if (c + k < cols)
                yield return new Node(r, c + k, Direction.R);
            if (c - k >= 0)
                yield return new Node(r, c - k, Direction.L);
        }
    }
}

Info[,,] InitializeNodes(int rows, int cols) {
    var dists = new Info[rows, cols, 4];
    for (var i = 0; i < rows; i++)
        for (var j = 0; j < cols; j++)
            for (var s = 0; s < 4; s++) {
                dists[i, j, s] = new Info();
            }
    return dists;
}

int Cost(char mapChar) {
    return (int)(mapChar - '0');
}

if (args.Length == 0) {
    EnableLogging = true;
    var testMap = new string[] {
        "13456",
        "23456",
        "34567",
        "54321",
    };
    int D((int, int, int, int) t) => Dist(testMap, new Node(t.Item1, t.Item2, Direction.R),
            new Node(t.Item3, t.Item4, Direction.R));
    TestUtils.TestCase("Dist", D, (0, 0, 0, 1), 3);
    TestUtils.TestCase("Dist", D, (0, 0, 1, 0), 2);
    TestUtils.TestCase("Dist", D, (0, 0, 3, 0), 10);
    TestUtils.TestCase("Dist", D, (0, 0, 0, 3), 12);
} else {
    Utils.AocMain(args, SolvePart1);
    Utils.AocMain(args, SolvePart2);
}

class Info {
    public Info() { }
    public int Distance { get; set; } = int.MaxValue;
    public Node Predecessor { get; set; } = new Node(-1, -1, Direction.R);
    public bool Final { get; set; } = false;
}

record struct Node(int Row, int Column, Direction LastDirection);
enum Direction { R, L, D, U };
