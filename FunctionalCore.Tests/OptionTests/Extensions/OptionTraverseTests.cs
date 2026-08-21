using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionTraverseTests
{
    /// <summary>
    /// 1. すべてのselectorがSomeを返す場合は、変換後のすべての値を保持するSomeを返す。
    /// </summary>
    [Test]
    public void Traverse_should_return_some_collection_when_all_results_are_some()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse(x => Option<int>.Some(x * 2));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { 2, 4, 6 }));
        });
    }

    /// <summary>
    /// 2. selectorがNoneを返した場合はNoneを返す。
    /// </summary>
    [Test]
    public void Traverse_should_return_none_when_selector_returns_none()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse(x =>
            x == 2
                ? Option<int>.None
                : Option<int>.Some(x));

        Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
    }

    /// <summary>
    /// 3. selectorは各要素に元の順序で実行される。
    /// </summary>
    [Test]
    public void Traverse_should_invoke_selector_for_each_item_in_order()
    {
        var items = new[] { 1, 2, 3 };
        var receivedItems = new List<int>();

        var result = items.Traverse(x =>
        {
            receivedItems.Add(x);
            return Option<int>.Some(x);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(receivedItems, Is.EqualTo(new[] { 1, 2, 3 }));
        });
    }

    /// <summary>
    /// 4. selectorがNoneを返した場合は、それ以降の要素にselectorを実行しない。
    /// </summary>
    [Test]
    public void Traverse_should_not_invoke_selector_after_none()
    {
        var items = new[] { 1, 2, 3 };
        int count = 0;

        var result = items.Traverse(x =>
        {
            count++;

            return x == 2
                ? Option<int>.None
                : Option<int>.Some(x);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
            Assert.That(count, Is.EqualTo(2));
        });
    }

    /// <summary>
    /// 5. すべてのselectorがSomeを返す場合は、成功値の順序を元の要素の順序に保持する。
    /// </summary>
    [Test]
    public void Traverse_should_preserve_value_order()
    {
        var items = new[] { 3, 1, 2 };

        var result = items.Traverse(x => Option<string>.Some($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { "value:3", "value:1", "value:2" }));
        });
    }

    /// <summary>
    /// 6. Traverseはselectorによって値の型を変更できる。
    /// </summary>
    [Test]
    public void Traverse_should_change_value_type()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse(x => Option<string>.Some($"value:{x}"));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { "value:1", "value:2", "value:3" }));
        });
    }

    /// <summary>
    /// 7. itemsが空の場合は、空のコレクションを保持するSomeを返す。
    /// </summary>
    [Test]
    public void Traverse_should_return_some_empty_collection_when_items_are_empty()
    {
        var items = Array.Empty<int>();

        var result = items.Traverse(x => Option<int>.Some(x));

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.Empty);
        });
    }

    /// <summary>
    /// 8. itemsがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Traverse_should_throw_argument_null_exception_when_items_is_null()
    {
        IEnumerable<int> items = null!;

        Assert.Throws<ArgumentNullException>(() =>
            items.Traverse(x => Option<int>.Some(x)));
    }

    /// <summary>
    /// 9. selectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Traverse_should_throw_argument_null_exception_when_selector_is_null()
    {
        var items = new[] { 1, 2, 3 };
        Func<int, Option<int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() => items.Traverse(selector!));
    }

    /// <summary>
    /// 10. selectorがdefault Optionを返した場合はNoneとして扱う。
    /// </summary>
    [Test]
    public void Traverse_should_return_none_when_selector_returns_default_option()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse(x =>
            x == 2
                ? default
                : Option<int>.Some(x));

        Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
    }

    /// <summary>
    /// 11. selectorがNoneを返した場合は、それ以降にdefault Optionを返す予定でもselectorを実行しない。
    /// </summary>
    [Test]
    public void Traverse_should_return_none_without_invoking_selector_after_none()
    {
        var items = new[] { 1, 2, 3 };
        int count = 0;

        var result = items.Traverse(x =>
        {
            count++;

            if (x == 2)
                return Option<int>.None;

            if (x == 3)
                return default;

            return Option<int>.Some(x);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
            Assert.That(count, Is.EqualTo(2));
        });
    }

    /// <summary>
    /// 12. selectorが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Traverse_should_propagate_exception_when_selector_throws()
    {
        var items = new[] { 1, 2, 3 };
        var expectedException = new NotSupportedException("selector error");

        Func<int, Option<int>> selector = _ => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() => items.Traverse(selector));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}