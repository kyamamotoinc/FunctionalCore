using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// EnsureAsyncのサンプル。
/// OkのValueを非同期で検証し、条件を満たさない場合はFailにする。
/// </summary>
public static class Sample_26_EnsureAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Result Sample 26 : EnsureAsync ===");

        var result = await Divide(10, 2)
            .AsTask()
            .EnsureAsync(
                IsLargeEnoughAsync,
                value => $"結果 {value} は3未満です。");

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

    private static async Task<bool> IsLargeEnoughAsync(int value)
    {
        await Task.Delay(100);

        return value >= 3;
    }
}