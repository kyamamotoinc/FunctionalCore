using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultMatchAsyncTests
{
    /// <summary>
    /// 1. ResultがOkの場合はonSuccessを実行し、その戻り値を返す。
    /// </summary>
    [Test]
    public async Task Ok_MatchAsync_should_return_onSuccess_result()
    {
        var ok = Result<string, int>.Ok(5);

        var result = await ok.AsTask().MatchAsync(
            value => Task.FromResult(value + 1),
            _ => Task.FromResult(-1));

        Assert.That(result, Is.EqualTo(6));
    }

    /// <summary>
    /// 2. ResultがOkの場合はonSuccessを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task Ok_MatchAsync_should_invoke_onSuccess_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        await ok.AsTask().MatchAsync(
            value =>
            {
                count++;
                return Task.FromResult(value + 1);
            },
            _ => Task.FromResult(-1));

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 3. ResultがOkの場合はonFailureを実行しない。
    /// </summary>
    [Test]
    public async Task Ok_MatchAsync_should_not_invoke_onFailure()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        await ok.AsTask().MatchAsync(
            value => Task.FromResult(value + 1),
            _ =>
            {
                count++;
                return Task.FromResult(-1);
            });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. ResultがFailの場合はonFailureを実行し、その戻り値を返す。
    /// </summary>
    [Test]
    public async Task Fail_MatchAsync_should_return_onFailure_result()
    {
        var fail = Result<string, int>.Fail("error");

        var result = await fail.AsTask().MatchAsync(
            value => Task.FromResult(value + 1),
            _ => Task.FromResult(-1));

        Assert.That(result, Is.EqualTo(-1));
    }

    /// <summary>
    /// 5. ResultがFailの場合はonFailureを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task Fail_MatchAsync_should_invoke_onFailure_once()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        await fail.AsTask().MatchAsync(
            value => Task.FromResult(value + 1),
            _ =>
            {
                count++;
                return Task.FromResult(-1);
            });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 6. ResultがFailの場合はonSuccessを実行しない。
    /// </summary>
    [Test]
    public async Task Fail_MatchAsync_should_not_invoke_onSuccess()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        await fail.AsTask().MatchAsync(
            value =>
            {
                count++;
                return Task.FromResult(value + 1);
            },
            _ => Task.FromResult(-1));

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 7. ResultがOkの場合でもonSuccessがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_MatchAsync_should_throw_argument_null_exception_when_onSuccess_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Task<int>>? onSuccess = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ok.AsTask().MatchAsync(onSuccess!, _ => Task.FromResult(-1)));
    }

    /// <summary>
    /// 8. ResultがFailの場合でもonFailureがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_MatchAsync_should_throw_argument_null_exception_when_onFailure_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Func<string, Task<int>>? onFailure = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await fail.AsTask().MatchAsync(value => Task.FromResult(value + 1), onFailure!));
    }

    /// <summary>
    /// 9. ResultがOkの場合でも未使用のonFailureがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_MatchAsync_should_throw_argument_null_exception_when_unused_onFailure_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Func<string, Task<int>>? onFailure = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ok.AsTask().MatchAsync(value => Task.FromResult(value + 1), onFailure!));
    }

    /// <summary>
    /// 10. ResultがFailの場合でも未使用のonSuccessがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_MatchAsync_should_throw_argument_null_exception_when_unused_onSuccess_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Func<int, Task<int>>? onSuccess = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await fail.AsTask().MatchAsync(onSuccess!, _ => Task.FromResult(-1)));
    }

    /// <summary>
    /// 11. resultTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void MatchAsync_should_throw_argument_null_exception_when_resultTask_is_null()
    {
        Task<Result<string, int>>? resultTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask!.MatchAsync(
                value => Task.FromResult(value + 1),
                _ => Task.FromResult(-1)));
    }

    /// <summary>
    /// 12. ResultがOkでonSuccessがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_MatchAsync_should_throw_invalid_operation_exception_when_onSuccess_returns_null_task()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Task<int>> onSuccess = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ok.AsTask().MatchAsync(onSuccess, _ => Task.FromResult(-1)));
    }

    /// <summary>
    /// 13. ResultがFailでonFailureがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_MatchAsync_should_throw_invalid_operation_exception_when_onFailure_returns_null_task()
    {
        var fail = Result<string, int>.Fail("error");
        Func<string, Task<int>> onFailure = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await fail.AsTask().MatchAsync(value => Task.FromResult(value + 1), onFailure));
    }

    /// <summary>
    /// 14. 選択された関数のTaskがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void MatchAsync_should_throw_invalid_operation_exception_when_selected_task_returns_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ok.AsTask().MatchAsync(
                _ => Task.FromResult((string)null!),
                _ => Task.FromResult("failure")));
    }

    /// <summary>
    /// 15. resultTaskが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// onSuccessとonFailureは実行しない。
    /// </summary>
    [Test]
    public void MatchAsync_should_throw_invalid_operation_exception_when_resultTask_returns_uninitialized_result()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));
        int successCount = 0;
        int failureCount = 0;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await resultTask.MatchAsync(
                value =>
                {
                    successCount++;
                    return Task.FromResult(value + 1);
                },
                _ =>
                {
                    failureCount++;
                    return Task.FromResult(-1);
                }));

        Assert.Multiple(() =>
        {
            Assert.That(successCount, Is.EqualTo(0));
            Assert.That(failureCount, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 16. 選択された関数が同期的に例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void MatchAsync_should_propagate_exception_when_selected_function_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("match error");

        Func<int, Task<int>> onSuccess = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await ok.AsTask().MatchAsync(onSuccess, _ => Task.FromResult(-1)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 17. 選択された関数が返したTaskが例外で完了した場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void MatchAsync_should_propagate_exception_when_selected_task_faults()
    {
        var fail = Result<string, int>.Fail("error");
        var expectedException = new NotSupportedException("match task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await fail.AsTask().MatchAsync(
                value => Task.FromResult(value + 1),
                _ => Task.FromException<int>(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 18. resultTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// onSuccessとonFailureは実行しない。
    /// </summary>
    [Test]
    public void MatchAsync_should_propagate_exception_when_resultTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        Task<Result<string, int>> resultTask = Task.FromException<Result<string, int>>(expectedException);
        int successCount = 0;
        int failureCount = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await resultTask.MatchAsync(
                value =>
                {
                    successCount++;
                    return Task.FromResult(value + 1);
                },
                _ =>
                {
                    failureCount++;
                    return Task.FromResult(-1);
                }));

        Assert.Multiple(() =>
        {
            Assert.That(actualException, Is.SameAs(expectedException));
            Assert.That(successCount, Is.EqualTo(0));
            Assert.That(failureCount, Is.EqualTo(0));
        });
    }
}