namespace FunctionalCore.Linq;

/// <summary>
/// Provides LINQ extension methods for Option<T>.
/// Option<T>に対するLINQ拡張メソッドを提供する。
/// </summary>
public static class OptionLinq
{
    /// <summary>
    /// LINQ Select (alias of Map).
    /// LINQ用のMap
    /// </summary>
    public static Option<U> Select<T, U>(this Option<T> option, Func<T, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return option.Map(selector);
    }

    /// <summary>
    /// LINQ SelectMany (alias of Bind + Map).
    /// LINQ用のBind + Map
    /// </summary>
    public static Option<V> SelectMany<T, U, V>(this Option<T> option, Func<T, Option<U>> selector, Func<T, U, V> projector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(projector);

        return option.Bind(x => selector(x).Map(y => projector(x, y)));
    }

    /// <summary>
    /// Filters the value using a predicate.
    /// 条件を満たさない場合は None に変換する。
    ///
    /// This is equivalent to a validation step.
    /// バリデーション用途
    /// </summary>
    public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return option.Ensure(predicate);
    }
}
