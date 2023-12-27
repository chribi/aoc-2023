using System.Text;
using LibAoc;
using static LibAoc.LogUtils;

int Solve(IEnumerable<string> input) {
    var g = ReadGraph(input);
    LogGraph(g);
    var attempt = 0;
    while (true) {
        // Probabilistic algorithm, but we know minimum cut size
        // ==> repeat until cut with weight 3 is found
        var (cutWeight, cut) = KargersAlgorithm(g);
        attempt++;
        Log("Attempt:", attempt, "Cut weight", cutWeight);
        if (cutWeight == 3) {
            var total = g.GetLength(0);
            var part1 = cut.Count;
            var part2 = total - part1;
            Log("Parts", part1, part2);
            return part1 * part2;
        }
    }
}

(int CutWeight, List<int> Cut) KargersAlgorithm(int[,] graph) {
    var nodeCount = graph.GetLength(0);
    var edges = new List<(int, int)>();
    var rng = new Random();
    for (var i = 0; i < nodeCount; i++) {
        for (var j = i + 1; j < nodeCount; j++) {
            if (graph[i, j] != 0)
                edges.Add((i, j));
        }
    }
    var components = new List<List<int>>();
    for(var i = 0; i < nodeCount; i++) {
        components.Add(new List<int>() { i });
    }

    // i = node count of contracted graph
    for (var i = nodeCount; i > 2; i--) {
        var e = GetRandomEdge();
        Contract(e);
    }

    var weight = 0;
    foreach (var nodeFrom in components[0]) {
        foreach (var nodeTo in components[1]) {
            weight += graph[nodeFrom, nodeTo];
        }
    }

    return (weight, components[0]);

    (int, int) GetRandomEdge() {
        while (true) {
            var k = rng.Next(edges.Count);
            var (from, to) = edges[k];
            edges.RemoveAt(k);
            var iFrom = GetComponentIndex(from);
            if (!components[iFrom].Contains(to))
                return (from, to);
        }
    }

    int GetComponentIndex(int node) {
        return components.FindIndex(c => c.Contains(node));
    }

    void Contract((int, int) edge) {
        var i = GetComponentIndex(edge.Item1);
        var j = GetComponentIndex(edge.Item2);
        var ci = components[i];
        var cj = components[j];
        ci.AddRange(cj);
        components.RemoveAt(j);
    }
}

int[,] ReadGraph(IEnumerable<string> input) {
    var edges = new Dictionary<string, List<string>>();
    foreach (var line in input) {
        var words = line.Split(' ');
        var nodeName = words[0].Replace(":", "");
        foreach (var neighbor in words[1..]) {
            AddEdge(nodeName, neighbor, edges);
            AddEdge(neighbor, nodeName, edges);
        }
    }

    return ToAdjacencyMatrix(edges);
}

void LogGraph(int[,] graph) {
    if (!EnableLogging) return;
    for (var i = 0; i < graph.GetLength(0); i++) {
        var sb = new StringBuilder();
        sb.AppendFormat("{0:00}: ", i);
        for (var j = 0; j < graph.GetLength(1); j++) {
            sb.Append(i <= j ? graph[i, j].ToString() : " ");
            sb.Append(" ");
        }
        Console.WriteLine(sb);
    }
}

void AddEdge(string from, string to, Dictionary<string, List<string>> edges) {
    List<string>? neighbors;
    if (!edges.TryGetValue(from, out neighbors)) {
        neighbors = new List<string>();
        edges.Add(from, neighbors);
    }
    neighbors.Add(to);
}

int[,] ToAdjacencyMatrix(Dictionary<string, List<string>> adjacencyLists) {
    var nodes = adjacencyLists.ToList();
    var m = new int[nodes.Count, nodes.Count];

    for (var i = 0; i < nodes.Count; i++) {
        Log(i, nodes[i].Key);
        foreach (var neighbor in nodes[i].Value) {
            m[i, NameToIndex(neighbor)] = 1;
        }
    }

    int NameToIndex(string nodeName) {
        return nodes.FindIndex(pair => pair.Key == nodeName);
    }

    return m;
}



if (args.Length == 0) {
    EnableLogging = true;
} else {
    Utils.AocMain(args, Solve);
}
