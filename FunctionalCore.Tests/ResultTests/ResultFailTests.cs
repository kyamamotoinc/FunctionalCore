namespace FunctionalCore.Tests.ResultTests;

public class ResultFailTests
{
    private Result<string, int> _fail;

    [SetUp]
    public void Setup()
    {
        _fail = Result<string, int>.Fail("error");
    }

    /// <summary>
    /// 1. Fail は内部の Error をそのまま返す（失敗レールの基本動作）
    /// </summary>
    [Test]
    public void Result_Fail_should_return_inner_Error()
    {
        Assert.That(_fail.Error, Is.EqualTo("error"));
    }

    /// <summary>
    /// 2. Fail は常に失敗状態である（IsSuccess = false）
    /// </summary>
    [Test]
    public void Result_Fail_should_not_be_success()
    {
        Assert.That(_fail.IsSuccess, Is.False);
    }

    /// <summary>
    /// 3. Fail の Error は null にならない（null 禁止）
    /// </summary>
    [Test]
    public void Result_Fail_Error_should_not_be_null()
    {
        Assert.That(_fail.Error, Is.Not.Null);
    }

    /// <summary>
    /// 4. Fail では Value にアクセスできない（成功レールのみ Value を持つ）
    /// </summary>
    [Test]
    public void Result_Fail_accessing_Value_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = _fail.Value);
    }

    /// <summary>
    /// 5. Fail(null) は許されない（null は Result の外側）
    /// </summary>
    [Test]
    public void Result_Fail_null_should_throw_exception()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string, int>.Fail(null));
    }

    /// <summary>
    /// 6. Fail 同士で Error が同じなら等しい（== と Equals）
    /// </summary>
    [Test]
    public void Fail_with_same_error_should_be_equal()
    {
        var other = Result<string, int>.Fail("error");

        Assert.That(_fail, Is.EqualTo(other));
        Assert.That(_fail.GetHashCode(), Is.EqualTo(other.GetHashCode()));
    }

    /// <summary>
    /// 7. Fail 同士で Error が異なれば等しくない
    /// </summary>
    [Test]
    public void Fail_with_different_error_should_not_be_equal()
    {
        var other = Result<string, int>.Fail("other");

        Assert.That(_fail, Is.Not.EqualTo(other));
    }

    /// <summary>
    /// 8. Fail の ToString は "Failure(error)" を返す
    /// </summary>
    [Test]
    public void Fail_ToString_should_return_formatted_value()
    {
        Assert.That(_fail.ToString(), Is.EqualTo("Fail(error)"));
    }
}