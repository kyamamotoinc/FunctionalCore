using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultToResultFromOptionTests
{
    /// <summary>
    /// 1. Some.ToResult(error) は Value を持つ Ok を返す
    /// </summary>
    [Test]
    public void Option_Some_ToResult_should_return_ok()
    {
        var option = Option<int>.Some(5);

        var result = option.ToResult<string, int>("error");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 2. None.ToResult(error) は指定した Error を持つ Fail を返す
    /// </summary>
    [Test]
    public void Option_None_ToResult_should_return_failure()
    {
        var option = Option<int>.None;

        var result = option.ToResult<string, int>("error");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 3. error が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_ToResult_null_error_should_throw()
    {
        var option = Option<int>.Some(5);
        string? error = null;

        Assert.Throws<ArgumentNullException>(() => option.ToResult<string, int>(error!));
    }
}