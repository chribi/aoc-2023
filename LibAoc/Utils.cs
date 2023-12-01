namespace LibAoc {
    public static class Utils {
        public static IEnumerable<string> ReadLines(string fileName) {
            using var f = File.OpenRead(fileName);
            using var sr = new StreamReader(f);

            string? line;
            while((line = sr.ReadLine()) != null) {
                yield return line;
            }
        }

        public static void AocMain<TResult>(string[] args, Func<IEnumerable<string>, TResult> solve) {
            if (args.Length == 0) {
                Console.WriteLine("No input file!");
                return;
            }

            var lines = ReadLines(args[0]);
            var result = solve(lines);
            Console.WriteLine($"Result: {result}");
        }

        public static void AocMain<TResult>(string[] args, Func<string, TResult> solve) {
            if (args.Length == 0) {
                Console.WriteLine("No input file!");
                return;
            }

            var text = File.ReadAllText(args[0]);
            var result = solve(text);
            Console.WriteLine($"Result: {result}");
        }

        public static Func<IEnumerable<string>, int> SolveLineByLine(Func<string, int> solveLine) {
            return lines => lines.Select(solveLine).Sum();
        }
    }
}
