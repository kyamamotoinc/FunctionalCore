using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// Traverseのサンプル。
/// 値の一覧をResultへ変換しながら、1つのResultにまとめる。
/// </summary>
public static class Sample_10_Traverse
{
    public static void Run()
    {
        Console.WriteLine("=== Sample 10 : Traverse ===");

        var values = new[] { 10, 20, 30 };

        var result = values.Traverse(CreatePositiveNumber);

        result.Match(
            numbers => Console.WriteLine($"合計: {numbers.Sum()}"),
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