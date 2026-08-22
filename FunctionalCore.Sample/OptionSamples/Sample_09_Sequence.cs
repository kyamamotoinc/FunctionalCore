using FunctionalCore;
using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.OptionSamples;

/// <summary>
/// Sequenceのサンプル。
/// 複数のOptionを1つのOptionにまとめる。
/// </summary>
public static class Sample_09_Sequence
{
    public static void Run()
    {
        Console.WriteLine("=== Option Sample 09 : Sequence ===");

        var options = new[]
        {
            CreatePositiveNumber(10),
            CreatePositiveNumber(20),
            CreatePositiveNumber(30)
        };

        var result = options.Sequence();

        result.Match(
            values => Console.WriteLine($"合計: {values.Sum()}"),
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