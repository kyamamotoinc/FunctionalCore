namespace FunctionalCore.Sample.ResultSamples
{
    /// <summary>
    /// Tap / TapErrorのサンプル。
    /// Resultの内容を変更せず、副作用だけを実行する。
    /// </summary>
    public static class Sample_06_Tap
    {
        public static void Run()
        {
            Console.WriteLine("=== Sample 05 : Tap ===");

            var result = Divide(10, 2)
                .Tap(value => Console.WriteLine($"成功値を確認: {value}"))
                .TapError(error => Console.WriteLine($"エラーを確認: {error}"))
                .Map(value => value * 2);

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
