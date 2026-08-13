using FunctionalCore.Linq;

namespace FunctionalCore.Tests.ResultTests.Linq;

public class ResultSelectManyTests
{
    private Result<string, int> _ok;
    private Result<string, int> _fail;

    [SetUp]
    public void Setup()
    {
        _ok = Result<string, int>.Ok(5);
        _fail = Result<string, int>.Fail("error");
    }

    /// <summary>
    /// 1. 元の Result と中間 Result がともに Ok の場合は projector の結果を持つ Ok を返す
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_should_return_projected_value()
    {
        var result = _ok.SelectMany(
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
        var result = _ok.SelectMany(
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
        int count = 0;

        _ok.SelectMany(
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
        int count = 0;

        _fail.SelectMany(
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
        var result = _fail.SelectMany(
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
        var result = _ok.SelectMany(
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
        int count = 0;

        _ok.SelectMany(
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
        int count = 0;

        _ok.SelectMany(
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
        Func<int, Result<string, int>>? selector = null;

        Assert.Throws<ArgumentNullException>(() =>
            _ok.SelectMany(selector!, (x, y) => x + y));
    }

    /// <summary>
    /// 10. projector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_SelectMany_null_projector_should_throw()
    {
        Func<int, int, int>? projector = null;

        Assert.Throws<ArgumentNullException>(() =>
            _ok.SelectMany(x => Result<string, int>.Ok(x + 1), projector!));
    }

    /// <summary>
    /// 11. selector が未初期化 Result を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_selector_returning_uninitialized_result_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _ok.SelectMany(
                _ => default(Result<string, int>),
                (x, y) => x + y));
    }

    /// <summary>
    /// 12. projector が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_SelectMany_projector_returning_null_should_throw()
    {
        Assert.Throws<InvalidOperationException>(() =>
            _ok.SelectMany(
                x => Result<string, int>.Ok(x + 1),
                (_, _) => (string)null!));
    }

    /// <summary>
    /// 13. 元の Result が Fail の場合は未初期化 Result を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Result_Fail_SelectMany_should_not_evaluate_uninitialized_selector_result()
    {
        var result = _fail.SelectMany(
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
        var result = _ok.SelectMany(
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
        var result =
            from x in _ok
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
        var result =
            from x in _ok
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
        var result =
            from x in _fail
            from y in Result<string, int>.Ok(x + 1)
            select x + y;

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }
}