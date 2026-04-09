namespace FunctionalCore.Tests.OptionTests;

public class OptionGetValueOrDefaultTests
{
    private Option<int> _some;
    private Option<int> _none;

    [SetUp]
    public void Setup()
    {
        _some = Option<int>.Some(5);
        _none = Option<int>.None;
    }

    /// <summary>
    /// 1. Some の場合は内部の値をそのまま返す（Some → value）
    /// </summary>
    [Test]
    public void Some_GetValueOrDefault_should_return_inner_value()
    {
        var value = _some.GetValueOrDefault();

        Assert.AreEqual(5, value);
    }

    /// <summary>
    /// 2. None の場合は default(T) を返す（None → default）
    /// </summary>
    [Test]
    public void None_GetValueOrDefault_should_return_default()
    {
        var value = _none.GetValueOrDefault();

        Assert.AreEqual(default(int), value);
    }

    /// <summary>
    /// 3. None の場合は型ごとの default が返る（参照型）
    /// </summary>
    [Test]
    public void None_GetValueOrDefault_reference_type_should_return_null()
    {
        var none = Option<string>.None;

        var value = none.GetValueOrDefault();

        Assert.IsNull(value);
    }

    /// <summary>
    /// 4. Some の場合は default 値と同じでも区別される（値の存在が優先）
    /// </summary>
    [Test]
    public void Some_with_default_value_should_return_value_not_none()
    {
        var some = Option<int>.Some(0);

        var value = some.GetValueOrDefault();

        Assert.AreEqual(0, value);
    }
}