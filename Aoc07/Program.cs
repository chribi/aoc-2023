using LibAoc;
using static LibAoc.LogUtils;

long SolvePart1(IEnumerable<string> line) {
    return
    line.Select(ReadHand)
        .Order(new HandComparer())
        .Select((hand, index) => hand.Bid * (index + 1))
        .Sum();
}
long SolvePart2(IEnumerable<string> line) {
    return
    line.Select(ReadHandPart2)
        .Order(new HandComparer())
        .Select((hand, index) => hand.Bid * (index + 1))
        .Sum();
}

Hand ReadHand(string line) {
    var s = line.Split(" ");
    var hand = MakeComparable(s[0]);
    var kind = GetKind(hand);
    return new Hand(hand, kind, long.Parse(s[1]));
}

Hand ReadHandPart2(string line) {
    var s = line.Split(" ");
    var hand = MakeComparablePart2(s[0]);
    var kind = GetKindPart2(hand);
    return new Hand(hand, kind, long.Parse(s[1]));
}

Kind GetKind(string hand) {
    var counts = hand
        .GroupBy(c => c)
        .Select(g => g.Count())
        .OrderDescending()
        .ToList();
    if (counts[0] == 5) return Kind.Five;
    if (counts[0] == 4) return Kind.Four;
    if (counts[0] == 3) {
        return counts[1] == 2 ? Kind.FullHouse : Kind.Three;
    }
    if (counts[0] == 2)
        return counts[1] == 2 ? Kind.TwoPair : Kind.Pair;
    return Kind.High;
}

Kind GetKindPart2(string hand) {
    var jokers = hand.Count(c => c == '0');
    var otherHand = hand.Replace("0", "");
    if (string.IsNullOrEmpty(otherHand)) return Kind.Five;
    var counts = otherHand
        .GroupBy(c => c)
        .Select(g => g.Count())
        .OrderDescending()
        .ToList();
    counts[0] += jokers;
    if (counts[0] == 5) return Kind.Five;
    if (counts[0] == 4) return Kind.Four;
    if (counts[0] == 3) {
        return counts[1] == 2 ? Kind.FullHouse : Kind.Three;
    }
    if (counts[0] == 2)
        return counts[1] == 2 ? Kind.TwoPair : Kind.Pair;
    return Kind.High;
}

string MakeComparable(string cards) {
    return cards
        .Replace("T", "a")
        .Replace("J", "b")
        .Replace("Q", "c")
        .Replace("K", "d")
        .Replace("A", "e");
}

string MakeComparablePart2(string cards) {
    return cards
        .Replace("T", "a")
        .Replace("J", "0")
        .Replace("Q", "c")
        .Replace("K", "d")
        .Replace("A", "e");
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

class HandComparer : IComparer<Hand>
{
    public int Compare(Hand x, Hand y)
    {
        var compareKind =  x.Kind.CompareTo(y.Kind);
        if (compareKind != 0) return compareKind;
        return x.Cards.CompareTo(y.Cards);
    }
}

record struct Hand(string Cards, Kind Kind, long Bid);
enum Kind {
    High,
    Pair,
    TwoPair,
    Three,
    FullHouse,
    Four,
    Five,
}
