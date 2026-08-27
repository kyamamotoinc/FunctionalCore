namespace FunctionalCore.Extensions;

/// <summary>
/// Provides extension methods for <see cref="Option{T}"/>.
/// <para><see cref="Option{T}"/> に対する拡張メソッドを提供する。</para>
/// </summary>
public static class OptionExtensions
{
    /// <summary>
    /// Executes one of two side effects according to whether the Option is Some or None without changing the Option.
    /// <para>Option が Some または None のどちらであるかに応じた副作用を実行し、Option 自体は変更しない。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The value type.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The Option to process.
    /// <para>処理対象の Option。</para>
    /// </param>
    /// <param name="onSome">
    /// The action to execute when <paramref name="option"/> is Some.
    /// <para><paramref name="option"/> が Some の場合に実行するアクション。</para>
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
    /// The value type.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The Option from which to extract the value.
    /// <para>値を取得する Option。</para>
    /// </param>
    /// <param name="toException">
    /// A function that creates the exception to throw when <paramref name="option"/> is None.
    /// <para><paramref name="option"/> が None の場合にスローする例外を生成する関数。</para>
    /// </param>
    /// <returns>
    /// The value contained in <paramref name="option"/>.
    /// <para><paramref name="option"/> が保持する値。</para>
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
    /// The value type.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The Option from which to obtain the value.
    /// <para>値を取得する Option。</para>
    /// </param>
    /// <param name="defaultValue">
    /// The fallback value to return when <paramref name="option"/> is None. The fallback may be null when the type permits null.
    /// <para><paramref name="option"/> が None の場合に返す代替値。型が null を許容する場合は null を指定できる。</para>
    /// </param>
    /// <returns>
    /// The contained value when <paramref name="option"/> is Some; otherwise <paramref name="defaultValue"/>.
    /// <para><paramref name="option"/> が Some の場合は保持している値、それ以外は <paramref name="defaultValue"/>。</para>
    /// </returns>
    public static T GetValueOr<T>(this Option<T> option, T defaultValue)
    {
        return option.HasValue ? option.Value : defaultValue;
    }

    /// <summary>
    /// Returns this Option when it is Some; otherwise returns the specified alternative Option.
    /// <para>この Option が Some の場合は自身を返し、None の場合は指定された代替 Option を返す。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The value type.
    /// <para>値の型。</para>
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
    /// The value type.
    /// <para>値の型。</para>
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
    /// Converts a <see cref="Result{E, T}"/> to an <see cref="Option{T}"/>, discarding the error when the Result is failed.
    /// <para><see cref="Result{E, T}"/> を <see cref="Option{T}"/> に変換し、Result が失敗している場合はエラーを破棄する。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The value type.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The Result to convert.
    /// <para>変換する Result。</para>
    /// </param>
    /// <returns>
    /// A Some containing the success value when <paramref name="result"/> is successful; otherwise None.
    /// <para><paramref name="result"/> が成功している場合は成功値を保持する Some、それ以外は None。</para>
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
    /// Converts a value to an Option, treating null as None.
    /// <para>値を Option に変換し、null の場合は None として扱う。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The value type.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="value">
    /// The value to convert.
    /// <para>変換する値。</para>
    /// </param>
    /// <returns>
    /// A Some containing <paramref name="value"/> when it is non-null; otherwise None.
    /// <para><paramref name="value"/> が null でない場合はその値を保持する Some、それ以外は None。</para>
    /// </returns>
    public static Option<T> ToOption<T>(this T value)
    {
        if (value is null)
            return Option<T>.None;

        return Option<T>.Some(value);
    }

    #endregion

    #region Collections / コレクション

    /// <summary>
    /// Converts a sequence of Options into a single Option containing all values when every Option is Some.
    /// <para>Option のシーケンスを、すべてが Some の場合に全ての値を保持する1つの Option にまとめる。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The value type.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="options">
    /// The sequence of Options to combine.
    /// <para>まとめる Option のシーケンス。</para>
    /// </param>
    /// <returns>
    /// A Some containing all values when every Option is Some; otherwise None. An empty sequence produces a Some containing an empty list.
    /// <para>すべての Option が Some の場合は全ての値を保持する Some、それ以外は None。空のシーケンスの場合は空のリストを保持する Some。</para>
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
    /// Applies an Option-producing function to each item and combines all produced values when every result is Some.
    /// <para>各要素に Option を返す関数を適用し、すべての結果が Some の場合に全ての値をまとめる。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The input value type.
    /// <para>入力値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The output value type.
    /// <para>出力値の型。</para>
    /// </typeparam>
    /// <param name="items">
    /// The source sequence.
    /// <para>元のシーケンス。</para>
    /// </param>
    /// <param name="f">
    /// A function that converts each item to an Option.
    /// <para>各要素を Option に変換する関数。</para>
    /// </param>
    /// <returns>
    /// A Some containing all produced values when every result is Some; otherwise None. An empty sequence produces a Some containing an empty list.
    /// <para>すべての結果が Some の場合は全ての値を保持する Some、それ以外は None。空のシーケンスの場合は空のリストを保持する Some。</para>
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
    /// Combines two Options using the specified selector when both are Some.
    /// <para>2つの Option がともに Some の場合に、指定された関数で値を組み合わせる。</para>
    /// </summary>
    /// <typeparam name="T">
    /// The first value type.
    /// <para>最初の値の型。</para>
    /// </typeparam>
    /// <typeparam name="R">
    /// The second value type.
    /// <para>2番目の値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The combined value type.
    /// <para>組み合わせ後の値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The first Option.
    /// <para>最初の Option。</para>
    /// </param>
    /// <param name="other">
    /// The second Option.
    /// <para>2番目の Option。</para>
    /// </param>
    /// <param name="selector">
    /// A function that combines the two values.
    /// <para>2つの値を組み合わせる関数。</para>
    /// </param>
    /// <returns>
    /// A Some containing the combined value when both Options are Some and the selector returns a non-null value; otherwise None.
    /// <para>両方の Option が Some かつ selector が null ではない値を返した場合は、その値を保持する Some。それ以外は None。</para>
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
