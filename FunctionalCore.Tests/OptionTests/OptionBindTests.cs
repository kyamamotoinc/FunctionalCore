namespace FunctionalCore.Tests.OptionTests;

public class OptionBindTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はbinderを実行し、そのOptionを返す。
    /// </summary>
    [Test]
    public void Some_Bind_should_return_binder_result()
    {
        var some = Option<int>.Some(5);

        var result = some.Bind(x => Option<int>.Some(x + 1));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. OptionがSomeの場合はbinderによって値の型を変更できる。
    /// </summary>
    [Test]
    public void Some_Bind_should_change_value_type()
    {
        var some = Option<int>.Some(5);

        var result = some.Bind(x => Option<string>.Some($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. OptionがSomeでbinderがNoneを返した場合はNoneを返す。
    /// </summary>
    [Test]
    public void Some_Bind_should_return_none_when_binder_returns_none()
    {
        var some = Option<int>.Some(5);

        var result = some.Bind(_ => Option<int>.None);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 4. OptionがSomeの場合はbinderを1回だけ実行する。
    /// </summary>
    [Test]
    public void Some_Bind_should_invoke_binder_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Bind(x =>
        {
            count++;
            return Option<int>.Some(x + 1);
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 5. OptionがNoneの場合はbinderを実行しない。
    /// </summary>
    [Test]
    public void None_Bind_should_not_invoke_binder()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = none.Bind(x =>
        {
            count++;
            return Option<int>.Some(x + 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 6. OptionがNoneの場合はNoneを返す。
    /// </summary>
    [Test]
    public void None_Bind_should_return_none()
    {
        var none = Option<int>.None;

        var result = none.Bind(x => Option<int>.Some(x + 1));

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 7. OptionがSomeの場合でもbinderがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_Bind_should_throw_argument_null_exception_when_binder_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, Option<string>>? binder = null;

        Assert.Throws<ArgumentNullException>(() => some.Bind(binder!));
    }

    /// <summary>
    /// 8. OptionがNoneの場合でもbinderがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_Bind_should_throw_argument_null_exception_when_binder_is_null()
    {
        var none = Option<int>.None;
        Func<int, Option<string>>? binder = null;

        Assert.Throws<ArgumentNullException>(() => none.Bind(binder!));
    }

    /// <summary>
    /// 9. OptionがSomeでbinderがdefault Optionを返した場合はNoneとして扱う。
    /// </summary>
    [Test]
    public void Some_Bind_should_return_none_when_binder_returns_default_option()
    {
        var some = Option<int>.Some(5);

        var result = some.Bind(_ => default(Option<string>));

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 10. OptionがNoneの場合はdefault Optionを返すbinderでも実行せず、Noneを返す。
    /// </summary>
    [Test]
    public void None_Bind_should_return_none_without_invoking_default_option_binder()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = none.Bind(_ =>
        {
            count++;
            return default(Option<string>);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<string>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 11. default OptionはNoneと同様にbinderを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public void Default_Bind_should_return_none_without_invoking_binder()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = defaultOption.Bind(x =>
        {
            count++;
            return Option<int>.Some(x + 1);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 12. OptionがSomeでbinderが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_Bind_should_propagate_exception_when_binder_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("binder error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            some.Bind<int>(_ => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}