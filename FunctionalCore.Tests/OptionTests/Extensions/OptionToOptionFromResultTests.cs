using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionToOptionFromResultTests
{
    /// <summary>
    /// 1. ResultがOkの場合はValueを保持するSomeを返す。
    /// </summary>
    [Test]
    public void Ok_ToOption_should_return_some()
    {
        var result = Result<string, int>.Ok(5);

        var option = result.ToOption();

        Assert.Multiple(() =>
        {
            Assert.That(option.HasValue, Is.True);
            Assert.That(option.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 2. ResultがFailの場合はNoneを返す。
    /// </summary>
    [Test]
    public void Fail_ToOption_should_return_none()
    {
        var result = Result<string, int>.Fail("error");

        var option = result.ToOption();

        Assert.That(option, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 3. ResultがOkの場合は元のValueをそのまま保持する。
    /// </summary>
    [Test]
    public void Ok_ToOption_should_keep_original_value()
    {
        var result = Result<string, int>.Ok(5);

        var option = result.ToOption();

        Assert.That(option.Value, Is.EqualTo(result.Value));
    }

    /// <summary>
    /// 4. 参照型のResultがOkの場合は同じインスタンスを保持するSomeを返す。
    /// </summary>
    [Test]
    public void Ok_ToOption_should_keep_same_instance_for_reference_type()
    {
        var value = new object();
        var result = Result<string, object>.Ok(value);

        var option = result.ToOption();

        Assert.That(option.Value, Is.SameAs(value));
    }

    /// <summary>
    /// 5. Resultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_ToOption_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.ToOption());
    }
}