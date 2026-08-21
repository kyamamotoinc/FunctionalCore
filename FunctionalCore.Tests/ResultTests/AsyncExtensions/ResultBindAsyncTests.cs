using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultBindAsyncTests
{
    /// <summary>
    /// 1. ResultがOkの場合はbinderを実行し、そのResultを返す。
    /// </summary>
    [Test]
    public async Task Ok_BindAsync_should_return_binder_result()
    {
        var ok = Result<string, int>.Ok(5);

        var result = await ok.AsTask().BindAsync(x => Task.FromResult(Result<string, int>.Ok(x + 1)));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. ResultがOkの場合はbinderによって成功値の型を変更できる。
    /// </summary>
    [Test]
    public async Task Ok_BindAsync_should_change_value_type()
    {
        var ok = Result<string, int>.Ok(5);

        var result = await ok.AsTask().BindAsync(x => Task.FromResult(Result<string, string>.Ok($"value:{x}")));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. ResultがOkでbinderがFailを返した場合は、そのFailを返す。
    /// </summary>
    [Test]
    public async Task Ok_BindAsync_should_return_fail_when_binder_returns_fail()
    {
        var ok = Result<string, int>.Ok(5);

        var result = await ok.AsTask().BindAsync(_ => Task.FromResult(Result<string, int>.Fail("bind error")));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("bind error"));
        });
    }

    /// <summary>
    /// 4. ResultがOkの場合はbinderを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task Ok_BindAsync_should_invoke_binder_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        await ok.AsTask().BindAsync(x =>
        {
            count++;
            return Task.FromResult(Result<string, int>.Ok(x + 1));
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. ResultがFailの場合はbinderを実行しない。
    /// </summary>
    [Test]
    public async Task Fail_BindAsync_should_not_invoke_binder()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = await fail.AsTask().BindAsync(x =>
        {
            count++;
            return Task.FromResult(Result<string, int>.Ok(x + 1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 6. ResultがFailの場合は元のErrorを保持する。
    /// </summary>
    [Test]
    public async Task Fail_BindAsync_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");

        var result = await fail.AsTask().BindAsync(x => Task.FromResult(Result<string, int>.Ok(x + 1)));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 7. ResultがOkの場合でもbinderがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_BindAsync_should_throw_argument_null_exception_when_binder_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Task<Result<string, string>>>? binder = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () => await ok.AsTask().BindAsync(binder!));
    }

    /// <summary>
    /// 8. ResultがFailの場合でもbinderがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_BindAsync_should_throw_argument_null_exception_when_binder_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Func<int, Task<Result<string, string>>>? binder = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () => await fail.AsTask().BindAsync(binder!));
    }

    /// <summary>
    /// 9. resultTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void BindAsync_should_throw_argument_null_exception_when_resultTask_is_null()
    {
        Task<Result<string, int>>? resultTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask!.BindAsync(x => Task.FromResult(Result<string, int>.Ok(x + 1))));
    }

    /// <summary>
    /// 10. resultTaskが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// binderは実行しない。
    /// </summary>
    [Test]
    public void BindAsync_should_throw_invalid_operation_exception_when_resultTask_returns_uninitialized_result()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));
        int count = 0;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await resultTask.BindAsync(x =>
            {
                count++;
                return Task.FromResult(Result<string, int>.Ok(x + 1));
            }));

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 11. ResultがOkでbinderがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_BindAsync_should_throw_invalid_operation_exception_when_binder_returns_null_task()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Task<Result<string, int>>> binder = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () => await ok.AsTask().BindAsync(binder));
    }

    /// <summary>
    /// 12. ResultがOkでbinderのTaskが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_BindAsync_should_throw_invalid_operation_exception_when_binder_task_returns_uninitialized_result()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ok.AsTask().BindAsync(_ => Task.FromResult(default(Result<string, string>))));
    }

    /// <summary>
    /// 13. ResultがFailの場合はnullのTaskを返すbinderでも実行せず、元のFailを返す。
    /// </summary>
    [Test]
    public async Task Fail_BindAsync_should_return_original_fail_without_invoking_null_task_binder()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        Func<int, Task<Result<string, int>>> binder = _ =>
        {
            count++;
            return null!;
        };

        var result = await fail.AsTask().BindAsync(binder);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 14. ResultがFailの場合は未初期化Resultを返すbinderでも実行せず、元のFailを返す。
    /// </summary>
    [Test]
    public async Task Fail_BindAsync_should_return_original_fail_without_invoking_uninitialized_result_binder()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = await fail.AsTask().BindAsync(_ =>
        {
            count++;
            return Task.FromResult(default(Result<string, string>));
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 15. ResultがOkでbinderが同期的に例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_BindAsync_should_propagate_exception_when_binder_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("binder error");
        Func<int, Task<Result<string, int>>> binder = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await ok.AsTask().BindAsync(binder));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 16. ResultがOkでbinderが返したTaskが例外で完了した場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_BindAsync_should_propagate_exception_when_binder_task_faults()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("binder task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await ok.AsTask().BindAsync(_ => Task.FromException<Result<string, int>>(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 17. resultTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// binderは実行しない。
    /// </summary>
    [Test]
    public void BindAsync_should_propagate_exception_when_resultTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        var resultTask = Task.FromException<Result<string, int>>(expectedException);
        int count = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await resultTask.BindAsync(x =>
            {
                count++;
                return Task.FromResult(Result<string, int>.Ok(x + 1));
            }));

        Assert.Multiple(() =>
        {
            Assert.That(actualException, Is.SameAs(expectedException));
            Assert.That(count, Is.EqualTo(0));
        });
    }
}