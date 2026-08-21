namespace FunctionalCore.Tests.ResultTests;

public class ResultTapErrorTests
{
    /// <summary>
    /// 1. ResultがFailの場合はactionを1回だけ実行する。
    /// </summary>
    [Test]
    public void Fail_TapError_should_invoke_action_once()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.TapError(_ => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. ResultがFailの場合はErrorをactionに渡す。
    /// </summary>
    [Test]
    public void Fail_TapError_should_pass_error_to_action()
    {
        var fail = Result<string, int>.Fail("error");
        string? receivedError = null;

        fail.TapError(error => receivedError = error);

        Assert.That(receivedError, Is.EqualTo("error"));
    }

    /// <summary>
    /// 3. ResultがOkの場合はactionを実行しない。
    /// </summary>
    [Test]
    public void Ok_TapError_should_not_invoke_action()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.TapError(_ => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. ResultがFailの場合は元のResultをそのまま返す。
    /// </summary>
    [Test]
    public void Fail_TapError_should_return_original_result()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.TapError(_ => { });

        Assert.That(result, Is.EqualTo(fail));
    }

    /// <summary>
    /// 5. ResultがOkの場合は元のResultをそのまま返す。
    /// </summary>
    [Test]
    public void Ok_TapError_should_return_original_result()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.TapError(_ => { });

        Assert.That(result, Is.EqualTo(ok));
    }

    /// <summary>
    /// 6. ResultがFailの場合でもactionがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_TapError_should_throw_argument_null_exception_when_action_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.TapError(null!));
    }

    /// <summary>
    /// 7. ResultがOkの場合でもactionがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_TapError_should_throw_argument_null_exception_when_action_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.TapError(null!));
    }

    /// <summary>
    /// 8. Resultがdefaultでactionもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_TapError_should_throw_invalid_operation_exception_before_action_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.TapError(null!));
    }
}