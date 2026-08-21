using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultSequenceTests
{
    /// <summary>
    /// 1. すべてOkの場合は、すべての値を持つOkを返す。
    /// </summary>
    [Test]
    public void Result_Sequence_all_ok_should_return_ok_collection()
    {
        var ok1 = Result<string, int>.Ok(1);
        var ok2 = Result<string, int>.Ok(2);
        var ok3 = Result<string, int>.Ok(3);

        var results = new[]
        {
            ok1,
            ok2,
            ok3
        };

        var result = results.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(new[] { 1, 2, 3 }));
        });
    }

    /// <summary>
    /// 2. Failを含む場合はFailを返す。
    /// </summary>
    [Test]
    public void Result_Sequence_containing_failure_should_return_failure()
    {
        var ok1 = Result<string, int>.Ok(1);
        var ok3 = Result<string, int>.Ok(3);
        var fail = Result<string, int>.Fail("error");

        var results = new[]
        {
            ok1,
            fail,
            ok3
        };

        var result = results.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 3. 複数のFailがある場合は最初のErrorを返す。
    /// </summary>
    [Test]
    public void Result_Sequence_multiple_failures_should_return_first_error()
    {
        var ok1 = Result<string, int>.Ok(1);

        var results = new[]
        {
            ok1,
            Result<string, int>.Fail("first error"),
            Result<string, int>.Fail("second error")
        };

        var result = results.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("first error"));
        });
    }

    /// <summary>
    /// 4. 空のシーケンスの場合は、空のコレクションを持つOkを返す。
    /// </summary>
    [Test]
    public void Result_Sequence_empty_collection_should_return_ok_empty_collection()
    {
        var results = Array.Empty<Result<string, int>>();

        var result = results.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.Empty);
        });
    }

    /// <summary>
    /// 5. nullのシーケンスを渡した場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Result_Sequence_null_results_should_throw()
    {
        IEnumerable<Result<string, int>> results = null!;

        Assert.Throws<ArgumentNullException>(() => results.Sequence());
    }

    /// <summary>
    /// 6. 成功値の順序は保持される。
    /// </summary>
    [Test]
    public void Result_Sequence_should_preserve_order()
    {
        var ok1 = Result<string, int>.Ok(1);
        var ok2 = Result<string, int>.Ok(2);
        var ok3 = Result<string, int>.Ok(3);

        var results = new[]
        {
            ok3,
            ok1,
            ok2
        };

        var result = results.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(new[] { 3, 1, 2 }));
        });
    }

    /// <summary>
    /// 7. 未初期化Resultを含む場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Result_Sequence_containing_uninitialized_result_should_throw()
    {
        var ok1 = Result<string, int>.Ok(1);
        var ok3 = Result<string, int>.Ok(3);

        var results = new[]
        {
            ok1,
            default(Result<string, int>),
            ok3
        };

        Assert.Throws<InvalidOperationException>(() => results.Sequence());
    }

    /// <summary>
    /// 8. Failより後ろに未初期化Resultがあっても評価されない。
    /// </summary>
    [Test]
    public void Result_Sequence_should_not_evaluate_items_after_failure()
    {
        var ok1 = Result<string, int>.Ok(1);
        var fail = Result<string, int>.Fail("error");

        var results = new[]
        {
            ok1,
            fail,
            default(Result<string, int>)
        };

        var result = results.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 9. resultsの列挙中に例外が発生した場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Result_Sequence_should_propagate_exception_when_enumeration_throws()
    {
        var expectedException = new NotSupportedException("enumeration error");

        IEnumerable<Result<string, int>> Results()
        {
            yield return Result<string, int>.Ok(1);
            throw expectedException;
        }

        var actualException = Assert.Throws<NotSupportedException>(() => Results().Sequence());

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}