using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultMapAsyncTests
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
    /// 1. Ok.MapAsync は selector を実行し、変換後の値を持つ Ok を返す
    /// </summary>
    [Test]
    public async Task Result_Ok_MapAsync_should_return_selector_result()
    {
        var result = await _ok.AsTask().MapAsync(x => Task.FromResult(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. Ok.MapAsync は成功値の型を変更できる
    /// </summary>
    [Test]
    public async Task Result_Ok_MapAsync_should_change_value_type()
    {
        var result = await _ok.AsTask().MapAsync(x => Task.FromResult($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. Ok.MapAsync は selector を1回だけ実行する
    /// </summary>
    [Test]
    public async Task Result_Ok_MapAsync_should_invoke_selector_once()
    {
        int count = 0;

        await _ok.AsTask().MapAsync(x =>
        {
            count++;
            return Task.FromResult(x + 1);
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. Fail.MapAsync は selector を実行しない
    /// </summary>
    [Test]
    public async Task Result_Fail_MapAsync_should_not_invoke_selector()
    {
        int count = 0;

        var result = await _fail.AsTask().MapAsync(x =>
        {
            count++;
            return Task.FromResult(x + 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 5. Fail.MapAsync は元の Error を保持する
    /// </summary>
    [Test]
    public async Task Result_Fail_MapAsync_should_keep_original_error()
    {
        var result = await _fail.AsTask().MapAsync(x => Task.FromResult(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 6. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_MapAsync_null_selector_should_throw()
    {
        Func<int, Task<string>>? selector = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _ok.AsTask().MapAsync(selector!));
    }

    /// <summary>
    /// 7. resultTask が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_MapAsync_null_result_task_should_throw()
    {
        Task<Result<string, int>>? resultTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask!.MapAsync(x => Task.FromResult(x + 1)));
    }

    /// <summary>
    /// 8. selector が null Task を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_MapAsync_selector_returning_null_task_should_throw()
    {
        Func<int, Task<string>> selector = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _ok.AsTask().MapAsync(selector));
    }

    /// <summary>
    /// 9. selector の Task が null の値を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_MapAsync_selector_returning_null_value_should_throw()
    {
        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _ok.AsTask().MapAsync(_ => Task.FromResult((string)null!)));
    }

    /// <summary>
    /// 10. 元の Task が未初期化 Result を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_MapAsync_uninitialized_source_result_should_throw()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await resultTask.MapAsync(x => Task.FromResult(x + 1)));
    }

    /// <summary>
    /// 11. Fail.MapAsync では null Task を返す selector でも実行されない
    /// </summary>
    [Test]
    public async Task Result_Fail_MapAsync_should_not_evaluate_null_task_selector()
    {
        Func<int, Task<string>> selector = _ => null!;

        var result = await _fail.AsTask().MapAsync(selector);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 12. Fail.MapAsync では null の値を返す selector でも実行されない
    /// </summary>
    [Test]
    public async Task Result_Fail_MapAsync_should_not_evaluate_null_value_selector()
    {
        var result = await _fail.AsTask().MapAsync(_ => Task.FromResult((string)null!));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }
}