using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultGetValueOrTests
{
    /// <summary>
    /// 1. Ok.GetValueOr は成功値を返す
    /// </summary>
    [Test]
    public void Result_Ok_GetValueOr_should_return_value()
    {
        var ok = Result<string, int>.Ok(5);
        var value = ok.GetValueOr(10);

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Fail.GetValueOr は指定された代替値を返す
    /// </summary>
    [Test]
    public void Result_Fail_GetValueOr_should_return_default_value()
    {
        var fail = Result<string, int>.Fail("error");
        var value = fail.GetValueOr(10);

        Assert.That(value, Is.EqualTo(10));
    }

    /// <summary>
    /// 3. Ok.GetValueOr は参照型でも成功値を返す
    /// </summary>
    [Test]
    public void Result_Ok_GetValueOr_reference_type_should_return_value()
    {
        var result = Result<string, string>.Ok("value");

        var value = result.GetValueOr("default");

        Assert.That(value, Is.EqualTo("value"));
    }

    /// <summary>
    /// 4. Fail.GetValueOr は参照型の代替値を返す
    /// </summary>
    [Test]
    public void Result_Fail_GetValueOr_reference_type_should_return_default_value()
    {
        var result = Result<string, string>.Fail("error");

        var value = result.GetValueOr("default");

        Assert.That(value, Is.EqualTo("default"));
    }

    /// <summary>
    /// 5. Ok.GetValueOr では代替値に null を指定しても成功値を返す
    /// </summary>
    [Test]
    public void Result_Ok_GetValueOr_null_default_value_should_return_value()
    {
        var result = Result<string, string>.Ok("value");

        var value = result.GetValueOr(null!);

        Assert.That(value, Is.EqualTo("value"));
    }

    /// <summary>
    /// 6. Fail.GetValueOr では代替値に null を指定した場合は null を返す
    /// </summary>
    [Test]
    public void Result_Fail_GetValueOr_null_default_value_should_return_null()
    {
        var result = Result<string, string>.Fail("error");

        var value = result.GetValueOr(null!);

        Assert.That(value, Is.Null);
    }

    /// <summary>
    /// 7. 未初期化 Result.GetValueOr は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_GetValueOr_should_throw()
    {
        var result = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => result.GetValueOr(10));
    }

    /// <summary>
    /// 8. 未初期化 Result は代替値が null でも InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_GetValueOr_null_default_value_should_throw()
    {
        var result = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => result.GetValueOr(null!));
    }
}