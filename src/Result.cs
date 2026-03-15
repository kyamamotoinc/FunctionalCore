namespace FunctionalCore
{
    /// <summary>
    /// Represents a result that can be either a success with a value or a failure with an error.
    /// 成功時の値、または失敗時のエラーを保持する結果型。
    /// </summary>
    /// <typeparam name="E">Type of error / エラーの型</typeparam>
    /// <typeparam name="T">Type of value / 値の型</typeparam>
    public readonly struct Result<E, T> : IEquatable<Result<E, T>>
    {
        /// <summary>
        /// Indicates whether the result is successful.
        /// 結果が成功かどうかを示す。
        /// </summary>
        public bool IsSuccess { get; }

        private readonly T _value;

        /// <summary>
        /// Gets the value if successful. Throws if the result is a failure.
        /// 成功時の値を取得する。失敗の場合は例外を投げる。
        /// </summary>
        public T Value
        {
            get
            {
                if (!IsSuccess)
                {
                    throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> does not contain a value.");
                }
                return _value;
            }
        }

        private readonly E _error;

        /// <summary>
        /// Gets the error if failed. Throws if the result is successful.
        /// 失敗時のエラーを取得する。成功の場合は例外を投げる。
        /// </summary>
        public E Error
        {
            get
            {
                if (IsSuccess)
                {
                    throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> does not contain an error.");
                }
                return _error;
            }
        }

        /// <summary>
        /// Private constructor for success.
        /// 成功用のプライベートコンストラクタ。
        /// </summary>
        private Result(T value)
        {
            ArgumentNullException.ThrowIfNull(value);

            IsSuccess = true;
            _error = default!;
            _value = value;
        }

        /// <summary>
        /// Private constructor for failure.
        /// 失敗用のプライベートコンストラクタ。
        /// </summary>
        private Result(E error)
        {
            ArgumentNullException.ThrowIfNull(error);

            IsSuccess = false;
            _error = error;
            _value = default!;
        }

        /// <summary>
        /// Creates a successful result.
        /// 成功結果を作成する。
        /// </summary>
        public static Result<E, T> Ok(T value) => new(value);

        /// <summary>
        /// Creates a failed result.
        /// 失敗結果を作成する。
        /// </summary>
        public static Result<E, T> Fail(E error) => new(error);

        /// <summary>
        /// Applies a function to the value if successful; otherwise returns the original error.
        /// 成功時に値へ関数を適用し、失敗時は元のエラーを保持したまま返す。
        /// </summary>
        public Result<E, U> Map<U>(Func<T, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);
            return IsSuccess
                ? Result<E, U>.Ok(selector(Value))
                : Result<E, U>.Fail(Error);
        }

        /// <summary>
        /// Applies a function that returns a Result if successful; otherwise returns the original error.
        /// 成功時に Result を返す関数を適用し、失敗時は元のエラーを保持したまま返す。
        /// </summary>
        public Result<E, U> Bind<U>(Func<T, Result<E, U>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            return IsSuccess ? binder(Value) : Result<E, U>.Fail(Error);
        }

        /// <summary>
        /// Matches success or failure and returns a value.
        /// 成功・失敗に応じて関数を適用し、値を返す。
        /// </summary>
        public U Match<U>(Func<T, U> funcOnSuccess, Func<E, U> funcOnFailure)
        {
            ArgumentNullException.ThrowIfNull(funcOnSuccess);
            ArgumentNullException.ThrowIfNull(funcOnFailure);

            return IsSuccess ? funcOnSuccess(Value) : funcOnFailure(Error);
        }

        #region For LINQ

        /// <summary>
        /// LINQ Select: maps the value when successful.
        /// LINQ の Select として、成功時の値をマップする。
        /// </summary>
        public Result<E, U> Select<U>(Func<T, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            return Map(selector);
        }

        /// <summary>
        /// LINQ SelectMany: binds and then projects a new value.
        /// LINQ の SelectMany として、Bind と Map を組み合わせる。
        /// </summary>
        public Result<E, V> SelectMany<U, V>(
            Func<T, Result<E, U>> binder,
            Func<T, U, V> projector)
        {
            ArgumentNullException.ThrowIfNull(binder);
            ArgumentNullException.ThrowIfNull(projector);

            return Bind(x => binder(x).Map(y => projector(x, y)));
        }

        #endregion

        /// <summary>
        /// Performs an action if successful, then returns this result.
        /// 成功時に副作用を実行し、その後同じ Result を返す。
        /// </summary>
        public Result<E, T> Tap(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (IsSuccess)
            {
                action(Value);
            }
            return this;
        }

        /// <summary>
        /// Performs an action if failed, then returns this result.
        /// 失敗時に副作用を実行し、その後同じ Result を返す。
        /// </summary>
        public Result<E, T> TapError(Action<E> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (!IsSuccess)
            {
                action(Error);
            }
            return this;
        }

        /// <summary>
        /// Performs an action regardless of success or failure, then returns this result.
        /// 成功・失敗に関わらず副作用を実行し、その後同じ Result を返す。
        /// </summary>
        public Result<E, T> TapBoth(Action<Result<E, T>> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            action(this);
            return this;
        }

        /// <summary>
        /// Ensures the value satisfies a predicate; otherwise converts to a failure.
        /// 値が条件を満たすか検証し、満たさない場合は失敗に変換する。
        /// </summary>
        public Result<E, T> Ensure(Func<T, bool> predicate, Func<T, E> errorFactory)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            ArgumentNullException.ThrowIfNull(errorFactory);

            if (!IsSuccess)
            {
                return this;
            }

            return predicate(Value)
                ? this
                : Fail(errorFactory(Value));
        }

        /// <summary>
        /// Returns a string representation of the result.
        /// 結果の内容を表す文字列を返す。
        /// </summary>
        public override string ToString()
        {
            return IsSuccess ? $"Success({Value})" : $"Failure({Error})";
        }

        /// <summary>
        /// Determines whether this instance and another are equal.
        /// このインスタンスが別のインスタンスと等しいかどうかを判定する。
        /// </summary>
        public bool Equals(Result<E, T> other)
        {
            if (IsSuccess != other.IsSuccess) return false;
            if (IsSuccess)
                return EqualityComparer<T>.Default.Equals(_value, other._value);
            return EqualityComparer<E>.Default.Equals(_error, other._error);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Result<E, T> other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return IsSuccess
                ? HashCode.Combine(true, _value)
                : HashCode.Combine(false, _error);
        }

        /// <summary>
        /// Equality operator.
        /// 等値演算子。
        /// </summary>
        public static bool operator ==(Result<E, T> left, Result<E, T> right) => left.Equals(right);

        /// <summary>
        /// Inequality operator.
        /// 不等値演算子。
        /// </summary>
        public static bool operator !=(Result<E, T> left, Result<E, T> right) => !(left == right);
    }
}
