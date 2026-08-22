using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// BindAsyncのサンプル。
/// Resultを返す非同期処理同士を連結する。
/// </summary>
public static class Sample_22_BindAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Result Sample 22 : BindAsync ===");

        var result = await ParseInt("10")
            .AsTask()
            .BindAsync(value => DivideAsync(value, 2));

        result.Match(
            value => Console.WriteLine($"結果: {value}"),
            error => Console.WriteLine($"エラー: {error}"));

        Console.WriteLine();
    }

    private static Result<string, int> ParseInt(string text)
    {
        if (!int.TryParse(text, out var value))
            return Result<string, int>.Fail("整数に変換できません。");

        return Result<string, int>.Ok(value);
    }

    private static async Task<Result<string, int>> DivideAsync(int x, int y)
    {
        await Task.Delay(100);

        if (y == 0)
            return Result<string, int>.Fail("0では割れません。");

        return Result<string, int>.Ok(x / y);
    }
}