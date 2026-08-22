using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// MatchAsyncのサンプル。
/// Ok / Fail に応じて非同期処理を分岐し、その戻り値を返す。
/// </summary>
public static class Sample_25_MatchAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Result Sample 25 : MatchAsync ===");

        var message = await Divide(10, 2)
            .AsTask()
            .MatchAsync(
                value => CreateSuccessMessageAsync(value),
                error => CreateFailureMessageAsync(error));

        Console.WriteLine(message);
        Console.WriteLine();
    }

    private static Result<string, int> Divide(int x, int y)
    {
        if (y == 0)
            return Result<string, int>.Fail("0では割れません。");

        return Result<string, int>.Ok(x / y);
    }

    private static async Task<string> CreateSuccessMessageAsync(int value)
    {
        await Task.Delay(100);

        return $"成功: {value}";
    }

    private static async Task<string> CreateFailureMessageAsync(string error)
    {
        await Task.Delay(100);

        return $"失敗: {error}";
    }
}