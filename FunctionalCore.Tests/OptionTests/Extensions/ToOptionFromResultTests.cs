using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.ResultTests.Extensions;
public class ToOptionFromResultTests
{
    Result<string, int> _ok;
    Result<string, int> _fail;

    [SetUp]
    public void Setup()
    {
        _ok = Result<string, int>.Ok(5);
        _fail = Result<string, int>.Fail("error");
    }

    [Test]
    public void Some_ToOption_should_return_Some()
    {
        Assert.That(_ok.ToOption(), Is.EqualTo(Option<int>.Some(5)));
    }

    [Test]
    public void None_ToOption_should_return_None()
    {
        Assert.That(_fail.ToOption(), Is.EqualTo(Option<int>.None));
    }

    [Test]
    public void None_ToOption_should_invoke_errorFactory()
    {
        Assert.That(_fail.ToOption(), Is.EqualTo(Option<int>.None));
    }

    [Test]
    public void Some_ToOption_should_not_invoke_errorFactory()
    {

        Assert.That(_ok.ToOption(), Is.EqualTo(Option<int>.Some(5)));
    }
}
