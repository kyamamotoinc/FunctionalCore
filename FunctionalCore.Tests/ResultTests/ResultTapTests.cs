namespace FunctionalCore.Tests.ResultTests;

public class ResultTapTests
{
    private Result<string, int> _ok;
    private Result<string, int> _fail;

    [SetUp]
    public void Setup()
    {
        _ok = Result<string, int>.Ok(5);
        _fail = Result<string, int>.Fail("error");
    }

    /// <summary>
    /// 1. Ok.Tap は action を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_Tap_should_invoke_action_once()
    {
        int count = 0;

        _ok.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Ok.Tap は成功値を action に渡す
    /// </summary>
    [Test]
    public void Result_Ok_Tap_should_pass_value_to_action()
    {
        int received = 0;

        _ok.Tap(value => received = value);

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. Fail.Tap は action を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Tap_should_not_invoke_action()
    {
        int count = 0;

        _fail.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. Ok.Tap は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public void Result_Ok_Tap_should_return_original_result()
    {
        var result = _ok.Tap(_ => { });

        Assert.That(result, Is.EqualTo(_ok));
    }

    /// <summary>
    /// 5. Fail.Tap は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public void Result_Fail_Tap_should_return_original_result()
    {
        var result = _fail.Tap(_ => { });

        Assert.That(result, Is.EqualTo(_fail));
    }

    /// <summary>
    /// 6. Tap の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Tap_null_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ok.Tap(null!));
    }
}