namespace FunctionalCore.Tests.ResultTests;

public class FailResultTests
{
    /// <summary>
    /// 1. FailのErrorにアクセスした場合は保持しているエラーを返す。
    /// </summary>
    [Test]
    public void Fail_accessing_Error_should_return_inner_error()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.That(fail.Error, Is.EqualTo("error"));
    }

    /// <summary>
    /// 2. Failは失敗状態であり、成功状態ではない。
    /// </summary>
    [Test]
    public void Fail_should_be_failure_and_not_success()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Multiple(() =>
        {
            Assert.That(fail.IsFailure, Is.True);
            Assert.That(fail.IsSuccess, Is.False);
        });
    }

    /// <summary>
    /// 3. FailのValueにアクセスした場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_accessing_Value_should_throw_invalid_operation_exception()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Throws<InvalidOperationException>(() => _ = fail.Value);
    }

    /// <summary>
    /// 4. nullをエラーとしてFailを生成した場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_should_throw_argument_null_exception_when_error_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string, int>.Fail(null!));
    }

    /// <summary>
    /// 5. 2つのFailが同じErrorを保持している場合は等しい。
    /// </summary>
    [Test]
    public void Fail_with_same_error_should_be_equal()
    {
        var fail = Result<string, int>.Fail("error");
        var other = Result<string, int>.Fail("error");

        Assert.Multiple(() =>
        {
            Assert.That(fail == other, Is.True);
            Assert.That(fail.Equals(other), Is.True);
            Assert.That(fail.GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 6. 2つのFailが異なるErrorを保持している場合は等しくない。
    /// </summary>
    [Test]
    public void Fail_with_different_error_should_not_be_equal()
    {
        var fail = Result<string, int>.Fail("error");
        var other = Result<string, int>.Fail("other");

        Assert.Multiple(() =>
        {
            Assert.That(fail != other, Is.True);
            Assert.That(fail.Equals(other), Is.False);
        });
    }

    /// <summary>
    /// 7. FailとOkは等しくない。
    /// </summary>
    [Test]
    public void Fail_and_Ok_should_not_be_equal()
    {
        var fail = Result<string, int>.Fail("error");
        var ok = Result<string, int>.Ok(5);

        Assert.Multiple(() =>
        {
            Assert.That(fail == ok, Is.False);
            Assert.That(fail != ok, Is.True);
            Assert.That(fail.Equals(ok), Is.False);
        });
    }

    /// <summary>
    /// 8. FailのToStringは"Fail(error)"形式の文字列を返す。
    /// </summary>
    [Test]
    public void Fail_ToString_should_return_formatted_value()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.That(fail.ToString(), Is.EqualTo("Fail(error)"));
    }
}