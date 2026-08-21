using FunctionalCore.Linq;

namespace FunctionalCore.Tests.ResultTests.Linq;

public class ResultSelectTests
{
    /// <summary>
    /// 1. ResultがOkの場合はselectorを実行し、変換後の値を保持するOkを返す。
    /// </summary>
    [Test]
    public void Ok_Select_should_return_mapped_result()
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
    /// 2. ResultがOkの場合はselectorによって成功値の型を変更できる。
    /// </summary>
    [Test]
    public void Ok_Select_should_change_value_type()
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
    /// 3. ResultがOkの場合はselectorを1回だけ実行する。
    /// </summary>
    [Test]
    public void Ok_Select_should_invoke_selector_once()
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
    /// 4. ResultがFailの場合はselectorを実行しない。
    /// </summary>
    [Test]
    public void Fail_Select_should_not_invoke_selector()
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
    /// 5. ResultがFailの場合は元のErrorを保持する。
    /// </summary>
    [Test]
    public void Fail_Select_should_keep_original_error()
    {
        var fail = Result<string, int>.Fail("error");
        var result = fail.Select(x => x + 1);

        Assert.Multiple(() =>
        {
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.IsSuccess, Is.False);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 6. ResultがOkの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Select_should_throw_argument_null_exception_when_selector_is_null()
    {
        var ok = Result<string, int>.Ok(5);
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => ok.Select(selector!));
    }

    /// <summary>
    /// 7. ResultがFailの場合でもselectorがnullの場合はArgumentNullExceptionを発生させる。
    /// </summary>
    [Test]
    public void Fail_Select_should_throw_argument_null_exception_when_selector_is_null()
    {
        var fail = Result<string, int>.Fail("error");
        Func<int, string>? selector = null;

        Assert.Throws<ArgumentNullException>(() => fail.Select(selector!));
    }

    /// <summary>
    /// 8. Resultがdefaultでselectorもnullの場合は、
    /// Resultの未初期化を優先してInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_Select_should_throw_invalid_operation_exception_before_selector_null_check()
    {
        var uninitialized = default(Result<string, int>);
        Func<int, string>? selector = null;

        Assert.Throws<InvalidOperationException>(() => uninitialized.Select(selector!));
    }

    /// <summary>
    /// 9. ResultがOkでselectorがnullを返した場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Ok_Select_should_throw_invalid_operation_exception_when_selector_returns_null()
    {
        var ok = Result<string, int>.Ok(5);

        Assert.Throws<InvalidOperationException>(() => ok.Select(_ => (string)null!));
    }

    /// <summary>
    /// 10. ResultがFailの場合はselectorを実行せず、元のFailを返す。
    /// selectorがnullを返す関数でも実行されない。
    /// </summary>
    [Test]
    public void Fail_Select_should_return_original_fail_without_invoking_selector()
    {
        var fail = Result<string, int>.Fail("error");
        int count = 0;

        var result = fail.Select(_ =>
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
    /// 11. LINQクエリ構文のselectでSelectを利用できる。
    /// </summary>
    [Test]
    public void Select_should_support_query_syntax()
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
    /// 12. ResultがFailの場合でもLINQクエリ構文のselectは元のErrorを保持する。
    /// </summary>
    [Test]
    public void Fail_Select_query_syntax_should_keep_original_error()
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