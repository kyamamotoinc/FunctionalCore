using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionOrTests
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
    /// 1. Some.Or は常に自身を返す（Some優先）
    /// </summary>
    [Test]
    public void Some_Or_should_return_self()
    {
        var other = Option<int>.Some(10);

        var result = _some.Or(other);

        Assert.AreEqual(_some, result);
    }

    /// <summary>
    /// 2. None.Or は代替Optionを返す（None → other）
    /// </summary>
    [Test]
    public void None_Or_should_return_other()
    {
        var other = Option<int>.Some(10);

        var result = _none.Or(other);

        Assert.AreEqual(other, result);
    }

    /// <summary>
    /// 3. None.Or(None) は None のまま
    /// </summary>
    [Test]
    public void None_Or_none_should_return_none()
    {
        var other = Option<int>.None;

        var result = _none.Or(other);

        Assert.IsFalse(result.HasValue);
    }

    /// <summary>
    /// 4. Some.Or は other を評価しない（即時版）
    /// </summary>
    [Test]
    public void Some_Or_should_not_use_other()
    {
        var other = Option<int>.Some(10);

        var result = _some.Or(other);

        Assert.AreEqual(5, result.Value);
    }

    /// <summary>
    /// 5. Some.Or(Func) は factory を実行しない（遅延評価）
    /// </summary>
    [Test]
    public void Some_Or_factory_should_not_be_invoked()
    {
        int count = 0;

        var result = _some.Or(() =>
        {
            count++;
            return Option<int>.Some(10);
        });

        Assert.AreEqual(0, count);
        Assert.AreEqual(5, result.Value);
    }

    /// <summary>
    /// 6. None.Or(Func) は factory を実行する（None → factory）
    /// </summary>
    [Test]
    public void None_Or_factory_should_be_invoked()
    {
        int count = 0;

        var result = _none.Or(() =>
        {
            count++;
            return Option<int>.Some(10);
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
        Assert.Throws<ArgumentNullException>(() => _none.Or((Func<Option<int>>)null));
    }
}