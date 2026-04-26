namespace FunctionalCore.Extensions;

/// <summary>
/// Extension methods for Option<T>
/// Option<T>用の拡張メソッド
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// Executes side-effect regardless of state.
    /// 状態に関係なく副作用を実行する。
    ///
    /// Does not change the Option.
    /// 状態は変更しない
    /// </summary>
    public static Option<T> TapBoth<T>(this Option<T> option, Action<T> onSome, Action onNone)
    {
        ArgumentNullException.ThrowIfNull(onSome);
        ArgumentNullException.ThrowIfNull(onNone);

        if (option.HasValue)
            onSome(option.Value);
        else
            onNone();

        return option;
    }

    #region Value Extraction / 値の取り出し
    /// <summary>
    /// Returns the value if present; otherwise throws the specified exception.
    /// 値があれば取得、なければ指定した例外を発生させる。
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

    /// <summary>
    /// Returns value if present, otherwise fallback.
    /// 値が存在すればそれを返し、無ければ代替値を返す。
    /// </summary>
    public static T GetValueOr<T>(this Option<T> option, T defaultValue)
    {
        ArgumentNullException.ThrowIfNull(defaultValue);

        return option.HasValue ? option.Value : defaultValue;
    }

    /// <summary>
    /// Returns this if Some, otherwise other.
    /// Someなら自身、Noneなら代替を返す。
    /// </summary>
    public static Option<T> Or<T>(this Option<T> option, Option<T> other)
    {
        return option.HasValue ? option : other;
    }

    /// <summary>
    /// Returns this if Some, otherwise factory result.
    /// Someなら自身、Noneなら生成結果
    /// </summary>
    public static Option<T> Or<T>(this Option<T> option, Func<Option<T>> otherFactory)
    {
        ArgumentNullException.ThrowIfNull(otherFactory);

        return option.HasValue ? option : otherFactory();
    }
    #endregion

    #region Conversions / 変換
    /// <summary>
    /// Converts Result<E, T> to Option<T>
    /// Result<E, T>をOption<T>に変換する。
    /// </summary>
    /// <typeparam name="E"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static Option<T> ToOption<E, T>(this Result<E, T> result)
    {
        return result.IsSuccess ? Option<T>.Some(result.Value) : Option<T>.None;
    }

    /// <summary>
    /// Converts any value to Option<T>.
    /// 任意の値をOption<T>に変換する。
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

    #region Collections / コレクション
    /// <summary>
    /// Sequences a list of Options into a single Option of a list.
    /// Option のリストをまとめて Option<List<T>> にする。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Option<IReadOnlyCollection<T>> Sequence<T>(this IEnumerable<Option<T>> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var lst = new List<T>();
        foreach (var opt in options)
        {
            if (opt.HasValue == false)
            {
                return Option<IReadOnlyCollection<T>>.None;
            }
            lst.Add(opt.Value);
        }

        return Option<IReadOnlyCollection<T>>.Some(lst);
    }

    /// <summary>
    /// Applies a function returning Option to each item and sequences the results.
    /// 各要素に Option を返す関数を適用し、結果をまとめる。
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


}