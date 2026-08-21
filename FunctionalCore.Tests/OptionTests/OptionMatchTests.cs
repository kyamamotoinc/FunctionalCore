namespace FunctionalCore.Tests.OptionTests;

public class OptionMatchTests
{
    /// <summary>
    /// 1. OptionがSomeの場合はonSomeを実行し、その戻り値を返す。
    /// </summary>
    [Test]
    public void Some_Match_should_return_onSome_result()
    {
        var some = Option<int>.Some(5);

        var result = some.Match(value => value + 1, () => -1);

        Assert.That(result, Is.EqualTo(6));
    }

    /// <summary>
    /// 2. OptionがSomeの場合はonSomeを1回だけ実行する。
    /// </summary>
    [Test]
    public void Some_Match_should_invoke_onSome_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Match(value =>
        {
            count++;
            return value + 1;
        }, () => -1);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 3. OptionがSomeの場合はonNoneを実行しない。
    /// </summary>
    [Test]
    public void Some_Match_should_not_invoke_onNone()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Match(
            value => value + 1,
            () =>
            {
                count++;
                return -1;
            });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 4. OptionがNoneの場合はonNoneを実行し、その戻り値を返す。
    /// </summary>
    [Test]
    public void None_Match_should_return_onNone_result()
    {
        var none = Option<int>.None;

        var result = none.Match(value => value + 1, () => -1);

        Assert.That(result, Is.EqualTo(-1));
    }

    /// <summary>
    /// 5. OptionがNoneの場合はonNoneを1回だけ実行する。
    /// </summary>
    [Test]
    public void None_Match_should_invoke_onNone_once()
    {
        var none = Option<int>.None;
        int count = 0;

        none.Match(
            value => value + 1,
            () =>
            {
                count++;
                return -1;
            });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 6. OptionがNoneの場合はonSomeを実行しない。
    /// </summary>
    [Test]
    public void None_Match_should_not_invoke_onSome()
    {
        var none = Option<int>.None;
        int count = 0;

        none.Match(value =>
        {
            count++;
            return value + 1;
        }, () => -1);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 7. OptionがSomeの場合でもonSomeがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_Match_should_throw_argument_null_exception_when_onSome_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() => some.Match(onSome!, () => -1));
    }

    /// <summary>
    /// 8. OptionがNoneの場合でもonNoneがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_Match_should_throw_argument_null_exception_when_onNone_is_null()
    {
        var none = Option<int>.None;
        Func<int>? onNone = null;

        Assert.Throws<ArgumentNullException>(() => none.Match(value => value + 1, onNone!));
    }

    /// <summary>
    /// 9. OptionがSomeの場合でも未使用のonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_Match_should_throw_argument_null_exception_when_unused_onNone_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int>? onNone = null;

        Assert.Throws<ArgumentNullException>(() => some.Match(value => value + 1, onNone!));
    }

    /// <summary>
    /// 10. OptionがNoneの場合でも未使用のonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_Match_should_throw_argument_null_exception_when_unused_onSome_is_null()
    {
        var none = Option<int>.None;
        Func<int, int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() => none.Match(onSome!, () => -1));
    }

    /// <summary>
    /// 11. OptionがSomeでonSomeがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_Match_should_throw_invalid_operation_exception_when_onSome_returns_null()
    {
        var some = Option<int>.Some(5);

        Assert.Throws<InvalidOperationException>(() =>
            some.Match(_ => (string)null!, () => "fallback"));
    }

    /// <summary>
    /// 12. OptionがNoneでonNoneがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_Match_should_throw_invalid_operation_exception_when_onNone_returns_null()
    {
        var none = Option<int>.None;

        Assert.Throws<InvalidOperationException>(() =>
            none.Match(_ => "value", () => (string)null!));
    }

    /// <summary>
    /// 13. OptionがSomeの場合はnullを返すonNoneでも実行せず、onSomeの戻り値を返す。
    /// </summary>
    [Test]
    public void Some_Match_should_return_onSome_result_without_invoking_null_returning_onNone()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        var result = some.Match(
            value => $"value:{value}",
            () =>
            {
                count++;
                return (string)null!;
            });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("value:5"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 14. OptionがNoneの場合はnullを返すonSomeでも実行せず、onNoneの戻り値を返す。
    /// </summary>
    [Test]
    public void None_Match_should_return_onNone_result_without_invoking_null_returning_onSome()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = none.Match(
            _ =>
            {
                count++;
                return (string)null!;
            },
            () => "none");

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo("none"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 15. default OptionはNoneと同様にonNoneを実行し、その戻り値を返す。
    /// </summary>
    [Test]
    public void Default_Match_should_behave_as_none()
    {
        var defaultOption = default(Option<int>);

        var result = defaultOption.Match(value => value + 1, () => -1);

        Assert.That(result, Is.EqualTo(-1));
    }

    /// <summary>
    /// 16. default Optionの場合でもonSomeがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Match_should_throw_argument_null_exception_when_onSome_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<int, int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() => defaultOption.Match(onSome!, () => -1));
    }

    /// <summary>
    /// 17. default Optionの場合でもonNoneがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Match_should_throw_argument_null_exception_when_onNone_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<int>? onNone = null;

        Assert.Throws<ArgumentNullException>(() => defaultOption.Match(value => value + 1, onNone!));
    }

    /// <summary>
    /// 18. OptionがSomeでonSomeが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_Match_should_propagate_exception_when_onSome_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("onSome error");
        Func<int, int> onSome = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => some.Match(onSome, () => -1));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 19. OptionがNoneでonNoneが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void None_Match_should_propagate_exception_when_onNone_throws()
    {
        var none = Option<int>.None;
        var expectedException = new NotSupportedException("onNone error");
        Func<int> onNone = () => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => none.Match(value => value + 1, onNone));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}