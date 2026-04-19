namespace FunctionalCore.Tests.ResultTests;

public class ResultMatchTests
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
    /// 1. Ok.Match は成功側の関数を実行し、その結果を返す
    /// </summary>
    [Test]
    public void Result_Ok_Match_should_invoke_success_func()
    {
        var result = _ok.Match(
            ok => ok + 1,
            err => -1
        );

        Assert.That(result, Is.EqualTo(6));
    }

    /// <summary>
    /// 2. Ok.Match は失敗側の関数を実行しない（排他性）
    /// </summary>
    [Test]
    public void Result_Ok_Match_should_not_invoke_failure_func()
    {
        int count = 0;
        var _ = _ok.Match(
            ok => ok + 1,
            err =>
            {
                count++;
                return -1;
            });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 3. Fail.Match は失敗側の関数を実行し、その結果を返す
    /// </summary>
    [Test]
    public void Result_Fail_Match_should_invoke_failure_func()
    {
        var result = _fail.Match(
            ok => ok + 1,
            err => -1
        );

        Assert.That(result, Is.EqualTo(-1));
    }

    /// <summary>
    /// 4. Fail.Match は成功側の関数を実行しない（排他性）
    /// </summary>
    [Test]
    public void Result_Fail_Match_should_not_invoke_success_func()
    {
        int count = 0;
        var _ = _fail.Match(
            ok =>
            {
                count++;
                return ok + 1;
            },
            err => -1
        );

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. 成功側 func が null の場合は ArgumentNullException
    /// </summary>
    [Test]
    public void Result_Ok_Match_null_success_func_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _ = _ok.Match<int>(null, err => -1)
        );
    }

    /// <summary>
    /// 6. 失敗側 func が null の場合も ArgumentNullException
    /// </summary>
    [Test]
    public void Result_Fail_Match_null_failure_func_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _ = _fail.Match(ok => ok + 1, null)
        );
    }

    /// <summary>
    /// 7. 成功側 func が null を返した場合は例外
    /// </summary>
    [Test]
    public void Result_Ok_Match_success_func_returning_null_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _ = _ok.Match(
                ok => (string)null,
                err => "fallback"
            )
        );
    }

    /// <summary>
    /// 8. 失敗側 func が null を返した場合も例外
    /// </summary>
    [Test]
    public void Result_Fail_Match_failure_func_returning_null_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _ = _fail.Match(
                ok => "ok",
                err => (string)null
            )
        );
    }

    /// <summary>
    /// 9. Match は常に結果を返す（型の保証）
    /// </summary>
    [Test]
    public void Result_Match_should_always_return_value()
    {
        var okResult = _ok.Match(ok => ok + 10, err => -1);
        var failResult = _fail.Match(ok => ok + 10, err => -1);

        Assert.IsInstanceOf<int>(okResult);
        Assert.IsInstanceOf<int>(failResult);
    }
}