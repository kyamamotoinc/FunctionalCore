using FunctionalCore;
using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// ToResultのサンプル。
/// SomeをOkへ、Noneを指定したErrorを持つFailへ変換する。
/// </summary>
public static class Sample_14_ToResult
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 14 : ToResult ===");

        var some = GetFirstCharacter("Hello")
            .ToResult("文字がありません。");

        var none = GetFirstCharacter("")
            .ToResult("文字がありません。");

        some.Match(
            value => Console.WriteLine($"成功: {value}"),
            error => Console.WriteLine($"失敗: {error}"));

        none.Match(
            value => Console.WriteLine($"成功: {value}"),
            error => Console.WriteLine($"失敗: {error}"));

        Console.WriteLine();
    }

    private static Option<char> GetFirstCharacter(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Option<char>.None;

        return Option<char>.Some(text[0]);
    }
}