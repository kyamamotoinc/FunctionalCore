using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionTapAsyncTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はonSomeを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task Some_TapAsync_should_invoke_onSome_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        await some.AsTask().TapAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. OptionがSomeの場合はValueをonSomeに渡す。
    /// </summary>
    [Test]
    public async Task Some_TapAsync_should_pass_value_to_onSome()
    {
        var some = Option<int>.Some(5);
        int receivedValue = 0;

        await some.AsTask().TapAsync(value =>
        {
            receivedValue = value;
            return Task.CompletedTask;
        });

        Assert.That(receivedValue, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. OptionがNoneの場合はonSomeを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public async Task None_TapAsync_should_return_none_without_invoking_onSome()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = await none.AsTask().TapAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 4. OptionがSomeの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public async Task Some_TapAsync_should_return_original_option()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().TapAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 5. OptionがNoneの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public async Task None_TapAsync_should_return_original_option()
    {
        var none = Option<int>.None;

        var result = await none.AsTask().TapAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(none));
    }

    /// <summary>
    /// 6. OptionがSomeの場合でもonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_TapAsync_should_throw_argument_null_exception_when_onSome_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, Task>? onSome = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await some.AsTask().TapAsync(onSome!));
    }

    /// <summary>
    /// 7. OptionがNoneの場合でもonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_TapAsync_should_throw_argument_null_exception_when_onSome_is_null()
    {
        var none = Option<int>.None;
        Func<int, Task>? onSome = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await none.AsTask().TapAsync(onSome!));
    }

    /// <summary>
    /// 8. optionTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void TapAsync_should_throw_argument_null_exception_when_optionTask_is_null()
    {
        Task<Option<int>>? optionTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await optionTask!.TapAsync(_ => Task.CompletedTask));
    }

    /// <summary>
    /// 9. OptionがSomeでonSomeがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_TapAsync_should_throw_invalid_operation_exception_when_onSome_returns_null_task()
    {
        var some = Option<int>.Some(5);
        Func<int, Task> onSome = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await some.AsTask().TapAsync(onSome));
    }

    /// <summary>
    /// 10. OptionがNoneの場合はnullのTaskを返すonSomeでも実行せず、Noneを返す。
    /// </summary>
    [Test]
    public async Task None_TapAsync_should_return_none_without_invoking_null_task_onSome()
    {
        var none = Option<int>.None;
        int count = 0;

        Func<int, Task> onSome = _ =>
        {
            count++;
            return null!;
        };

        var result = await none.AsTask().TapAsync(onSome);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 11. default OptionはNoneと同様にonSomeを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public async Task Default_TapAsync_should_return_none_without_invoking_onSome()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = await defaultOption.AsTask().TapAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 12. default Optionの場合でもonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_TapAsync_should_throw_argument_null_exception_when_onSome_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<int, Task>? onSome = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await defaultOption.AsTask().TapAsync(onSome!));
    }

    /// <summary>
    /// 13. OptionがSomeでonSomeが同期的に例外を発生させた場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_TapAsync_should_propagate_exception_when_onSome_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("onSome error");
        Func<int, Task> onSome = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await some.AsTask().TapAsync(onSome));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 14. OptionがSomeでonSomeが返したTaskが例外で完了した場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_TapAsync_should_propagate_exception_when_onSome_task_faults()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("onSome task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await some.AsTask().TapAsync(_ => Task.FromException(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 15. optionTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// onSomeは実行しない。
    /// </summary>
    [Test]
    public void TapAsync_should_propagate_exception_when_optionTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        Task<Option<int>> optionTask = Task.FromException<Option<int>>(expectedException);
        int count = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await optionTask.TapAsync(_ =>
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