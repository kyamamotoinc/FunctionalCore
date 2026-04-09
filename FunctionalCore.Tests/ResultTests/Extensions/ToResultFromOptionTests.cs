using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;
public class ToResultFromOptionTests
{
    Option<int> _some;
    Option<int> _none;

    [SetUp]
    public void Setup()
    {
        _some = Option<int>.Some(5);
        _none = Option<int>.None;
    }

    [Test]
    public void Some_ToResult_should_return_Ok()
    {
        Assert.AreEqual(_some.ToResult("error"), Result<string, int>.Ok(5));
    }

    [Test]
    public void None_ToResult_should_return_Fail()
    {
        Assert.AreEqual(_none.ToResult("error"), Result<string, int>.Fail("error"));
    }

    [Test]
    public void None_ToResult_should_invoke_errorFactory()
    {
        var a = _none.ToResult<Func<string>, int>(() => "error");
        Assert.AreEqual(a, Result<string, int>.Fail("error"));
    }

    [Test]
    public void Some_ToResult_should_not_invoke_errorFactory()
    {

        Assert.AreEqual(_some.ToResult<Func<string>, int>(() => "error"), Result<string, int>.Ok(5));
    }
}
