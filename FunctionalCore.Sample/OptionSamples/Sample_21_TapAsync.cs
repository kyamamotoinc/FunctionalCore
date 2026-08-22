using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// TapAsyncのサンプル。
/// Someの場合だけ非同期の副作用を実行し、元のOptionをそのまま返す。
/// </summary>
public static class Sample_21_TapAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Option Sample 21 : TapAsync ===");

        var result = await GetFirstCharacter("Hello")
            .AsTask()
            .TapAsync(LogAsync);

        result.Match(
            value => Console.WriteLine($"最終結果: {value}"),
            () => Console.WriteLine("値がありません。"));

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }

    private static async Task LogAsync(char value)
    {
        await Task.Delay(100);

        Console.WriteLine($"値を記録: {value}");
    }
}