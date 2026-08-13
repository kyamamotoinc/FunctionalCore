using FunctionalCore.AsyncExtensions;

namespace FunctionalCore.Tests.ResultTests.AsyncExtensions;

public class ResultAsTaskTests
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
    /// 1. Ok.AsTask は元の Ok を保持する完了済み Task を返す
    /// </summary>
    [Test]
    public async Task Result_Ok_AsTask_should_return_original_result()
    {
        var result = await _ok.AsTask();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(_ok));
            Assert.That(result.IsSuccess, Is.True);
            Assert.That(result.Value, Is.EqualTo(5));
        });
    }

    /// <summary>
    /// 2. Fail.AsTask は元の Fail を保持する完了済み Task を返す
    /// </summary>
    [Test]
    public async Task Result_Fail_AsTask_should_return_original_result()
    {
        var result = await _fail.AsTask();

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(_fail));
            Assert.That(result.IsFailure, Is.True);
            Assert.That(result.Error, Is.EqualTo("error"));
        });
    }

    /// <summary>
    /// 3. Ok.AsTask は完了済み Task を返す
    /// </summary>
    [Test]
    public void Result_Ok_AsTask_should_return_completed_task()
    {
        var task = _ok.AsTask();

        Assert.That(task.IsCompletedSuccessfully, Is.True);
    }

    /// <summary>
    /// 4. Fail.AsTask は完了済み Task を返す
    /// </summary>
    [Test]
    public void Result_Fail_AsTask_should_return_completed_task()
    {
        var task = _fail.AsTask();

        Assert.That(task.IsCompletedSuccessfully, Is.True);
    }

    /// <summary>
    /// 5. 未初期化 Result を AsTask すると InvalidOperationException が発生する
    /// </summary>
    [Test]
    public void Result_Default_AsTask_should_throw()
    {
        var result = default(Result<string, int>);

        Assert.Throws<InvalidOperationException>(() => result.AsTask());
    }
}