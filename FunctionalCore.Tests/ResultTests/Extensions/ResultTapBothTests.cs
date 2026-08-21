using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultTapBothTests
{
    /// <summary>
    /// 1. ResultがOkの場合はonSuccessだけを実行する。
    /// </summary>
    [Test]
    public void Ok_TapBoth_should_invoke_only_onSuccess()
    {
        var ok = Result<string, int>.Ok(5);
        int successCount = 0;
        int failureCount = 0;

        ok.TapBoth(_ => successCount++, _ => failureCount++);

        Assert.Multiple(() =>
        {
            Assert.That(successCount, Is.EqualTo(1));
            Assert.That(failureCount, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 2. ResultがFailの場合はonFailureだけを実行する。
    /// </summary>
    [Test]
    public void Fail_TapBoth_should_invoke_only_onFailure()
    {
        var fail = Result<string, int>.Fail("error");
        int successCount = 0;
        int failureCount = 0;

        fail.TapBoth(_ => successCount++, _ => failureCount++);

        Assert.Multiple(() =>
        {
            Assert.That(successCount, Is.EqualTo(0));
            Assert.That(failureCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 3. ResultがOkの場合は成功値をonSuccessに渡す。
    /// </summary>
    [Test]
    public void Ok_TapBoth_should_pass_value_to_onSuccess()
    {
        var ok = Result<string, int>.Ok(5);
        int receivedValue = 0;

        ok.TapBoth(value => receivedValue = value, _ => { });

        Assert.That(receivedValue, Is.EqualTo(5));
    }

    /// <summary>
    /// 4. ResultがFailの場合はErrorをonFailureに渡す。
    /// </summary>
    [Test]
    public void Fail_TapBoth_should_pass_error_to_onFailure()
    {
        var fail = Result<string, int>.Fail("error");
        string? receivedError = null;

        fail.TapBoth(_ => { }, error => receivedError = error);

        Assert.That(receivedError, Is.EqualTo("error"));
    }

    /// <summary>
    /// 5. TapBothを実行しても元のResultをそのまま返す。
    /// </summary>
    [Test]
    public void TapBoth_should_return_original_result()
    {
        var ok = Result<string, int>.Ok(5);
        var fail = Result<string, int>.Fail("error");

        var okResult = ok.TapBoth(_ => { }, _ => { });
        var failResult = fail.TapBoth(_ => { }, _ => { });

        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.EqualTo(ok));
            Assert.That(failResult, Is.EqualTo(fail));
        });
    }

    /// <summary>
    /// 6. ResultがOkの場合でもonSuccessがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_TapBoth_should_throw_argument_null_exception_when_onSuccess_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.TapBoth(null!, _ => { }));
    }

    /// <summary>
    /// 7. ResultがFailの場合でもonFailureがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_TapBoth_should_throw_argument_null_exception_when_onFailure_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.TapBoth(_ => { }, null!));
    }

    /// <summary>
    /// 8. ResultがOkの場合でも未使用のonFailureがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_TapBoth_should_throw_argument_null_exception_when_unused_onFailure_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.TapBoth(_ => { }, null!));
    }

    /// <summary>
    /// 9. ResultがFailの場合でも未使用のonSuccessがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_TapBoth_should_throw_argument_null_exception_when_unused_onSuccess_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.TapBoth(null!, _ => { }));
    }

    /// <summary>
    /// 10. Resultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_TapBoth_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.TapBoth(_ => { }, _ => { }));
    }

    /// <summary>
    /// 11. ResultがdefaultでonSuccessもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_TapBoth_should_throw_invalid_operation_exception_before_onSuccess_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.TapBoth(null!, _ => { }));
    }

    /// <summary>
    /// 12. ResultがdefaultでonFailureもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_TapBoth_should_throw_invalid_operation_exception_before_onFailure_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.TapBoth(_ => { }, null!));
    }

    /// <summary>
    /// 13. ResultがOkでonSuccessが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_TapBoth_should_propagate_exception_when_onSuccess_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("onSuccess error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            ok.TapBoth(_ => throw expectedException, _ => { }));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 14. ResultがFailでonFailureが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Fail_TapBoth_should_propagate_exception_when_onFailure_throws()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new NotSupportedException("onFailure error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            fail.TapBoth(_ => { }, _ => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}