
namespace FunctionalCore.Tests.OptionTests;

public class OptionSelectTests
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
    /// 1. Some.Select は selector を実行し、Some を返す
    /// </summary>
    [Test]
    public void Option_Some_Select_should_return_selector_result()
    {
        var opt = some.Select(x => x + 1);
        Assert.AreEqual(6, opt.Value);
    }

    /// <summary>
    /// 2. None.Select は selector を実行しない
    /// </summary>
    [Test]
    public void Option_None_Select_should_not_invoke_selector()
    {
        int count = 0;

        var opt = none.Select(x =>
        {
            count++;
            return x + 1;
        });

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 3. selector が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Option_Select_null_selector_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => some.Select<string>(null));
    }
}
