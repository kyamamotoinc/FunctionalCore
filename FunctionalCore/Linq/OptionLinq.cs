namespace FunctionalCore.Linq;

/// <summary>
/// Provides LINQ query syntax support for <c>Option&lt;T&gt;</c>.
/// <para><c>Option&lt;T&gt;</c> を LINQ クエリ構文で使用するための拡張メソッドを提供する。</para>
/// </summary>
/// <remarks>
/// These methods delegate to the corresponding Option operations and preserve their semantics.
/// <para>
/// 各メソッドは対応する Option の操作へ処理を委譲し、その意味と契約を維持する。
/// </para>
/// </remarks>
public static class OptionLinq
{
    /// <summary>
    /// Transforms the contained value when the Option is Some.
    /// Supports LINQ query syntax.
    /// <para>
    /// Option が Some の場合に保持している値を変換する。
    /// LINQ クエリ構文に対応する。
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the original value.
    /// <para>変換前の値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the transformed value.
    /// <para>変換後の値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The Option to transform.
    /// <para>変換対象の Option。</para>
    /// </param>
    /// <param name="selector">
    /// A function that transforms the contained value.
    /// If it returns null, the result is converted to None.
    /// <para>
    /// 保持している値を変換する関数。
    /// null を返した場合は None に変換される。
    /// </para>
    /// </param>
    /// <returns>
    /// A Some containing the transformed value when <paramref name="option"/> is Some
    /// and <paramref name="selector"/> returns a non-null value;
    /// otherwise None.
    /// <para>
    /// <paramref name="option"/> が Some で、
    /// <paramref name="selector"/> が null ではない値を返した場合は、その値を保持する Some。
    /// それ以外は None。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> is null.
    /// <para><paramref name="selector"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Option<U> Select<T, U>(this Option<T> option, Func<T, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return option.Map(selector);
    }

    /// <summary>
    /// Connects an Option to another Option-producing operation and projects the values into a final result.
    /// Supports LINQ query syntax with multiple <c>from</c> clauses.
    /// <para>
    /// Option を次の Option を返す処理へ接続し、元の値と中間値から最終結果を生成する。
    /// 複数の <c>from</c> を使用する LINQ クエリ構文に対応する。
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the original value.
    /// <para>元の値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the intermediate value.
    /// <para>中間値の型。</para>
    /// </typeparam>
    /// <typeparam name="V">
    /// The type of the projected value.
    /// <para>最終的に生成される値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The source Option.
    /// <para>接続元の Option。</para>
    /// </param>
    /// <param name="selector">
    /// A function that transforms the original value into an intermediate Option.
    /// <para>元の値から中間 Option を生成する関数。</para>
    /// </param>
    /// <param name="projector">
    /// A function that combines the original value and intermediate value into the final value.
    /// If it returns null, the result is converted to None.
    /// <para>
    /// 元の値と中間値から最終値を生成する関数。
    /// null を返した場合は None に変換される。
    /// </para>
    /// </param>
    /// <returns>
    /// A Some containing the projected value when both the source and intermediate Options are Some
    /// and <paramref name="projector"/> returns a non-null value;
    /// otherwise None.
    /// <para>
    /// 元の Option と中間 Option の両方が Some で、
    /// <paramref name="projector"/> が null ではない値を返した場合は、その値を保持する Some。
    /// それ以外は None。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> or <paramref name="projector"/> is null.
    /// <para><paramref name="selector"/> または <paramref name="projector"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Option<V> SelectMany<T, U, V>(this Option<T> option, Func<T, Option<U>> selector, Func<T, U, V> projector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(projector);

        return option.Bind(x => selector(x).Map(y => projector(x, y)));
    }

    /// <summary>
    /// Filters the contained value using the specified predicate.
    /// Supports the <c>where</c> clause in LINQ query syntax.
    /// <para>
    /// 指定された条件で保持している値を絞り込む。
    /// LINQ クエリ構文の <c>where</c> 句に対応する。
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the contained value.
    /// <para>保持する値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The Option to filter.
    /// <para>絞り込み対象の Option。</para>
    /// </param>
    /// <param name="predicate">
    /// A function that determines whether the contained value should be preserved.
    /// <para>保持している値を残すかどうかを判定する関数。</para>
    /// </param>
    /// <returns>
    /// The original Option when it is None or when <paramref name="predicate"/> returns <see langword="true"/>;
    /// otherwise None.
    /// <para>
    /// 元の Option が None、または <paramref name="predicate"/> が <see langword="true"/> を返した場合は元の Option。
    /// 条件を満たさない場合は None。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicate"/> is null.
    /// <para><paramref name="predicate"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return option.Ensure(predicate);
    }
}
