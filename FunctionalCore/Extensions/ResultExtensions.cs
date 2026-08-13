namespace FunctionalCore.Extensions;

/// <summary>
/// Provides extension methods for <c>Result&lt;E, T&gt;</c>.
/// <para><c>Result&lt;E, T&gt;</c> に対する拡張メソッドを提供する。</para>
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Executes a side effect according to whether the Result is successful or failed without changing the Result.
    /// <para>Result が成功または失敗のどちらであるかに応じた副作用を実行し、Result 自体は変更しない。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type.
    /// <para>成功値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The Result to process.
    /// <para>処理対象の Result。</para>
    /// </param>
    /// <param name="onSuccess">
    /// The action to execute when <paramref name="result"/> is successful.
    /// <para><paramref name="result"/> が成功している場合に実行するアクション。</para>
    /// </param>
    /// <param name="onFailure">
    /// The action to execute when <paramref name="result"/> is failed.
    /// <para><paramref name="result"/> が失敗している場合に実行するアクション。</para>
    /// </param>
    /// <returns>
    /// The original Result unchanged.
    /// <para>変更されていない元の Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.
    /// <para><paramref name="onSuccess"/> または <paramref name="onFailure"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is uninitialized.
    /// <para><paramref name="result"/> が未初期化の場合にスローされる。</para>
    /// </exception>
    public static Result<E, T> TapBoth<E, T>(this Result<E, T> result, Action<T> onSuccess, Action<E> onFailure)
    {
        result.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        if (result.IsSuccess)
            onSuccess(result.Value);
        else
            onFailure(result.Error);

        return result;
    }

    #region Value Extraction / 値の取り出し

    /// <summary>
    /// Returns the success value when the Result is successful; otherwise throws an exception produced by the specified factory.
    /// <para>Result が成功している場合は成功値を返し、失敗している場合は指定されたファクトリで生成した例外をスローする。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type.
    /// <para>成功値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The Result from which to extract the success value.
    /// <para>成功値を取得する Result。</para>
    /// </param>
    /// <param name="toException">
    /// A function that converts the error into an exception when <paramref name="result"/> is failed.
    /// <para><paramref name="result"/> が失敗している場合に、エラーから例外を生成する関数。</para>
    /// </param>
    /// <returns>
    /// The success value contained in <paramref name="result"/>.
    /// <para><paramref name="result"/> が保持する成功値。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="toException"/> is null.
    /// <para><paramref name="toException"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is uninitialized, or when <paramref name="toException"/> returns null.
    /// <para><paramref name="result"/> が未初期化、または <paramref name="toException"/> が null を返した場合にスローされる。</para>
    /// </exception>
    public static T ValueOrThrow<E, T>(this Result<E, T> result, Func<E, Exception> toException)
    {
        result.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(toException);

        if (result.IsSuccess)
            return result.Value;

        var ex = toException(result.Error);

        if (ex is null)
            throw new InvalidOperationException("Exception factory must not return null.");

        throw ex;
    }

    /// <summary>
    /// Returns the success value when the Result is successful; otherwise returns the specified fallback value.
    /// <para>Result が成功している場合は成功値を返し、失敗している場合は指定された代替値を返す。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type.
    /// <para>成功値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The Result from which to obtain the value.
    /// <para>値を取得する Result。</para>
    /// </param>
    /// <param name="defaultValue">
    /// The fallback value to return when <paramref name="result"/> is failed. Must not be null.
    /// <para><paramref name="result"/> が失敗している場合に返す代替値。null は許可されない。</para>
    /// </param>
    /// <returns>
    /// The success value when <paramref name="result"/> is successful; otherwise <paramref name="defaultValue"/>.
    /// <para><paramref name="result"/> が成功している場合は成功値、それ以外は <paramref name="defaultValue"/>。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="defaultValue"/> is null.
    /// <para><paramref name="defaultValue"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is uninitialized.
    /// <para><paramref name="result"/> が未初期化の場合にスローされる。</para>
    /// </exception>
    public static T GetValueOr<E, T>(this Result<E, T> result, T defaultValue)
    {
        result.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(defaultValue);

        return result.IsSuccess ? result.Value : defaultValue;
    }

    /// <summary>
    /// Returns this Result when it is successful; otherwise returns the specified alternative Result.
    /// <para>この Result が成功している場合は自身を返し、失敗している場合は指定された代替 Result を返す。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type.
    /// <para>成功値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The source Result.
    /// <para>元の Result。</para>
    /// </param>
    /// <param name="other">
    /// The alternative Result to return when <paramref name="result"/> is failed.
    /// <para><paramref name="result"/> が失敗している場合に返す代替 Result。</para>
    /// </param>
    /// <returns>
    /// <paramref name="result"/> when it is successful; otherwise <paramref name="other"/>.
    /// <para><paramref name="result"/> が成功している場合は自身、それ以外は <paramref name="other"/>。</para>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is uninitialized,
    /// or when <paramref name="result"/> is failed and <paramref name="other"/> is uninitialized.
    /// <para>
    /// <paramref name="result"/> が未初期化、
    /// または <paramref name="result"/> が失敗していて <paramref name="other"/> が未初期化の場合にスローされる。
    /// </para>
    /// </exception>
    public static Result<E, T> Or<E, T>(this Result<E, T> result, Result<E, T> other)
    {
        result.ThrowIfNotInitialized();

        if (result.IsSuccess)
            return result;

        other.ThrowIfNotInitialized();

        return other;
    }

    /// <summary>
    /// Returns this Result when it is successful; otherwise returns an alternative Result produced by the specified factory.
    /// <para>この Result が成功している場合は自身を返し、失敗している場合は指定されたファクトリで生成した代替 Result を返す。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type.
    /// <para>成功値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The source Result.
    /// <para>元の Result。</para>
    /// </param>
    /// <param name="otherFactory">
    /// A function that produces the alternative Result when <paramref name="result"/> is failed.
    /// <para><paramref name="result"/> が失敗している場合に代替 Result を生成する関数。</para>
    /// </param>
    /// <returns>
    /// <paramref name="result"/> when it is successful;
    /// otherwise the Result produced by <paramref name="otherFactory"/>.
    /// <para>
    /// <paramref name="result"/> が成功している場合は自身、
    /// それ以外は <paramref name="otherFactory"/> が生成した Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="otherFactory"/> is null.
    /// <para><paramref name="otherFactory"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is uninitialized,
    /// or when <paramref name="otherFactory"/> returns an uninitialized Result.
    /// <para>
    /// <paramref name="result"/> が未初期化、
    /// または <paramref name="otherFactory"/> が未初期化の Result を返した場合にスローされる。
    /// </para>
    /// </exception>
    public static Result<E, T> Or<E, T>(this Result<E, T> result, Func<Result<E, T>> otherFactory)
    {
        result.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(otherFactory);

        if (result.IsSuccess)
            return result;

        var other = otherFactory();
        other.ThrowIfNotInitialized();

        return other;
    }

    #endregion

    #region Conversions / 変換

    /// <summary>
    /// Converts an Option to a Result using the specified error when the Option is None.
    /// <para>Option を Result に変換し、None の場合は指定されたエラーを使用する。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The value type.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The Option to convert.
    /// <para>変換する Option。</para>
    /// </param>
    /// <param name="error">
    /// The error to use when <paramref name="option"/> is None. Must not be null.
    /// <para><paramref name="option"/> が None の場合に使用するエラー。null は許可されない。</para>
    /// </param>
    /// <returns>
    /// A successful Result containing the Option value when <paramref name="option"/> is Some;
    /// otherwise a failed Result containing <paramref name="error"/>.
    /// <para>
    /// <paramref name="option"/> が Some の場合は値を保持する成功 Result。
    /// None の場合は <paramref name="error"/> を保持する失敗 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="error"/> is null.
    /// <para><paramref name="error"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Result<E, T> ToResult<E, T>(this Option<T> option, E error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return option.ToResult(() => error);
    }

    /// <summary>
    /// Converts an Option to a Result using an error produced by the specified factory when the Option is None.
    /// <para>Option を Result に変換し、None の場合は指定されたファクトリで生成したエラーを使用する。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The value type.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="option">
    /// The Option to convert.
    /// <para>変換する Option。</para>
    /// </param>
    /// <param name="errorFactory">
    /// A function that creates the error when <paramref name="option"/> is None.
    /// <para><paramref name="option"/> が None の場合にエラーを生成する関数。</para>
    /// </param>
    /// <returns>
    /// A successful Result containing the Option value when <paramref name="option"/> is Some;
    /// otherwise a failed Result containing the generated error.
    /// <para>
    /// <paramref name="option"/> が Some の場合は値を保持する成功 Result。
    /// None の場合は生成されたエラーを保持する失敗 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="errorFactory"/> is null.
    /// <para><paramref name="errorFactory"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="errorFactory"/> returns null.
    /// <para><paramref name="errorFactory"/> が null を返した場合にスローされる。</para>
    /// </exception>
    public static Result<E, T> ToResult<E, T>(this Option<T> option, Func<E> errorFactory)
    {
        ArgumentNullException.ThrowIfNull(errorFactory);

        if (option.HasValue)
            return Result<E, T>.Ok(option.Value);

        var error = errorFactory();

        if (error is null)
            throw new InvalidOperationException("Error factory must not return null.");

        return Result<E, T>.Fail(error);
    }

    /// <summary>
    /// Converts a value to a Result, using the specified error when the value is null.
    /// <para>値を Result に変換し、値が null の場合は指定されたエラーを使用する。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The value type.
    /// <para>値の型。</para>
    /// </typeparam>
    /// <param name="value">
    /// The value to convert.
    /// <para>変換する値。</para>
    /// </param>
    /// <param name="errorIfNull">
    /// The error to use when <paramref name="value"/> is null.
    /// <para><paramref name="value"/> が null の場合に使用するエラー。</para>
    /// </param>
    /// <returns>
    /// A successful Result containing <paramref name="value"/> when it is non-null;
    /// otherwise a failed Result containing <paramref name="errorIfNull"/>.
    /// <para>
    /// <paramref name="value"/> が null でない場合はその値を保持する成功 Result。
    /// null の場合は <paramref name="errorIfNull"/> を保持する失敗 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is null and <paramref name="errorIfNull"/> is null.
    /// <para><paramref name="value"/> が null で、かつ <paramref name="errorIfNull"/> も null の場合にスローされる。</para>
    /// </exception>
    public static Result<E, T> ToResult<E, T>(this T value, E errorIfNull)
    {
        if (value is null)
            return Result<E, T>.Fail(errorIfNull);

        return Result<E, T>.Ok(value);
    }

    #endregion

    #region Collections / コレクション

    /// <summary>
    /// Converts a sequence of Results into a single Result containing all success values.
    /// <para>Result のシーケンスを、すべての成功値を保持する1つの Result にまとめる。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type.
    /// <para>成功値の型。</para>
    /// </typeparam>
    /// <param name="results">
    /// The sequence of Results to combine.
    /// <para>まとめる Result のシーケンス。</para>
    /// </param>
    /// <returns>
    /// A successful Result containing all values when every Result is successful;
    /// otherwise a failed Result containing the first encountered error.
    /// An empty sequence produces a successful Result containing an empty list.
    /// <para>
    /// すべての Result が成功している場合は、すべての成功値を保持するリストを含む成功 Result。
    /// 失敗が存在する場合は、最初に見つかったエラーを保持する失敗 Result。
    /// 空のシーケンスの場合は空のリストを保持する成功 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="results"/> is null.
    /// <para><paramref name="results"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the sequence contains an uninitialized Result.
    /// <para>シーケンスに未初期化の Result が含まれている場合にスローされる。</para>
    /// </exception>
    public static Result<E, IReadOnlyList<T>> Sequence<E, T>(this IEnumerable<Result<E, T>> results)
    {
        ArgumentNullException.ThrowIfNull(results);

        var lst = new List<T>();

        foreach (var r in results)
        {
            r.ThrowIfNotInitialized();

            if (!r.IsSuccess)
                return Result<E, IReadOnlyList<T>>.Fail(r.Error);

            lst.Add(r.Value);
        }

        return Result<E, IReadOnlyList<T>>.Ok(lst);
    }

    /// <summary>
    /// Applies a Result-producing function to each item and combines the results into a single Result.
    /// <para>各要素に Result を返す関数を適用し、その結果を1つの Result にまとめる。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The input value type.
    /// <para>入力値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The resulting success value type.
    /// <para>結果の成功値の型。</para>
    /// </typeparam>
    /// <param name="items">
    /// The sequence of values to transform.
    /// <para>変換する値のシーケンス。</para>
    /// </param>
    /// <param name="selector">
    /// A function that transforms each value into a Result.
    /// <para>各値を Result に変換する関数。</para>
    /// </param>
    /// <returns>
    /// A successful Result containing all transformed values when every application of
    /// <paramref name="selector"/> succeeds; otherwise a failed Result containing the first encountered error.
    /// An empty sequence produces a successful Result containing an empty list.
    /// <para>
    /// <paramref name="selector"/> のすべての適用結果が成功している場合は、
    /// 変換されたすべての値を保持するリストを含む成功 Result。
    /// 失敗が存在する場合は、最初に見つかったエラーを保持する失敗 Result。
    /// 空のシーケンスの場合は空のリストを保持する成功 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="items"/> or <paramref name="selector"/> is null.
    /// <para><paramref name="items"/> または <paramref name="selector"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="selector"/> returns an uninitialized Result.
    /// <para><paramref name="selector"/> が未初期化の Result を返した場合にスローされる。</para>
    /// </exception>
    public static Result<E, IReadOnlyList<U>> Traverse<E, T, U>(this IEnumerable<T> items, Func<T, Result<E, U>> selector)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(selector);

        var lst = new List<U>();

        foreach (var item in items)
        {
            var r = selector(item);
            r.ThrowIfNotInitialized();

            if (!r.IsSuccess)
                return Result<E, IReadOnlyList<U>>.Fail(r.Error);

            lst.Add(r.Value);
        }

        return Result<E, IReadOnlyList<U>>.Ok(lst);
    }

    #endregion

    /// <summary>
    /// Combines two successful Results by applying the specified selector to their values.
    /// <para>2つの成功 Result が保持する値に指定された selector を適用して、1つの Result にまとめる。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type of the first Result.
    /// <para>1つ目の Result の成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="R">
    /// The success value type of the second Result.
    /// <para>2つ目の Result の成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the combined value.
    /// <para>組み合わせた値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The first Result.
    /// <para>1つ目の Result。</para>
    /// </param>
    /// <param name="other">
    /// The second Result.
    /// <para>2つ目の Result。</para>
    /// </param>
    /// <param name="selector">
    /// A function that combines the success values of both Results. Must not return null.
    /// <para>2つの Result の成功値を組み合わせる関数。null を返してはならない。</para>
    /// </param>
    /// <returns>
    /// A successful Result containing the value produced by <paramref name="selector"/> when both Results are successful.
    /// If <paramref name="result"/> is failed, its error is returned.
    /// Otherwise, if <paramref name="other"/> is failed, its error is returned.
    /// <para>
    /// 両方の Result が成功している場合は、<paramref name="selector"/> が生成した値を保持する成功 Result。
    /// <paramref name="result"/> が失敗している場合は、そのエラーを保持する失敗 Result。
    /// それ以外で <paramref name="other"/> が失敗している場合は、そのエラーを保持する失敗 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> is null.
    /// <para><paramref name="selector"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> or <paramref name="other"/> is uninitialized,
    /// or when <paramref name="selector"/> returns null.
    /// <para>
    /// <paramref name="result"/> または <paramref name="other"/> が未初期化、
    /// または <paramref name="selector"/> が null を返した場合にスローされる。
    /// </para>
    /// </exception>
    public static Result<E, U> Combine<E, T, R, U>(this Result<E, T> result, Result<E, R> other, Func<T, R, U> selector)
    {
        result.ThrowIfNotInitialized();
        other.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(selector);

        if (!result.IsSuccess)
            return Result<E, U>.Fail(result.Error);

        if (!other.IsSuccess)
            return Result<E, U>.Fail(other.Error);

        var val = selector(result.Value, other.Value);

        if (val is null)
            throw new InvalidOperationException("Selector must not return null");

        return Result<E, U>.Ok(val);
    }
}
