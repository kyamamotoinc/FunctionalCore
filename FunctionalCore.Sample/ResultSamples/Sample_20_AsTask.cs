using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// AsTaskのサンプル。
/// ResultをTask&lt;Result&lt;E, T&gt;&gt;へ変換し、非同期パイプラインへ入る。
/// </summary>
public static class Sample_20_AsTask
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Result Sample 20 : AsTask ===");

        var result = await Divide(10, 2)
            .AsTask();

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