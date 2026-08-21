using FunctionalCore;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Ensureのサンプル。
/// Someの値が条件を満たす場合だけ保持する。
/// </summary>
public static class Sample_05_Ensure
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 05 : Ensure ===");

        var some = GetFirstCharacter("Hello")
            .Ensure(c => char.IsLetter(c));

        var none = GetFirstCharacter("123")
            .Ensure(c => char.IsLetter(c));

        some.Match(
            value => Console.WriteLine($"文字です: {value}"),
            () => Console.WriteLine("条件を満たしません。"));

        none.Match(
            value => Console.WriteLine($"文字です: {value}"),
            () => Console.WriteLine("条件を満たしません。"));

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }
}