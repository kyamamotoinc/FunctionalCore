using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// Combineのサンプル。
/// 2つのResultがともにOkの場合に値を組み合わせる。
/// </summary>
public static class Sample_08_Combine
{
    public static void Run()
    {
        Console.WriteLine("=== Sample 08 : Combine ===");

        var width = CreatePositiveNumber(10);
        var height = CreatePositiveNumber(5);

        var result = width.Combine(height, (w, h) => w * h);

        result.Match(
            value => Console.WriteLine($"面積: {value}"),
            error => Console.WriteLine($"エラー: {error}"));

        Console.WriteLine();
    }

    private static Result<string, int> CreatePositiveNumber(int value)
    {
        if (value <= 0)
            return Result<string, int>.Fail("0より大きい値を指定してください。");

        return Result<string, int>.Ok(value);
    }
}