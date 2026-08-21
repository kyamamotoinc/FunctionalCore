namespace FunctionalCore.Tests.UnitTests;

/// <summary>
/// Tests for Unit.
/// <para>Unit のテスト。</para>
/// </summary>
public class UnitTests
{
    /// <summary>
    /// 1. 2つのUnitは等しい。
    /// </summary>
    [Test]
    public void Unit_Equals_should_return_true_when_other_is_unit()
    {
        var left = Unit.Value;
        var right = Unit.Value;

        Assert.That(left.Equals(right), Is.True);
    }

    /// <summary>
    /// 2. objectとして比較した2つのUnitも等しい。
    /// </summary>
    [Test]
    public void Unit_Equals_object_should_return_true_when_other_is_unit()
    {
        var unit = Unit.Value;
        object other = Unit.Value;

        Assert.That(unit.Equals(other), Is.True);
    }

    /// <summary>
    /// 3. objectとして異なる型と比較した場合はfalseを返す。
    /// </summary>
    [Test]
    public void Unit_Equals_object_should_return_false_when_other_is_different_type()
    {
        var unit = Unit.Value;
        object other = new();

        Assert.That(unit.Equals(other), Is.False);
    }

    /// <summary>
    /// 4. Unitのハッシュコードは常に0を返す。
    /// </summary>
    [Test]
    public void Unit_GetHashCode_should_return_zero()
    {
        var unit = Unit.Value;

        Assert.That(unit.GetHashCode(), Is.EqualTo(0));
    }

    /// <summary>
    /// 5. ==演算子で2つのUnitを比較した場合はtrueを返す。
    /// </summary>
    [Test]
    public void Unit_Equality_operator_should_return_true()
    {
        var left = Unit.Value;
        var right = Unit.Value;

        Assert.That(left == right, Is.True);
    }

    /// <summary>
    /// 6. !=演算子で2つのUnitを比較した場合はfalseを返す。
    /// </summary>
    [Test]
    public void Unit_Inequality_operator_should_return_false()
    {
        var left = Unit.Value;
        var right = Unit.Value;

        Assert.That(left != right, Is.False);
    }

    /// <summary>
    /// 7. UnitのToStringは"()"を返す。
    /// </summary>
    [Test]
    public void Unit_ToString_should_return_parentheses()
    {
        var unit = Unit.Value;

        Assert.That(unit.ToString(), Is.EqualTo("()"));
    }

    /// <summary>
    /// 8. default(Unit)はUnit.Valueと等しい。
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