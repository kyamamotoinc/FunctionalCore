using FunctionalCore;
using FunctionalCore.Linq;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// Selectのサンプル。
/// Okの場合だけValueを変換する。
/// LINQクエリ構文のselectでも利用できる。
/// </summary>
public static class Sample_18_Select
{
    public static void Run()
    {
        Console.WriteLine("=== Result Sample 18 : Select ===");

        var result = Divide(10, 2)
            .Select(value => value * 2);

        result.Match(
            value => Console.WriteLine($"メソッド構文: {value}"),
            error => Console.WriteLine($"エラー: {error}"));

        var queryResult =
            from value in Divide(10, 2)
            select value * 2;

        queryResult.Match(
            value => Console.WriteLine($"クエリ構文: {value}"),
            error => Console.WriteLine($"エラー: {error}"));

        Console.WriteLine();
    }

    private static Result<string, int> Divide(int x, int y)
    {
        if (y == 0)
            return Result<string, int>.Fail("0では割れません。");

        return Result<string, int>.Ok(x / y);
    }
}