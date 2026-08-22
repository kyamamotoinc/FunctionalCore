using FunctionalCore.Extensions;

namespace FunctionalCore.Sample.ResultSamples;

/// <summary>
/// ValueOrThrowのサンプル。
/// Okの場合はValueを返し、Failの場合は指定した例外を投げる。
/// </summary>
public static class Sample_16_ValueOrThrow
{
    public static void Run()
    {
        Console.WriteLine("=== Result Sample 16 : ValueOrThrow ===");

        try
        {
            var value = Divide(10, 0)
                .ValueOrThrow(error => new InvalidOperationException(error));

            Console.WriteLine($"結果: {value}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"例外: {ex.Message}");
        }

        Console.WriteLine();
    }

    private static Result<string, int> Divide(int x, int y)
    {
        if (y == 0)
            return Result<string, int>.Fail("0では割れません。");

        return Result<string, int>.Ok(x / y);
    }
}