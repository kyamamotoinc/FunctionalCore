using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// MapAsyncのサンプル。
/// OkのValueを非同期で変換する。
/// </summary>
public static class Sample_21_MapAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Result Sample 21 : MapAsync ===");

        var result = await Divide(10, 2)
            .AsTask()
            .MapAsync(DoubleAsync);

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

    private static async Task<int> DoubleAsync(int value)
    {
        await Task.Delay(100);

        return value * 2;
    }
}