using FunctionalCore;
using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// ValueOrThrowのサンプル。
/// Someの場合はValueを返し、Noneの場合は指定した例外を投げる。
/// </summary>
public static class Sample_13_ValueOrThrow
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 13 : ValueOrThrow ===");

        try
        {
            var value = GetFirstCharacter("")
                .ValueOrThrow(() => new InvalidOperationException("値がありません。"));

            Console.WriteLine($"結果: {value}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"例外: {ex.Message}");
        }

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }
}