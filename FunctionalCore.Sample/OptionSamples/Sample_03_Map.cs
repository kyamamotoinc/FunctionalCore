using FunctionalCore;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Mapのサンプル。
/// Someの場合だけValueを変換する。
/// </summary>
public static class Sample_03_Map
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 03 : Map ===");

        var some = GetFirstCharacter("Hello")
            .Map(c => char.ToUpper(c));

        var none = GetFirstCharacter("")
            .Map(c => char.ToUpper(c));

        some.Match(
            value => Console.WriteLine($"変換後: {value}"),
            () => Console.WriteLine("値がありません。"));

        none.Match(
            value => Console.WriteLine($"変換後: {value}"),
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