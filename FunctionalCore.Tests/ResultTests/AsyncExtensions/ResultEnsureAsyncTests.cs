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
        var predicateCount = 0;
        var errorFactoryCount = 0;

        var result = await fail.AsTask().EnsureAsync(
            x => { predicateCount++; return Task.FromResult(x > 0); },
            x => { errorFactoryCount++; return "Value must be positive"; });

        Assert.Multiple(() =>
        {
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
        var predicateCount = 0;
        var errorFactoryCount = 0;

        var result = await ok.AsTask().EnsureAsync(
            x => { predicateCount++; return Task.FromResult(x > 0); },
            x => { errorFactoryCount++; return "Value must be positive"; });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(predicateCount, Is.EqualTo(1));
            Assert.That(errorFactoryCount, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 3. ResultがOkでpredicateがfalseを返す場合は、
    /// errorFactoryが生成したエラーを保持するFailを返す。
    /// predicateとerrorFactoryはそれぞれ1回実行する。
    /// </summary>
    [Test]
    public async Task Ok_EnsureAsync_should_return_fail_when_predicate_returns_false()
    {
        var ok = Result<string, int>.Ok(5);
        var predicateCount = 0;
        var errorFactoryCount = 0;

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
        {
            await resultTask!.EnsureAsync(x => Task.FromResult(x > 10), x => "Value must be larger than 10");
        });
    }

    /// <summary>
    /// 5. ResultがOkの場合でもpredicateがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await ok.AsTask().EnsureAsync(null!, x => "Predicate is null");
        });
    }

    /// <summary>
    /// 6. ResultがFailの場合でもpredicateがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_EnsureAsync_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await fail.AsTask().EnsureAsync(null!, x => "Predicate is null");
        });
    }

    /// <summary>
    /// 7. ResultがOkの場合でもerrorFactoryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_throw_argument_null_exception_when_errorFactory_is_null()
    {
        var ok = Result<string, int>.Ok(10);

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await ok.AsTask().EnsureAsync(x => Task.FromResult(x > 5), null!);
        });
    }

    /// <summary>
    /// 8. ResultがFailの場合でもerrorFactoryがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_EnsureAsync_should_throw_argument_null_exception_when_errorFactory_is_null()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await fail.AsTask().EnsureAsync(x => Task.FromResult(x > 5), null!);
        });
    }

    /// <summary>
    /// 9. predicateがnullのTaskを返す場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_throw_invalid_operation_exception_when_predicate_returns_null_task()
    {
        var ok = Result<string, int>.Ok(10);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await ok.AsTask().EnsureAsync(x => null!, x => "Predicate returns null");
        });
    }

    /// <summary>
    /// 10. predicateが同期的に例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_propagate_exception_when_predicate_throws()
    {
        var ok = Result<string, int>.Ok(10);
        var exception = new NotSupportedException("predicate error");

        var actual = Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            await ok.AsTask().EnsureAsync(x => throw exception, x => "Predicate throws exception");
        });

        Assert.That(actual, Is.SameAs(exception));
    }

    /// <summary>
    /// 11. predicateが返したTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_propagate_exception_when_predicate_task_faults()
    {
        var ok = Result<string, int>.Ok(10);
        var exception = new NotSupportedException("predicate error");

        var actual = Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            await ok.AsTask().EnsureAsync(x => Task.FromException<bool>(exception), x => "Predicate task throws exception");
        });

        Assert.That(actual, Is.SameAs(exception));
    }

    /// <summary>
    /// 12. predicateがfalseを返した後にerrorFactoryが例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_propagate_exception_when_errorFactory_throws()
    {
        var ok = Result<string, int>.Ok(10);
        var exception = new NotSupportedException("error factory error");

        var actual = Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            await ok.AsTask().EnsureAsync(x => Task.FromResult(x > 20), x => throw exception);
        });

        Assert.That(actual, Is.SameAs(exception));
    }

    /// <summary>
    /// 13. predicateがfalseを返した後にerrorFactoryがnullを返す場合は、
    /// InvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_EnsureAsync_should_throw_invalid_operation_exception_when_errorFactory_returns_null()
    {
        var ok = Result<string, int>.Ok(10);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await ok.AsTask().EnsureAsync(x => Task.FromResult(false), x => null!);
        });
    }

    /// <summary>
    /// 14. resultTaskがdefaultのResultを返す場合はInvalidOperationExceptionを発生させる。
    /// predicateとerrorFactoryは実行しない。
    /// </summary>
    [Test]
    public void EnsureAsync_should_throw_invalid_operation_exception_when_resultTask_returns_default_result()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));
        var predicateCount = 0;
        var errorFactoryCount = 0;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await resultTask.EnsureAsync(
                x => { predicateCount++; return Task.FromResult(x > 0); },
                x => { errorFactoryCount++; return "Value must be positive"; });
        });

        Assert.Multiple(() =>
        {
            Assert.That(predicateCount, Is.EqualTo(0));
            Assert.That(errorFactoryCount, Is.EqualTo(0));
        });
    }
}