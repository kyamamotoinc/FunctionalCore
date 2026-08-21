using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionTapNoneAsyncTests
{
    /// <summary>
    /// 1. OptionがNoneの場合はonNoneを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task None_TapNoneAsync_should_invoke_onNone_once()
    {
        var none = Option<int>.None;
        int count = 0;

        await none.AsTask().TapNoneAsync(() =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. OptionがSomeの場合はonNoneを実行せず、元のSomeを返す。
    /// </summary>
    [Test]
    public async Task Some_TapNoneAsync_should_return_original_some_without_invoking_onNone()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        var result = await some.AsTask().TapNoneAsync(() =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(some));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 3. OptionがNoneの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public async Task None_TapNoneAsync_should_return_original_option()
    {
        var none = Option<int>.None;

        var result = await none.AsTask().TapNoneAsync(() => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(none));
    }

    /// <summary>
    /// 4. OptionがSomeの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public async Task Some_TapNoneAsync_should_return_original_option()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().TapNoneAsync(() => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 5. OptionがNoneの場合でもonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_TapNoneAsync_should_throw_argument_null_exception_when_onNone_is_null()
    {
        var none = Option<int>.None;
        Func<Task>? onNone = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await none.AsTask().TapNoneAsync(onNone!));
    }

    /// <summary>
    /// 6. OptionがSomeの場合でもonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_TapNoneAsync_should_throw_argument_null_exception_when_onNone_is_null()
    {
        var some = Option<int>.Some(5);
        Func<Task>? onNone = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await some.AsTask().TapNoneAsync(onNone!));
    }

    /// <summary>
    /// 7. optionTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void TapNoneAsync_should_throw_argument_null_exception_when_optionTask_is_null()
    {
        Task<Option<int>>? optionTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await optionTask!.TapNoneAsync(() => Task.CompletedTask));
    }

    /// <summary>
    /// 8. OptionがNoneでonNoneがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_TapNoneAsync_should_throw_invalid_operation_exception_when_onNone_returns_null_task()
    {
        var none = Option<int>.None;
        Func<Task> onNone = () => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await none.AsTask().TapNoneAsync(onNone));
    }

    /// <summary>
    /// 9. OptionがSomeの場合はnullのTaskを返すonNoneでも実行せず、元のSomeを返す。
    /// </summary>
    [Test]
    public async Task Some_TapNoneAsync_should_return_original_some_without_invoking_null_task_onNone()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        Func<Task> onNone = () =>
        {
            count++;
            return null!;
        };

        var result = await some.AsTask().TapNoneAsync(onNone);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(some));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 10. default OptionはNoneと同様にonNoneを1回実行し、Noneを返す。
    /// </summary>
    [Test]
    public async Task Default_TapNoneAsync_should_invoke_onNone_once_and_return_none()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = await defaultOption.AsTask().TapNoneAsync(() =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 11. default Optionの場合でもonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_TapNoneAsync_should_throw_argument_null_exception_when_onNone_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<Task>? onNone = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await defaultOption.AsTask().TapNoneAsync(onNone!));
    }

    /// <summary>
    /// 12. OptionがNoneでonNoneが同期的に例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void None_TapNoneAsync_should_propagate_exception_when_onNone_throws()
    {
        var none = Option<int>.None;
        var expectedException = new NotSupportedException("onNone error");
        Func<Task> onNone = () => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await none.AsTask().TapNoneAsync(onNone));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 13. OptionがNoneでonNoneが返したTaskが例外で完了した場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void None_TapNoneAsync_should_propagate_exception_when_onNone_task_faults()
    {
        var none = Option<int>.None;
        var expectedException = new NotSupportedException("onNone task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await none.AsTask().TapNoneAsync(() => Task.FromException(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 14. optionTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// onNoneは実行しない。
    /// </summary>
    [Test]
    public void TapNoneAsync_should_propagate_exception_when_optionTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        Task<Option<int>> optionTask = Task.FromException<Option<int>>(expectedException);
        int count = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await optionTask.TapNoneAsync(() =>
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