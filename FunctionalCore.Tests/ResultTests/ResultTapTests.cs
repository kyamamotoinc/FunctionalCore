namespace FunctionalCore.Tests.ResultTests;

public class ResultTapTests
{
    /// <summary>
    /// 1. Ok.Tap は action を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_Tap_should_invoke_action_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Ok.Tap は成功値を action に渡す
    /// </summary>
    [Test]
    public void Result_Ok_Tap_should_pass_value_to_action()
    {
        var ok = Result<string, int>.Ok(5);
        int received = 0;

        ok.Tap(value => received = value);

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. Fail.Tap は action を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Tap_should_not_invoke_action()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. Ok.Tap は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public void Result_Ok_Tap_should_return_original_result()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Tap(_ => { });

        Assert.That(result, Is.EqualTo(ok));
    }

    /// <summary>
    /// 5. Fail.Tap は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public void Result_Fail_Tap_should_return_original_result()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Tap(_ => { });

        Assert.That(result, Is.EqualTo(fail));
    }

    /// <summary>
    /// 6. Tap の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Tap_null_action_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<ArgumentNullException>(() => ok.Tap(null!));
    }
}