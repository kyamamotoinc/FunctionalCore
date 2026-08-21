using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultTraverseTests
{
    /// <summary>
    /// 1. すべてのselectorがOkを返す場合は、変換後のすべての値を保持するOkを返す。
    /// </summary>
    [Test]
    public void Traverse_should_return_ok_collection_when_all_results_are_ok()
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
    /// 2. selectorがFailを返した場合は、そのFailを返す。
    /// </summary>
    [Test]
    public void Traverse_should_return_fail_when_selector_returns_fail()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse<int, int, int>(x =>
            x == 2
                ? Result<int, int>.Fail(100)
                : Result<int, int>.Ok(x));

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo(100));
        });
    }

    /// <summary>
    /// 3. selectorが複数のFailを返し得る場合は、最初のFailのErrorを返す。
    /// </summary>
    [Test]
    public void Traverse_should_return_first_error_when_selector_can_return_multiple_fails()
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
    /// 4. selectorは各要素に元の順序で実行される。
    /// </summary>
    [Test]
    public void Traverse_should_invoke_selector_for_each_item_in_order()
    {
        var items = new[] { 1, 2, 3 };
        var receivedItems = new List<int>();

        var result = items.Traverse<int, int, int>(x =>
        {
            receivedItems.Add(x);
            return Result<int, int>.Ok(x);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(receivedItems, Is.EqualTo(new[] { 1, 2, 3 }));
        });
    }

    /// <summary>
    /// 5. selectorがFailを返した場合は、それ以降の要素にselectorを実行しない。
    /// </summary>
    [Test]
    public void Traverse_should_not_invoke_selector_after_fail()
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
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo(100));
            Assert.That(count, Is.EqualTo(2));
        });
    }

    /// <summary>
    /// 6. すべてのselectorがOkを返す場合は、成功値の順序を元の要素の順序に保持する。
    /// </summary>
    [Test]
    public void Traverse_should_preserve_value_order()
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
    /// 7. Traverseはselectorによって成功値の型を変更できる。
    /// </summary>
    [Test]
    public void Traverse_should_change_value_type()
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
    /// 8. itemsが空の場合は、空のコレクションを保持するOkを返す。
    /// </summary>
    [Test]
    public void Traverse_should_return_ok_empty_collection_when_items_are_empty()
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
    /// 9. itemsがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Traverse_should_throw_argument_null_exception_when_items_is_null()
    {
        IEnumerable<int> items = null!;

        Assert.Throws<ArgumentNullException>(() =>
            items.Traverse<string, int, int>(x => Result<string, int>.Ok(x)));
    }

    /// <summary>
    /// 10. selectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Traverse_should_throw_argument_null_exception_when_selector_is_null()
    {
        var items = new[] { 1, 2, 3 };
        Func<int, Result<string, int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() => items.Traverse(selector!));
    }

    /// <summary>
    /// 11. selectorが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Traverse_should_throw_invalid_operation_exception_when_selector_returns_uninitialized_result()
    {
        var items = new[] { 1, 2, 3 };

        Assert.Throws<InvalidOperationException>(() =>
            items.Traverse<string, int, int>(_ => default));
    }

    /// <summary>
    /// 12. selectorがFailを返した場合は、それ以降に未初期化Resultを返す予定でもselectorを実行しない。
    /// </summary>
    [Test]
    public void Traverse_should_return_fail_without_invoking_selector_after_fail()
    {
        var items = new[] { 1, 2, 3 };
        int count = 0;

        var result = items.Traverse<string, int, int>(x =>
        {
            count++;

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
            Assert.That(count, Is.EqualTo(2));
        });
    }

    /// <summary>
    /// 13. selectorが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Traverse_should_propagate_exception_when_selector_throws()
    {
        var items = new[] { 1, 2, 3 };
        var expectedException = new NotSupportedException("selector error");

        var actualException = Assert.Throws<NotSupportedException>(() =>
            items.Traverse<string, int, int>(_ => throw expectedException));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}