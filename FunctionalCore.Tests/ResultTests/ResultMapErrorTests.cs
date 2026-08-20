namespace FunctionalCore.Tests.ResultTests;

public class ResultMapErrorTests
{
    /// <summary>
    /// 1. Fail.MapError は errorMapper を実行し、変換後の Error を持つ Fail を返す
    /// </summary>
    [Test]
    public void Result_Fail_MapError_should_return_mapped_error()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.MapError(error => $"mapped:{error}");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("mapped:error"));
        });
    }

    /// <summary>
    /// 2. Fail.MapError はエラー型を変更できる
    /// </summary>
    [Test]
    public void Result_Fail_MapError_should_change_error_type()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.MapError(error => error.Length);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 3. Fail.MapError は errorMapper を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Fail_MapError_should_invoke_error_mapper_once()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.MapError(error =>
        {
            count++;
            return $"mapped:{error}";
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. Ok.MapError は errorMapper を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_MapError_should_not_invoke_error_mapper()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.MapError(error =>
        {
            count++;
            return $"mapped:{error}";
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. Ok.MapError は元の Value を保持する
    /// </summary>
    [Test]
    public void Result_Ok_MapError_should_keep_original_value()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.MapError(error => $"mapped:{error}");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 6. MapError は元の Result を変更しない
    /// </summary>
    [Test]
    public void Result_MapError_should_not_modify_original_result()
    {
        var ok = Result<string, int>.Ok(5);
        var fail = Result<string, int>.Fail("error");

        ok.MapError(error => $"mapped:{error}");
        fail.MapError(error => $"mapped:{error}");

        Assert.Multiple(() =>
        {
            Assert.That(ok, Is.EqualTo(Result<string, int>.Ok(5)));
            Assert.That(fail, Is.EqualTo(Result<string, int>.Fail("error")));
        });
    }

    /// <summary>
    /// 7. errorMapper が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_MapError_null_error_mapper_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Assert.Throws<ArgumentNullException>(() => fail.MapError<int>(null!));
    }

    /// <summary>
    /// 8. Fail.MapError で errorMapper が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Fail_MapError_error_mapper_returning_null_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Assert.Throws<InvalidOperationException>(() => fail.MapError(_ => (string)null!));
    }

    /// <summary>
    /// 9. Ok.MapError では null を返す errorMapper でも実行されない
    /// </summary>
    [Test]
    public void Result_Ok_MapError_should_not_evaluate_null_returning_error_mapper()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.MapError(_ => (string)null!);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }
}