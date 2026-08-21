using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultOrTests
{
    /// <summary>
    /// 1. ResultがOkの場合は元のResultをそのまま返す。
    /// </summary>
    [Test]
    public void Ok_Or_should_return_original_result()
    {
        var ok = Result<string, int>.Ok(5);
        var other = Result<string, int>.Ok(10);

        var result = ok.Or(other);

        Assert.That(result, Is.EqualTo(ok));
    }

    /// <summary>
    /// 2. ResultがFailの場合は指定された代替Resultを返す。
    /// </summary>
    [Test]
    public void Fail_Or_should_return_other_result()
    {
        var fail = Result<string, int>.Fail("error");
        var other = Result<string, int>.Ok(10);

        var result = fail.Or(other);

        Assert.That(result, Is.EqualTo(other));
    }

    /// <summary>
    /// 3. ResultがFailで代替ResultもFailの場合は、代替ResultのFailを返す。
    /// </summary>
    [Test]
    public void Fail_Or_should_return_other_fail_when_other_is_fail()
    {
        var fail = Result<string, int>.Fail("error");
        var other = Result<string, int>.Fail("other error");

        var result = fail.Or(other);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("other error"));
        });
    }

    /// <summary>
    /// 4. ResultがOkの場合は代替Resultを使用せず、元の成功値を保持する。
    /// </summary>
    [Test]
    public void Ok_Or_should_keep_original_value()
    {
        var ok = Result<string, int>.Ok(5);
        var other = Result<string, int>.Ok(10);

        var result = ok.Or(other);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 5. ResultがOkの場合は代替Resultが未初期化でも検証せず、元のResultを返す。
    /// </summary>
    [Test]
    public void Ok_Or_should_return_original_result_without_validating_uninitialized_other()
    {
        var ok = Result<string, int>.Ok(5);
        var other = default(Result<string, int>);

        var result = ok.Or(other);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(result, Is.EqualTo(ok));
        });
    }

    /// <summary>
    /// 6. ResultがFailで代替Resultが未初期化の場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Or_should_throw_invalid_operation_exception_when_other_is_uninitialized()
    {
        var fail = Result<string, int>.Fail("error");
        var other = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => fail.Or(other));
    }

    /// <summary>
    /// 7. Resultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Or_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, int>);
        var other = Result<string, int>.Ok(10);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Or(other));
    }

    /// <summary>
    /// 8. ResultがOkの場合はotherFactoryを実行せず、元のResultを返す。
    /// </summary>
    [Test]
    public void Ok_Or_factory_should_return_original_result_without_invoking_otherFactory()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var result = ok.Or(() =>
        {
            count++;
            return Result<string, int>.Ok(10);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(ok));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 9. ResultがFailの場合はotherFactoryを1回だけ実行し、そのResultを返す。
    /// </summary>
    [Test]
    public void Fail_Or_factory_should_invoke_otherFactory_once_and_return_its_result()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = fail.Or(() =>
        {
            count++;
            return Result<string, int>.Ok(10);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(10));
            Assert.That(count, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 10. ResultがFailでotherFactoryがFailを返した場合は、そのFailを返す。
    /// </summary>
    [Test]
    public void Fail_Or_factory_should_return_fail_when_otherFactory_returns_fail()
    {
        var fail = Result<string, int>.Fail("error");

        var result = fail.Or(() => Result<string, int>.Fail("other error"));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("other error"));
        });
    }

    /// <summary>
    /// 11. ResultがOkの場合でもotherFactoryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Or_factory_should_throw_argument_null_exception_when_otherFactory_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => ok.Or((Func<Result<string, int>>)null!));
    }

    /// <summary>
    /// 12. ResultがFailの場合でもotherFactoryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Or_factory_should_throw_argument_null_exception_when_otherFactory_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.Or((Func<Result<string, int>>)null!));
    }

    /// <summary>
    /// 13. ResultがdefaultでotherFactoryもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Or_factory_should_throw_invalid_operation_exception_before_otherFactory_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Or((Func<Result<string, int>>)null!));
    }

    /// <summary>
    /// 14. ResultがFailでotherFactoryが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Or_factory_should_throw_invalid_operation_exception_when_otherFactory_returns_uninitialized_result()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<InvalidOperationException>(() => fail.Or(() => default(Result<string, int>)));
    }

    /// <summary>
    /// 15. ResultがOkの場合は未初期化Resultを返すotherFactoryでも実行せず、元のResultを返す。
    /// </summary>
    [Test]
    public void Ok_Or_factory_should_return_original_result_without_invoking_uninitialized_result_factory()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var result = ok.Or(() =>
        {
            count++;
            return default(Result<string, int>);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(ok));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 16. ResultがFailでotherFactoryが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Fail_Or_factory_should_propagate_exception_when_otherFactory_throws()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new NotSupportedException("factory error");

        var actualException = Assert.Throws<NotSupportedException>(() => fail.Or(() => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 17. ResultがOkの場合は例外を発生させるotherFactoryでも実行せず、元のResultを返す。
    /// </summary>
    [Test]
    public void Ok_Or_factory_should_return_original_result_without_invoking_throwing_otherFactory()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var result = ok.Or(() =>
        {
            count++;
            throw new NotSupportedException("factory error");
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(ok));
            Assert.That(count, Is.EqualTo(0));
        });
    }
}