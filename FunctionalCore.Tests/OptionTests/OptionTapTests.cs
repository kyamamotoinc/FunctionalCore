
using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests;

public class OptionTapTests
{
    private Option<int> _some;
    private Option<int> _none;

    [SetUp]
    public void Setup()
    {
        _some = Option<int>.Some(5);
        _none = Option<int>.None;
    }

    // -----------------------------
    // Tap (Some の副作用)
    // -----------------------------

    /// <summary>
    /// 1. Tap は Some のとき副作用を実行する
    /// </summary>
    [Test]
    public void Option_Some_Tap_should_invoke_action()
    {
        int count = 0;

        _some.Tap(x => count++);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 2. Tap は None のとき副作用を実行しない
    /// </summary>
    [Test]
    public void Option_None_Tap_should_not_invoke_action()
    {
        int count = 0;

        _none.Tap(x => count++);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 3. Tap は Option を変えずに返す（レールを変えない）
    /// </summary>
    [Test]
    public void Option_Tap_should_return_same_option()
    {
        var opt = _some.Tap(x => { });
        Assert.That(opt.Value, Is.EqualTo(_some.Value));
    }

    /// <summary>
    /// 4. Tap の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Option_Tap_null_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _some.Tap(null!));
    }

    //// -----------------------------
    //// TapNone (None の副作用)
    //// -----------------------------

    ///// <summary>
    ///// 5. TapNone は None のとき副作用を実行する
    ///// </summary>
    //[Test]
    //public void Option_None_TapNone_should_invoke_action()
    //{
    //    int count = 0;

    //    _none.TapNone(() => count++);

    //    Assert.That(count, Is.EqualTo(1));
    //}

    ///// <summary>
    ///// 6. TapNone は Some のとき副作用を実行しない
    ///// </summary>
    //[Test]
    //public void Option_Some_TapNone_should_not_invoke_action()
    //{
    //    int count = 0;

    //    _some.TapNone(() => count++);

    //    Assert.That(count, Is.EqualTo(0));
    //}

    ///// <summary>
    ///// 7. TapNone の action が null → ArgumentNullException
    ///// </summary>
    //[Test]
    //public void Option_TapNone_null_action_should_throw()
    //{
    //    ;
    //    Assert.Throws<ArgumentNullException>(() => _none.TapNone(null!));
    //}

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