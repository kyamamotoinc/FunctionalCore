namespace FunctionalCore.Tests.OptionTests;

public class OptionEqualityTests
{
    /// <summary>
    /// 1. Equals(object)にnullを渡した場合はfalseを返す。
    /// </summary>
    [Test]
    public void Equals_object_should_return_false_when_other_is_null()
    {
        var some = Option<int>.Some(5);

        Assert.That(some.Equals(null), Is.False);
    }

    /// <summary>
    /// 2. Equals(object)に異なる型を渡した場合はfalseを返す。
    /// </summary>
    [Test]
    public void Equals_object_should_return_false_when_other_is_different_type()
    {
        var some = Option<int>.Some(5);

        Assert.That(some.Equals("not option"), Is.False);
    }

    /// <summary>
    /// 3. objectとして比較した同じ値のSomeは等しい。
    /// </summary>
    [Test]
    public void Some_Equals_object_should_return_true_when_value_is_same()
    {
        var some = Option<int>.Some(5);
        object other = Option<int>.Some(5);

        Assert.That(some.Equals(other), Is.True);
    }

    /// <summary>
    /// 4. objectとして比較した異なる値のSomeは等しくない。
    /// </summary>
    [Test]
    public void Some_Equals_object_should_return_false_when_value_is_different()
    {
        var some = Option<int>.Some(5);
        object other = Option<int>.Some(10);

        Assert.That(some.Equals(other), Is.False);
    }

    /// <summary>
    /// 5. objectとして比較したNone同士は等しい。
    /// </summary>
    [Test]
    public void None_Equals_object_should_return_true_when_other_is_none()
    {
        var none = Option<int>.None;
        object other = Option<int>.None;

        Assert.That(none.Equals(other), Is.True);
    }

    /// <summary>
    /// 6. objectとして比較したdefault OptionとNoneは等しい。
    /// </summary>
    [Test]
    public void Default_Equals_object_should_return_true_when_other_is_none()
    {
        var defaultOption = default(Option<int>);
        object other = Option<int>.None;

        Assert.That(defaultOption.Equals(other), Is.True);
    }

    /// <summary>
    /// 7. SomeとNoneはobjectとして比較しても等しくない。
    /// </summary>
    [Test]
    public void Some_Equals_object_should_return_false_when_other_is_none()
    {
        var some = Option<int>.Some(5);
        object other = Option<int>.None;

        Assert.That(some.Equals(other), Is.False);
    }

    /// <summary>
    /// 8. 同じ値を保持するSomeは同じハッシュコードを返す。
    /// </summary>
    [Test]
    public void Some_with_same_value_should_have_same_hash_code()
    {
        var first = Option<int>.Some(5);
        var second = Option<int>.Some(5);

        Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
    }

    /// <summary>
    /// 9. Noneとdefault Optionは同じハッシュコードを返す。
    /// </summary>
    [Test]
    public void None_and_Default_Option_should_have_same_hash_code()
    {
        var none = Option<int>.None;
        var defaultOption = default(Option<int>);

        Assert.That(none.GetHashCode(), Is.EqualTo(defaultOption.GetHashCode()));
    }
}