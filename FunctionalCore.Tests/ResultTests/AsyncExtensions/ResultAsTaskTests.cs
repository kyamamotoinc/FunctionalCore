using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultAsTaskTests
{
    /// <summary>
    /// 1. ResultがOkの場合は元のOkを保持するTaskを返す。
    /// </summary>
    [Test]
    public async Task Ok_AsTask_should_return_original_result()
    {
        var ok = Result<string, int>.Ok(5);

        var result = await ok.AsTask();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(ok));
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 2. ResultがFailの場合は元のFailを保持するTaskを返す。
    /// </summary>
    [Test]
    public async Task Fail_AsTask_should_return_original_result()
    {
        var fail = Result<string, int>.Fail("error");

        var result = await fail.AsTask();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(fail));
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 3. ResultがOkの場合は完了済みTaskを返す。
    /// </summary>
    [Test]
    public void Ok_AsTask_should_return_completed_task()
    {
        var ok = Result<string, int>.Ok(5);

        var task = ok.AsTask();

        Assert.That(task.IsCompletedSuccessfully, Is.True);
    }

    /// <summary>
    /// 4. ResultがFailの場合は完了済みTaskを返す。
    /// </summary>
    [Test]
    public void Fail_AsTask_should_return_completed_task()
    {
        var fail = Result<string, int>.Fail("error");

        var task = fail.AsTask();

        Assert.That(task.IsCompletedSuccessfully, Is.True);
    }

    /// <summary>
    /// 5. Resultがdefaultの場合はInvalidOperationExceptionを発生させる。
    /// </summary>
    [Test]
    public void Default_AsTask_should_throw_invalid_operation_exception()
    {
        var uninitialized = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => uninitialized.AsTask());
    }
}