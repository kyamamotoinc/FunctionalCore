using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionTapBothTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はonSomeだけを実行する。
    /// </summary>
    [Test]
    public void Some_TapBoth_should_invoke_only_onSome()
    {
        var some = Option<int>.Some(5);
        int someCount = 0;
        int noneCount = 0;

        some.TapBoth(_ => someCount++, () => noneCount++);

        Assert.Multiple(() =>
        {
            Assert.That(someCount, Is.EqualTo(1));
            Assert.That(noneCount, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 2. OptionがNoneの場合はonNoneだけを実行する。
    /// </summary>
    [Test]
    public void None_TapBoth_should_invoke_only_onNone()
    {
        var none = Option<int>.None;
        int someCount = 0;
        int noneCount = 0;

        none.TapBoth(_ => someCount++, () => noneCount++);

        Assert.Multiple(() =>
        {
            Assert.That(someCount, Is.EqualTo(0));
            Assert.That(noneCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 3. OptionがSomeの場合はValueをonSomeに渡す。
    /// </summary>
    [Test]
    public void Some_TapBoth_should_pass_value_to_onSome()
    {
        var some = Option<int>.Some(5);
        int receivedValue = 0;

        some.TapBoth(value => receivedValue = value, () => { });

        Assert.That(receivedValue, Is.EqualTo(5));
    }

    /// <summary>
    /// 4. OptionがSomeの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public void Some_TapBoth_should_return_original_option()
    {
        var some = Option<int>.Some(5);

        var result = some.TapBoth(_ => { }, () => { });

        Assert.That(result, Is.EqualTo(some));
    }

    /// <summary>
    /// 5. OptionがNoneの場合は元のOptionをそのまま返す。
    /// </summary>
    [Test]
    public void None_TapBoth_should_return_original_option()
    {
        var none = Option<int>.None;

        var result = none.TapBoth(_ => { }, () => { });

        Assert.That(result, Is.EqualTo(none));
    }

    /// <summary>
    /// 6. OptionがSomeの場合でもonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_TapBoth_should_throw_argument_null_exception_when_onSome_is_null()
    {
        var some = Option<int>.Some(5);
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.TapBoth(onSome!, () => { }));
    }

    /// <summary>
    /// 7. OptionがNoneの場合でもonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_TapBoth_should_throw_argument_null_exception_when_onNone_is_null()
    {
        var none = Option<int>.None;
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.TapBoth(_ => { }, onNone!));
    }

    /// <summary>
    /// 8. OptionがSomeの場合でも未使用のonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_TapBoth_should_throw_argument_null_exception_when_unused_onNone_is_null()
    {
        var some = Option<int>.Some(5);
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.TapBoth(_ => { }, onNone!));
    }

    /// <summary>
    /// 9. OptionがNoneの場合でも未使用のonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_TapBoth_should_throw_argument_null_exception_when_unused_onSome_is_null()
    {
        var none = Option<int>.None;
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.TapBoth(onSome!, () => { }));
    }

    /// <summary>
    /// 10. default OptionはNoneと同様にonNoneだけを実行し、Noneを返す。
    /// </summary>
    [Test]
    public void Default_TapBoth_should_behave_as_none()
    {
        var defaultOption = default(Option<int>);
        int someCount = 0;
        int noneCount = 0;

        var result = defaultOption.TapBoth(
            _ => someCount++,
            () => noneCount++);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(someCount, Is.EqualTo(0));
            Assert.That(noneCount, Is.EqualTo(1));
        });
    }

    /// <summary>
    /// 11. default Optionの場合でもonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_TapBoth_should_throw_argument_null_exception_when_onSome_is_null()
    {
        var defaultOption = default(Option<int>);
        Action<int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            defaultOption.TapBoth(onSome!, () => { }));
    }

    /// <summary>
    /// 12. default Optionの場合でもonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_TapBoth_should_throw_argument_null_exception_when_onNone_is_null()
    {
        var defaultOption = default(Option<int>);
        Action? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            defaultOption.TapBoth(_ => { }, onNone!));
    }

    /// <summary>
    /// 13. OptionがSomeでonSomeが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_TapBoth_should_propagate_exception_when_onSome_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("onSome error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            some.TapBoth(_ => throw expectedException, () => { }));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 14. OptionがNoneでonNoneが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void None_TapBoth_should_propagate_exception_when_onNone_throws()
    {
        var none = Option<int>.None;
        var expectedException = new NotSupportedException("onNone error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            none.TapBoth(_ => { }, () => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}