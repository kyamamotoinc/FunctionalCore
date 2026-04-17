namespace FunctionalCore.Tests.OptionTests;

public class ResultGetValueOrTests
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
    /// 1. Ok の場合は fallback を無視して内部の値を返す（Ok → value）
    /// </summary>
    [Test]
    public void Ok_GetValueOr_should_return_inner_value()
    {
        var value = _ok.GetValueOr(999);

        Assert.AreEqual(5, value);
    }

    /// <summary>
    /// 2. Fail の場合は fallback を返す（Fail → fallback）
    /// </summary>
    [Test]
    public void Fail_GetValueOr_should_return_fallback()
    {
        var value = _fail.GetValueOr(999);

        Assert.AreEqual(999, value);
    }

    /// <summary>
    /// 3. fallback が default 値でも正しく返される（Fail → default）
    /// </summary>
    [Test]
    public void Fail_GetValueOr_with_default_should_return_default()
    {
        var value = _fail.GetValueOr(default);

        Assert.AreEqual(default(int), value);
    }

    /// <summary>
    /// 4. Ok の値が default と同じでも fallback は使われない（Ok優先）
    /// </summary>
    [Test]
    public void Ok_with_default_value_should_ignore_fallback()
    {
        var some = Result<string, int>.Ok(0);

        var value = some.GetValueOr(999);

        Assert.AreEqual(0, value);
    }

    /// <summary>
    /// 5. 参照型: Fail の場合は fallback の参照をそのまま返す
    /// </summary>
    [Test]
    public void Fail_GetValueOr_reference_type_should_return_same_instance()
    {
        var fallback = "fallback";
        var none = Result<string, string>.Fail("error");

        var value = none.GetValueOr(fallback);

        Assert.AreSame(fallback, value);
    }

    /// <summary>
    /// 6. 参照型: Some の場合は fallback を無視する
    /// </summary>
    [Test]
    public void Ok_GetValueOr_reference_type_should_ignore_fallback()
    {
        var some = Result<string, string>.Ok("value");

        var value = some.GetValueOr("fallback");

        Assert.AreEqual("value", value);
    }
}