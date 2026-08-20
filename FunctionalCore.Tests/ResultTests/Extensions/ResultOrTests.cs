using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultOrTests
{
    /// <summary>
    /// 1. Ok.Or は自身を返す
    /// </summary>
    [Test]
    public void Result_Ok_Or_should_return_original_result()
    {
        var ok = Result<string, int>.Ok(5);
        var other = Result<string, int>.Ok(10);

        var result = ok.Or(other);

        Assert.That(result, Is.EqualTo(ok));
    }

    /// <summary>
    /// 2. Fail.Or は代替 Result を返す
    /// </summary>
    [Test]
    public void Result_Fail_Or_should_return_other_result()
    {
        var fail = Result<string, int>.Fail("error");
        var other = Result<string, int>.Ok(10);

        var result = fail.Or(other);

        Assert.That(result, Is.EqualTo(other));
    }

    /// <summary>
    /// 3. Fail.Or に Fail を渡した場合は代替側の Error を持つ Fail を返す
    /// </summary>
    [Test]
    public void Result_Fail_Or_failure_should_return_other_failure()
    {
        var fail = Result<string, int>.Fail("error");
        var other = Result<string, int>.Fail("other error");

        var result = fail.Or(other);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("other error"));
        });
    }

    /// <summary>
    /// 4. Ok.Or は代替 Result を採用しない
    /// </summary>
    [Test]
    public void Result_Ok_Or_should_ignore_other_result()
    {
        var ok = Result<string, int>.Ok(5);
        var other = Result<string, int>.Ok(10);

        var result = ok.Or(other);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 5. Ok.Or では代替 Result が未初期化でも使用されない
    /// </summary>
    [Test]
    public void Result_Ok_Or_should_not_validate_unused_uninitialized_other()
    {
        var ok = Result<string, int>.Ok(5);
        var other = default(Result<string, int>);

        var result = ok.Or(other);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 6. Fail.Or で代替 Result が未初期化の場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Fail_Or_uninitialized_other_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        var other = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => fail.Or(other));
    }

    /// <summary>
    /// 7. Ok.Or(Func) は factory を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_Or_factory_should_not_be_invoked()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var result = ok.Or(() =>
        {
            count++;
            return Result<string, int>.Ok(10);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 8. Fail.Or(Func) は factory を1回だけ実行し、その Result を返す
    /// </summary>
    [Test]
    public void Result_Fail_Or_factory_should_be_invoked_once()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = fail.Or(() =>
        {
            count++;
            return Result<string, int>.Ok(10);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(10));
        });
    }

    /// <summary>
    /// 9. Fail.Or(Func) の factory が Fail を返した場合はその Fail を返す
    /// </summary>
    [Test]
    public void Result_Fail_Or_factory_returning_failure_should_return_failure()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Or(() => Result<string, int>.Fail("other error"));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("other error"));
        });
    }

    /// <summary>
    /// 10. factory が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Or_null_factory_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Assert.Throws<ArgumentNullException>(() => fail.Or((Func<Result<string, int>>)null!));
    }

    /// <summary>
    /// 11. Fail.Or(Func) の factory が未初期化 Result を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Fail_Or_factory_returning_uninitialized_result_should_throw()
    {
        var fail = Result<string, int>.Fail("error");
        Assert.Throws<InvalidOperationException>(() => fail.Or(() => default(Result<string, int>)));
    }

    /// <summary>
    /// 12. Ok.Or(Func) では未初期化 Result を返す factory でも実行されない
    /// </summary>
    [Test]
    public void Result_Ok_Or_should_not_evaluate_uninitialized_factory_result()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Or(() => default(Result<string, int>));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 13. 未初期化 Result で Or を呼び出すと InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_Or_should_throw()
    {
        var result = default(Result<string, int>);
        var other = Result<string, int>.Ok(10);

        Assert.Throws<InvalidOperationException>(() => result.Or(other));
    }

    /// <summary>
    /// 14. 未初期化 Result で Or(Func) を呼び出すと InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_Or_factory_should_throw()
    {
        var result = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() =>
            result.Or(() => Result<string, int>.Ok(10)));
    }
}