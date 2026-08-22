using FunctionalCore;
using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// AsTaskのサンプル。
/// OptionをTask&lt;Option&lt;T&gt;&gt;へ変換し、非同期パイプラインへ入る。
/// </summary>
public static class Sample_18_AsTask
{
    public static async Task RunAsync()
    {
        Console.WriteLine("=== Option Sample 18 : AsTask ===");

        var option = GetFirstCharacter("Hello");

        var result = await option.AsTask();

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