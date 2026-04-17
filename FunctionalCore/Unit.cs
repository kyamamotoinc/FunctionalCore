using System;

namespace FunctionalCore;

/// <summary>
/// Represents the Unit type, a type that has only one possible value.
/// </summary>
/// <remarks>
/// Used in functional programming to represent the absence of a meaningful value,
/// similar to <c>void</c>, but as a first-class value.
/// </remarks>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// The canonical Unit value.
    /// </summary>
    public static readonly Unit Value = default;

    /// <summary>
    /// Determines whether the specified Unit is equal to the current Unit.
    /// </summary>
    /// <param name="other">The Unit to compare with. 比較対象の Unit。</param>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Determines whether the specified object is equal to the current Unit.
    /// </summary>
    /// <param name="obj">The object to compare. 比較対象のオブジェクト。</param>
    /// <returns>True if the object is a Unit; otherwise, false. オブジェクトが Unit の場合は true。</returns>
    public override bool Equals(object? obj) => obj is Unit;

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <remarks>
    /// Since Unit has no state, all instances share the same hash code.
    /// </remarks>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Determines whether two Unit values are equal.
    /// </summary>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>
    /// Determines whether two Unit values are not equal.
    /// </summary>
    public static bool operator !=(Unit left, Unit right) => false;

    /// <summary>
    /// Returns the string representation of Unit.
    /// </summary>
    /// <returns>"()"</returns>
    public override string ToString() => "()";
}