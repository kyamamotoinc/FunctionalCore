namespace FunctionalCore.Tests.ResultTests;

public class ResultTapErrorTests
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
    /// 1. TapError は失敗レールで副作用を実行する
    /// </summary>
    [Test]
    public void Result_Fail_TapError_should_invoke_action()
    {
        int count = 0;
        _fail.TapError(x => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. TapError は成功レールでは副作用を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_TapError_should_not_invoke_action()
    {
        int count = 0;
        _ok.TapError(x => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 3. TapError の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Result_TapError_null_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _fail.TapError(null!));
    }
}

