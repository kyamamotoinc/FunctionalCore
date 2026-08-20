using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;

public class ResultCombineTests
{
    /// <summary>
    /// 1. 両方が Ok の場合は selector を実行し、組み合わせた値を持つ Ok を返す
    /// </summary>
    [Test]
    public void Result_Ok_Ok_Combine_should_return_combined_value()
    {
        var ok3 = Result<string, int>.Ok(3);
        var ok5 = Result<string, int>.Ok(5);
        var result = ok3.Combine(ok5, (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.IsFailure, Is.False);
            Assert.That(result.Value, Is.EqualTo(8));
        });
    }

    /// <summary>
    /// 2. Combine は成功値の型を変更できる
    /// </summary>
    [Test]
    public void Result_Ok_Ok_Combine_should_change_value_type()
    {
        var ok3 = Result<string, int>.Ok(3);
        var ok5 = Result<string, int>.Ok(5);
        var result = ok3.Combine(ok5, (x, y) => $"{x}:{y}");

        Assert.Multiple(() =>
        {
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo("3:5"));
        });
    }

    /// <summary>
    /// 3. 両方が Ok の場合は selector を1回だけ実行する
    /// </summary>
    [Test]
    public void Result_Ok_Ok_Combine_should_invoke_selector_once()
    {
        var ok3 = Result<string, int>.Ok(3);
        var ok5 = Result<string, int>.Ok(5);
        int count = 0;

        ok3.Combine(ok5, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. 1つ目が Fail の場合は1つ目の Error を返す
    /// </summary>
    [Test]
    public void Result_Fail_Ok_Combine_should_return_first_error()
    {
        var fail = Result<string, int>.Fail("error");
        var ok5 = Result<string, int>.Ok(5);
        var result = fail.Combine(ok5, (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 5. 2つ目が Fail の場合は2つ目の Error を返す
    /// </summary>
    [Test]
    public void Result_Ok_Fail_Combine_should_return_second_error()
    {
        var ok3 = Result<string, int>.Ok(3);
        var fail = Result<string, int>.Fail("error");
        var result = ok3.Combine(fail, (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 6. 両方が Fail の場合は1つ目の Error を優先する
    /// </summary>
    [Test]
    public void Result_Fail_Fail_Combine_should_return_first_error()
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
    /// 7. 1つ目が Fail の場合は selector を実行しない
    /// </summary>
    [Test]
    public void Result_Fail_Ok_Combine_should_not_invoke_selector()
    {
        var fail = Result<string, int>.Fail("error");
        var ok5 = Result<string, int>.Ok(5);
        int count = 0;

        fail.Combine(ok5, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 8. 2つ目が Fail の場合は selector を実行しない
    /// </summary>
    [Test]
    public void Result_Ok_Fail_Combine_should_not_invoke_selector()
    {
        var ok3 = Result<string, int>.Ok(3);
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        ok3.Combine(fail, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 9. selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Result_Combine_null_selector_should_throw()
    {
        var ok3 = Result<string, int>.Ok(3);
        var ok5 = Result<string, int>.Ok(5);
        Func<int, int, int>? selector = null;

        Assert.Throws<ArgumentNullException>(() => ok3.Combine(ok5, selector!));
    }

    /// <summary>
    /// 10. selector が null を返した場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Ok_Ok_Combine_selector_returning_null_should_throw()
    {
        var ok3 = Result<string, int>.Ok(3);
        var ok5 = Result<string, int>.Ok(5);
        Assert.Throws<InvalidOperationException>(() => ok3.Combine(ok5, (_, _) => (string)null!));
    }

    /// <summary>
    /// 11. 1つ目が Fail の場合は null を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Result_Fail_Ok_Combine_should_not_evaluate_null_returning_selector()
    {
        var fail = Result<string, int>.Fail("error");
        var ok5 = Result<string, int>.Ok(5);
        var result = fail.Combine(ok5, (_, _) => (string)null!);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 12. 2つ目が Fail の場合は null を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Result_Ok_Fail_Combine_should_not_evaluate_null_returning_selector()
    {
        var ok3 = Result<string, int>.Ok(3);
        var fail = Result<string, int>.Fail("error");
        var result = ok3.Combine(fail, (_, _) => (string)null!);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 13. 1つ目の Result が未初期化の場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Combine_uninitialized_first_result_should_throw()
    {
        var ok3 = Result<string, int>.Ok(3);
        var first = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => first.Combine(ok3, (x, y) => x + y));
    }

    /// <summary>
    /// 14. 2つ目の Result が未初期化の場合は InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Combine_uninitialized_second_result_should_throw()
    {
        var ok3 = Result<string, int>.Ok(3);
        var second = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => ok3.Combine(second, (x, y) => x + y));
    }
}