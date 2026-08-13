using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultTapAsyncTests
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
    /// 1. Ok.TapAsync は onSuccess を1回だけ実行する
    /// </summary>
    [Test]
    public async Task Result_Ok_TapAsync_should_invoke_action_once()
    {
        int count = 0;

        await _ok.AsTask().TapAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Ok.TapAsync は成功値を onSuccess に渡す
    /// </summary>
    [Test]
    public async Task Result_Ok_TapAsync_should_pass_value_to_action()
    {
        int received = 0;

        await _ok.AsTask().TapAsync(value =>
        {
            received = value;
            return Task.CompletedTask;
        });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. Fail.TapAsync は onSuccess を実行しない
    /// </summary>
    [Test]
    public async Task Result_Fail_TapAsync_should_not_invoke_action()
    {
        int count = 0;

        var result = await _fail.AsTask().TapAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 4. Ok.TapAsync は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public async Task Result_Ok_TapAsync_should_return_original_result()
    {
        var result = await _ok.AsTask().TapAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(_ok));
    }

    /// <summary>
    /// 5. Fail.TapAsync は元の Result を変更せずに返す
    /// </summary>
    [Test]
    public async Task Result_Fail_TapAsync_should_return_original_result()
    {
        var result = await _fail.AsTask().TapAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(_fail));
    }

    /// <summary>
    /// 6. onSuccess が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_TapAsync_null_action_should_throw()
    {
        Func<int, Task>? onSuccess = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _ok.AsTask().TapAsync(onSuccess!));
    }

    /// <summary>
    /// 7. resultTask が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_TapAsync_null_result_task_should_throw()
    {
        Task<Result<string, int>>? resultTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask!.TapAsync(_ => Task.CompletedTask));
    }

    /// <summary>
    /// 8. Ok.TapAsync で onSuccess が null Task を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_TapAsync_action_returning_null_task_should_throw()
    {
        Func<int, Task> onSuccess = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _ok.AsTask().TapAsync(onSuccess));
    }

    /// <summary>
    /// 9. Fail.TapAsync では null Task を返す onSuccess でも実行されない
    /// </summary>
    [Test]
    public async Task Result_Fail_TapAsync_should_not_evaluate_null_task_action()
    {
        Func<int, Task> onSuccess = _ => null!;

        var result = await _fail.AsTask().TapAsync(onSuccess);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 10. 元の Task が未初期化 Result を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_TapAsync_uninitialized_source_result_should_throw()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await resultTask.TapAsync(_ => Task.CompletedTask));
    }
}