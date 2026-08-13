using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionGetValueOrTests
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
    /// 1. Some.GetValueOr は内部の Value を返す
    /// </summary>
    [Test]
    public void Option_Some_GetValueOr_should_return_inner_value()
    {
        var value = _some.GetValueOr(999);

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. None.GetValueOr は fallback を返す
    /// </summary>
    [Test]
    public void Option_None_GetValueOr_should_return_fallback()
    {
        var value = _none.GetValueOr(999);

        Assert.That(value, Is.EqualTo(999));
    }

    /// <summary>
    /// 3. None.GetValueOr は default 値を fallback として返せる
    /// </summary>
    [Test]
    public void Option_None_GetValueOr_with_default_should_return_default()
    {
        var value = _none.GetValueOr(default);

        Assert.That(value, Is.EqualTo(default(int)));
    }

    /// <summary>
    /// 4. Some.GetValueOr は Value が default 値でも fallback を使用しない
    /// </summary>
    [Test]
    public void Option_Some_GetValueOr_with_default_value_should_ignore_fallback()
    {
        var some = Option<int>.Some(0);

        var value = some.GetValueOr(999);

        Assert.That(value, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. 参照型の None.GetValueOr は fallback の同一インスタンスを返す
    /// </summary>
    [Test]
    public void Option_None_GetValueOr_reference_type_should_return_same_instance()
    {
        var fallback = new object();
        var none = Option<object>.None;

        var value = none.GetValueOr(fallback);

        Assert.That(value, Is.SameAs(fallback));
    }

    /// <summary>
    /// 6. 参照型の Some.GetValueOr は内部のインスタンスを返す
    /// </summary>
    [Test]
    public void Option_Some_GetValueOr_reference_type_should_return_same_instance()
    {
        var original = new object();
        var fallback = new object();
        var some = Option<object>.Some(original);

        var value = some.GetValueOr(fallback);

        Assert.That(value, Is.SameAs(original));
    }

    /// <summary>
    /// 7. fallback が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_GetValueOr_null_fallback_should_throw()
    {
        var none = Option<string>.None;

        Assert.Throws<ArgumentNullException>(() => none.GetValueOr(null!));
    }

    /// <summary>
    /// 8. Some でも fallback が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Some_GetValueOr_null_fallback_should_throw()
    {
        var some = Option<string>.Some("value");

        Assert.Throws<ArgumentNullException>(() => some.GetValueOr(null!));
    }

    /// <summary>
    /// 9. Default Option は None と同様に fallback を返す
    /// </summary>
    [Test]
    public void Option_Default_GetValueOr_should_return_fallback()
    {
        var option = default(Option<int>);

        var value = option.GetValueOr(999);

        Assert.That(value, Is.EqualTo(999));
    }
}