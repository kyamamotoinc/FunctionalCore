
using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests;

public class OptionTapNoneTests
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
    /// 1. TapNone は None のとき副作用を実行する
    /// </summary>
    [Test]
    public void Option_None_TapNone_should_invoke_action()
    {
        int count = 0;

        _none.TapNone(() => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. TapNone は Some のとき副作用を実行しない
    /// </summary>
    [Test]
    public void Option_Some_TapNone_should_not_invoke_action()
    {
        int count = 0;

        _some.TapNone(() => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 3. TapNone の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Option_TapNone_null_action_should_throw()
    {
        ;
        Assert.Throws<ArgumentNullException>(() => _none.TapNone(null!));
    }

    //// -----------------------------
    //// TapBoth (Some / None 両方で副作用)
    //// -----------------------------

    ///// <summary>
    ///// 8. TapBoth は Some のとき副作用を実行する
    ///// </summary>
    //[Test]
    //public void Option_Some_TapBoth_should_invoke_action()
    //{
    //    int count = 0;

    //    _some.TapBoth(_ => count++);

    //    Assert.That(count, Is.EqualTo(1));
    //}

    ///// <summary>
    ///// 9. TapBoth は None のときも副作用を実行する
    ///// </summary>
    //[Test]
    //public void Option_None_TapBoth_should_invoke_action()
    //{
    //    int count = 0;

    //    _none.TapBoth(_ => count++);

    //    Assert.That(count, Is.EqualTo(1));
    //}

    ///// <summary>
    ///// 10. TapBoth の action が null → ArgumentNullException
    ///// </summary>
    //[Test]
    //public void Option_TapBoth_null_acion_should_throw()
    //{
    //    Assert.Throws<ArgumentNullException>(() => _some.TapBoth(null!));

    //}
}