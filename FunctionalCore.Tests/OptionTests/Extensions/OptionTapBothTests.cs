using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionTapBothTests
{
    /// <summary>
    /// 1. Some.TapBoth は Some 側の action だけを実行する
    /// </summary>
    [Test]
    public void Option_Some_TapBoth_should_invoke_only_some_action()
    {
        var some = Option<int>.Some(5);
        int someCount = 0;
        int noneCount = 0;

        some.TapBoth(_ => someCount++, () => noneCount++);

        Assert.Multiple(() =>
        {
            Assert.That(someCount, Is.EqualTo(1));
            Assert.That(noneCount, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 2. None.TapBoth は None 側の action だけを実行する
    /// </summary>
    [Test]
    public void Option_None_TapBoth_should_invoke_only_none_action()
    {
        var none = Option<int>.None;
        int someCount = 0;
        int noneCount = 0;

        none.TapBoth(_ => someCount++, () => noneCount++);

        Assert.Multiple(() =>
        {
            Assert.That(someCount, Is.EqualTo(0));
            Assert.That(noneCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 3. Some.TapBoth は Value を Some 側の action に渡す
    /// </summary>
    [Test]
    public void Option_Some_TapBoth_should_pass_value_to_some_action()
    {
        var some = Option<int>.Some(5);
        int received = 0;

        some.TapBoth(value => received = value, () => { });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 4. Some.TapBoth は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_Some_TapBoth_should_return_original_option()
    {
        var some = Option<int>.Some(5);
        var result = some.TapBoth(_ => { }, () => { });

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 5. None.TapBoth は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_None_TapBoth_should_return_original_option()
    {
        var none = Option<int>.None;
        var result = none.TapBoth(_ => { }, () => { });

        Assert.That(result, Is.EqualTo(none));
    }

    /// <summary>
    /// 6. Some 側の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_TapBoth_null_some_action_should_throw()
    {
        var some = Option<int>.Some(5);
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.TapBoth(onSome!, () => { }));
    }

    /// <summary>
    /// 7. None 側の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_TapBoth_null_none_action_should_throw()
    {
        var none = Option<int>.None;
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.TapBoth(_ => { }, onNone!));
    }

    /// <summary>
    /// 8. Some でも未使用の None 側 action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Some_TapBoth_null_unused_none_action_should_throw()
    {
        var some = Option<int>.Some(5);
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.TapBoth(_ => { }, onNone!));
    }

    /// <summary>
    /// 9. None でも未使用の Some 側 action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_TapBoth_null_unused_some_action_should_throw()
    {
        var none = Option<int>.None;
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.TapBoth(onSome!, () => { }));
    }

    /// <summary>
    /// 10. Default Option は None と同様に None 側の action を実行する
    /// </summary>
    [Test]
    public void Option_Default_TapBoth_should_behave_as_none()
    {
        var option = default(Option<int>);
        int someCount = 0;
        int noneCount = 0;

        var result = option.TapBoth(
            _ => someCount++,
            () => noneCount++);

        Assert.Multiple(() =>
        {
            Assert.That(someCount, Is.EqualTo(0));
            Assert.That(noneCount, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}