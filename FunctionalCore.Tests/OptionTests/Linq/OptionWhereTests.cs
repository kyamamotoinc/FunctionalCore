using FunctionalCore.Linq;

namespace FunctionalCore.Tests.OptionTests.Linq;
public class OptionWhereTests
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
    /// 1. Some.Where は predicate を実行し、条件を満たす場合は Some を返す
    /// </summary>
    [Test]
    public void Option_Some_Where_should_return_some_if_predicate_is_true()
    {
        var opt = _some.Where(x => x > 0);
        Assert.That(opt.Value, Is.EqualTo(5));
    }

    /// <summary>
    /// 2. Some.Where は predicate を実行し、条件を満たさない場合は None を返す
    /// </summary>
    [Test]
    public void Option_Some_Where_should_return_none_if_predicate_is_false()
    {
        var opt = _some.Where(x => x < 0);
        Assert.That(opt, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 3. None.Where は predicate を実行しない
    /// </summary>
    [Test]
    public void Option_None_Where_should_not_invoke_predicate()
    {
        int count = 0;

        var opt = _none.Where(x =>
        {
            count++;
            return x > 0;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. Some.Where は predicate が null の場合、ArgumentNullException をスローする
    /// </summary>
    [Test]
    public void Option_Some_Where_null_predicate_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _some.Where(null!));
    }
}
