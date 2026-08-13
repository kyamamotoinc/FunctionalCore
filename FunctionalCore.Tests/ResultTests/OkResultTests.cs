namespace FunctionalCore.Tests.ResultTests;

public class OkResultTests
{
    private Result<string, int> _ok;

    [SetUp]
    public void Setup()
    {
        _ok = Result<string, int>.Ok(5);
    }

    /// <summary>
    /// 1. Ok は内部の Value をそのまま返す
    /// </summary>
    [Test]
    public void Result_Ok_should_return_inner_Value()
    {
        Assert.That(_ok.Value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Ok は成功状態である
    /// </summary>
    [Test]
    public void Result_Ok_should_be_success()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_ok.IsSuccess, Is.True);
            Assert.That(_ok.IsFailure, Is.False);
        });
    }

    /// <summary>
    /// 3. Ok では Error にアクセスできない
    /// </summary>
    [Test]
    public void Result_Ok_accessing_Error_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = _ok.Error);
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
        var other = Result<string, int>.Ok(5);

        Assert.Multiple(() =>
        {
            Assert.That(_ok == other, Is.True);
            Assert.That(_ok.Equals(other), Is.True);
            Assert.That(_ok.GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 6. Ok 同士で Value が異なれば等しくない
    /// </summary>
    [Test]
    public void Ok_with_different_value_should_not_be_equal()
    {
        var other = Result<string, int>.Ok(10);

        Assert.Multiple(() =>
        {
            Assert.That(_ok != other, Is.True);
            Assert.That(_ok.Equals(other), Is.False);
        });
    }

    /// <summary>
    /// 7. Ok と Fail は等しくない
    /// </summary>
    [Test]
    public void Ok_and_Fail_should_not_be_equal()
    {
        var fail = Result<string, int>.Fail("error");

        Assert.Multiple(() =>
        {
            Assert.That(_ok == fail, Is.False);
            Assert.That(_ok != fail, Is.True);
            Assert.That(_ok.Equals(fail), Is.False);
        });
    }

    /// <summary>
    /// 8. Ok の ToString は "Ok(value)" を返す
    /// </summary>
    [Test]
    public void Ok_ToString_should_return_formatted_value()
    {
        Assert.That(_ok.ToString(), Is.EqualTo("Ok(5)"));
    }
}