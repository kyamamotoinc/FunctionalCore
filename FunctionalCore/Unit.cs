using System;

namespace FunctionalCore;

/// <summary>
/// Represents the Unit type, a type that has only one possible value.
/// Unit型を表します。この型はただ1つの値しか持ちません。
/// </summary>
/// <remarks>
/// Used in functional programming to represent the absence of a meaningful value,
/// similar to <c>void</c>, but as a first-class value.
/// 関数型プログラミングにおいて、意味のある値が存在しないことを表現するために使用されます。
/// C# の <c>void</c> に似ていますが、値として扱える点が異なります。
/// </remarks>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// The canonical Unit value.
    /// Unitの標準的な値を表します。
    /// </summary>
    public static readonly Unit Value = default;

    /// <summary>
    /// Determines whether the specified Unit is equal to the current Unit.
    /// 指定された Unit が現在の Unit と等しいかどうかを判定します。
    /// </summary>
    /// <param name="other">The Unit to compare with. 比較対象の Unit。</param>
    /// <returns>Always true. 常に true を返します。</returns>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Determines whether the specified object is equal to the current Unit.
    /// 指定されたオブジェクトが現在の Unit と等しいかどうかを判定します。
    /// </summary>
    /// <param name="obj">The object to compare. 比較対象のオブジェクト。</param>
    /// <returns>True if the object is a Unit; otherwise, false. オブジェクトが Unit の場合は true。</returns>
    public override bool Equals(object? obj) => obj is Unit;

    /// <summary>
    /// Returns the hash code for this instance.
    /// このインスタンスのハッシュコードを返します。
    /// </summary>
    /// <remarks>
    /// Since Unit has no state, all instances share the same hash code.
    /// Unit は状態を持たないため、すべてのインスタンスで同じハッシュコードを返します。
    /// </remarks>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Determines whether two Unit values are equal.
    /// 2つの Unit 値が等しいかどうかを判定します。
    /// </summary>
    /// <returns>Always true. 常に true を返します。</returns>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>
    /// Determines whether two Unit values are not equal.
    /// 2つの Unit 値が等しくないかどうかを判定します。
    /// </summary>
    /// <returns>Always false. 常に false を返します。</returns>
    public static bool operator !=(Unit left, Unit right) => false;

    /// <summary>
    /// Returns the string representation of Unit.
    /// Unit の文字列表現を返します。
    /// </summary>
    /// <returns>"()"</returns>
    public override string ToString() => "()";
}