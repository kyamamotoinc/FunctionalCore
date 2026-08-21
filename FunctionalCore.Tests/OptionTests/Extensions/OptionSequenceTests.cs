using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionSequenceTests
{
    /// <summary>
    /// 1. すべて Some の場合は、すべての値を持つ Some を返す
    /// </summary>
    [Test]
    public void Option_Sequence_all_some_should_return_some_collection()
    {
        var some1 = Option<int>.Some(1);
        var some2 = Option<int>.Some(2);
        var some3 = Option<int>.Some(3);

        var options = new[]
        {
            some1,
            some2,
            some3
        };

        var result = options.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { 1, 2, 3 }));
        });
    }

    /// <summary>
    /// 2. None を含む場合は None を返す
    /// </summary>
    [Test]
    public void Option_Sequence_containing_none_should_return_none()
    {
        var some1 = Option<int>.Some(1);
        var some3 = Option<int>.Some(3);
        var none = Option<int>.None;

        var options = new[]
        {
            some1,
            none,
            some3
        };

        var result = options.Sequence();

        Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
    }

    /// <summary>
    /// 3. 複数の None が含まれていても None を返す
    /// </summary>
    [Test]
    public void Option_Sequence_containing_multiple_none_should_return_none()
    {
        var some2 = Option<int>.Some(2);
        var none = Option<int>.None;

        var options = new[]
        {
            none,
            some2,
            none
        };

        var result = options.Sequence();

        Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
    }

    /// <summary>
    /// 4. 空のシーケンスの場合は、空のコレクションを持つ Some を返す
    /// </summary>
    [Test]
    public void Option_Sequence_empty_collection_should_return_some_empty_collection()
    {
        var options = Array.Empty<Option<int>>();

        var result = options.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.Empty);
        });
    }

    /// <summary>
    /// 5. null のシーケンスを渡した場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_Sequence_null_options_should_throw()
    {
        IEnumerable<Option<int>> options = null!;

        Assert.Throws<ArgumentNullException>(() => options.Sequence());
    }

    /// <summary>
    /// 6. Some の値の順序は保持される
    /// </summary>
    [Test]
    public void Option_Sequence_should_preserve_order()
    {
        var some1 = Option<int>.Some(1);
        var some2 = Option<int>.Some(2);
        var some3 = Option<int>.Some(3);

        var options = new[]
        {
            some3,
            some1,
            some2
        };

        var result = options.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { 3, 1, 2 }));
        });
    }

    /// <summary>
    /// 7. Default Option を含む場合は None を返す
    /// </summary>
    [Test]
    public void Option_Sequence_containing_default_option_should_return_none()
    {
        var some1 = Option<int>.Some(1);
        var some3 = Option<int>.Some(3);

        var options = new[]
        {
            some1,
            default(Option<int>),
            some3
        };

        var result = options.Sequence();

        Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
    }

    /// <summary>
    /// 8. None に到達した時点で後続要素の列挙を停止する
    /// </summary>
    [Test]
    public void Option_Sequence_should_stop_enumeration_after_none()
    {
        int count = 0;

        IEnumerable<Option<int>> Options()
        {
            count++;
            yield return Option<int>.Some(1);

            count++;
            yield return Option<int>.None;

            count++;
            yield return Option<int>.Some(3);
        }

        var result = Options().Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
            Assert.That(count, Is.EqualTo(2));
        });
    }
}