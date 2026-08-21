namespace FunctionalCore.Tests.OptionTests;

public class DefaultOptionTests
{
    /// <summary>
    /// 1. Default Option は値を保持していない
    /// </summary>
    [Test]
    public void Default_Option_should_not_have_value()
    {
        var def = default(Option<int>);
        Assert.That(def.HasValue, Is.False);
    }

    /// <summary>
    /// 2. Default Option は None と等しい
    /// </summary>
    [Test]
    public void Default_Option_should_equal_none()
    {
        var def = default(Option<int>);
        Assert.Multiple(() =>
        {
            Assert.That(def, Is.EqualTo(Option<int>.None));
            Assert.That(def == Option<int>.None, Is.True);
            Assert.That(def.Equals(Option<int>.None), Is.True);
        });
    }

    /// <summary>
    /// 3. Default Option では Value にアクセスできない
    /// </summary>
    [Test]
    public void Default_Option_accessing_value_should_throw()
    {
        var def = default(Option<int>);
        Assert.Throws<InvalidOperationException>(() => _ = def.Value);
    }

    /// <summary>
    /// 4. Default Option の ToString は "None" を返す
    /// </summary>
    [Test]
    public void Default_Option_ToString_should_return_none()
    {
        var def = default(Option<int>);
        Assert.That(def.ToString(), Is.EqualTo("None"));
    }

    /// <summary>
    /// 5. Default Option 同士は等しい
    /// </summary>
    [Test]
    public void Two_default_Options_should_be_equal()
    {
        var def = default(Option<int>);
        var other = default(Option<int>);

        Assert.Multiple(() =>
        {
            Assert.That(def == other, Is.True);
            Assert.That(def.Equals(other), Is.True);
            Assert.That(def.GetHashCode(), Is.EqualTo(other.GetHashCode()));
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