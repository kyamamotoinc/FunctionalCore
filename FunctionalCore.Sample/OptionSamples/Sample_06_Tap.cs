using FunctionalCore;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Tapのサンプル。
/// Someの場合だけ副作用を実行し、元のOptionをそのまま返す。
/// </summary>
public static class Sample_06_Tap
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 06 : Tap ===");

        var some = GetFirstCharacter("Hello")
            .Tap(value => Console.WriteLine($"値を確認: {value}"));

        var none = GetFirstCharacter("")
            .Tap(value => Console.WriteLine($"値を確認: {value}"));

        some.Match(
            value => Console.WriteLine($"最終結果: {value}"),
            () => Console.WriteLine("値がありません。"));

        none.Match(
            value => Console.WriteLine($"最終結果: {value}"),
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