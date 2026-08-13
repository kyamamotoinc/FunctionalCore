using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionTapBothTests
{
    private Option<int> _some;
    private Option<int> _none;

    [SetUp]
    public void Setup()
    {
        _some = Option<int>.Some(5);
        _none = Option<int>.None;
    }

    /// <summary>
    /// 1. Some.TapBoth は Some 側の action だけを実行する
    /// </summary>
    [Test]
    public void Option_Some_TapBoth_should_invoke_only_some_action()
    {
        int someCount = 0;
        int noneCount = 0;

        _some.TapBoth(_ => someCount++, () => noneCount++);

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
        int someCount = 0;
        int noneCount = 0;

        _none.TapBoth(_ => someCount++, () => noneCount++);

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
        int received = 0;

        _some.TapBoth(value => received = value, () => { });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 4. Some.TapBoth は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_Some_TapBoth_should_return_original_option()
    {
        var result = _some.TapBoth(_ => { }, () => { });

        Assert.That(result, Is.EqualTo(_some));
    }

    /// <summary>
    /// 5. None.TapBoth は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_None_TapBoth_should_return_original_option()
    {
        var result = _none.TapBoth(_ => { }, () => { });

        Assert.That(result, Is.EqualTo(_none));
    }

    /// <summary>
    /// 6. Some 側の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_TapBoth_null_some_action_should_throw()
    {
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            _some.TapBoth(onSome!, () => { }));
    }

    /// <summary>
    /// 7. None 側の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_TapBoth_null_none_action_should_throw()
    {
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            _none.TapBoth(_ => { }, onNone!));
    }

    /// <summary>
    /// 8. Some でも未使用の None 側 action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Some_TapBoth_null_unused_none_action_should_throw()
    {
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            _some.TapBoth(_ => { }, onNone!));
    }

    /// <summary>
    /// 9. None でも未使用の Some 側 action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_TapBoth_null_unused_some_action_should_throw()
    {
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            _none.TapBoth(onSome!, () => { }));
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