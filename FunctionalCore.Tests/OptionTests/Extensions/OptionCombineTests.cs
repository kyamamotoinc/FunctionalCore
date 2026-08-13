using FunctionalCore.Extensions;

namespace FunctionalCore.Tests.OptionTests.Extensions;

public class OptionCombineTests
{
    private Option<int> _some3;
    private Option<int> _some5;
    private Option<int> _none;

    [SetUp]
    public void Setup()
    {
        _some3 = Option<int>.Some(3);
        _some5 = Option<int>.Some(5);
        _none = Option<int>.None;
    }

    /// <summary>
    /// 1. 両方が Some の場合は selector を実行し、組み合わせた値を持つ Some を返す
    /// </summary>
    [Test]
    public void Option_Some_Some_Combine_should_return_combined_value()
    {
        var result = _some3.Combine(_some5, (x, y) => x + y);

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo(8));
        });
    }

    /// <summary>
    /// 2. Combine は値の型を変更できる
    /// </summary>
    [Test]
    public void Option_Some_Some_Combine_should_change_value_type()
    {
        var result = _some3.Combine(_some5, (x, y) => $"{x}:{y}");

        Assert.Multiple(() =>
        {
            Assert.That(result.HasValue, Is.True);
            Assert.That(result.Value, Is.EqualTo("3:5"));
        });
    }

    /// <summary>
    /// 3. 両方が Some の場合は selector を1回だけ実行する
    /// </summary>
    [Test]
    public void Option_Some_Some_Combine_should_invoke_selector_once()
    {
        int count = 0;

        _some3.Combine(_some5, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(1));
    }

    /// <summary>
    /// 4. 1つ目が None の場合は None を返す
    /// </summary>
    [Test]
    public void Option_None_Some_Combine_should_return_none()
    {
        var result = _none.Combine(_some5, (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 5. 2つ目が None の場合は None を返す
    /// </summary>
    [Test]
    public void Option_Some_None_Combine_should_return_none()
    {
        var result = _some3.Combine(_none, (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 6. 両方が None の場合は None を返す
    /// </summary>
    [Test]
    public void Option_None_None_Combine_should_return_none()
    {
        var result = _none.Combine(_none, (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 7. 1つ目が None の場合は selector を実行しない
    /// </summary>
    [Test]
    public void Option_None_Some_Combine_should_not_invoke_selector()
    {
        int count = 0;

        _none.Combine(_some5, (x, y) =>
        {
            count++;
            return x + y;
        });

        Assert.That(count, Is.EqualTo(0));
    }

    /// <summary>
    /// 8. 2つ目が None の場合は selector を実行しない
    /// </summary>
    [Test]
    public void Option_Some_None_Combine_should_not_invoke_selector()
    {
        int count = 0;

        _some3.Combine(_none, (x, y) =>
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
    public void Option_Combine_null_selector_should_throw()
    {
        Func<int, int, int>? selector = null;

        Assert.Throws<ArgumentNullException>(() =>
            _some3.Combine(_some5, selector!));
    }

    /// <summary>
    /// 10. 両方が Some で selector が null を返した場合は None を返す
    /// </summary>
    [Test]
    public void Option_Some_Some_Combine_selector_returning_null_should_return_none()
    {
        var result = _some3.Combine(_some5, (_, _) => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 11. 1つ目が None の場合は null を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Option_None_Some_Combine_should_not_evaluate_null_returning_selector()
    {
        var result = _none.Combine(_some5, (_, _) => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 12. 2つ目が None の場合は null を返す selector でも実行されない
    /// </summary>
    [Test]
    public void Option_Some_None_Combine_should_not_evaluate_null_returning_selector()
    {
        var result = _some3.Combine(_none, (_, _) => (string)null!);

        Assert.That(result, Is.EqualTo(Option<string>.None));
    }

    /// <summary>
    /// 13. Default Option は None と同様に扱われる
    /// </summary>
    [Test]
    public void Option_Default_Some_Combine_should_return_none()
    {
        var option = default(Option<int>);

        var result = option.Combine(_some5, (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 14. 2つ目が Default Option の場合も None と同様に扱われる
    /// </summary>
    [Test]
    public void Option_Some_Default_Combine_should_return_none()
    {
        var option = default(Option<int>);

        var result = _some3.Combine(option, (x, y) => x + y);

        Assert.That(result, Is.EqualTo(Option<int>.None));
    }

    /// <summary>
    /// 15. 1つ目が None でも selector が null の場合は ArgumentNullException が発生する
    /// </summary>
    [Test]
    public void Option_None_Combine_null_selector_should_throw()
    {
        Func<int, int, int>? selector = null;

        Assert.Throws<ArgumentNullException>(() =>
            _none.Combine(_some5, selector!));
    }
}