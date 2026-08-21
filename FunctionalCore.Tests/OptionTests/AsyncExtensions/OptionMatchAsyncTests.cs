using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionMatchAsyncTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はonSomeを実行し、その戻り値を返す。
    /// </summary>
    [Test]
    public async Task Some_MatchAsync_should_return_onSome_result()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().MatchAsync(
            value => Task.FromResult(value + 1),
            () => Task.FromResult(-1));

        Assert.That(result, Is.EqualTo(6));
    }

    /// <summary>
    /// 2. OptionがSomeの場合はonSomeを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task Some_MatchAsync_should_invoke_onSome_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        await some.AsTask().MatchAsync(
            value =>
            {
                count++;
                return Task.FromResult(value + 1);
            },
            () => Task.FromResult(-1));

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 3. OptionがSomeの場合はonNoneを実行しない。
    /// </summary>
    [Test]
    public async Task Some_MatchAsync_should_not_invoke_onNone()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        await some.AsTask().MatchAsync(
            value => Task.FromResult(value + 1),
            () =>
            {
                count++;
                return Task.FromResult(-1);
            });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. OptionがNoneの場合はonNoneを実行し、その戻り値を返す。
    /// </summary>
    [Test]
    public async Task None_MatchAsync_should_return_onNone_result()
    {
        var none = Option<int>.None;

        var result = await none.AsTask().MatchAsync(
            value => Task.FromResult(value + 1),
            () => Task.FromResult(-1));

        Assert.That(result, Is.EqualTo(-1));
    }

    /// <summary>
    /// 5. OptionがNoneの場合はonNoneを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task None_MatchAsync_should_invoke_onNone_once()
    {
        var none = Option<int>.None;
        int count = 0;

        await none.AsTask().MatchAsync(
            value => Task.FromResult(value + 1),
            () =>
            {
                count++;
                return Task.FromResult(-1);
            });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 6. OptionがNoneの場合はonSomeを実行しない。
    /// </summary>
    [Test]
    public async Task None_MatchAsync_should_not_invoke_onSome()
    {
        var none = Option<int>.None;
        int count = 0;

        await none.AsTask().MatchAsync(
            value =>
            {
                count++;
                return Task.FromResult(value + 1);
            },
            () => Task.FromResult(-1));

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 7. OptionがSomeの場合でもonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_MatchAsync_should_throw_argument_null_exception_when_onSome_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, Task<int>>? onSome = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await some.AsTask().MatchAsync(onSome!, () => Task.FromResult(-1)));
    }

    /// <summary>
    /// 8. OptionがNoneの場合でもonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_MatchAsync_should_throw_argument_null_exception_when_onNone_is_null()
    {
        var none = Option<int>.None;
        Func<Task<int>>? onNone = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await none.AsTask().MatchAsync(value => Task.FromResult(value + 1), onNone!));
    }

    /// <summary>
    /// 9. OptionがSomeの場合でも未使用のonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_MatchAsync_should_throw_argument_null_exception_when_unused_onNone_is_null()
    {
        var some = Option<int>.Some(5);
        Func<Task<int>>? onNone = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await some.AsTask().MatchAsync(value => Task.FromResult(value + 1), onNone!));
    }

    /// <summary>
    /// 10. OptionがNoneの場合でも未使用のonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_MatchAsync_should_throw_argument_null_exception_when_unused_onSome_is_null()
    {
        var none = Option<int>.None;
        Func<int, Task<int>>? onSome = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await none.AsTask().MatchAsync(onSome!, () => Task.FromResult(-1)));
    }

    /// <summary>
    /// 11. optionTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void MatchAsync_should_throw_argument_null_exception_when_optionTask_is_null()
    {
        Task<Option<int>>? optionTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await optionTask!.MatchAsync(
                value => Task.FromResult(value + 1),
                () => Task.FromResult(-1)));
    }

    /// <summary>
    /// 12. OptionがSomeでonSomeがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_MatchAsync_should_throw_invalid_operation_exception_when_onSome_returns_null_task()
    {
        var some = Option<int>.Some(5);
        Func<int, Task<int>> onSome = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await some.AsTask().MatchAsync(onSome, () => Task.FromResult(-1)));
    }

    /// <summary>
    /// 13. OptionがNoneでonNoneがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_MatchAsync_should_throw_invalid_operation_exception_when_onNone_returns_null_task()
    {
        var none = Option<int>.None;
        Func<Task<int>> onNone = () => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await none.AsTask().MatchAsync(value => Task.FromResult(value + 1), onNone));
    }

    /// <summary>
    /// 14. 選択された関数のTaskがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void MatchAsync_should_throw_invalid_operation_exception_when_selected_task_returns_null()
    {
        var some = Option<int>.Some(5);

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await some.AsTask().MatchAsync(
                _ => Task.FromResult((string)null!),
                () => Task.FromResult("none")));
    }

    /// <summary>
    /// 15. default OptionはNoneと同様にonNoneを実行し、その戻り値を返す。
    /// </summary>
    [Test]
    public async Task Default_MatchAsync_should_behave_as_none()
    {
        var defaultOption = default(Option<int>);

        var result = await defaultOption.AsTask().MatchAsync(
            value => Task.FromResult(value + 1),
            () => Task.FromResult(-1));

        Assert.That(result, Is.EqualTo(-1));
    }

    /// <summary>
    /// 16. 選択された関数が同期的に例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void MatchAsync_should_propagate_exception_when_selected_function_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("match error");
        Func<int, Task<int>> onSome = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await some.AsTask().MatchAsync(onSome, () => Task.FromResult(-1)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 17. 選択された関数が返したTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void MatchAsync_should_propagate_exception_when_selected_task_faults()
    {
        var none = Option<int>.None;
        var expectedException = new NotSupportedException("match task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await none.AsTask().MatchAsync(
                value => Task.FromResult(value + 1),
                () => Task.FromException<int>(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 18. optionTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// onSomeとonNoneは実行しない。
    /// </summary>
    [Test]
    public void MatchAsync_should_propagate_exception_when_optionTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        Task<Option<int>> optionTask = Task.FromException<Option<int>>(expectedException);
        int someCount = 0;
        int noneCount = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await optionTask.MatchAsync(
                value =>
                {
                    someCount++;
                    return Task.FromResult(value + 1);
                },
                () =>
                {
                    noneCount++;
                    return Task.FromResult(-1);
                }));

        Assert.Multiple(() =>
        {
            Assert.That(actualException, Is.SameAs(expectedException));
            Assert.That(someCount, Is.EqualTo(0));
            Assert.That(noneCount, Is.EqualTo(0));
        });
    }
}