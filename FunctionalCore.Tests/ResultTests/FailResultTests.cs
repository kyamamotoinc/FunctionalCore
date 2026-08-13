namespace FunctionalCore.Tests.ResultTests;

public class FailResultTests
{
    private Result<string, int> _fail;

    [SetUp]
    public void Setup()
    {
        _fail = Result<string, int>.Fail("error");
    }

    /// <summary>
    /// 1. Fail は内部の Error をそのまま返す
    /// </summary>
    [Test]
    public void Result_Fail_should_return_inner_Error()
    {
        Assert.That(_fail.Error, Is.EqualTo("error"));
    }

    /// <summary>
    /// 2. Fail は失敗状態である
    /// </summary>
    [Test]
    public void Result_Fail_should_be_failure()
    {
        Assert.Multiple(() =>
        {
            Assert.That(_fail.IsSuccess, Is.False);
            Assert.That(_fail.IsFailure, Is.True);
        });
    }

    /// <summary>
    /// 3. Fail では Value にアクセスできない
    /// </summary>
    [Test]
    public void Result_Fail_accessing_Value_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = _fail.Value);
    }

    /// <summary>
    /// 4. Fail(null) は許されない
    /// </summary>
    [Test]
    public void Result_Fail_null_should_throw_exception()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string, int>.Fail(null!));
    }

    /// <summary>
    /// 5. Fail 同士で Error が同じなら等しい
    /// </summary>
    [Test]
    public void Fail_with_same_error_should_be_equal()
    {
        var other = Result<string, int>.Fail("error");

        Assert.Multiple(() =>
        {
            Assert.That(_fail == other, Is.True);
            Assert.That(_fail.Equals(other), Is.True);
            Assert.That(_fail.GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 6. Fail 同士で Error が異なれば等しくない
    /// </summary>
    [Test]
    public void Fail_with_different_error_should_not_be_equal()
    {
        var other = Result<string, int>.Fail("other");

        Assert.Multiple(() =>
        {
            Assert.That(_fail != other, Is.True);
            Assert.That(_fail.Equals(other), Is.False);
        });
    }

    /// <summary>
    /// 7. Fail と Ok は等しくない
    /// </summary>
    [Test]
    public void Fail_and_Ok_should_not_be_equal()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Multiple(() =>
        {
            Assert.That(_fail == ok, Is.False);
            Assert.That(_fail != ok, Is.True);
            Assert.That(_fail.Equals(ok), Is.False);
        });
    }

    /// <summary>
    /// 8. Fail の ToString は "Fail(error)" を返す
    /// </summary>
    [Test]
    public void Fail_ToString_should_return_formatted_value()
    {
        Assert.That(_fail.ToString(), Is.EqualTo("Fail(error)"));
    }
}