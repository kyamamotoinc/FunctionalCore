namespace FunctionalCore.Tests.OptionTests;

public class OptionMapTests
{
    /// <summary>
    /// 1. Some.Map は selector を実行し、変換後の値を持つ Some を返す
    /// </summary>
    [Test]
    public void Option_Some_Map_should_return_selector_result()
    {
        var some = Option<int>.Some(5);
        var result = some.Map(x => x + 1);

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
        var some = Option<int>.Some(5);
        var result = some.Map(x => $"value:{x}");

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
        var some = Option<int>.Some(5);
        int count = 0;

        some.Map(x =>
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
        var none = Option<int>.None;
        int count = 0;

        var result = none.Map(x =>
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
        var none = Option<int>.None;
        var result = none.Map(x => x + 1);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 6. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Map_null_selector_should_throw()
    {
        var some = Option<int>.Some(5);
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => some.Map(selector!));
    }

    /// <summary>
    /// 7. None でも selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Map_null_selector_should_throw()
    {
        var none = Option<int>.None;
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => none.Map(selector!));
    }

    /// <summary>
    /// 8. Some.Map で selector が null を返した場合は None に変換する
    /// </summary>
    [Test]
    public void Option_Some_Map_selector_returning_null_should_return_none()
    {
        var some = Option<int>.Some(5);
        var result = some.Map(_ => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 9. None.Map では null を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Option_None_Map_should_not_evaluate_null_returning_selector()
    {
        var none = Option<int>.None;
        var result = none.Map(_ => (string)null!);

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