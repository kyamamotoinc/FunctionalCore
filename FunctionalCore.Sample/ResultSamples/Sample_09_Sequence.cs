using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// Sequenceのサンプル。
/// 複数のResultを1つのResultにまとめる。
/// </summary>
public static class Sample_09_Sequence
{
    public static void Run()
    {
        Console.WriteLine("=== Result Sample 09 : Sequence ===");

        var results = new[]
        {
            CreatePositiveNumber(10),
            CreatePositiveNumber(20),
            CreatePositiveNumber(30)
        };

        var result = results.Sequence();

        result.Match(
            values => Console.WriteLine($"合計: {values.Sum()}"),
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