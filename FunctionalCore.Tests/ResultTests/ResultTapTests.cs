namespace FunctionalCore.Tests.ResultTests;


public class ResultTapTests
{
    private Result<string, int> ok;
    private Result<string, int> fail;

   [SetUp]
    public void Setup()
    {
        ok = Result<string, int>.Ok(5);
        fail = Result<string, int>.Fail("error");
    }

    // -----------------------------
    // Tap (成功レールの副作用)
    // -----------------------------

    /// <summary>
    /// 1. Tap は成功レールで副作用を実行する
    /// </summary>
    [Test]
    public void Result_Ok_Tap_should_invoke_action()
    {
        int count = 0;

        ok.Tap(x => count++);

        Assert.AreEqual(1, count);
    }

    /// <summary>
    /// 2. Tap は失敗レールでは副作用を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Tap_should_not_invoke_action()
    {
        int count = 0;

        fail.Tap(x => count++);

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 3. Tap は Result を変えずに返す（レールを変えない）
    /// </summary>
    [Test]
    public void Result_Tap_should_return_same_result()
    {
        var res = ok.Tap(x => { });
        Assert.AreEqual(ok.Value, res.Value);
    }

    /// <summary>
    /// 4. Tap の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Result_Tap_null_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = ok.Tap(null));
    }

    // -----------------------------
    // TapError (失敗レールの副作用)
    // -----------------------------

    /// <summary>
    /// 5. TapError は失敗レールで副作用を実行する
    /// </summary>
    [Test]
    public void Result_Fail_TapError_should_invoke_action()
    {
        int count = 0;

        fail.TapError(err => count++);

        Assert.AreEqual(1, count);
    }

    /// <summary>
    /// 6. TapError は成功レールでは副作用を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_TapError_should_not_invoke_action()
    {
        int count = 0;

        ok.TapError(err => count++);

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 7. TapError の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Result_TapError_null_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = fail.TapError(null));
    }

    // -----------------------------
    // TapBoth (成功・失敗問わず副作用)
    // -----------------------------

    /// <summary>
    /// 8. TapBoth は成功レールでも副作用を実行する
    /// </summary>
    [Test]
    public void Result_Ok_TapBoth_should_invoke_action()
    {
        int count = 0;

        ok.TapBoth(r => count++);

        Assert.AreEqual(1, count);
    }

    /// <summary>
    /// 9. TapBoth は失敗レールでも副作用を実行する
    /// </summary>
    [Test]
    public void Result_Fail_TapBoth_should_invoke_action()
    {
        int count = 0;

        fail.TapBoth(r => count++);

        Assert.AreEqual(1, count);
    }

    /// <summary>
    /// 10. TapBoth の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Result_TapBoth_null_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = ok.TapBoth(null));
    }
}

