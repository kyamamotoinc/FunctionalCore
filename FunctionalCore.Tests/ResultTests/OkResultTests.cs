namespace FunctionalCore.Tests.ResultTests;

public class OkResultTests
{
    /// <summary>
    /// 1. OkのValueにアクセスした場合は保持している成功値を返す。
    /// </summary>
    [Test]
    public void Ok_accessing_Value_should_return_inner_value()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.That(ok.Value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Okは成功状態であり、失敗状態ではない。
    /// </summary>
    [Test]
    public void Ok_should_be_success_and_not_failure()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Multiple(() =>
        {
            Assert.That(ok.IsSuccess, Is.True);
            Assert.That(ok.IsFailure, Is.False);
        });
    }

    /// <summary>
    /// 3. OkのErrorにアクセスした場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_accessing_Error_should_throw_invalid_operation_exception()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() => _ = ok.Error);
    }

    /// <summary>
    /// 4. nullを成功値としてOkを生成した場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_should_throw_argument_null_exception_when_value_is_null()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string, string>.Ok(null!));
    }

    /// <summary>
    /// 5. 2つのOkが同じValueを保持している場合は等しい。
    /// </summary>
    [Test]
    public void Ok_with_same_value_should_be_equal()
    {
        var ok = Result<string, int>.Ok(5);
        var other = Result<string, int>.Ok(5);

        Assert.Multiple(() =>
        {
            Assert.That(ok == other, Is.True);
            Assert.That(ok.Equals(other), Is.True);
            Assert.That(ok.GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 6. 2つのOkが異なるValueを保持している場合は等しくない。
    /// </summary>
    [Test]
    public void Ok_with_different_value_should_not_be_equal()
    {
        var ok = Result<string, int>.Ok(5);
        var other = Result<string, int>.Ok(10);

        Assert.Multiple(() =>
        {
            Assert.That(ok != other, Is.True);
            Assert.That(ok.Equals(other), Is.False);
        });
    }

    /// <summary>
    /// 7. OkとFailは等しくない。
    /// </summary>
    [Test]
    public void Ok_and_Fail_should_not_be_equal()
    {
        var ok = Result<string, int>.Ok(5);
        var fail = Result<string, int>.Fail("error");

        Assert.Multiple(() =>
        {
            Assert.That(ok == fail, Is.False);
            Assert.That(ok != fail, Is.True);
            Assert.That(ok.Equals(fail), Is.False);
        });
    }

    /// <summary>
    /// 8. OkのToStringは"Ok(value)"形式の文字列を返す。
    /// </summary>
    [Test]
    public void Ok_ToString_should_return_formatted_value()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.That(ok.ToString(), Is.EqualTo("Ok(5)"));
    }
}