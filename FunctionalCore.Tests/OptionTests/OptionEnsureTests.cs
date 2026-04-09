

namespace FunctionalCore.Tests.OptionTests;

public class OptionEnsureTests
{
    private Option<int> _some;
    private Option<int> _none;

    [SetUp]
    public void Setup()
    {
        _some = Option<int>.Some(5);
        _none = Option<int>.None;
    }

    /// <summary>
    /// 1. Some.Ensure は predicate が true の場合、値を保持する（Some → Some）
    /// </summary>
    [Test]
    public void Option_Some_Ensure_true_should_keep_some()
    {
        var result = _some.Ensure(x => x > 0);

        Assert.AreEqual(Option<int>.Some(5), result);
    }

    /// <summary>
    /// 2. Some.Ensure は predicate が false の場合 None を返す（Some → None）
    /// </summary>
    [Test]
    public void Option_Some_Ensure_false_should_return_none()
    {
        var result = _some.Ensure(x => x < 0);

        Assert.AreEqual(Option<int>.None, result);
    }

    /// <summary>
    /// 3. None.Ensure は predicate を実行せず None を返す（None → None）
    /// </summary>
    [Test]
    public void Option_None_Ensure_should_not_invoke_predicate()
    {
        int count = 0;

        var result = _none.Ensure(x =>
        {
            count++;
            return true;
        });

        Assert.AreEqual(0, count);
        Assert.AreEqual(Option<int>.None, result);
    }

    /// <summary>
    /// 4. predicate が null の場合は例外を投げる
    /// </summary>
    [Test]
    public void Option_Ensure_null_predicate_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _some.Ensure(null));
    }
}

