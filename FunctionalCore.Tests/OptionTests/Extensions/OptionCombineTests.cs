using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionCombineTests
{
    /// <summary>
    /// 1. 両方のOptionがSomeの場合はselectorを実行し、組み合わせた値を保持するSomeを返す。
    /// </summary>
    [Test]
    public void Some_Some_Combine_should_return_combined_option()
    {
        var first = Option<int>.Some(3);
        var second = Option<int>.Some(5);

        var result = first.Combine(second, (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(8));
        });
    }

    /// <summary>
    /// 2. 両方のOptionがSomeの場合はselectorによって値の型を変更できる。
    /// </summary>
    [Test]
    public void Some_Some_Combine_should_change_value_type()
    {
        var first = Option<int>.Some(3);
        var second = Option<int>.Some(5);

        var result = first.Combine(second, (x, y) => $"{x}:{y}");

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("3:5"));
        });
    }

    /// <summary>
    /// 3. 両方のOptionがSomeの場合はselectorを1回だけ実行する。
    /// </summary>
    [Test]
    public void Some_Some_Combine_should_invoke_selector_once()
    {
        var first = Option<int>.Some(3);
        var second = Option<int>.Some(5);
        int count = 0;

        first.Combine(second, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. 1つ目のOptionがNoneの場合はNoneを返す。
    /// </summary>
    [Test]
    public void None_Some_Combine_should_return_none()
    {
        var first = Option<int>.None;
        var second = Option<int>.Some(5);

        var result = first.Combine(second, (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 5. 2つ目のOptionがNoneの場合はNoneを返す。
    /// </summary>
    [Test]
    public void Some_None_Combine_should_return_none()
    {
        var first = Option<int>.Some(3);
        var second = Option<int>.None;

        var result = first.Combine(second, (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 6. 両方のOptionがNoneの場合はNoneを返す。
    /// </summary>
    [Test]
    public void None_None_Combine_should_return_none()
    {
        var first = Option<int>.None;
        var second = Option<int>.None;

        var result = first.Combine(second, (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 7. 1つ目のOptionがNoneの場合はselectorを実行しない。
    /// </summary>
    [Test]
    public void None_Some_Combine_should_not_invoke_selector()
    {
        var first = Option<int>.None;
        var second = Option<int>.Some(5);
        int count = 0;

        first.Combine(second, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 8. 2つ目のOptionがNoneの場合はselectorを実行しない。
    /// </summary>
    [Test]
    public void Some_None_Combine_should_not_invoke_selector()
    {
        var first = Option<int>.Some(3);
        var second = Option<int>.None;
        int count = 0;

        first.Combine(second, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 9. 両方のOptionがSomeの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_Some_Combine_should_throw_argument_null_exception_when_selector_is_null()
    {
        var first = Option<int>.Some(3);
        var second = Option<int>.Some(5);
        Func<int, int, int>? selector = null;

        Assert.Throws<ArgumentNullException>(() => first.Combine(second, selector!));
    }

    /// <summary>
    /// 10. 1つ目のOptionがNoneの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_Some_Combine_should_throw_argument_null_exception_when_selector_is_null()
    {
        var first = Option<int>.None;
        var second = Option<int>.Some(5);
        Func<int, int, int>? selector = null;

        Assert.Throws<ArgumentNullException>(() => first.Combine(second, selector!));
    }

    /// <summary>
    /// 11. 2つ目のOptionがNoneの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_None_Combine_should_throw_argument_null_exception_when_selector_is_null()
    {
        var first = Option<int>.Some(3);
        var second = Option<int>.None;
        Func<int, int, int>? selector = null;

        Assert.Throws<ArgumentNullException>(() => first.Combine(second, selector!));
    }

    /// <summary>
    /// 12. 両方のOptionがSomeでselectorがnullを返した場合はNoneを返す。
    /// </summary>
    [Test]
    public void Some_Some_Combine_should_return_none_when_selector_returns_null()
    {
        var first = Option<int>.Some(3);
        var second = Option<int>.Some(5);

        var result = first.Combine(second, (_, _) => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 13. 1つ目のOptionがNoneの場合はnullを返すselectorでも実行せず、Noneを返す。
    /// </summary>
    [Test]
    public void None_Some_Combine_should_return_none_without_invoking_null_returning_selector()
    {
        var first = Option<int>.None;
        var second = Option<int>.Some(5);
        int count = 0;

        var result = first.Combine(second, (_, _) =>
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
    /// 14. 2つ目のOptionがNoneの場合はnullを返すselectorでも実行せず、Noneを返す。
    /// </summary>
    [Test]
    public void Some_None_Combine_should_return_none_without_invoking_null_returning_selector()
    {
        var first = Option<int>.Some(3);
        var second = Option<int>.None;
        int count = 0;

        var result = first.Combine(second, (_, _) =>
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
    /// 15. 1つ目がdefault Optionの場合はNoneとして扱う。
    /// </summary>
    [Test]
    public void Default_Some_Combine_should_return_none()
    {
        var first = default(Option<int>);
        var second = Option<int>.Some(5);

        var result = first.Combine(second, (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 16. 2つ目がdefault Optionの場合はNoneとして扱う。
    /// </summary>
    [Test]
    public void Some_Default_Combine_should_return_none()
    {
        var first = Option<int>.Some(3);
        var second = default(Option<int>);

        var result = first.Combine(second, (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 17. default Optionの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Some_Combine_should_throw_argument_null_exception_when_selector_is_null()
    {
        var first = default(Option<int>);
        var second = Option<int>.Some(5);
        Func<int, int, int>? selector = null;

        Assert.Throws<ArgumentNullException>(() => first.Combine(second, selector!));
    }

    /// <summary>
    /// 18. 両方のOptionがSomeでselectorが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_Some_Combine_should_propagate_exception_when_selector_throws()
    {
        var first = Option<int>.Some(3);
        var second = Option<int>.Some(5);
        var expectedException = new NotSupportedException("selector error");

        Func<int, int, int> selector = (_, _) => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => first.Combine(second, selector));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}