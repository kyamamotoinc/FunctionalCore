using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultTapErrorAsyncTests
{
    /// <summary>
    /// 1. ResultがFailの場合はonFailureを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task Fail_TapErrorAsync_should_invoke_onFailure_once()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        await fail.AsTask().TapErrorAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. ResultがFailの場合はErrorをonFailureに渡す。
    /// </summary>
    [Test]
    public async Task Fail_TapErrorAsync_should_pass_error_to_onFailure()
    {
        var fail = Result<string, int>.Fail("error");
        string? receivedError = null;

        await fail.AsTask().TapErrorAsync(error =>
        {
            receivedError = error;
            return Task.CompletedTask;
        });

        Assert.That(receivedError, Is.EqualTo("error"));
    }

    /// <summary>
    /// 3. ResultがOkの場合はonFailureを実行しない。
    /// </summary>
    [Test]
    public async Task Ok_TapErrorAsync_should_not_invoke_onFailure()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var result = await ok.AsTask().TapErrorAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 4. ResultがFailの場合は元のResultをそのまま返す。
    /// </summary>
    [Test]
    public async Task Fail_TapErrorAsync_should_return_original_result()
    {
        var fail = Result<string, int>.Fail("error");

        var result = await fail.AsTask().TapErrorAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(fail));
    }

    /// <summary>
    /// 5. ResultがOkの場合は元のResultをそのまま返す。
    /// </summary>
    [Test]
    public async Task Ok_TapErrorAsync_should_return_original_result()
    {
        var ok = Result<string, int>.Ok(5);

        var result = await ok.AsTask().TapErrorAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(ok));
    }

    /// <summary>
    /// 6. ResultがFailの場合でもonFailureがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_TapErrorAsync_should_throw_argument_null_exception_when_onFailure_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Func<string, Task>? onFailure = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await fail.AsTask().TapErrorAsync(onFailure!));
    }

    /// <summary>
    /// 7. ResultがOkの場合でもonFailureがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_TapErrorAsync_should_throw_argument_null_exception_when_onFailure_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Func<string, Task>? onFailure = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ok.AsTask().TapErrorAsync(onFailure!));
    }

    /// <summary>
    /// 8. resultTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void TapErrorAsync_should_throw_argument_null_exception_when_resultTask_is_null()
    {
        Task<Result<string, int>>? resultTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask!.TapErrorAsync(_ => Task.CompletedTask));
    }

    /// <summary>
    /// 9. ResultがFailでonFailureがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_TapErrorAsync_should_throw_invalid_operation_exception_when_onFailure_returns_null_task()
    {
        var fail = Result<string, int>.Fail("error");
        Func<string, Task> onFailure = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await fail.AsTask().TapErrorAsync(onFailure));
    }

    /// <summary>
    /// 10. ResultがOkの場合はnullのTaskを返すonFailureでも実行せず、元のOkを返す。
    /// </summary>
    [Test]
    public async Task Ok_TapErrorAsync_should_return_original_ok_without_invoking_null_task_onFailure()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        Func<string, Task> onFailure = _ =>
        {
            count++;
            return null!;
        };

        var result = await ok.AsTask().TapErrorAsync(onFailure);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(result, Is.EqualTo(ok));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 11. resultTaskが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// onFailureは実行しない。
    /// </summary>
    [Test]
    public void TapErrorAsync_should_throw_invalid_operation_exception_when_resultTask_returns_uninitialized_result()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));
        int count = 0;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await resultTask.TapErrorAsync(_ =>
            {
                count++;
                return Task.CompletedTask;
            }));

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 12. ResultがFailでonFailureが同期的に例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Fail_TapErrorAsync_should_propagate_exception_when_onFailure_throws()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new NotSupportedException("onFailure error");

        Func<string, Task> onFailure = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await fail.AsTask().TapErrorAsync(onFailure));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 13. ResultがFailでonFailureが返したTaskが例外で完了した場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Fail_TapErrorAsync_should_propagate_exception_when_onFailure_task_faults()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new NotSupportedException("onFailure task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await fail.AsTask().TapErrorAsync(_ => Task.FromException(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 14. resultTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// onFailureは実行しない。
    /// </summary>
    [Test]
    public void TapErrorAsync_should_propagate_exception_when_resultTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        Task<Result<string, int>> resultTask = Task.FromException<Result<string, int>>(expectedException);
        int count = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await resultTask.TapErrorAsync(_ =>
            {
                count++;
                return Task.CompletedTask;
            }));

        Assert.Multiple(() =>
        {
            Assert.That(actualException, Is.SameAs(expectedException));
            Assert.That(count, Is.EqualTo(0));
        });
    }
}