using NUnit.Framework;
using System;

namespace FunctionalCore.Tests.OptionTests;

public class OptionBindTests
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
    /// 1. Some.Bind は binder を適用し、その結果を返す（Some → Some）
    /// </summary>
    [Test]
    public void Option_Some_Bind_should_return_binder_result()
    {
        var result = _some.Bind(x => Option<int>.Some(x + 1));

        Assert.AreEqual(Option<int>.Some(6), result);
    }

    /// <summary>
    /// 2. Some.Bind は None を返すことができる（Some → None）
    /// </summary>
    [Test]
    public void Option_Some_Bind_can_return_None()
    {
        var result = _some.Bind(x => Option<int>.None);

        Assert.AreEqual(Option<int>.None, result);
    }

    /// <summary>
    /// 3. None.Bind は binder を実行しない
    /// </summary>
    [Test]
    public void Option_None_Bind_should_not_invoke_binder()
    {
        int count = 0;

        _none.Bind(x =>
        {
            count++;
            return Option<int>.Some(x + 1);
        });

        Assert.AreEqual(0, count);
    }

    /// <summary>
    /// 4. None.Bind は None を返す（None → None）
    /// </summary>
    [Test]
    public void Option_None_Bind_should_return_None()
    {
        var result = _none.Bind(x => Option<int>.Some(x + 1));

        Assert.AreEqual(Option<int>.None, result);
    }

    /// <summary>
    /// 5. binder が null の場合は例外を投げる
    /// </summary>
    [Test]
    public void Option_Bind_null_binder_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() => _some.Bind<int>(null));
    }

    // 6. binder が null を返すことは設計上禁止（契約違反）
    //    呼び出し側の責務とするためテスト対象外
}