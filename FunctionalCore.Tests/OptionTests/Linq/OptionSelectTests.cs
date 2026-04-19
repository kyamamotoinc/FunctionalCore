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
    /// 1. Some.Select は selector を実行し、Some を返す
    /// </summary>
    [Test]
    public void Option_Some_Select_should_return_selector_result()
    {
        var opt = _some.Select(x => x + 1);
        var opt1 = from x in _some
                   select x + 1;

        Assert.That(opt.Value, Is.EqualTo(6));
        Assert.That(opt1.Value, Is.EqualTo(6));
    }

    /// <summary>
    /// 2. None.Select は selector を実行しない
    /// </summary>
    [Test]
    public void Option_None_Select_should_not_invoke_selector()
    {
        int count = 0;
        Func<int, int> selector = x =>
                {
                    count++;
                    return x + 1;
                };

        var opt = _none.Select(selector);

        var opt1 = from x in _none
                   select selector;

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 3. selector が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Option_Select_null_selector_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _some.Select<int, int>(null!));
    }

    /// <summary>
    /// 4. selector が null を返した場合 → None に変換される
    /// </summary>
    [Test]
    public void Option_Select_selector_returning_null_should_return_none()
    {
        var result =
            from x in _some
            select (string)null!;

        Assert.That(result.HasValue, Is.False);
    }
}
