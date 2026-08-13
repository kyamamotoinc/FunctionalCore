using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionBindAsyncTests
{
    private Option<int> _some;
    private Option<int> _none;

    [SetUp]
    public void Setup()
    {
        _some = Option<int>.Some(5);
        _none = Option<int>.None;
    }

    /// <summary>
    /// 1. Some.BindAsync は binder を実行し、その Option を返す
    /// </summary>
    [Test]
    public async Task Option_Some_BindAsync_should_return_binder_result()
    {
        var result = await _some.AsTask().BindAsync(x => Task.FromResult(Option<int>.Some(x + 1)));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. Some.BindAsync は値の型を変更できる
    /// </summary>
    [Test]
    public async Task Option_Some_BindAsync_should_change_value_type()
    {
        var result = await _some.AsTask().BindAsync(x => Task.FromResult(Option<string>.Some($"value:{x}")));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. binder が None を返した場合は None を返す
    /// </summary>
    [Test]
    public async Task Option_Some_BindAsync_binder_returning_none_should_return_none()
    {
        var result = await _some.AsTask().BindAsync(_ => Task.FromResult(Option<int>.None));

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 4. Some.BindAsync は binder を1回だけ実行する
    /// </summary>
    [Test]
    public async Task Option_Some_BindAsync_should_invoke_binder_once()
    {
        int count = 0;

        await _some.AsTask().BindAsync(x =>
        {
            count++;
            return Task.FromResult(Option<int>.Some(x + 1));
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. None.BindAsync は binder を実行しない
    /// </summary>
    [Test]
    public async Task Option_None_BindAsync_should_not_invoke_binder()
    {
        int count = 0;

        var result = await _none.AsTask().BindAsync(x =>
        {
            count++;
            return Task.FromResult(Option<int>.Some(x + 1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }

    /// <summary>
    /// 6. binder が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_BindAsync_null_binder_should_throw()
    {
        Func<int, Task<Option<string>>>? binder = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _some.AsTask().BindAsync(binder!));
    }

    /// <summary>
    /// 7. None でも binder が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_BindAsync_null_binder_should_throw()
    {
        Func<int, Task<Option<string>>>? binder = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _none.AsTask().BindAsync(binder!));
    }

    /// <summary>
    /// 8. optionTask が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_BindAsync_null_option_task_should_throw()
    {
        Task<Option<int>>? optionTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await optionTask!.BindAsync(x => Task.FromResult(Option<int>.Some(x + 1))));
    }

    /// <summary>
    /// 9. binder が null Task を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Option_Some_BindAsync_binder_returning_null_task_should_throw()
    {
        Func<int, Task<Option<int>>> binder = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _some.AsTask().BindAsync(binder));
    }

    /// <summary>
    /// 10. None.BindAsync では null Task を返す binder でも実行されない
    /// </summary>
    [Test]
    public async Task Option_None_BindAsync_should_not_evaluate_null_task_binder()
    {
        Func<int, Task<Option<int>>> binder = _ => null!;

        var result = await _none.AsTask().BindAsync(binder);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 11. binder が Default Option を返した場合は None として扱われる
    /// </summary>
    [Test]
    public async Task Option_Some_BindAsync_binder_returning_default_option_should_return_none()
    {
        var result = await _some.AsTask().BindAsync(_ => Task.FromResult(default(Option<string>)));

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 12. Default Option は None と同様に binder を実行しない
    /// </summary>
    [Test]
    public async Task Option_Default_BindAsync_should_not_invoke_binder()
    {
        var option = default(Option<int>);
        int count = 0;

        var result = await option.AsTask().BindAsync(x =>
        {
            count++;
            return Task.FromResult(Option<int>.Some(x + 1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}