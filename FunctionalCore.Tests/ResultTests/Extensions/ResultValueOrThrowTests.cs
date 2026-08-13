using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultValueOrThrowTests
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
    /// 1. Ok.ValueOrThrow は内部の Value を返す
    /// </summary>
    [Test]
    public void Result_Ok_ValueOrThrow_should_return_inner_value()
    {
        var value = _ok.ValueOrThrow(error => new InvalidOperationException(error));

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Ok.ValueOrThrow は例外 factory を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_ValueOrThrow_should_not_invoke_exception_factory()
    {
        int count = 0;

        var value = _ok.ValueOrThrow(error =>
        {
            count++;
            return new InvalidOperationException(error);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 3. Fail.ValueOrThrow は factory が生成した例外を投げる
    /// </summary>
    [Test]
    public void Result_Fail_ValueOrThrow_should_throw_factory_exception()
    {
        Assert.Throws<InvalidOperationException>(() => _fail.ValueOrThrow(error => new InvalidOperationException(error)));
    }

    /// <summary>
    /// 4. Fail.ValueOrThrow は Error を例外 factory に渡す
    /// </summary>
    [Test]
    public void Result_Fail_ValueOrThrow_should_pass_error_to_exception_factory()
    {
        string? received = null;

        Assert.Throws<InvalidOperationException>(() => _fail.ValueOrThrow(error =>
        {
            received = error;
            return new InvalidOperationException(error);
        }));

        Assert.That(received, Is.EqualTo("error"));
    }

    /// <summary>
    /// 5. Fail.ValueOrThrow は例外 factory を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Fail_ValueOrThrow_should_invoke_exception_factory_once()
    {
        int count = 0;

        Assert.Throws<InvalidOperationException>(() => _fail.ValueOrThrow(error =>
        {
            count++;
            return new InvalidOperationException(error);
        }));

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 6. Fail.ValueOrThrow は factory が生成した例外インスタンスをそのまま投げる
    /// </summary>
    [Test]
    public void Result_Fail_ValueOrThrow_should_throw_same_exception_instance()
    {
        var expected = new InvalidOperationException("expected");

        var actual = Assert.Throws<InvalidOperationException>(() => _fail.ValueOrThrow(_ => expected));

        Assert.That(actual, Is.SameAs(expected));
    }

    /// <summary>
    /// 7. exception factory が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_ValueOrThrow_null_exception_factory_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _fail.ValueOrThrow(null!));
    }

    /// <summary>
    /// 8. Fail.ValueOrThrow で exception factory が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Fail_ValueOrThrow_exception_factory_returning_null_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _fail.ValueOrThrow(_ => null!));
    }

    /// <summary>
    /// 9. Ok.ValueOrThrow では null を返す exception factory でも実行されない
    /// </summary>
    [Test]
    public void Result_Ok_ValueOrThrow_should_not_evaluate_null_returning_exception_factory()
    {
        var value = _ok.ValueOrThrow(_ => null!);

        Assert.That(value, Is.EqualTo(5));
    }

    /// <summary>
    /// 10. 未初期化 Result で ValueOrThrow を呼び出すと InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_ValueOrThrow_should_throw()
    {
        var result = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => result.ValueOrThrow(error => new Exception(error)));
    }
}