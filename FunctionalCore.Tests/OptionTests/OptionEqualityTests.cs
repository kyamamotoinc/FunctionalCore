namespace FunctionalCore.Tests.OptionTests;

public class OptionEqualityTests
{
    /// <summary>
    /// 1. Equals(object) に null を渡した場合は false を返す
    /// </summary>
    [Test]
    public void Option_Equals_null_should_return_false()
    {
        var option = Option<int>.Some(5);

        Assert.That(option.Equals(null), Is.False);
    }

    /// <summary>
    /// 2. Equals(object) に異なる型を渡した場合は false を返す
    /// </summary>
    [Test]
    public void Option_Equals_different_type_should_return_false()
    {
        var option = Option<int>.Some(5);

        Assert.That(option.Equals("not option"), Is.False);
    }

    /// <summary>
    /// 3. object として比較した同じ値の Some は等しい
    /// </summary>
    [Test]
    public void Option_Some_Equals_object_with_same_value_should_return_true()
    {
        var option = Option<int>.Some(5);
        object other = Option<int>.Some(5);

        Assert.That(option.Equals(other), Is.True);
    }

    /// <summary>
    /// 4. object として比較した異なる値の Some は等しくない
    /// </summary>
    [Test]
    public void Option_Some_Equals_object_with_different_value_should_return_false()
    {
        var option = Option<int>.Some(5);
        object other = Option<int>.Some(10);

        Assert.That(option.Equals(other), Is.False);
    }

    /// <summary>
    /// 5. object として比較した None 同士は等しい
    /// </summary>
    [Test]
    public void Option_None_Equals_object_none_should_return_true()
    {
        var option = Option<int>.None;
        object other = Option<int>.None;

        Assert.That(option.Equals(other), Is.True);
    }

    /// <summary>
    /// 6. object として比較した Default Option と None は等しい
    /// </summary>
    [Test]
    public void Option_Default_Equals_object_none_should_return_true()
    {
        var option = default(Option<int>);
        object other = Option<int>.None;

        Assert.That(option.Equals(other), Is.True);
    }

    /// <summary>
    /// 7. Some と None は object 経由でも等しくない
    /// </summary>
    [Test]
    public void Option_Some_Equals_object_none_should_return_false()
    {
        var option = Option<int>.Some(5);
        object other = Option<int>.None;

        Assert.That(option.Equals(other), Is.False);
    }

    /// <summary>
    /// 8. 等しい Some は同じハッシュコードを返す
    /// </summary>
    [Test]
    public void Equal_Some_options_should_have_same_hash_code()
    {
        var first = Option<int>.Some(5);
        var second = Option<int>.Some(5);

        Assert.That(first.GetHashCode(), Is.EqualTo(second.GetHashCode()));
    }

    /// <summary>
    /// 9. None と Default Option は同じハッシュコードを返す
    /// </summary>
    [Test]
    public void None_and_Default_option_should_have_same_hash_code()
    {
        var none = Option<int>.None;
        var option = default(Option<int>);

        Assert.That(none.GetHashCode(), Is.EqualTo(option.GetHashCode()));
    }
}