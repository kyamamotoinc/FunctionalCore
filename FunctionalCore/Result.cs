namespace FunctionalCore;

/// <summary>
/// Represents a result that can be either a success with a value,
/// or a failure with an error.
/// 成功時の値、または失敗時のエラーを保持する型
/// </summary>
/// <typeparam name="E">Type of error / エラーの型</typeparam>
/// <typeparam name="T">Type of value / 値の型</typeparam>
public readonly struct Result<E, T> : IEquatable<Result<E, T>>
{
    /// <summary>
    /// Indicates whether the result is successful
    /// 成功かどうかを示す
    /// </summary>
    public bool IsSuccess { get; }

    private readonly T _value;
    /// <summary>
    /// Gets the value if successful. Throws if failed.
    /// 成功時の値を取得。失敗時は例外を投げる
    /// </summary>
    public T Value
    {
        get
        {
            if (!IsSuccess)
                throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> does not contain a value.");

            return _value;
        }
    }

    private readonly E _error;
    /// <summary>
    /// Gets the error if failed. Throws if successful.
    /// 失敗時のエラーを取得。成功時は例外を投げる
    /// </summary>
    public E Error
    {
        get
        {
            if (IsSuccess)
                throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> does not contain an error.");

            return _error;
        }
    }

    /// <summary>
    /// Private constructor for success
    /// 成功時のプライベートコンストラクタ
    /// </summary>
    /// <param name="value">値</param>
    private Result(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        IsSuccess = true;
        _error = default!;
        _value = value;
    }

    /// <summary>
    /// Private constructor for failure
    /// 失敗時のプライベートコンストラクタ
    /// </summary>
    /// <param name="error"></param>
    private Result(E error)
    {
        ArgumentNullException.ThrowIfNull(error);

        IsSuccess = false;
        _error = error;
        _value = default!;
    }

    /// <summary>
    /// Creates a successful Result
    /// 成功のResultを作成
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static Result<E, T> Ok(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new Result<E, T>(value);
    }

    /// <summary>
    /// Creates a failed Result
    /// 失敗のResultを作成
    /// </summary>
    /// <param name="error"></param>
    /// <returns></returns>
    public static Result<E, T> Fail(E error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result<E, T>(error);
    }

    /// <summary>
    /// Applies a function to the value if it is successful, otherwise returns failure
    /// 成功時は関数を適用し、失敗時はそのまま返す
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public Result<E, U> Map<U>(Func<T, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (!IsSuccess)
            return Result<E, U>.Fail(Error);

        var value = selector(_value);

        if (value is null)
            throw new InvalidOperationException("Selector must not return null.");

        return Result<E, U>.Ok(value);
    }

    /// <summary>
    /// Applies a function returning Result if successful, otherwise returns failure
    /// 成功時はResultを返す関数を適用し、失敗時はそのまま返す
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="binder"></param>
    /// <returns></returns>
    public Result<E, U> Bind<U>(Func<T, Result<E, U>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);

        return IsSuccess ? binder(_value) : Result<E, U>.Fail(_error);
    }

    #region For LINQ

    /// <summary>
    /// Maps value to a new Result. Supports LINQ query syntax.
    /// 値をマップして新しいResultを返す。LINQ構文対応
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public Result<E, U> Select<U>(Func<T, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        return Map(selector);
    }

    /// <summary>
    /// Maps and flattens nested Results. Supports LINQ query syntax.
    /// Resultを返す関数を適用しフラット化する。LINQ構文対応
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <typeparam name="V"></typeparam>
    /// <param name="selector"></param>
    /// <param name="projector"></param>
    /// <returns></returns>
    public Result<E, V> SelectMany<U, V>(Func<T, Result<E, U>> selector, Func<T, U, V> projector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(projector);

        return Bind(x => selector(x).Map(y => projector(x, y)));
    }

    #endregion

    /// <summary>
    /// Maps only the error in case of failure.
    /// If it is successful, the value remains unchanged.
    /// 失敗時のエラーだけを変換する
    /// 成功時は値そのまま
    /// </summary>
    /// <typeparam name="E1">The target error type / 変換後のエラー型</typeparam>
    /// <param name="errorMapper">Function to map the error / 変換関数</param>
    /// <returns>The Result with mapped error / 変換後の Result</returns>
    public Result<E1, T> MapError<E1>(Func<E, E1> errorMapper)
    {
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
    /// Performs an action if successful, then returns the original Result.
    /// 成功時に副作用を実行し、元のResultを返す
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result<E, T> Tap(Action<T> onSuccess)
    {
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
    /// 失敗時に副作用を実行し、元のResultを返す
    /// </summary>
    /// <param name="onFailure"></param>
    /// <returns></returns>
    public Result<E, T> TapError(Action<E> onFailure)
    {
        ArgumentNullException.ThrowIfNull(onFailure);

        if (IsSuccess)
        {
            return this;
        }
        onFailure(_error);

        return this;
    }

    /// <summary>
    /// Performs an action regardless of success or failure
    /// 成功・失敗問わず副作用を実行する
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public Result<E, T> TapBoth(Action<Result<E, T>> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        action(this);

        return this;
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
    /// Matches success or failure and returns a value
    /// 成功/失敗に応じた関数を適用し値を返す
    /// </summary>
    /// <typeparam name="U"></typeparam>
    /// <param name="funcOnSuccess"></param>
    /// <param name="funcOnFailure"></param>
    /// <returns></returns>
    public U Match<U>(Func<T, U> funcOnSuccess, Func<E, U> funcOnFailure)
    {
        ArgumentNullException.ThrowIfNull(funcOnSuccess);
        ArgumentNullException.ThrowIfNull(funcOnFailure);

        var value = IsSuccess ? funcOnSuccess(_value) : funcOnFailure(_error);
        if (value is null)
            throw new InvalidOperationException("Match function must not return null.");

        return value;
    }

    /// <summary>
    /// Returns the value if it is successful; otherwise returns the default value of <typeparamref name="T"/>.
    /// 成功した場合はその値を返し、失敗した場合は <typeparamref name="T"/> の既定値を返します。
    /// </summary>
    /// <returns>
    /// The contained value if it is successful; otherwise the default value of <typeparamref name="T"/>.
    /// 成功した場合はその値、失敗した場合は既定値。
    /// </returns>
    public T GetValueOrDefault()
    {
        return IsSuccess ? _value : default!;
    }

    /// <summary>
    /// Returns the value if it is successful; otherwise returns the specified fallback value.
    /// 成功した場合はその値を返し、失敗した場合は指定した代替値を返します。
    /// </summary>
    /// <param name="defaultValue">
    /// The value to return if the result is a failure.
    /// 失敗した場合に返す代替値。
    /// </param>
    /// <returns>
    /// The contained value if it is successful; otherwise the specified fallback value.
    /// 成功した場合はその値、失敗した場合は指定した代替値。
    /// </returns>
    public T GetValueOr(T defaultValue)
    {
        return IsSuccess ? _value : defaultValue;
    }

    /// <summary>
    /// Returns this result if it is successful; otherwise returns the specified alternative result.
    /// 成功した場合はこの Result を返し、失敗した場合には指定した代替 Result を返します。
    /// </summary>
    /// <param name="other">
    /// The alternative result to return if this result is fail.
    /// 失敗した場合に返す代替の Result。
    /// </param>
    /// <returns>
    /// This result if it is successful; otherwise the specified alternative result.
    /// 成功した場合はこの Result、それ以外は代替 Result。
    /// </returns>
    public Result<E, T> Or(Result<E, T> other)
    {
        return IsSuccess ? this : other;
    }

    /// <summary>
    /// Returns this result if it is successful; otherwise returns a result produced by the specified factory.
    /// 成功した場合はこの Result を返し、失敗した場合は指定した関数で生成された Result を返します。
    /// </summary>
    /// <param name="otherFactory">
    /// A function that produces an alternative result if no value is present.
    /// 失敗した場合に代替 Result を生成する関数。
    /// </param>
    /// <returns>
    /// This result if it is successful; otherwise the result produced by the factory.
    /// 成功した場合はこの Result、それ以外は生成された Result。
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="otherFactory"/> is null.
    /// <paramref name="otherFactory"/> が null の場合にスローされます。
    /// </exception>
    public Result<E, T> Or(Func<Result<E, T>> otherFactory)
    {
        ArgumentNullException.ThrowIfNull(otherFactory);

        return IsSuccess ? this : otherFactory();
    }

    public override string ToString()
    {
        return IsSuccess ? $"Success({_value})" : $"Failure({_error})";
    }

    // --- Equality ---
    public bool Equals(Result<E, T> other)
    {
        if (IsSuccess != other.IsSuccess) return false;
        if (IsSuccess) return EqualityComparer<T>.Default.Equals(_value, other._value);

        return EqualityComparer<E>.Default.Equals(_error, other._error);
    }

    public override bool Equals(object obj)
    {
        return obj is Result<E, T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            // 成功か失敗かで分岐し、それぞれの値をハッシュに含める
            int hash = 17;
            hash = hash * 23 + IsSuccess.GetHashCode();

            if (IsSuccess)
            {
                hash = hash * 23 + EqualityComparer<T>.Default.GetHashCode(_value);
            }
            else
            {
                hash = hash * 23 + EqualityComparer<E>.Default.GetHashCode(_error);
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
}