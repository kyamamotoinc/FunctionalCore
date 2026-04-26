namespace FunctionalCore;

/// <summary>
/// Represents the result of an operation (Success or Failure).
/// 処理結果（成功 / 失敗）を表現する型。
///
/// This type replaces exceptions with explicit error values.
/// 例外の代わりに、失敗を値として扱う。
///
/// Design rules:
/// - Success always contains a value
/// - Failure always contains an error
/// - null is not allowed
/// - operations must preserve these invariants
///
/// 設計ルール:
/// - Success は必ず値を持つ
/// - Failure は必ずエラーを持つ
/// - nullは禁止
/// - すべての操作はこの不変条件を守る
/// </summary>
/// <typeparam name="E">The error type. / エラーの型。</typeparam>
/// <typeparam name="T">The success value type. / 成功時の値の型。</typeparam>
public readonly struct Result<E, T> : IEquatable<Result<E, T>>
{
    /// <summary>
    /// Indicates whether the result is successful
    /// 成功かどうかを示す。
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the result is initialized.
    /// Uninitialized results do not allow access to Value and Error.
    /// 初期化済みかどうかを示す。
    /// 未初期化のResultはValueとErrorにアクセスできない。
    /// </summary>
    internal bool IsInitialized { get; }

    private readonly T _value;
    /// <summary>
    /// Gets the value if successful.
    /// 成功時の値を取得する。
    ///
    /// Throws if the result is uninitialized or failure.
    /// 未初期化または失敗時は例外を投げる。
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if uninitialized or if the result is a failure.
    /// 未初期化または失敗の場合に投げられる。
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
    /// 失敗時のエラーを取得する。
    ///
    /// Throws if the result is uninitialized or success.
    /// 未初期化または成功時は例外を投げる。
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if uninitialized or if the result is a success.
    /// 未初期化または成功の場合に投げられる。
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
    /// 成功結果を生成する。
    ///
    /// null is not allowed.
    /// nullは禁止。
    /// </summary>
    /// <param name="value">The success value. / 成功時の値。</param>
    private Result(T value)
    {
        IsInitialized = true;
        IsSuccess = true;
        _error = default!;
        _value = value;
    }

    /// <summary>
    /// Creates a failed result.
    /// 失敗結果を生成する。
    ///
    /// null is not allowed.
    /// nullは禁止。
    /// </summary>
    /// <param name="error">The error value. / エラーの値。</param>
    private Result(E error)
    {
        IsInitialized = true;
        IsSuccess = false;
        _error = error;
        _value = default!;
    }

    /// <summary>
    /// Creates a success (Ok).
    /// 成功(Result.Ok)を生成する。
    /// </summary>
    /// <param name="value">The success value. Must not be null. / 成功時の値。nullは禁止。</param>
    /// <returns>A successful Result containing the value. / 値を持つ成功Result。</returns>
    /// <exception cref="ArgumentNullException">Thrown if value is null. / valueがnullの場合に投げられる。</exception>
    public static Result<E, T> Ok(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new Result<E, T>(value);
    }

    /// <summary>
    /// Creates a failure.
    /// 失敗(Result.Fail)を生成する。
    /// </summary>
    /// <param name="error">The error value. Must not be null. / エラーの値。nullは禁止。</param>
    /// <returns>A failed Result containing the error. / エラーを持つ失敗Result。</returns>
    /// <exception cref="ArgumentNullException">Thrown if error is null. / errorがnullの場合に投げられる。</exception>
    public static Result<E, T> Fail(E error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result<E, T>(error);
    }

    /// <summary>
    /// Transforms the value if successful.
    /// 成功時のみ値を変換する。
    ///
    /// If selector returns null, it throws.
    /// nullは許可されない。（例外を投げる）
    /// </summary>
    /// <typeparam name="U">The type of the transformed value. / 変換後の値の型。</typeparam>
    /// <param name="selector">A function to transform the value. Must not return null. / 値を変換する関数。nullを返してはならない。</param>
    /// <returns>A Result with the transformed value, or the original failure. / 変換後の値を持つResult、または元の失敗Result。</returns>
    /// <exception cref="ArgumentNullException">Thrown if selector is null. / selectorがnullの場合に投げられる。</exception>
    /// <exception cref="InvalidOperationException">Thrown if selector returns null. / selectorがnullを返した場合に投げられる。</exception>
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
    /// 失敗時のエラーだけを変換する。
    /// 成功時は値そのまま。
    /// </summary>
    /// <typeparam name="E1">The target error type. / 変換後のエラー型。</typeparam>
    /// <param name="errorMapper">A function to transform the error. Must not return null. / エラーを変換する関数。nullを返してはならない。</param>
    /// <returns>A Result with the mapped error, or the original success. / 変換後のエラーを持つResult、または元の成功Result。</returns>
    /// <exception cref="ArgumentNullException">Thrown if errorMapper is null. / errorMapperがnullの場合に投げられる。</exception>
    /// <exception cref="InvalidOperationException">Thrown if errorMapper returns null. / errorMapperがnullを返した場合に投げられる。</exception>
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
    /// Applies a function returning Result and flattens the result.
    /// Resultを返す関数を適用し、ネストを解消する。
    ///
    /// This is used to chain operations that may fail.
    /// 失敗しうる処理を連結するために使う。
    /// </summary>
    /// <typeparam name="U">The type of the value in the returned Result. / 返されるResultの値の型。</typeparam>
    /// <param name="binder">A function that takes the value and returns a new Result. / 値を受け取り新しいResultを返す関数。</param>
    /// <returns>The Result returned by binder, or the original failure. / binderが返すResult、または元の失敗Result。</returns>
    /// <exception cref="ArgumentNullException">Thrown if binder is null. / binderがnullの場合に投げられる。</exception>
    public Result<E, U> Bind<U>(Func<T, Result<E, U>> binder)
    {
        this.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(binder);

        return IsSuccess ? binder(_value) : Result<E, U>.Fail(_error);
    }

    /// <summary>
    /// Matches success or failure and returns a value.
    /// 成功/失敗に応じた関数を適用し値を返す。
    /// </summary>
    /// <typeparam name="U">The return type. / 戻り値の型。</typeparam>
    /// <param name="onSuccess">A function to apply if successful. Must not return null. / 成功時に適用する関数。nullを返してはならない。</param>
    /// <param name="onFailure">A function to apply if failed. Must not return null. / 失敗時に適用する関数。nullを返してはならない。</param>
    /// <returns>The result of the applied function. / 適用した関数の戻り値。</returns>
    /// <exception cref="ArgumentNullException">Thrown if onSuccess or onFailure is null. / onSuccessまたはonFailureがnullの場合に投げられる。</exception>
    /// <exception cref="InvalidOperationException">Thrown if the applied function returns null. / 適用した関数がnullを返した場合に投げられる。</exception>
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
    /// 成功/失敗に応じたアクションを実行する。
    /// </summary>
    /// <param name="onSuccess">An action to execute if successful. / 成功時に実行するアクション。</param>
    /// <param name="onFailure">An action to execute if failed. / 失敗時に実行するアクション。</param>
    /// <exception cref="ArgumentNullException">Thrown if onSuccess or onFailure is null. / onSuccessまたはonFailureがnullの場合に投げられる。</exception>
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
    /// Validates the value using a predicate. Converts to failure if the predicate fails.
    /// 値に条件を課し、違反時は失敗Resultへ変換する。
    /// </summary>
    /// <param name="predicate">A function to test the value. / 値を検証する関数。</param>
    /// <param name="errorFactory">A function to create an error from the value when the predicate fails. / 条件違反時に値からエラーを生成する関数。</param>
    /// <returns>The original Result if successful and predicate passes; otherwise a failure. / 成功かつ条件を満たす場合は元のResult、それ以外は失敗Result。</returns>
    /// <exception cref="ArgumentNullException">Thrown if predicate or errorFactory is null. / predicateまたはerrorFactoryがnullの場合に投げられる。</exception>
    /// <exception cref="InvalidOperationException">Thrown if errorFactory returns null. / errorFactoryがnullを返した場合に投げられる。</exception>
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
    /// 成功時に副作用を実行し、元のResultを返す。
    /// </summary>
    /// <param name="onSuccess">An action to execute on the value if successful. / 成功時に値に対して実行するアクション。</param>
    /// <returns>The original Result unchanged. / 変更されていない元のResult。</returns>
    /// <exception cref="ArgumentNullException">Thrown if onSuccess is null. / onSuccessがnullの場合に投げられる。</exception>
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
    /// 失敗時に副作用を実行し、元のResultを返す。
    /// </summary>
    /// <param name="onFailure">An action to execute on the error if failed. / 失敗時にエラーに対して実行するアクション。</param>
    /// <returns>The original Result unchanged. / 変更されていない元のResult。</returns>
    /// <exception cref="ArgumentNullException">Thrown if onFailure is null. / onFailureがnullの場合に投げられる。</exception>
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
    /// Result の文字列表現を返す。
    /// </summary>
    /// <returns>
    /// "Ok(value)" if successful, "Fail(error)" if failed, or "uninitialized" if not initialized.
    /// 成功時は "Ok(value)"、失敗時は "Fail(error)"、未初期化時は "uninitialized"。
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
        if (IsInitialized != other.IsInitialized) return false;
        if (!IsInitialized) return true; // 未初期化同士は等しいとみなす
        if (IsSuccess != other.IsSuccess) return false;
        if (IsSuccess) return EqualityComparer<T>.Default.Equals(_value, other._value);

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
            {
                hash = hash * 23 + EqualityComparer<T>.Default.GetHashCode(_value!);
            }
            else
            {
                hash = hash * 23 + EqualityComparer<E>.Default.GetHashCode(_error!);
            }

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