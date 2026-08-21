using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// Orのサンプル。
/// Failの場合に代替Resultへ切り替える。
/// </summary>
public static class Sample_13_Or
{
    public static void Run()
    {
        Console.WriteLine("=== Sample 13 : Or ===");

        var result = Divide(10, 0)
            .Or(Divide(10, 2));

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