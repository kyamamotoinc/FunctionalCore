using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionToOptionFromResultTests
{
    /// <summary>
    /// 1. Ok.ToOption は Value を持つ Some を返す
    /// </summary>
    [Test]
    public void Result_Ok_ToOption_should_return_some()
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
    /// 2. Fail.ToOption は None を返す
    /// </summary>
    [Test]
    public void Result_Fail_ToOption_should_return_none()
    {
        var result = Result<string, int>.Fail("error");

        var option = result.ToOption();

        Assert.That(option, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 3. ToOption は Ok の Value を変更しない
    /// </summary>
    [Test]
    public void Result_Ok_ToOption_should_keep_original_value()
    {
        var result = Result<string, int>.Ok(5);

        var option = result.ToOption();

        Assert.That(option.Value, Is.EqualTo(result.Value));
    }

    /// <summary>
    /// 4. 参照型の Ok.ToOption は同じインスタンスを保持する
    /// </summary>
    [Test]
    public void Result_Ok_ToOption_reference_type_should_keep_same_instance()
    {
        var value = new object();
        var result = Result<string, object>.Ok(value);

        var option = result.ToOption();

        Assert.That(option.Value, Is.SameAs(value));
    }

    /// <summary>
    /// 5. 未初期化 Result を ToOption すると InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_ToOption_should_throw()
    {
        var result = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => result.ToOption());
    }
}