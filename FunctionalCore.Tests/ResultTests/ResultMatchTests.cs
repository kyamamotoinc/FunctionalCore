namespace FunctionalCore.Tests.ResultTests;

public class ResultMatchTests
{
    /// <summary>
    /// 1. Ok.Match は成功側の関数を実行し、その結果を返す
    /// </summary>
    [Test]
    public void Result_Ok_Match_should_return_success_func_result()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Match(value => value + 1, _ => -1);

        Assert.That(result, Is.EqualTo(6));
    }

    /// <summary>
    /// 2. Ok.Match は成功側の関数を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_Match_should_invoke_success_func_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Match(value =>
        {
            count++;
            return value + 1;
        }, _ => -1);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 3. Ok.Match は失敗側の関数を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_Match_should_not_invoke_failure_func()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Match(value => value + 1, _ =>
        {
            count++;
            return -1;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. Fail.Match は失敗側の関数を実行し、その結果を返す
    /// </summary>
    [Test]
    public void Result_Fail_Match_should_return_failure_func_result()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Match(value => value + 1, _ => -1);

        Assert.That(result, Is.EqualTo(-1));
    }

    /// <summary>
    /// 5. Fail.Match は失敗側の関数を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Fail_Match_should_invoke_failure_func_once()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Match(value => value + 1, _ =>
        {
            count++;
            return -1;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 6. Fail.Match は成功側の関数を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Match_should_not_invoke_success_func()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Match(value =>
        {
            count++;
            return value + 1;
        }, _ => -1);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 7. 成功側の関数が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Match_null_success_func_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<ArgumentNullException>(() => ok.Match<int>(null!, _ => -1));
    }

    /// <summary>
    /// 8. 失敗側の関数が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Match_null_failure_func_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Assert.Throws<ArgumentNullException>(() => fail.Match(value => value + 1, null!));
    }

    /// <summary>
    /// 9. Ok.Match で成功側の関数が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_Match_success_func_returning_null_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<InvalidOperationException>(() => ok.Match(_ => (string)null!, _ => "fallback"));
    }

    /// <summary>
    /// 10. Fail.Match で失敗側の関数が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Fail_Match_failure_func_returning_null_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Assert.Throws<InvalidOperationException>(() => fail.Match(_ => "success", _ => (string)null!));
    }

    /// <summary>
    /// 11. Ok.Match では null を返す失敗側の関数でも実行されない
    /// </summary>
    [Test]
    public void Result_Ok_Match_should_not_evaluate_null_returning_failure_func()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Match(value => $"value:{value}", _ => (string)null!);

        Assert.That(result, Is.EqualTo("value:5"));
    }

    /// <summary>
    /// 12. Fail.Match では null を返す成功側の関数でも実行されない
    /// </summary>
    [Test]
    public void Result_Fail_Match_should_not_evaluate_null_returning_success_func()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Match(_ => (string)null!, error => $"error:{error}");

        Assert.That(result, Is.EqualTo("error:error"));
    }

    /// <summary>
    /// 13. 未初期化 Result で Match(Func, Func) を呼び出すと InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_Match_should_throw()
    {
        var result = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() =>
            result.Match(value => value + 1, _ => -1));
    }

    /// <summary>
    /// 14. Ok でも失敗側の関数が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_Match_null_unused_failure_func_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Func<string, int>? onFailure = null;

        Assert.Throws<ArgumentNullException>(() =>
            ok.Match(value => value + 1, onFailure!));
    }

    /// <summary>
    /// 15. Fail でも成功側の関数が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Fail_Match_null_unused_success_func_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Func<int, int>? onSuccess = null;

        Assert.Throws<ArgumentNullException>(() =>
            fail.Match(onSuccess!, _ => -1));
    }
}