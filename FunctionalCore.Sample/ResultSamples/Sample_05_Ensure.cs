namespace FunctionalCore.Sample.ResultSamples
{
    /// <summary>
    /// Ensureのサンプル。
    /// 成功値に追加条件を設定する。
    /// </summary>
    public static class Sample_05_Ensure
    {
        public static void Run()
        {
            Console.WriteLine("=== Result Sample 05 : Ensure ===");

            var result = Divide(4, 2)
                .Ensure(x => x >= 3, x => $"結果 {x} は3未満です。");

            result.Match(
                value => Console.WriteLine($"結果: {value}"),
                error => Console.WriteLine($"エラー: {error}"));

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
