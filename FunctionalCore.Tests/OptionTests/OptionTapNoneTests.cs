namespace FunctionalCore.Tests.OptionTests;

public class OptionTapNoneTests
{
    /// <summary>
    /// 1. OptionがNoneの場合はactionを1回だけ実行する。
    /// </summary>
    [Test]
    public void None_TapNone_should_invoke_action_once()
    {
        var none = Option<int>.None;
        int count = 0;

        none.TapNone(() => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. OptionがSomeの場合はactionを実行しない。
    /// </summary>
    [Test]
    public void Some_TapNone_should_not_invoke_action()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.TapNone(() => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 3. OptionがNoneの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public void None_TapNone_should_return_original_option()
    {
        var none = Option<int>.None;

        var result = none.TapNone(() => { });

        Assert.That(result, Is.EqualTo(none));
    }

    /// <summary>
    /// 4. OptionがSomeの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public void Some_TapNone_should_return_original_option()
    {
        var some = Option<int>.Some(5);

        var result = some.TapNone(() => { });

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 5. OptionがNoneの場合でもactionがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_TapNone_should_throw_argument_null_exception_when_action_is_null()
    {
        var none = Option<int>.None;
        Action? action = null;

        Assert.Throws<ArgumentNullException>(() => none.TapNone(action!));
    }

    /// <summary>
    /// 6. OptionがSomeの場合でもactionがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_TapNone_should_throw_argument_null_exception_when_action_is_null()
    {
        var some = Option<int>.Some(5);
        Action? action = null;

        Assert.Throws<ArgumentNullException>(() => some.TapNone(action!));
    }

    /// <summary>
    /// 7. default OptionはNoneと同様にactionを1回実行し、Noneを返す。
    /// </summary>
    [Test]
    public void Default_TapNone_should_invoke_action_once_and_return_none()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = defaultOption.TapNone(() => count++);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 8. default Optionの場合でもactionがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_TapNone_should_throw_argument_null_exception_when_action_is_null()
    {
        var defaultOption = default(Option<int>);
        Action? action = null;

        Assert.Throws<ArgumentNullException>(() => defaultOption.TapNone(action!));
    }

    /// <summary>
    /// 9. OptionがNoneでactionが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void None_TapNone_should_propagate_exception_when_action_throws()
    {
        var none = Option<int>.None;
        var expectedException = new NotSupportedException("action error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            none.TapNone(() => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}