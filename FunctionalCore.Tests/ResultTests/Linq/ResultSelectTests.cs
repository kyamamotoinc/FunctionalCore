using FunctionalCore.Linq;

namespace FunctionalCore.Tests.ResultTests.Linq;

public class ResultSelectTests
{
    /// <summary>
    /// 1. Ok.Select は selector を実行し、変換後の値を持つ Ok を返す
    /// </summary>
    [Test]
    public void Result_Ok_Select_should_return_selector_result()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Select(x => x + 1);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 2. Ok.Select は成功値の型を変更できる
    /// </summary>
    [Test]
    public void Result_Ok_Select_should_change_value_type()
    {
        var ok = Result<string, int>.Ok(5);
        var result = ok.Select(x => $"value:{x}");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("value:5"));
        });
    }

    /// <summary>
    /// 3. Ok.Select は selector を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_Select_should_invoke_selector_once()
    {
        var ok = Result<string, int>.Ok(5);
        int count = 0;

        ok.Select(x =>
        {
            count++;
            return x + 1;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. Fail.Select は selector を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Select_should_not_invoke_selector()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        fail.Select(x =>
        {
            count++;
            return x + 1;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 5. Fail.Select は元の Error を保持する
    /// </summary>
    [Test]
    public void Result_Fail_Select_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Select(x => x + 1);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 6. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Select_null_selector_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => ok.Select(selector!));
    }

    /// <summary>
    /// 7. Ok.Select で selector が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_Select_selector_returning_null_should_throw()
    {
        var ok = Result<string, int>.Ok(5);
        Assert.Throws<InvalidOperationException>(() => ok.Select(_ => (string)null!));
    }

    /// <summary>
    /// 8. Fail.Select では null を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Result_Fail_Select_should_not_evaluate_null_returning_selector()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Select(_ => (string)null!);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 9. LINQ クエリ構文の select で Select が利用できる
    /// </summary>
    [Test]
    public void Result_Select_should_support_query_syntax()
    {
        var ok = Result<string, int>.Ok(5);
        var result =
            from x in ok
            select x + 1;

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(6));
        });
    }

    /// <summary>
    /// 10. Fail に対する LINQ クエリ構文の select は元の Error を保持する
    /// </summary>
    [Test]
    public void Result_Fail_Select_query_syntax_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");
        var result =
            from x in fail
            select x + 1;

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }
}