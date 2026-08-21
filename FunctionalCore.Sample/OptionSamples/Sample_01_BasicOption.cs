using FunctionalCore;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Optionの最も基本的な使い方。
/// 値が存在する場合はSome、存在しない場合はNoneで表す。
/// </summary>
public static class Sample_01_BasicOption
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 01 : Basic Option ===");

        var some = GetFirstCharacter("Hello");
        var none = GetFirstCharacter("");

        Console.WriteLine(some);
        Console.WriteLine(none);

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }
}