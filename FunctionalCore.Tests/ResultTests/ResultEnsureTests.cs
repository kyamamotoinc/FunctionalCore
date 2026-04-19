namespace FunctionalCore.Tests.ResultTests;

public class ResultEnsureTests
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
    /// 1. Ok.Ensure(predicate=true) は Ok のまま（Ok → Ok）
    /// </summary>
    [Test]
    public void Result_Ok_Ensure_true_should_keep_ok()
    {
        var result = _ok.Ensure(x => x > 0, x => "invalid");

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Ok.Ensure(predicate=false) は Fail に落とす（Ok → Fail）
    /// </summary>
    [Test]
    public void Result_Ok_Ensure_false_should_return_fail()
    {
        var result = _ok.Ensure(x => x < 0, x => "invalid");

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("invalid"));
    }

    /// <summary>
    /// 3. Fail.Ensure は predicate を実行せず Fail のまま（Fail → Fail）
    /// </summary>
    [Test]
    public void Result_Fail_Ensure_should_not_invoke_predicate()
    {
        int count = 0;

        var result = _fail.Ensure(x =>
        {
            count++;
            return true;
        }, x => "invalid");

        Assert.That(count, Is.EqualTo(0));
        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("error"));
    }

    /// <summary>
    /// 4. predicate が null の場合は ArgumentNullException
    /// </summary>
    [Test]
    public void Result_Ensure_null_predicate_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = _ok.Ensure(null!, x => "invalid"));
    }

    /// <summary>
    /// 5. error が null の場合も ArgumentNullException（null 禁止の世界観）
    /// </summary>
    [Test]
    public void Result_Ensure_null_error_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = _ok.Ensure(x => false, null!));
    }
}

