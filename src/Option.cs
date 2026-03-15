namespace FunctionalCore
{
    /// <summary>
    /// Represents an optional value.
    /// 値が存在するかもしれないことを表す型。
    /// </summary>
    /// <typeparam name="T">Type of the value / 値の型</typeparam>
    public readonly struct Option<T> : IEquatable<Option<T>>
    {
        /// <summary>
        /// Indicates whether a value exists.
        /// 値が存在するかどうかを示す。
        /// </summary>
        public bool HasValue { get; }

        private readonly T _value;

        /// <summary>
        /// Gets the value if it exists. Throws if no value exists.
        /// 値が存在する場合に値を取得する。存在しない場合は例外を投げる。
        /// </summary>
        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException($"Option<{typeof(T).Name}> does not contain a value.");
                }
                return _value;
            }
        }

        /// <summary>
        /// Private constructor for Some(value).
        /// 値あり(Some)用のプライベートコンストラクタ。
        /// </summary>
        private Option(T value)
        {
            ArgumentNullException.ThrowIfNull(value);

            HasValue = true;
            _value = value;
        }

        /// <summary>
        /// Creates an Option with a value (Some).
        /// 値あり(Some)の Option を作成する。
        /// </summary>
        public static Option<T> Some(T value)
        {
            ArgumentNullException.ThrowIfNull(value);
            return new Option<T>(value);
        }

        /// <summary>
        /// Creates an Option without a value (None).
        /// 値なし(None)の Option を取得する。
        /// </summary>
        public static Option<T> None() => default;

        /// <summary>
        /// Applies a function to the value if it exists; otherwise returns None.
        /// 値が存在する場合に関数を適用し、存在しない場合は None を返す。
        /// </summary>
        public Option<U> Map<U>(Func<T, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (!HasValue)
            {
                return Option<U>.None();
            }

            var result = selector(_value);
            return result is null ? Option<U>.None() : Option<U>.Some(result);
        }

        /// <summary>
        /// Applies a function returning Option if a value exists; otherwise returns None.
        /// 値が存在する場合に Option を返す関数を適用し、存在しない場合は None を返す。
        /// </summary>
        public Option<U> Bind<U>(Func<T, Option<U>> binder)
        {
            ArgumentNullException.ThrowIfNull(binder);
            return HasValue ? binder(Value) : Option<U>.None();
        }

        /// <summary>
        /// Matches Some/None and returns a value.
        /// Some/None に応じて関数を適用し、値を返す。
        /// </summary>
        public U Match<U>(Func<T, U> funcOnSome, Func<U> funcOnNone)
        {
            ArgumentNullException.ThrowIfNull(funcOnSome);
            ArgumentNullException.ThrowIfNull(funcOnNone);

            return HasValue ? funcOnSome(Value) : funcOnNone();
        }

        #region For LINQ

        /// <summary>
        /// LINQ Select: maps the value when present.
        /// LINQ の Select として、値がある場合にマップする。
        /// </summary>
        public Option<U> Select<U>(Func<T, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);
            return Map(selector);
        }

        /// <summary>
        /// LINQ SelectMany: binds and then projects a new value.
        /// LINQ の SelectMany として、Bind と Map を組み合わせる。
        /// </summary>
        public Option<V> SelectMany<U, V>(
            Func<T, Option<U>> binder,
            Func<T, U, V> projector)
        {
            ArgumentNullException.ThrowIfNull(binder);
            ArgumentNullException.ThrowIfNull(projector);

            return Bind(x => binder(x).Map(y => projector(x, y)));
        }

        /// <summary>
        /// Filters the value with a predicate. Returns None if predicate fails.
        /// 値に対して条件を適用し、満たさない場合は None を返す。
        /// </summary>
        public Option<T> Where(Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);

            if (!HasValue)
            {
                return this;
            }

            return predicate(_value) ? this : Option<T>.None();
        }

        #endregion

        /// <summary>
        /// Performs an action if a value exists, then returns this Option.
        /// 値が存在する場合に副作用を実行し、その後同じ Option を返す。
        /// </summary>
        public Option<T> Tap(Action<T> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (HasValue)
            {
                action(Value);
            }
            return this;
        }

        /// <summary>
        /// Performs an action if no value exists, then returns this Option.
        /// 値が存在しない場合に副作用を実行し、その後同じ Option を返す。
        /// </summary>
        public Option<T> TapError(Action action)
        {
            ArgumentNullException.ThrowIfNull(action);

            if (!HasValue)
            {
                action();
            }
            return this;
        }

        /// <summary>
        /// Performs an action regardless of value existence, then returns this Option.
        /// 値の有無に関わらず副作用を実行し、その後同じ Option を返す。
        /// </summary>
        public Option<T> TapBoth(Action<Option<T>> action)
        {
            ArgumentNullException.ThrowIfNull(action);

            action(this);
            return this;
        }

        /// <summary>
        /// Determines whether this instance and another are equal.
        /// このインスタンスが別のインスタンスと等しいかどうかを判定する。
        /// </summary>
        public bool Equals(Option<T> other)
        {
            if (HasValue != other.HasValue) return false;
            if (!HasValue) return true;
            return EqualityComparer<T>.Default.Equals(_value, other._value);
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return obj is Option<T> other && Equals(other);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HasValue ? _value?.GetHashCode() ?? 0 : 0;
        }

        /// <summary>
        /// Returns a string representation of the option.
        /// Option の内容を表す文字列を返す。
        /// </summary>
        public override string ToString()
        {
            return HasValue ? $"Some({_value})" : "None";
        }

        /// <summary>
        /// Equality operator.
        /// 等値演算子。
        /// </summary>
        public static bool operator ==(Option<T> left, Option<T> right) => left.Equals(right);

        /// <summary>
        /// Inequality operator.
        /// 不等値演算子。
        /// </summary>
        public static bool operator !=(Option<T> left, Option<T> right) => !(left == right);
    }
}
