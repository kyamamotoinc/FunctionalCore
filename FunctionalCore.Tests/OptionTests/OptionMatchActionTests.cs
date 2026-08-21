namespace FunctionalCore.Tests.OptionTests;

public class OptionMatchActionTests
{
    /// <summary>
    /// 1. Some.Match は Some 側の action を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_MatchAction_should_invoke_some_action_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Match(
            _ => count++,
            () => { });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Some.Match は Value を Some 側の action に渡す
    /// </summary>
    [Test]
    public void Option_Some_MatchAction_should_pass_value_to_some_action()
    {
        var some = Option<int>.Some(5);
        int received = 0;

        some.Match(
            value => received = value,
            () => { });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. Some.Match は None 側の action を実行しない
    /// </summary>
    [Test]
    public void Option_Some_MatchAction_should_not_invoke_none_action()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Match(
            _ => { },
            () => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. None.Match は None 側の action を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_None_MatchAction_should_invoke_none_action_once()
    {
        var none = Option<int>.None;
        int count = 0;

        none.Match(
            _ => { },
            () => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. None.Match は Some 側の action を実行しない
    /// </summary>
    [Test]
    public void Option_None_MatchAction_should_not_invoke_some_action()
    {
        var none = Option<int>.None;
        int count = 0;

        none.Match(
            _ => count++,
            () => { });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 6. Some 側の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_MatchAction_null_some_action_should_throw()
    {
        var some = Option<int>.Some(5);
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.Match(onSome!, () => { }));
    }

    /// <summary>
    /// 7. None 側の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_MatchAction_null_none_action_should_throw()
    {
        var none = Option<int>.None;
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.Match(_ => { }, onNone!));
    }

    /// <summary>
    /// 8. Some でも未使用の None 側 action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Some_MatchAction_null_unused_none_action_should_throw()
    {
        var some = Option<int>.Some(5);
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.Match(_ => { }, onNone!));
    }

    /// <summary>
    /// 9. None でも未使用の Some 側 action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_MatchAction_null_unused_some_action_should_throw()
    {
        var none = Option<int>.None;
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.Match(onSome!, () => { }));
    }

    /// <summary>
    /// 10. Default Option は None と同様に None 側の action を実行する
    /// </summary>
    [Test]
    public void Option_Default_MatchAction_should_behave_as_none()
    {
        var option = default(Option<int>);
        int someCount = 0;
        int noneCount = 0;

        option.Match(
            _ => someCount++,
            () => noneCount++);

        Assert.Multiple(() =>
        {
            Assert.That(someCount, Is.EqualTo(0));
            Assert.That(noneCount, Is.EqualTo(1));
        });
    }
}