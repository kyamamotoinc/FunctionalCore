namespace FunctionalCore.Tests.OptionTests;

public class OptionNoneTests
{
    private Option<int> _none;

    [SetUp]
    public void Setup()
    {
        _none = Option<int>.None;
    }

    /// <summary>
    /// 1. None は値を持たない（HasValue = false）
    /// </summary>
    [Test]
    public void Option_None_does_not_have_value()
    {
        Assert.IsFalse(_none.HasValue);
    }

    /// <summary>
    /// 2. None の Value にアクセスすると例外
    /// </summary>
    [Test]
    public void Option_None_accessing_Value_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = _none.Value);
    }

    /// <summary>
    /// 3. None 同士は常に等しい（==）
    /// </summary>
    [Test]
    public void None_should_be_equal_to_None()
    {
        var other = Option<int>.None;
        Assert.That(_none, Is.EqualTo(other));
    }

    /// <summary>
    /// 4. None 同士は Equals でも等しい
    /// </summary>
    [Test]
    public void None_should_be_equal_via_Equals()
    {
        var other = Option<int>.None;
        Assert.That(_none, Is.EqualTo(other));
    }

    /// <summary>
    /// 5. Some と None は等しくない
    /// </summary>
    [Test]
    public void Some_and_None_should_not_be_equal()
    {
        var some = Option<int>.Some(5);
        Assert.That(_none, Is.Not.EqualTo(some));
        //Assert.That(some != _none);
    }

    /// <summary>
    /// 6. None の ToString は "None"
    /// </summary>
    [Test]
    public void None_ToString_should_return_None()
    {
        Assert.That(_none.ToString(), Is.EqualTo("None"));
    }

    /// <summary>
    /// 7. None のハッシュコードは等しい
    /// </summary>
    [Test]
    public void None_hashcode_should_be_equal()
    {
        var other = Option<int>.None;
        Assert.That(_none.GetHashCode(), Is.EqualTo(other.GetHashCode()));
    }
}