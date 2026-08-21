namespace FunctionalCore.Tests.ResultTests;

public class DefaultResultTests
{
    /// <summary>
    /// 1. 未初期化Resultは成功でも失敗でもない。
    /// </summary>
    [Test]
    public void Default_Result_should_be_neither_success_nor_failure()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Multiple(() =>
        {
            Assert.That(uninitialized.IsSuccess, Is.False);
            Assert.That(uninitialized.IsFailure, Is.False);
        });
    }

    /// <summary>
    /// 2. 未初期化ResultのValueにアクセスした場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Result_accessing_Value_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => _ = uninitialized.Value);
    }

    /// <summary>
    /// 3. 未初期化ResultのErrorにアクセスした場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Result_accessing_Error_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => _ = uninitialized.Error);
    }

    /// <summary>
    /// 4. 未初期化ResultのToStringは未初期化状態を示す。
    /// </summary>
    [Test]
    public void Default_Result_ToString_should_indicate_uninitialized()
    {
        var uninitialized = default(Result<string, string>);

        Assert.That(uninitialized.ToString(), Does.Contain("uninitialized"));
    }

    /// <summary>
    /// 5. 未初期化Result同士は等しい。
    /// </summary>
    [Test]
    public void Two_default_Results_should_be_equal()
    {
        var uninitialized = default(Result<string, string>);
        var other = default(Result<string, string>);

        Assert.Multiple(() =>
        {
            Assert.That(uninitialized == other, Is.True);
            Assert.That(uninitialized.Equals(other), Is.True);
            Assert.That(uninitialized.GetHashCode(), Is.EqualTo(other.GetHashCode()));
        });
    }

    /// <summary>
    /// 6. 配列生成によってdefault値になったResultのValueにアクセスした場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Result_array_default_element_accessing_Value_should_throw_invalid_operation_exception()
    {
        var results = new Result<string, string>[1];

        Assert.Throws<InvalidOperationException>(() => _ = results[0].Value);
    }

    /// <summary>
    /// 7. 配列生成によってdefault値になったResultのErrorにアクセスした場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Result_array_default_element_accessing_Error_should_throw_invalid_operation_exception()
    {
        var results = new Result<string, string>[1];

        Assert.Throws<InvalidOperationException>(() => _ = results[0].Error);
    }

    /// <summary>
    /// 8. 未初期化ResultでMapを呼び出した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Result_Map_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Map(x => x));
    }

    /// <summary>
    /// 9. 未初期化ResultでMapErrorを呼び出した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Result_MapError_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.MapError(error => error));
    }

    /// <summary>
    /// 10. 未初期化ResultでBindを呼び出した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Result_Bind_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Bind(value => Result<string, string>.Ok(value)));
    }

    /// <summary>
    /// 11. 未初期化ResultでMatchを呼び出した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Result_Match_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Match(value => value, error => error));
    }

    /// <summary>
    /// 12. 未初期化ResultでEnsureを呼び出した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Result_Ensure_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Ensure(_ => true, value => value));
    }

    /// <summary>
    /// 13. 未初期化ResultでTapを呼び出した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Result_Tap_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.Tap(_ => { }));
    }

    /// <summary>
    /// 14. 未初期化ResultでTapErrorを呼び出した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Result_TapError_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, string>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.TapError(_ => { }));
    }
}