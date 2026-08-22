using FunctionalCore;
using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Combineのサンプル。
/// 2つのOptionがともにSomeの場合に値を組み合わせる。
/// </summary>
public static class Sample_08_Combine
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 08 : Combine ===");

        var width = CreatePositiveNumber(10);
        var height = CreatePositiveNumber(5);

        var result = width.Combine(height, (w, h) => w * h);

        result.Match(
            value => Console.WriteLine($"面積: {value}"),
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