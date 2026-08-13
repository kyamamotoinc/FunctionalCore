namespace FunctionalCore.Tests.OptionTests;

public class OptionMapTests
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
    /// 1. Some.Map は selector を実行し、変換後の値を持つ Some を返す
    /// </summary>
    [Test]
    public void Option_Some_Map_should_return_selector_result()
    {
        var result = _some.Map(x => x + 1);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. Some.Map は値の型を変更できる
    /// </summary>
    [Test]
    public void Option_Some_Map_should_change_value_type()
    {
        var result = _some.Map(x => $"value:{x}");

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. Some.Map は selector を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_Map_should_invoke_selector_once()
    {
        int count = 0;

        _some.Map(x =>
        {
            count++;
            return x + 1;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. None.Map は selector を実行しない
    /// </summary>
    [Test]
    public void Option_None_Map_should_not_invoke_selector()
    {
        int count = 0;

        var result = _none.Map(x =>
        {
            count++;
            return x + 1;
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result.HasValue, Is.False);
        });
    }

    /// <summary>
    /// 5. None.Map は None を返す
    /// </summary>
    [Test]
    public void Option_None_Map_should_return_none()
    {
        var result = _none.Map(x => x + 1);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 6. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Map_null_selector_should_throw()
    {
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => _some.Map(selector!));
    }

    /// <summary>
    /// 7. None でも selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Map_null_selector_should_throw()
    {
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => _none.Map(selector!));
    }

    /// <summary>
    /// 8. Some.Map で selector が null を返した場合は None に変換する
    /// </summary>
    [Test]
    public void Option_Some_Map_selector_returning_null_should_return_none()
    {
        var result = _some.Map(_ => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 9. None.Map では null を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Option_None_Map_should_not_evaluate_null_returning_selector()
    {
        var result = _none.Map(_ => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 10. Default Option は None と同様に selector を実行せず None を返す
    /// </summary>
    [Test]
    public void Option_Default_Map_should_return_none()
    {
        var option = default(Option<int>);
        int count = 0;

        var result = option.Map(x =>
        {
            count++;
            return x + 1;
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }
}