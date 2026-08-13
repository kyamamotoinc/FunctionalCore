using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionTapNoneAsyncTests
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
    /// 1. None.TapNoneAsync は onNone を1回だけ実行する
    /// </summary>
    [Test]
    public async Task Option_None_TapNoneAsync_should_invoke_action_once()
    {
        int count = 0;

        await _none.AsTask().TapNoneAsync(() =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Some.TapNoneAsync は onNone を実行しない
    /// </summary>
    [Test]
    public async Task Option_Some_TapNoneAsync_should_not_invoke_action()
    {
        int count = 0;

        var result = await _some.AsTask().TapNoneAsync(() =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(_some));
        });
    }

    /// <summary>
    /// 3. None.TapNoneAsync は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public async Task Option_None_TapNoneAsync_should_return_original_option()
    {
        var result = await _none.AsTask().TapNoneAsync(() => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(_none));
    }

    /// <summary>
    /// 4. Some.TapNoneAsync は元の Option を変更せずに返す
    /// </summary>
    [Test]
    public async Task Option_Some_TapNoneAsync_should_return_original_option()
    {
        var result = await _some.AsTask().TapNoneAsync(() => Task.CompletedTask);

        Assert.That(result, Is.EqualTo(_some));
    }

    /// <summary>
    /// 5. onNone が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_TapNoneAsync_null_action_should_throw()
    {
        Func<Task>? onNone = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _none.AsTask().TapNoneAsync(onNone!));
    }

    /// <summary>
    /// 6. Some でも onNone が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Some_TapNoneAsync_null_action_should_throw()
    {
        Func<Task>? onNone = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _some.AsTask().TapNoneAsync(onNone!));
    }

    /// <summary>
    /// 7. optionTask が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_TapNoneAsync_null_option_task_should_throw()
    {
        Task<Option<int>>? optionTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await optionTask!.TapNoneAsync(() => Task.CompletedTask));
    }

    /// <summary>
    /// 8. None.TapNoneAsync で onNone が null Task を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Option_None_TapNoneAsync_action_returning_null_task_should_throw()
    {
        Func<Task> onNone = () => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _none.AsTask().TapNoneAsync(onNone));
    }

    /// <summary>
    /// 9. Some.TapNoneAsync では null Task を返す onNone でも実行されない
    /// </summary>
    [Test]
    public async Task Option_Some_TapNoneAsync_should_not_evaluate_null_task_action()
    {
        Func<Task> onNone = () => null!;

        var result = await _some.AsTask().TapNoneAsync(onNone);

        Assert.That(result, Is.EqualTo(_some));
    }

    /// <summary>
    /// 10. Default Option は None と同様に onNone を実行する
    /// </summary>
    [Test]
    public async Task Option_Default_TapNoneAsync_should_invoke_action()
    {
        var option = default(Option<int>);
        int count = 0;

        var result = await option.AsTask().TapNoneAsync(() =>
        {
            count++;
            return Task.CompletedTask;
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}