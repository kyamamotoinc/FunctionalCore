namespace FunctionalCore.Linq;

/// <summary>
/// <para>Provides LINQ extension methods for Option&lt;T&gt;.</para>
/// <para>Option<T>に対するLINQ拡張メソッドを提供する。</para>
/// </summary>
public static class OptionLinq
{
    /// <summary>
    /// Maps the value to a new Option. Supports LINQ query syntax.
    /// <para>値をマップして新しいOptionを返す。LINQ構文対応。</para>
    ///
    /// If selector returns null, it is converted to None.
    /// <para>selector が null を返した場合は None に変換される。</para>
    /// </summary>
    /// <typeparam name="T">The type of the original value. / 元の値の型。</typeparam>
    /// <typeparam name="U">The type of the transformed value. / 変換後の値の型。</typeparam>
    /// <param name="option">The Option to transform. / 変換対象のOption。</param>
    /// <param name="selector">A function to transform the value. Returning null is converted to None. / 値を変換する関数。nullを返した場合はNoneに変換される。</param>
    /// <returns>
    /// An Option with the transformed value, or None.
    /// <para>変換後の値を持つOption、またはNone。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if selector is null. / selectorがnullの場合に投げられる。</exception>
    public static Option<U> Select<T, U>(this Option<T> option, Func<T, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return option.Map(selector);
    }

    /// <summary>
    /// Maps and flattens nested Options. Supports LINQ query syntax.
    /// <para>Optionを返す関数を適用しフラット化する。LINQ構文対応。</para>
    /// </summary>
    /// <typeparam name="T">The type of the original value. / 元の値の型。</typeparam>
    /// <typeparam name="U">The type of the value in the intermediate Option. / 中間Optionの値の型。</typeparam>
    /// <typeparam name="V">The type of the value in the final Option. / 最終Optionの値の型。</typeparam>
    /// <param name="option">The Option to transform. / 変換対象のOption。</param>
    /// <param name="selector">A function to transform the value into an intermediate Option. / 値を中間Optionに変換する関数。</param>
    /// <param name="projector">A function to project the final value from the original and intermediate values. / 元の値と中間値から最終値を生成する関数。</param>
    /// <returns>
    /// An Option with the projected value, or None.
    /// <para>投影された値を持つOption、またはNone。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if selector or projector is null. / selectorまたはprojectorがnullの場合に投げられる。</exception>
    public static Option<V> SelectMany<T, U, V>(this Option<T> option, Func<T, Option<U>> selector, Func<T, U, V> projector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(projector);

        return option.Bind(x => selector(x).Map(y => projector(x, y)));
    }

    /// <summary>
    /// Filters the value using a predicate. Converts to None if the predicate fails.
    /// <para>条件を満たさない場合は None に変換する。</para>
    ///
    /// This is equivalent to a validation step.
    /// <para>バリデーション用途。</para>
    /// </summary>
    /// <typeparam name="T">The type of the value. / 値の型。</typeparam>
    /// <param name="option">The Option to filter. / フィルタリング対象のOption。</param>
    /// <param name="predicate">A function to test the value. / 値を検証する関数。</param>
    /// <returns>
    /// The original Option if the predicate passes; otherwise None.
    /// <para>条件を満たす場合は元のOption、それ以外はNone。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if predicate is null. / predicateがnullの場合に投げられる。</exception>
    public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return option.Ensure(predicate);
    }
}
