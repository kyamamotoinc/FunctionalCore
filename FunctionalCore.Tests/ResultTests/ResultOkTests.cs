namespace FunctionalCore.Tests.ResultTests;

public class ResultOkTests
{
    private Result<string, int> _ok;

    [SetUp]
    public void Setup()
    {
        _ok = Result<string, int>.Ok(5);
    }

    /// <summary>
    /// 1. Ok は内部の Value を返す（成功レールの基本動作）
    /// </summary>
    [Test]
    public void Result_Ok_should_return_inner_Value()
    {
        Assert.DoesNotThrow(() => _ = _ok.Value);
        Assert.AreEqual(5, _ok.Value);
    }

    /// <summary>
    /// 2. Ok は常に成功状態である（IsSuccess = true）
    /// </summary>
    [Test]
    public void Result_Ok_should_be_success()
    {
        Assert.IsTrue(_ok.IsSuccess);
    }

    /// <summary>
    /// 3. Ok の Value は null にならない（null 禁止）
    /// </summary>
    [Test]
    public void Result_Ok_Value_should_not_be_null()
    {
        Assert.IsNotNull(_ok.Value);
    }

    /// <summary>
    /// 4. Ok では Error にアクセスできない（失敗レールのみ Error を持つ）
    /// </summary>
    [Test]
    public void Result_Ok_accessing_Error_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = _ok.Error);
    }

    /// <summary>
    /// 5. Ok(null) は許されない（null は Result の外側）
    /// </summary>
    [Test]
    public void Result_Ok_null_should_throw_exception()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string, string>.Ok(null!));
    }

    /// <summary>
    /// 6. Ok 同士で Value が同じなら等しい（== と Equals）
    /// </summary>
    [Test]
    public void Ok_with_same_value_should_be_equal()
    {
        var other = Result<string, int>.Ok(5);

        Assert.IsTrue(_ok == other);
        Assert.IsTrue(_ok.Equals(other));
        Assert.AreEqual(_ok.GetHashCode(), other.GetHashCode());
    }

    /// <summary>
    /// 7. Ok 同士で Value が異なれば等しくない
    /// </summary>
    [Test]
    public void Ok_with_different_value_should_not_be_equal()
    {
        var other = Result<string, int>.Ok(10);

        Assert.AreNotEqual(_ok, other);
        Assert.IsTrue(_ok != other);
    }

    /// <summary>
    /// 8. Ok の ToString は "Success(value)" を返す
    /// </summary>
    [Test]
    public void Ok_ToString_should_return_formatted_value()
    {
        Assert.AreEqual("Ok(5)", _ok.ToString());
    }
}