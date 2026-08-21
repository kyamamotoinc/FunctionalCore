using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultTraverseTests
{
    /// <summary>
    /// 1. すべてのselectorがOkを返す場合は、変換後の値を持つOkを返す。
    /// </summary>
    [Test]
    public void Result_Traverse_all_ok_should_return_ok_collection()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse<int, int, int>(x => Result<int, int>.Ok(x * 2));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(new[] { 2, 4, 6 }));
        });
    }

    /// <summary>
    /// 2. selectorがFailを返した場合はFailを返す。
    /// </summary>
    [Test]
    public void Result_Traverse_containing_failure_should_return_failure()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse<int, int, int>(x =>
            x == 2
                ? Result<int, int>.Fail(100)
                : Result<int, int>.Ok(x));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo(100));
        });
    }

    /// <summary>
    /// 3. 複数のFailが発生し得る場合は最初のErrorを返す。
    /// </summary>
    [Test]
    public void Result_Traverse_multiple_failures_should_return_first_error()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse<int, int, int>(x =>
            x switch
            {
                2 => Result<int, int>.Fail(200),
                3 => Result<int, int>.Fail(300),
                _ => Result<int, int>.Ok(x)
            });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo(200));
        });
    }

    /// <summary>
    /// 4. selectorは各要素に順番に実行される。
    /// </summary>
    [Test]
    public void Result_Traverse_should_invoke_selector_for_each_item()
    {
        var items = new[] { 1, 2, 3 };
        var received = new List<int>();

        var result = items.Traverse<int, int, int>(x =>
        {
            received.Add(x);
            return Result<int, int>.Ok(x);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(received, Is.EqualTo(new[] { 1, 2, 3 }));
        });
    }

    /// <summary>
    /// 5. Failが発生した後の要素にはselectorを実行しない。
    /// </summary>
    [Test]
    public void Result_Traverse_should_not_invoke_selector_after_failure()
    {
        var items = new[] { 1, 2, 3 };
        int count = 0;

        var result = items.Traverse<int, int, int>(x =>
        {
            count++;

            return x == 2
                ? Result<int, int>.Fail(100)
                : Result<int, int>.Ok(x);
        });

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(2));
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo(100));
        });
    }

    /// <summary>
    /// 6. 成功値の順序は元の要素の順序を保持する。
    /// </summary>
    [Test]
    public void Result_Traverse_should_preserve_order()
    {
        var items = new[] { 3, 1, 2 };

        var result = items.Traverse<int, int, string>(x => Result<int, string>.Ok($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { "value:3", "value:1", "value:2" }));
        });
    }

    /// <summary>
    /// 7. Traverseは成功値の型を変更できる。
    /// </summary>
    [Test]
    public void Result_Traverse_should_change_value_type()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse<string, int, string>(x => Result<string, string>.Ok($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { "value:1", "value:2", "value:3" }));
        });
    }

    /// <summary>
    /// 8. 空のシーケンスの場合は、空のコレクションを持つOkを返す。
    /// </summary>
    [Test]
    public void Result_Traverse_empty_collection_should_return_ok_empty_collection()
    {
        var items = Array.Empty<int>();

        var result = items.Traverse<string, int, int>(x => Result<string, int>.Ok(x));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        });
    }

    /// <summary>
    /// 9. nullのシーケンスを渡した場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Result_Traverse_null_items_should_throw()
    {
        IEnumerable<int> items = null!;

        Assert.Throws<ArgumentNullException>(() =>
            items.Traverse<string, int, int>(x => Result<string, int>.Ok(x)));
    }

    /// <summary>
    /// 10. selectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Result_Traverse_null_selector_should_throw()
    {
        var items = new[] { 1, 2, 3 };
        Func<int, Result<string, int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() => items.Traverse(selector!));
    }

    /// <summary>
    /// 11. selectorが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Result_Traverse_selector_returning_uninitialized_result_should_throw()
    {
        var items = new[] { 1, 2, 3 };

        Assert.Throws<InvalidOperationException>(() =>
            items.Traverse<string, int, int>(_ => default));
    }

    /// <summary>
    /// 12. Failより後ろで未初期化Resultを返す予定でも評価されない。
    /// </summary>
    [Test]
    public void Result_Traverse_should_not_evaluate_uninitialized_result_after_failure()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse<string, int, int>(x =>
        {
            if (x == 2)
                return Result<string, int>.Fail("error");

            if (x == 3)
                return default;

            return Result<string, int>.Ok(x);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 13. selectorが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Result_Traverse_should_propagate_exception_when_selector_throws()
    {
        var items = new[] { 1, 2, 3 };
        var expectedException = new NotSupportedException("selector error");

        Func<int, Result<string, int>> selector = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => items.Traverse(selector));

        Assert.That(actualException, Is.SameAs(expectedException));
    }

    /// <summary>
    /// 14. itemsの列挙中に例外が発生した場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Result_Traverse_should_propagate_exception_when_enumeration_throws()
    {
        var expectedException = new NotSupportedException("enumeration error");

        IEnumerable<int> Items()
        {
            yield return 1;
            throw expectedException;
        }

        var actualException = Assert.Throws<NotSupportedException>(() =>
            Items().Traverse<string, int, int>(x => Result<string, int>.Ok(x)));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}