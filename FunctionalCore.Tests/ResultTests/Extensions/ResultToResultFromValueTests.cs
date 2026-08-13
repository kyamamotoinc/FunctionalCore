using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultToResultFromValueTests
{
    /// <summary>
    /// 1. null ではない値を ToResult すると Value を持つ Ok を返す
    /// </summary>
    [Test]
    public void Value_ToResult_non_null_should_return_ok()
    {
        var value = "value";

        var result = value.ToResult<string, string>("error");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo("value"));
        });
    }

    /// <summary>
    /// 2. null の値を ToResult すると指定した Error を持つ Fail を返す
    /// </summary>
    [Test]
    public void Value_ToResult_null_should_return_failure()
    {
        string? value = null;

        var result = value.ToResult<string, string>("error");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 3. 参照型の値を ToResult した場合は同じインスタンスを保持する
    /// </summary>
    [Test]
    public void Value_ToResult_reference_type_should_keep_same_instance()
    {
        var value = new object();

        var result = value.ToResult<string, object>("error");

        Assert.That(result.Value, Is.SameAs(value));
    }

    /// <summary>
    /// 4. 値が null で errorIfNull も null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Value_ToResult_null_value_and_null_error_should_throw()
    {
        string? value = null;

        Assert.Throws<ArgumentNullException>(() =>
            value.ToResult<string, string>(null!));
    }

    /// <summary>
    /// 5. 値が null でなくても errorIfNull が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Value_ToResult_non_null_value_and_null_error_should_throw()
    {
        var value = "value";

        Assert.Throws<ArgumentNullException>(() =>
            value.ToResult<string, string>(null!));
    }
}