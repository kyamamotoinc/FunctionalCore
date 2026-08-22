using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// Recoverのサンプル。
/// Failの場合に代替値を生成し、Okへ回復する。
/// </summary>
public static class Sample_11_Recover
{
    public static void Run()
    {
        Console.WriteLine("=== Result Sample 11 : Recover ===");

        var result = Divide(10, 0)
            .Recover(_ => 0);

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