namespace FunctionalCore.Tests.ResultTests;

public class DefaultResultTests
{
    /// <summary>
    /// 1. Default Result は成功でも失敗でもない
    /// </summary>
    [Test]
    public void Default_Result_should_be_neither_success_nor_failure()
    {
        Assert.Multiple(() =>
        {
            Assert.That(default(Result<string, string>).IsSuccess, Is.False);
            Assert.That(default(Result<string, string>).IsFailure, Is.False);
        });
    }

    /// <summary>
    /// 2. Default Result では Value にアクセスできない
    /// </summary>
    [Test]
    public void Default_Result_accessing_Value_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = default(Result<string, string>).Value);
    }

    /// <summary>
    /// 3. Default Result では Error にアクセスできない
    /// </summary>
    [Test]
    public void Default_Result_accessing_Error_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = default(Result<string, string>).Error);
    }

    /// <summary>
    /// 4. Default Result の ToString は未初期化を示す
    /// </summary>
    [Test]
    public void Default_Result_ToString_should_indicate_uninitialized()
    {
        Assert.That(default(Result<string, string>).ToString(), Does.Contain("uninitialized"));
    }

    /// <summary>
    /// 5. Default Result 同士は等しい
    /// </summary>
    [Test]
    public void Two_default_Results_should_be_equal()
    {
        var other = default(Result<string, string>);

        Assert.Multiple(() =>
        {
            Assert.That(default(Result<string, string>) == other, Is.True);
            Assert.That(default(Result<string, string>).Equals(other), Is.True);
            Assert.That(default(Result<string, string>).GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 6. 配列で生成された Result の初期値では Value にアクセスできない
    /// </summary>
    [Test]
    public void Array_initialized_Result_should_throw_on_Value()
    {
        var results = new Result<string, string>[1];

        Assert.Throws<InvalidOperationException>(() => _ = results[0].Value);
    }

    /// <summary>
    /// 7. 配列で生成された Result の初期値では Error にアクセスできない
    /// </summary>
    [Test]
    public void Array_initialized_Result_should_throw_on_Error()
    {
        var results = new Result<string, string>[1];

        Assert.Throws<InvalidOperationException>(() => _ = results[0].Error);
    }

    /// <summary>
    /// 8. Default Result で Map を呼び出すと例外が発生する
    /// </summary>
    [Test]
    public void Default_Result_Map_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => default(Result<string, string>).Map(x => x));
    }

    /// <summary>
    /// 9. Default Result で MapError を呼び出すと例外が発生する
    /// </summary>
    [Test]
    public void Default_Result_MapError_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => default(Result<string, string>).MapError(e => e));
    }

    /// <summary>
    /// 10. Default Result で Bind を呼び出すと例外が発生する
    /// </summary>
    [Test]
    public void Default_Result_Bind_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => default(Result<string, string>).Bind(x => Result<string, string>.Ok(x)));
    }

    /// <summary>
    /// 11. Default Result で Match を呼び出すと例外が発生する
    /// </summary>
    [Test]
    public void Default_Result_Match_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => default(Result<string, string>).Match(x => x, e => e));
    }

    /// <summary>
    /// 12. Default Result で Ensure を呼び出すと例外が発生する
    /// </summary>
    [Test]
    public void Default_Result_Ensure_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => default(Result<string, string>).Ensure(x => true, x => x));
    }

    /// <summary>
    /// 13. Default Result で Tap を呼び出すと例外が発生する
    /// </summary>
    [Test]
    public void Default_Result_Tap_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => default(Result<string, string>).Tap(_ => { }));
    }

    /// <summary>
    /// 14. Default Result で TapError を呼び出すと例外が発生する
    /// </summary>
    [Test]
    public void Default_Result_TapError_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => default(Result<string, string>).TapError(_ => { }));
    }
}