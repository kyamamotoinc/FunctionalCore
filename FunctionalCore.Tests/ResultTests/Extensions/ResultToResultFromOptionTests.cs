using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultToResultFromOptionTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はValueを保持するOkを返す。
    /// </summary>
    [Test]
    public void Some_ToResult_should_return_ok()
    {
        var some = Option<int>.Some(5);

        var result = some.ToResult<string, int>("error");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 2. OptionがNoneの場合は指定されたErrorを保持するFailを返す。
    /// </summary>
    [Test]
    public void None_ToResult_should_return_fail()
    {
        var none = Option<int>.None;

        var result = none.ToResult<string, int>("error");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 3. OptionがSomeの場合でもerrorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_ToResult_should_throw_argument_null_exception_when_error_is_null()
    {
        var some = Option<int>.Some(5);
        string? error = null;

        Assert.Throws<ArgumentNullException>(() => some.ToResult<string, int>(error!));
    }

    /// <summary>
    /// 4. OptionがNoneの場合でもerrorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_ToResult_should_throw_argument_null_exception_when_error_is_null()
    {
        var none = Option<int>.None;
        string? error = null;

        Assert.Throws<ArgumentNullException>(() => none.ToResult<string, int>(error!));
    }
}