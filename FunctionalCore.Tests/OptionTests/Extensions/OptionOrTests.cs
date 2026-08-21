using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionOrTests
{
    /// <summary>
    /// 1. OptionがSomeの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public void Some_Or_should_return_original_option()
    {
        var some = Option<int>.Some(5);
        var other = Option<int>.Some(10);

        var result = some.Or(other);

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 2. OptionがNoneの場合は代替Optionを返す。
    /// </summary>
    [Test]
    public void None_Or_should_return_other_option()
    {
        var none = Option<int>.None;
        var other = Option<int>.Some(10);

        var result = none.Or(other);

        Assert.That(result, Is.EqualTo(other));
    }

    /// <summary>
    /// 3. OptionがNoneで代替OptionもNoneの場合はNoneを返す。
    /// </summary>
    [Test]
    public void None_Or_should_return_none_when_other_is_none()
    {
        var none = Option<int>.None;

        var result = none.Or(Option<int>.None);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 4. OptionがSomeの場合は代替Optionを採用しない。
    /// </summary>
    [Test]
    public void Some_Or_should_ignore_other_option()
    {
        var some = Option<int>.Some(5);
        var other = Option<int>.Some(10);

        var result = some.Or(other);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 5. OptionがSomeの場合は代替Optionがdefaultでも元のOptionを返す。
    /// </summary>
    [Test]
    public void Some_Or_should_return_original_option_when_other_is_default()
    {
        var some = Option<int>.Some(5);
        var other = default(Option<int>);

        var result = some.Or(other);

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 6. OptionがNoneで代替Optionがdefaultの場合はNoneを返す。
    /// </summary>
    [Test]
    public void None_Or_should_return_none_when_other_is_default()
    {
        var none = Option<int>.None;
        var other = default(Option<int>);

        var result = none.Or(other);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 7. OptionがSomeの場合はfactoryを実行せず、元のOptionを返す。
    /// </summary>
    [Test]
    public void Some_Or_factory_should_return_original_option_without_invoking_factory()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        var result = some.Or(() =>
        {
            count++;
            return Option<int>.Some(10);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(some));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 8. OptionがNoneの場合はfactoryを1回だけ実行し、そのOptionを返す。
    /// </summary>
    [Test]
    public void None_Or_factory_should_invoke_factory_once_and_return_its_option()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = none.Or(() =>
        {
            count++;
            return Option<int>.Some(10);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(10));
            Assert.That(count, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 9. OptionがNoneでfactoryがNoneを返した場合はNoneを返す。
    /// </summary>
    [Test]
    public void None_Or_factory_should_return_none_when_factory_returns_none()
    {
        var none = Option<int>.None;

        var result = none.Or(() => Option<int>.None);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 10. OptionがNoneでfactoryがdefault Optionを返した場合はNoneとして扱う。
    /// </summary>
    [Test]
    public void None_Or_factory_should_return_none_when_factory_returns_default_option()
    {
        var none = Option<int>.None;

        var result = none.Or(() => default(Option<int>));

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 11. OptionがNoneの場合でもfactoryがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_Or_factory_should_throw_argument_null_exception_when_factory_is_null()
    {
        var none = Option<int>.None;
        Func<Option<int>>? factory = null;

        Assert.Throws<ArgumentNullException>(() => none.Or(factory!));
    }

    /// <summary>
    /// 12. OptionがSomeの場合でもfactoryがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_Or_factory_should_throw_argument_null_exception_when_factory_is_null()
    {
        var some = Option<int>.Some(5);
        Func<Option<int>>? factory = null;

        Assert.Throws<ArgumentNullException>(() => some.Or(factory!));
    }

    /// <summary>
    /// 13. default OptionはNoneと同様に代替Optionを返す。
    /// </summary>
    [Test]
    public void Default_Or_should_return_other_option()
    {
        var defaultOption = default(Option<int>);
        var other = Option<int>.Some(10);

        var result = defaultOption.Or(other);

        Assert.That(result, Is.EqualTo(other));
    }

    /// <summary>
    /// 14. default OptionはNoneと同様にfactoryを1回だけ実行し、そのOptionを返す。
    /// </summary>
    [Test]
    public void Default_Or_factory_should_invoke_factory_once_and_return_its_option()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = defaultOption.Or(() =>
        {
            count++;
            return Option<int>.Some(10);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(10));
            Assert.That(count, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 15. default Optionの場合でもfactoryがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Or_factory_should_throw_argument_null_exception_when_factory_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<Option<int>>? factory = null;

        Assert.Throws<ArgumentNullException>(() => defaultOption.Or(factory!));
    }

    /// <summary>
    /// 16. OptionがNoneでfactoryが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void None_Or_factory_should_propagate_exception_when_factory_throws()
    {
        var none = Option<int>.None;
        var expectedException = new NotSupportedException("factory error");

        Func<Option<int>> factory = () => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => none.Or(factory));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}