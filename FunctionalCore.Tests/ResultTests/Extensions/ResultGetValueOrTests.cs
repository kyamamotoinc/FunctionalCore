using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

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
    /// 1. Ok.GetValueOr は内部の Value を返す
    /// </summary>
    [Test]
    public void Result_Ok_GetValueOr_should_return_inner_value()
    {
        var value = _ok.GetValueOr(999);

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Fail.GetValueOr は fallback を返す
    /// </summary>
    [Test]
    public void Result_Fail_GetValueOr_should_return_fallback()
    {
        var value = _fail.GetValueOr(999);

        Assert.That(value, Is.EqualTo(999));
    }

    /// <summary>
    /// 3. Fail.GetValueOr は default 値を fallback として返せる
    /// </summary>
    [Test]
    public void Result_Fail_GetValueOr_with_default_should_return_default()
    {
        var value = _fail.GetValueOr(default);

        Assert.That(value, Is.EqualTo(default(int)));
    }

    /// <summary>
    /// 4. Ok.GetValueOr は Value が default 値でも fallback を使用しない
    /// </summary>
    [Test]
    public void Result_Ok_GetValueOr_with_default_value_should_ignore_fallback()
    {
        var ok = Result<string, int>.Ok(0);

        var value = ok.GetValueOr(999);

        Assert.That(value, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. 参照型の Fail.GetValueOr は fallback の同一インスタンスを返す
    /// </summary>
    [Test]
    public void Result_Fail_GetValueOr_reference_type_should_return_same_instance()
    {
        var fallback = new object();
        var fail = Result<string, object>.Fail("error");

        var value = fail.GetValueOr(fallback);

        Assert.That(value, Is.SameAs(fallback));
    }

    /// <summary>
    /// 6. 参照型の Ok.GetValueOr は fallback を使用しない
    /// </summary>
    [Test]
    public void Result_Ok_GetValueOr_reference_type_should_ignore_fallback()
    {
        var original = new object();
        var fallback = new object();
        var ok = Result<string, object>.Ok(original);

        var value = ok.GetValueOr(fallback);

        Assert.That(value, Is.SameAs(original));
    }

    /// <summary>
    /// 7. 参照型で fallback が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_GetValueOr_null_fallback_should_throw()
    {
        var result = Result<string, string>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => result.GetValueOr(null!));
    }

    /// <summary>
    /// 8. Ok でも fallback が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_GetValueOr_null_fallback_should_throw()
    {
        var result = Result<string, string>.Ok("value");

        Assert.Throws<ArgumentNullException>(() => result.GetValueOr(null!));
    }

    /// <summary>
    /// 9. 未初期化 Result で GetValueOr を呼び出すと InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_GetValueOr_should_throw()
    {
        var result = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => result.GetValueOr(999));
    }
}