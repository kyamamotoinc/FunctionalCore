namespace FunctionalCore;

/// <summary>
/// Represents the result of an operation as either success or failure.
/// <para>処理結果を成功または失敗として表現する型。</para>
///
/// Expected failures are represented as explicit error values rather than exceptions.
/// <para>想定内の処理失敗を、例外ではなく明示的なエラー値として扱う。</para>
///
/// <para>
/// Design rules:
/// <para>- Success always contains a non-null value.</para>
/// <para>- Failure always contains a non-null error.</para>
/// <para>- An uninitialized Result is neither success nor failure.</para>
/// <para>- Operations preserve these invariants.</para>
/// </para>
///
/// <para>
/// 設計ルール:
/// <para>- 成功時は必ず null ではない値を持つ。</para>
/// <para>- 失敗時は必ず null ではないエラーを持つ。</para>
/// <para>- 未初期化の Result は成功でも失敗でもない。</para>
/// <para>- すべての操作はこれらの不変条件を維持する。</para>
/// </para>
/// </summary>
/// <typeparam name="E">
/// The error type.
/// <para>失敗時のエラー型。</para>
/// </typeparam>
/// <typeparam name="T">
/// The success value type.
/// <para>成功時の値の型。</para>
/// </typeparam>
public readonly struct Result<E, T> : IEquatable<Result<E, T>>
{
    /// <summary>
    /// Indicates whether this Result represents success.
    /// <para>この Result が成功を表すかどうかを示す。</para>
    /// </summary>
    /// <remarks>
    /// Returns <see langword="false"/> for an uninitialized Result.
    /// <para>未初期化の Result では <see langword="false"/> を返す。</para>
    /// </remarks>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether this Result represents failure.
    /// <para>この Result が失敗を表すかどうかを示す。</para>
    /// </summary>
    /// <remarks>
    /// Returns <see langword="false"/> for an uninitialized Result.
    /// <para>未初期化の Result では <see langword="false"/> を返す。</para>
    /// </remarks>
    public bool IsFailure => IsInitialized && !IsSuccess;

    /// <summary>
    /// Indicates whether this Result has been initialized.
    /// <para>この Result が初期化済みかどうかを示す。</para>
    /// </summary>
    /// <remarks>
    /// An uninitialized Result does not allow access to <see cref="Value"/> or <see cref="Error"/>.
    /// <para>未初期化の Result では <see cref="Value"/> または <see cref="Error"/> にアクセスできない。</para>
    /// </remarks>
    internal bool IsInitialized { get; }

    private readonly T _value;

    /// <summary>
    /// Gets the success value.
    /// <para>成功時の値を取得する。</para>
    /// </summary>
    /// <value>
    /// The value contained in a successful Result.
    /// <para>成功している Result が保持する値。</para>
    /// </value>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized or represents failure.
    /// <para>この Result が未初期化、または失敗を表している場合にスローされる。</para>
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
    /// Gets the failure error.
    /// <para>失敗時のエラーを取得する。</para>
    /// </summary>
    /// <value>
    /// The error contained in a failed Result.
    /// <para>失敗している Result が保持するエラー。</para>
    /// </value>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized or represents success.
    /// <para>この Result が未初期化、または成功を表している場合にスローされる。</para>
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

    private Result(T value)
    {
        IsInitialized = true;
        IsSuccess = true;
        _error = default!;
        _value = value;
    }

    private Result(E error)
    {
        IsInitialized = true;
        IsSuccess = false;
        _error = error;
        _value = default!;
    }

    /// <summary>
    /// Creates a successful Result containing the specified value.
    /// <para>指定された値を保持する成功 Result を生成する。</para>
    /// </summary>
    /// <param name="value">
    /// The success value. Must not be null.
    /// <para>成功時の値。null は許可されない。</para>
    /// </param>
    /// <returns>
    /// A successful Result containing <paramref name="value"/>.
    /// <para><paramref name="value"/> を保持する成功 Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is null.
    /// <para><paramref name="value"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Result<E, T> Ok(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new Result<E, T>(value);
    }

    /// <summary>
    /// Creates a failed Result containing the specified error.
    /// <para>指定されたエラーを保持する失敗 Result を生成する。</para>
    /// </summary>
    /// <param name="error">
    /// The failure error. Must not be null.
    /// <para>失敗時のエラー。null は許可されない。</para>
    /// </param>
    /// <returns>
    /// A failed Result containing <paramref name="error"/>.
    /// <para><paramref name="error"/> を保持する失敗 Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="error"/> is null.
    /// <para><paramref name="error"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Result<E, T> Fail(E error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return new Result<E, T>(error);
    }

    /// <summary>
    /// Applies one of two functions according to the state of this Result and returns the produced value.
    /// <para>Result の成功または失敗に応じた関数を適用し、その結果を返す。</para>
    /// </summary>
    /// <typeparam name="U">
    /// The returned value type.
    /// <para>戻り値の型。</para>
    /// </typeparam>
    /// <param name="onSuccess">
    /// The function to execute for a successful Result.
    /// <para>成功時に実行する関数。</para>
    /// </param>
    /// <param name="onFailure">
    /// The function to execute for a failed Result.
    /// <para>失敗時に実行する関数。</para>
    /// </param>
    /// <returns>
    /// The value returned by the function corresponding to the state of this Result.
    /// <para>この Result の状態に対応する関数が返した値。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.
    /// <para><paramref name="onSuccess"/> または <paramref name="onFailure"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized, or when the selected function returns null.
    /// <para>この Result が未初期化、または選択された関数が null を返した場合にスローされる。</para>
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
    /// Executes one of two actions according to the state of this Result.
    /// <para>Result の成功または失敗に応じたアクションを実行する。</para>
    /// </summary>
    /// <param name="onSuccess">
    /// The action to execute for a successful Result.
    /// <para>成功時に実行するアクション。</para>
    /// </param>
    /// <param name="onFailure">
    /// The action to execute for a failed Result.
    /// <para>失敗時に実行するアクション。</para>
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="onSuccess"/> or <paramref name="onFailure"/> is null.
    /// <para><paramref name="onSuccess"/> または <paramref name="onFailure"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized.
    /// <para>この Result が未初期化の場合にスローされる。</para>
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
    /// Connects a successful Result to another Result-producing operation.
    /// <para>成功している Result を、次の Result を返す処理へ接続する。</para>
    /// </summary>
    /// <typeparam name="U">
    /// The success value type of the next Result.
    /// <para>次の Result の成功値の型。</para>
    /// </typeparam>
    /// <param name="binder">
    /// A function that receives the success value and produces the next Result.
    /// <para>成功値を受け取り、次の Result を生成する関数。</para>
    /// </param>
    /// <returns>
    /// The Result returned by <paramref name="binder"/> when this Result is successful;
    /// otherwise a failure containing the original error.
    /// <para>
    /// 成功時は <paramref name="binder"/> が返した Result。
    /// 失敗時は元のエラーを保持する失敗 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="binder"/> is null.
    /// <para><paramref name="binder"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized, or when <paramref name="binder"/> returns an uninitialized Result.
    /// <para>この Result が未初期化、または <paramref name="binder"/> が未初期化の Result を返した場合にスローされる。</para>
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
    /// Transforms the success value while preserving the current error type.
    /// <para>現在のエラー型を維持したまま、成功値を変換する。</para>
    /// </summary>
    /// <typeparam name="U">
    /// The transformed success value type.
    /// <para>変換後の成功値の型。</para>
    /// </typeparam>
    /// <param name="selector">
    /// A function that transforms the success value. Must not return null.
    /// <para>成功値を変換する関数。null を返してはならない。</para>
    /// </param>
    /// <returns>
    /// A successful Result containing the transformed value when this Result is successful;
    /// otherwise a failure containing the original error.
    /// <para>
    /// 成功時は変換後の値を保持する成功 Result。
    /// 失敗時は元のエラーを保持する失敗 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> is null.
    /// <para><paramref name="selector"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized, or when <paramref name="selector"/> returns null.
    /// <para>この Result が未初期化、または <paramref name="selector"/> が null を返した場合にスローされる。</para>
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
    /// Transforms the error of a failed Result while preserving the success value.
    /// <para>成功値を維持したまま、失敗時のエラーを変換する。</para>
    /// </summary>
    /// <typeparam name="E1">
    /// The transformed error type.
    /// <para>変換後のエラー型。</para>
    /// </typeparam>
    /// <param name="errorMapper">
    /// A function that transforms the error. Must not return null.
    /// <para>エラーを変換する関数。null を返してはならない。</para>
    /// </param>
    /// <returns>
    /// A failure containing the transformed error when this Result is failed;
    /// otherwise a success containing the original value.
    /// <para>
    /// 失敗時は変換後のエラーを保持する失敗 Result。
    /// 成功時は元の値を保持する成功 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="errorMapper"/> is null.
    /// <para><paramref name="errorMapper"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized, or when <paramref name="errorMapper"/> returns null.
    /// <para>この Result が未初期化、または <paramref name="errorMapper"/> が null を返した場合にスローされる。</para>
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
    /// Validates the success value and converts the Result to failure when the predicate is not satisfied.
    /// <para>成功値を検証し、条件を満たさない場合は失敗 Result に変換する。</para>
    /// </summary>
    /// <param name="predicate">
    /// A function that determines whether the success value satisfies the required condition.
    /// <para>成功値が条件を満たすかどうかを判定する関数。</para>
    /// </param>
    /// <param name="errorFactory">
    /// A function that creates the error when <paramref name="predicate"/> returns <see langword="false"/>.
    /// <para><paramref name="predicate"/> が <see langword="false"/> を返した場合にエラーを生成する関数。</para>
    /// </param>
    /// <returns>
    /// This Result when it is already failed or when the predicate succeeds;
    /// otherwise a failed Result containing the generated error.
    /// <para>
    /// 既に失敗している場合、または条件を満たす場合は元の Result。
    /// 条件を満たさない場合は生成されたエラーを保持する失敗 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicate"/> or <paramref name="errorFactory"/> is null.
    /// <para><paramref name="predicate"/> または <paramref name="errorFactory"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized, or when <paramref name="errorFactory"/> returns null.
    /// <para>この Result が未初期化、または <paramref name="errorFactory"/> が null を返した場合にスローされる。</para>
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
    /// Executes a side effect for a successful Result without changing the Result.
    /// <para>成功している Result に対して副作用を実行し、Result 自体は変更しない。</para>
    /// </summary>
    /// <param name="onSuccess">
    /// The action to execute for the success value.
    /// <para>成功値に対して実行するアクション。</para>
    /// </param>
    /// <returns>
    /// The original Result unchanged.
    /// <para>変更されていない元の Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="onSuccess"/> is null.
    /// <para><paramref name="onSuccess"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized.
    /// <para>この Result が未初期化の場合にスローされる。</para>
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
    /// Executes a side effect for a failed Result without changing the Result.
    /// <para>失敗している Result に対して副作用を実行し、Result 自体は変更しない。</para>
    /// </summary>
    /// <param name="onFailure">
    /// The action to execute for the error.
    /// <para>エラーに対して実行するアクション。</para>
    /// </param>
    /// <returns>
    /// The original Result unchanged.
    /// <para>変更されていない元の Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="onFailure"/> is null.
    /// <para><paramref name="onFailure"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Result is uninitialized.
    /// <para>この Result が未初期化の場合にスローされる。</para>
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
    /// Returns the string representation of this Result.
    /// <para>この Result の文字列表現を返す。</para>
    /// </summary>
    /// <returns>
    /// <c>Ok(value)</c> for success, <c>Fail(error)</c> for failure,
    /// or an uninitialized representation when this Result is not initialized.
    /// <para>
    /// 成功時は <c>Ok(value)</c>、失敗時は <c>Fail(error)</c>、
    /// 未初期化時は未初期化状態を表す文字列。
    /// </para>
    /// </returns>
    public override string ToString()
    {
        if (!IsInitialized)
            return $"Result<{typeof(E).Name}, {typeof(T).Name}>(uninitialized)";

        return IsSuccess ? $"Ok({_value})" : $"Fail({_error})";
    }

    #region Equality

    /// <summary>
    /// Determines whether this Result is equal to another Result.
    /// <para>この Result と指定された Result が等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="other">
    /// The Result to compare with this instance.
    /// <para>このインスタンスと比較する Result。</para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> when both Results have the same initialization state,
    /// the same success/failure state, and equal contained values or errors; otherwise <see langword="false"/>.
    /// Two uninitialized Results are considered equal.
    /// <para>
    /// 初期化状態、成功/失敗状態、および保持する値またはエラーが等しい場合は <see langword="true"/>。
    /// それ以外の場合は <see langword="false"/>。
    /// 未初期化の Result 同士は等しいものとして扱う。
    /// </para>
    /// </returns>
    public bool Equals(Result<E, T> other)
    {
        if (IsInitialized != other.IsInitialized) return false;
        if (!IsInitialized) return true;
        if (IsSuccess != other.IsSuccess) return false;
        if (IsSuccess) return EqualityComparer<T>.Default.Equals(_value, other._value);

        return EqualityComparer<E>.Default.Equals(_error, other._error);
    }

    /// <summary>
    /// Determines whether this Result is equal to the specified object.
    /// <para>この Result と指定されたオブジェクトが等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="obj">
    /// The object to compare with this instance.
    /// <para>このインスタンスと比較するオブジェクト。</para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> when <paramref name="obj"/> is an equal Result; otherwise <see langword="false"/>.
    /// <para><paramref name="obj"/> が等しい Result の場合は <see langword="true"/>、それ以外は <see langword="false"/>。</para>
    /// </returns>
    public override bool Equals(object? obj)
    {
        return obj is Result<E, T> other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this Result.
    /// <para>この Result のハッシュコードを返す。</para>
    /// </summary>
    /// <returns>
    /// A hash code based on the initialization state, success/failure state, and contained value or error.
    /// An uninitialized Result returns <c>0</c>.
    /// <para>
    /// 初期化状態、成功/失敗状態、および保持する値またはエラーに基づくハッシュコード。
    /// 未初期化の Result は <c>0</c> を返す。
    /// </para>
    /// </returns>
    public override int GetHashCode()
    {
        if (!IsInitialized)
            return 0;

        unchecked
        {
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

    /// <summary>
    /// Determines whether two Results are equal.
    /// <para>2つの Result が等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="left">
    /// The left Result.
    /// <para>左辺の Result。</para>
    /// </param>
    /// <param name="right">
    /// The right Result.
    /// <para>右辺の Result。</para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> when both Results are equal; otherwise <see langword="false"/>.
    /// <para>両方の Result が等しい場合は <see langword="true"/>、それ以外は <see langword="false"/>。</para>
    /// </returns>
    public static bool operator ==(Result<E, T> left, Result<E, T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two Results are not equal.
    /// <para>2つの Result が等しくないかどうかを判定する。</para>
    /// </summary>
    /// <param name="left">
    /// The left Result.
    /// <para>左辺の Result。</para>
    /// </param>
    /// <param name="right">
    /// The right Result.
    /// <para>右辺の Result。</para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> when both Results are not equal; otherwise <see langword="false"/>.
    /// <para>両方の Result が等しくない場合は <see langword="true"/>、それ以外は <see langword="false"/>。</para>
    /// </returns>
    public static bool operator !=(Result<E, T> left, Result<E, T> right)
    {
        return !(left == right);
    }

    #endregion
}
