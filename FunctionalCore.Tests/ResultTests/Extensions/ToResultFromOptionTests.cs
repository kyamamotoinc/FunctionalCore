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
        Assert.That(_some.ToResult("error"), Is.EqualTo(Result<string, int>.Ok(5)));
    }

    [Test]
    public void None_ToResult_should_return_Fail()
    {
        Assert.That(_none.ToResult("error"), Is.EqualTo(Result<string, int>.Fail("error")));
    }

    [Test]
    public void None_ToResult_should_invoke_errorFactory()
    {
        Assert.That(_none.ToResult(() => "error"), Is.EqualTo(Result<string, int>.Fail("error")));
    }

    [Test]
    public void Some_ToResult_should_not_invoke_errorFactory()
    {

        Assert.That(_some.ToResult(() => "error"), Is.EqualTo(Result<string, int>.Ok(5)));
    }
}
