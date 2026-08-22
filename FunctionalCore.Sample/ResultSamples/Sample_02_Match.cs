namespace FunctionalCore.Sample.ResultSamples
{
    /// <summary>
    /// Matchのサンプル。
    /// Resultの成功・失敗に応じて処理を分岐する。
    /// </summary>
    public static class Sample_02_Match
    {
        public static void Run()
        {
            Console.WriteLine("=== Result Sample 02 : Match ===");

            var success = Divide(10, 2);
            var failure = Divide(10, 0);

            success.Match(
                value => Console.WriteLine($"成功: {value}"),
                error => Console.WriteLine($"失敗: {error}"));

            failure.Match(
                value => Console.WriteLine($"成功: {value}"),
                error => Console.WriteLine($"失敗: {error}"));

            Console.WriteLine();
        }

        private static Result<string, int> Divide(int x, int y)
        {
            if (y == 0)
                return Result<string, int>.Fail("0では割れません。");

            return Result<string, int>.Ok(x / y);
        }
    }
}
