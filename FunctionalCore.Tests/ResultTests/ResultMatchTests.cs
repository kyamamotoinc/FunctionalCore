namespace FunctionalCore.Tests.ResultTests;

public class ResultMatchTests
{
    /// <summary>
    /// 1. ResultがOkの場合はonSuccessを実行し、その戻り値を返す。
    /// </summary>
    [Test]
    public void Ok_Match_should_return_onSuccess_result()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Match(value => value + 1, _ => -1);

        Assert.That(result, Is.EqualTo(6));
    }

    /// <summary>
    /// 2. ResultがOkの場合はonSuccessを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_Match_should_invoke_onSuccess_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Match(value =>
        {
            count++;
            return value + 1;
        }, _ => -1);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 3. ResultがOkの場合はonFailureを実行しない。
    /// </summary>
    [Test]
    public void Ok_Match_should_not_invoke_onFailure()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Match(value => value + 1, _ =>
        {
            count++;
            return -1;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. ResultがFailの場合はonFailureを実行し、その戻り値を返す。
    /// </summary>
    [Test]
    public void Fail_Match_should_return_onFailure_result()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Match(value => value + 1, _ => -1);

        Assert.That(result, Is.EqualTo(-1));
    }

    /// <summary>
    /// 5. ResultがFailの場合はonFailureを1回だけ実行する。
    /// </summary>
    [Test]
    public void Fail_Match_should_invoke_onFailure_once()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Match(value => value + 1, _ =>
        {
            count++;
            return -1;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 6. ResultがFailの場合はonSuccessを実行しない。
    /// </summary>
    [Test]
    public void Fail_Match_should_not_invoke_onSuccess()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Match(value =>
        {
            count++;
            return value + 1;
        }, _ => -1);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 7. ResultがOkの場合でもonSuccessがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Match_should_throw_argument_null_exception_when_onSuccess_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.Match<int>(null!, _ => -1));
    }

    /// <summary>
    /// 8. ResultがFailの場合でもonFailureがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Match_should_throw_argument_null_exception_when_onFailure_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.Match(value => value + 1, null!));
    }

    /// <summary>
    /// 9. ResultがOkでonSuccessがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Match_should_throw_invalid_operation_exception_when_onSuccess_returns_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() => ok.Match(_ => (string)null!, _ => "fallback"));
    }

    /// <summary>
    /// 10. ResultがFailでonFailureがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Match_should_throw_invalid_operation_exception_when_onFailure_returns_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<InvalidOperationException>(() => fail.Match(_ => "success", _ => (string)null!));
    }

    /// <summary>
    /// 11. ResultがOkの場合はnullを返すonFailureでも実行せず、onSuccessの戻り値を返す。
    /// </summary>
    [Test]
    public void Ok_Match_should_return_onSuccess_result_without_invoking_null_returning_onFailure()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var result = ok.Match(
            value => $"value:{value}",
            _ =>
            {
                count++;
                return (string)null!;
            });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("value:5"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 12. ResultがFailの場合はnullを返すonSuccessでも実行せず、onFailureの戻り値を返す。
    /// </summary>
    [Test]
    public void Fail_Match_should_return_onFailure_result_without_invoking_null_returning_onSuccess()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = fail.Match(
            _ =>
            {
                count++;
                return (string)null!;
            },
            error => $"error:{error}");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("error:error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 13. Resultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Match_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Match(value => value + 1, _ => -1));
    }

    /// <summary>
    /// 14. ResultがOkの場合でも未使用のonFailureがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Match_should_throw_argument_null_exception_when_unused_onFailure_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Func<string, int>? onFailure = null;

        Assert.Throws<ArgumentNullException>(() => ok.Match(value => value + 1, onFailure!));
    }

    /// <summary>
    /// 15. ResultがFailの場合でも未使用のonSuccessがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Match_should_throw_argument_null_exception_when_unused_onSuccess_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Func<int, int>? onSuccess = null;

        Assert.Throws<ArgumentNullException>(() => fail.Match(onSuccess!, _ => -1));
    }

    /// <summary>
    /// 16. ResultがdefaultでonSuccessもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Match_should_throw_invalid_operation_exception_before_onSuccess_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Match<int>(null!, _ => -1));
    }

    /// <summary>
    /// 17. ResultがdefaultでonFailureもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Match_should_throw_invalid_operation_exception_before_onFailure_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Match(value => value + 1, null!));
    }

    /// <summary>
    /// 18. ResultがOkでonSuccessが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_Match_should_propagate_exception_when_onSuccess_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("onSuccess error");
        Func<int, int> onSuccess = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() =>
            ok.Match(onSuccess, _ => -1));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 19. ResultがFailでonFailureが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Fail_Match_should_propagate_exception_when_onFailure_throws()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new NotSupportedException("onFailure error");
        Func<string, int> onFailure = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() =>
            fail.Match(value => value + 1, onFailure));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}