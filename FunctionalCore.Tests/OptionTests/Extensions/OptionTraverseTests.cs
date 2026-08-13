using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionTraverseTests
{
    /// <summary>
    /// 1. すべての selector が Some を返す場合は、変換後の値を持つ Some を返す
    /// </summary>
    [Test]
    public void Option_Traverse_all_some_should_return_some_collection()
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
    /// 2. selector が None を返した場合は None を返す
    /// </summary>
    [Test]
    public void Option_Traverse_selector_returning_none_should_return_none()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse(x =>
            x == 2
                ? Option<int>.None
                : Option<int>.Some(x));

        Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
    }

    /// <summary>
    /// 3. selector は各要素に順番に実行される
    /// </summary>
    [Test]
    public void Option_Traverse_should_invoke_selector_for_each_item()
    {
        var items = new[] { 1, 2, 3 };
        var received = new List<int>();

        var result = items.Traverse(x =>
        {
            received.Add(x);
            return Option<int>.Some(x);
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(received, Is.EqualTo(new[] { 1, 2, 3 }));
        });
    }

    /// <summary>
    /// 4. None が発生した後の要素には selector を実行しない
    /// </summary>
    [Test]
    public void Option_Traverse_should_not_invoke_selector_after_none()
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
            Assert.That(count, Is.EqualTo(2));
            Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
        });
    }

    /// <summary>
    /// 5. 成功値の順序は元の要素の順序を保持する
    /// </summary>
    [Test]
    public void Option_Traverse_should_preserve_order()
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
    /// 6. Traverse は値の型を変更できる
    /// </summary>
    [Test]
    public void Option_Traverse_should_change_value_type()
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
    /// 7. 空のシーケンスの場合は、空のコレクションを持つ Some を返す
    /// </summary>
    [Test]
    public void Option_Traverse_empty_collection_should_return_some_empty_collection()
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
    /// 8. null のシーケンスを渡した場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Traverse_null_items_should_throw()
    {
        IEnumerable<int> items = null!;

        Assert.Throws<ArgumentNullException>(() =>
            items.Traverse(x => Option<int>.Some(x)));
    }

    /// <summary>
    /// 9. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Traverse_null_selector_should_throw()
    {
        var items = new[] { 1, 2, 3 };
        Func<int, Option<int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() =>
            items.Traverse(selector!));
    }

    /// <summary>
    /// 10. selector が Default Option を返した場合は None として扱われる
    /// </summary>
    [Test]
    public void Option_Traverse_selector_returning_default_option_should_return_none()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse(x =>
            x == 2
                ? default
                : Option<int>.Some(x));

        Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
    }

    /// <summary>
    /// 11. None より後ろで Default Option を返す予定でも selector は実行されない
    /// </summary>
    [Test]
    public void Option_Traverse_should_not_evaluate_default_option_after_none()
    {
        var items = new[] { 1, 2, 3 };

        var result = items.Traverse(x =>
        {
            if (x == 2)
                return Option<int>.None;

            if (x == 3)
                return default;

            return Option<int>.Some(x);
        });

        Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
    }
}