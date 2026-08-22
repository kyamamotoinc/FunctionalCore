using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// MatchAsyncのサンプル。
/// Some / None に応じて非同期処理を分岐し、その戻り値を返す。
/// </summary>
public static class Sample_23_MatchAsync
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Option Sample 23 : MatchAsync ===");

        var message = await GetFirstCharacter("Hello")
            .AsTask()
            .MatchAsync(
                value => CreateSomeMessageAsync(value),
                CreateNoneMessageAsync);

        Console.WriteLine(message);
        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }

    private static async Task<string> CreateSomeMessageAsync(char value)
    {
        await Task.Delay(100);

        return $"値があります: {value}";
    }

    private static async Task<string> CreateNoneMessageAsync()
    {
        await Task.Delay(100);

        return "値がありません。";
    }
}