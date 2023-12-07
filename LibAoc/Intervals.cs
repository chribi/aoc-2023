namespace LibAoc {
    /// <summary>
    /// All intervals are [low, high)
    /// </summary>
    public class Interval {
        public long Low { get; }
        public long High { get; }
        public long Length => High - Low;
        public Interval(long low, long high) {
            Low = low;
            High = high;
        }

        public static Interval WithLength(long high, long length) {
            return new Interval(high, high + length);
        }

        public bool Contains(long number) => Low <= number && number < High;

        public bool Intersects(Interval other) {
            return IntersectWith(other) != null;
        }

        public Interval? IntersectWith(Interval other) {
            var low = Math.Max(Low, other.Low);
            var high = Math.Min(High, other.High);
            if (high <= low) return null;
            return new Interval(low, high);
        }

        public override string ToString()
        {
            return $"[{Low}, {High})";
        }
    }
}
