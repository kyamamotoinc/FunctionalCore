using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionBindAsyncTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はbinderを実行し、そのOptionを返す。
    /// </summary>
    [Test]
    public async Task Some_BindAsync_should_return_binder_result()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().BindAsync(x => Task.FromResult(Option<int>.Some(x + 1)));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. OptionがSomeの場合はbinderによって値の型を変更できる。
    /// </summary>
    [Test]
    public async Task Some_BindAsync_should_change_value_type()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().BindAsync(x => Task.FromResult(Option<string>.Some($"value:{x}")));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. OptionがSomeでbinderがNoneを返した場合はNoneを返す。
    /// </summary>
    [Test]
    public async Task Some_BindAsync_should_return_none_when_binder_returns_none()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().BindAsync(_ => Task.FromResult(Option<int>.None));

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 4. OptionがSomeの場合はbinderを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task Some_BindAsync_should_invoke_binder_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        await some.AsTask().BindAsync(x =>
        {
            count++;
            return Task.FromResult(Option<int>.Some(x + 1));
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. OptionがNoneの場合はbinderを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public async Task None_BindAsync_should_return_none_without_invoking_binder()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = await none.AsTask().BindAsync(x =>
        {
            count++;
            return Task.FromResult(Option<int>.Some(x + 1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 6. OptionがSomeの場合でもbinderがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_BindAsync_should_throw_argument_null_exception_when_binder_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, Task<Option<string>>>? binder = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await some.AsTask().BindAsync(binder!));
    }

    /// <summary>
    /// 7. OptionがNoneの場合でもbinderがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_BindAsync_should_throw_argument_null_exception_when_binder_is_null()
    {
        var none = Option<int>.None;
        Func<int, Task<Option<string>>>? binder = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await none.AsTask().BindAsync(binder!));
    }

    /// <summary>
    /// 8. optionTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void BindAsync_should_throw_argument_null_exception_when_optionTask_is_null()
    {
        Task<Option<int>>? optionTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await optionTask!.BindAsync(x => Task.FromResult(Option<int>.Some(x + 1))));
    }

    /// <summary>
    /// 9. OptionがSomeでbinderがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_BindAsync_should_throw_invalid_operation_exception_when_binder_returns_null_task()
    {
        var some = Option<int>.Some(5);
        Func<int, Task<Option<int>>> binder = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await some.AsTask().BindAsync(binder));
    }

    /// <summary>
    /// 10. OptionがNoneの場合はnullのTaskを返すbinderでも実行せず、Noneを返す。
    /// </summary>
    [Test]
    public async Task None_BindAsync_should_return_none_without_invoking_null_task_binder()
    {
        var none = Option<int>.None;
        int count = 0;

        Func<int, Task<Option<int>>> binder = _ =>
        {
            count++;
            return null!;
        };

        var result = await none.AsTask().BindAsync(binder);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 11. OptionがSomeでbinderがdefault Optionを返した場合はNoneとして扱う。
    /// </summary>
    [Test]
    public async Task Some_BindAsync_should_return_none_when_binder_returns_default_option()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().BindAsync(_ => Task.FromResult(default(Option<string>)));

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 12. default OptionはNoneと同様にbinderを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public async Task Default_BindAsync_should_return_none_without_invoking_binder()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = await defaultOption.AsTask().BindAsync(x =>
        {
            count++;
            return Task.FromResult(Option<int>.Some(x + 1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 13. default Optionの場合でもbinderがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_BindAsync_should_throw_argument_null_exception_when_binder_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<int, Task<Option<string>>>? binder = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await defaultOption.AsTask().BindAsync(binder!));
    }

    /// <summary>
    /// 14. OptionがSomeでbinderが同期的に例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_BindAsync_should_propagate_exception_when_binder_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("binder error");
        Func<int, Task<Option<int>>> binder = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await some.AsTask().BindAsync(binder));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 15. OptionがSomeでbinderが返したTaskが例外で完了した場合は、
    /// その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_BindAsync_should_propagate_exception_when_binder_task_faults()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("binder task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await some.AsTask().BindAsync(_ => Task.FromException<Option<int>>(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 16. optionTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// binderは実行しない。
    /// </summary>
    [Test]
    public void BindAsync_should_propagate_exception_when_optionTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        Task<Option<int>> optionTask = Task.FromException<Option<int>>(expectedException);
        int count = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await optionTask.BindAsync(x =>
            {
                count++;
                return Task.FromResult(Option<int>.Some(x + 1));
            }));

        Assert.Multiple(() =>
        {
            Assert.That(actualException, Is.SameAs(expectedException));
            Assert.That(count, Is.EqualTo(0));
        });
    }
}