namespace FunctionalCore.Extensions;

/// <summary>
/// Provides extension methods for Option<T>
/// <para>Option<T>に対する拡張メソッドを提供する。</para>
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// Executes side-effect regardless of state.
    /// <para>状態に関係なく副作用を実行する。</para>
    ///
    /// Does not change the Option.
    /// <para>状態は変更しない</para>
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
    /// <para>値があれば取得、なければ指定した例外を発生させる。</para>
    /// </summary>
    /// <param name="option">The option to extract the value from. / 値を抽出するオプション。</param>
    /// <param name="toException">A function that creates the exception to throw. / 発生させる例外を作成する関数。</param>
    /// <returns>
    /// The value if present.
    /// <para>値が存在する場合。</para>
    /// </returns>
    public static T ValueOrThrow<T>(this Option<T> option, Func<Exception> toException)
    {
        ArgumentNullException.ThrowIfNull(toException);

        if (option.HasValue)
            return option.Value;

        var ex = toException();
        if (ex is null)
            throw new InvalidOperationException("Exception factory must not return null.");
             
        throw ex;
    }

    /// <summary>
    /// Returns value if present, otherwise fallback.
    /// <para>値が存在すればそれを返し、無ければ代替値を返す。</para>
    /// </summary>
    /// <typeparam name="T">The type of the value. / 値の型。</typeparam>
    /// <param name="option"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static T GetValueOr<T>(this Option<T> option, T defaultValue)
    {
        ArgumentNullException.ThrowIfNull(defaultValue);

        return option.HasValue ? option.Value : defaultValue;
    }

    /// <summary>
    /// Returns this if Some, otherwise other.
    /// <para>Someなら自身、Noneなら代替を返す。</para>
    /// </summary>
    /// <typeparam name="T">The type of the value. / 値の型。</typeparam>
    /// <param name="option"></param>
    /// <param name="other"></param>
    /// <returns></returns>
    public static Option<T> Or<T>(this Option<T> option, Option<T> other)
    {
        return option.HasValue ? option : other;
    }

    /// <summary>
    /// Returns this if Some, otherwise factory result.
    /// <para>Someなら自身、Noneなら生成結果</para>
    /// </summary>
    /// <typeparam name="T">The type of the value. / 値の型。</typeparam>
    /// <param name="option"></param>
    /// <param name="otherFactory"></param>
    /// <returns></returns>
    public static Option<T> Or<T>(this Option<T> option, Func<Option<T>> otherFactory)
    {
        ArgumentNullException.ThrowIfNull(otherFactory);

        return option.HasValue ? option : otherFactory();
    }
    #endregion

    #region Conversions / 変換
    /// <summary>
    /// Converts Result<E, T> to Option<T>
    /// <para>Result<E, T>をOption<T>に変換する。</para>
    /// </summary>
    /// <typeparam name="E">The type of the error. / エラーの型。</typeparam>
    /// <typeparam name="T">The type of the value. / 値の型。</typeparam>
    /// <param name="result"></param>
    /// <returns></returns>
    public static Option<T> ToOption<E, T>(this Result<E, T> result)
    {
        result.ThrowIfNotInitialized();

        return result.IsSuccess ? Option<T>.Some(result.Value) : Option<T>.None;
    }

    /// <summary>
    /// Converts any value to Option<T>.
    /// <para>任意の値をOption<T>に変換する。</para>
    /// </summary>
    /// <typeparam name="T">The type of the value. / 値の型。</typeparam>
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
    /// <para>Option のリストをまとめて Option<List<T>> にする。</para>
    /// </summary>
    /// <typeparam name="T">The type of the value. / 値の型。</typeparam>
    /// <param name="options"></param>
    /// <returns></returns>
    public static Option<IReadOnlyList<T>> Sequence<T>(this IEnumerable<Option<T>> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        var lst = new List<T>();
        foreach (var opt in options)
        {
            if (!opt.HasValue)
                return Option<IReadOnlyList<T>>.None;

            lst.Add(opt.Value);
        }

        return Option<IReadOnlyList<T>>.Some(lst);
    }

    /// <summary>
    /// Applies a function returning Option to each item and sequences the results.
    /// <para>各要素に Option を返す関数を適用し、結果をまとめる。</para>
    /// </summary>
    /// <typeparam name="T">The type of the input values. / 入力値の型。</typeparam>
    /// <typeparam name="U">The type of the output values. / 出力値の型。</typeparam>
    /// <param name="items"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Thrown if items or f is null. / items または f が null の場合に投げられる。</exception>
    public static Option<IReadOnlyList<U>> Traverse<T, U>(this IEnumerable<T> items, Func<T, Option<U>> f)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(f);

        var lst = new List<U>();
        foreach (var item in items)
        {
            var opt = f(item);
            if (!opt.HasValue)
                return Option<IReadOnlyList<U>>.None;

            lst.Add(opt.Value);
        }

        return Option<IReadOnlyList<U>>.Some(lst);
    }
    #endregion

    /// <summary>
    /// Combines two Option values using a selector function.
    /// <para>2つの Option を関数で組み合わせる。</para>
    /// </summary>
    /// <typeparam name="T">The type of the first option's value. / 最初の Option の値の型。</typeparam>
    /// <typeparam name="R">The type of the second option's value. / 2番目の Option の値の型。</typeparam>
    /// <typeparam name="U">The type of the combined value. / 組み合わされた値の型。</typeparam>
    /// <param name="option">The first option. / 最初の Option。</param>
    /// <param name="other">The second option. / 2番目の Option。</param>
    /// <param name="selector">The function to combine the values. / 値を組み合わせる関数。</param>
    /// <returns>
    /// The combined option.
    /// <para>組み合わされた Option。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if selector is null. / selectorがnullの場合に投げられる。</exception>
    public static Option<U> Combine<T, R, U>(this Option<T> option, Option<R> other, Func<T, R, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (!option.HasValue || !other.HasValue)
            return Option<U>.None;

        var value = selector(option.Value, other.Value);

        return  value is null ? Option<U>.None : Option<U>.Some(value);
    }
}
