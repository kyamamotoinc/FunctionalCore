
using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests;

public class OptionTapBothTests
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
    /// 8. TapBoth は Some のとき副作用を実行する
    /// </summary>
    [Test]
    public void Option_Some_TapBoth_should_invoke_action()
    {
        int count = 0;

        _some.TapBoth(_ => count++, () => count--);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 9. TapBoth は None のときも副作用を実行する
    /// </summary>
    [Test]
    public void Option_None_TapBoth_should_invoke_action()
    {
        int count = 0;

        _none.TapBoth(_ => count++, () => count--);

        Assert.That(count, Is.EqualTo(-1));
    }

    /// <summary>
    /// 10. TapBoth の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Option_TapBoth_null_acion_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _some.TapBoth(null!, null!));
    }
}