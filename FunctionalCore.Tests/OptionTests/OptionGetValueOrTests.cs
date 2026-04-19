using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests;

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
    /// 1. Some の場合は fallback を無視して内部の値を返す（Some → value）
    /// </summary>
    [Test]
    public void Some_GetValueOr_should_return_inner_value()
    {
        var value = _some.GetValueOr(999);

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. None の場合は fallback を返す（None → fallback）
    /// </summary>
    [Test]
    public void None_GetValueOr_should_return_fallback()
    {
        var value = _none.GetValueOr(999);

        Assert.That(value, Is.EqualTo(999));
    }

    /// <summary>
    /// 3. fallback が default 値でも正しく返される（None → default）
    /// </summary>
    [Test]
    public void None_GetValueOr_with_default_should_return_default()
    {
        var value = _none.GetValueOr(default);

        Assert.That(value, Is.EqualTo(default(int)));
    }

    /// <summary>
    /// 4. Some の値が default と同じでも fallback は使われない（Some優先）
    /// </summary>
    [Test]
    public void Some_with_default_value_should_ignore_fallback()
    {
        var some = Option<int>.Some(0);

        var value = some.GetValueOr(999);

        Assert.That(value, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. 参照型: None の場合は fallback の参照をそのまま返す
    /// </summary>
    [Test]
    public void None_GetValueOr_reference_type_should_return_same_instance()
    {
        var fallback = "fallback";
        var none = Option<string>.None;

        var value = none.GetValueOr(fallback);

        Assert.That(value, Is.SameAs(fallback));
    }

    /// <summary>
    /// 6. 参照型: Some の場合は fallback を無視する
    /// </summary>
    [Test]
    public void Some_GetValueOr_reference_type_should_ignore_fallback()
    {
        var some = Option<string>.Some("value");

        var value = some.GetValueOr("fallback");

        Assert.That(value, Is.EqualTo("value"));
    }
}