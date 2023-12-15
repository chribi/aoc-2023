namespace LibAoc {
    public static class TestUtils {
        public static void Test<TIn, TOut>(string name, Func<TIn, TOut> func,
                IEnumerable<(TIn Input, TOut Expected)> testCases) {
            foreach (var testCase in testCases) {
                TestCase(name, func, testCase.Input, testCase.Expected);
            }
        }

        public static void TestCase<TIn, TOut>(string name, Func<TIn, TOut> func,
                TIn input, TOut expected) {
            var result = func(input);
            if (Equals(result, expected)) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{name}: {input} => {result}");
            } else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{name}: {input} => {result}, expected {expected}");
            }
            Console.ResetColor();
        }
    }
}
