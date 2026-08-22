using FunctionalCore;
using FunctionalCore.Linq;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// SelectManyのサンプル。
/// Resultを返す処理をLINQクエリ構文で連結する。
/// </summary>
public static class Sample_19_SelectMany
{
    public static void Run()
    {
        Console.WriteLine("=== Result Sample 19 : SelectMany ===");

        var result =
            from x in ParseInt("10")
            from y in Divide(x, 2)
            select y * 3;

        result.Match(
            value => Console.WriteLine($"結果: {value}"),
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