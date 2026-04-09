
namespace FunctionalCore.Tests.OptionTests;

public class OptionTapTests
{
    private Option<int> some;
    private Option<int> none;

    [SetUp]
    public void Setup()
    {
        some = Option<int>.Some(5);
        none = Option<int>.None;
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

        some.Tap(x => count++);

        Assert.AreEqual(1, count);
    }

    /// <summary>
    /// 2. Tap は None のとき副作用を実行しない
    /// </summary>
    [Test]
    public void Option_None_Tap_should_not_invoke_action()
    {
        int count = 0;

        none.Tap(x => count++);

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 3. Tap は Option を変えずに返す（レールを変えない）
    /// </summary>
    [Test]
    public void Option_Tap_should_return_same_option()
    {
        var opt = some.Tap(x => { });
        Assert.AreEqual(some.Value, opt.Value);
    }

    /// <summary>
    /// 4. Tap の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Option_Tap_null_action_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => some.Tap(null));
    }

    // -----------------------------
    // TapNone (None の副作用)
    // -----------------------------

    /// <summary>
    /// 5. TapNone は None のとき副作用を実行する
    /// </summary>
    [Test]
    public void Option_None_TapNone_should_invoke_action()
    {
        int count = 0;

        none.TapNone(() => count++);

        Assert.AreEqual(1, count);
    }

    /// <summary>
    /// 6. TapNone は Some のとき副作用を実行しない
    /// </summary>
    [Test]
    public void Option_Some_TapNone_should_not_invoke_action()
    {
        int count = 0;

        some.TapNone(() => count++);

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 7. TapNone の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Option_TapNone_null_action_should_throw()
    {
        ;
        Assert.Throws<ArgumentNullException>(() => none.TapNone(null));
    }

    // -----------------------------
    // TapBoth (Some / None 両方で副作用)
    // -----------------------------

    /// <summary>
    /// 8. TapBoth は Some のとき副作用を実行する
    /// </summary>
    [Test]
    public void Option_Some_TapBoth_should_invoke_action()
    {
        int count = 0;

        some.TapBoth(_ => count++);

        Assert.AreEqual(1, count);
    }

    /// <summary>
    /// 9. TapBoth は None のときも副作用を実行する
    /// </summary>
    [Test]
    public void Option_None_TapBoth_should_invoke_action()
    {
        int count = 0;

        none.TapBoth(_ => count++);

        Assert.AreEqual(1, count);
    }

    /// <summary>
    /// 10. TapBoth の action が null → ArgumentNullException
    /// </summary>
    [Test]
    public void Option_TapBoth_null_acion_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => some.TapBoth(null));

    }
}