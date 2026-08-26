using FunctionalCore;

namespace FunctionalCore.Sample.PipelineSamples;

/// <summary>
/// Resultを使って複数の処理を1本のパイプラインとして連結するサンプル。
/// </summary>
public static class ResultPipeline
{
    public static void Run()
    {
        Console.WriteLine("=== Pipeline Sample : ResultPipeline ===");

        var result = ParseInt("10")
            .Ensure(x => x > 0, x => $"{x} は0より大きい必要があります。")
            .Bind(x => Divide(x, 2))
            .Map(x => x * 2)
            .Tap(x => Console.WriteLine($"途中結果: {x}"));

        result.Match(
            value => Console.WriteLine($"最終結果: {value}"),
            error => Console.WriteLine($"エラー: {error}"));

        Console.WriteLine();
    }

    private static Result<string, int> ParseInt(string text)
    {
        if (!int.TryParse(text, out var value))
            return Result<string, int>.Fail("整数に変換できません。");

        return Result<string, int>.Ok(value);
    }

    private static Result<string, int> Divide(int x, int y)
    {
        if (y == 0)
            return Result<string, int>.Fail("0では割れません。");

        return Result<string, int>.Ok(x / y);
    }
}