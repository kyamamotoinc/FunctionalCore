using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// EnsureAsyncのサンプル。
/// SomeのValueを非同期で検証し、条件を満たす場合だけ保持する。
/// </summary>
public static class Sample_24_EnsureAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Option Sample 24 : EnsureAsync ===");

        var result = await GetFirstCharacter("123")
            .AsTask()
            .EnsureAsync(IsLetterAsync);

        result.Match(
            value => Console.WriteLine($"結果: {value}"),
            () => Console.WriteLine("条件を満たしません。"));

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }

    private static async Task<bool> IsLetterAsync(char value)
    {
        await Task.Delay(100);

        return char.IsLetter(value);
    }
}