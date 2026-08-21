using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionValueOrThrowTests
{
    /// <summary>
    /// 1. Some.ValueOrThrow は内部の Value を返す
    /// </summary>
    [Test]
    public void Option_Some_ValueOrThrow_should_return_inner_value()
    {
        var some = Option<int>.Some(5);
        var value = some.ValueOrThrow(() => new InvalidOperationException());

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Some.ValueOrThrow は例外 factory を実行しない
    /// </summary>
    [Test]
    public void Option_Some_ValueOrThrow_should_not_invoke_exception_factory()
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
            Assert.That(count, Is.EqualTo(0));
            Assert.That(value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 3. None.ValueOrThrow は factory が生成した例外を投げる
    /// </summary>
    [Test]
    public void Option_None_ValueOrThrow_should_throw_factory_exception()
    {
        var none = Option<int>.None;
        Assert.Throws<InvalidOperationException>(() =>
            none.ValueOrThrow(() => new InvalidOperationException("error")));
    }

    /// <summary>
    /// 4. None.ValueOrThrow は例外 factory を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_None_ValueOrThrow_should_invoke_exception_factory_once()
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
    /// 5. None.ValueOrThrow は factory が生成した例外インスタンスをそのまま投げる
    /// </summary>
    [Test]
    public void Option_None_ValueOrThrow_should_throw_same_exception_instance()
    {
        var none = Option<int>.None;
        var expected = new InvalidOperationException("expected");

        var actual = Assert.Throws<InvalidOperationException>(() =>
            none.ValueOrThrow(() => expected));

        Assert.That(actual, Is.SameAs(expected));
    }

    /// <summary>
    /// 6. exception factory が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_ValueOrThrow_null_exception_factory_should_throw()
    {
        var none = Option<int>.None;
        Func<Exception>? factory = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.ValueOrThrow(factory!));
    }

    /// <summary>
    /// 7. Some でも exception factory が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Some_ValueOrThrow_null_exception_factory_should_throw()
    {
        var some = Option<int>.Some(5);
        Func<Exception>? factory = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.ValueOrThrow(factory!));
    }

    /// <summary>
    /// 8. None.ValueOrThrow で exception factory が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Option_None_ValueOrThrow_exception_factory_returning_null_should_throw()
    {
        var none = Option<int>.None;
        Assert.Throws<InvalidOperationException>(() =>
            none.ValueOrThrow(() => null!));
    }

    /// <summary>
    /// 9. Some.ValueOrThrow では null を返す exception factory でも実行されない
    /// </summary>
    [Test]
    public void Option_Some_ValueOrThrow_should_not_evaluate_null_returning_exception_factory()
    {
        var some = Option<int>.Some(5);
        var value = some.ValueOrThrow(() => null!);

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 10. Default Option は None と同様に factory が生成した例外を投げる
    /// </summary>
    [Test]
    public void Option_Default_ValueOrThrow_should_behave_as_none()
    {
        var option = default(Option<int>);

        Assert.Throws<InvalidOperationException>(() =>
            option.ValueOrThrow(() => new InvalidOperationException("error")));
    }

    /// <summary>
    /// 11. Default Option で exception factory が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Option_Default_ValueOrThrow_exception_factory_returning_null_should_throw()
    {
        var option = default(Option<int>);

        Assert.Throws<InvalidOperationException>(() =>
            option.ValueOrThrow(() => null!));
    }
}