namespace FunctionalCore.Tests.OptionTests;

public class NoneOptionTests
{
    /// <summary>
    /// 1. Noneは値を保持していない状態である。
    /// </summary>
    [Test]
    public void None_should_not_have_value()
    {
        var none = Option<int>.None;

        Assert.That(none.HasValue, Is.False);
    }

    /// <summary>
    /// 2. NoneでValueにアクセスした場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_should_throw_invalid_operation_exception_when_accessing_value()
    {
        var none = Option<int>.None;

        Assert.Throws<InvalidOperationException>(() => _ = none.Value);
    }

    /// <summary>
    /// 3. None同士は等しい。
    /// </summary>
    [Test]
    public void None_should_equal_another_none()
    {
        var none = Option<int>.None;
        var other = Option<int>.None;

        Assert.Multiple(() =>
        {
            Assert.That(none == other, Is.True);
            Assert.That(none.Equals(other), Is.True);
            Assert.That(none.GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 4. NoneとSomeは等しくない。
    /// </summary>
    [Test]
    public void None_and_Some_should_not_be_equal()
    {
        var none = Option<int>.None;
        var some = Option<int>.Some(5);

        Assert.Multiple(() =>
        {
            Assert.That(none == some, Is.False);
            Assert.That(none != some, Is.True);
            Assert.That(none.Equals(some), Is.False);
        });
    }

    /// <summary>
    /// 5. NoneのToStringは"None"を返す。
    /// </summary>
    [Test]
    public void None_ToString_should_return_none()
    {
        var none = Option<int>.None;

        Assert.That(none.ToString(), Is.EqualTo("None"));
    }
}