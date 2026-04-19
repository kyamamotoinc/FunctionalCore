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
public readonly struct Result<E, T> : IEquatable<Result<E, T>>
{
    private readonly bool _isInitialized;

    internal bool IsInitialized => _isInitialized;
    /// <summary>
    /// Indicates whether the result is successful
    /// 成功かどうかを示す
    /// </summary>
    public bool IsSuccess { get; }

    private readonly T _value;
    /// <summary>
    /// Gets the value if successful.
    /// 成功時の値を取得する
    ///
    /// Throws if the result is failure.
    /// 失敗時は例外を投げる
    /// </summary>
    public T Value
    {
        get
        {
            ThrowIfNotInitialized();
            if (!IsSuccess)
                throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> does not contain a value.");

            return _value;
        }
    }

    private readonly E _error;
    /// <summary>
    /// Gets the error if failed.
    /// 失敗時のエラーを取得する
    ///
    /// Throws if the result is success.
    /// 成功時は例外を投げる
    /// </summary>
    public E Error
    {
        get
        {
            ThrowIfNotInitialized();
            if (IsSuccess)
                throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> does not contain an error.");

            return _error;
        }
    }

    /// <summary>
    /// Creates a successful result.
    /// 成功結果を生成する
    ///
    /// null is not allowed.
    /// nullは禁止
    /// </summary>
    private Result(T value)
    {
        _isInitialized = true;
        IsSuccess = true;
        _error = default!;
        _value = value;
    }

    /// <summary>
    /// Creates a failed result.
    /// 失敗結果を生成する
    ///
    /// null is not allowed.
    /// nullは禁止
    /// </summary>
    private Result(E error)
    {
        _isInitialized = true;
        IsSuccess = false;
        _error = error;
        _value = default!;
    }

    private void ThrowIfNotInitialized()
    {
        if (!_isInitialized)
            throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> is not initialized.");
    }

    /// <summary>
    /// Creates a success (Ok).
    /// 成功(Result.Ok)
    /// </summary>
    public static Result<E, T> Ok(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new Result<E, T>(value);
    }

    /// <summary>
    /// Creates a failure.
    /// 失敗(Result.Fail)
    /// </summary>
    public static Result<E, T> Fail(E error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result<E, T>(error);
    }

    /// <summary>
    /// Transforms the value if successful.
    /// 成功時のみ値を変換する
    ///
    /// If selector returns null, it throws.
    /// nullは許可されない（例外）
    /// </summary>
    public Result<E, U> Map<U>(Func<T, U> selector)
    {
        ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(selector);

        if (!IsSuccess)
            return Result<E, U>.Fail(Error);

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
    /// <typeparam name="E1">The target error type / 変換後のエラー型</typeparam>
    /// <param name="errorMapper">Function to map the error / 変換関数</param>
    /// <returns>The Result with mapped error / 変換後の Result</returns>
    public Result<E1, T> MapError<E1>(Func<E, E1> errorMapper)
    {
        ThrowIfNotInitialized();
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
    /// Resultを返す関数を適用し、ネストを解消する
    ///
    /// This is used to chain operations that may fail.
    /// 失敗しうる処理を連結するために使う
    /// </summary>
    public Result<E, U> Bind<U>(Func<T, Result<E, U>> binder)
    {
        ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(binder);

        return IsSuccess ? binder(_value) : Result<E, U>.Fail(_error);
    }

    /// <summary>
    /// Matches success or failure and returns a value
    /// 成功/失敗に応じた関数を適用し値を返す
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    /// <returns></returns>
    public U Match<U>(Func<T, U> onSuccess, Func<E, U> onFailure)
    {
        ThrowIfNotInitialized();
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
    /// <typeparam name="U"></typeparam>
    /// <param name="onSuccess"></param>
    /// <param name="onFailure"></param>
    /// <returns></returns>
    public void Match(Action<T> onSuccess, Action<E> onFailure)
    {
        ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        if (IsSuccess)
            onSuccess(_value);
        else
            onFailure(_error);
    }

    /// <summary>
    /// Validates the value using a predicate. Converts to failure if the predicate fails.
    /// 値に条件を課し、違反時は失敗Resultへ変換
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="errorFactory">Function to create an error from the value when the predicate fails</param>
    /// <returns></returns>
    public Result<E, T> Ensure(Func<T, bool> predicate, Func<T, E> errorFactory)
    {
        ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(errorFactory);

        if (!IsSuccess)
        {
            return this;
        }

        if (predicate(_value))
        {
            return this;
        }

        var error = errorFactory(_value);
        if (error is null)
            throw new InvalidOperationException("Error factory must not return null.");

        return Fail(error);
    }

    /// <summary>
    /// Performs an action if successful, then returns the original Result.
    /// 成功時に副作用を実行し、元のResultを返す。
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result<E, T> Tap(Action<T> onSuccess)
    {
        ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(onSuccess);

        if (!IsSuccess)
        {
            return this;
        }
        onSuccess(_value);

        return this;
    }

    /// <summary>
    /// Performs an action if failed, then returns the original Result.
    /// 失敗時に副作用を実行し、元のResultを返す。
    /// </summary>
    /// <param name="onFailure"></param>
    /// <returns></returns>
    public Result<E, T> TapError(Action<E> onFailure)
    {
        ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(onFailure);

        if (IsSuccess)
        {
            return this;
        }
        onFailure(_error);

        return this;
    }

    public override string ToString()
    {
        if (!_isInitialized)
            return $"Result<{typeof(E).Name}, {typeof(T).Name}>(uninitialized)";
        return IsSuccess ? $"Ok({_value})" : $"Fail({_error})";
    }

    #region Equality
    public bool Equals(Result<E, T> other)
    {
        if (_isInitialized != other._isInitialized) return false;
        if (!_isInitialized) return true; // 未初期化同士は等しいとみなす
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
        if (!_isInitialized) return 0;
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