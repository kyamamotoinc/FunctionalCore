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
        _ok.Tap(x => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Tap は失敗レールでは副作用を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Tap_should_not_invoke_action()
    {
        int count = 0;
        _fail.Tap(x => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 3. Tap は Result を変えずに返す（レールを変えない）
    /// </summary>
    [Test]
    public void Result_Tap_should_return_same_result()
    {
        int count = 0;
        var result =  _ok.Tap(x => count++);

        Assert.That(result, Is.EqualTo(_ok));
    }

    /// <summary>
    /// 4. Tap の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Result_Tap_null_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ok.Tap(null!));
    }

    //// -----------------------------
    //// TapError (失敗レールの副作用)
    //// -----------------------------

    ///// <summary>
    ///// 5. TapError は失敗レールで副作用を実行する
    ///// </summary>
    //[Test]
    //public void Result_Fail_TapError_should_invoke_action()
    //{
    //    int count = 0;
    //    _fail.TapError(x => count++);

    //    Assert.That(count, Is.EqualTo(1));
    //}

    ///// <summary>
    ///// 6. TapError は成功レールでは副作用を実行しない
    ///// </summary>
    //[Test]
    //public void Result_Ok_TapError_should_not_invoke_action()
    //{
    //    int count = 0;
    //    _ok.TapError(x => count++);

    //    Assert.That(count, Is.EqualTo(0));
    //}

    ///// <summary>
    ///// 7. TapError の action が null → ArgumentNullException
    ///// </summary>
    //[Test]
    //public void Result_TapError_null_action_should_throw()
    //{
    //    Assert.Throws<ArgumentNullException>(() => _fail.TapError(null!));
    //}

    // -----------------------------
    // TapBoth (成功・失敗問わず副作用)
    // -----------------------------

    /// <summary>
    /// 8. TapBoth は成功レールでも副作用を実行する
    /// </summary>
    //[Test]
    //public void Result_Ok_TapBoth_should_invoke_action()
    //{
    //    int count = 0;


    //    //Assert.AreEqual(1, count);
    //}

    ///// <summary>
    ///// 9. TapBoth は失敗レールでも副作用を実行する
    ///// </summary>
    //[Test]
    //public void Result_Fail_TapBoth_should_invoke_action()
    //{
    //    int count = 0;


    //}

    ///// <summary>
    ///// 10. TapBoth の action が null → ArgumentNullException
    ///// </summary>
    //[Test]
    //public void Result_TapBoth_null_action_should_throw()
    //{
    //}
}

