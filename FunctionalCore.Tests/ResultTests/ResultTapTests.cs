namespace FunctionalCore.Tests.ResultTests;

public class ResultTapTests
{
    /// <summary>
    /// 1. ResultがOkの場合はactionを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_Tap_should_invoke_action_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. ResultがOkの場合は成功値をactionに渡す。
    /// </summary>
    [Test]
    public void Ok_Tap_should_pass_value_to_action()
    {
        var ok = Result<string, int>.Ok(5);
        int received = 0;

        ok.Tap(value => received = value);

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. ResultがFailの場合はactionを実行しない。
    /// </summary>
    [Test]
    public void Fail_Tap_should_not_invoke_action()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Tap(_ => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. ResultがOkの場合は元のResultをそのまま返す。
    /// </summary>
    [Test]
    public void Ok_Tap_should_return_original_result()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Tap(_ => { });

        Assert.That(result, Is.EqualTo(ok));
    }

    /// <summary>
    /// 5. ResultがFailの場合は元のResultをそのまま返す。
    /// </summary>
    [Test]
    public void Fail_Tap_should_return_original_result()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Tap(_ => { });

        Assert.That(result, Is.EqualTo(fail));
    }

    /// <summary>
    /// 6. ResultがOkの場合でもactionがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Tap_should_throw_argument_null_exception_when_action_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.Tap(null!));
    }

    /// <summary>
    /// 7. ResultがFailの場合でもactionがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Tap_should_throw_argument_null_exception_when_action_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.Tap(null!));
    }

    /// <summary>
    /// 8. Resultがdefaultでactionもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Tap_should_throw_invalid_operation_exception_before_action_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Tap(null!));
    }

    /// <summary>
    /// 9. ResultがOkでactionが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_Tap_should_propagate_exception_when_action_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("action error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            ok.Tap(_ => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}