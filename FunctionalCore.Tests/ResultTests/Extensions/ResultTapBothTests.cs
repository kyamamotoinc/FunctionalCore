using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultTapBothTests
{
    /// <summary>
    /// 1. Ok.TapBoth は成功側の action だけを実行する
    /// </summary>
    [Test]
    public void Result_Ok_TapBoth_should_invoke_only_success_action()
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
    /// 2. Fail.TapBoth は失敗側の action だけを実行する
    /// </summary>
    [Test]
    public void Result_Fail_TapBoth_should_invoke_only_failure_action()
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
    /// 3. Ok.TapBoth は成功値を成功側の action に渡す
    /// </summary>
    [Test]
    public void Result_Ok_TapBoth_should_pass_value_to_success_action()
    {
        var ok = Result<string, int>.Ok(5);
        int received = 0;

        ok.TapBoth(value => received = value, _ => { });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 4. Fail.TapBoth は Error を失敗側の action に渡す
    /// </summary>
    [Test]
    public void Result_Fail_TapBoth_should_pass_error_to_failure_action()
    {
        var fail = Result<string, int>.Fail("error");
        string? received = null;

        fail.TapBoth(_ => { }, error => received = error);

        Assert.That(received, Is.EqualTo("error"));
    }

    /// <summary>
    /// 5. TapBoth は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public void Result_TapBoth_should_return_original_result()
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
    /// 6. TapBoth の成功側 action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_TapBoth_null_success_action_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<ArgumentNullException>(() => ok.TapBoth(null!, _ => { }));
    }

    /// <summary>
    /// 7. TapBoth の失敗側 action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_TapBoth_null_failure_action_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Assert.Throws<ArgumentNullException>(() => fail.TapBoth(_ => { }, null!));
    }

    /// <summary>
    /// 8. 未初期化 Result で TapBoth を呼び出すと InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_TapBoth_should_throw()
    {
        var result = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() =>
            result.TapBoth(_ => { }, _ => { }));
    }
}