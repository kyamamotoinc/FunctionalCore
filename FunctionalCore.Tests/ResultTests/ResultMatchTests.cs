namespace FunctionalCore.Tests.ResultTests;

public class ResultMatchTests
{
    private Result<string, int> ok;
    private Result<string, int> fail;

    [SetUp]
    public void Setup()
    {
        ok = Result<string, int>.Ok(5);
        fail = Result<string, int>.Fail("error");
    }

    /// <summary>
    /// 1. Ok.Match は成功側の関数を実行し、その結果を返す
    /// </summary>
    [Test]
    public void Result_Ok_Match_should_invoke_success_func()
    {
        var result = ok.Match(
            ok => ok + 1,
            err => -1
        );

        Assert.AreEqual(6, result);
    }

    /// <summary>
    /// 2. Ok.Match は失敗側の関数を実行しない（排他性）
    /// </summary>
    [Test]
    public void Result_Ok_Match_should_not_invoke_failure_func()
    {
        int count = 0;
        var _ = ok.Match(
            ok => ok + 1,
            err =>
            {
                count++;
                return -1;
            });

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 3. Fail.Match は失敗側の関数を実行し、その結果を返す
    /// </summary>
    [Test]
    public void Result_Fail_Match_should_invoke_failure_func()
    {
        var result = fail.Match(
            ok => ok + 1,
            err => -1
        );

        Assert.AreEqual(-1, result);
    }

    /// <summary>
    /// 4. Fail.Match は成功側の関数を実行しない（排他性）
    /// </summary>
    [Test]
    public void Result_Fail_Match_should_not_invoke_success_func()
    {
        int count = 0;
        var _ = fail.Match(
            ok =>
            {
                count++;
                return ok + 1;
            },
            err => -1
        );

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 5. 成功側 func が null の場合は ArgumentNullException
    /// </summary>
    [Test]
    public void Result_Ok_Match_null_success_func_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _ = ok.Match<int>(null, err => -1)
        );
    }

    /// <summary>
    /// 6. 失敗側 func が null の場合も ArgumentNullException
    /// </summary>
    [Test]
    public void Result_Fail_Match_null_failure_func_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _ = fail.Match(ok => ok + 1, null)
        );
    }

    /// <summary>
    /// 7. 成功側 func が null を返した場合は例外
    /// </summary>
    [Test]
    public void Result_Ok_Match_success_func_returning_null_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _ = ok.Match(
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
            _ = fail.Match(
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
        var okResult = ok.Match(ok => ok + 10, err => -1);
        var failResult = fail.Match(ok => ok + 10, err => -1);

        Assert.IsInstanceOf<int>(okResult);
        Assert.IsInstanceOf<int>(failResult);
    }
}