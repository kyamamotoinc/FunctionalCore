namespace FunctionalCore.Tests.OptionTests;

public class OptionMapTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はselectorを実行し、変換後の値を保持するSomeを返す。
    /// </summary>
    [Test]
    public void Some_Map_should_return_mapped_option()
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
    /// 2. OptionがSomeの場合はselectorによって値の型を変更できる。
    /// </summary>
    [Test]
    public void Some_Map_should_change_value_type()
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
    /// 3. OptionがSomeの場合はselectorを1回だけ実行する。
    /// </summary>
    [Test]
    public void Some_Map_should_invoke_selector_once()
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
    /// 4. OptionがNoneの場合はselectorを実行しない。
    /// </summary>
    [Test]
    public void None_Map_should_not_invoke_selector()
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
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 5. OptionがNoneの場合はNoneを返す。
    /// </summary>
    [Test]
    public void None_Map_should_return_none()
    {
        var none = Option<int>.None;

        var result = none.Map(x => x + 1);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 6. OptionがSomeの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_Map_should_throw_argument_null_exception_when_selector_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => some.Map(selector!));
    }

    /// <summary>
    /// 7. OptionがNoneの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_Map_should_throw_argument_null_exception_when_selector_is_null()
    {
        var none = Option<int>.None;
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => none.Map(selector!));
    }

    /// <summary>
    /// 8. OptionがSomeでselectorがnullを返した場合はNoneを返す。
    /// </summary>
    [Test]
    public void Some_Map_should_return_none_when_selector_returns_null()
    {
        var some = Option<int>.Some(5);

        var result = some.Map(_ => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 9. OptionがNoneの場合はnullを返すselectorでも実行せず、Noneを返す。
    /// </summary>
    [Test]
    public void None_Map_should_return_none_without_invoking_null_returning_selector()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = none.Map(_ =>
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
    /// 10. default OptionはNoneと同様にselectorを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public void Default_Map_should_return_none_without_invoking_selector()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = defaultOption.Map(x =>
        {
            count++;
            return x + 1;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 11. default Optionの場合でもselectorがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Map_should_throw_argument_null_exception_when_selector_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => defaultOption.Map(selector!));
    }

    /// <summary>
    /// 12. OptionがSomeでselectorが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_Map_should_propagate_exception_when_selector_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("selector error");

        Func<int, int> selector = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => some.Map(selector));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}