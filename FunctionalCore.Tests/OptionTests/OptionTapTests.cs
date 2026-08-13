namespace FunctionalCore.Tests.OptionTests;

public class OptionTapTests
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
    /// 1. Some.Tap は action を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_Tap_should_invoke_action_once()
    {
        int count = 0;

        _some.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Some.Tap は Value を action に渡す
    /// </summary>
    [Test]
    public void Option_Some_Tap_should_pass_value_to_action()
    {
        int received = 0;

        _some.Tap(value => received = value);

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. None.Tap は action を実行しない
    /// </summary>
    [Test]
    public void Option_None_Tap_should_not_invoke_action()
    {
        int count = 0;

        _none.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. Some.Tap は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_Some_Tap_should_return_original_option()
    {
        var result = _some.Tap(_ => { });

        Assert.That(result, Is.EqualTo(_some));
    }

    /// <summary>
    /// 5. None.Tap は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public void Option_None_Tap_should_return_original_option()
    {
        var result = _none.Tap(_ => { });

        Assert.That(result, Is.EqualTo(_none));
    }

    /// <summary>
    /// 6. action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Tap_null_action_should_throw()
    {
        Action<int>? action = null;

        Assert.Throws<ArgumentNullException>(() => _some.Tap(action!));
    }

    /// <summary>
    /// 7. None でも action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Tap_null_action_should_throw()
    {
        Action<int>? action = null;

        Assert.Throws<ArgumentNullException>(() => _none.Tap(action!));
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