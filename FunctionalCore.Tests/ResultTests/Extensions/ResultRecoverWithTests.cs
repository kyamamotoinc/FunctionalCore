using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultRecoverWithTests
{
    /// <summary>
    /// 1. ResultがOkの場合は元のOkをそのまま返し、recoveryを実行しない。
    /// </summary>
    [Test]
    public void Ok_RecoverWith_should_return_original_ok_without_invoking_recovery()
    {
        var ok = Result<string, int>.Ok(10);
        int count = 0;

        var recoveredResult = ok.RecoverWith(error =>
        {
            count++;
            return Result<string, int>.Ok(20);
        });

        Assert.Multiple(() =>
        {
            Assert.That(recoveredResult, Is.EqualTo(ok));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 2. ResultがFailの場合はrecoveryを1回実行し、そのResultを返す。
    /// </summary>
    [Test]
    public void Fail_RecoverWith_should_invoke_recovery_once_and_return_its_result()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;
        string? receivedError = null;

        var recoveredResult = fail.RecoverWith(error =>
        {
            count++;
            receivedError = error;
            return Result<string, int>.Ok(20);
        });

        Assert.Multiple(() =>
        {
            Assert.That(recoveredResult.IsSuccess, Is.True);
            Assert.That(recoveredResult.Value, Is.EqualTo(20));
            Assert.That(count, Is.EqualTo(1));
            Assert.That(receivedError, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 3. Resultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// recoveryは実行しない。
    /// </summary>
    [Test]
    public void Default_RecoverWith_should_throw_invalid_operation_exception_without_invoking_recovery()
    {
        var uninitialized = default(Result<string, int>);
        int count = 0;

        Assert.Throws<InvalidOperationException>(() => uninitialized.RecoverWith(error =>
        {
            count++;
            return Result<string, int>.Ok(20);
        }));

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. ResultがOkの場合でもrecoveryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_RecoverWith_should_throw_argument_null_exception_when_recovery_is_null()
    {
        var ok = Result<string, int>.Ok(10);

        Assert.Throws<ArgumentNullException>(() => ok.RecoverWith(null!));
    }

    /// <summary>
    /// 5. ResultがFailの場合でもrecoveryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_RecoverWith_should_throw_argument_null_exception_when_recovery_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => fail.RecoverWith(null!));
    }

    /// <summary>
    /// 6. Resultがdefaultでrecoveryもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_RecoverWith_should_throw_invalid_operation_exception_before_recovery_null_check()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.RecoverWith(null!));
    }

    /// <summary>
    /// 7. ResultがFailでrecoveryが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_RecoverWith_should_throw_invalid_operation_exception_when_recovery_returns_uninitialized_result()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<InvalidOperationException>(() => fail.RecoverWith(_ => default(Result<string, int>)));
    }

    /// <summary>
    /// 8. ResultがFailでrecoveryがFailを返した場合は、そのFailをそのまま返す。
    /// </summary>
    [Test]
    public void Fail_RecoverWith_should_return_recovered_fail_when_recovery_returns_fail()
    {
        var fail = Result<string, int>.Fail("original error");
        var recoveredResult = fail.RecoverWith(_ => Result<string, int>.Fail("recovery error"));

        Assert.Multiple(() =>
        {
            Assert.That(recoveredResult.IsFailure, Is.True);
            Assert.That(recoveredResult.Error, Is.EqualTo("recovery error"));
        });
    }

    /// <summary>
    /// 9. ResultがFailでrecoveryが例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Fail_RecoverWith_should_propagate_exception_when_recovery_throws()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new NotSupportedException("recovery error");

        var actualException = Assert.Throws<NotSupportedException>(() => fail.RecoverWith(_ => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}