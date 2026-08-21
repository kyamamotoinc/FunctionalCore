using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionAsTaskTests
{
    /// <summary>
    /// 1. Some.AsTask は元の Some を保持する完了済み Task を返す
    /// </summary>
    [Test]
    public async Task Option_Some_AsTask_should_return_original_option()
    {
        var some = Option<int>.Some(5);
        var result = await some.AsTask();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(some));
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 2. None.AsTask は元の None を保持する完了済み Task を返す
    /// </summary>
    [Test]
    public async Task Option_None_AsTask_should_return_original_option()
    {
        var none = Option<int>.None;
        var result = await none.AsTask();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(none));
            Assert.That(result.HasValue, Is.False);
        });
    }

    /// <summary>
    /// 3. Some.AsTask は完了済み Task を返す
    /// </summary>
    [Test]
    public void Option_Some_AsTask_should_return_completed_task()
    {
        var some = Option<int>.Some(5);
        var task = some.AsTask();

        Assert.That(task.IsCompletedSuccessfully, Is.True);
    }

    /// <summary>
    /// 4. None.AsTask は完了済み Task を返す
    /// </summary>
    [Test]
    public void Option_None_AsTask_should_return_completed_task()
    {
        var none = Option<int>.None;
        var task = none.AsTask();

        Assert.That(task.IsCompletedSuccessfully, Is.True);
    }

    /// <summary>
    /// 5. Default Option.AsTask は None を保持する完了済み Task を返す
    /// </summary>
    [Test]
    public async Task Option_Default_AsTask_should_return_none()
    {
        var option = default(Option<int>);

        var result = await option.AsTask();

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }
}