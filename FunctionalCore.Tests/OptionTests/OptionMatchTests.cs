namespace FunctionalCore.Tests.OptionTests;

public class OptionMatchTests
{
    /// <summary>
    /// 1. Some.Match は Some 側の関数を実行し、その結果を返す
    /// </summary>
    [Test]
    public void Option_Some_Match_should_return_some_func_result()
    {
        var some = Option<int>.Some(5);
        var result = some.Match(value => value + 1, () => -1);

        Assert.That(result, Is.EqualTo(6));
    }

    /// <summary>
    /// 2. Some.Match は Some 側の関数を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_Match_should_invoke_some_func_once()
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
    /// 3. Some.Match は None 側の関数を実行しない
    /// </summary>
    [Test]
    public void Option_Some_Match_should_not_invoke_none_func()
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
    /// 4. None.Match は None 側の関数を実行し、その結果を返す
    /// </summary>
    [Test]
    public void Option_None_Match_should_return_none_func_result()
    {
        var none = Option<int>.None;
        var result = none.Match(value => value + 1, () => -1);

        Assert.That(result, Is.EqualTo(-1));
    }

    /// <summary>
    /// 5. None.Match は None 側の関数を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_None_Match_should_invoke_none_func_once()
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
    /// 6. None.Match は Some 側の関数を実行しない
    /// </summary>
    [Test]
    public void Option_None_Match_should_not_invoke_some_func()
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
    /// 7. Some 側の関数が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Match_null_some_func_should_throw()
    {
        var some = Option<int>.Some(5);
        Func<int, int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.Match(onSome!, () => -1));
    }

    /// <summary>
    /// 8. None 側の関数が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Match_null_none_func_should_throw()
    {
        var none = Option<int>.None;
        Func<int>? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.Match(value => value + 1, onNone!));
    }

    /// <summary>
    /// 9. Some でも未使用の None 側関数が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Some_Match_null_unused_none_func_should_throw()
    {
        var some = Option<int>.Some(5);
        Func<int>? onNone = null;

        Assert.Throws<ArgumentNullException>(() =>
            some.Match(value => value + 1, onNone!));
    }

    /// <summary>
    /// 10. None でも未使用の Some 側関数が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Match_null_unused_some_func_should_throw()
    {
        var none = Option<int>.None;
        Func<int, int>? onSome = null;

        Assert.Throws<ArgumentNullException>(() =>
            none.Match(onSome!, () => -1));
    }

    /// <summary>
    /// 11. Some.Match で Some 側の関数が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Option_Some_Match_some_func_returning_null_should_throw()
    {
        var some = Option<int>.Some(5);
        Assert.Throws<InvalidOperationException>(() =>
            some.Match(_ => (string)null!, () => "fallback"));
    }

    /// <summary>
    /// 12. None.Match で None 側の関数が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Option_None_Match_none_func_returning_null_should_throw()
    {
        var none = Option<int>.None;
        Assert.Throws<InvalidOperationException>(() =>
            none.Match(_ => "value", () => (string)null!));
    }

    /// <summary>
    /// 13. Some.Match では null を返す None 側の関数でも実行されない
    /// </summary>
    [Test]
    public void Option_Some_Match_should_not_evaluate_null_returning_none_func()
    {
        var some = Option<int>.Some(5);
        var result = some.Match(value => $"value:{value}", () => (string)null!);

        Assert.That(result, Is.EqualTo("value:5"));
    }

    /// <summary>
    /// 14. None.Match では null を返す Some 側の関数でも実行されない
    /// </summary>
    [Test]
    public void Option_None_Match_should_not_evaluate_null_returning_some_func()
    {
        var none = Option<int>.None;
        var result = none.Match(_ => (string)null!, () => "none");

        Assert.That(result, Is.EqualTo("none"));
    }

    /// <summary>
    /// 15. Default Option は None と同様に None 側の関数を実行する
    /// </summary>
    [Test]
    public void Option_Default_Match_should_behave_as_none()
    {
        var option = default(Option<int>);

        var result = option.Match(value => value + 1, () => -1);

        Assert.That(result, Is.EqualTo(-1));
    }
}