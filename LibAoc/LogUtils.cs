namespace LibAoc {
    public static class LogUtils {
        public static bool EnableLogging = false;

        public static void Log(params object[] values) {
            if (!EnableLogging) return;
            Console.WriteLine(string.Join(" ", values));
        }
    }
}
