using FunctionalCore.Linq;

namespace FunctionalCore.Tests.OptionTests.Linq;

public class OptionSelectManyTests
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
    /// 1. 元の Option と中間 Option がともに Some の場合は projector の結果を持つ Some を返す
    /// </summary>
    [Test]
    public void Option_Some_SelectMany_should_return_projected_value()
    {
        var result = _some.SelectMany(
            x => Option<int>.Some(x + 1),
            (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(11));
        });
    }

    /// <summary>
    /// 2. SelectMany は最終的な値の型を変更できる
    /// </summary>
    [Test]
    public void Option_Some_SelectMany_should_change_value_type()
    {
        var result = _some.SelectMany(
            x => Option<int>.Some(x + 1),
            (x, y) => $"{x}:{y}");

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("5:6"));
        });
    }

    /// <summary>
    /// 3. Some.SelectMany は selector を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_SelectMany_should_invoke_selector_once()
    {
        int count = 0;

        _some.SelectMany(
            x =>
            {
                count++;
                return Option<int>.Some(x + 1);
            },
            (x, y) => x + y);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. 元の Option が None の場合は selector を実行しない
    /// </summary>
    [Test]
    public void Option_None_SelectMany_should_not_invoke_selector()
    {
        int count = 0;

        var result = _none.SelectMany(
            x =>
            {
                count++;
                return Option<int>.Some(x + 1);
            },
            (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }

    /// <summary>
    /// 5. selector が None を返した場合は None を返す
    /// </summary>
    [Test]
    public void Option_Some_SelectMany_selector_returning_none_should_return_none()
    {
        var result = _some.SelectMany(
            _ => Option<int>.None,
            (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 6. selector が None を返した場合は projector を実行しない
    /// </summary>
    [Test]
    public void Option_Some_SelectMany_selector_none_should_not_invoke_projector()
    {
        int count = 0;

        var result = _some.SelectMany(
            _ => Option<int>.None,
            (x, y) =>
            {
                count++;
                return x + y;
            });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }

    /// <summary>
    /// 7. 両方が Some の場合は projector を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_SelectMany_should_invoke_projector_once()
    {
        int count = 0;

        _some.SelectMany(
            x => Option<int>.Some(x + 1),
            (x, y) =>
            {
                count++;
                return x + y;
            });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 8. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_SelectMany_null_selector_should_throw()
    {
        Func<int, Option<int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() =>
            _some.SelectMany(selector!, (x, y) => x + y));
    }

    /// <summary>
    /// 9. projector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_SelectMany_null_projector_should_throw()
    {
        Func<int, int, int>? projector = null;

        Assert.Throws<ArgumentNullException>(() =>
            _some.SelectMany(x => Option<int>.Some(x + 1), projector!));
    }

    /// <summary>
    /// 10. None でも selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_SelectMany_null_selector_should_throw()
    {
        Func<int, Option<int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() =>
            _none.SelectMany(selector!, (x, y) => x + y));
    }

    /// <summary>
    /// 11. None でも projector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_SelectMany_null_projector_should_throw()
    {
        Func<int, int, int>? projector = null;

        Assert.Throws<ArgumentNullException>(() =>
            _none.SelectMany(x => Option<int>.Some(x + 1), projector!));
    }

    /// <summary>
    /// 12. selector が Default Option を返した場合は None を返す
    /// </summary>
    [Test]
    public void Option_Some_SelectMany_selector_returning_default_should_return_none()
    {
        var result = _some.SelectMany(
            _ => default(Option<int>),
            (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 13. projector が null を返した場合は None を返す
    /// </summary>
    [Test]
    public void Option_Some_SelectMany_projector_returning_null_should_return_none()
    {
        var result = _some.SelectMany(
            x => Option<int>.Some(x + 1),
            (_, _) => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 14. selector が None を返した場合は null を返す projector でも実行されない
    /// </summary>
    [Test]
    public void Option_SelectMany_selector_none_should_not_evaluate_null_returning_projector()
    {
        var result = _some.SelectMany(
            _ => Option<int>.None,
            (_, _) => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 15. LINQ クエリ構文の複数 from で SelectMany が利用できる
    /// </summary>
    [Test]
    public void Option_SelectMany_should_support_query_syntax()
    {
        var result =
            from x in _some
            from y in Option<int>.Some(x + 1)
            select x + y;

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(11));
        });
    }

    /// <summary>
    /// 16. LINQ クエリ構文で中間 Option が None の場合は None を返す
    /// </summary>
    [Test]
    public void Option_SelectMany_query_syntax_intermediate_none_should_return_none()
    {
        var result =
            from x in _some
            from y in Option<int>.None
            select x + y;

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 17. LINQ クエリ構文で元の Option が None の場合は None を返す
    /// </summary>
    [Test]
    public void Option_SelectMany_query_syntax_source_none_should_return_none()
    {
        var result =
            from x in _none
            from y in Option<int>.Some(x + 1)
            select x + y;

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 18. Default Option は None と同様に扱われる
    /// </summary>
    [Test]
    public void Option_Default_SelectMany_should_return_none()
    {
        var option = default(Option<int>);

        var result = option.SelectMany(
            x => Option<int>.Some(x + 1),
            (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }
}