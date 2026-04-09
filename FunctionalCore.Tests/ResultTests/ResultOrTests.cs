namespace FunctionalCore.Tests.OptionTests;

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
    /// 1. Some.Or は常に自身を返す（Some優先）
    /// </summary>
    [Test]
    public void Ok_Or_should_return_self()
    {
        var other = Result<string, int>.Ok(10);

        var result = _ok.Or(other);

        Assert.AreEqual(_ok, result);
    }

    /// <summary>
    /// 2. None.Or は代替Optionを返す（None → other）
    /// </summary>
    [Test]
    public void Fail_Or_should_return_other()
    {
        var other = Result<string, int>.Ok(10);

        var result = _fail.Or(other);

        Assert.AreEqual(other, result);
    }

    /// <summary>
    /// 3. None.Or(None) は None のまま
    /// </summary>
    [Test]
    public void Fail_Or_none_should_return_none()
    {
        var other = Result<string, int>.Fail("error");

        var result = _fail.Or(other);

        Assert.IsFalse(result.IsSuccess);
    }

    /// <summary>
    /// 4. Some.Or は other を評価しない（即時版）
    /// </summary>
    [Test]
    public void Ok_Or_should_not_use_other()
    {
        var other = Result<string, int>.Ok(10);

        var result = _ok.Or(other);

        Assert.AreEqual(5, result.Value);
    }

    /// <summary>
    /// 5. Some.Or(Func) は factory を実行しない（遅延評価）
    /// </summary>
    [Test]
    public void Ok_Or_factory_should_not_be_invoked()
    {
        int count = 0;

        var result = _ok.Or(() =>
        {
            count++;
            return Result<string, int>.Ok(10);
        });

        Assert.AreEqual(0, count);
        Assert.AreEqual(5, result.Value);
    }

    /// <summary>
    /// 6. None.Or(Func) は factory を実行する（None → factory）
    /// </summary>
    [Test]
    public void Fail_Or_factory_should_be_invoked()
    {
        int count = 0;

        var result = _fail.Or(() =>
        {
            count++;
            return Result<string, int>.Ok(10);
        });

        Assert.AreEqual(1, count);
        Assert.AreEqual(10, result.Value);
    }

    /// <summary>
    /// 7. factory が null の場合は ArgumentNullException
    /// </summary>
    [Test]
    public void Or_null_factory_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _fail.Or((Func<Result<string, int>>)null!));
    }
}