using FunctionalCore.Linq;

namespace FunctionalCore.Tests.OptionTests.Linq;

public class OptionWhereTests
{
    /// <summary>
    /// 1. OptionがSomeでpredicateがtrueを返す場合は元のSomeをそのまま返す。
    /// </summary>
    [Test]
    public void Some_Where_should_return_original_some_when_predicate_returns_true()
    {
        var some = Option<int>.Some(5);

        var result = some.Where(x => x > 0);

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
    public void Some_Where_should_return_none_when_predicate_returns_false()
    {
        var some = Option<int>.Some(5);

        var result = some.Where(x => x < 0);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 3. OptionがSomeの場合はpredicateを1回だけ実行する。
    /// </summary>
    [Test]
    public void Some_Where_should_invoke_predicate_once()
    {
        var some = Option<int>.Some(5);
        int count = 0;

        some.Where(x =>
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
    public void Some_Where_should_pass_value_to_predicate()
    {
        var some = Option<int>.Some(5);
        int receivedValue = 0;

        some.Where(value =>
        {
            receivedValue = value;
            return true;
        });

        Assert.That(receivedValue, Is.EqualTo(5));
    }

    /// <summary>
    /// 5. OptionがNoneの場合はpredicateを実行せず、Noneを返す。
    /// </summary>
    [Test]
    public void None_Where_should_return_none_without_invoking_predicate()
    {
        var none = Option<int>.None;
        int count = 0;

        var result = none.Where(x =>
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
    /// 6. OptionがSomeの場合でもpredicateがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Some_Where_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var some = Option<int>.Some(5);
        Func<int, bool>? predicate = null;

        Assert.Throws<ArgumentNullException>(() => some.Where(predicate!));
    }

    /// <summary>
    /// 7. OptionがNoneの場合でもpredicateがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void None_Where_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var none = Option<int>.None;
        Func<int, bool>? predicate = null;

        Assert.Throws<ArgumentNullException>(() => none.Where(predicate!));
    }

    /// <summary>
    /// 8. LINQクエリ構文のwhereでWhereを利用できる。
    /// </summary>
    [Test]
    public void Where_should_support_query_syntax()
    {
        var some = Option<int>.Some(5);

        var result =
            from x in some
            where x > 0
            select x;

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 9. LINQクエリ構文のwhereでpredicateがfalseの場合はNoneを返す。
    /// </summary>
    [Test]
    public void Where_query_syntax_should_return_none_when_predicate_returns_false()
    {
        var some = Option<int>.Some(5);

        var result =
            from x in some
            where x < 0
            select x;

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 10. OptionがNoneの場合はLINQクエリ構文のwhereでもNoneを返す。
    /// </summary>
    [Test]
    public void None_Where_query_syntax_should_return_none()
    {
        var none = Option<int>.None;

        var result =
            from x in none
            where x > 0
            select x;

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 11. default OptionはNoneと同様にWhereでNoneを返す。
    /// </summary>
    [Test]
    public void Default_Where_should_return_none()
    {
        var defaultOption = default(Option<int>);

        var result = defaultOption.Where(x => x > 0);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 12. default Optionの場合でもpredicateがnullならArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Where_should_throw_argument_null_exception_when_predicate_is_null()
    {
        var defaultOption = default(Option<int>);
        Func<int, bool>? predicate = null;

        Assert.Throws<ArgumentNullException>(() => defaultOption.Where(predicate!));
    }

    /// <summary>
    /// 13. OptionがSomeでpredicateが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Some_Where_should_propagate_exception_when_predicate_throws()
    {
        var some = Option<int>.Some(5);
        var expectedException = new NotSupportedException("predicate error");
        Func<int, bool> predicate = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => some.Where(predicate));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}