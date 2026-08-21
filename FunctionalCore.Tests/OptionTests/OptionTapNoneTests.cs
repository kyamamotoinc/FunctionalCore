namespace FunctionalCore.Tests.OptionTests;

public class OptionTapNoneTests
{
    /// <summary>
    /// 1. None.TapNone は action を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_None_TapNone_should_invoke_action_once()
    {
        var none = Option<int>.None;
        int count = 0;

        none.TapNone(() => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Some.TapNone は action を実行しない
    /// </summary>
    [Test]
    public void Option_Some_TapNone_should_not_invoke_action()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.TapNone(() => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 3. None.TapNone は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_None_TapNone_should_return_original_option()
    {
        var none = Option<int>.None;
        var result = none.TapNone(() => { });

        Assert.That(result, Is.EqualTo(none));
    }

    /// <summary>
    /// 4. Some.TapNone は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_Some_TapNone_should_return_original_option()
    {
        var some = Option<int>.Some(5);
        var result = some.TapNone(() => { });

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 5. action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_TapNone_null_action_should_throw()
    {
        var none = Option<int>.None;
        Action? action = null;

        Assert.Throws<ArgumentNullException>(() => none.TapNone(action!));
    }

    /// <summary>
    /// 6. Some でも action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Some_TapNone_null_action_should_throw()
    {
        var some = Option<int>.Some(5);
        Action? action = null;

        Assert.Throws<ArgumentNullException>(() => some.TapNone(action!));
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