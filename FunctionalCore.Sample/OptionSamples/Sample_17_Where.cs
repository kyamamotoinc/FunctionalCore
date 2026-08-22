using FunctionalCore;
using FunctionalCore.Linq;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Whereのサンプル。
/// Someの値が条件を満たす場合だけ保持する。
/// LINQクエリ構文のwhereでも利用できる。
/// </summary>
public static class Sample_17_Where
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 17 : Where ===");

        var result =
            from c in GetFirstCharacter("Hello")
            where char.IsLetter(c)
            select c;

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
}