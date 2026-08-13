namespace FunctionalCore.Extensions;

/// <summary>
/// Provides extension methods for <c>Option&lt;T&gt;</c>.
/// <para><c>Option&lt;T&gt;</c> に対する拡張メソッドを提供する。</para>
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// Executes a side effect according to whether the Option is Some or None without changing the Option.
    /// <para>Option が Some または None のどちらであるかに応じた副作用を実行し、Option 自体は変更しない。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the contained value.
    /// <para>保持する値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The Option to process.
    /// <para>処理対象の Option。</para>
    /// </param>
    /// <param name="onSome">
    /// The action to execute when <paramref name="option"/> contains a value.
    /// <para><paramref name="option"/> が値を保持している場合に実行するアクション。</para>
    /// </param>
    /// <param name="onNone">
    /// The action to execute when <paramref name="option"/> is None.
    /// <para><paramref name="option"/> が None の場合に実行するアクション。</para>
    /// </param>
    /// <returns>
    /// The original Option unchanged.
    /// <para>変更されていない元の Option。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="onSome"/> or <paramref name="onNone"/> is null.
    /// <para><paramref name="onSome"/> または <paramref name="onNone"/> が null の場合にスローされる。</para>
    /// </exception>
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
    /// Returns the contained value when the Option is Some; otherwise throws an exception produced by the specified factory.
    /// <para>Option が Some の場合は保持している値を返し、None の場合は指定されたファクトリで生成した例外をスローする。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the contained value.
    /// <para>保持する値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The Option from which to extract the value.
    /// <para>値を取得する Option。</para>
    /// </param>
    /// <param name="toException">
    /// A function that creates the exception to throw when the Option is None.
    /// <para>Option が None の場合にスローする例外を生成する関数。</para>
    /// </param>
    /// <returns>
    /// The contained value when <paramref name="option"/> is Some.
    /// <para><paramref name="option"/> が Some の場合に保持している値。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="toException"/> is null.
    /// <para><paramref name="toException"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="toException"/> returns null.
    /// <para><paramref name="toException"/> が null を返した場合にスローされる。</para>
    /// </exception>
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
    /// Returns the contained value when the Option is Some; otherwise returns the specified fallback value.
    /// <para>Option が Some の場合は保持している値を返し、None の場合は指定された代替値を返す。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the contained value.
    /// <para>保持する値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The Option from which to obtain the value.
    /// <para>値を取得する Option。</para>
    /// </param>
    /// <param name="defaultValue">
    /// The fallback value to return when <paramref name="option"/> is None. Must not be null.
    /// <para><paramref name="option"/> が None の場合に返す代替値。null は許可されない。</para>
    /// </param>
    /// <returns>
    /// The contained value when the Option is Some; otherwise <paramref name="defaultValue"/>.
    /// <para>Option が Some の場合は保持している値、それ以外は <paramref name="defaultValue"/>。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="defaultValue"/> is null.
    /// <para><paramref name="defaultValue"/> が null の場合にスローされる。</para>
    /// </exception>
    public static T GetValueOr<T>(this Option<T> option, T defaultValue)
    {
        ArgumentNullException.ThrowIfNull(defaultValue);

        return option.HasValue ? option.Value : defaultValue;
    }

    /// <summary>
    /// Returns this Option when it is Some; otherwise returns the specified alternative Option.
    /// <para>この Option が Some の場合は自身を返し、None の場合は指定された代替 Option を返す。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the contained value.
    /// <para>保持する値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The source Option.
    /// <para>元の Option。</para>
    /// </param>
    /// <param name="other">
    /// The alternative Option to return when <paramref name="option"/> is None.
    /// <para><paramref name="option"/> が None の場合に返す代替 Option。</para>
    /// </param>
    /// <returns>
    /// <paramref name="option"/> when it is Some; otherwise <paramref name="other"/>.
    /// <para><paramref name="option"/> が Some の場合は自身、それ以外は <paramref name="other"/>。</para>
    /// </returns>
    public static Option<T> Or<T>(this Option<T> option, Option<T> other)
    {
        return option.HasValue ? option : other;
    }

    /// <summary>
    /// Returns this Option when it is Some; otherwise returns an alternative Option produced by the specified factory.
    /// <para>この Option が Some の場合は自身を返し、None の場合は指定されたファクトリで生成した代替 Option を返す。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the contained value.
    /// <para>保持する値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The source Option.
    /// <para>元の Option。</para>
    /// </param>
    /// <param name="otherFactory">
    /// A function that produces the alternative Option when <paramref name="option"/> is None.
    /// <para><paramref name="option"/> が None の場合に代替 Option を生成する関数。</para>
    /// </param>
    /// <returns>
    /// <paramref name="option"/> when it is Some; otherwise the Option produced by <paramref name="otherFactory"/>.
    /// <para><paramref name="option"/> が Some の場合は自身、それ以外は <paramref name="otherFactory"/> が生成した Option。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="otherFactory"/> is null.
    /// <para><paramref name="otherFactory"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Option<T> Or<T>(this Option<T> option, Func<Option<T>> otherFactory)
    {
        ArgumentNullException.ThrowIfNull(otherFactory);

        return option.HasValue ? option : otherFactory();
    }

    #endregion

    #region Conversions / 変換

    /// <summary>
    /// Converts a Result to an Option by preserving only its success value.
    /// <para>Result の成功値だけを保持して Option に変換する。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type of the Result.
    /// <para>Result のエラー型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type of the Result.
    /// <para>Result の成功値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The Result to convert.
    /// <para>変換する Result。</para>
    /// </param>
    /// <returns>
    /// Some containing the success value when <paramref name="result"/> is successful;
    /// otherwise None.
    /// <para>
    /// <paramref name="result"/> が成功している場合は成功値を保持する Some。
    /// 失敗している場合は None。
    /// </para>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is uninitialized.
    /// <para><paramref name="result"/> が未初期化の場合にスローされる。</para>
    /// </exception>
    public static Option<T> ToOption<E, T>(this Result<E, T> result)
    {
        result.ThrowIfNotInitialized();

        return result.IsSuccess ? Option<T>.Some(result.Value) : Option<T>.None;
    }

    /// <summary>
    /// Converts a value to an Option.
    /// <para>任意の値を Option に変換する。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the value.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="value">
    /// The value to convert.
    /// <para>変換する値。</para>
    /// </param>
    /// <returns>
    /// Some containing <paramref name="value"/> when it is non-null;
    /// otherwise None.
    /// <para>
    /// <paramref name="value"/> が null でない場合はその値を保持する Some。
    /// null の場合は None。
    /// </para>
    /// </returns>
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
    /// Converts a sequence of Options into a single Option containing all values.
    /// <para>Option のシーケンスを、すべての値を保持する1つの Option にまとめる。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the contained values.
    /// <para>保持する値の型。</para>
    /// </typeparam>
    /// <param name="options">
    /// The sequence of Options to combine.
    /// <para>まとめる Option のシーケンス。</para>
    /// </param>
    /// <returns>
    /// Some containing all values when every Option is Some;
    /// otherwise None.
    /// An empty sequence produces Some containing an empty list.
    /// <para>
    /// すべての Option が Some の場合は、すべての値を保持するリストを含む Some。
    /// 1つでも None が存在する場合は None。
    /// 空のシーケンスの場合は空のリストを保持する Some。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="options"/> is null.
    /// <para><paramref name="options"/> が null の場合にスローされる。</para>
    /// </exception>
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
    /// Applies an Option-producing function to each item and combines the results into a single Option.
    /// <para>各要素に Option を返す関数を適用し、その結果を1つの Option にまとめる。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of the input values.
    /// <para>入力値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the resulting values.
    /// <para>結果の値の型。</para>
    /// </typeparam>
    /// <param name="items">
    /// The sequence of values to transform.
    /// <para>変換する値のシーケンス。</para>
    /// </param>
    /// <param name="f">
    /// A function that transforms each value into an Option.
    /// <para>各値を Option に変換する関数。</para>
    /// </param>
    /// <returns>
    /// Some containing all transformed values when every application of <paramref name="f"/> returns Some;
    /// otherwise None.
    /// An empty sequence produces Some containing an empty list.
    /// <para>
    /// <paramref name="f"/> のすべての適用結果が Some の場合は、
    /// 変換されたすべての値を保持するリストを含む Some。
    /// 1つでも None の場合は None。
    /// 空のシーケンスの場合は空のリストを保持する Some。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="items"/> or <paramref name="f"/> is null.
    /// <para><paramref name="items"/> または <paramref name="f"/> が null の場合にスローされる。</para>
    /// </exception>
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
    /// Combines two Options by applying the specified selector to their values.
    /// <para>2つの Option が保持する値に指定された selector を適用して、1つの Option にまとめる。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The value type of the first Option.
    /// <para>1つ目の Option の値の型。</para>
    /// </typeparam>
    /// <typeparam name="R">
    /// The value type of the second Option.
    /// <para>2つ目の Option の値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the combined value.
    /// <para>組み合わせた値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The first Option.
    /// <para>1つ目の Option。</para>
    /// </param>
    /// <param name="other">
    /// The second Option.
    /// <para>2つ目の Option。</para>
    /// </param>
    /// <param name="selector">
    /// A function that combines the values of both Options.
    /// If it returns null, the result is converted to None.
    /// <para>
    /// 2つの Option の値を組み合わせる関数。
    /// null を返した場合は None に変換される。
    /// </para>
    /// </param>
    /// <returns>
    /// Some containing the value produced by <paramref name="selector"/> when both Options are Some
    /// and the selector returns a non-null value; otherwise None.
    /// <para>
    /// 両方の Option が Some で、かつ <paramref name="selector"/> が null ではない値を返した場合は、
    /// その値を保持する Some。
    /// それ以外は None。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> is null.
    /// <para><paramref name="selector"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Option<U> Combine<T, R, U>(this Option<T> option, Option<R> other, Func<T, R, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (!option.HasValue || !other.HasValue)
            return Option<U>.None;

        var value = selector(option.Value, other.Value);

        return value is null ? Option<U>.None : Option<U>.Some(value);
    }
}
