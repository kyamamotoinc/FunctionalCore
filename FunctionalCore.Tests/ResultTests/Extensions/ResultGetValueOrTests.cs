using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultGetValueOrTests
{
    /// <summary>
    /// 1. ResultがOkの場合は成功値を返す。
    /// </summary>
    [Test]
    public void Ok_GetValueOr_should_return_value()
    {
        var ok = Result<string, int>.Ok(5);
        var value = ok.GetValueOr(10);

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. ResultがFailの場合は指定された代替値を返す。
    /// </summary>
    [Test]
    public void Fail_GetValueOr_should_return_defaultValue()
    {
        var fail = Result<string, int>.Fail("error");
        var value = fail.GetValueOr(10);

        Assert.That(value, Is.EqualTo(10));
    }

    /// <summary>
    /// 3. ResultがOkで成功値が参照型の場合も、その成功値を返す。
    /// </summary>
    [Test]
    public void Ok_GetValueOr_should_return_value_for_reference_type()
    {
        var ok = Result<string, string>.Ok("value");
        var value = ok.GetValueOr("default");

        Assert.That(value, Is.EqualTo("value"));
    }

    /// <summary>
    /// 4. ResultがFailで代替値が参照型の場合は、その代替値を返す。
    /// </summary>
    [Test]
    public void Fail_GetValueOr_should_return_defaultValue_for_reference_type()
    {
        var fail = Result<string, string>.Fail("error");
        var value = fail.GetValueOr("default");

        Assert.That(value, Is.EqualTo("default"));
    }

    /// <summary>
    /// 5. ResultがOkの場合は代替値がnullでも成功値を返す。
    /// </summary>
    [Test]
    public void Ok_GetValueOr_should_return_value_when_defaultValue_is_null()
    {
        var ok = Result<string, string>.Ok("value");
        var value = ok.GetValueOr(null!);

        Assert.That(value, Is.EqualTo("value"));
    }

    /// <summary>
    /// 6. ResultがFailの場合は代替値がnullならnullを返す。
    /// </summary>
    [Test]
    public void Fail_GetValueOr_should_return_null_when_defaultValue_is_null()
    {
        var fail = Result<string, string>.Fail("error");
        var value = fail.GetValueOr(null!);

        Assert.That(value, Is.Null);
    }

    /// <summary>
    /// 7. Resultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_GetValueOr_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.GetValueOr(10));
    }

    /// <summary>
    /// 8. Resultがdefaultで代替値がnullの場合でも、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_GetValueOr_should_throw_invalid_operation_exception_when_defaultValue_is_null()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.GetValueOr(null!));
    }
}