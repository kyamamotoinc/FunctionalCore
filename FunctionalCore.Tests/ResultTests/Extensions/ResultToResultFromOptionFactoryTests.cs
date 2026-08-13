using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultToResultFromOptionFactoryTests
{
    /// <summary>
    /// 1. Some.ToResult(errorFactory) は Value を持つ Ok を返す
    /// </summary>
    [Test]
    public void Option_Some_ToResult_factory_should_return_ok()
    {
        var option = Option<int>.Some(5);

        var result = option.ToResult<string, int>(() => "error");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 2. Some.ToResult(errorFactory) は errorFactory を実行しない
    /// </summary>
    [Test]
    public void Option_Some_ToResult_should_not_invoke_error_factory()
    {
        var option = Option<int>.Some(5);
        int count = 0;

        var result = option.ToResult<string, int>(() =>
        {
            count++;
            return "error";
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 3. None.ToResult(errorFactory) は errorFactory を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_None_ToResult_should_invoke_error_factory_once()
    {
        var option = Option<int>.None;
        int count = 0;

        var result = option.ToResult<string, int>(() =>
        {
            count++;
            return "generated error";
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("generated error"));
        });
    }

    /// <summary>
    /// 4. errorFactory が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_ToResult_null_error_factory_should_throw()
    {
        var option = Option<int>.None;
        Func<string>? errorFactory = null;

        Assert.Throws<ArgumentNullException>(() => option.ToResult<string, int>(errorFactory!));
    }

    /// <summary>
    /// 5. None.ToResult で errorFactory が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Option_None_ToResult_error_factory_returning_null_should_throw()
    {
        var option = Option<int>.None;

        Assert.Throws<InvalidOperationException>(() => option.ToResult<string, int>(() => null!));
    }

    /// <summary>
    /// 6. Some.ToResult では null を返す errorFactory でも実行されない
    /// </summary>
    [Test]
    public void Option_Some_ToResult_should_not_evaluate_null_returning_error_factory()
    {
        var option = Option<int>.Some(5);

        var result = option.ToResult<string, int>(() => null!);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }
}