using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultToResultFromValueTests
{
    /// <summary>
    /// 1. 値がnullでない場合は、その値を保持するOkを返す。
    /// </summary>
    [Test]
    public void ToResult_should_return_ok_when_value_is_not_null()
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
    /// 2. 値がnullの場合は、指定されたErrorを保持するFailを返す。
    /// </summary>
    [Test]
    public void ToResult_should_return_fail_when_value_is_null()
    {
        string? value = null;

        var result = value.ToResult<string, string>("error");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 3. 参照型の値がnullでない場合は、同じインスタンスを保持するOkを返す。
    /// </summary>
    [Test]
    public void ToResult_should_keep_same_instance_for_reference_type()
    {
        var value = new object();

        var result = value.ToResult<string, object>("error");

        Assert.That(result.Value, Is.SameAs(value));
    }

    /// <summary>
    /// 4. 値がnullでerrorIfNullもnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void ToResult_should_throw_argument_null_exception_when_value_and_errorIfNull_are_null()
    {
        string? value = null;

        Assert.Throws<ArgumentNullException>(() =>
            value.ToResult<string, string>(null!));
    }

    /// <summary>
    /// 5. 値がnullでない場合でもerrorIfNullがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void ToResult_should_throw_argument_null_exception_when_errorIfNull_is_null()
    {
        var value = "value";

        Assert.Throws<ArgumentNullException>(() =>
            value.ToResult<string, string>(null!));
    }
}