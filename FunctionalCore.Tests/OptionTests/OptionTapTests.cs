namespace FunctionalCore.Tests.OptionTests;

public class OptionTapTests
{
    /// <summary>
    /// 1. Some.Tap は action を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_Tap_should_invoke_action_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Some.Tap は Value を action に渡す
    /// </summary>
    [Test]
    public void Option_Some_Tap_should_pass_value_to_action()
    {
        var some = Option<int>.Some(5);
        int received = 0;

        some.Tap(value => received = value);

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. None.Tap は action を実行しない
    /// </summary>
    [Test]
    public void Option_None_Tap_should_not_invoke_action()
    {
        var none = Option<int>.None;
        int count = 0;

        none.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. Some.Tap は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_Some_Tap_should_return_original_option()
    {
        var some = Option<int>.Some(5);
        var result = some.Tap(_ => { });

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 5. None.Tap は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_None_Tap_should_return_original_option()
    {
        var none = Option<int>.None;
        var result = none.Tap(_ => { });

        Assert.That(result, Is.EqualTo(none));
    }

    /// <summary>
    /// 6. action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Tap_null_action_should_throw()
    {
        var some = Option<int>.Some(5);
        Action<int>? action = null;

        Assert.Throws<ArgumentNullException>(() => some.Tap(action!));
    }

    /// <summary>
    /// 7. None でも action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Tap_null_action_should_throw()
    {
        var none = Option<int>.None;
        Action<int>? action = null;

        Assert.Throws<ArgumentNullException>(() => none.Tap(action!));
    }

    /// <summary>
    /// 8. Default Option は None と同様に action を実行せず None を返す
    /// </summary>
    [Test]
    public void Option_Default_Tap_should_not_invoke_action()
    {
        var option = default(Option<int>);
        int count = 0;

        var result = option.Tap(_ => count++);

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}