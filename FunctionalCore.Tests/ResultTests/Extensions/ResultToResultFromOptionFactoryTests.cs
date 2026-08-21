using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultToResultFromOptionFactoryTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はValueを保持するOkを返す。
    /// </summary>
    [Test]
    public void Some_ToResult_factory_should_return_ok()
    {
        var some = Option<int>.Some(5);

        var result = some.ToResult<string, int>(() => "error");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 2. OptionがSomeの場合はerrorFactoryを実行しない。
    /// </summary>
    [Test]
    public void Some_ToResult_factory_should_not_invoke_errorFactory()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        var result = some.ToResult<string, int>(() =>
        {
            count++;
            return "error";
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 3. OptionがNoneの場合はerrorFactoryを1回だけ実行し、
    /// 生成されたErrorを保持するFailを返す。
    /// </summary>
    [Test]
    public void None_ToResult_factory_should_invoke_errorFactory_once_and_return_fail()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = none.ToResult<string, int>(() =>
        {
            count++;
            return "generated error";
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("generated error"));
            Assert.That(count, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 4. OptionがSomeの場合でもerrorFactoryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_ToResult_factory_should_throw_argument_null_exception_when_errorFactory_is_null()
    {
        var some = Option<int>.Some(5);
        Func<string>? errorFactory = null;

        Assert.Throws<ArgumentNullException>(() => some.ToResult<string, int>(errorFactory!));
    }

    /// <summary>
    /// 5. OptionがNoneの場合でもerrorFactoryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_ToResult_factory_should_throw_argument_null_exception_when_errorFactory_is_null()
    {
        var none = Option<int>.None;
        Func<string>? errorFactory = null;

        Assert.Throws<ArgumentNullException>(() => none.ToResult<string, int>(errorFactory!));
    }

    /// <summary>
    /// 6. OptionがNoneでerrorFactoryがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_ToResult_factory_should_throw_invalid_operation_exception_when_errorFactory_returns_null()
    {
        var none = Option<int>.None;

        Assert.Throws<InvalidOperationException>(() => none.ToResult<string, int>(() => null!));
    }

    /// <summary>
    /// 7. OptionがSomeの場合はnullを返すerrorFactoryでも実行せず、
    /// Valueを保持するOkを返す。
    /// </summary>
    [Test]
    public void Some_ToResult_factory_should_return_ok_without_invoking_null_returning_errorFactory()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        var result = some.ToResult<string, int>(() =>
        {
            count++;
            return null!;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(0));
        });
    }
}