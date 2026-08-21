using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultValueOrThrowTests
{
    /// <summary>
    /// 1. ResultがOkの場合は成功値を返す。
    /// </summary>
    [Test]
    public void Ok_ValueOrThrow_should_return_value()
    {
        var ok = Result<string, int>.Ok(5);

        var value = ok.ValueOrThrow(error => new InvalidOperationException(error));

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. ResultがOkの場合はtoExceptionを実行しない。
    /// </summary>
    [Test]
    public void Ok_ValueOrThrow_should_not_invoke_toException()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var value = ok.ValueOrThrow(error =>
        {
            count++;
            return new InvalidOperationException(error);
        });

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 3. ResultがFailの場合はtoExceptionが生成した例外をスローする。
    /// </summary>
    [Test]
    public void Fail_ValueOrThrow_should_throw_exception_created_by_toException()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<InvalidOperationException>(() =>
            fail.ValueOrThrow(error => new InvalidOperationException(error)));
    }

    /// <summary>
    /// 4. ResultがFailの場合はErrorをtoExceptionに渡す。
    /// </summary>
    [Test]
    public void Fail_ValueOrThrow_should_pass_error_to_toException()
    {
        var fail = Result<string, int>.Fail("error");
        string? receivedError = null;

        Assert.Throws<InvalidOperationException>(() => fail.ValueOrThrow(error =>
        {
            receivedError = error;
            return new InvalidOperationException(error);
        }));

        Assert.That(receivedError, Is.EqualTo("error"));
    }

    /// <summary>
    /// 5. ResultがFailの場合はtoExceptionを1回だけ実行する。
    /// </summary>
    [Test]
    public void Fail_ValueOrThrow_should_invoke_toException_once()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        Assert.Throws<InvalidOperationException>(() => fail.ValueOrThrow(error =>
        {
            count++;
            return new InvalidOperationException(error);
        }));

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 6. ResultがFailの場合はtoExceptionが生成した例外インスタンスをそのままスローする。
    /// </summary>
    [Test]
    public void Fail_ValueOrThrow_should_throw_same_exception_instance_created_by_toException()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new InvalidOperationException("expected");

        var actualException = Assert.Throws<InvalidOperationException>(() =>
            fail.ValueOrThrow(_ => expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 7. ResultがOkの場合でもtoExceptionがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_ValueOrThrow_should_throw_argument_null_exception_when_toException_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.ValueOrThrow(null!));
    }

    /// <summary>
    /// 8. ResultがFailの場合でもtoExceptionがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_ValueOrThrow_should_throw_argument_null_exception_when_toException_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.ValueOrThrow(null!));
    }

    /// <summary>
    /// 9. ResultがdefaultでtoExceptionもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_ValueOrThrow_should_throw_invalid_operation_exception_before_toException_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.ValueOrThrow(null!));
    }

    /// <summary>
    /// 10. ResultがFailでtoExceptionがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_ValueOrThrow_should_throw_invalid_operation_exception_when_toException_returns_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<InvalidOperationException>(() => fail.ValueOrThrow(_ => null!));
    }

    /// <summary>
    /// 11. ResultがOkの場合はnullを返すtoExceptionでも実行せず、成功値を返す。
    /// </summary>
    [Test]
    public void Ok_ValueOrThrow_should_return_value_without_invoking_null_returning_toException()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var value = ok.ValueOrThrow(_ =>
        {
            count++;
            return null!;
        });

        Assert.Multiple(() =>
        {
            Assert.That(value, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 12. Resultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_ValueOrThrow_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() =>
            uninitialized.ValueOrThrow(error => new Exception(error)));
    }

    /// <summary>
    /// 13. ResultがFailでtoExceptionが例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Fail_ValueOrThrow_should_propagate_exception_when_toException_throws()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new NotSupportedException("factory error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            fail.ValueOrThrow(_ => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}