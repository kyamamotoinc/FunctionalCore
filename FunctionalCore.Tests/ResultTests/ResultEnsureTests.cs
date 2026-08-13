namespace FunctionalCore.Tests.ResultTests;

public class ResultEnsureTests
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
    /// 1. Ok.Ensure で predicate が true の場合は元の Ok を保持する
    /// </summary>
    [Test]
    public void Result_Ok_Ensure_true_should_keep_original_ok()
    {
        var result = _ok.Ensure(x => x > 0, _ => "invalid");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(result, Is.EqualTo(_ok));
        });
    }

    /// <summary>
    /// 2. Ok.Ensure で predicate が false の場合は Fail に変換する
    /// </summary>
    [Test]
    public void Result_Ok_Ensure_false_should_return_failure()
    {
        var result = _ok.Ensure(x => x < 0, _ => "invalid");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("invalid"));
        });
    }

    /// <summary>
    /// 3. Ok.Ensure は predicate を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_Ensure_should_invoke_predicate_once()
    {
        int count = 0;

        _ok.Ensure(x =>
        {
            count++;
            return x > 0;
        }, _ => "invalid");

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. predicate が true の場合は errorFactory を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_Ensure_true_should_not_invoke_error_factory()
    {
        int count = 0;

        _ok.Ensure(x => x > 0, _ =>
        {
            count++;
            return "invalid";
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. predicate が false の場合は errorFactory を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_Ensure_false_should_invoke_error_factory_once()
    {
        int count = 0;

        var result = _ok.Ensure(x => x < 0, _ =>
        {
            count++;
            return "invalid";
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("invalid"));
        });
    }

    /// <summary>
    /// 6. Fail.Ensure は predicate を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Ensure_should_not_invoke_predicate()
    {
        int count = 0;

        _fail.Ensure(x =>
        {
            count++;
            return true;
        }, _ => "invalid");

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 7. Fail.Ensure は errorFactory を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Ensure_should_not_invoke_error_factory()
    {
        int count = 0;

        _fail.Ensure(_ => false, _ =>
        {
            count++;
            return "invalid";
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 8. Fail.Ensure は元の Error を保持する
    /// </summary>
    [Test]
    public void Result_Fail_Ensure_should_keep_original_error()
    {
        var result = _fail.Ensure(_ => false, _ => "invalid");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(result, Is.EqualTo(_fail));
        });
    }

    /// <summary>
    /// 9. predicate が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Ensure_null_predicate_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ok.Ensure(null!, _ => "invalid"));
    }

    /// <summary>
    /// 10. errorFactory が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Ensure_null_error_factory_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ok.Ensure(_ => false, null!));
    }

    /// <summary>
    /// 11. predicate が false で errorFactory が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_Ensure_error_factory_returning_null_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ok.Ensure(_ => false, _ => null!));
    }

    /// <summary>
    /// 12. predicate が true の場合は null を返す errorFactory でも実行されない
    /// </summary>
    [Test]
    public void Result_Ok_Ensure_true_should_not_evaluate_null_returning_error_factory()
    {
        var result = _ok.Ensure(_ => true, _ => null!);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 13. Fail.Ensure では null を返す errorFactory でも実行されない
    /// </summary>
    [Test]
    public void Result_Fail_Ensure_should_not_evaluate_null_returning_error_factory()
    {
        var result = _fail.Ensure(_ => false, _ => null!);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }
}