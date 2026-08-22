using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionEnsureAsyncTests
{
    /// <summary>
    /// 1. OptionがSomeでpredicateがtrueを返す場合は元のSomeを返す。
    /// </summary>
    [Test]
    public async Task Some_EnsureAsync_should_return_original_some_when_predicate_returns_true()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().EnsureAsync(x => Task.FromResult(x > 0));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(result, Is.EqualTo(some));
        });
    }

    /// <summary>
    /// 2. OptionがSomeでpredicateがfalseを返す場合はNoneを返す。
    /// </summary>
    [Test]
    public async Task Some_EnsureAsync_should_return_none_when_predicate_returns_false()
    {
        var some = Option<int>.Some(5);

        var result = await some.AsTask().EnsureAsync(x => Task.FromResult(x < 0));

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 3. OptionがSomeの場合はpredicateを1回だけ実行し、Valueを渡す。
    /// </summary>
    [Test]
    public async Task Some_EnsureAsync_should_invoke_predicate_once_and_pass_value()
    {
        var some = Option<int>.Some(5);
        int count = 0;
        int receivedValue = 0;

        await some.AsTask().EnsureAsync(value =>
        {
            count++;
            receivedValue = value;
            return Task.FromResult(true);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(receivedValue, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 4. OptionがNoneの場合はpredicateを実行せずNoneを返す。
    /// </summary>
    [Test]
    public async Task None_EnsureAsync_should_return_none_without_invoking_predicate()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = await none.AsTask().EnsureAsync(x =>
        {
            count++;
            return Task.FromResult(true);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }

    /// <summary>
    /// 5. optionTaskがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void EnsureAsync_should_throw_argument_null_exception_when_optionTask_is_null()
    {
        Task<Option<int>>? optionTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await optionTask!.EnsureAsync(x => Task.FromResult(x > 0)));
    }

    /// <summary>
    /// 6. predicateがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void EnsureAsync_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, Task<bool>>? predicate = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await some.AsTask().EnsureAsync(predicate!));
    }

    /// <summary>
    /// 7. OptionがSomeでpredicateがnullのTaskを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_EnsureAsync_should_throw_invalid_operation_exception_when_predicate_returns_null_task()
    {
        var some = Option<int>.Some(5);
        Func<int, Task<bool>> predicate = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await some.AsTask().EnsureAsync(predicate));
    }

    /// <summary>
    /// 8. predicateが同期的に例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_EnsureAsync_should_propagate_exception_when_predicate_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("predicate error");
        Func<int, Task<bool>> predicate = _ => throw expectedException;

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await some.AsTask().EnsureAsync(predicate));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 9. predicateが返したTaskが例外で完了した場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_EnsureAsync_should_propagate_exception_when_predicate_task_faults()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("predicate task error");

        var actualException = Assert.ThrowsAsync<NotSupportedException>(async () =>
            await some.AsTask().EnsureAsync(_ => Task.FromException<bool>(expectedException)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 10. default OptionはNoneと同様にpredicateを実行せずNoneを返す。
    /// </summary>
    [Test]
    public async Task Default_EnsureAsync_should_behave_as_none()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = await defaultOption.AsTask().EnsureAsync(x =>
        {
            count++;
            return Task.FromResult(true);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}