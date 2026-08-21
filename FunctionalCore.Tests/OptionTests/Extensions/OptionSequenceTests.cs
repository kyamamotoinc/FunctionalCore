using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionSequenceTests
{
    /// <summary>
    /// 1. すべてのOptionがSomeの場合は、すべての値を保持するSomeを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_some_collection_when_all_options_are_some()
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
    /// 2. Noneを含む場合はNoneを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_none_when_options_contain_none()
    {
        var some1 = Option<int>.Some(1);
        var none = Option<int>.None;
        var some3 = Option<int>.Some(3);

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
    /// 3. 複数のNoneを含む場合もNoneを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_none_when_options_contain_multiple_none()
    {
        var none = Option<int>.None;
        var some2 = Option<int>.Some(2);

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
    /// 4. 空のシーケンスの場合は、空のコレクションを保持するSomeを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_some_empty_collection_when_options_are_empty()
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
    /// 5. optionsがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Sequence_should_throw_argument_null_exception_when_options_is_null()
    {
        IEnumerable<Option<int>> options = null!;

        Assert.Throws<ArgumentNullException>(() => options.Sequence());
    }

    /// <summary>
    /// 6. すべてのOptionがSomeの場合は、値の順序を保持する。
    /// </summary>
    [Test]
    public void Sequence_should_preserve_value_order()
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
    /// 7. default Optionを含む場合はNoneとして扱い、Noneを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_none_when_options_contain_default_option()
    {
        var some1 = Option<int>.Some(1);
        var defaultOption = default(Option<int>);
        var some3 = Option<int>.Some(3);

        var options = new[]
        {
            some1,
            defaultOption,
            some3
        };

        var result = options.Sequence();

        Assert.That(result, Is.EqualTo(Option<IReadOnlyList<int>>.None));
    }

    /// <summary>
    /// 8. Noneに到達した場合は、それ以降の要素を列挙せずNoneを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_none_without_enumerating_items_after_none()
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

    /// <summary>
    /// 9. optionsの列挙中に例外が発生した場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Sequence_should_propagate_exception_when_enumeration_throws()
    {
        var expectedException = new NotSupportedException("enumeration error");

        IEnumerable<Option<int>> Options()
        {
            yield return Option<int>.Some(1);
            throw expectedException;
        }

        var actualException = Assert.Throws<NotSupportedException>(() => Options().Sequence());

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}