namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// TapErrorのサンプル。
/// Failの場合だけ副作用を実行し、元のResultをそのまま返す。
/// </summary>
public static class Sample_07_TapError
{
    public static void Run()
    {
        Console.WriteLine("=== Result Sample 07 : TapError ===");

        var result = Divide(10, 0)
            .TapError(error => Console.WriteLine($"エラーを記録: {error}"));

        result.Match(
            value => Console.WriteLine($"結果: {value}"),
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