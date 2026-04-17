namespace FunctionalCore;

/// <summary>
/// Extension methods for Option<T>
/// Option<T>用の拡張メソッド
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// Returns the value if present; otherwise throws the specified exception.
    /// 値があれば取得、なければ指定した例外を発生させる
    /// </summary>
    /// <param name="toException"></param>
    /// <returns></returns>
    public static T ValueOrThrow<T>(this Option<T> option, Func<Exception> toException)
    {
        ArgumentNullException.ThrowIfNull(toException);

        if (option.HasValue)
        {
            return option.Value;
        }
        throw toException();
    }


    #region Collections / コレクション

    /// <summary>
    /// Sequences a list of Options into a single Option of a list.
    /// Option のリストをまとめて Option<List<T>> にする
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Option<IEnumerable<T>> Sequence<T>(this IEnumerable<Option<T>> options)
    {
        var lst = new List<T>();
        foreach (var opt in options)
        {
            if (opt.HasValue == false)
            {
                return Option<IEnumerable<T>>.None;
            }
            lst.Add(opt.Value);
        }

        return Option<IEnumerable<T>>.Some(lst);
    }

    /// <summary>
    /// Applies a function returning Option to each item and sequences the results.
    /// 各要素に Option を返す関数を適用し、結果をまとめる
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="U"></typeparam>
    /// <param name="items"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    public static Option<IReadOnlyList<U>> Traverse<T, U>(this IEnumerable<T> items, Func<T, Option<U>> f)
    {
        ArgumentNullException.ThrowIfNull(f);

        var lst = new List<U>();
        foreach (var item in items)
        {
            var opt = f(item);
            if (opt.HasValue == false)
            {
                return Option<IReadOnlyList<U>>.None;
            }
            lst.Add(opt.Value);
        }

        return Option<IReadOnlyList<U>>.Some(lst);
    }

    #endregion

    #region Conversions

    /// <summary>
    /// Converts Result<E, T> to Option<T>
    /// Result<E, T>をOption<T>に変換する
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static Option<T> ToOptionFromResult<E, T>(this Result<E, T> result)
    {
        return result.IsSuccess ? Option<T>.Some(result.Value) : Option<T>.None;
    }

    /// <summary>
    /// Converts any value to Option<T>.
    /// 任意の値をOption<T>に変換する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Option<T> ToOption<T>(this T value)
    {
        if (value is null)
        {
            return Option<T>.None;
        }

        return Option<T>.Some(value);
    }

    #endregion
}