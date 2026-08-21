namespace FunctionalCore.Tests.OptionTests;

public class OptionMatchActionTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はonSomeを1回だけ実行する。
    /// </summary>
    [Test]
    public void Some_MatchAction_should_invoke_onSome_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Match(
            _ => count++,
            () => { });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. OptionがSomeの場合はValueをonSomeに渡す。
    /// </summary>
    [Test]
    public void Some_MatchAction_should_pass_value_to_onSome()
    {
        var some = Option<int>.Some(5);
        int receivedValue = 0;

        some.Match(
            value => receivedValue = value,
            () => { });

        Assert.That(receivedValue, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. OptionがSomeの場合はonNoneを実行しない。
    /// </summary>
    [Test]
    public void Some_MatchAction_should_not_invoke_onNone()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Match(
            _ => { },
            () => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. OptionがNoneの場合はonNoneを1回だけ実行する。
    /// </summary>
    [Test]
    public void None_MatchAction_should_invoke_onNone_once()
    {
        var none = Option<int>.None;
        int count = 0;

        none.Match(
            _ => { },
            () => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. OptionがNoneの場合はonSomeを実行しない。
    /// </summary>
    [Test]
    public void None_MatchAction_should_not_invoke_onSome()
    {
        var none = Option<int>.None;
        int count = 0;

        none.Match(
            _ => count++,
            () => { });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 6. OptionがSomeの場合でもonSomeがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_MatchAction_should_throw_argument_null_exception_when_onSome_is_null()
    {
        var some = Option<int>.Some(5);
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.Match(onSome!, () => { }));
    }

    /// <summary>
    /// 7. OptionがNoneの場合でもonNoneがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_MatchAction_should_throw_argument_null_exception_when_onNone_is_null()
    {
        var none = Option<int>.None;
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.Match(_ => { }, onNone!));
    }

    /// <summary>
    /// 8. OptionがSomeの場合でも未使用のonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_MatchAction_should_throw_argument_null_exception_when_unused_onNone_is_null()
    {
        var some = Option<int>.Some(5);
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.Match(_ => { }, onNone!));
    }

    /// <summary>
    /// 9. OptionがNoneの場合でも未使用のonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_MatchAction_should_throw_argument_null_exception_when_unused_onSome_is_null()
    {
        var none = Option<int>.None;
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.Match(onSome!, () => { }));
    }

    /// <summary>
    /// 10. default OptionはNoneと同様にonNoneを実行する。
    /// </summary>
    [Test]
    public void Default_MatchAction_should_behave_as_none()
    {
        var defaultOption = default(Option<int>);
        int someCount = 0;
        int noneCount = 0;

        defaultOption.Match(
            _ => someCount++,
            () => noneCount++);

        Assert.Multiple(() =>
        {
            Assert.That(someCount, Is.EqualTo(0));
            Assert.That(noneCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 11. default Optionの場合でもonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_MatchAction_should_throw_argument_null_exception_when_onSome_is_null()
    {
        var defaultOption = default(Option<int>);
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            defaultOption.Match(onSome!, () => { }));
    }

    /// <summary>
    /// 12. default Optionの場合でもonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_MatchAction_should_throw_argument_null_exception_when_onNone_is_null()
    {
        var defaultOption = default(Option<int>);
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            defaultOption.Match(_ => { }, onNone!));
    }

    /// <summary>
    /// 13. OptionがSomeでonSomeが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_MatchAction_should_propagate_exception_when_onSome_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("onSome error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            some.Match(_ => throw expectedException, () => { }));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 14. OptionがNoneでonNoneが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void None_MatchAction_should_propagate_exception_when_onNone_throws()
    {
        var none = Option<int>.None;
        var expectedException = new NotSupportedException("onNone error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            none.Match(_ => { }, () => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}