using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionAsTaskTests
{
    /// <summary>
    /// 1. OptionがSomeの場合は元のSomeを保持するTaskを返す。
    /// </summary>
    [Test]
    public async Task Some_AsTask_should_return_original_option()
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
    /// 2. OptionがNoneの場合は元のNoneを保持するTaskを返す。
    /// </summary>
    [Test]
    public async Task None_AsTask_should_return_original_option()
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
    /// 3. OptionがSomeの場合は完了済みTaskを返す。
    /// </summary>
    [Test]
    public void Some_AsTask_should_return_completed_task()
    {
        var some = Option<int>.Some(5);

        var task = some.AsTask();

        Assert.That(task.IsCompletedSuccessfully, Is.True);
    }

    /// <summary>
    /// 4. OptionがNoneの場合は完了済みTaskを返す。
    /// </summary>
    [Test]
    public void None_AsTask_should_return_completed_task()
    {
        var none = Option<int>.None;

        var task = none.AsTask();

        Assert.That(task.IsCompletedSuccessfully, Is.True);
    }

    /// <summary>
    /// 5. default OptionはNoneを保持する完了済みTaskを返す。
    /// </summary>
    [Test]
    public async Task Default_AsTask_should_return_none()
    {
        var defaultOption = default(Option<int>);

        var task = defaultOption.AsTask();
        var result = await task;

        Assert.Multiple(() =>
        {
            Assert.That(task.IsCompletedSuccessfully, Is.True);
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}