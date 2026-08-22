using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// BindAsyncのサンプル。
/// Optionを返す非同期処理同士を連結する。
/// </summary>
public static class Sample_20_BindAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Option Sample 20 : BindAsync ===");

        var result = await GetFirstCharacter("Hello")
            .AsTask()
            .BindAsync(ParseDigitAsync);

        result.Match(
            value => Console.WriteLine($"数値: {value}"),
            () => Console.WriteLine("値がありません。"));

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }

    private static async Task<Option<int>> ParseDigitAsync(char c)
    {
        await Task.Delay(100);

        if (!char.IsDigit(c))
            return Option<int>.None;

        return Option<int>.Some(c - '0');
    }
}