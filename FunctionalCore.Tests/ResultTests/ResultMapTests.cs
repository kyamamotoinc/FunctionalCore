namespace FunctionalCore.Tests.ResultTests;

public class ResultMapTests
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
    /// 1. Ok.Map は selector を実行し、変換結果を持つ成功 Result を返す（成功→成功）
    /// </summary>
    [Test]
    public void Result_Ok_Map_should_return_selector_result()
    {
        var resOk2 = _ok.Map(x => x + 1);
        Assert.That(resOk2.Value, Is.EqualTo(6));
        Assert.That(resOk2.IsSuccess, Is.True);
    }

    /// <summary>
    /// 2. Fail.Map は selector を実行せず、元のエラーを保持する（失敗→失敗）
    /// </summary>
    [Test]
    public void Result_Fail_Map_should_not_invoke_selector_and_keep_error()
    {
        int count = 0;
        var resFail2 = _fail.Map(x =>
        {
            count++;
            return x + 1;
        });

        Assert.That(count, Is.EqualTo(0));
        Assert.That(resFail2.Error, Is.EqualTo("error"));
        Assert.That(resFail2.IsSuccess, Is.False);
    }

    /// <summary>
    /// 3. Map は常に Result を返す（網羅性）
    /// </summary>
    [Test]
    public void Result_Map_should_always_return_result()
    {
        var okResult = _ok.Map(x => x + 1);
        var failResult = _fail.Map(x => x + 1);

        Assert.IsInstanceOf<Result<string, int>>(okResult);
        Assert.IsInstanceOf<Result<string, int>>(failResult);
    }

    /// <summary>
    /// 4. Map は元の Result を変更しない（不変性）
    /// </summary>
    [Test]
    public void Result_Map_should_not_modify_original_result()
    {
        _ok.Map(x => x + 1);
        _fail.Map(x => x + 1);

        Assert.That(_ok, Is.EqualTo(Result<string, int>.Ok(5)));
        Assert.That(_fail, Is.EqualTo(Result<string, int>.Fail("error")));
    }

    /// <summary>
    /// 5. selector が null の場合は ArgumentNullException
    /// </summary>
    [Test]
    public void Result_Ok_Map_null_selector_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = _ok.Map<string>(null!));
    }

    /// <summary>
    /// 6. selector が null を返した場合は InvalidOperationException
    /// </summary>
    [Test]
    public void Result_Ok_Map_selector_returning_null_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = _ok.Map(x => (string)null!));
    }
}