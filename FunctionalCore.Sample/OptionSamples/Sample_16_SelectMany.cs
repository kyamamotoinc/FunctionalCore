using FunctionalCore;
using FunctionalCore.Linq;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// SelectManyのサンプル。
/// Optionを返す処理をLINQクエリ構文で連結する。
/// </summary>
public static class Sample_16_SelectMany
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 16 : SelectMany ===");

        var result =
            from first in GetFirstCharacter("12")
            from number in ParseDigit(first)
            select number * 10;

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

    private static Option<int> ParseDigit(char c)
    {
        if (!char.IsDigit(c))
            return Option<int>.None;

        return Option<int>.Some(c - '0');
    }
}