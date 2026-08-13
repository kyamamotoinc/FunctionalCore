namespace FunctionalCore.Tests.ResultTests;

public class ResultEqualityTests
{
    /// <summary>
    /// 1. Result.Equals(object) に null を渡した場合は false を返す
    /// </summary>
    [Test]
    public void Result_Equals_null_should_return_false()
    {
        var result = Result<string, int>.Ok(5);

        Assert.That(result.Equals(null), Is.False);
    }

    /// <summary>
    /// 2. Result.Equals(object) に異なる型を渡した場合は false を返す
    /// </summary>
    [Test]
    public void Result_Equals_different_type_should_return_false()
    {
        var result = Result<string, int>.Ok(5);

        Assert.That(result.Equals("not result"), Is.False);
    }

    /// <summary>
    /// 3. 未初期化 Result と Ok は等しくない
    /// </summary>
    [Test]
    public void Default_Result_and_Ok_should_not_be_equal()
    {
        var uninitialized = default(Result<string, int>);
        var ok = Result<string, int>.Ok(5);

        Assert.Multiple(() =>
        {
            Assert.That(uninitialized == ok, Is.False);
            Assert.That(uninitialized != ok, Is.True);
            Assert.That(uninitialized.Equals(ok), Is.False);
        });
    }

    /// <summary>
    /// 4. 未初期化 Result と Fail は等しくない
    /// </summary>
    [Test]
    public void Default_Result_and_Fail_should_not_be_equal()
    {
        var uninitialized = default(Result<string, int>);
        var fail = Result<string, int>.Fail("error");

        Assert.Multiple(() =>
        {
            Assert.That(uninitialized == fail, Is.False);
            Assert.That(uninitialized != fail, Is.True);
            Assert.That(uninitialized.Equals(fail), Is.False);
        });
    }

    /// <summary>
    /// 5. object として比較した同一内容の Ok は等しい
    /// </summary>
    [Test]
    public void Result_Ok_Equals_object_with_same_value_should_return_true()
    {
        var result = Result<string, int>.Ok(5);
        object other = Result<string, int>.Ok(5);

        Assert.That(result.Equals(other), Is.True);
    }

    /// <summary>
    /// 6. object として比較した同一内容の Fail は等しい
    /// </summary>
    [Test]
    public void Result_Fail_Equals_object_with_same_error_should_return_true()
    {
        var result = Result<string, int>.Fail("error");
        object other = Result<string, int>.Fail("error");

        Assert.That(result.Equals(other), Is.True);
    }
}