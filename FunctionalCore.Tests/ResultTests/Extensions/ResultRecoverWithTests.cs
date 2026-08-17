using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultRecoverWithTests
{
    /// <summary>
    /// 1. ResultがOkの場合、元の成功値を維持し、recoveryを実行しない。
    /// </summary>
    [Test]
    public void RecoverWith_should_not_invoke_recovery_when_result_is_ok()
    {
        var result = Result<string, int>.Ok(10);
        int count = 0;

        var actual = result.RecoverWith(error =>
        {
            count++;
            return Result<string, int>.Ok(20);
        });

        Assert.Multiple(() =>
        {
            Assert.That(actual, Is.EqualTo(result));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 2. ResultがFailの場合、recovery関数を実行し、その結果を返す。
    /// </summary>
    [Test]
    public void RecoverWith_should_invoke_recovery_when_result_is_fail()
    {
        var result = Result<string, int>.Fail("error");
        int count = 0;
        string? receivedError = null;

        var actual = result.RecoverWith(error =>
        {
            count++;
            receivedError = error;
            return Result<string, int>.Ok(20);
        });

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsSuccess, Is.True);
            Assert.That(actual.Value, Is.EqualTo(20));
            Assert.That(count, Is.EqualTo(1));
            Assert.That(receivedError, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 3. Resultがdefaultの場合は InvalidOperationException を発生させる。
    ///    recovery関数は実行されない。
    /// </summary>
    [Test]
    public void RecoverWith_should_throw_exception_when_result_is_default()
    {
        var result = default(Result<string, int>);
        int count = 0;

        Assert.Throws<InvalidOperationException>(() => result.RecoverWith(error =>
        {
            count++;
            return Result<string, int>.Ok(20);
        }));

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. ResultがOkの場合でもrecovery関数がnullの場合は ArgumentNullException を発生させる。
    /// </summary>
    [Test]
    public void Ok_RecoverWith_should_throw_exception_when_recovery_function_is_null()
    {
        var result = Result<string, int>.Ok(10);

        Assert.Throws<ArgumentNullException>(() => result.RecoverWith(null!));
    }

    /// <summary>
    /// 5. ResultがFailの場合でもrecovery関数がnullの場合は ArgumentNullException を発生させる。
    /// </summary>
    [Test]
    public void Fail_RecoverWith_should_throw_exception_when_recovery_function_is_null()
    {
        var result = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => result.RecoverWith(null!));
    }

    /// <summary>
    /// 6. Resultがdefaultでrecovery関数もnullの場合は、
    /// Resultの未初期化を優先して InvalidOperationException を発生させる。
    /// </summary>
    [Test]
    public void Default_RecoverWith_should_throw_invalid_operation_exception_before_null_recovery_check()
    {
        var result = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => result.RecoverWith(null!));
    }

    /// <summary>
    /// 7. recovery関数がdefaultのResultを返した場合は InvalidOperationException を発生させる。
    /// </summary>
    [Test]
    public void RecoverWith_should_throw_invalid_operation_exception_when_recovery_function_returns_default_value()
    {
        var result = Result<string, int>.Fail("error");

        Assert.Throws<InvalidOperationException>(() => result.RecoverWith(_ => default(Result<string, int>)));
    }

    /// <summary>
    /// 8. recovery関数がFail結果を返した場合は、そのFail結果を返す。
    /// </summary>
    [Test]
    public void RecoverWith_should_return_recovered_Fail_result_when_recovery_function_returns_Fail_result()
    {
        var original = Result<string, int>.Fail("original error");
        var actual = original.RecoverWith(_ => Result<string, int>.Fail("recovery error"));

        Assert.Multiple(() =>
        {
            Assert.That(actual.IsFailure, Is.True);
            Assert.That(actual.Error, Is.EqualTo("recovery error"));
        });
    }

    /// <summary>
    /// 9. recovery関数が例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void RecoverWith_should_propagate_exception_thrown_by_recovery_function()
    {
        var result = Result<string, int>.Fail("error");
        var exception = new NotSupportedException("recovery error");

        var actual = Assert.Throws<NotSupportedException>(() => result.RecoverWith(_ => throw exception));

        Assert.That(actual, Is.SameAs(exception));
    }
}