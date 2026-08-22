namespace FunctionalCore.Sample.ResultSamples
{
    /// <summary>
    /// Bindのサンプル。
    /// Resultを返す処理同士を連結する。
    /// </summary>
    public static class Sample_04_Bind
    {
        public static void Run()
        {
            Console.WriteLine("=== Result Sample 04 : Bind ===");

            var result = ParseInt("10")
                .Bind(x => Divide(x, 2));

            result.Match(
                value => Console.WriteLine($"結果: {value}"),
                error => Console.WriteLine($"エラー: {error}"));

            Console.WriteLine();
        }

        private static Result<string, int> ParseInt(string text)
        {
            if (!int.TryParse(text, out var value))
                return Result<string, int>.Fail("整数に変換できません。");

            return Result<string, int>.Ok(value);
        }

        private static Result<string, int> Divide(int x, int y)
        {
            if (y == 0)
                return Result<string, int>.Fail("0では割れません。");

            return Result<string, int>.Ok(x / y);
        }
    }
}
