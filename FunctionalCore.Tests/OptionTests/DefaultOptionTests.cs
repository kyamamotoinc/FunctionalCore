namespace FunctionalCore.Tests.OptionTests;

public class DefaultOptionTests
{
    private Option<int> _default;

    [SetUp]
    public void Setup()
    {
        _default = default;
    }

    /// <summary>
    /// 1. Default Option は値を保持していない
    /// </summary>
    [Test]
    public void Default_Option_should_not_have_value()
    {
        Assert.That(_default.HasValue, Is.False);
    }

    /// <summary>
    /// 2. Default Option は None と等しい
    /// </summary>
    [Test]
    public void Default_Option_should_equal_none()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_default, Is.EqualTo(Option<int>.None));
            Assert.That(_default == Option<int>.None, Is.True);
            Assert.That(_default.Equals(Option<int>.None), Is.True);
        });
    }

    /// <summary>
    /// 3. Default Option では Value にアクセスできない
    /// </summary>
    [Test]
    public void Default_Option_accessing_value_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = _default.Value);
    }

    /// <summary>
    /// 4. Default Option の ToString は "None" を返す
    /// </summary>
    [Test]
    public void Default_Option_ToString_should_return_none()
    {
        Assert.That(_default.ToString(), Is.EqualTo("None"));
    }

    /// <summary>
    /// 5. Default Option 同士は等しい
    /// </summary>
    [Test]
    public void Two_default_Options_should_be_equal()
    {
        var other = default(Option<int>);

        Assert.Multiple(() =>
        {
            Assert.That(_default == other, Is.True);
            Assert.That(_default.Equals(other), Is.True);
            Assert.That(_default.GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 6. 配列で生成された Option の初期値は None と等しい
    /// </summary>
    [Test]
    public void Array_initialized_Option_should_be_none()
    {
        var options = new Option<int>[1];

        Assert.Multiple(() =>
        {
            Assert.That(options[0].HasValue, Is.False);
            Assert.That(options[0], Is.EqualTo(Option<int>.None));
        });
    }
}