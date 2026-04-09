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
        var res = _ok.Ensure(x => x > 0, x => "invalid");
        Assert.IsTrue(res.IsSuccess);
        Assert.AreEqual(5, res.Value);
    }

    /// <summary>
    /// 2. Ok.Ensure(predicate=false) は Fail に落とす（Ok → Fail）
    /// </summary>
    [Test]
    public void Result_Ok_Ensure_false_should_return_fail()
    {
        var res = _ok.Ensure(x => x < 0, x => "invalid");
        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual("invalid", res.Error);
    }

    /// <summary>
    /// 3. Fail.Ensure は predicate を実行せず Fail のまま（Fail → Fail）
    /// </summary>
    [Test]
    public void Result_Fail_Ensure_should_not_invoke_predicate()
    {
        int count = 0;

        var res = _fail.Ensure(x =>
        {
            count++;
            return true;
        }, x => "invalid");

        Assert.AreEqual(0, count);
        Assert.IsFalse(res.IsSuccess);
        Assert.AreEqual("error", res.Error);
    }

    /// <summary>
    /// 4. predicate が null の場合は ArgumentNullException
    /// </summary>
    [Test]
    public void Result_Ensure_null_predicate_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = _ok.Ensure(null, x => "invalid"));
    }

    /// <summary>
    /// 5. error が null の場合も ArgumentNullException（null 禁止の世界観）
    /// </summary>
    [Test]
    public void Result_Ensure_null_error_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = _ok.Ensure(x => false, null));
    }
}

