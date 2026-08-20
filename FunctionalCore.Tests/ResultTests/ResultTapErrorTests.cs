namespace FunctionalCore.Tests.ResultTests;

public class ResultTapErrorTests
{
    /// <summary>
    /// 1. Fail.TapError は action を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Fail_TapError_should_invoke_action_once()
    {
        var fail = Result<string, int>.Fail("error"); ;
        int count = 0;

        fail.TapError(_ => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Fail.TapError は Error を action に渡す
    /// </summary>
    [Test]
    public void Result_Fail_TapError_should_pass_error_to_action()
    {
        var fail = Result<string, int>.Fail("error");
        string? received = null;

        fail.TapError(error => received = error);

        Assert.That(received, Is.EqualTo("error"));
    }

    /// <summary>
    /// 3. Ok.TapError は action を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_TapError_should_not_invoke_action()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.TapError(_ => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. Fail.TapError は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public void Result_Fail_TapError_should_return_original_result()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.TapError(_ => { });

        Assert.That(result, Is.EqualTo(fail));
    }

    /// <summary>
    /// 5. Ok.TapError は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public void Result_Ok_TapError_should_return_original_result()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.TapError(_ => { });

        Assert.That(result, Is.EqualTo(ok));
    }

    /// <summary>
    /// 6. TapError の action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_TapError_null_action_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Assert.Throws<ArgumentNullException>(() => fail.TapError(null!));
    }
}