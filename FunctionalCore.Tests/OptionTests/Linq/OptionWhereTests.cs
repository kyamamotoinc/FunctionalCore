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
    /// 1. Some.Where で predicate が true の場合は元の Some を返す
    /// </summary>
    [Test]
    public void Option_Some_Where_true_should_keep_original_some()
    {
        var result = _some.Where(x => x > 0);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(result, Is.EqualTo(_some));
        });
    }

    /// <summary>
    /// 2. Some.Where で predicate が false の場合は None を返す
    /// </summary>
    [Test]
    public void Option_Some_Where_false_should_return_none()
    {
        var result = _some.Where(x => x < 0);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 3. Some.Where は predicate を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_Where_should_invoke_predicate_once()
    {
        int count = 0;

        _some.Where(x =>
        {
            count++;
            return x > 0;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. Some.Where は Value を predicate に渡す
    /// </summary>
    [Test]
    public void Option_Some_Where_should_pass_value_to_predicate()
    {
        int received = 0;

        _some.Where(value =>
        {
            received = value;
            return true;
        });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 5. None.Where は predicate を実行しない
    /// </summary>
    [Test]
    public void Option_None_Where_should_not_invoke_predicate()
    {
        int count = 0;

        var result = _none.Where(x =>
        {
            count++;
            return true;
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(0));
            Assert.That(result, Is.EqualTo(Option<int>.None));
        });
    }

    /// <summary>
    /// 6. predicate が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Where_null_predicate_should_throw()
    {
        Func<int, bool>? predicate = null;

        Assert.Throws<ArgumentNullException>(() => _some.Where(predicate!));
    }

    /// <summary>
    /// 7. None でも predicate が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Where_null_predicate_should_throw()
    {
        Func<int, bool>? predicate = null;

        Assert.Throws<ArgumentNullException>(() => _none.Where(predicate!));
    }

    /// <summary>
    /// 8. LINQ クエリ構文の where で Where が利用できる
    /// </summary>
    [Test]
    public void Option_Where_should_support_query_syntax()
    {
        var result =
            from x in _some
            where x > 0
            select x;

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 9. LINQ クエリ構文の where で条件を満たさない場合は None を返す
    /// </summary>
    [Test]
    public void Option_Where_query_syntax_false_should_return_none()
    {
        var result =
            from x in _some
            where x < 0
            select x;

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 10. None に対する LINQ クエリ構文の where は None を返す
    /// </summary>
    [Test]
    public void Option_None_Where_query_syntax_should_return_none()
    {
        var result =
            from x in _none
            where x > 0
            select x;

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 11. Default Option は None と同様に扱われる
    /// </summary>
    [Test]
    public void Option_Default_Where_should_return_none()
    {
        var option = default(Option<int>);

        var result = option.Where(x => x > 0);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }
}