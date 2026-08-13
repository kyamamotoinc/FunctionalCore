using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.OptionTests.AsyncExtensions;

public class OptionMapAsyncTests
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
    /// 1. Some.MapAsync は selector を実行し、変換後の値を持つ Some を返す
    /// </summary>
    [Test]
    public async Task Option_Some_MapAsync_should_return_selector_result()
    {
        var result = await _some.AsTask().MapAsync(x => Task.FromResult(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. Some.MapAsync は値の型を変更できる
    /// </summary>
    [Test]
    public async Task Option_Some_MapAsync_should_change_value_type()
    {
        var result = await _some.AsTask().MapAsync(x => Task.FromResult($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. Some.MapAsync は selector を1回だけ実行する
    /// </summary>
    [Test]
    public async Task Option_Some_MapAsync_should_invoke_selector_once()
    {
        int count = 0;

        await _some.AsTask().MapAsync(x =>
        {
            count++;
            return Task.FromResult(x + 1);
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. None.MapAsync は selector を実行しない
    /// </summary>
    [Test]
    public async Task Option_None_MapAsync_should_not_invoke_selector()
    {
        int count = 0;

        var result = await _none.AsTask().MapAsync(x =>
        {
            count++;
            return Task.FromResult(x + 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }

    /// <summary>
    /// 5. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_MapAsync_null_selector_should_throw()
    {
        Func<int, Task<string>>? selector = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _some.AsTask().MapAsync(selector!));
    }

    /// <summary>
    /// 6. None でも selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_MapAsync_null_selector_should_throw()
    {
        Func<int, Task<string>>? selector = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await _none.AsTask().MapAsync(selector!));
    }

    /// <summary>
    /// 7. optionTask が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_MapAsync_null_option_task_should_throw()
    {
        Task<Option<int>>? optionTask = null;

        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await optionTask!.MapAsync(x => Task.FromResult(x + 1)));
    }

    /// <summary>
    /// 8. selector が null Task を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Option_Some_MapAsync_selector_returning_null_task_should_throw()
    {
        Func<int, Task<string>> selector = _ => null!;

        Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _some.AsTask().MapAsync(selector));
    }

    /// <summary>
    /// 9. selector の Task が null 値を返した場合は None を返す
    /// </summary>
    [Test]
    public async Task Option_Some_MapAsync_selector_returning_null_value_should_return_none()
    {
        var result = await _some.AsTask().MapAsync(_ => Task.FromResult((string)null!));

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 10. None.MapAsync では null Task を返す selector でも実行されない
    /// </summary>
    [Test]
    public async Task Option_None_MapAsync_should_not_evaluate_null_task_selector()
    {
        Func<int, Task<string>> selector = _ => null!;

        var result = await _none.AsTask().MapAsync(selector);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 11. None.MapAsync では null 値を返す selector でも実行されない
    /// </summary>
    [Test]
    public async Task Option_None_MapAsync_should_not_evaluate_null_value_selector()
    {
        var result = await _none.AsTask().MapAsync(_ => Task.FromResult((string)null!));

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 12. Default Option は None と同様に selector を実行しない
    /// </summary>
    [Test]
    public async Task Option_Default_MapAsync_should_not_invoke_selector()
    {
        var option = default(Option<int>);
        int count = 0;

        var result = await option.AsTask().MapAsync(x =>
        {
            count++;
            return Task.FromResult(x + 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}