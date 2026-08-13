using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultTapErrorAsyncTests
{
    private Result<string, int> _ok;
    private Result<string, int> _fail;

    [SetUp]
    public void Setup()
    {
        _ok = Result<string, int>.Ok(5);
        _fail = Result<string, int>.Fail("error");
    }

    /// <summary>
    /// 1. Fail.TapErrorAsync は onFailure を1回だけ実行する
    /// </summary>
    [Test]
    public async Task Result_Fail_TapErrorAsync_should_invoke_action_once()
    {
        int count = 0;

        await _fail.AsTask().TapErrorAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Fail.TapErrorAsync は Error を onFailure に渡す
    /// </summary>
    [Test]
    public async Task Result_Fail_TapErrorAsync_should_pass_error_to_action()
    {
        string? received = null;

        await _fail.AsTask().TapErrorAsync(error =>
        {
            received = error;
            return Task.CompletedTask;
        });

        Assert.That(received, Is.EqualTo("error"));
    }

    /// <summary>
    /// 3. Ok.TapErrorAsync は onFailure を実行しない
    /// </summary>
    [Test]
    public async Task Result_Ok_TapErrorAsync_should_not_invoke_action()
    {
        int count = 0;

        var result = await _ok.AsTask().TapErrorAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 4. Fail.TapErrorAsync は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public async Task Result_Fail_TapErrorAsync_should_return_original_result()
    {
        var result = await _fail.AsTask().TapErrorAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(_fail));
    }

    /// <summary>
    /// 5. Ok.TapErrorAsync は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public async Task Result_Ok_TapErrorAsync_should_return_original_result()
    {
        var result = await _ok.AsTask().TapErrorAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(_ok));
    }

    /// <summary>
    /// 6. onFailure が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_TapErrorAsync_null_action_should_throw()
    {
        Func<string, Task>? onFailure = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _fail.AsTask().TapErrorAsync(onFailure!));
    }

    /// <summary>
    /// 7. resultTask が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_TapErrorAsync_null_result_task_should_throw()
    {
        Task<Result<string, int>>? resultTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask!.TapErrorAsync(_ => Task.CompletedTask));
    }

    /// <summary>
    /// 8. Fail.TapErrorAsync で onFailure が null Task を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Fail_TapErrorAsync_action_returning_null_task_should_throw()
    {
        Func<string, Task> onFailure = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _fail.AsTask().TapErrorAsync(onFailure));
    }

    /// <summary>
    /// 9. Ok.TapErrorAsync では null Task を返す onFailure でも実行されない
    /// </summary>
    [Test]
    public async Task Result_Ok_TapErrorAsync_should_not_evaluate_null_task_action()
    {
        Func<string, Task> onFailure = _ => null!;

        var result = await _ok.AsTask().TapErrorAsync(onFailure);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 10. 元の Task が未初期化 Result を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_TapErrorAsync_uninitialized_source_result_should_throw()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await resultTask.TapErrorAsync(_ => Task.CompletedTask));
    }
}