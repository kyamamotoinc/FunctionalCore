using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultTapAsyncTests
{
    /// <summary>
    /// 1. ResultがOkの場合はonSuccessを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task Ok_TapAsync_should_invoke_onSuccess_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        await ok.AsTask().TapAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. ResultがOkの場合は成功値をonSuccessに渡す。
    /// </summary>
    [Test]
    public async Task Ok_TapAsync_should_pass_value_to_onSuccess()
    {
        var ok = Result<string, int>.Ok(5);
        int receivedValue = 0;

        await ok.AsTask().TapAsync(value =>
        {
            receivedValue = value;
            return Task.CompletedTask;
        });

        Assert.That(receivedValue, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. ResultがFailの場合はonSuccessを実行しない。
    /// </summary>
    [Test]
    public async Task Fail_TapAsync_should_not_invoke_onSuccess()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = await fail.AsTask().TapAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 4. ResultがOkの場合は元のResultをそのまま返す。
    /// </summary>
    [Test]
    public async Task Ok_TapAsync_should_return_original_result()
    {
        var ok = Result<string, int>.Ok(5);

        var result = await ok.AsTask().TapAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(ok));
    }

    /// <summary>
    /// 5. ResultがFailの場合は元のResultをそのまま返す。
    /// </summary>
    [Test]
    public async Task Fail_TapAsync_should_return_original_result()
    {
        var fail = Result<string, int>.Fail("error");

        var result = await fail.AsTask().TapAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(fail));
    }

    /// <summary>
    /// 6. ResultがOkの場合でもonSuccessがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_TapAsync_should_throw_argument_null_exception_when_onSuccess_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Task>? onSuccess = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ok.AsTask().TapAsync(onSuccess!));
    }

    /// <summary>
    /// 7. ResultがFailの場合でもonSuccessがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_TapAsync_should_throw_argument_null_exception_when_onSuccess_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Func<int, Task>? onSuccess = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await fail.AsTask().TapAsync(onSuccess!));
    }

    /// <summary>
    /// 8. resultTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void TapAsync_should_throw_argument_null_exception_when_resultTask_is_null()
    {
        Task<Result<string, int>>? resultTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask!.TapAsync(_ => Task.CompletedTask));
    }

    /// <summary>
    /// 9. ResultがOkでonSuccessがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_TapAsync_should_throw_invalid_operation_exception_when_onSuccess_returns_null_task()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Task> onSuccess = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ok.AsTask().TapAsync(onSuccess));
    }

    /// <summary>
    /// 10. ResultがFailの場合はnullのTaskを返すonSuccessでも実行せず、元のFailを返す。
    /// </summary>
    [Test]
    public async Task Fail_TapAsync_should_return_original_fail_without_invoking_null_task_onSuccess()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        Func<int, Task> onSuccess = _ =>
        {
            count++;
            return null!;
        };

        var result = await fail.AsTask().TapAsync(onSuccess);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 11. resultTaskが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// onSuccessは実行しない。
    /// </summary>
    [Test]
    public void TapAsync_should_throw_invalid_operation_exception_when_resultTask_returns_uninitialized_result()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));
        int count = 0;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await resultTask.TapAsync(_ =>
            {
                count++;
                return Task.CompletedTask;
            }));

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 12. ResultがOkでonSuccessが同期的に例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_TapAsync_should_propagate_exception_when_onSuccess_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("onSuccess error");

        Func<int, Task> onSuccess = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await ok.AsTask().TapAsync(onSuccess));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 13. ResultがOkでonSuccessが返したTaskが例外で完了した場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_TapAsync_should_propagate_exception_when_onSuccess_task_faults()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("onSuccess task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await ok.AsTask().TapAsync(_ => Task.FromException(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 14. resultTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// onSuccessは実行しない。
    /// </summary>
    [Test]
    public void TapAsync_should_propagate_exception_when_resultTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        Task<Result<string, int>> resultTask = Task.FromException<Result<string, int>>(expectedException);
        int count = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await resultTask.TapAsync(_ =>
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