namespace FunctionalCore.Tests.OptionTests;

public class ResultGetValueOrDefaultTests
{
    private Result<string, int> _ok;
    private Result<string, int> _fail;

    [SetUp]
    public void Setup()
    {
        _ok = Result<string, int>.Ok(5);
        _fail = Result<string, int>.Fail("error");
    }

    /// <summary>
    /// 1. Ok の場合は内部の値をそのまま返す（Ok → value）
    /// </summary>
    [Test]
    public void Ok_GetValueOrDefault_should_return_inner_value()
    {
        var value = _ok.GetValueOrDefault();

        Assert.AreEqual(5, value);
    }

    /// <summary>
    /// 2. Fail の場合は default(T) を返す（Fail → default）
    /// </summary>
    [Test]
    public void Fail_GetValueOrDefault_should_return_default()
    {
        var value = _fail.GetValueOrDefault();

        Assert.AreEqual(default(int), value);
    }

    /// <summary>
    /// 3. Fail の場合は型ごとの default が返る（参照型）
    /// </summary>
    [Test]
    public void Fail_GetValueOrDefault_reference_type_should_return_null()
    {
        var none = Option<string>.None;

        var value = none.GetValueOrDefault();

        Assert.IsNull(value);
    }

    /// <summary>
    /// 4. Ok の場合は default 値と同じでも区別される（値の存在が優先）
    /// </summary>
    [Test]
    public void Ok_with_default_value_should_return_value_not_none()
    {
        var some = Option<int>.Some(0);

        var value = some.GetValueOrDefault();

        Assert.AreEqual(0, value);
    }
}