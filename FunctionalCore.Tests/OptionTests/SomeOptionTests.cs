namespace FunctionalCore.Tests.OptionTests;

public class SomeOptionTests
{
    /// <summary>
    /// 1. Someは内部のValueをそのまま返す。
    /// </summary>
    [Test]
    public void Some_should_return_inner_value()
    {
        var some = Option<int>.Some(5);

        Assert.That(some.Value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Someは値を保持している状態である。
    /// </summary>
    [Test]
    public void Some_should_have_value()
    {
        var some = Option<int>.Some(5);

        Assert.That(some.HasValue, Is.True);
    }

    /// <summary>
    /// 3. Someにnullを渡した場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_should_throw_argument_null_exception_when_value_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => Option<string>.Some(null!));
    }

    /// <summary>
    /// 4. Someは値型のdefault値を正常な値として保持できる。
    /// </summary>
    [Test]
    public void Some_should_allow_default_value_for_value_type()
    {
        var some = Option<int>.Some(default);

        Assert.Multiple(() =>
        {
            Assert.That(some.HasValue, Is.True);
            Assert.That(some.Value, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 5. 参照型のSomeは渡されたインスタンスをそのまま保持する。
    /// </summary>
    [Test]
    public void Some_should_keep_same_instance_for_reference_type()
    {
        var value = new object();

        var some = Option<object>.Some(value);

        Assert.That(some.Value, Is.SameAs(value));
    }

    /// <summary>
    /// 6. Some同士でValueが同じ場合は等しい。
    /// </summary>
    [Test]
    public void Some_with_same_value_should_be_equal()
    {
        var some = Option<int>.Some(5);
        var other = Option<int>.Some(5);

        Assert.Multiple(() =>
        {
            Assert.That(some == other, Is.True);
            Assert.That(some.Equals(other), Is.True);
            Assert.That(some.GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 7. Some同士でValueが異なる場合は等しくない。
    /// </summary>
    [Test]
    public void Some_with_different_value_should_not_be_equal()
    {
        var some = Option<int>.Some(5);
        var other = Option<int>.Some(10);

        Assert.Multiple(() =>
        {
            Assert.That(some != other, Is.True);
            Assert.That(some.Equals(other), Is.False);
        });
    }

    /// <summary>
    /// 8. SomeとNoneは等しくない。
    /// </summary>
    [Test]
    public void Some_and_None_should_not_be_equal()
    {
        var some = Option<int>.Some(5);
        var none = Option<int>.None;

        Assert.Multiple(() =>
        {
            Assert.That(some == none, Is.False);
            Assert.That(some != none, Is.True);
            Assert.That(some.Equals(none), Is.False);
        });
    }

    /// <summary>
    /// 9. SomeのToStringは"Some(value)"を返す。
    /// </summary>
    [Test]
    public void Some_ToString_should_return_formatted_value()
    {
        var some = Option<int>.Some(5);

        Assert.That(some.ToString(), Is.EqualTo("Some(5)"));
    }
}