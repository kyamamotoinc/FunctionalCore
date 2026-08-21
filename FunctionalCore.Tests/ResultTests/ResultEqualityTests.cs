namespace FunctionalCore.Tests.ResultTests;

public class ResultEqualityTests
{
    /// <summary>
    /// 1. Equals(object)にnullを渡した場合はfalseを返す。
    /// </summary>
    [Test]
    public void Equals_object_should_return_false_when_other_is_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.That(ok.Equals(null), Is.False);
    }

    /// <summary>
    /// 2. Equals(object)に異なる型を渡した場合はfalseを返す。
    /// </summary>
    [Test]
    public void Equals_object_should_return_false_when_other_is_different_type()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.That(ok.Equals("not result"), Is.False);
    }

    /// <summary>
    /// 3. 未初期化ResultとOkは等しくない。
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
    /// 4. 未初期化ResultとFailは等しくない。
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
    /// 5. objectとして比較した同一内容のOkは等しい。
    /// </summary>
    [Test]
    public void Ok_Equals_object_should_return_true_when_value_is_same()
    {
        var ok = Result<string, int>.Ok(5);
        object other = Result<string, int>.Ok(5);

        Assert.That(ok.Equals(other), Is.True);
    }

    /// <summary>
    /// 6. objectとして比較した同一内容のFailは等しい。
    /// </summary>
    [Test]
    public void Fail_Equals_object_should_return_true_when_error_is_same()
    {
        var fail = Result<string, int>.Fail("error");
        object other = Result<string, int>.Fail("error");

        Assert.That(fail.Equals(other), Is.True);
    }
}