namespace FunctionalCore.Tests.OptionTests;

public class OptionEnsureTests
{
    /// <summary>
    /// 1. OptionがSomeでpredicateがtrueを返す場合は元のSomeをそのまま返す。
    /// </summary>
    [Test]
    public void Some_Ensure_should_return_original_some_when_predicate_returns_true()
    {
        var some = Option<int>.Some(5);

        var result = some.Ensure(x => x > 0);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(result, Is.EqualTo(some));
        });
    }

    /// <summary>
    /// 2. OptionがSomeでpredicateがfalseを返す場合はNoneを返す。
    /// </summary>
    [Test]
    public void Some_Ensure_should_return_none_when_predicate_returns_false()
    {
        var some = Option<int>.Some(5);

        var result = some.Ensure(x => x < 0);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 3. OptionがSomeの場合はpredicateを1回だけ実行する。
    /// </summary>
    [Test]
    public void Some_Ensure_should_invoke_predicate_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Ensure(x =>
        {
            count++;
            return x > 0;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. OptionがSomeの場合はValueをpredicateに渡す。
    /// </summary>
    [Test]
    public void Some_Ensure_should_pass_value_to_predicate()
    {
        var some = Option<int>.Some(5);
        int receivedValue = 0;

        some.Ensure(value =>
        {
            receivedValue = value;
            return true;
        });

        Assert.That(receivedValue, Is.EqualTo(5));
    }

    /// <summary>
    /// 5. OptionがNoneの場合はpredicateを実行しない。
    /// </summary>
    [Test]
    public void None_Ensure_should_not_invoke_predicate()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = none.Ensure(x =>
        {
            count++;
            return true;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 6. OptionがNoneの場合はNoneを返す。
    /// </summary>
    [Test]
    public void None_Ensure_should_return_none()
    {
        var none = Option<int>.None;

        var result = none.Ensure(_ => true);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 7. OptionがSomeの場合でもpredicateがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_Ensure_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, bool>? predicate = null;

        Assert.Throws<ArgumentNullException>(() => some.Ensure(predicate!));
    }

    /// <summary>
    /// 8. OptionがNoneの場合でもpredicateがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_Ensure_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var none = Option<int>.None;
        Func<int, bool>? predicate = null;

        Assert.Throws<ArgumentNullException>(() => none.Ensure(predicate!));
    }

    /// <summary>
    /// 9. default OptionはNoneと同様にpredicateを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public void Default_Ensure_should_return_none_without_invoking_predicate()
    {
        var defaultOption = default(Option<int>);
        int count = 0;

        var result = defaultOption.Ensure(x =>
        {
            count++;
            return true;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<int>.None));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 10. default Optionの場合でもpredicateがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Ensure_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<int, bool>? predicate = null;

        Assert.Throws<ArgumentNullException>(() => defaultOption.Ensure(predicate!));
    }

    /// <summary>
    /// 11. OptionがSomeでpredicateが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_Ensure_should_propagate_exception_when_predicate_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("predicate error");

        Func<int, bool> predicate = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => some.Ensure(predicate));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}