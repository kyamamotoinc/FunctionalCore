namespace FunctionalCore.Tests;

public class ToResultFromOptionTests
{
    Option<int> some;
    Option<int> none;

    [SetUp]
    public void Setup()
    {
        some = Option<int>.Some(5);
        none = Option<int>.None;
    }

    [Test]
    public void Some_ToResult_should_return_Ok()
    {
        Assert.AreEqual(some.ToResultFromOption("error"), Result<string, int>.Ok(5));
    }

    [Test]
    public void None_ToResult_should_return_Fail()
    {
        Assert.AreEqual(none.ToResultFromOption("error"), Result<string, int>.Fail("error"));
    }

    [Test]
    public void None_ToResult_should_invoke_errorFactory()
    {
        Assert.AreEqual(none.ToResultFromOption(() => "error"), Result<string, int>.Fail("error"));
    }

    [Test]
    public void Some_ToResult_should_not_invoke_errorFactory()
    {
        Assert.AreEqual(some.ToResultFromOption(() => "error"), Result<string, int>.Ok(5));
    }
}
