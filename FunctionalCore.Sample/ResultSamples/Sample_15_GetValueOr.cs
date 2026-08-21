using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// GetValueOrのサンプル。
/// Okの場合はValueを返し、Failの場合は代替値を返す。
/// </summary>
public static class Sample_15_GetValueOr
{
    public static void Run()
    {
        Console.WriteLine("=== Sample 15 : GetValueOr ===");

        var successValue = Divide(10, 2)
            .GetValueOr(0);

        var failureValue = Divide(10, 0)
            .GetValueOr(0);

        Console.WriteLine($"成功時: {successValue}");
        Console.WriteLine($"失敗時: {failureValue}");

        Console.WriteLine();
    }

    private static Result<string, int> Divide(int x, int y)
    {
        if (y == 0)
            return Result<string, int>.Fail("0では割れません。");

        return Result<string, int>.Ok(x / y);
    }
}