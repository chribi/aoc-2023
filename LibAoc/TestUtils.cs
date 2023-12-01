namespace LibAoc {
    public static class TestUtils {
        public static void Test<TIn, TOut>(string name, Func<TIn, TOut> func,
                IEnumerable<(TIn Input, TOut Expected)> testCases) {
            foreach (var testCase in testCases) {
                var result = func(testCase.Input);
                if (Equals(result, testCase.Expected)) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{name}: {testCase.Input} => {result}");
                } else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"{name}: {testCase.Input} => {result}, expected {testCase.Expected}");
                }
                Console.ResetColor();
            }
        }
    }
}
