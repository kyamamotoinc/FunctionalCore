namespace FunctionalCore;

/// <summary>
/// Represents the Unit type, a type that has only one possible value.
/// <para>Unit型を表す。この型はただ1つの値しか持たない。</para>
/// </summary>
/// <remarks>
/// Used in functional programming to represent the absence of a meaningful value,
/// similar to <c>void</c>, but as a first-class value.
/// <para>
/// 関数型プログラミングにおいて、意味のある値が存在しないことを表現するために使用される。
/// C# の <c>void</c> に似ているが、値として扱える点が異なる。
/// </para>
/// </remarks>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// The canonical Unit value.
    /// <para>Unitの標準的な値を表す。</para>
    /// </summary>
    public static readonly Unit Value = default;

    /// <summary>
    /// Determines whether the specified Unit is equal to the current Unit.
    /// <para>指定された Unit が現在の Unit と等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="other">The Unit to compare with. 比較対象の Unit。</param>
    /// <returns>
    /// Always true.
    /// <para>常に true を返す。</para>
    /// </returns>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Determines whether the specified object is equal to the current Unit.
    /// <para>指定されたオブジェクトが現在の Unit と等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="obj">The object to compare. 比較対象のオブジェクト。</param>
    /// <returns>
    /// True if the object is a Unit; otherwise, false.
    /// <para>オブジェクトが Unit の場合は true。</para>
    /// </returns>
    public override bool Equals(object? obj) => obj is Unit;

    /// <summary>
    /// Returns the hash code for this instance.
    /// <para>このインスタンスのハッシュコードを返す。</para>
    /// </summary>
    /// <remarks>
    /// Since Unit has no state, all instances share the same hash code.
    /// Unit は状態を持たないため、すべてのインスタンスで同じハッシュコードを返す。
    /// </remarks>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Determines whether two Unit values are equal.
    /// <para>2つの Unit 値が等しいかどうかを判定する。</para>
    /// </summary>
    /// <returns>
    /// Always true.
    /// <para>常に true を返す。</para>
    /// </returns>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>
    /// Determines whether two Unit values are not equal.
    /// <para>2つの Unit 値が等しくないかどうかを判定する。</para> 
    /// </summary>
    /// <returns>
    /// Always false.
    /// <para>常に false を返す。</para>
    /// </returns>
    public static bool operator !=(Unit left, Unit right) => false;

    /// <summary>
    /// Returns the string representation of Unit.
    /// <para>Unit の文字列表現を返す。</para>
    /// </summary>
    /// <returns>"()"</returns>
    public override string ToString() => "()";
}
