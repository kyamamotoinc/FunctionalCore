using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionTapAsyncTests
{
    /// <summary>
    /// 1. Some.TapAsync は onSome を1回だけ実行する
    /// </summary>
    [Test]
    public async Task Option_Some_TapAsync_should_invoke_action_once()
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
    /// 2. Some.TapAsync は Value を onSome に渡す
    /// </summary>
    [Test]
    public async Task Option_Some_TapAsync_should_pass_value_to_action()
    {
        var some = Option<int>.Some(5);
        int received = 0;

        await some.AsTask().TapAsync(value =>
        {
            received = value;
            return Task.CompletedTask;
        });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 3. None.TapAsync は onSome を実行しない
    /// </summary>
    [Test]
    public async Task Option_None_TapAsync_should_not_invoke_action()
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
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }

    /// <summary>
    /// 4. Some.TapAsync は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public async Task Option_Some_TapAsync_should_return_original_option()
    {
        var some = Option<int>.Some(5);
        var result = await some.AsTask().TapAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 5. None.TapAsync は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public async Task Option_None_TapAsync_should_return_original_option()
    {
        var none = Option<int>.None;
        var result = await none.AsTask().TapAsync(_ => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(none));
    }

    /// <summary>
    /// 6. onSome が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_TapAsync_null_action_should_throw()
    {
        var some = Option<int>.Some(5);
        Func<int, Task>? onSome = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await some.AsTask().TapAsync(onSome!));
    }

    /// <summary>
    /// 7. None でも onSome が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_TapAsync_null_action_should_throw()
    {
        var none = Option<int>.None;
        Func<int, Task>? onSome = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await none.AsTask().TapAsync(onSome!));
    }

    /// <summary>
    /// 8. optionTask が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_TapAsync_null_option_task_should_throw()
    {
        Task<Option<int>>? optionTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await optionTask!.TapAsync(_ => Task.CompletedTask));
    }

    /// <summary>
    /// 9. Some.TapAsync で onSome が null Task を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Option_Some_TapAsync_action_returning_null_task_should_throw()
    {
        var some = Option<int>.Some(5);
        Func<int, Task> onSome = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await some.AsTask().TapAsync(onSome));
    }

    /// <summary>
    /// 10. None.TapAsync では null Task を返す onSome でも実行されない
    /// </summary>
    [Test]
    public async Task Option_None_TapAsync_should_not_evaluate_null_task_action()
    {
        var none = Option<int>.None;
        Func<int, Task> onSome = _ => null!;

        var result = await none.AsTask().TapAsync(onSome);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 11. Default Option は None と同様に onSome を実行しない
    /// </summary>
    [Test]
    public async Task Option_Default_TapAsync_should_not_invoke_action()
    {
        var option = default(Option<int>);
        int count = 0;

        var result = await option.AsTask().TapAsync(_ =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}