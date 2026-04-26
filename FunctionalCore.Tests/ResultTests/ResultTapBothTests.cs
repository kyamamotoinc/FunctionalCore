using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests;

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
    /// 8. TapBoth は成功レールでも副作用を実行する
    /// </summary>
    [Test]
    public void Result_Ok_TapBoth_should_invoke_action()
    {
        int count = 0;
        _ok.TapBoth(x => count++, _ => count--);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 9. TapBoth は失敗レールでも副作用を実行する
    /// </summary>
    [Test]
    public void Result_Fail_TapBoth_should_invoke_action()
    {
        int count = 0;
        _fail.TapBoth(x => count++, _ => count--);

        Assert.That(count, Is.EqualTo(-1));
    }

    /// <summary>
    /// 10. TapBoth の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Result_TapBoth_null_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ok.TapBoth(null!, null!));
    }
}

