using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// TapErrorAsyncのサンプル。
/// Failの場合だけ非同期の副作用を実行し、元のResultをそのまま返す。
/// </summary>
public static class Sample_24_TapErrorAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Result Sample 24 : TapErrorAsync ===");

        var result = await Divide(10, 0)
            .AsTask()
            .TapErrorAsync(LogErrorAsync);

        result.Match(
            value => Console.WriteLine($"最終結果: {value}"),
            error => Console.WriteLine($"最終エラー: {error}"));

        Console.WriteLine();
    }

    private static Result<string, int> Divide(int x, int y)
    {
        if (y == 0)
            return Result<string, int>.Fail("0では割れません。");

        return Result<string, int>.Ok(x / y);
    }

    private static async Task LogErrorAsync(string error)
    {
        await Task.Delay(100);

        Console.WriteLine($"エラーを記録: {error}");
    }
}