using FunctionalCore.Linq;

namespace FunctionalCore.Tests.OptionTests.Linq;

public class OptionSelectManyTests
{
    /// <summary>
    /// 1. 元のOptionとselectorが返すOptionがともにSomeの場合は、
    /// projectorの戻り値を保持するSomeを返す。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_return_projected_option()
    {
        var some = Option<int>.Some(5);

        var result = some.SelectMany(
            x => Option<int>.Some(x + 1),
            (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(11));
        });
    }

    /// <summary>
    /// 2. 元のOptionとselectorが返すOptionがともにSomeの場合は、
    /// projectorによって最終的な値の型を変更できる。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_change_value_type()
    {
        var some = Option<int>.Some(5);

        var result = some.SelectMany(
            x => Option<int>.Some(x + 1),
            (x, y) => $"{x}:{y}");

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("5:6"));
        });
    }

    /// <summary>
    /// 3. OptionがSomeの場合はselectorを1回だけ実行する。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_invoke_selector_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.SelectMany(
            x =>
            {
                count++;
                return Option<int>.Some(x + 1);
            },
            (x, y) => x + y);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. 元のOptionがNoneの場合はselectorを実行しない。
    /// </summary>
    [Test]
    public void None_SelectMany_should_not_invoke_selector()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = none.SelectMany(
            x =>
            {
                count++;
                return Option<int>.Some(x + 1);
            },
            (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 5. OptionがSomeでselectorがNoneを返した場合はNoneを返す。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_return_none_when_selector_returns_none()
    {
        var some = Option<int>.Some(5);

        var result = some.SelectMany(
            _ => Option<int>.None,
            (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 6. selectorがNoneを返した場合はprojectorを実行しない。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_not_invoke_projector_when_selector_returns_none()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        var result = some.SelectMany(
            _ => Option<int>.None,
            (x, y) =>
            {
                count++;
                return x + y;
            });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 7. 元のOptionとselectorが返すOptionがともにSomeの場合はprojectorを1回だけ実行する。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_invoke_projector_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.SelectMany(
            x => Option<int>.Some(x + 1),
            (x, y) =>
            {
                count++;
                return x + y;
            });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 8. OptionがSomeの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_throw_argument_null_exception_when_selector_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, Option<int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.SelectMany(selector!, (x, y) => x + y));
    }

    /// <summary>
    /// 9. OptionがSomeの場合でもprojectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_throw_argument_null_exception_when_projector_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, int, int>? projector = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.SelectMany(x => Option<int>.Some(x + 1), projector!));
    }

    /// <summary>
    /// 10. OptionがNoneの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_SelectMany_should_throw_argument_null_exception_when_selector_is_null()
    {
        var none = Option<int>.None;
        Func<int, Option<int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.SelectMany(selector!, (x, y) => x + y));
    }

    /// <summary>
    /// 11. OptionがNoneの場合でもprojectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_SelectMany_should_throw_argument_null_exception_when_projector_is_null()
    {
        var none = Option<int>.None;
        Func<int, int, int>? projector = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.SelectMany(x => Option<int>.Some(x + 1), projector!));
    }

    /// <summary>
    /// 12. OptionがSomeでselectorがdefault Optionを返した場合はNoneとして扱う。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_return_none_when_selector_returns_default_option()
    {
        var some = Option<int>.Some(5);

        var result = some.SelectMany(
            _ => default(Option<int>),
            (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 13. OptionがSomeでprojectorがnullを返した場合はNoneを返す。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_return_none_when_projector_returns_null()
    {
        var some = Option<int>.Some(5);

        var result = some.SelectMany(
            x => Option<int>.Some(x + 1),
            (_, _) => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 14. selectorがNoneを返した場合はnullを返すprojectorでも実行せず、Noneを返す。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_return_none_without_invoking_null_returning_projector()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        var result = some.SelectMany(
            _ => Option<int>.None,
            (_, _) =>
            {
                count++;
                return (string)null!;
            });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<string>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 15. LINQクエリ構文の複数fromでSelectManyを利用できる。
    /// </summary>
    [Test]
    public void SelectMany_should_support_query_syntax()
    {
        var some = Option<int>.Some(5);

        var result =
            from x in some
            from y in Option<int>.Some(x + 1)
            select x + y;

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(11));
        });
    }

    /// <summary>
    /// 16. LINQクエリ構文で中間OptionがNoneの場合はNoneを返す。
    /// </summary>
    [Test]
    public void SelectMany_query_syntax_should_return_none_when_intermediate_option_is_none()
    {
        var some = Option<int>.Some(5);

        var result =
            from x in some
            from y in Option<int>.None
            select x + y;

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 17. LINQクエリ構文で元のOptionがNoneの場合はNoneを返す。
    /// </summary>
    [Test]
    public void None_SelectMany_query_syntax_should_return_none()
    {
        var none = Option<int>.None;

        var result =
            from x in none
            from y in Option<int>.Some(x + 1)
            select x + y;

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 18. default OptionはNoneと同様にSelectManyでNoneを返す。
    /// </summary>
    [Test]
    public void Default_SelectMany_should_return_none()
    {
        var defaultOption = default(Option<int>);

        var result = defaultOption.SelectMany(
            x => Option<int>.Some(x + 1),
            (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 19. default Optionの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_SelectMany_should_throw_argument_null_exception_when_selector_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<int, Option<int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() =>
            defaultOption.SelectMany(selector!, (x, y) => x + y));
    }

    /// <summary>
    /// 20. default Optionの場合でもprojectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_SelectMany_should_throw_argument_null_exception_when_projector_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<int, int, int>? projector = null;

        Assert.Throws<ArgumentNullException>(() =>
            defaultOption.SelectMany(x => Option<int>.Some(x + 1), projector!));
    }

    /// <summary>
    /// 21. OptionがSomeでselectorが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_propagate_exception_when_selector_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("selector error");
        Func<int, Option<int>> selector = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() =>
            some.SelectMany(selector, (x, y) => x + y));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 22. OptionがSomeでprojectorが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_SelectMany_should_propagate_exception_when_projector_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("projector error");
        Func<int, int, int> projector = (_, _) => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() =>
            some.SelectMany(
                x => Option<int>.Some(x + 1),
                projector));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}