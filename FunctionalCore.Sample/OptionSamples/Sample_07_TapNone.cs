using FunctionalCore;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// TapNoneのサンプル。
/// Noneの場合だけ副作用を実行し、元のOptionをそのまま返す。
/// </summary>
public static class Sample_07_TapNone
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 07 : TapNone ===");

        var some = GetFirstCharacter("Hello")
            .TapNone(() => Console.WriteLine("値がありません。"));

        var none = GetFirstCharacter("")
            .TapNone(() => Console.WriteLine("値がありません。"));

        some.Match(
            value => Console.WriteLine($"最終結果: {value}"),
            () => Console.WriteLine("最終結果: None"));

        none.Match(
            value => Console.WriteLine($"最終結果: {value}"),
            () => Console.WriteLine("最終結果: None"));

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }
}