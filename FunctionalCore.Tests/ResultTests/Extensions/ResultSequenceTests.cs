using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultSequenceTests
{
    /// <summary>
    /// 1. すべてのResultがOkの場合は、すべての成功値を保持するOkを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_ok_collection_when_all_results_are_ok()
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
    /// 2. Failを含む場合は、そのFailを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_fail_when_results_contain_fail()
    {
        var ok1 = Result<string, int>.Ok(1);
        var fail = Result<string, int>.Fail("error");
        var ok3 = Result<string, int>.Ok(3);

        var results = new[]
        {
            ok1,
            fail,
            ok3
        };

        var result = results.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 3. 複数のFailを含む場合は、最初のFailのErrorを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_first_error_when_results_contain_multiple_fails()
    {
        var ok = Result<string, int>.Ok(1);
        var firstFail = Result<string, int>.Fail("first error");
        var secondFail = Result<string, int>.Fail("second error");

        var results = new[]
        {
            ok,
            firstFail,
            secondFail
        };

        var result = results.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("first error"));
        });
    }

    /// <summary>
    /// 4. 空のシーケンスの場合は、空のコレクションを保持するOkを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_ok_empty_collection_when_results_are_empty()
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
    /// 5. resultsがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Sequence_should_throw_argument_null_exception_when_results_is_null()
    {
        IEnumerable<Result<string, int>> results = null!;

        Assert.Throws<ArgumentNullException>(() => results.Sequence());
    }

    /// <summary>
    /// 6. すべてのResultがOkの場合は、成功値の順序を保持する。
    /// </summary>
    [Test]
    public void Sequence_should_preserve_value_order()
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
    public void Sequence_should_throw_invalid_operation_exception_when_results_contain_uninitialized_result()
    {
        var ok1 = Result<string, int>.Ok(1);
        var uninitialized = default(Result<string, int>);
        var ok3 = Result<string, int>.Ok(3);

        var results = new[]
        {
            ok1,
            uninitialized,
            ok3
        };

        Assert.Throws<InvalidOperationException>(() => results.Sequence());
    }

    /// <summary>
    /// 8. Failより後ろのResultは列挙せず、最初のFailを返す。
    /// </summary>
    [Test]
    public void Sequence_should_return_first_fail_without_evaluating_results_after_fail()
    {
        var ok = Result<string, int>.Ok(1);
        var fail = Result<string, int>.Fail("error");
        var uninitialized = default(Result<string, int>);

        var results = new[]
        {
            ok,
            fail,
            uninitialized
        };

        var result = results.Sequence();

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }
}