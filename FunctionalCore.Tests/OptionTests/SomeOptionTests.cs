namespace FunctionalCore.Tests.OptionTests;

public class SomeOptionTests
{
    private Option<int> _some;

    [SetUp]
    public void Setup()
    {
        _some = Option<int>.Some(5);
    }

    /// <summary>
    /// 1. Some は内部の Value をそのまま返す
    /// </summary>
    [Test]
    public void Option_Some_should_return_inner_value()
    {
        Assert.That(_some.Value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Some は値を保持している状態である
    /// </summary>
    [Test]
    public void Option_Some_should_have_value()
    {
        Assert.That(_some.HasValue, Is.True);
    }

    /// <summary>
    /// 3. Some(null) は許されない
    /// </summary>
    [Test]
    public void Option_Some_null_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => Option<string>.Some(null!));
    }

    /// <summary>
    /// 4. Some は default 値を正常な値として保持できる
    /// </summary>
    [Test]
    public void Option_Some_default_value_should_have_value()
    {
        var some = Option<int>.Some(default);

        Assert.Multiple(() =>
        {
            Assert.That(some.HasValue, Is.True);
            Assert.That(some.Value, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 5. 参照型の Some は渡されたインスタンスをそのまま保持する
    /// </summary>
    [Test]
    public void Option_Some_reference_type_should_keep_same_instance()
    {
        var value = new object();

        var some = Option<object>.Some(value);

        Assert.That(some.Value, Is.SameAs(value));
    }

    /// <summary>
    /// 6. Some 同士で Value が同じなら等しい
    /// </summary>
    [Test]
    public void Some_with_same_value_should_be_equal()
    {
        var other = Option<int>.Some(5);

        Assert.Multiple(() =>
        {
            Assert.That(_some == other, Is.True);
            Assert.That(_some.Equals(other), Is.True);
            Assert.That(_some.GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 7. Some 同士で Value が異なれば等しくない
    /// </summary>
    [Test]
    public void Some_with_different_value_should_not_be_equal()
    {
        var other = Option<int>.Some(10);

        Assert.Multiple(() =>
        {
            Assert.That(_some != other, Is.True);
            Assert.That(_some.Equals(other), Is.False);
        });
    }

    /// <summary>
    /// 8. Some と None は等しくない
    /// </summary>
    [Test]
    public void Some_and_None_should_not_be_equal()
    {
        var none = Option<int>.None;

        Assert.Multiple(() =>
        {
            Assert.That(_some == none, Is.False);
            Assert.That(_some != none, Is.True);
            Assert.That(_some.Equals(none), Is.False);
        });
    }

    /// <summary>
    /// 9. Some の ToString は "Some(value)" を返す
    /// </summary>
    [Test]
    public void Some_ToString_should_return_formatted_value()
    {
        Assert.That(_some.ToString(), Is.EqualTo("Some(5)"));
    }
}