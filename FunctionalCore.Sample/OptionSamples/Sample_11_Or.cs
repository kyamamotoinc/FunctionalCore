using FunctionalCore;
using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Orのサンプル。
/// Noneの場合に代替Optionへ切り替える。
/// </summary>
public static class Sample_11_Or
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 11 : Or ===");

        var result = GetFirstCharacter("")
            .Or(() => GetFirstCharacter("Fallback"));

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
}