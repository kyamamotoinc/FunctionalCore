using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionToOptionFromValueTests
{
    /// <summary>
    /// 1. 値がnullでない場合は、そのValueを保持するSomeを返す。
    /// </summary>
    [Test]
    public void Value_ToOption_should_return_some_when_value_is_not_null()
    {
        var value = "value";

        var option = value.ToOption();

        Assert.Multiple(() =>
        {
            Assert.That(option.HasValue, Is.True);
            Assert.That(option.Value, Is.EqualTo("value"));
        });
    }

    /// <summary>
    /// 2. 値がnullの場合はNoneを返す。
    /// </summary>
    [Test]
    public void Value_ToOption_should_return_none_when_value_is_null()
    {
        string? value = null;

        var option = value.ToOption();

        Assert.That(option, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 3. 値型のdefault値は正常な値として扱い、そのValueを保持するSomeを返す。
    /// </summary>
    [Test]
    public void Value_ToOption_should_return_some_for_default_value_type()
    {
        int value = default;

        var option = value.ToOption();

        Assert.Multiple(() =>
        {
            Assert.That(option.HasValue, Is.True);
            Assert.That(option.Value, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 4. 参照型の値がnullでない場合は、同じインスタンスを保持するSomeを返す。
    /// </summary>
    [Test]
    public void Value_ToOption_should_keep_same_instance_for_reference_type()
    {
        var value = new object();

        var option = value.ToOption();

        Assert.That(option.Value, Is.SameAs(value));
    }

    /// <summary>
    /// 5. 空文字列はnullではないため、正常な値としてSomeに保持する。
    /// </summary>
    [Test]
    public void Value_ToOption_should_return_some_for_empty_string()
    {
        var value = string.Empty;

        var option = value.ToOption();

        Assert.Multiple(() =>
        {
            Assert.That(option.HasValue, Is.True);
            Assert.That(option.Value, Is.EqualTo(string.Empty));
        });
    }
}