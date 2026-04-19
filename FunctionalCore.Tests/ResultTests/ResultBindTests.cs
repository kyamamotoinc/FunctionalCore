namespace FunctionalCore.Tests.ResultTests;


public class ResultBindTests
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
    /// 1. Ok.Bind は binder を実行し、Result を返す（成功→成功）
    /// </summary>
    [Test]
    public void Result_Ok_Bind_should_return_binder_result()
    {
        var result = _ok.Bind(x => Result<string, int>.Ok(x + 1));

        Assert.That(result.IsSuccess);
        Assert.That(result.Value, Is.EqualTo(6));
    }

    /// <summary>
    /// 2. Ok.Bind は成功のまま（成功→成功）
    /// </summary>
    [Test]
    public void Result_Ok_Bind_should_be_success()
    {
        var result = _ok.Bind(x => Result<string, int>.Ok(x + 1));
        Assert.That(result.IsSuccess);
    }

    /// <summary>
    /// 3. Ok.Bind が失敗を返す場合（成功→失敗）
    /// </summary>
    [Test]
    public void Result_Ok_Bind_can_return_failure()
    {
        var result = _ok.Bind(x => Result<string, int>.Fail("bind error"));
        Assert.That(result.Error, Is.EqualTo("bind error"));
    }

    /// <summary>
    /// 4. Fail.Bind は binder を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Bind_should_not_invoke_binder()
    {
        int count = 0;
        var res = _fail.Bind(x =>
        {
            count++;
            return Result<string, int>.Ok(x + 1);
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. Fail.Bind は Error を保持する
    /// </summary>
    [Test]
    public void Result_Fail_Bind_should_keep_error()
    {
        var result = _fail.Bind(x => Result<string, int>.Ok(x + 1));
        Assert.That(result.Error, Is.EqualTo("error"));
    }

    /// <summary>
    /// 6. binder が null の場合は ArgumentNullException
    /// </summary>
    [Test]
    public void Result_Ok_Bind_null_binder_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = _ok.Bind<string>(null!));
    }

    // 7. binder が null を返すケースは Resuil.Ok(null) が禁止されているため型レベルで不可能 → テスト対象外
}
