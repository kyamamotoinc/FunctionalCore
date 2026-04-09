namespace FunctionalCore.Tests.OptionTests;

public class OptionMatchTests
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
    /// 1. Some.Match は成功側の関数を実行し、その結果を返す（Some → successFunc）
    /// </summary>
    [Test]
    public void Option_Some_Match_should_invoke_success_func()
    {
        var opt = _some.Match(
            x => x + 1,
            () => -1);

        Assert.AreEqual(6, opt);
    }

    /// <summary>
    /// 2. Some.Match は失敗側の関数を実行しない（排他性の保証）
    /// </summary>
    [Test]
    public void Option_Some_Match_should_not_invoke_failure_func()
    {
        int count = 0;
        var _ = _some.Match(
            x => x + 1,
            () =>
            {
                count++;
                return -1;
            });

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 3. None.Match は失敗側の関数を実行し、その結果を返す（None → failureFunc）
    /// </summary>
    [Test]
    public void Option_None_Match_should_invoke_failure_func()
    {
        var opt = _none.Match(
            x => x + 1,
            () => -1
        );

        Assert.AreEqual(-1, opt);
    }

    /// <summary>
    /// 4. None.Match は成功側の関数を実行しない（排他性の保証）
    /// </summary>
    [Test]
    public void Option_None_Match_should_not_invoke_success_func()
    {
        int count = 0;
        var _ = _none.Match(
            x =>
            {
                count++;
                return x + 1;
            },
            () => -1);

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 5. 成功側 func が null の場合は ArgumentNullException（null 禁止の世界観）
    /// </summary>
    [Test]
    public void Option_Some_Match_null_success_func_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = _some.Match<int>(null, () => -1));
    }

    /// <summary>
    /// 6. 失敗側 func が null の場合も ArgumentNullException（対称性の保証）
    /// </summary>
    [Test]
    public void Option_None_Match_null_failure_func_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _ = _none.Match(x => x + 1, null));
    }

    /// <summary>
    /// 7. 成功側 func が null を返した場合は例外（Match は null を外に出さない）
    /// </summary>
    [Test]
    public void Option_Some_Match_success_func_returning_null_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = _some.Match(
            x => (string)null,
            () => "fallback"
        ));
    }

    /// <summary>
    /// 8. 失敗側 func が null を返した場合も例外（出口としての Match の責務）
    /// </summary>
    [Test]
    public void Option_None_Match_failure_func_returning_null_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() => _ = _none.Match(
            x => "ok",
            () => (string)null
        ));
    }
}
