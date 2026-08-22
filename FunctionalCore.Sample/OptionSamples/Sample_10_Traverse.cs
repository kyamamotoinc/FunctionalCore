using FunctionalCore;
using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Traverseのサンプル。
/// 値の一覧をOptionへ変換しながら、1つのOptionにまとめる。
/// </summary>
public static class Sample_10_Traverse
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 10 : Traverse ===");

        var values = new[] { 10, 20, 30 };

        var result = values.Traverse(CreatePositiveNumber);

        result.Match(
            numbers => Console.WriteLine($"合計: {numbers.Sum()}"),
            () => Console.WriteLine("値がありません。"));

        Console.WriteLine();
    }

    private static Option<int> CreatePositiveNumber(int value)
    {
        if (value <= 0)
            return Option<int>.None;

        return Option<int>.Some(value);
    }
}