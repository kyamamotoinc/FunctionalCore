using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultBindAsyncTests
{
    /// <summary>
    /// 1. Ok.BindAsync は binder を実行し、その Result を返す
    /// </summary>
    [Test]
    public async Task Result_Ok_BindAsync_should_return_binder_result()
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
    /// 2. Ok.BindAsync は成功値の型を変更できる
    /// </summary>
    [Test]
    public async Task Result_Ok_BindAsync_should_change_value_type()
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
    /// 3. Ok.BindAsync の binder が Fail を返した場合は Fail を返す
    /// </summary>
    [Test]
    public async Task Result_Ok_BindAsync_should_return_failure_when_binder_fails()
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
    /// 4. Ok.BindAsync は binder を1回だけ実行する
    /// </summary>
    [Test]
    public async Task Result_Ok_BindAsync_should_invoke_binder_once()
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
    /// 5. Fail.BindAsync は binder を実行しない
    /// </summary>
    [Test]
    public async Task Result_Fail_BindAsync_should_not_invoke_binder()
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
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 6. Fail.BindAsync は元の Error を保持する
    /// </summary>
    [Test]
    public async Task Result_Fail_BindAsync_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");
        var result = await fail.AsTask().BindAsync(x => Task.FromResult(Result<string, int>.Ok(x + 1)));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 7. binder が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_BindAsync_null_binder_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Task<Result<string, string>>>? binder = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () => await ok.AsTask().BindAsync(binder!));
    }

    /// <summary>
    /// 8. resultTask が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_BindAsync_null_result_task_should_throw()
    {
        Task<Result<string, int>>? resultTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask!.BindAsync(x => Task.FromResult(Result<string, int>.Ok(x + 1))));
    }

    /// <summary>
    /// 9. binder が null Task を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_BindAsync_binder_returning_null_task_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Task<Result<string, int>>> binder = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ok.AsTask().BindAsync(binder));
    }

    /// <summary>
    /// 10. binder の Task が未初期化 Result を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_BindAsync_binder_returning_uninitialized_result_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ok.AsTask().BindAsync(_ => Task.FromResult(default(Result<string, string>))));
    }

    /// <summary>
    /// 11. 元の Task が未初期化 Result を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_BindAsync_uninitialized_source_result_should_throw()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await resultTask.BindAsync(x => Task.FromResult(Result<string, int>.Ok(x + 1))));
    }

    /// <summary>
    /// 12. Fail.BindAsync では null Task を返す binder でも実行されない
    /// </summary>
    [Test]
    public async Task Result_Fail_BindAsync_should_not_evaluate_null_task_binder()
    {
        var fail = Result<string, int>.Fail("error");
        Func<int, Task<Result<string, int>>> binder = _ => null!;

        var result = await fail.AsTask().BindAsync(binder);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 13. Fail.BindAsync では未初期化 Result を返す binder でも実行されない
    /// </summary>
    [Test]
    public async Task Result_Fail_BindAsync_should_not_evaluate_uninitialized_binder_result()
    {
        var fail = Result<string, int>.Fail("error");
        var result = await fail.AsTask().BindAsync(_ => Task.FromResult(default(Result<string, string>)));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }
}