using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultOrTests
{
    private Result<string, int> _ok;
    private Result<string, int> _fail;

    [SetUp]
    public void Setup()
    {
        _ok = Result<string, int>.Ok(5);
        _fail = Result<string, int>.Fail("error");
    }

    /// <summary>
    /// 1. Ok.Or は自身を返す
    /// </summary>
    [Test]
    public void Result_Ok_Or_should_return_original_result()
    {
        var other = Result<string, int>.Ok(10);

        var result = _ok.Or(other);

        Assert.That(result, Is.EqualTo(_ok));
    }

    /// <summary>
    /// 2. Fail.Or は代替 Result を返す
    /// </summary>
    [Test]
    public void Result_Fail_Or_should_return_other_result()
    {
        var other = Result<string, int>.Ok(10);

        var result = _fail.Or(other);

        Assert.That(result, Is.EqualTo(other));
    }

    /// <summary>
    /// 3. Fail.Or に Fail を渡した場合は代替側の Error を持つ Fail を返す
    /// </summary>
    [Test]
    public void Result_Fail_Or_failure_should_return_other_failure()
    {
        var other = Result<string, int>.Fail("other error");

        var result = _fail.Or(other);

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
        var other = Result<string, int>.Ok(10);

        var result = _ok.Or(other);

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
        var other = default(Result<string, int>);

        var result = _ok.Or(other);

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
        var other = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => _fail.Or(other));
    }

    /// <summary>
    /// 7. Ok.Or(Func) は factory を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_Or_factory_should_not_be_invoked()
    {
        int count = 0;

        var result = _ok.Or(() =>
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
        int count = 0;

        var result = _fail.Or(() =>
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
        var result = _fail.Or(() => Result<string, int>.Fail("other error"));

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
        Assert.Throws<ArgumentNullException>(() => _fail.Or((Func<Result<string, int>>)null!));
    }

    /// <summary>
    /// 11. Fail.Or(Func) の factory が未初期化 Result を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Fail_Or_factory_returning_uninitialized_result_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _fail.Or(() => default(Result<string, int>)));
    }

    /// <summary>
    /// 12. Ok.Or(Func) では未初期化 Result を返す factory でも実行されない
    /// </summary>
    [Test]
    public void Result_Ok_Or_should_not_evaluate_uninitialized_factory_result()
    {
        var result = _ok.Or(() => default(Result<string, int>));

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