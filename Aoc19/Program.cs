using System.Text.RegularExpressions;
using LibAoc;
using static LibAoc.LogUtils;

long SolvePart1(IEnumerable<string> lines) {
    var input = Utils.SplitAtEmptyLines(lines).ToList();

    var workflows = input[0].Select(ReadWorkflow)
        .ToDictionary(w => w.Name, w => w);
    var parts = input[1].Select(ReadPart);

    return parts.Where(part => ApplyWorkflows(workflows, part)).Sum(PartValue);
}

long SolvePart2(IEnumerable<string> lines) {
    var input = Utils.SplitAtEmptyLines(lines).ToList();

    var workflows = input[0].Select(ReadWorkflow)
        .ToDictionary(w => w.Name, w => w);
    var paths = GetAccepted(workflows, workflows["in"]);
    return paths.Sum(GetPartOptions);
}

Workflow ReadWorkflow(string line) {
    var m = Regex.Match(line, @"(\w+)\{(.*)\}");
    var name = m.Groups[1].Value;
    var rules = m.Groups[2].Value.Split(",");
    return new(name, rules.Select(ReadRule).ToList());
}

Rule ReadRule(string rule) {
    var parts = rule.Split(':');
    var result = parts[^1];
    if (parts.Length == 1) return new Rule(new('Z', '<', 0), result);
    var n = ParseUtils.GetNumbers(parts[0])[0];
    var condition = new Condition(parts[0][0], parts[0][1], n);
    return new Rule(condition, result);
}

Part ReadPart(string line) {
    var nums = ParseUtils.GetNumbers(line);
    return new Part(nums[0], nums[1], nums[2], nums[3]);
}

long PartValue(Part part) => part.X + part.M + part.A + part.S;

bool ApplyWorkflows(Dictionary<string, Workflow> workflows, Part part) {
    var current = workflows["in"];
    while (true) {
        foreach (var rule in current.Rules) {
            if (CheckCondition(rule.Condition, part)) {
                if (rule.Result == "A") return true;
                if (rule.Result == "R") return false;
                current = workflows[rule.Result];
                break;
            }
        }
    }
}

bool CheckCondition(Condition cond, Part part) {
    var partValue = cond.Var switch {
        'x' => part.X, 'm' => part.M, 'a' => part.A, 's' => part.S, _ => -1
    };
    return cond.Comparison == '>' ? partValue > cond.Value : partValue < cond.Value;
}

Condition Negate(Condition cond) {
    if (cond.Comparison == '>') {
        return new(cond.Var, '<', cond.Value + 1);
    } else {
        return new(cond.Var, '>', cond.Value - 1);
    }
}

List<List<Condition>> GetAccepted(Dictionary<string, Workflow> workflows, Workflow current) {
    List<List<Condition>> results = new List<List<Condition>>();
    var previousRulesNeg = new List<Condition>();
    foreach (var rule in current.Rules) {
        List<List<Condition>> paths;
        if (rule.Result == "R" || rule.Result == "A") {
            paths = new List<List<Condition>>();
            if (rule.Result == "A")
                paths.Add(new List<Condition>());
        } else {
            paths = GetAccepted(workflows, workflows[rule.Result]);
        }
        foreach (var path in paths) {
            if (rule.Condition.Var != 'Z')
                path.Add(rule.Condition);
            path.AddRange(previousRulesNeg);
        }
        results.AddRange(paths);
        previousRulesNeg.Add(Negate(rule.Condition));
    }
    return results;
}

long GetPartOptions(List<Condition> path) {
    var x = GetVarOptions(path, 'x');
    var m = GetVarOptions(path, 'm');
    var a = GetVarOptions(path, 'a');
    var s = GetVarOptions(path, 's');
    return x * m * a * s;
}

long GetVarOptions(List<Condition> path, char var) {
    var rules = path.Where(rule => rule.Var == var);
    var (min, max) = (1L, 4000L);
    foreach (var rule in rules) {
        if (rule.Comparison == '>') {
            min = Math.Max(min, rule.Value + 1);
        } else {
            max = Math.Min(max, rule.Value - 1);
        }
    }
    return max - min + 1;
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

record struct Condition(char Var, char Comparison, long Value) {
    override public string ToString() => $"{Var} {Comparison} {Value}";
}
record struct Rule(Condition Condition, string Result) {
    override public string ToString() => $"{Condition} => {Result}";
};
record struct Workflow(string Name, List<Rule> Rules) {
    override public string ToString() => $"{Name}\n\t{string.Join("\n\t", Rules)}";
}
record struct Part(long X, long M, long A, long S);
