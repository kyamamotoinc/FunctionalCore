namespace FunctionalCore.Tests.UnitTests;

/// <summary>
/// Tests for Unit.
/// <para>Unit のテスト。</para>
/// </summary>
public class UnitTests
{
    /// <summary>
    /// 1. Unit 同士は等しい
    /// </summary>
    [Test]
    public void Unit_Equals_should_return_true()
    {
        var left = Unit.Value;
        var right = Unit.Value;

        Assert.That(left.Equals(right), Is.True);
    }

    /// <summary>
    /// 2. object として比較した Unit 同士も等しい
    /// </summary>
    [Test]
    public void Unit_Equals_object_should_return_true()
    {
        var unit = Unit.Value;
        object other = Unit.Value;

        Assert.That(unit.Equals(other), Is.True);
    }

    /// <summary>
    /// 3. Unit 以外の object とは等しくない
    /// </summary>
    [Test]
    public void Unit_Equals_non_unit_object_should_return_false()
    {
        var unit = Unit.Value;
        object other = new();

        Assert.That(unit.Equals(other), Is.False);
    }

    /// <summary>
    /// 4. Unit のハッシュコードは常に 0 を返す
    /// </summary>
    [Test]
    public void Unit_GetHashCode_should_return_zero()
    {
        var unit = Unit.Value;

        Assert.That(unit.GetHashCode(), Is.EqualTo(0));
    }

    /// <summary>
    /// 5. == 演算子では Unit 同士は等しい
    /// </summary>
    [Test]
    public void Unit_Equality_operator_should_return_true()
    {
        var left = Unit.Value;
        var right = Unit.Value;

        Assert.That(left == right, Is.True);
    }

    /// <summary>
    /// 6. != 演算子では Unit 同士は等しくないとは判定されない
    /// </summary>
    [Test]
    public void Unit_Inequality_operator_should_return_false()
    {
        var left = Unit.Value;
        var right = Unit.Value;

        Assert.That(left != right, Is.False);
    }

    /// <summary>
    /// 7. ToString は "()" を返す
    /// </summary>
    [Test]
    public void Unit_ToString_should_return_parentheses()
    {
        var unit = Unit.Value;

        Assert.That(unit.ToString(), Is.EqualTo("()"));
    }

    /// <summary>
    /// 8. default(Unit) は Unit.Value と等しい
    /// </summary>
    [Test]
    public void Default_Unit_should_equal_Unit_Value()
    {
        var unit = default(Unit);

        Assert.Multiple(() =>
        {
            Assert.That(unit, Is.EqualTo(Unit.Value));
            Assert.That(unit == Unit.Value, Is.True);
            Assert.That(unit.GetHashCode(), Is.EqualTo(Unit.Value.GetHashCode()));
        });
    }
}