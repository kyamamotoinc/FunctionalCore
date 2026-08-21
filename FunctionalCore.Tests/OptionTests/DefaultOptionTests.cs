namespace FunctionalCore.Tests.OptionTests;

public class DefaultOptionTests
{
    /// <summary>
    /// 1. defaultのOptionは値を保持していない。
    /// </summary>
    [Test]
    public void Default_Option_should_not_have_value()
    {
        var defaultOption = default(Option<int>);

        Assert.That(defaultOption.HasValue, Is.False);
    }

    /// <summary>
    /// 2. defaultのOptionはNoneと等しい。
    /// </summary>
    [Test]
    public void Default_Option_should_equal_none()
    {
        var defaultOption = default(Option<int>);

        Assert.Multiple(() =>
        {
            Assert.That(defaultOption, Is.EqualTo(Option<int>.None));
            Assert.That(defaultOption == Option<int>.None, Is.True);
            Assert.That(defaultOption.Equals(Option<int>.None), Is.True);
        });
    }

    /// <summary>
    /// 3. defaultのOptionでValueにアクセスした場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Option_should_throw_invalid_operation_exception_when_accessing_value()
    {
        var defaultOption = default(Option<int>);

        Assert.Throws<InvalidOperationException>(() => _ = defaultOption.Value);
    }

    /// <summary>
    /// 4. defaultのOptionのToStringは"None"を返す。
    /// </summary>
    [Test]
    public void Default_Option_ToString_should_return_none()
    {
        var defaultOption = default(Option<int>);

        Assert.That(defaultOption.ToString(), Is.EqualTo("None"));
    }

    /// <summary>
    /// 5. 2つのdefault Optionは等しい。
    /// </summary>
    [Test]
    public void Default_Option_should_equal_another_default_option()
    {
        var defaultOption = default(Option<int>);
        var other = default(Option<int>);

        Assert.Multiple(() =>
        {
            Assert.That(defaultOption == other, Is.True);
            Assert.That(defaultOption.Equals(other), Is.True);
            Assert.That(defaultOption.GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 6. Option配列の初期値はNoneと等しい。
    /// </summary>
    [Test]
    public void Default_Option_in_array_should_equal_none()
    {
        var options = new Option<int>[1];

        Assert.Multiple(() =>
        {
            Assert.That(options[0].HasValue, Is.False);
            Assert.That(options[0], Is.EqualTo(Option<int>.None));
        });
    }
}