using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultMapAsyncTests
{
    /// <summary>
    /// 1. ResultがOkの場合はselectorを実行し、変換後の値を保持するOkを返す。
    /// </summary>
    [Test]
    public async Task Ok_MapAsync_should_return_mapped_result()
    {
        var ok = Result<string, int>.Ok(5);

        var result = await ok.AsTask().MapAsync(x => Task.FromResult(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. ResultがOkの場合はselectorによって成功値の型を変更できる。
    /// </summary>
    [Test]
    public async Task Ok_MapAsync_should_change_value_type()
    {
        var ok = Result<string, int>.Ok(5);

        var result = await ok.AsTask().MapAsync(x => Task.FromResult($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. ResultがOkの場合はselectorを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task Ok_MapAsync_should_invoke_selector_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        await ok.AsTask().MapAsync(x =>
        {
            count++;
            return Task.FromResult(x + 1);
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. ResultがFailの場合はselectorを実行しない。
    /// </summary>
    [Test]
    public async Task Fail_MapAsync_should_not_invoke_selector()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = await fail.AsTask().MapAsync(x =>
        {
            count++;
            return Task.FromResult(x + 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 5. ResultがFailの場合は元のErrorを保持する。
    /// </summary>
    [Test]
    public async Task Fail_MapAsync_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");

        var result = await fail.AsTask().MapAsync(x => Task.FromResult(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 6. ResultがOkの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_MapAsync_should_throw_argument_null_exception_when_selector_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Task<string>>? selector = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await ok.AsTask().MapAsync(selector!));
    }

    /// <summary>
    /// 7. ResultがFailの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_MapAsync_should_throw_argument_null_exception_when_selector_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Func<int, Task<string>>? selector = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await fail.AsTask().MapAsync(selector!));
    }

    /// <summary>
    /// 8. resultTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void MapAsync_should_throw_argument_null_exception_when_resultTask_is_null()
    {
        Task<Result<string, int>>? resultTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await resultTask!.MapAsync(x => Task.FromResult(x + 1)));
    }

    /// <summary>
    /// 9. ResultがOkでselectorがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_MapAsync_should_throw_invalid_operation_exception_when_selector_returns_null_task()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Task<string>> selector = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ok.AsTask().MapAsync(selector));
    }

    /// <summary>
    /// 10. ResultがOkでselectorのTaskがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_MapAsync_should_throw_invalid_operation_exception_when_selector_task_returns_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await ok.AsTask().MapAsync(_ => Task.FromResult((string)null!)));
    }

    /// <summary>
    /// 11. resultTaskが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// selectorは実行しない。
    /// </summary>
    [Test]
    public void MapAsync_should_throw_invalid_operation_exception_when_resultTask_returns_uninitialized_result()
    {
        var resultTask = Task.FromResult(default(Result<string, int>));
        int count = 0;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await resultTask.MapAsync(x =>
            {
                count++;
                return Task.FromResult(x + 1);
            }));

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 12. ResultがFailの場合はnullのTaskを返すselectorでも実行せず、元のFailを返す。
    /// </summary>
    [Test]
    public async Task Fail_MapAsync_should_return_original_fail_without_invoking_null_task_selector()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        Func<int, Task<string>> selector = _ =>
        {
            count++;
            return null!;
        };

        var result = await fail.AsTask().MapAsync(selector);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 13. ResultがFailの場合はnullを返すTaskを生成するselectorでも実行せず、元のFailを返す。
    /// </summary>
    [Test]
    public async Task Fail_MapAsync_should_return_original_fail_without_invoking_null_value_selector()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = await fail.AsTask().MapAsync(_ =>
        {
            count++;
            return Task.FromResult((string)null!);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 14. ResultがOkでselectorが同期的に例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_MapAsync_should_propagate_exception_when_selector_throws()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("selector error");

        Func<int, Task<string>> selector = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await ok.AsTask().MapAsync(selector));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 15. ResultがOkでselectorが返したTaskが例外で完了した場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_MapAsync_should_propagate_exception_when_selector_task_faults()
    {
        var ok = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("selector task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await ok.AsTask().MapAsync(_ => Task.FromException<string>(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 16. resultTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// selectorは実行しない。
    /// </summary>
    [Test]
    public void MapAsync_should_propagate_exception_when_resultTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        Task<Result<string, int>> resultTask = Task.FromException<Result<string, int>>(expectedException);
        int count = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await resultTask.MapAsync(x =>
            {
                count++;
                return Task.FromResult(x + 1);
            }));

        Assert.Multiple(() =>
        {
            Assert.That(actualException, Is.SameAs(expectedException));
            Assert.That(count, Is.EqualTo(0));
        });
    }
}