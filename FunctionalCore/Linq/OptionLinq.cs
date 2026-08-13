namespace FunctionalCore.Linq;

/// <summary>
/// Provides LINQ query syntax support for Option&lt;T&gt;.
/// <para>Option&lt;T&gt; に LINQ クエリ構文を提供する。</para>
/// </summary>
public static class OptionLinq
{
    /// <summary>
    /// Projects the value of an Option into a new form when the Option is Some.
    /// If the selector returns null, None is returned.
    /// <para>
    /// Option が Some の場合に値を新しい形式へ変換する。
    /// selector が null を返した場合は None を返す。
    /// </para>
    ///
    /// This method enables LINQ Select syntax.
    /// <para>LINQ の Select 構文を有効にする。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The source value type.
    /// <para>元の値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The projected value type.
    /// <para>変換後の値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The source Option.
    /// <para>元の Option。</para>
    /// </param>
    /// <param name="selector">
    /// A function that transforms the contained value.
    /// <para>保持している値を変換する関数。</para>
    /// </param>
    /// <returns>
    /// Some containing the projected value when the source is Some and the selector returns a non-null value;
    /// otherwise None.
    /// <para>
    /// 元の Option が Some で selector が null ではない値を返した場合は、
    /// 変換後の値を保持する Some。
    /// それ以外は None。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> is null.
    /// <para><paramref name="selector"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Option<U> Select<T, U>(this Option<T> option, Func<T, U> selector)
    {
        return option.Map(selector);
    }

    /// <summary>
    /// Projects the value into another Option and combines both values into a final Option.
    /// If the projector returns null, None is returned.
    /// <para>
    /// 値を別の Option に変換し、元の値と中間の値から最終的な Option を生成する。
    /// projector が null を返した場合は None を返す。
    /// </para>
    ///
    /// This method enables LINQ SelectMany syntax.
    /// <para>LINQ の SelectMany 構文を有効にする。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The source value type.
    /// <para>元の値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The intermediate value type.
    /// <para>中間の値の型。</para>
    /// </typeparam>
    /// <typeparam name="V">
    /// The final value type.
    /// <para>最終的な値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The source Option.
    /// <para>元の Option。</para>
    /// </param>
    /// <param name="selector">
    /// A function that transforms the contained value into another Option.
    /// <para>保持している値を別の Option に変換する関数。</para>
    /// </param>
    /// <param name="projector">
    /// A function that combines the original and intermediate values.
    /// <para>元の値と中間の値を組み合わせる関数。</para>
    /// </param>
    /// <returns>
    /// Some containing the projected value when all operations produce values and the projector returns a non-null value;
    /// otherwise None.
    /// <para>
    /// すべての処理で値が存在し、projector が null ではない値を返した場合は、
    /// 最終的な値を保持する Some。
    /// それ以外は None。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> or <paramref name="projector"/> is null.
    /// <para>
    /// <paramref name="selector"/> または <paramref name="projector"/> が null の場合にスローされる。
    /// </para>
    /// </exception>
    public static Option<V> SelectMany<T, U, V>(this Option<T> option, Func<T, Option<U>> selector, Func<T, U, V> projector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(projector);

        return option.Bind(t => selector(t).Map(u => projector(t, u)));
    }

    /// <summary>
    /// Filters the value of an Option using the specified predicate.
    /// <para>指定された条件を使用して Option の値を絞り込む。</para>
    ///
    /// This method enables LINQ where syntax.
    /// <para>LINQ の where 構文を有効にする。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The value type.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The source Option.
    /// <para>元の Option。</para>
    /// </param>
    /// <param name="predicate">
    /// A function that tests the contained value.
    /// <para>保持している値を検証する関数。</para>
    /// </param>
    /// <returns>
    /// The original Option when it is Some and the predicate returns true;
    /// otherwise None.
    /// <para>
    /// Some で条件を満たす場合は元の Option。
    /// それ以外は None。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicate"/> is null.
    /// <para><paramref name="predicate"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate)
    {
        return option.Ensure(predicate);
    }
}
