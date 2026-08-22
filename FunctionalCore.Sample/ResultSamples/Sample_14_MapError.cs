namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// MapErrorのサンプル。
/// Failの場合にErrorだけを変換する。
/// </summary>
public static class Sample_14_MapError
{
    public static void Run()
    {
        Console.WriteLine("=== Result Sample 14 : MapError ===");

        var result = Divide(10, 0)
            .MapError(error => $"計算エラー: {error}");

        result.Match(
            value => Console.WriteLine($"結果: {value}"),
            error => Console.WriteLine(error));

        Console.WriteLine();
    }

    private static Result<string, int> Divide(int x, int y)
    {
        if (y == 0)
            return Result<string, int>.Fail("0では割れません。");

        return Result<string, int>.Ok(x / y);
    }
}