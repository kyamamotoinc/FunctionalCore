namespace FunctionalCore.Tests.OptionTests;


public class OptionMapTests
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
    /// 1. Some.Map は selector を適用し Some を返す（Some → Some）
    /// </summary>
    [Test]
    public void Option_Some_Map_should_return_mapped_Some()
    {
        var result = _some.Map(x => x + 1);

        Assert.That(result, Is.EqualTo(Option<int>.Some(6)));
    }

    /// <summary>
    /// 2. Some.Map は selector が null を返した場合 None を返す（Some → None）
    /// </summary>
    [Test]
    public void Option_Some_Map_selector_returning_null_returns_None()
    {
        var result = _some.Map(x => (string)null);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 3. None.Map は selector を実行しない
    /// </summary>
    [Test]
    public void Option_None_Map_should_not_invoke_selector()
    {
        int count = 0;

        _none.Map(x =>
        {
            count++;
            return x + 1;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. None.Map は None を返す（None → None）
    /// </summary>
    [Test]
    public void Option_None_Map_should_return_None()
    {
        var result = _none.Map(x => x + 1);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 5. selector が null の場合は例外
    /// </summary>
    [Test]
    public void Option_Map_null_selector_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _some.Map<int>(null!));
    }
}
