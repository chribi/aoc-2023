using System.Text.RegularExpressions;

namespace LibAoc {
    public static class ParseUtils {
        public static List<long> GetNumbers(string line) {
            var nums = new Regex(@"-?\d+").Matches(line);
            return nums.Select(m => long.Parse(m.Value)).ToList();
        }
    }
}
