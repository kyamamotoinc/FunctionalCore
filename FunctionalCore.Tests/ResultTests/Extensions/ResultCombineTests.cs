using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultCombineTests
{
    /// <summary>
    /// 1. 両方のResultがOkの場合はselectorを実行し、組み合わせた値を保持するOkを返す。
    /// </summary>
    [Test]
    public void Ok_Ok_Combine_should_return_combined_result()
    {
        var first = Result<string, int>.Ok(3);
        var second = Result<string, int>.Ok(5);

        var result = first.Combine(second, (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(8));
        });
    }

    /// <summary>
    /// 2. 両方のResultがOkの場合はselectorによって成功値の型を変更できる。
    /// </summary>
    [Test]
    public void Ok_Ok_Combine_should_change_value_type()
    {
        var first = Result<string, int>.Ok(3);
        var second = Result<string, int>.Ok(5);

        var result = first.Combine(second, (x, y) => $"{x}:{y}");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("3:5"));
        });
    }

    /// <summary>
    /// 3. 両方のResultがOkの場合はselectorを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_Ok_Combine_should_invoke_selector_once()
    {
        var first = Result<string, int>.Ok(3);
        var second = Result<string, int>.Ok(5);
        int count = 0;

        first.Combine(second, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. 1つ目のResultがFailの場合は1つ目のErrorを返す。
    /// </summary>
    [Test]
    public void Fail_Ok_Combine_should_return_first_error()
    {
        var first = Result<string, int>.Fail("first error");
        var second = Result<string, int>.Ok(5);

        var result = first.Combine(second, (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("first error"));
        });
    }

    /// <summary>
    /// 5. 2つ目のResultがFailの場合は2つ目のErrorを返す。
    /// </summary>
    [Test]
    public void Ok_Fail_Combine_should_return_second_error()
    {
        var first = Result<string, int>.Ok(3);
        var second = Result<string, int>.Fail("second error");

        var result = first.Combine(second, (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("second error"));
        });
    }

    /// <summary>
    /// 6. 両方のResultがFailの場合は1つ目のErrorを優先して返す。
    /// </summary>
    [Test]
    public void Fail_Fail_Combine_should_return_first_error()
    {
        var first = Result<string, int>.Fail("first error");
        var second = Result<string, int>.Fail("second error");

        var result = first.Combine(second, (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("first error"));
        });
    }

    /// <summary>
    /// 7. 1つ目のResultがFailの場合はselectorを実行しない。
    /// </summary>
    [Test]
    public void Fail_Ok_Combine_should_not_invoke_selector()
    {
        var first = Result<string, int>.Fail("error");
        var second = Result<string, int>.Ok(5);
        int count = 0;

        first.Combine(second, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 8. 2つ目のResultがFailの場合はselectorを実行しない。
    /// </summary>
    [Test]
    public void Ok_Fail_Combine_should_not_invoke_selector()
    {
        var first = Result<string, int>.Ok(3);
        var second = Result<string, int>.Fail("error");
        int count = 0;

        first.Combine(second, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 9. 両方のResultがOkの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Ok_Combine_should_throw_argument_null_exception_when_selector_is_null()
    {
        var first = Result<string, int>.Ok(3);
        var second = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => first.Combine(second, (Func<int, int, int>)null!));
    }

    /// <summary>
    /// 10. 1つ目のResultがFailの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Ok_Combine_should_throw_argument_null_exception_when_selector_is_null()
    {
        var first = Result<string, int>.Fail("error");
        var second = Result<string, int>.Ok(5);

        Assert.Throws<ArgumentNullException>(() => first.Combine(second, (Func<int, int, int>)null!));
    }

    /// <summary>
    /// 11. 2つ目のResultがFailの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Fail_Combine_should_throw_argument_null_exception_when_selector_is_null()
    {
        var first = Result<string, int>.Ok(3);
        var second = Result<string, int>.Fail("error");

        Assert.Throws<ArgumentNullException>(() => first.Combine(second, (Func<int, int, int>)null!));
    }

    /// <summary>
    /// 12. 1つ目のResultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Ok_Combine_should_throw_invalid_operation_exception()
    {
        var first = default(Result<string, int>);
        var second = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() => first.Combine(second, (x, y) => x + y));
    }

    /// <summary>
    /// 13. 2つ目のResultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Default_Combine_should_throw_invalid_operation_exception()
    {
        var first = Result<string, int>.Ok(3);
        var second = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => first.Combine(second, (x, y) => x + y));
    }

    /// <summary>
    /// 14. 1つ目のResultがdefaultでselectorもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Ok_Combine_should_throw_invalid_operation_exception_before_selector_null_check()
    {
        var first = default(Result<string, int>);
        var second = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() => first.Combine(second, (Func<int, int, int>)null!));
    }

    /// <summary>
    /// 15. 2つ目のResultがdefaultでselectorもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Default_Combine_should_throw_invalid_operation_exception_before_selector_null_check()
    {
        var first = Result<string, int>.Ok(3);
        var second = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => first.Combine(second, (Func<int, int, int>)null!));
    }

    /// <summary>
    /// 16. 両方のResultがOkでselectorがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Ok_Combine_should_throw_invalid_operation_exception_when_selector_returns_null()
    {
        var first = Result<string, int>.Ok(3);
        var second = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() => first.Combine(second, (_, _) => (string)null!));
    }

    /// <summary>
    /// 17. 1つ目のResultがFailの場合はnullを返すselectorでも実行せず、1つ目のFailを返す。
    /// </summary>
    [Test]
    public void Fail_Ok_Combine_should_return_first_fail_without_invoking_null_returning_selector()
    {
        var first = Result<string, int>.Fail("error");
        var second = Result<string, int>.Ok(5);
        int count = 0;

        var result = first.Combine(second, (_, _) =>
        {
            count++;
            return (string)null!;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 18. 2つ目のResultがFailの場合はnullを返すselectorでも実行せず、2つ目のFailを返す。
    /// </summary>
    [Test]
    public void Ok_Fail_Combine_should_return_second_fail_without_invoking_null_returning_selector()
    {
        var first = Result<string, int>.Ok(3);
        var second = Result<string, int>.Fail("error");
        int count = 0;

        var result = first.Combine(second, (_, _) =>
        {
            count++;
            return (string)null!;
        });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 19. 両方のResultがOkでselectorが例外を発生させた場合は、その例外をそのまま伝播させる。
    /// </summary>
    [Test]
    public void Ok_Ok_Combine_should_propagate_exception_when_selector_throws()
    {
        var first = Result<string, int>.Ok(3);
        var second = Result<string, int>.Ok(5);
        var expectedException = new NotSupportedException("selector error");
        Func<int, int, int> selector = (_, _) => throw expectedException;

        var actualException = Assert.Throws<NotSupportedException>(() =>
            first.Combine(second, selector));

        Assert.That(actualException, Is.SameAs(expectedException));
    }
}