using FunctionalCore;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Matchのサンプル。
/// Some / None に応じて処理を分岐する。
/// </summary>
public static class Sample_02_Match
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 02 : Match ===");

        var some = GetFirstCharacter("Hello");
        var none = GetFirstCharacter("");

        some.Match(
            value => Console.WriteLine($"値があります: {value}"),
            () => Console.WriteLine("値がありません。"));

        none.Match(
            value => Console.WriteLine($"値があります: {value}"),
            () => Console.WriteLine("値がありません。"));

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }
}