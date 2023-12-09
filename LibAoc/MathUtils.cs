namespace LibAoc {
    public static class MathUtils {
        public static long Gcd(long a, long b) {
            while (b != 0) {
                var tmp = a;
                a = b;
                b = tmp % b;
            }
            return a;
        }

        public static long Gcd(IEnumerable<long> nums) {
            return nums.Aggregate(Gcd);
        }

        public static long Lcm(long a, long b) {
            var gcd = Gcd(a, b);
            return (a / gcd) * b;
        }

        public static long Lcm(IEnumerable<long> nums) {
            return nums.Aggregate(Lcm);
        }
    }
}
