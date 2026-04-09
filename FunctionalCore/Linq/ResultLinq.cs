namespace FunctionalCore.Linq;

public static class ResultLinq
{
    /// <summary>
    /// Maps value to a new Result. Supports LINQ query syntax.
    /// 値をマップして新しいResultを返す。LINQ構文対応
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static Result<E, U> Select<E, T, U>(this Result<E, T> result, Func<T, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return result.Map(selector);
    }

    /// <summary>
    /// Maps and flattens nested Results. Supports LINQ query syntax.
    /// Resultを返す関数を適用しフラット化する。LINQ構文対応
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="selector"></param>
    /// <param name="projector"></param>
    /// <returns></returns>
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
