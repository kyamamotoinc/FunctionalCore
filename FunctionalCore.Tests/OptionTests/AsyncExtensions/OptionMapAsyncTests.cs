using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionMapAsyncTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はselectorを実行し、変換後の値を保持するSomeを返す。
    /// </summary>
    [Test]
    public async Task Some_MapAsync_should_return_mapped_option()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().MapAsync(x => Task.FromResult(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. OptionがSomeの場合はselectorによって値の型を変更できる。
    /// </summary>
    [Test]
    public async Task Some_MapAsync_should_change_value_type()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().MapAsync(x => Task.FromResult($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. OptionがSomeの場合はselectorを1回だけ実行する。
    /// </summary>
    [Test]
    public async Task Some_MapAsync_should_invoke_selector_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        await some.AsTask().MapAsync(x =>
        {
            count++;
            return Task.FromResult(x + 1);
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. OptionがNoneの場合はselectorを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public async Task None_MapAsync_should_return_none_without_invoking_selector()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = await none.AsTask().MapAsync(x =>
        {
            count++;
            return Task.FromResult(x + 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 5. OptionがSomeの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_MapAsync_should_throw_argument_null_exception_when_selector_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, Task<string>>? selector = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await some.AsTask().MapAsync(selector!));
    }

    /// <summary>
    /// 6. OptionがNoneの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_MapAsync_should_throw_argument_null_exception_when_selector_is_null()
    {
        var none = Option<int>.None;
        Func<int, Task<string>>? selector = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await none.AsTask().MapAsync(selector!));
    }

    /// <summary>
    /// 7. optionTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void MapAsync_should_throw_argument_null_exception_when_optionTask_is_null()
    {
        Task<Option<int>>? optionTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await optionTask!.MapAsync(x => Task.FromResult(x + 1)));
    }

    /// <summary>
    /// 8. OptionがSomeでselectorがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_MapAsync_should_throw_invalid_operation_exception_when_selector_returns_null_task()
    {
        var some = Option<int>.Some(5);
        Func<int, Task<string>> selector = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await some.AsTask().MapAsync(selector));
    }

    /// <summary>
    /// 9. OptionがSomeでselectorのTaskがnullを返した場合はNoneを返す。
    /// </summary>
    [Test]
    public async Task Some_MapAsync_should_return_none_when_selector_task_returns_null()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().MapAsync(_ => Task.FromResult((string)null!));

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 10. OptionがNoneの場合はnullのTaskを返すselectorでも実行せず、Noneを返す。
    /// </summary>
    [Test]
    public async Task None_MapAsync_should_return_none_without_invoking_null_task_selector()
    {
        var none = Option<int>.None;
        int count = 0;

        Func<int, Task<string>> selector = _ =>
        {
            count++;
            return null!;
        };

        var result = await none.AsTask().MapAsync(selector);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<string>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 11. OptionがNoneの場合はnullを返すTaskを生成するselectorでも実行せず、Noneを返す。
    /// </summary>
    [Test]
    public async Task None_MapAsync_should_return_none_without_invoking_null_value_selector()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = await none.AsTask().MapAsync(_ =>
        {
            count++;
            return Task.FromResult((string)null!);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<string>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 12. default OptionはNoneと同様にselectorを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public async Task Default_MapAsync_should_return_none_without_invoking_selector()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = await defaultOption.AsTask().MapAsync(x =>
        {
            count++;
            return Task.FromResult(x + 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 13. default Optionの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_MapAsync_should_throw_argument_null_exception_when_selector_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<int, Task<string>>? selector = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await defaultOption.AsTask().MapAsync(selector!));
    }

    /// <summary>
    /// 14. OptionがSomeでselectorが同期的に例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_MapAsync_should_propagate_exception_when_selector_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("selector error");
        Func<int, Task<string>> selector = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await some.AsTask().MapAsync(selector));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 15. OptionがSomeでselectorが返したTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_MapAsync_should_propagate_exception_when_selector_task_faults()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("selector task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await some.AsTask().MapAsync(_ => Task.FromException<string>(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 16. optionTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// selectorは実行しない。
    /// </summary>
    [Test]
    public void MapAsync_should_propagate_exception_when_optionTask_faults()
    {
        var expectedException = new NotSupportedException("source task error");
        Task<Option<int>> optionTask = Task.FromException<Option<int>>(expectedException);
        int count = 0;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await optionTask.MapAsync(x =>
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