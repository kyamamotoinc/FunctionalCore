namespace FunctionalCore.Tests.OptionTests;

public class OptionTapTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はactionを1回だけ実行する。
    /// </summary>
    [Test]
    public void Some_Tap_should_invoke_action_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. OptionがSomeの場合はValueをactionに渡す。
    /// </summary>
    [Test]
    public void Some_Tap_should_pass_value_to_action()
    {
        var some = Option<int>.Some(5);
        int receivedValue = 0;

        some.Tap(value => receivedValue = value);

        Assert.That(receivedValue, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. OptionがNoneの場合はactionを実行しない。
    /// </summary>
    [Test]
    public void None_Tap_should_not_invoke_action()
    {
        var none = Option<int>.None;
        int count = 0;

        none.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. OptionがSomeの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public void Some_Tap_should_return_original_option()
    {
        var some = Option<int>.Some(5);

        var result = some.Tap(_ => { });

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 5. OptionがNoneの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public void None_Tap_should_return_original_option()
    {
        var none = Option<int>.None;

        var result = none.Tap(_ => { });

        Assert.That(result, Is.EqualTo(none));
    }

    /// <summary>
    /// 6. OptionがSomeの場合でもactionがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_Tap_should_throw_argument_null_exception_when_action_is_null()
    {
        var some = Option<int>.Some(5);
        Action<int>? action = null;

        Assert.Throws<ArgumentNullException>(() => some.Tap(action!));
    }

    /// <summary>
    /// 7. OptionがNoneの場合でもactionがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_Tap_should_throw_argument_null_exception_when_action_is_null()
    {
        var none = Option<int>.None;
        Action<int>? action = null;

        Assert.Throws<ArgumentNullException>(() => none.Tap(action!));
    }

    /// <summary>
    /// 8. default OptionはNoneと同様にactionを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public void Default_Tap_should_return_none_without_invoking_action()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = defaultOption.Tap(_ => count++);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 9. default Optionの場合でもactionがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Tap_should_throw_argument_null_exception_when_action_is_null()
    {
        var defaultOption = default(Option<int>);
        Action<int>? action = null;

        Assert.Throws<ArgumentNullException>(() => defaultOption.Tap(action!));
    }

    /// <summary>
    /// 10. OptionがSomeでactionが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_Tap_should_propagate_exception_when_action_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("action error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            some.Tap(_ => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}