using FunctionalCore.Linq;

namespace FunctionalCore.Tests.ResultTests.Linq;

public class ResultSelectManyTests
{
    /// <summary>
    /// 1. 元のResultとselectorが返すResultがともにOkの場合はprojectorの戻り値を保持するOkを返す。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_return_projected_result()
    {
        var ok = Result<string, int>.Ok(5);

        var result = ok.SelectMany(
            x => Result<string, int>.Ok(x + 1),
            (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(11));
        });
    }

    /// <summary>
    /// 2. SelectManyはprojectorによって最終的な成功値の型を変更できる。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_change_value_type()
    {
        var ok = Result<string, int>.Ok(5);

        var result = ok.SelectMany(
            x => Result<string, int>.Ok(x + 1),
            (x, y) => $"{x}:{y}");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("5:6"));
        });
    }

    /// <summary>
    /// 3. ResultがOkの場合はselectorを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_invoke_selector_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.SelectMany(
            x =>
            {
                count++;
                return Result<string, int>.Ok(x + 1);
            },
            (x, y) => x + y);

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. 元のResultがFailの場合はselectorを実行しない。
    /// </summary>
    [Test]
    public void Fail_SelectMany_should_not_invoke_selector()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.SelectMany(
            x =>
            {
                count++;
                return Result<string, int>.Ok(x + 1);
            },
            (x, y) => x + y);

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. 元のResultがFailの場合は元のErrorを保持する。
    /// </summary>
    [Test]
    public void Fail_SelectMany_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");

        var result = fail.SelectMany(
            x => Result<string, int>.Ok(x + 1),
            (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 6. selectorがFailを返した場合は、そのFailを返す。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_return_fail_when_selector_returns_fail()
    {
        var ok = Result<string, int>.Ok(5);

        var result = ok.SelectMany(
            _ => Result<string, int>.Fail("selector error"),
            (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("selector error"));
        });
    }

    /// <summary>
    /// 7. selectorがFailを返した場合はprojectorを実行しない。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_not_invoke_projector_when_selector_returns_fail()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.SelectMany(
            _ => Result<string, int>.Fail("selector error"),
            (x, y) =>
            {
                count++;
                return x + y;
            });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 8. 元のResultとselectorが返すResultがともにOkの場合はprojectorを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_invoke_projector_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.SelectMany(
            x => Result<string, int>.Ok(x + 1),
            (x, y) =>
            {
                count++;
                return x + y;
            });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 9. ResultがOkの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_throw_argument_null_exception_when_selector_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Result<string, int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() => ok.SelectMany(selector!, (x, y) => x + y));
    }

    /// <summary>
    /// 10. ResultがFailの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_SelectMany_should_throw_argument_null_exception_when_selector_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Func<int, Result<string, int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() => fail.SelectMany(selector!, (x, y) => x + y));
    }

    /// <summary>
    /// 11. ResultがOkの場合でもprojectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_throw_argument_null_exception_when_projector_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, int, int>? projector = null;

        Assert.Throws<ArgumentNullException>(() => ok.SelectMany(x => Result<string, int>.Ok(x + 1), projector!));
    }

    /// <summary>
    /// 12. ResultがFailの場合でもprojectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_SelectMany_should_throw_argument_null_exception_when_projector_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Func<int, int, int>? projector = null;

        Assert.Throws<ArgumentNullException>(() => fail.SelectMany(x => Result<string, int>.Ok(x + 1), projector!));
    }

    /// <summary>
    /// 13. Resultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_SelectMany_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() =>
            uninitialized.SelectMany(x => Result<string, int>.Ok(x + 1), (x, y) => x + y));
    }

    /// <summary>
    /// 14. Resultがdefaultでselectorもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_SelectMany_should_throw_invalid_operation_exception_before_selector_null_check()
    {
        var uninitialized = default(Result<string, int>);
        Func<int, Result<string, int>>? selector = null;

        Assert.Throws<InvalidOperationException>(() =>
            uninitialized.SelectMany(selector!, (x, y) => x + y));
    }

    /// <summary>
    /// 15. Resultがdefaultでprojectorもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_SelectMany_should_throw_invalid_operation_exception_before_projector_null_check()
    {
        var uninitialized = default(Result<string, int>);
        Func<int, int, int>? projector = null;

        Assert.Throws<InvalidOperationException>(() =>
            uninitialized.SelectMany(x => Result<string, int>.Ok(x + 1), projector!));
    }

    /// <summary>
    /// 16. selectorが未初期化Resultを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_throw_invalid_operation_exception_when_selector_returns_uninitialized_result()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() =>
            ok.SelectMany(
                _ => default(Result<string, int>),
                (x, y) => x + y));
    }

    /// <summary>
    /// 17. projectorがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_throw_invalid_operation_exception_when_projector_returns_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() =>
            ok.SelectMany(
                x => Result<string, int>.Ok(x + 1),
                (_, _) => (string)null!));
    }

    /// <summary>
    /// 18. 元のResultがFailの場合はselectorを実行せず、元のFailを返す。
    /// selectorが未初期化Resultを返す関数でも実行されない。
    /// </summary>
    [Test]
    public void Fail_SelectMany_should_return_original_fail_without_invoking_selector()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = fail.SelectMany(
            _ =>
            {
                count++;
                return default(Result<string, int>);
            },
            (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 19. selectorがFailを返した場合はprojectorを実行せず、そのFailを返す。
    /// projectorがnullを返す関数でも実行されない。
    /// </summary>
    [Test]
    public void Ok_SelectMany_should_return_selector_fail_without_invoking_projector()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        var result = ok.SelectMany(
            _ => Result<string, int>.Fail("selector error"),
            (_, _) =>
            {
                count++;
                return (string)null!;
            });

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("selector error"));
            Assert.That(count, Is.EqualTo(0));
        });
    }

    /// <summary>
    /// 20. LINQクエリ構文の複数fromでSelectManyを利用できる。
    /// </summary>
    [Test]
    public void SelectMany_should_support_query_syntax()
    {
        var ok = Result<string, int>.Ok(5);

        var result =
            from x in ok
            from y in Result<string, int>.Ok(x + 1)
            select x + y;

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(11));
        });
    }

    /// <summary>
    /// 21. LINQクエリ構文で中間ResultがFailの場合は、そのErrorを保持する。
    /// </summary>
    [Test]
    public void SelectMany_query_syntax_should_return_intermediate_fail()
    {
        var ok = Result<string, int>.Ok(5);

        var result =
            from x in ok
            from y in Result<string, int>.Fail("selector error")
            select x + y;

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("selector error"));
        });
    }

    /// <summary>
    /// 22. LINQクエリ構文で元のResultがFailの場合は元のErrorを保持する。
    /// </summary>
    [Test]
    public void Fail_SelectMany_query_syntax_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");

        var result =
            from x in fail
            from y in Result<string, int>.Ok(x + 1)
            select x + y;

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }
}