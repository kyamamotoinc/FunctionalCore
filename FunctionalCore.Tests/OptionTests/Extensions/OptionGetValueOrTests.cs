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
    /// 1. Some.GetValueOr は保持している値を返す
    /// </summary>
    [Test]
    public void Option_Some_GetValueOr_should_return_value()
    {
        var value = _some.GetValueOr(10);

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. None.GetValueOr は指定された代替値を返す
    /// </summary>
    [Test]
    public void Option_None_GetValueOr_should_return_default_value()
    {
        var value = _none.GetValueOr(10);

        Assert.That(value, Is.EqualTo(10));
    }

    /// <summary>
    /// 3. Some.GetValueOr は参照型でも保持している値を返す
    /// </summary>
    [Test]
    public void Option_Some_GetValueOr_reference_type_should_return_value()
    {
        var option = Option<string>.Some("value");

        var value = option.GetValueOr("default");

        Assert.That(value, Is.EqualTo("value"));
    }

    /// <summary>
    /// 4. None.GetValueOr は参照型の代替値を返す
    /// </summary>
    [Test]
    public void Option_None_GetValueOr_reference_type_should_return_default_value()
    {
        var option = Option<string>.None;

        var value = option.GetValueOr("default");

        Assert.That(value, Is.EqualTo("default"));
    }

    /// <summary>
    /// 5. Some.GetValueOr では代替値に null を指定しても保持している値を返す
    /// </summary>
    [Test]
    public void Option_Some_GetValueOr_null_default_value_should_return_value()
    {
        var option = Option<string>.Some("value");

        var value = option.GetValueOr(null!);

        Assert.That(value, Is.EqualTo("value"));
    }

    /// <summary>
    /// 6. None.GetValueOr では代替値に null を指定した場合は null を返す
    /// </summary>
    [Test]
    public void Option_None_GetValueOr_null_default_value_should_return_null()
    {
        var option = Option<string>.None;

        var value = option.GetValueOr(null!);

        Assert.That(value, Is.Null);
    }

    /// <summary>
    /// 7. Default Option.GetValueOr は None と同様に代替値を返す
    /// </summary>
    [Test]
    public void Option_Default_GetValueOr_should_return_default_value()
    {
        var option = default(Option<int>);

        var value = option.GetValueOr(10);

        Assert.That(value, Is.EqualTo(10));
    }

    /// <summary>
    /// 8. Default Option.GetValueOr では代替値に null を指定した場合は null を返す
    /// </summary>
    [Test]
    public void Option_Default_GetValueOr_null_default_value_should_return_null()
    {
        var option = default(Option<string>);

        var value = option.GetValueOr(null!);

        Assert.That(value, Is.Null);
    }
}