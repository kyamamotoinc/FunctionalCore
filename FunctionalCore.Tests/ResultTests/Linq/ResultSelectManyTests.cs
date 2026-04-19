using FunctionalCore.Linq;

namespace FunctionalCore.Tests;

public class ResultSelectManyTests
{
    private Result<string, int> _ok1;
    private Result<string, string> _ok2;
    private Result<string, double> _ok3;
    private Result<string, int> _fail1;
    private Result<string, string> _fail2;
    private Result<string, double> _fail3;

    [SetUp]
    public void Setup()
    {
        _ok1 = Result<string, int>.Ok(5);
        _ok2 = Result<string, string>.Ok("hello");
        _ok3 = Result<string, double>.Ok(3.14);
        _fail1 = Result<string, int>.Fail("error1");
        _fail2 = Result<string, string>.Fail("error2");
        _fail3 = Result<string, double>.Fail("error3");
    }

    /// <summary>
    /// 1. 全て成功 → 最終結果が正しい値を持つ
    /// </summary>
    [Test]
    public void SelectMany_all_ok_should_return_success()
    {
        var result =
            from x in _ok1
            from y in _ok2
            from z in _ok3
            select $"{x}-{y}:{z}";

        Assert.That(result.IsSuccess, Is.True);
        Assert.That(result.Value, Is.EqualTo("5-hello:3.14"));
    }

    /// <summary>
    /// 2. 最初が失敗 → 失敗が伝播し後続はスキップされる
    /// </summary>
    [Test]
    public void SelectMany_first_fail_should_propagate_error()
    {
        var result =
            from x in _fail1
            from y in _ok2
            from z in _ok3
            select $"{x}-{y}:{z}";

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("error1"));
    }

    /// <summary>
    /// 3. 2番目が失敗 → 失敗が伝播し後続はスキップされる
    /// </summary>
    [Test]
    public void SelectMany_second_fail_should_propagate_error()
    {
        var result =
            from x in _ok1
            from y in _fail2
            from z in _ok3
            select $"{x}-{y}:{z}";

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("error2"));
    }

    /// <summary>
    /// 4. 3番目が失敗 → 失敗が伝播する
    /// </summary>
    [Test]
    public void SelectMany_third_fail_should_propagate_error()
    {
        var result =
            from x in _ok1
            from y in _ok2
            from z in _fail3
            select $"{x}-{y}:{z}";

        Assert.That(result.IsSuccess, Is.False);
        Assert.That(result.Error, Is.EqualTo("error3"));
    }

    /// <summary>
    /// 5. selector が null → ArgumentNullException
    /// </summary>
    [Test]
    public void SelectMany_null_selector_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _ok1.SelectMany(
                (Func<int, Result<string, string>>)null!,
                (x, y) => $"{x}-{y}"
            ));
    }

    /// <summary>
    /// 6. projector が null → ArgumentNullException
    /// </summary>
    [Test]
    public void SelectMany_null_projector_should_throw()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _ok1.SelectMany(
                x => _ok2,
                (Func<int, string, string>)null!
            ));
    }

    /// <summary>
    /// 7. 未初期化の Result → InvalidOperationException
    /// </summary>
    [Test]
    public void SelectMany_uninitialized_result_should_throw()
    {
        var uninitialized = default(Result<string, int>);
        Assert.Throws<InvalidOperationException>(() =>
        {
            var _ =
                from x in uninitialized
                from y in _ok2
                select $"{x}-{y}";
        });
    }
}
