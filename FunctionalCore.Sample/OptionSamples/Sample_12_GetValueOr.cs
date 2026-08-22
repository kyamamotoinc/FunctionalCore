using FunctionalCore;
using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// GetValueOrのサンプル。
/// Someの場合はValueを返し、Noneの場合は代替値を返す。
/// </summary>
public static class Sample_12_GetValueOr
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 12 : GetValueOr ===");

        var someValue = GetFirstCharacter("Hello")
            .GetValueOr('?');

        var noneValue = GetFirstCharacter("")
            .GetValueOr('?');

        Console.WriteLine($"Someの場合: {someValue}");
        Console.WriteLine($"Noneの場合: {noneValue}");

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }
}