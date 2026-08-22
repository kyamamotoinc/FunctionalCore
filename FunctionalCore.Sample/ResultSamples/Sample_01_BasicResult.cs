namespace FunctionalCore.Sample.ResultSamples
{
    /// <summary>
    /// Resultの最も基本的な使い方。
    /// 想定された失敗を例外ではなく戻り値として表す。
    /// </summary>
    public static class Sample_01_BasicResult
    {
        public static void Run()
        {
            Console.WriteLine("=== Result Sample 01 : Basic Result ===");

            var ok = Divide(10, 2);
            var fail = Divide(10, 0);

            Console.WriteLine(ok);
            Console.WriteLine(fail);
        }

        private static Result<string, int> Divide(int x, int y)
        {
            if (y == 0)
                return Result<string, int>.Fail("0では割れません。");

            return Result<string, int>.Ok(x / y);
        }
    }
}
