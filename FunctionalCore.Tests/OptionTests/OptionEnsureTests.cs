namespace FunctionalCore.Tests.OptionTests;

public class OptionEnsureTests
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
    /// 1. Some.Ensure で predicate が true の場合は元の Some を保持する
    /// </summary>
    [Test]
    public void Option_Some_Ensure_true_should_keep_original_some()
    {
        var result = _some.Ensure(x => x > 0);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
            Assert.That(result, Is.EqualTo(_some));
        });
    }

    /// <summary>
    /// 2. Some.Ensure で predicate が false の場合は None を返す
    /// </summary>
    [Test]
    public void Option_Some_Ensure_false_should_return_none()
    {
        var result = _some.Ensure(x => x < 0);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 3. Some.Ensure は predicate を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_Ensure_should_invoke_predicate_once()
    {
        int count = 0;

        _some.Ensure(x =>
        {
            count++;
            return x > 0;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. Some.Ensure は Value を predicate に渡す
    /// </summary>
    [Test]
    public void Option_Some_Ensure_should_pass_value_to_predicate()
    {
        int received = 0;

        _some.Ensure(value =>
        {
            received = value;
            return true;
        });

        Assert.That(received, Is.EqualTo(5));
    }

    /// <summary>
    /// 5. None.Ensure は predicate を実行しない
    /// </summary>
    [Test]
    public void Option_None_Ensure_should_not_invoke_predicate()
    {
        int count = 0;

        var result = _none.Ensure(x =>
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
    /// 6. None.Ensure は None を返す
    /// </summary>
    [Test]
    public void Option_None_Ensure_should_return_none()
    {
        var result = _none.Ensure(_ => true);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 7. predicate が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Ensure_null_predicate_should_throw()
    {
        Func<int, bool>? predicate = null;

        Assert.Throws<ArgumentNullException>(() => _some.Ensure(predicate!));
    }

    /// <summary>
    /// 8. None でも predicate が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Ensure_null_predicate_should_throw()
    {
        Func<int, bool>? predicate = null;

        Assert.Throws<ArgumentNullException>(() => _none.Ensure(predicate!));
    }

    /// <summary>
    /// 9. Default Option は None と同様に predicate を実行しない
    /// </summary>
    [Test]
    public void Option_Default_Ensure_should_not_invoke_predicate()
    {
        var option = default(Option<int>);
        int count = 0;

        var result = option.Ensure(x =>
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
}