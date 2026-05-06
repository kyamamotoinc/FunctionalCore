namespace FunctionalCore.Linq;

/// <summary>
/// Provides LINQ extension methods for Result<E, T>.
/// Result<E, T>に対するLINQ拡張メソッドを提供する。
/// </summary>
public static class ResultLinq
{

    /// <summary>
    /// Maps value to a new Result. Supports LINQ query syntax.
    /// 値をマップして新しいResultを返す。LINQ構文対応
    /// </summary>
    /// <typeparam name="E">The error type. / エラーの型。</typeparam>
    /// <typeparam name="T">The type of the original value. / 元の値の型。</typeparam>
    /// <typeparam name="U">The type of the transformed value. / 変換後の値の型。</typeparam>
    /// <param name="result">The Result to transform. / 変換対象のResult。</param>
    /// <param name="selector">A function to transform the value. Must not return null. / 値を変換する関数。nullを返してはならない。</param>
    /// <exception cref="ArgumentNullException">Thrown if selector is null. / selectorがnullの場合に投げられる。</exception>
    /// <exception cref="InvalidOperationException">Thrown if selector returns null. / selectorがnullを返した場合に投げられる。</exception>
    /// <returns>A Result with the transformed value, or the original error. / 変換後の値を持つResult、または元のエラー。</returns>
    public static Result<E, U> Select<E, T, U>(this Result<E, T> result, Func<T, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return result.Map(selector);
    }

    /// <summary>
    /// Maps and flattens nested Results. Supports LINQ query syntax.
    /// Resultを返す関数を適用しフラット化する。LINQ構文対応
    /// </summary>
    /// <typeparam name="E">The error type. / エラーの型。</typeparam>
    /// <typeparam name="T">The type of the original value. / 元の値の型。</typeparam>
    /// <typeparam name="U">The type of the value in the intermediate Result. / 中間Resultの値の型。</typeparam>
    /// <typeparam name="V">The type of the value in the final Result. / 最終Resultの値の型。</typeparam>
    /// <param name="result">The Result to transform. / 変換対象のResult。</param>
    /// <param name="selector">A function to transform the value into an intermediate Result. / 値を中間Resultに変換する関数。</param>
    /// <param name="projector">A function to project the final value from the original and intermediate values. / 元の値と中間値から最終値を生成する関数。</param>
    /// <exception cref="ArgumentNullException">Thrown if selector or projector is null. / selectorまたはprojectorがnullの場合に投げられる。</exception>
    /// <returns>A Result with the projected value, or the original error. / 投影された値を持つResult、または元のエラー。</returns>
    public static Result<E, V> SelectMany<E, T, U, V>(this Result<E, T> result, Func<T, Result<E, U>> selector, Func<T, U, V> projector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(projector);

        return result.Bind(x => selector(x).Map(y => projector(x, y)));
    }

    // Where is intentionally omitted.
    // Use result.Ensure(predicate, errorFactory) directly,
    // as Result requires an explicit error when the predicate fails.
}
