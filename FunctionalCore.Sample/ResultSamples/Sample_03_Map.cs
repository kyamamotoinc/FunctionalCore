namespace FunctionalCore.Sample.ResultSamples
{
    /// <summary>
    /// Mapのサンプル。
    /// 成功値だけを変換する。
    /// </summary>
    public static class Sample_03_Map
    {
        public static void Run()
        {
            Console.WriteLine("=== Sample 02 : Map ===");

            var result = Divide(10, 2)
                .Map(x => x * 2);

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
