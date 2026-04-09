namespace FunctionalCore.Tests.OptionTests;

public class OptionSomeTests
{
    private Option<int> _some;

    [SetUp]
    public void Setup()
    {
        _some = Option<int>.Some(5);
    }

    /// <summary>
    /// 1. Some は内部の値をそのまま返す（値の存在が保証されている）
    /// </summary>
    [Test]
    public void Some_Value_should_return_inner_value()
    {
        Assert.AreEqual(5, _some.Value);
    }

    /// <summary>
    /// 2. Some は常に値を持つ（HasValue = true）
    /// </summary>
    [Test]
    public void Some_HasValue_should_be_true()
    {
        Assert.IsTrue(_some.HasValue);
    }

    /// <summary>
    /// 3. Some の Value は null にならない（null禁止の契約）
    /// </summary>
    [Test]
    public void Some_Value_should_not_be_null()
    {
        Assert.IsNotNull(_some.Value);
    }

    /// <summary>
    /// 4. Some(null) は ArgumentNullException を投げる（null禁止）
    /// </summary>
    [Test]
    public void Some_null_should_throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Option<string>.Some(null));
    }

    /// <summary>
    /// 5. Some 同士で値が同じなら等しい
    /// </summary>
    [Test]
    public void Some_with_same_value_should_be_equal()
    {
        var other = Option<int>.Some(5);
        Assert.IsTrue(_some == other);
    }

    /// <summary>
    /// 6. Some 同士で値が違えば等しくない
    /// </summary>
    [Test]
    public void Some_with_different_value_should_not_be_equal()
    {
        var other = Option<int>.Some(10);
        Assert.IsTrue(_some != other);
    }

    /// <summary>
    /// 7. Some の ToString は "Some(value)" を返す
    /// </summary>
    [Test]
    public void Some_ToString_should_return_formatted_value()
    {
        Assert.AreEqual("Some(5)", _some.ToString());
    }
}