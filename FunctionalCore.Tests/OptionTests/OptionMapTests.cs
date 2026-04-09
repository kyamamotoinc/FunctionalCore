namespace FunctionalCore.Tests.ResultTests;


public class OptionMapTests
{
    private Option<int> some;
    private Option<int> none;


    [SetUp]
    public void Setup()
    {
        some = Option<int>.Some(5);
        none = Option<int>.None;
    }

    /// <summary>
    /// 1. Some.Map は selector を適用し Some を返す（Some → Some）
    /// </summary>
    [Test]
    public void Option_Some_Map_should_return_mapped_Some()
    {
        var result = some.Map(x => x + 1);

        Assert.AreEqual(Option<int>.Some(6), result);
    }

    /// <summary>
    /// 2. Some.Map は selector が null を返した場合 None を返す（Some → None）
    /// </summary>
    [Test]
    public void Option_Some_Map_selector_returning_null_returns_None()
    {
        var result = some.Map(x => (string)null);

        Assert.AreEqual(Option<string>.None, result);
    }

    /// <summary>
    /// 3. None.Map は selector を実行しない
    /// </summary>
    [Test]
    public void Option_None_Map_should_not_invoke_selector()
    {
        int count = 0;

        none.Map(x =>
        {
            count++;
            return x + 1;
        });

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 4. None.Map は None を返す（None → None）
    /// </summary>
    [Test]
    public void Option_None_Map_should_return_None()
    {
        var result = none.Map(x => x + 1);

        Assert.AreEqual(Option<int>.None, result);
    }

    /// <summary>
    /// 5. selector が null の場合は例外
    /// </summary>
    [Test]
    public void Option_Map_null_selector_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => some.Map<int>(null));
    }
}
