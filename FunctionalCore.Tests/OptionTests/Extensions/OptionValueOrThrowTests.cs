using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionValueOrThrowTests
{
    /// <summary>
    /// 1. OptionがSomeの場合は保持しているValueを返す。
    /// </summary>
    [Test]
    public void Some_ValueOrThrow_should_return_value()
    {
        var some = Option<int>.Some(5);

        var value = some.ValueOrThrow(() => new InvalidOperationException());

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. OptionがSomeの場合はexceptionFactoryを実行しない。
    /// </summary>
    [Test]
    public void Some_ValueOrThrow_should_not_invoke_exceptionFactory()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        var value = some.ValueOrThrow(() =>
        {
            count++;
            return new InvalidOperationException();
        });

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 3. OptionがNoneの場合はexceptionFactoryが生成した例外をスローする。
    /// </summary>
    [Test]
    public void None_ValueOrThrow_should_throw_exception_created_by_exceptionFactory()
    {
        var none = Option<int>.None;

        Assert.Throws<InvalidOperationException>(() =>
            none.ValueOrThrow(() => new InvalidOperationException("error")));
    }

    /// <summary>
    /// 4. OptionがNoneの場合はexceptionFactoryを1回だけ実行する。
    /// </summary>
    [Test]
    public void None_ValueOrThrow_should_invoke_exceptionFactory_once()
    {
        var none = Option<int>.None;
        int count = 0;

        Assert.Throws<InvalidOperationException>(() =>
            none.ValueOrThrow(() =>
            {
                count++;
                return new InvalidOperationException("error");
            }));

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. OptionがNoneの場合はexceptionFactoryが生成した例外インスタンスをそのままスローする。
    /// </summary>
    [Test]
    public void None_ValueOrThrow_should_throw_same_exception_instance_created_by_exceptionFactory()
    {
        var none = Option<int>.None;
        var expectedException = new InvalidOperationException("expected");

        var actualException = Assert.Throws<InvalidOperationException>(() =>
            none.ValueOrThrow(() => expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 6. OptionがNoneの場合でもexceptionFactoryがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_ValueOrThrow_should_throw_argument_null_exception_when_exceptionFactory_is_null()
    {
        var none = Option<int>.None;
        Func<Exception>? exceptionFactory = null;

        Assert.Throws<ArgumentNullException>(() => none.ValueOrThrow(exceptionFactory!));
    }

    /// <summary>
    /// 7. OptionがSomeの場合でもexceptionFactoryがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_ValueOrThrow_should_throw_argument_null_exception_when_exceptionFactory_is_null()
    {
        var some = Option<int>.Some(5);
        Func<Exception>? exceptionFactory = null;

        Assert.Throws<ArgumentNullException>(() => some.ValueOrThrow(exceptionFactory!));
    }

    /// <summary>
    /// 8. OptionがNoneでexceptionFactoryがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_ValueOrThrow_should_throw_invalid_operation_exception_when_exceptionFactory_returns_null()
    {
        var none = Option<int>.None;

        Assert.Throws<InvalidOperationException>(() =>
            none.ValueOrThrow(() => null!));
    }

    /// <summary>
    /// 9. OptionがSomeの場合はnullを返すexceptionFactoryでも実行せず、Valueを返す。
    /// </summary>
    [Test]
    public void Some_ValueOrThrow_should_return_value_without_invoking_null_returning_exceptionFactory()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        var value = some.ValueOrThrow(() =>
        {
            count++;
            return null!;
        });

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 10. default OptionはNoneと同様にexceptionFactoryが生成した例外をスローする。
    /// </summary>
    [Test]
    public void Default_ValueOrThrow_should_behave_as_none()
    {
        var defaultOption = default(Option<int>);

        Assert.Throws<InvalidOperationException>(() =>
            defaultOption.ValueOrThrow(() => new InvalidOperationException("error")));
    }

    /// <summary>
    /// 11. default OptionでexceptionFactoryがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_ValueOrThrow_should_throw_invalid_operation_exception_when_exceptionFactory_returns_null()
    {
        var defaultOption = default(Option<int>);

        Assert.Throws<InvalidOperationException>(() =>
            defaultOption.ValueOrThrow(() => null!));
    }

    /// <summary>
    /// 12. default Optionの場合でもexceptionFactoryがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_ValueOrThrow_should_throw_argument_null_exception_when_exceptionFactory_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<Exception>? exceptionFactory = null;

        Assert.Throws<ArgumentNullException>(() =>
            defaultOption.ValueOrThrow(exceptionFactory!));
    }

    /// <summary>
    /// 13. OptionがNoneでexceptionFactoryが例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void None_ValueOrThrow_should_propagate_exception_when_exceptionFactory_throws()
    {
        var none = Option<int>.None;
        var expectedException = new NotSupportedException("factory error");

        Func<Exception> exceptionFactory = () => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() =>
            none.ValueOrThrow(exceptionFactory));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}