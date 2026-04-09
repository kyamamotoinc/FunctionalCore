namespace FunctionalCore.Tests.ResultTests;

public class OptionNoneTests
{
    private Option<int> none;

    [SetUp]
    public void Setup()
    {
        none = Option<int>.None;
    }

    /// <summary>
    /// 1. None は値を持たない（HasValue = false）
    /// </summary>
    [Test]
    public void Option_None_does_not_have_value()
    {
        Assert.IsFalse(none.HasValue);
    }

    /// <summary>
    /// 2. None の Value にアクセスすると例外
    /// </summary>
    [Test]
    public void Option_None_accessing_Value_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = none.Value);
    }

    /// <summary>
    /// 3. None 同士は常に等しい（==）
    /// </summary>
    [Test]
    public void None_should_be_equal_to_None()
    {
        var other = Option<int>.None;
        Assert.IsTrue(none == other);
    }

    /// <summary>
    /// 4. None 同士は Equals でも等しい
    /// </summary>
    [Test]
    public void None_should_be_equal_via_Equals()
    {
        var other = Option<int>.None;
        Assert.IsTrue(none.Equals(other));
    }

    /// <summary>
    /// 5. Some と None は等しくない
    /// </summary>
    [Test]
    public void Some_and_None_should_not_be_equal()
    {
        var some = Option<int>.Some(5);
        Assert.AreNotEqual(some, none);
        Assert.IsTrue(some != none);
    }

    /// <summary>
    /// 6. None の ToString は "None"
    /// </summary>
    [Test]
    public void None_ToString_should_return_None()
    {
        Assert.AreEqual("None", none.ToString());
    }

    /// <summary>
    /// 7. None のハッシュコードは等しい
    /// </summary>
    [Test]
    public void None_hashcode_should_be_equal()
    {
        var other = Option<int>.None;
        Assert.AreEqual(none.GetHashCode(), other.GetHashCode());
    }
}