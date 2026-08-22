using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// TapNoneAsyncのサンプル。
/// Noneの場合だけ非同期の副作用を実行し、元のOptionをそのまま返す。
/// </summary>
public static class Sample_22_TapNoneAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Option Sample 22 : TapNoneAsync ===");

        var result = await GetFirstCharacter("")
            .AsTask()
            .TapNoneAsync(LogNoneAsync);

        result.Match(
            value => Console.WriteLine($"最終結果: {value}"),
            () => Console.WriteLine("最終結果: None"));

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }

    private static async Task LogNoneAsync()
    {
        await Task.Delay(100);

        Console.WriteLine("値がないことを記録しました。");
    }
}