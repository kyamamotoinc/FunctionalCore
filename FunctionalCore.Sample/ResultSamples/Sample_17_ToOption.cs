using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// ToOptionのサンプル。
/// ResultをOptionへ変換する。
/// </summary>
public static class Sample_17_ToOption
{
    public static void Run()
    {
        Console.WriteLine("=== Sample 17 : ToOption ===");

        var success = Divide(10, 2)
            .ToOption();

        var failure = Divide(10, 0)
            .ToOption();

        success.Match(
            value => Console.WriteLine($"成功時: {value}"),
            () => Console.WriteLine("値がありません。"));

        failure.Match(
            value => Console.WriteLine($"成功時: {value}"),
            () => Console.WriteLine("値がありません。"));

        Console.WriteLine();
    }

    private static Result<string, int> Divide(int x, int y)
    {
        if (y == 0)
            return Result<string, int>.Fail("0では割れません。");

        return Result<string, int>.Ok(x / y);
    }
}