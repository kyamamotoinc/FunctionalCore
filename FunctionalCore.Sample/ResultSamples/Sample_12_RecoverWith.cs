using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// RecoverWithのサンプル。
/// Failの場合に別のResultを生成して回復する。
/// </summary>
public static class Sample_12_RecoverWith
{
    public static void Run()
    {
        Console.WriteLine("=== Result Sample 12 : RecoverWith ===");

        var result = Divide(10, 0)
            .RecoverWith(error => Divide(10, 2));

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