namespace FunctionalCore.Tests.OptionTests;

public class OptionTapNoneTests
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
    /// 1. None.TapNone は action を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_None_TapNone_should_invoke_action_once()
    {
        int count = 0;

        _none.TapNone(() => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Some.TapNone は action を実行しない
    /// </summary>
    [Test]
    public void Option_Some_TapNone_should_not_invoke_action()
    {
        int count = 0;

        _some.TapNone(() => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 3. None.TapNone は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_None_TapNone_should_return_original_option()
    {
        var result = _none.TapNone(() => { });

        Assert.That(result, Is.EqualTo(_none));
    }

    /// <summary>
    /// 4. Some.TapNone は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_Some_TapNone_should_return_original_option()
    {
        var result = _some.TapNone(() => { });

        Assert.That(result, Is.EqualTo(_some));
    }

    /// <summary>
    /// 5. action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_TapNone_null_action_should_throw()
    {
        Action? action = null;

        Assert.Throws<ArgumentNullException>(() => _none.TapNone(action!));
    }

    /// <summary>
    /// 6. Some でも action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Some_TapNone_null_action_should_throw()
    {
        Action? action = null;

        Assert.Throws<ArgumentNullException>(() => _some.TapNone(action!));
    }

    /// <summary>
    /// 7. Default Option は None と同様に action を実行する
    /// </summary>
    [Test]
    public void Option_Default_TapNone_should_invoke_action()
    {
        var option = default(Option<int>);
        int count = 0;

        var result = option.TapNone(() => count++);

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}