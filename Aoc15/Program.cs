using LibAoc;
using static LibAoc.LogUtils;

int SolvePart1(string line) {
    var steps = line.Split(",");
    return steps.Select(Hash).Sum();
}

int Hash(string input) {
    byte current = 0;
    foreach (var c in input) {
        current += (byte)c;
        current *= 17;
    }
    return current;
}

long SolvePart2(string line) {
    var boxes = new List<Lens>[256];
    for (var i = 0; i < 256; i++) boxes[i] = new List<Lens>();

    var steps = line.Split(",");
    foreach (var step in steps) {
        var (label, instr, focal) = ReadStep(step);
        var box = boxes[Hash(label)];
        if (instr == '-') {
            box.RemoveAll(lens => lens.Label == label);
        } else {
            var i = box.FindIndex(lens => lens.Label == label);
            if (i >= 0) {
                box[i] = new Lens(label, focal);
            } else {
                box.Add(new Lens(label, focal));
            }
        }
    }

    var result = 0L;
    for (var i = 0; i < 256; i++) {
        if (boxes[i].Any())
            Log("Box ", i, BoxToString(boxes[i]));
        var boxSum = boxes[i].Select((lens, j) => (j + 1) * lens.FocalLength).Sum();
        result += (i + 1) * boxSum;
    }

    return result;
}

string BoxToString(List<Lens> box) {
    return string.Join(", ", box.Select(lens => $"{lens.Label}={lens.FocalLength}"));
}

(string, char, int) ReadStep(string step) {
    if (step[^1] == '-') {
        return (step[0..^1], '-', 0);
    }
    var lens = int.Parse(step[^1..^0]);
    return (step[0..^2], '=', lens);
}

if (args.Length == 0) {
    EnableLogging = true;
    TestUtils.TestCase("Hash", Hash, "HASH", 52);
    TestUtils.TestCase("SolvePart1", SolvePart1,
            "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7",
            1320);
    TestUtils.TestCase("SolvePart2", SolvePart2,
            "rn=1,cm-,qp=3,cm=2,qp-,pc=4,ot=9,ab=5,pc-,pc=6,ot=7",
            145);
} else {
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart1));
    Utils.AocMain(args, Utils.SolveLineByLine(SolvePart2));
}

record struct Lens(string Label, int FocalLength);
