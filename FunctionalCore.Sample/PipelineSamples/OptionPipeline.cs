using FunctionalCore;

namespace FunctionalCore.Sample.PipelineSamples;

/// <summary>
/// Optionを使って複数の処理を1本のパイプラインとして連結するサンプル。
/// </summary>
public static class OptionPipeline
{
    public static void Run()
    {
        Console.WriteLine("=== Pipeline Sample : OptionPipeline ===");

        var result = GetFirstCharacter("123")
            .Bind(ParseDigit)
            .Ensure(x => x > 0)
            .Map(x => x * 2)
            .Tap(x => Console.WriteLine($"途中結果: {x}"));

        result.Match(
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

    private static Option<int> ParseDigit(char c)
    {
        if (!char.IsDigit(c))
            return Option<int>.None;

        return Option<int>.Some(c - '0');
    }
}