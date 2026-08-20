namespace FunctionalCore.Tests.ResultTests;

public class ResultMapTests
{
    /// <summary>
    /// 1. Ok.Map は selector を実行し、変換後の値を持つ成功 Result を返す
    /// </summary>
    [Test]
    public void Result_Ok_Map_should_return_selector_result()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Map(x => x + 1);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. Ok.Map は成功値の型を変更できる
    /// </summary>
    [Test]
    public void Result_Ok_Map_should_change_value_type()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Map(x => $"value:{x}");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. Ok.Map は selector を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_Map_should_invoke_selector_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Map(x =>
        {
            count++;
            return x + 1;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. Fail.Map は selector を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Map_should_not_invoke_selector()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Map(x =>
        {
            count++;
            return x + 1;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. Fail.Map は元の Error を保持する
    /// </summary>
    [Test]
    public void Result_Fail_Map_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Map(x => x + 1);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 6. Map は元の Result を変更しない
    /// </summary>
    [Test]
    public void Result_Map_should_not_modify_original_result()
    {
        var ok = Result<string, int>.Ok(5);
        var fail = Result<string, int>.Fail("error");

        ok.Map(x => x + 1);
        fail.Map(x => x + 1);

        Assert.Multiple(() =>
        {
            Assert.That(ok, Is.EqualTo(Result<string, int>.Ok(5)));
            Assert.That(fail, Is.EqualTo(Result<string, int>.Fail("error")));
        });
    }

    /// <summary>
    /// 7. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_Map_null_selector_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<ArgumentNullException>(() => ok.Map<string>(null!));
    }

    /// <summary>
    /// 8. selector が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_Map_selector_returning_null_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<InvalidOperationException>(() => ok.Map(_ => (string)null!));
    }

    /// <summary>
    /// 9. Fail.Map では null を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Result_Fail_Map_should_not_evaluate_null_returning_selector()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Map(_ => (string)null!);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }
}