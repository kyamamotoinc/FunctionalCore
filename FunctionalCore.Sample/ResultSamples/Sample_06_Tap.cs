namespace FunctionalCore.Sample.ResultSamples
{
    /// <summary>
    /// Tapのサンプル。
    /// Okの場合だけ副作用を実行し、元のResultをそのまま返す。
    /// </summary>
    public static class Sample_06_Tap
    {
        public static void Run()
        {
            Console.WriteLine("=== Sample 06 : Tap ===");

            var result = Divide(10, 2)
                .Tap(value => Console.WriteLine($"成功値を確認: {value}"));

            result.Match(
                value => Console.WriteLine($"最終結果: {value}"),
                error => Console.WriteLine($"最終エラー: {error}"));

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
