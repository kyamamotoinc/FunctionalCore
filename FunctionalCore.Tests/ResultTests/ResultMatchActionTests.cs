namespace FunctionalCore.Tests.ResultTests;

public class ResultMatchActionTests
{
    /// <summary>
    /// 1. ResultがOkの場合はonSuccessを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_MatchAction_should_invoke_onSuccess_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Match(
            _ => count++,
            _ => { });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. ResultがOkの場合は成功値をonSuccessに渡す。
    /// </summary>
    [Test]
    public void Ok_MatchAction_should_pass_value_to_onSuccess()
    {
        var ok = Result<string, int>.Ok(5);
        int received = 0;

        ok.Match(
            value => received = value,
            _ => { });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. ResultがOkの場合はonFailureを実行しない。
    /// </summary>
    [Test]
    public void Ok_MatchAction_should_not_invoke_onFailure()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Match(
            _ => { },
            _ => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. ResultがFailの場合はonFailureを1回だけ実行する。
    /// </summary>
    [Test]
    public void Fail_MatchAction_should_invoke_onFailure_once()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Match(
            _ => { },
            _ => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. ResultがFailの場合はErrorをonFailureに渡す。
    /// </summary>
    [Test]
    public void Fail_MatchAction_should_pass_error_to_onFailure()
    {
        var fail = Result<string, int>.Fail("error");
        string? receivedError = null;

        fail.Match(
            _ => { },
            error => receivedError = error);

        Assert.That(receivedError, Is.EqualTo("error"));
    }

    /// <summary>
    /// 6. ResultがFailの場合はonSuccessを実行しない。
    /// </summary>
    [Test]
    public void Fail_MatchAction_should_not_invoke_onSuccess()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Match(
            _ => count++,
            _ => { });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 7. ResultがOkの場合でもonSuccessがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_MatchAction_should_throw_argument_null_exception_when_onSuccess_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Action<int>? onSuccess = null;

        Assert.Throws<ArgumentNullException>(() => ok.Match(onSuccess!, _ => { }));
    }

    /// <summary>
    /// 8. ResultがFailの場合でもonFailureがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_MatchAction_should_throw_argument_null_exception_when_onFailure_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Action<string>? onFailure = null;

        Assert.Throws<ArgumentNullException>(() => fail.Match(_ => { }, onFailure!));
    }

    /// <summary>
    /// 9. ResultがOkの場合でも未使用のonFailureがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_MatchAction_should_throw_argument_null_exception_when_unused_onFailure_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Action<string>? onFailure = null;

        Assert.Throws<ArgumentNullException>(() => ok.Match(_ => { }, onFailure!));
    }

    /// <summary>
    /// 10. ResultがFailの場合でも未使用のonSuccessがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_MatchAction_should_throw_argument_null_exception_when_unused_onSuccess_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Action<int>? onSuccess = null;

        Assert.Throws<ArgumentNullException>(() => fail.Match(onSuccess!, _ => { }));
    }

    /// <summary>
    /// 11. Resultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_MatchAction_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Match(_ => { }, _ => { }));
    }

    /// <summary>
    /// 12. ResultがdefaultでonSuccessもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_MatchAction_should_throw_invalid_operation_exception_before_onSuccess_null_check()
    {
        var uninitialized = default(Result<string, int>);
        Action<int>? onSuccess = null;

        Assert.Throws<InvalidOperationException>(() => uninitialized.Match(onSuccess!, _ => { }));
    }

    /// <summary>
    /// 13. ResultがdefaultでonFailureもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_MatchAction_should_throw_invalid_operation_exception_before_onFailure_null_check()
    {
        var uninitialized = default(Result<string, int>);
        Action<string>? onFailure = null;

        Assert.Throws<InvalidOperationException>(() => uninitialized.Match(_ => { }, onFailure!));
    }

    /// <summary>
    /// 14. ResultがOkでonSuccessが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_MatchAction_should_propagate_exception_when_onSuccess_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("onSuccess error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            ok.Match(_ => throw expectedException, _ => { }));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 15. ResultがFailでonFailureが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Fail_MatchAction_should_propagate_exception_when_onFailure_throws()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new NotSupportedException("onFailure error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            fail.Match(_ => { }, _ => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}