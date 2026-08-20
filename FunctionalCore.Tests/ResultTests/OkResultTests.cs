namespace FunctionalCore.Tests.ResultTests;

public class OkResultTests
{
    //private Result<string, int> _ok;

    //[SetUp]
    //public void Setup()
    //{
    //    _ok = Result<string, int>.Ok(5);
    //}

    /// <summary>
    /// 1. Ok は内部の Value をそのまま返す
    /// </summary>
    [Test]
    public void Result_Ok_should_return_inner_Value()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.That(ok.Value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Ok は成功状態である
    /// </summary>
    [Test]
    public void Result_Ok_should_be_success()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Multiple(() =>
        {
            Assert.That(ok.IsSuccess, Is.True);
            Assert.That(ok.IsFailure, Is.False);
        });
    }

    /// <summary>
    /// 3. Ok では Error にアクセスできない
    /// </summary>
    [Test]
    public void Result_Ok_accessing_Error_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<InvalidOperationException>(() => _ = ok.Error);
    }

    /// <summary>
    /// 4. Ok(null) は許されない
    /// </summary>
    [Test]
    public void Result_Ok_null_should_throw_exception()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string, string>.Ok(null!));
    }

    /// <summary>
    /// 5. Ok 同士で Value が同じなら等しい
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
    /// 6. Ok 同士で Value が異なれば等しくない
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
    /// 7. Ok と Fail は等しくない
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
    /// 8. Ok の ToString は "Ok(value)" を返す
    /// </summary>
    [Test]
    public void Ok_ToString_should_return_formatted_value()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.That(ok.ToString(), Is.EqualTo("Ok(5)"));
    }
}