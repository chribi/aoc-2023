using LibAoc;
using static LibAoc.LogUtils;

long SolvePart1(IEnumerable<string> lines) {
    var modules = ReadModules(lines);
    var (low, high) = CountPulses(modules, 1000);
    Log("Low:", low, "High:", high);
    return low * high;
}

long SolvePart2(IEnumerable<string> lines) {
    var modules = ReadModules(lines);
    // Each output of the broadcaster sends to a closed submachine, that gets
    // joined again via &gf, which only triggers rx
    // Calculate cycle lengths for each submachine, then use LCM to figure
    // out when each submachine simultanously fires a 1 at &gf
    var submachines = Decompose(modules);
    var cycleTimes = new List<long>();
    foreach (var (submachine, terminal) in submachines) {
        Log($"===============\nSubmachine for {terminal}");
        foreach (var module in submachine.Values) Log(module);
        // structure of input allows simpler calculation, as
        // the high pulse to gf is always only pulsed once and at the last step
        // of the cycle
        var (from, to, _) = FindCycle(submachine);
        cycleTimes.Add(to - from);
    }
    return MathUtils.Lcm(cycleTimes);
}

List<(Dictionary<string, Module>, string)> Decompose(Dictionary<string, Module> modules) {
    var result = new List<(Dictionary<string, Module>, string)>();
    foreach (var broadcastTarget in modules["broadcaster"].Targets) {
        var submachine = BuildSubmachine(broadcastTarget, modules);
        result.Add(submachine);
    }
    return result;
}

(Dictionary<string, Module>, string) BuildSubmachine(string initial, Dictionary<string, Module> modules) {
    string? terminal = null;
    var q = new Queue<string>();
    q.Enqueue(initial);

    var submachine = new Dictionary<string, Module>();
    submachine.Add("broadcaster",
            new Module("broadcaster", new[] { initial }, ModuleType.Broadcast));
    while (q.Any()) {
        var name = q.Dequeue();
        if (submachine.ContainsKey(name)) continue;
        var module = modules[name];
        submachine.Add(name, module);
        // &gf node joining all the submachines together
        if (module.Targets.All(target => target == "gf")) {
            terminal = name;
            continue;
        }

        foreach (var target in module.Targets) {
            if (!submachine.ContainsKey(target))
                q.Enqueue(target);
        }
    }
    if (terminal == null) throw new Exception($"No terminal for {initial}");
    return (submachine, terminal);
}


Dictionary<string, Module> ReadModules(IEnumerable<string> lines) {
    var modules = lines.Select(ReadModule)
        .ToDictionary(m => m.Name, m => m);

    // connect inputs
    foreach (var module in modules.Values) {
        foreach (var target in module.Targets) {
            if (modules.TryGetValue(target, out var targetModule)) {
                targetModule.Inputs.Add(module.Name);
            }
        }
    }

    return modules;
}

Module ReadModule(string line) {
    var parts = line.Split("->");
    var targets = parts[1].Split(",").Select(s => s.Trim());
    var name = parts[0];
    var type = ModuleType.Broadcast;
    if (name[0] == '&') {
        name = name[1..];
        type = ModuleType.Conjunction;
    } else if (name[0] == '%') {
        name = name[1..];
        type = ModuleType.FlipFlop;
    }
    return new Module(name.Trim(), targets, type);
}

ulong GetState(Dictionary<string, Module> modules) {
    var state = 0UL;
    foreach (var module in modules.Values) {
        state <<= 1;
        if (module.State) {
            state |= 1;
        }
    }
    return state;
}

(int, int, List<int>) FindCycle(Dictionary<string, Module> modules) {
    var states = new Dictionary<ulong, int>();
    states.Add(0, 0);
    var terminalPulses = new List<int>();
    for (var n = 1; ; n++) {
        var gfPulsedHigh = PressButton2(modules);
        if (gfPulsedHigh)
            terminalPulses.Add(n);
        var state = GetState(modules);
        if (states.ContainsKey(state)) {
            var last = states[state];
            Log("Cycle detected:", state, "Cycle:", last, n, "Highs:",
                    string.Join(", ", terminalPulses));
            return (last, n, terminalPulses);
        } else {
            states.Add(state, n);
        }
    }
}

(long Low, long High) CountPulses(Dictionary<string, Module> modules, int buttonPresses) {
    var lowPulses = 0L;
    var highPulses = 0L;
    for (var n = 1; n <= buttonPresses; n++) {
        Log($"=================\nButton Press {n}");
        var (low, high) = PressButton(modules);
        lowPulses += low;
        highPulses += high;
    }

    return (lowPulses, highPulses);
}

bool PressButton2(Dictionary<string, Module> modules) {
    var queue = new Queue<(string, string, bool)>();
    queue.Enqueue(("button", "broadcaster", false));

    bool gfPulsedHigh = false;
    while (queue.Any()) {
        var (sender, target, pulse) = queue.Dequeue();
        if (target == "gf" && pulse) {
            gfPulsedHigh = true;
        }
        if (modules.TryGetValue(target, out var module)) {
            var send = false;
            if (module.Type == ModuleType.Broadcast) {
                send = pulse;
                module.State = pulse;
            } else if (module.Type == ModuleType.FlipFlop) {
                if (pulse) continue;
                module.State = !module.State;
                send = module.State;
            } else {
                send = !module.Inputs.All(input => modules[input].State);
                module.State = send;
            }

            foreach (var targetName in module.Targets) {
                queue.Enqueue((module.Name, targetName, send));
            }
        }
    }

    return gfPulsedHigh;
}

(long Low, long High) PressButton(Dictionary<string, Module> modules) {
    var (low, high) = (0L, 0L);
    var queue = new Queue<(string, string, bool)>();
    queue.Enqueue(("button", "broadcaster", false));

    while (queue.Any()) {
        var (sender, target, pulse) = queue.Dequeue();
        Log($"{sender} -[ {(pulse ? 1 : 0)} ]-> {target}");
        if (pulse) high++; else low++;
        if (modules.TryGetValue(target, out var module)) {
            var send = false;
            if (module.Type == ModuleType.Broadcast) {
                send = pulse;
                module.State = pulse;
            } else if (module.Type == ModuleType.FlipFlop) {
                if (pulse) continue;
                module.State = !module.State;
                send = module.State;
            } else {
                send = !module.Inputs.All(input => modules[input].State);
                module.State = send;
            }

            foreach (var targetName in module.Targets) {
                queue.Enqueue((module.Name, targetName, send));
            }
        }
    }

    return (low, high);
}

if (args.Length == 0) {
    EnableLogging = true;
} else {
    Utils.AocMain(args, SolvePart1);
    Utils.AocMain(args, SolvePart2);
}

enum ModuleType { Broadcast, FlipFlop, Conjunction }
class Module {
    public Module(string name, IEnumerable<string> targets, ModuleType type) {
        Name = name;
        Targets = targets.ToList();
        Inputs = new List<string>();
        Type = type;
    }

    public string Name { get; }
    public bool State { get; set; }
    public List<string> Targets { get; }
    public List<string> Inputs { get; }
    public ModuleType Type { get; }
    public override string ToString() {
        return $"{Type}: {Name} -> {string.Join(", ", Targets)}";
    }
}

