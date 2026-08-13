namespace FunctionalCore;

/// <summary>
/// Represents the Unit type, which has exactly one possible value.
/// <para>ただ1つの値だけを持つ Unit 型を表す。</para>
/// </summary>
/// <remarks>
/// Unit is used when an operation has no meaningful return value but a value is still required,
/// such as the success value of <c>Result&lt;E, Unit&gt;</c>.
/// <para>
/// Unit は、意味のある戻り値を持たない処理でも値が必要な場合に使用する。
/// 例えば <c>Result&lt;E, Unit&gt;</c> の成功値として使用できる。
/// </para>
///
/// Unlike <see langword="void"/>, Unit is a first-class value and can therefore be stored,
/// returned, and passed as an argument.
/// <para>
/// <see langword="void"/> とは異なり、Unit は値として扱えるため、
/// 保持、返却、引数としての受け渡しができる。
/// </para>
/// </remarks>
public readonly struct Unit : IEquatable<Unit>
{
    /// <summary>
    /// Gets the canonical Unit value.
    /// <para>Unit の標準値を取得する。</para>
    /// </summary>
    /// <value>
    /// The single meaningful Unit value.
    /// <para>Unit が持つ唯一の意味上の値。</para>
    /// </value>
    public static readonly Unit Value = default;

    /// <summary>
    /// Determines whether this Unit is equal to another Unit.
    /// <para>この Unit と指定された Unit が等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="other">
    /// The Unit to compare with this instance.
    /// <para>このインスタンスと比較する Unit。</para>
    /// </param>
    /// <returns>
    /// Always <see langword="true"/>.
    /// <para>常に <see langword="true"/> を返す。</para>
    /// </returns>
    public bool Equals(Unit other) => true;

    /// <summary>
    /// Determines whether this Unit is equal to the specified object.
    /// <para>この Unit と指定されたオブジェクトが等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="obj">
    /// The object to compare with this instance.
    /// <para>このインスタンスと比較するオブジェクト。</para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> when <paramref name="obj"/> is a Unit;
    /// otherwise <see langword="false"/>.
    /// <para>
    /// <paramref name="obj"/> が Unit の場合は <see langword="true"/>、
    /// それ以外は <see langword="false"/>。
    /// </para>
    /// </returns>
    public override bool Equals(object? obj) => obj is Unit;

    /// <summary>
    /// Returns the hash code for this Unit.
    /// <para>この Unit のハッシュコードを返す。</para>
    /// </summary>
    /// <returns>
    /// Always <c>0</c>.
    /// <para>常に <c>0</c> を返す。</para>
    /// </returns>
    /// <remarks>
    /// All Unit values are equal and therefore share the same hash code.
    /// <para>すべての Unit は等しいため、同じハッシュコードを返す。</para>
    /// </remarks>
    public override int GetHashCode() => 0;

    /// <summary>
    /// Determines whether two Unit values are equal.
    /// <para>2つの Unit が等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="left">
    /// The left Unit.
    /// <para>左辺の Unit。</para>
    /// </param>
    /// <param name="right">
    /// The right Unit.
    /// <para>右辺の Unit。</para>
    /// </param>
    /// <returns>
    /// Always <see langword="true"/>.
    /// <para>常に <see langword="true"/> を返す。</para>
    /// </returns>
    public static bool operator ==(Unit left, Unit right) => true;

    /// <summary>
    /// Determines whether two Unit values are not equal.
    /// <para>2つの Unit が等しくないかどうかを判定する。</para>
    /// </summary>
    /// <param name="left">
    /// The left Unit.
    /// <para>左辺の Unit。</para>
    /// </param>
    /// <param name="right">
    /// The right Unit.
    /// <para>右辺の Unit。</para>
    /// </param>
    /// <returns>
    /// Always <see langword="false"/>.
    /// <para>常に <see langword="false"/> を返す。</para>
    /// </returns>
    public static bool operator !=(Unit left, Unit right) => false;

    /// <summary>
    /// Returns the string representation of Unit.
    /// <para>Unit の文字列表現を返す。</para>
    /// </summary>
    /// <returns>
    /// <c>()</c>.
    /// <para><c>()</c> を返す。</para>
    /// </returns>
    public override string ToString() => "()";
}
