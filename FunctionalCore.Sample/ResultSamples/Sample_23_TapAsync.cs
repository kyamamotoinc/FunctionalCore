using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// TapAsyncのサンプル。
/// Okの場合だけ非同期の副作用を実行し、元のResultをそのまま返す。
/// </summary>
public static class Sample_23_TapAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Result Sample 23 : TapAsync ===");

        var result = await Divide(10, 2)
            .AsTask()
            .TapAsync(LogAsync);

        result.Match(
            value => Console.WriteLine($"最終結果: {value}"),
            error => Console.WriteLine($"エラー: {error}"));

        Console.WriteLine();
    }

    private static Result<string, int> Divide(int x, int y)
    {
        if (y == 0)
            return Result<string, int>.Fail("0では割れません。");

        return Result<string, int>.Ok(x / y);
    }

    private static async Task LogAsync(int value)
    {
        await Task.Delay(100);

        Console.WriteLine($"値を記録: {value}");
    }
}