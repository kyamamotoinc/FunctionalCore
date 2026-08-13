using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultTapBothTests
{
    private Result<string, int> _ok;
    private Result<string, int> _fail;

    [SetUp]
    public void Setup()
    {
        _ok = Result<string, int>.Ok(5);
        _fail = Result<string, int>.Fail("error");
    }

    /// <summary>
    /// 1. Ok.TapBoth は成功側の action だけを実行する
    /// </summary>
    [Test]
    public void Result_Ok_TapBoth_should_invoke_only_success_action()
    {
        int successCount = 0;
        int failureCount = 0;

        _ok.TapBoth(_ => successCount++, _ => failureCount++);

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
        int successCount = 0;
        int failureCount = 0;

        _fail.TapBoth(_ => successCount++, _ => failureCount++);

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
        int received = 0;

        _ok.TapBoth(value => received = value, _ => { });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 4. Fail.TapBoth は Error を失敗側の action に渡す
    /// </summary>
    [Test]
    public void Result_Fail_TapBoth_should_pass_error_to_failure_action()
    {
        string? received = null;

        _fail.TapBoth(_ => { }, error => received = error);

        Assert.That(received, Is.EqualTo("error"));
    }

    /// <summary>
    /// 5. TapBoth は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public void Result_TapBoth_should_return_original_result()
    {
        var okResult = _ok.TapBoth(_ => { }, _ => { });
        var failResult = _fail.TapBoth(_ => { }, _ => { });

        Assert.Multiple(() =>
        {
            Assert.That(okResult, Is.EqualTo(_ok));
            Assert.That(failResult, Is.EqualTo(_fail));
        });
    }

    /// <summary>
    /// 6. TapBoth の成功側 action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_TapBoth_null_success_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ok.TapBoth(null!, _ => { }));
    }

    /// <summary>
    /// 7. TapBoth の失敗側 action が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_TapBoth_null_failure_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _fail.TapBoth(_ => { }, null!));
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