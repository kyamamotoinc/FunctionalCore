namespace FunctionalCore.Tests.ResultTests;

public class ResultBindTests
{
    /// <summary>
    /// 1. ResultがOkの場合はbinderを実行し、そのResultを返す。
    /// </summary>
    [Test]
    public void Ok_Bind_should_return_binder_result()
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
    /// 2. ResultがOkの場合はbinderによって成功値の型を変更できる。
    /// </summary>
    [Test]
    public void Ok_Bind_should_change_value_type()
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
    /// 3. ResultがOkでbinderがFailを返した場合は、そのFailを返す。
    /// </summary>
    [Test]
    public void Ok_Bind_should_return_fail_when_binder_returns_fail()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Bind(_ => Result<string, int>.Fail("bind error"));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("bind error"));
        });
    }

    /// <summary>
    /// 4. ResultがOkの場合はbinderを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_Bind_should_invoke_binder_once()
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
    /// 5. ResultがFailの場合はbinderを実行しない。
    /// </summary>
    [Test]
    public void Fail_Bind_should_not_invoke_binder()
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
    /// 6. ResultがFailの場合は元のErrorを保持する。
    /// </summary>
    [Test]
    public void Fail_Bind_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Bind(x => Result<string, int>.Ok(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 7. ResultがOkの場合でもbinderがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Bind_should_throw_argument_null_exception_when_binder_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.Bind<string>(null!));
    }

    /// <summary>
    /// 8. ResultがFailの場合でもbinderがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Bind_should_throw_argument_null_exception_when_binder_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.Bind<string>(null!));
    }

    /// <summary>
    /// 9. Resultがdefaultでbinderもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Bind_should_throw_invalid_operation_exception_before_binder_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Bind<string>(null!));
    }

    /// <summary>
    /// 10. ResultがOkでbinderが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Bind_should_throw_invalid_operation_exception_when_binder_returns_uninitialized_result()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() => ok.Bind(_ => default(Result<string, string>)));
    }

    /// <summary>
    /// 11. ResultがFailの場合はbinderを実行せず、元のFailを返す。
    /// binderが未初期化Resultを返す関数でも実行されない。
    /// </summary>
    [Test]
    public void Fail_Bind_should_return_original_fail_without_invoking_binder()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = fail.Bind(_ =>
        {
            count++;
            return default(Result<string, string>);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }
}