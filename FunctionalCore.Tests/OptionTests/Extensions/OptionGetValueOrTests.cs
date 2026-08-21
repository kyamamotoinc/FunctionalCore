using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionGetValueOrTests
{
    /// <summary>
    /// 1. OptionがSomeの場合は保持しているValueを返す。
    /// </summary>
    [Test]
    public void Some_GetValueOr_should_return_value()
    {
        var some = Option<int>.Some(5);

        var value = some.GetValueOr(10);

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. OptionがNoneの場合は指定されたfallbackを返す。
    /// </summary>
    [Test]
    public void None_GetValueOr_should_return_fallback()
    {
        var none = Option<int>.None;

        var value = none.GetValueOr(10);

        Assert.That(value, Is.EqualTo(10));
    }

    /// <summary>
    /// 3. 参照型のOptionがSomeの場合も保持しているValueを返す。
    /// </summary>
    [Test]
    public void Some_GetValueOr_should_return_value_for_reference_type()
    {
        var some = Option<string>.Some("value");

        var value = some.GetValueOr("fallback");

        Assert.That(value, Is.EqualTo("value"));
    }

    /// <summary>
    /// 4. 参照型のOptionがNoneの場合は指定されたfallbackを返す。
    /// </summary>
    [Test]
    public void None_GetValueOr_should_return_fallback_for_reference_type()
    {
        var none = Option<string>.None;

        var value = none.GetValueOr("fallback");

        Assert.That(value, Is.EqualTo("fallback"));
    }

    /// <summary>
    /// 5. OptionがSomeの場合はfallbackがnullでも保持しているValueを返す。
    /// </summary>
    [Test]
    public void Some_GetValueOr_should_return_value_when_fallback_is_null()
    {
        var some = Option<string>.Some("value");

        var value = some.GetValueOr(null!);

        Assert.That(value, Is.EqualTo("value"));
    }

    /// <summary>
    /// 6. OptionがNoneでfallbackがnullの場合はnullを返す。
    /// </summary>
    [Test]
    public void None_GetValueOr_should_return_null_when_fallback_is_null()
    {
        var none = Option<string>.None;

        var value = none.GetValueOr(null!);

        Assert.That(value, Is.Null);
    }

    /// <summary>
    /// 7. default OptionはNoneと同様に指定されたfallbackを返す。
    /// </summary>
    [Test]
    public void Default_GetValueOr_should_return_fallback()
    {
        var defaultOption = default(Option<int>);

        var value = defaultOption.GetValueOr(10);

        Assert.That(value, Is.EqualTo(10));
    }

    /// <summary>
    /// 8. default Optionでfallbackがnullの場合はNoneと同様にnullを返す。
    /// </summary>
    [Test]
    public void Default_GetValueOr_should_return_null_when_fallback_is_null()
    {
        var defaultOption = default(Option<string>);

        var value = defaultOption.GetValueOr(null!);

        Assert.That(value, Is.Null);
    }
}