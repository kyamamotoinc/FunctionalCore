using FunctionalCore;
using FunctionalCore.Linq;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Selectのサンプル。
/// Someの場合だけValueを変換する。
/// LINQクエリ構文のselectでも利用できる。
/// </summary>
public static class Sample_15_Select
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 15 : Select ===");

        var option = GetFirstCharacter("Hello");

        var result = option.Select(c => char.ToLower(c));

        result.Match(
            value => Console.WriteLine($"メソッド構文: {value}"),
            () => Console.WriteLine("値がありません。"));

        var queryResult =
            from c in option
            select char.ToLower(c);

        queryResult.Match(
            value => Console.WriteLine($"クエリ構文: {value}"),
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