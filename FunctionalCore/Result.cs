namespace FunctionalCore;

/// <summary>
/// Represents the result of an operation (Success or Failure).
/// <para>処理結果（成功 / 失敗）を表現する型。</para>
///
/// This type represents expected operation failures as explicit error values.
/// <para>想定内の処理失敗を、例外ではなく明示的なエラー値として扱う。</para>
///
/// <para>
/// Design rules:
/// <para>- Success always contains a value</para>
/// <para>- Failure always contains an error</para>
/// <para>- null is not allowed</para>
/// <para>- operations must preserve these invariants</para>
/// </para>
///
/// <para>
/// 設計ルール:
/// <para>- Success は必ず値を持つ</para>
/// <para>- Failure は必ずエラーを持つ</para>
/// <para>- nullは禁止</para>
/// <para>- すべての操作はこの不変条件を守る</para>
/// </para>
/// </summary>
/// <typeparam name="E">
/// The error type.
/// <para>エラーの型。</para>
/// </typeparam>
/// <typeparam name="T">
/// The success value type.
/// <para>成功時の値の型。</para>
/// </typeparam>
public readonly struct Result<E, T> : IEquatable<Result<E, T>>
{
    /// <summary>
    /// Indicates whether the result is successful.
    /// <para>成功かどうかを示す。</para>
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the result is a failure.
    /// <para>失敗かどうかを示す。</para>
    /// </summary>
    public bool IsFailure => IsInitialized && !IsSuccess;

    /// <summary>
    /// Indicates whether the result is initialized.
    /// Uninitialized results do not allow access to Value and Error.
    /// <para>
    /// 初期化済みかどうかを示す。
    /// 未初期化のResultはValueとErrorにアクセスできない。
    /// </para>
    /// </summary>
    internal bool IsInitialized { get; }

    private readonly T _value;

    /// <summary>
    /// Gets the value if successful.
    /// <para>成功時の値を取得する。</para>
    ///
    /// Throws if the result is uninitialized or failure.
    /// <para>未初期化または失敗時は例外を投げる。</para>
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if uninitialized or if the result is a failure.
    /// <para>未初期化または失敗の場合に投げられる。</para>
    /// </exception>
    public T Value
    {
        get
        {
            this.ThrowIfNotInitialized();

            if (!IsSuccess)
                throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> does not contain a value.");

            return _value;
        }
    }

    private readonly E _error;

    /// <summary>
    /// Gets the error if failed.
    /// <para>失敗時のエラーを取得する。</para>
    ///
    /// Throws if the result is uninitialized or success.
    /// <para>未初期化または成功時は例外を投げる。</para>
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if uninitialized or if the result is a success.
    /// <para>未初期化または成功の場合に投げられる。</para>
    /// </exception>
    public E Error
    {
        get
        {
            this.ThrowIfNotInitialized();

            if (IsSuccess)
                throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> does not contain an error.");

            return _error;
        }
    }

    /// <summary>
    /// Creates a successful result.
    /// <para>成功結果を生成する。</para>
    ///
    /// null is not allowed.
    /// <para>nullは禁止。</para>
    /// </summary>
    /// <param name="value">
    /// The success value.
    /// <para>成功時の値。</para>
    /// </param>
    private Result(T value)
    {
        IsInitialized = true;
        IsSuccess = true;
        _error = default!;
        _value = value;
    }

    /// <summary>
    /// Creates a failed result.
    /// <para>失敗結果を生成する。</para>
    ///
    /// null is not allowed.
    /// <para>nullは禁止。</para>
    /// </summary>
    /// <param name="error">
    /// The error value.
    /// <para>エラーの値。</para>
    /// </param>
    private Result(E error)
    {
        IsInitialized = true;
        IsSuccess = false;
        _error = error;
        _value = default!;
    }

    /// <summary>
    /// Creates a success (Ok).
    /// <para>成功(Result.Ok)を生成する。</para>
    /// </summary>
    /// <param name="value">
    /// The success value. Must not be null.
    /// <para>成功時の値。nullは禁止。</para>
    /// </param>
    /// <returns>
    /// A successful Result containing the value.
    /// <para>値を持つ成功Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if value is null.
    /// <para>valueがnullの場合に投げられる。</para>
    /// </exception>
    public static Result<E, T> Ok(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new Result<E, T>(value);
    }

    /// <summary>
    /// Creates a failure.
    /// <para>失敗(Result.Fail)を生成する。</para>
    /// </summary>
    /// <param name="error">
    /// The error value. Must not be null.
    /// <para>エラーの値。nullは禁止。</para>
    /// </param>
    /// <returns>
    /// A failed Result containing the error.
    /// <para>エラーを持つ失敗Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if error is null.
    /// <para>errorがnullの場合に投げられる。</para>
    /// </exception>
    public static Result<E, T> Fail(E error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result<E, T>(error);
    }

    /// <summary>
    /// Matches success or failure and returns a value.
    /// <para>成功/失敗に応じた関数を適用し値を返す。</para>
    /// </summary>
    /// <typeparam name="U">
    /// The return type.
    /// <para>戻り値の型。</para>
    /// </typeparam>
    /// <param name="onSuccess">
    /// A function to apply if successful. Must not return null.
    /// <para>成功時に適用する関数。nullを返してはならない。</para>
    /// </param>
    /// <param name="onFailure">
    /// A function to apply if failed. Must not return null.
    /// <para>失敗時に適用する関数。nullを返してはならない。</para>
    /// </param>
    /// <returns>
    /// The result of the applied function.
    /// <para>適用した関数の戻り値。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if onSuccess or onFailure is null.
    /// <para>onSuccessまたはonFailureがnullの場合に投げられる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this Result is uninitialized or if the applied function returns null.
    /// <para>この Result が未初期化、または適用した関数がnullを返した場合に投げられる。</para>
    /// </exception>
    public U Match<U>(Func<T, U> onSuccess, Func<E, U> onFailure)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        var value = IsSuccess ? onSuccess(_value) : onFailure(_error);

        if (value is null)
            throw new InvalidOperationException("Match function must not return null.");

        return value;
    }

    /// <summary>
    /// Matches success or failure and executes an action.
    /// <para>成功/失敗に応じたアクションを実行する。</para>
    /// </summary>
    /// <param name="onSuccess">
    /// An action to execute if successful.
    /// <para>成功時に実行するアクション。</para>
    /// </param>
    /// <param name="onFailure">
    /// An action to execute if failed.
    /// <para>失敗時に実行するアクション。</para>
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if onSuccess or onFailure is null.
    /// <para>onSuccessまたはonFailureがnullの場合に投げられる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this Result is uninitialized.
    /// <para>この Result が未初期化の場合に投げられる。</para>
    /// </exception>
    public void Match(Action<T> onSuccess, Action<E> onFailure)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        if (IsSuccess)
            onSuccess(_value);
        else
            onFailure(_error);
    }

    /// <summary>
    /// Applies a function returning Result and flattens the result.
    /// <para>Resultを返す関数を適用し、ネストを解消する。</para>
    ///
    /// This is used to chain operations that may fail.
    /// <para>失敗しうる処理を連結するために使う。</para>
    /// </summary>
    /// <typeparam name="U">
    /// The type of the value in the returned Result.
    /// <para>返されるResultの値の型。</para>
    /// </typeparam>
    /// <param name="binder">
    /// A function that takes the value and returns a new Result.
    /// <para>値を受け取り新しいResultを返す関数。</para>
    /// </param>
    /// <returns>
    /// The Result returned by binder, or the original failure.
    /// <para>binderが返すResult、または元の失敗Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if binder is null.
    /// <para>binderがnullの場合に投げられる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this Result is uninitialized or if binder returns an uninitialized Result.
    /// <para>この Result が未初期化、または binder が未初期化の Result を返した場合に投げられる。</para>
    /// </exception>
    public Result<E, U> Bind<U>(Func<T, Result<E, U>> binder)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(binder);

        if (!IsSuccess)
            return Result<E, U>.Fail(_error);

        var next = binder(_value);
        next.ThrowIfNotInitialized();

        return next;
    }

    /// <summary>
    /// Transforms the successful value by using the specified selector.
    /// <para>成功値を指定された selector で変換する。</para>
    ///
    /// If this Result is a failure, the original error is preserved and
    /// <paramref name="selector"/> is not invoked.
    /// If <paramref name="selector"/> returns null,
    /// an <see cref="InvalidOperationException"/> is thrown.
    /// <para>
    /// この Result が失敗している場合は元のエラーを保持し、
    /// <paramref name="selector"/> は実行しない。
    /// <paramref name="selector"/> が null を返した場合は、
    /// <see cref="InvalidOperationException"/> をスローする。
    /// </para>
    /// </summary>
    /// <typeparam name="U">
    /// The type of the transformed value.
    /// <para>変換後の値の型。</para>
    /// </typeparam>
    /// <param name="selector">
    /// A function that transforms the successful value.
    /// Must not be null and must not return null.
    /// <para>
    /// 成功値を変換する関数。
    /// null は許可されず、null を返してはならない。
    /// </para>
    /// </param>
    /// <returns>
    /// A Result containing the transformed value when successful;
    /// otherwise a failed Result containing the original error.
    /// <para>
    /// 成功している場合は変換後の値を保持する Result。
    /// 失敗している場合は元のエラーを保持する失敗 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> is null.
    /// <para><paramref name="selector"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized,
    /// or when <paramref name="selector"/> returns null.
    /// <para>
    /// この Result が未初期化、
    /// または <paramref name="selector"/> が null を返した場合にスローされる。
    /// </para>
    /// </exception>
    public Result<E, U> Map<U>(Func<T, U> selector)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(selector);

        if (!IsSuccess)
            return Result<E, U>.Fail(_error);

        var value = selector(_value);

        if (value is null)
            throw new InvalidOperationException("Selector must not return null.");

        return Result<E, U>.Ok(value);
    }

    /// <summary>
    /// Maps only the error in case of failure.
    /// If it is successful, the value remains unchanged.
    /// <para>
    /// 失敗時のエラーだけを変換する。
    /// 成功時は値そのまま。
    /// </para>
    /// </summary>
    /// <typeparam name="E1">
    /// The target error type.
    /// <para>変換後のエラー型。</para>
    /// </typeparam>
    /// <param name="errorMapper">
    /// A function to transform the error. Must not return null.
    /// <para>エラーを変換する関数。nullを返してはならない。</para>
    /// </param>
    /// <returns>
    /// A Result with the mapped error, or the original success.
    /// <para>変換後のエラーを持つResult、または元の成功Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if errorMapper is null.
    /// <para>errorMapperがnullの場合に投げられる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this Result is uninitialized or if errorMapper returns null.
    /// <para>この Result が未初期化、または errorMapper がnullを返した場合に投げられる。</para>
    /// </exception>
    public Result<E1, T> MapError<E1>(Func<E, E1> errorMapper)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(errorMapper);

        if (!IsSuccess)
        {
            var error = errorMapper(_error);

            if (error is null)
                throw new InvalidOperationException("Error mapper must not return null.");

            return Result<E1, T>.Fail(error);
        }

        return Result<E1, T>.Ok(_value);
    }

    /// <summary>
    /// Validates the value using a predicate. Converts to failure if the predicate fails.
    /// <para>値に条件を課し、違反時は失敗Resultへ変換する。</para>
    /// </summary>
    /// <param name="predicate">
    /// A function to test the value.
    /// <para>値を検証する関数。</para>
    /// </param>
    /// <param name="errorFactory">
    /// A function to create an error from the value when the predicate fails.
    /// <para>条件違反時に値からエラーを生成する関数。</para>
    /// </param>
    /// <returns>
    /// The original Result if successful and predicate passes; otherwise a failure.
    /// <para>成功かつ条件を満たす場合は元のResult、それ以外は失敗Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if predicate or errorFactory is null.
    /// <para>predicateまたはerrorFactoryがnullの場合に投げられる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this Result is uninitialized or if errorFactory returns null.
    /// <para>この Result が未初期化、または errorFactory がnullを返した場合に投げられる。</para>
    /// </exception>
    public Result<E, T> Ensure(Func<T, bool> predicate, Func<T, E> errorFactory)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(errorFactory);

        if (!IsSuccess)
            return this;

        if (predicate(_value))
            return this;

        var error = errorFactory(_value);

        if (error is null)
            throw new InvalidOperationException("Error factory must not return null.");

        return Fail(error);
    }

    /// <summary>
    /// Performs an action if successful, then returns the original Result.
    /// <para>成功時に副作用を実行し、元のResultを返す。</para>
    /// </summary>
    /// <param name="onSuccess">
    /// An action to execute on the value if successful.
    /// <para>成功時に値に対して実行するアクション。</para>
    /// </param>
    /// <returns>
    /// The original Result unchanged.
    /// <para>変更されていない元のResult。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if onSuccess is null.
    /// <para>onSuccessがnullの場合に投げられる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this Result is uninitialized.
    /// <para>この Result が未初期化の場合に投げられる。</para>
    /// </exception>
    public Result<E, T> Tap(Action<T> onSuccess)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(onSuccess);

        if (!IsSuccess)
            return this;

        onSuccess(_value);

        return this;
    }

    /// <summary>
    /// Performs an action if failed, then returns the original Result.
    /// <para>失敗時に副作用を実行し、元のResultを返す。</para>
    /// </summary>
    /// <param name="onFailure">
    /// An action to execute on the error if failed.
    /// <para>失敗時にエラーに対して実行するアクション。</para>
    /// </param>
    /// <returns>
    /// The original Result unchanged.
    /// <para>変更されていない元のResult。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if onFailure is null.
    /// <para>onFailureがnullの場合に投げられる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if this Result is uninitialized.
    /// <para>この Result が未初期化の場合に投げられる。</para>
    /// </exception>
    public Result<E, T> TapError(Action<E> onFailure)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(onFailure);

        if (IsSuccess)
            return this;

        onFailure(_error);

        return this;
    }

    /// <summary>
    /// Returns the string representation of Result.
    /// <para>Result の文字列表現を返す。</para>
    /// </summary>
    /// <returns>
    /// "Ok(value)" if successful, "Fail(error)" if failed, or "uninitialized" if not initialized.
    /// <para>成功時は "Ok(value)"、失敗時は "Fail(error)"、未初期化時は "uninitialized"。</para>
    /// </returns>
    public override string ToString()
    {
        if (!IsInitialized)
            return $"Result<{typeof(E).Name}, {typeof(T).Name}>(uninitialized)";

        return IsSuccess ? $"Ok({_value})" : $"Fail({_error})";
    }

    #region Equality

    public bool Equals(Result<E, T> other)
    {
        if (IsInitialized != other.IsInitialized)
            return false;

        if (!IsInitialized)
            return true; // 未初期化同士は等しいとみなす

        if (IsSuccess != other.IsSuccess)
            return false;

        if (IsSuccess)
            return EqualityComparer<T>.Default.Equals(_value, other._value);

        return EqualityComparer<E>.Default.Equals(_error, other._error);
    }

    public override bool Equals(object? obj)
    {
        return obj is Result<E, T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        if (!IsInitialized)
            return 0;

        unchecked
        {
            // 成功か失敗かで分岐し、それぞれの値をハッシュに含める
            int hash = 17;
            hash = hash * 23 + IsSuccess.GetHashCode();

            if (IsSuccess)
                hash = hash * 23 + EqualityComparer<T>.Default.GetHashCode(_value!);
            else
                hash = hash * 23 + EqualityComparer<E>.Default.GetHashCode(_error!);

            return hash;
        }
    }

    public static bool operator ==(Result<E, T> left, Result<E, T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Result<E, T> left, Result<E, T> right)
    {
        return !(left == right);
    }

    #endregion
}
