using FunctionalCore.Linq;

namespace FunctionalCore.Tests.OptionTests.Linq;

public class OptionSelectTests
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
    /// 1. Some.Select は selector を実行し、変換後の値を持つ Some を返す
    /// </summary>
    [Test]
    public void Option_Some_Select_should_return_selector_result()
    {
        var result = _some.Select(x => x + 1);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. Some.Select は値の型を変更できる
    /// </summary>
    [Test]
    public void Option_Some_Select_should_change_value_type()
    {
        var result = _some.Select(x => $"value:{x}");

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. Some.Select は selector を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_Select_should_invoke_selector_once()
    {
        int count = 0;

        _some.Select(x =>
        {
            count++;
            return x + 1;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. None.Select は selector を実行しない
    /// </summary>
    [Test]
    public void Option_None_Select_should_not_invoke_selector()
    {
        int count = 0;

        var result = _none.Select(x =>
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

    /// <summary>
    /// 5. None.Select は None を返す
    /// </summary>
    [Test]
    public void Option_None_Select_should_return_none()
    {
        var result = _none.Select(x => x + 1);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 6. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Select_null_selector_should_throw()
    {
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => _some.Select(selector!));
    }

    /// <summary>
    /// 7. None でも selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Select_null_selector_should_throw()
    {
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => _none.Select(selector!));
    }

    /// <summary>
    /// 8. Some.Select で selector が null を返した場合は None を返す
    /// </summary>
    [Test]
    public void Option_Some_Select_selector_returning_null_should_return_none()
    {
        var result = _some.Select(_ => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 9. None.Select では null を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Option_None_Select_should_not_evaluate_null_returning_selector()
    {
        var result = _none.Select(_ => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 10. LINQ クエリ構文の select で Select が利用できる
    /// </summary>
    [Test]
    public void Option_Select_should_support_query_syntax()
    {
        var result =
            from x in _some
            select x + 1;

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 11. None に対する LINQ クエリ構文の select は None を返す
    /// </summary>
    [Test]
    public void Option_None_Select_query_syntax_should_return_none()
    {
        var result =
            from x in _none
            select x + 1;

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 12. Default Option は None と同様に扱われる
    /// </summary>
    [Test]
    public void Option_Default_Select_should_return_none()
    {
        var option = default(Option<int>);

        var result = option.Select(x => x + 1);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }
}