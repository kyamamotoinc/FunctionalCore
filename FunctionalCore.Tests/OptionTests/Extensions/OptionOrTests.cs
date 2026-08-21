using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionOrTests
{
    /// <summary>
    /// 1. Some.Or は自身を返す
    /// </summary>
    [Test]
    public void Option_Some_Or_should_return_original_option()
    {
        var some = Option<int>.Some(5);
        var other = Option<int>.Some(10);

        var result = some.Or(other);

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 2. None.Or は代替 Option を返す
    /// </summary>
    [Test]
    public void Option_None_Or_should_return_other_option()
    {
        var none = Option<int>.None;
        var other = Option<int>.Some(10);

        var result = none.Or(other);

        Assert.That(result, Is.EqualTo(other));
    }

    /// <summary>
    /// 3. None.Or に None を渡した場合は None を返す
    /// </summary>
    [Test]
    public void Option_None_Or_none_should_return_none()
    {
        var none = Option<int>.None;
        var result = none.Or(Option<int>.None);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 4. Some.Or は代替 Option を採用しない
    /// </summary>
    [Test]
    public void Option_Some_Or_should_ignore_other_option()
    {
        var some = Option<int>.Some(5);
        var result = some.Or(Option<int>.Some(10));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 5. Some.Or に Default Option を渡しても自身を返す
    /// </summary>
    [Test]
    public void Option_Some_Or_default_should_return_original_option()
    {
        var some = Option<int>.Some(5);
        var other = default(Option<int>);

        var result = some.Or(other);

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 6. None.Or に Default Option を渡した場合は None を返す
    /// </summary>
    [Test]
    public void Option_None_Or_default_should_return_none()
    {
        var none = Option<int>.None;
        var other = default(Option<int>);

        var result = none.Or(other);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 7. Some.Or(Func) は factory を実行しない
    /// </summary>
    [Test]
    public void Option_Some_Or_factory_should_not_be_invoked()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        var result = some.Or(() =>
        {
            count++;
            return Option<int>.Some(10);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(some));
        });
    }

    /// <summary>
    /// 8. None.Or(Func) は factory を1回だけ実行し、その Option を返す
    /// </summary>
    [Test]
    public void Option_None_Or_factory_should_be_invoked_once()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = none.Or(() =>
        {
            count++;
            return Option<int>.Some(10);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(10));
        });
    }

    /// <summary>
    /// 9. None.Or(Func) の factory が None を返した場合は None を返す
    /// </summary>
    [Test]
    public void Option_None_Or_factory_returning_none_should_return_none()
    {
        var none = Option<int>.None;
        var result = none.Or(() => Option<int>.None);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 10. None.Or(Func) の factory が Default Option を返した場合は None を返す
    /// </summary>
    [Test]
    public void Option_None_Or_factory_returning_default_should_return_none()
    {
        var none = Option<int>.None;
        var result = none.Or(() => default(Option<int>));

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 11. factory が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Or_null_factory_should_throw()
    {
        var none = Option<int>.None;
        Func<Option<int>>? factory = null;

        Assert.Throws<ArgumentNullException>(() => none.Or(factory!));
    }

    /// <summary>
    /// 12. Some でも factory が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Some_Or_null_factory_should_throw()
    {
        var some = Option<int>.Some(5);
        Func<Option<int>>? factory = null;

        Assert.Throws<ArgumentNullException>(() => some.Or(factory!));
    }

    /// <summary>
    /// 13. Default Option.Or は None と同様に代替 Option を返す
    /// </summary>
    [Test]
    public void Option_Default_Or_should_return_other_option()
    {
        var option = default(Option<int>);
        var other = Option<int>.Some(10);

        var result = option.Or(other);

        Assert.That(result, Is.EqualTo(other));
    }

    /// <summary>
    /// 14. Default Option.Or(Func) は None と同様に factory を実行する
    /// </summary>
    [Test]
    public void Option_Default_Or_factory_should_be_invoked()
    {
        var option = default(Option<int>);
        int count = 0;

        var result = option.Or(() =>
        {
            count++;
            return Option<int>.Some(10);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(10));
        });
    }
}