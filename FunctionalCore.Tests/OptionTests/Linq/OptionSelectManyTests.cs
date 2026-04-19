using FunctionalCore.Linq;

namespace FunctionalCore.Tests.OptionTests.Linq;

public class OptionSelectManyTests
{
    private Option<int> _some1;
    private Option<string> _some2;
    private Option<double> _some3;
    private Option<int> _none1;
    private Option<string> _none2;
    private Option<double> _none3;

    [SetUp]
    public void Setup()
    {
        _some1 = Option<int>.Some(5);
        _some2 = Option<string>.Some("hello");
        _some3 = Option<double>.Some(3.14);
        _none1 = Option<int>.None;
        _none2 = Option<string>.None;
        _none3 = Option<double>.None;
    }

    /// <summary>
    /// 1. 全て Some → 最終結果が正しい値を持つ
    /// </summary>
    [Test]
    public void SelectMany_all_some_should_return_some()
    {
        var result =
            from x in _some1
            from y in _some2
            from z in _some3
            select $"{x}-{y}:{z}";

        Assert.That(result.HasValue, Is.True);
        Assert.That(result.Value, Is.EqualTo("5-hello:3.14"));
    }

    /// <summary>
    /// 2. 最初が None → None が伝播し後続はスキップされる
    /// </summary>
    [Test]
    public void SelectMany_first_none_should_propagate_none()
    {
        var result =
            from x in _none1
            from y in _some2
            from z in _some3
            select $"{x}-{y}:{z}";

        Assert.That(result.HasValue, Is.False);
    }

    /// <summary>
    /// 3. 2番目が None → None が伝播し後続はスキップされる
    /// </summary>
    [Test]
    public void SelectMany_second_none_should_propagate_none()
    {
        var result =
            from x in _some1
            from y in _none2
            from z in _some3
            select $"{x}-{y}:{z}";

        Assert.That(result.HasValue, Is.False);
    }

    /// <summary>
    /// 4. 3番目が None → None が伝播する
    /// </summary>
    [Test]
    public void SelectMany_third_none_should_propagate_none()
    {
        var result =
            from x in _some1
            from y in _some2
            from z in _none3
            select $"{x}-{y}:{z}";

        Assert.That(result.HasValue, Is.False);
    }

    /// <summary>
    /// 5. selector が null → ArgumentNullException
    /// </summary>
    [Test]
    public void SelectMany_null_selector_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _some1.SelectMany(
                (Func<int, Option<string>>)null!,
                (x, y) => $"{x}-{y}"
            ));
    }

    /// <summary>
    /// 6. projector が null → ArgumentNullException
    /// </summary>
    [Test]
    public void SelectMany_null_projector_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _some1.SelectMany(
                x => _some2,
                (Func<int, string, string>)null!
            ));
    }

    /// <summary>
    /// 7. default の Option → Some として扱われない
    /// </summary>
    [Test]
    public void SelectMany_default_option_should_propagate_none()
    {
        var defaultOption = default(Option<int>);
        var result =
            from x in defaultOption
            from y in _some2
            select $"{x}-{y}";

        Assert.That(result.HasValue, Is.False);
    }
}

