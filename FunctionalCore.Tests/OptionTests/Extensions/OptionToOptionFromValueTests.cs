using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionToOptionFromValueTests
{
    /// <summary>
    /// 1. null ではない値を ToOption すると Value を持つ Some を返す
    /// </summary>
    [Test]
    public void Value_ToOption_non_null_should_return_some()
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
    /// 2. null の値を ToOption すると None を返す
    /// </summary>
    [Test]
    public void Value_ToOption_null_should_return_none()
    {
        string? value = null;

        var option = value.ToOption();

        Assert.That(option, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 3. 値型の default 値は正常な Some として扱われる
    /// </summary>
    [Test]
    public void Value_ToOption_default_value_type_should_return_some()
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
    /// 4. 参照型の値を ToOption した場合は同じインスタンスを保持する
    /// </summary>
    [Test]
    public void Value_ToOption_reference_type_should_keep_same_instance()
    {
        var value = new object();

        var option = value.ToOption();

        Assert.That(option.Value, Is.SameAs(value));
    }

    /// <summary>
    /// 5. null ではない空文字列は Some として扱われる
    /// </summary>
    [Test]
    public void Value_ToOption_empty_string_should_return_some()
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