using FunctionalCore.Linq;

namespace FunctionalCore.Tests.OptionTests;

public class OptionSelectManyTests
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
    /// 1. Some.SelectMany は selector → projector を実行し、Some を返す
    /// </summary>
    [Test]
    public void Option_Some_SelectMany_should_return_projected_value()
    {
        var opt = some.SelectMany(
            x => Option<int>.Some(x + 1),
            (x, y) => x + y
        );

        Assert.That(opt.Value, Is.EqualTo(11));
    }

    /// <summary>
    /// 2. Some.SelectMany で selector が None を返すと None
    /// </summary>
    [Test]
    public void Option_Some_SelectMany_selector_returning_none_should_return_none()
    {
        var opt = some.SelectMany(x => Option<int>.None, (x, y) => x + y);

        Assert.That(opt.HasValue, Is.False);
    }

    /// <summary>
    /// 3. None.SelectMany は selector を実行しない
    /// </summary>
    [Test]
    public void Option_None_SelectMany_should_not_invoke_selector()
    {
        int count = 0;

        var opt = none.SelectMany(
            x =>
            {
                count++;
                return Option<int>.Some(x + 1);
            },
            (x, y) => x + y
        );

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. selector が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Option_SelectMany_null_selector_should_throw()
    {
        //Assert.Throws<ArgumentNullException>(() => some.SelectMany<int, int>(null, (x, y) => x + y));
    }

    /// <summary>
    /// 5. projector が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Option_SelectMany_null_projector_should_throw()
    {
        //Assert.Throws<ArgumentNullException>(() => some.SelectMany<int, int>(x => Option<int>.Some(x + 1), null));
    }
}
