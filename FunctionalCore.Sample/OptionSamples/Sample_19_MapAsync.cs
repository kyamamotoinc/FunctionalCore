using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// MapAsyncのサンプル。
/// SomeのValueを非同期で変換する。
/// </summary>
public static class Sample_19_MapAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Option Sample 19 : MapAsync ===");

        var result = await GetFirstCharacter("Hello")
            .AsTask()
            .MapAsync(ToUpperAsync);

        result.Match(
            value => Console.WriteLine($"結果: {value}"),
            () => Console.WriteLine("値がありません。"));

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }

    private static async Task<char> ToUpperAsync(char c)
    {
        await Task.Delay(100);

        return char.ToUpper(c);
    }
}