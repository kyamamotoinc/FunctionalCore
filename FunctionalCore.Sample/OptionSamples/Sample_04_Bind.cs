using FunctionalCore;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Bindのサンプル。
/// Optionを返す処理同士を連結する。
/// </summary>
public static class Sample_04_Bind
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 04 : Bind ===");

        var some = GetFirstCharacter("123")
            .Bind(ParseDigit);

        var none = GetFirstCharacter("")
            .Bind(ParseDigit);

        some.Match(
            value => Console.WriteLine($"数値: {value}"),
            () => Console.WriteLine("値がありません。"));

        none.Match(
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

    private static Option<int> ParseDigit(char c)
    {
        if (!char.IsDigit(c))
            return Option<int>.None;

        return Option<int>.Some(c - '0');
    }
}