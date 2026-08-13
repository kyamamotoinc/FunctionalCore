namespace FunctionalCore.Tests.OptionTests;

public class OptionBindTests
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
    /// 1. Some.Bind は binder を実行し、その Option を返す
    /// </summary>
    [Test]
    public void Option_Some_Bind_should_return_binder_result()
    {
        var result = _some.Bind(x => Option<int>.Some(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. Some.Bind は値の型を変更できる
    /// </summary>
    [Test]
    public void Option_Some_Bind_should_change_value_type()
    {
        var result = _some.Bind(x => Option<string>.Some($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. Some.Bind の binder が None を返した場合は None を返す
    /// </summary>
    [Test]
    public void Option_Some_Bind_binder_returning_none_should_return_none()
    {
        var result = _some.Bind(_ => Option<int>.None);

        Assert.That(result.HasValue, Is.False);
    }

    /// <summary>
    /// 4. Some.Bind は binder を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_Bind_should_invoke_binder_once()
    {
        int count = 0;

        _some.Bind(x =>
        {
            count++;
            return Option<int>.Some(x + 1);
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. None.Bind は binder を実行しない
    /// </summary>
    [Test]
    public void Option_None_Bind_should_not_invoke_binder()
    {
        int count = 0;

        var result = _none.Bind(x =>
        {
            count++;
            return Option<int>.Some(x + 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result.HasValue, Is.False);
        });
    }

    /// <summary>
    /// 6. None.Bind は None を返す
    /// </summary>
    [Test]
    public void Option_None_Bind_should_return_none()
    {
        var result = _none.Bind(x => Option<int>.Some(x + 1));

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 7. binder が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Bind_null_binder_should_throw()
    {
        Func<int, Option<string>>? binder = null;

        Assert.Throws<ArgumentNullException>(() => _some.Bind(binder!));
    }

    /// <summary>
    /// 8. None でも binder が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Bind_null_binder_should_throw()
    {
        Func<int, Option<string>>? binder = null;

        Assert.Throws<ArgumentNullException>(() => _none.Bind(binder!));
    }

    /// <summary>
    /// 9. binder が Default Option を返した場合は None として扱われる
    /// </summary>
    [Test]
    public void Option_Some_Bind_default_option_should_return_none()
    {
        var result = _some.Bind(_ => default(Option<string>));

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 10. None.Bind では Default Option を返す binder でも実行されない
    /// </summary>
    [Test]
    public void Option_None_Bind_should_not_evaluate_default_binder_result()
    {
        var result = _none.Bind(_ => default(Option<string>));

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 11. Default Option は None と同様に binder を実行せず None を返す
    /// </summary>
    [Test]
    public void Option_Default_Bind_should_return_none()
    {
        var option = default(Option<int>);
        int count = 0;

        var result = option.Bind(x =>
        {
            count++;
            return Option<int>.Some(x + 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}