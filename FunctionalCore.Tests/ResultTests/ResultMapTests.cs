namespace FunctionalCore.Tests.ResultTests;

public class ResultMapTests
{
    /// <summary>
    /// 1. ResultがOkの場合はselectorを実行し、変換後の値を保持するOkを返す。
    /// </summary>
    [Test]
    public void Ok_Map_should_return_mapped_result()
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
    /// 2. ResultがOkの場合はselectorによって成功値の型を変更できる。
    /// </summary>
    [Test]
    public void Ok_Map_should_change_value_type()
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
    /// 3. ResultがOkの場合はselectorを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_Map_should_invoke_selector_once()
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
    /// 4. ResultがFailの場合はselectorを実行しない。
    /// </summary>
    [Test]
    public void Fail_Map_should_not_invoke_selector()
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
    /// 5. ResultがFailの場合は元のErrorを保持する。
    /// </summary>
    [Test]
    public void Fail_Map_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Map(x => x + 1);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 6. Mapを実行しても元のResultは変更されない。
    /// </summary>
    [Test]
    public void Map_should_not_modify_original_result()
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
    /// 7. ResultがOkの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Map_should_throw_argument_null_exception_when_selector_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.Map<string>(null!));
    }

    /// <summary>
    /// 8. ResultがFailの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Map_should_throw_argument_null_exception_when_selector_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.Map<string>(null!));
    }

    /// <summary>
    /// 9. Resultがdefaultでselectorもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Map_should_throw_invalid_operation_exception_before_selector_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Map<string>(null!));
    }

    /// <summary>
    /// 10. ResultがOkでselectorがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Map_should_throw_invalid_operation_exception_when_selector_returns_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() => ok.Map(_ => (string)null!));
    }

    /// <summary>
    /// 11. ResultがFailの場合はselectorを実行せず、元のFailを返す。
    /// selectorがnullを返す関数でも実行されない。
    /// </summary>
    [Test]
    public void Fail_Map_should_return_original_fail_without_invoking_selector()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = fail.Map(_ =>
        {
            count++;
            return (string)null!;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 12. ResultがOkでselectorが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_Map_should_propagate_exception_when_selector_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("selector error");
        Func<int, int> selector = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => ok.Map(selector));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}