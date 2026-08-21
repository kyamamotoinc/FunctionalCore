namespace FunctionalCore.Tests.ResultTests;

public class ResultMapErrorTests
{
    /// <summary>
    /// 1. ResultがFailの場合はerrorMapperを実行し、変換後のErrorを保持するFailを返す。
    /// </summary>
    [Test]
    public void Fail_MapError_should_return_mapped_error()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.MapError(error => $"mapped:{error}");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("mapped:error"));
        });
    }

    /// <summary>
    /// 2. ResultがFailの場合はerrorMapperによってエラー型を変更できる。
    /// </summary>
    [Test]
    public void Fail_MapError_should_change_error_type()
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
    /// 3. ResultがFailの場合はerrorMapperを1回だけ実行する。
    /// </summary>
    [Test]
    public void Fail_MapError_should_invoke_errorMapper_once()
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
    /// 4. ResultがOkの場合はerrorMapperを実行しない。
    /// </summary>
    [Test]
    public void Ok_MapError_should_not_invoke_errorMapper()
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
    /// 5. ResultがOkの場合は元のValueを保持する。
    /// </summary>
    [Test]
    public void Ok_MapError_should_keep_original_value()
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
    /// 6. MapErrorを実行しても元のResultは変更されない。
    /// </summary>
    [Test]
    public void MapError_should_not_modify_original_result()
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
    /// 7. ResultがFailの場合でもerrorMapperがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_MapError_should_throw_argument_null_exception_when_errorMapper_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.MapError<int>(null!));
    }

    /// <summary>
    /// 8. ResultがOkの場合でもerrorMapperがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_MapError_should_throw_argument_null_exception_when_errorMapper_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.MapError<int>(null!));
    }

    /// <summary>
    /// 9. ResultがdefaultでerrorMapperもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_MapError_should_throw_invalid_operation_exception_before_errorMapper_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.MapError<int>(null!));
    }

    /// <summary>
    /// 10. ResultがFailでerrorMapperがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_MapError_should_throw_invalid_operation_exception_when_errorMapper_returns_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<InvalidOperationException>(() => fail.MapError(_ => (string)null!));
    }

    /// <summary>
    /// 11. ResultがOkの場合はerrorMapperを実行せず、元のOkを返す。
    /// errorMapperがnullを返す関数でも実行されない。
    /// </summary>
    [Test]
    public void Ok_MapError_should_return_original_ok_without_invoking_errorMapper()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var result = ok.MapError(_ =>
        {
            count++;
            return (string)null!;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(result, Is.EqualTo(ok));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 12. ResultがFailでerrorMapperが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Fail_MapError_should_propagate_exception_when_errorMapper_throws()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new NotSupportedException("errorMapper error");
        Func<string, string> errorMapper = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => fail.MapError(errorMapper));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}