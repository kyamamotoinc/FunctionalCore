namespace FunctionalCore.Tests.ResultTests;

public class ResultBindTests
{
    /// <summary>
    /// 1. Ok.Bind は binder を実行し、その Result を返す
    /// </summary>
    [Test]
    public void Result_Ok_Bind_should_return_binder_result()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Bind(x => Result<string, int>.Ok(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. Ok.Bind は成功値の型を変更できる
    /// </summary>
    [Test]
    public void Result_Ok_Bind_should_change_value_type()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Bind(x => Result<string, string>.Ok($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. Ok.Bind の binder が Fail を返した場合は失敗になる
    /// </summary>
    [Test]
    public void Result_Ok_Bind_should_return_failure_when_binder_fails()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Bind(_ => Result<string, int>.Fail("bind error"));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("bind error"));
        });
    }

    /// <summary>
    /// 4. Ok.Bind は binder を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_Bind_should_invoke_binder_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Bind(x =>
        {
            count++;
            return Result<string, int>.Ok(x + 1);
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. Fail.Bind は binder を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Bind_should_not_invoke_binder()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Bind(x =>
        {
            count++;
            return Result<string, int>.Ok(x + 1);
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 6. Fail.Bind は元の Error を保持する
    /// </summary>
    [Test]
    public void Result_Fail_Bind_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Bind(x => Result<string, int>.Ok(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 7. binder が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_Bind_null_binder_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<ArgumentNullException>(() => ok.Bind<string>(null!));
    }

    /// <summary>
    /// 8. binder が未初期化 Result を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_Bind_uninitialized_result_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<InvalidOperationException>(() => ok.Bind(_ => default(Result<string, string>)));
    }

    /// <summary>
    /// 9. Fail.Bind では binder が未初期化 Result を返す関数でも実行されない
    /// </summary>
    [Test]
    public void Result_Fail_Bind_should_not_evaluate_uninitialized_binder_result()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Bind(_ => default(Result<string, string>));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }
}