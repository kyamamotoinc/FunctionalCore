namespace FunctionalCore.Tests.ResultTests;

public class ResultEnsureTests
{
    /// <summary>
    /// 1. ResultがOkでpredicateがtrueを返す場合は元のOkをそのまま返す。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_return_original_ok_when_predicate_returns_true()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Ensure(x => x > 0, _ => "invalid");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(result, Is.EqualTo(ok));
        });
    }

    /// <summary>
    /// 2. ResultがOkでpredicateがfalseを返す場合はerrorFactoryが生成したエラーを持つFailを返す。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_return_fail_when_predicate_returns_false()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Ensure(x => x < 0, _ => "invalid");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("invalid"));
        });
    }

    /// <summary>
    /// 3. ResultがOkの場合はpredicateを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_invoke_predicate_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Ensure(x =>
        {
            count++;
            return x > 0;
        }, _ => "invalid");

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. ResultがOkでpredicateがtrueを返す場合はerrorFactoryを実行しない。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_not_invoke_errorFactory_when_predicate_returns_true()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Ensure(x => x > 0, _ =>
        {
            count++;
            return "invalid";
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. ResultがOkでpredicateがfalseを返す場合はerrorFactoryを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_invoke_errorFactory_once_when_predicate_returns_false()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var result = ok.Ensure(x => x < 0, _ =>
        {
            count++;
            return "invalid";
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("invalid"));
        });
    }

    /// <summary>
    /// 6. ResultがFailの場合はpredicateを実行しない。
    /// </summary>
    [Test]
    public void Fail_Ensure_should_not_invoke_predicate()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Ensure(x =>
        {
            count++;
            return true;
        }, _ => "invalid");

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 7. ResultがFailの場合はerrorFactoryを実行しない。
    /// </summary>
    [Test]
    public void Fail_Ensure_should_not_invoke_errorFactory()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Ensure(_ => false, _ =>
        {
            count++;
            return "invalid";
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 8. ResultがFailの場合は元のFailをそのまま返す。
    /// </summary>
    [Test]
    public void Fail_Ensure_should_return_original_fail()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Ensure(_ => false, _ => "invalid");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(result, Is.EqualTo(fail));
        });
    }

    /// <summary>
    /// 9. ResultがOkの場合でもpredicateがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.Ensure(null!, _ => "invalid"));
    }

    /// <summary>
    /// 10. ResultがFailの場合でもpredicateがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Ensure_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.Ensure(null!, _ => "invalid"));
    }

    /// <summary>
    /// 11. ResultがOkの場合でもerrorFactoryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_throw_argument_null_exception_when_errorFactory_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.Ensure(_ => false, null!));
    }

    /// <summary>
    /// 12. ResultがFailの場合でもerrorFactoryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Ensure_should_throw_argument_null_exception_when_errorFactory_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.Ensure(_ => false, null!));
    }

    /// <summary>
    /// 13. Resultがdefaultでpredicateもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Ensure_should_throw_invalid_operation_exception_before_predicate_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Ensure(null!, _ => "invalid"));
    }

    /// <summary>
    /// 14. ResultがdefaultでerrorFactoryもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Ensure_should_throw_invalid_operation_exception_before_errorFactory_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Ensure(_ => true, null!));
    }

    /// <summary>
    /// 15. ResultがOkでpredicateがfalseを返し、errorFactoryがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_throw_invalid_operation_exception_when_errorFactory_returns_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() => ok.Ensure(_ => false, _ => null!));
    }

    /// <summary>
    /// 16. ResultがOkでpredicateがtrueを返す場合は、
    /// nullを返すerrorFactoryでも実行せず、元のOkを返す。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_return_original_ok_without_invoking_null_returning_errorFactory_when_predicate_returns_true()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var result = ok.Ensure(_ => true, _ =>
        {
            count++;
            return null!;
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
    /// 17. ResultがFailの場合はnullを返すerrorFactoryも実行せず、元のFailを返す。
    /// </summary>
    [Test]
    public void Fail_Ensure_should_return_original_fail_without_invoking_null_returning_errorFactory()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = fail.Ensure(_ => false, _ =>
        {
            count++;
            return null!;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(result, Is.EqualTo(fail));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 18. ResultがOkでpredicateが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// errorFactoryは実行しない。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_propagate_exception_when_predicate_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("predicate error");
        int errorFactoryCount = 0;

        Func<int, bool> predicate = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() =>
            ok.Ensure(predicate, _ =>
            {
                errorFactoryCount++;
                return "invalid";
            }));

        Assert.Multiple(() =>
        {
            Assert.That(actualException, Is.SameAs(expectedException));
            Assert.That(errorFactoryCount, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 19. ResultがOkでpredicateがfalseを返した後にerrorFactoryが例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_Ensure_should_propagate_exception_when_errorFactory_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("errorFactory error");
        Func<int, string> errorFactory = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() =>
            ok.Ensure(_ => false, errorFactory));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}