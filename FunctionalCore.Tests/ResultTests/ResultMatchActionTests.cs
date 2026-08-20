namespace FunctionalCore.Tests.ResultTests;

public class ResultMatchActionTests
{
    /// <summary>
    /// 1. Ok.Match は成功側の action を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_MatchAction_should_invoke_success_action_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Match(
            _ => count++,
            _ => { });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Ok.Match は成功値を成功側の action に渡す
    /// </summary>
    [Test]
    public void Result_Ok_MatchAction_should_pass_value_to_success_action()
    {
        var ok = Result<string, int>.Ok(5);
        int received = 0;

        ok.Match(
            value => received = value,
            _ => { });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. Ok.Match は失敗側の action を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_MatchAction_should_not_invoke_failure_action()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Match(
            _ => { },
            _ => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. Fail.Match は失敗側の action を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Fail_MatchAction_should_invoke_failure_action_once()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Match(
            _ => { },
            _ => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. Fail.Match は Error を失敗側の action に渡す
    /// </summary>
    [Test]
    public void Result_Fail_MatchAction_should_pass_error_to_failure_action()
    {
        var fail = Result<string, int>.Fail("error");
        string? received = null;

        fail.Match(
            _ => { },
            error => received = error);

        Assert.That(received, Is.EqualTo("error"));
    }

    /// <summary>
    /// 6. Fail.Match は成功側の action を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_MatchAction_should_not_invoke_success_action()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Match(
            _ => count++,
            _ => { });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 7. 成功側の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_MatchAction_null_success_action_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Action<int>? onSuccess = null;

        Assert.Throws<ArgumentNullException>(() =>
            ok.Match(onSuccess!, _ => { }));
    }

    /// <summary>
    /// 8. 失敗側の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_MatchAction_null_failure_action_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Action<string>? onFailure = null;

        Assert.Throws<ArgumentNullException>(() =>
            fail.Match(_ => { }, onFailure!));
    }

    /// <summary>
    /// 9. Ok でも失敗側の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_MatchAction_null_unused_failure_action_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Action<string>? onFailure = null;

        Assert.Throws<ArgumentNullException>(() =>
            ok.Match(_ => { }, onFailure!));
    }

    /// <summary>
    /// 10. Fail でも成功側の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Fail_MatchAction_null_unused_success_action_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Action<int>? onSuccess = null;

        Assert.Throws<ArgumentNullException>(() =>
            fail.Match(onSuccess!, _ => { }));
    }

    /// <summary>
    /// 11. 未初期化 Result で Match(Action, Action) を呼び出すと InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_MatchAction_should_throw()
    {
        var result = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() =>
            result.Match(_ => { }, _ => { }));
    }
}