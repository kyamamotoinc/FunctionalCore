using FunctionalCore.Linq;

namespace FunctionalCore.Tests.ResultTests.Linq;

public class ResultSelectManyTests
{
    /// <summary>
    /// 1. 元の Result と中間 Result がともに Ok の場合は projector の結果を持つ Ok を返す
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_should_return_projected_value()
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
    /// 2. SelectMany は最終的な成功値の型を変更できる
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_should_change_value_type()
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
    /// 3. Ok.SelectMany は selector を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_should_invoke_selector_once()
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
    /// 4. 元の Result が Fail の場合は selector を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_SelectMany_should_not_invoke_selector()
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
    /// 5. 元の Result が Fail の場合は元の Error を保持する
    /// </summary>
    [Test]
    public void Result_Fail_SelectMany_should_keep_original_error()
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
    /// 6. selector が Fail を返した場合はその Error を持つ Fail を返す
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_selector_returning_failure_should_return_failure()
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
    /// 7. selector が Fail を返した場合は projector を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_selector_failure_should_not_invoke_projector()
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
    /// 8. 両方が Ok の場合は projector を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_should_invoke_projector_once()
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
    /// 9. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_SelectMany_null_selector_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, Result<string, int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() =>
            ok.SelectMany(selector!, (x, y) => x + y));
    }

    /// <summary>
    /// 10. projector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_SelectMany_null_projector_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, int, int>? projector = null;

        Assert.Throws<ArgumentNullException>(() =>
            ok.SelectMany(x => Result<string, int>.Ok(x + 1), projector!));
    }

    /// <summary>
    /// 11. selector が未初期化 Result を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_selector_returning_uninitialized_result_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<InvalidOperationException>(() =>
            ok.SelectMany(
                _ => default(Result<string, int>),
                (x, y) => x + y));
    }

    /// <summary>
    /// 12. projector が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_projector_returning_null_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<InvalidOperationException>(() =>
            ok.SelectMany(
                x => Result<string, int>.Ok(x + 1),
                (_, _) => (string)null!));
    }

    /// <summary>
    /// 13. 元の Result が Fail の場合は未初期化 Result を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Result_Fail_SelectMany_should_not_evaluate_uninitialized_selector_result()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.SelectMany(
            _ => default(Result<string, int>),
            (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 14. selector が Fail を返した場合は null を返す projector でも実行されない
    /// </summary>
    [Test]
    public void Result_SelectMany_selector_failure_should_not_evaluate_null_returning_projector()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.SelectMany(
            _ => Result<string, int>.Fail("selector error"),
            (_, _) => (string)null!);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("selector error"));
        });
    }

    /// <summary>
    /// 15. LINQ クエリ構文の複数 from で SelectMany が利用できる
    /// </summary>
    [Test]
    public void Result_SelectMany_should_support_query_syntax()
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
    /// 16. LINQ クエリ構文で中間 Result が Fail の場合はその Error を保持する
    /// </summary>
    [Test]
    public void Result_SelectMany_query_syntax_intermediate_failure_should_return_failure()
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
    /// 17. LINQ クエリ構文で元の Result が Fail の場合は元の Error を保持する
    /// </summary>
    [Test]
    public void Result_SelectMany_query_syntax_source_failure_should_keep_original_error()
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