using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultEnsureAsyncTests
{
    /// <summary>
    /// 1. ResultがFailの場合は元のFailをそのまま返す。
    /// predicateとerrorFactoryは実行しない。
    /// </summary>
    [Test]
    public async Task Fail_EnsureAsync_should_return_original_fail_without_invoking_predicate_or_errorFactory()
    {
        var fail = Result<string, int>.Fail("error");
        int predicateCount = 0;
        int errorFactoryCount = 0;

        var result = await fail.AsTask().EnsureAsync(
            x => { predicateCount++; return Task.FromResult(x > 0); },
            x => { errorFactoryCount++; return "Value must be positive"; });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(fail));
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(predicateCount, Is.EqualTo(0));
            Assert.That(errorFactoryCount, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 2. ResultがOkでpredicateがtrueを返す場合は元のOkをそのまま返す。
    /// predicateは1回実行し、errorFactoryは実行しない。
    /// </summary>
    [Test]
    public async Task Ok_EnsureAsync_should_return_original_ok_when_predicate_returns_true()
    {
        var ok = Result<string, int>.Ok(5);
        int predicateCount = 0;
        int errorFactoryCount = 0;

        var result = await ok.AsTask().EnsureAsync(
            x => { predicateCount++; return Task.FromResult(x > 0); },
            x => { errorFactoryCount++; return "Value must be positive"; });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(ok));
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(predicateCount, Is.EqualTo(1));
            Assert.That(errorFactoryCount, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 3. ResultがOkでpredicateがfalseを返す場合は、
    /// errorFactoryが生成したErrorを保持するFailを返す。
    /// predicateとerrorFactoryはそれぞれ1回実行する。
    /// </summary>
    [Test]
    public async Task Ok_EnsureAsync_should_return_fail_when_predicate_returns_false()
    {
        var ok = Result<string, int>.Ok(5);
        int predicateCount = 0;
        int errorFactoryCount = 0;

        var result = await ok.AsTask().EnsureAsync(
            x => { predicateCount++; return Task.FromResult(x > 10); },
            x => { errorFactoryCount++; return "Value must be larger than 10"; });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("Value must be larger than 10"));
            Assert.That(predicateCount, Is.EqualTo(1));
            Assert.That(errorFactoryCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 4. resultTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void EnsureAsync_should_throw_argument_null_exception_when_resultTask_is_null()
    {
        Task<Result<string, int>>? resultTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask!.EnsureAsync(x => Task.FromResult(x > 10), x => "Value must be larger than 10"));
    }

    /// <summary>
    /// 5. ResultがOkの場合でもpredicateがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ok.AsTask().EnsureAsync(null!, x => "Predicate is null"));
    }

    /// <summary>
    /// 6. ResultがFailの場合でもpredicateがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_EnsureAsync_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await fail.AsTask().EnsureAsync(null!, x => "Predicate is null"));
    }

    /// <summary>
    /// 7. ResultがOkの場合でもerrorFactoryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_throw_argument_null_exception_when_errorFactory_is_null()
    {
        var ok = Result<string, int>.Ok(10);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ok.AsTask().EnsureAsync(x => Task.FromResult(x > 5), null!));
    }

    /// <summary>
    /// 8. ResultがFailの場合でもerrorFactoryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_EnsureAsync_should_throw_argument_null_exception_when_errorFactory_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await fail.AsTask().EnsureAsync(x => Task.FromResult(x > 5), null!));
    }

    /// <summary>
    /// 9. resultTaskが未初期化Resultを返しpredicateもnullの場合は、
    /// predicateのnullチェックを優先してArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void EnsureAsync_should_throw_argument_null_exception_before_uninitialized_result_check_when_predicate_is_null()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask.EnsureAsync(null!, x => "error"));
    }

    /// <summary>
    /// 10. resultTaskが未初期化Resultを返しerrorFactoryもnullの場合は、
    /// errorFactoryのnullチェックを優先してArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void EnsureAsync_should_throw_argument_null_exception_before_uninitialized_result_check_when_errorFactory_is_null()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask.EnsureAsync(x => Task.FromResult(true), null!));
    }

    /// <summary>
    /// 11. ResultがOkでpredicateがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_throw_invalid_operation_exception_when_predicate_returns_null_task()
    {
        var ok = Result<string, int>.Ok(10);
        Func<int, Task<bool>> predicate = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ok.AsTask().EnsureAsync(predicate, x => "Predicate returns null"));
    }

    /// <summary>
    /// 12. ResultがOkでpredicateが同期的に例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_propagate_exception_when_predicate_throws()
    {
        var ok = Result<string, int>.Ok(10);
        var expectedException = new NotSupportedException("predicate error");
        Func<int, Task<bool>> predicate = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await ok.AsTask().EnsureAsync(predicate, x => "Predicate throws exception"));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 13. ResultがOkでpredicateが返したTaskが例外で完了した場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_propagate_exception_when_predicate_task_faults()
    {
        var ok = Result<string, int>.Ok(10);
        var expectedException = new NotSupportedException("predicate task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await ok.AsTask().EnsureAsync(
                x => Task.FromException<bool>(expectedException),
                x => "Predicate task throws exception"));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 14. ResultがOkでpredicateがfalseを返した後にerrorFactoryが例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_propagate_exception_when_errorFactory_throws()
    {
        var ok = Result<string, int>.Ok(10);
        var expectedException = new NotSupportedException("error factory error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await ok.AsTask().EnsureAsync(x => Task.FromResult(false), x => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 15. ResultがOkでpredicateがfalseを返した後にerrorFactoryがnullを返した場合は、
    /// InvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_throw_invalid_operation_exception_when_errorFactory_returns_null()
    {
        var ok = Result<string, int>.Ok(10);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ok.AsTask().EnsureAsync(x => Task.FromResult(false), x => null!));
    }

    /// <summary>
    /// 16. resultTaskが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// predicateとerrorFactoryは実行しない。
    /// </summary>
    [Test]
    public void EnsureAsync_should_throw_invalid_operation_exception_when_resultTask_returns_uninitialized_result()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));
        int predicateCount = 0;
        int errorFactoryCount = 0;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await resultTask.EnsureAsync(
                x => { predicateCount++; return Task.FromResult(x > 0); },
                x => { errorFactoryCount++; return "Value must be positive"; }));

        Assert.Multiple(() =>
        {
            Assert.That(predicateCount, Is.EqualTo(0));
            Assert.That(errorFactoryCount, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 17. resultTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// predicateとerrorFactoryは実行しない。
    /// </summary>
    [Test]
    public void EnsureAsync_should_propagate_exception_when_resultTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        Task<Result<string, int>> resultTask = Task.FromException<Result<string, int>>(expectedException);
        int predicateCount = 0;
        int errorFactoryCount = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await resultTask.EnsureAsync(
                x => { predicateCount++; return Task.FromResult(x > 0); },
                x => { errorFactoryCount++; return "Value must be positive"; }));

        Assert.Multiple(() =>
        {
            Assert.That(actualException, Is.SameAs(expectedException));
            Assert.That(predicateCount, Is.EqualTo(0));
            Assert.That(errorFactoryCount, Is.EqualTo(0));
        });
    }
}