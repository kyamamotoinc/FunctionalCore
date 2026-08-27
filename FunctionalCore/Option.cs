namespace FunctionalCore;

/// <summary>
/// Represents an optional value as either Some or None.
/// <para>値の存在を Some または None として明示的に表現する型。</para>
///
/// Absence is represented explicitly as None rather than null.
/// <para>値が存在しない状態を null ではなく None として明示的に表現する。</para>
///
/// <para>
/// Design rules:
/// <para>- Some never contains null.</para>
/// <para>- Absence is represented as None.</para>
/// <para>- Operations preserve these invariants.</para>
/// </para>
///
/// <para>
/// 設計ルール:
/// <para>- Some は null を保持しない。</para>
/// <para>- 値が存在しない場合は None で表現する。</para>
/// <para>- すべての操作はこれらの不変条件を維持する。</para>
/// </para>
/// </summary>
/// <typeparam name="T">
/// The type of the optional value.
/// <para>保持する値の型。</para>
/// </typeparam>
public readonly struct Option<T> : IEquatable<Option<T>>
{
    private static readonly Option<T> _none = new Option<T>(false);

    /// <summary>
    /// Indicates whether this Option contains a value.
    /// <para>この Option が値を保持しているかどうかを示す。</para>
    /// </summary>
    public bool HasValue { get; }

    private readonly T _value;

    /// <summary>
    /// Gets the contained value.
    /// <para>保持している値を取得する。</para>
    /// </summary>
    /// <value>
    /// The value contained in a Some.
    /// <para>Some が保持している値。</para>
    /// </value>
    /// <exception cref="InvalidOperationException">
    /// Thrown when this Option is None.
    /// <para>この Option が None の場合にスローされる。</para>
    /// </exception>
    public T Value
    {
        get
        {
            if (!HasValue)
                throw new InvalidOperationException($"Option<{typeof(T).Name}> has no value.");

            return _value;
        }
    }

    private Option(T value)
    {
        HasValue = true;
        _value = value;
    }

    private Option(bool hasValue)
    {
        HasValue = hasValue;
        _value = default!;
    }

    /// <summary>
    /// Creates a Some containing the specified value.
    /// <para>指定された値を保持する Some を生成する。</para>
    /// </summary>
    /// <param name="value">
    /// The value to contain. Must not be null.
    /// <para>保持する値。null は許可されない。</para>
    /// </param>
    /// <returns>
    /// A Some containing <paramref name="value"/>.
    /// <para><paramref name="value"/> を保持する Some。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is null.
    /// <para><paramref name="value"/> が null の場合にスローされる。</para>
    /// </exception>
    public static Option<T> Some(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return new Option<T>(value);
    }

    /// <summary>
    /// Gets an Option representing the absence of a value.
    /// <para>値が存在しないことを表す Option を取得する。</para>
    /// </summary>
    /// <value>
    /// The None value.
    /// <para>None を表す値。</para>
    /// </value>
    public static Option<T> None => _none;

    /// <summary>
    /// Applies one of two functions according to whether this Option is Some or None and returns the produced value.
    /// <para>この Option が Some または None のどちらであるかに応じた関数を適用し、その結果を返す。</para>
    /// </summary>
    /// <typeparam name="U">
    /// The returned value type.
    /// <para>戻り値の型。</para>
    /// </typeparam>
    /// <param name="onSome">
    /// The function to execute when this Option contains a value.
    /// <para>値が存在する場合に実行する関数。</para>
    /// </param>
    /// <param name="onNone">
    /// The function to execute when this Option is None.
    /// <para>値が存在しない場合に実行する関数。</para>
    /// </param>
    /// <returns>
    /// The value returned by the function corresponding to the state of this Option.
    /// <para>この Option の状態に対応する関数が返した値。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="onSome"/> or <paramref name="onNone"/> is null.
    /// <para><paramref name="onSome"/> または <paramref name="onNone"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the selected function returns null.
    /// <para>選択された関数が null を返した場合にスローされる。</para>
    /// </exception>
    public U Match<U>(Func<T, U> onSome, Func<U> onNone)
    {
        ArgumentNullException.ThrowIfNull(onSome);
        ArgumentNullException.ThrowIfNull(onNone);

        var value = HasValue ? onSome(_value) : onNone();

        if (value is null)
            throw new InvalidOperationException("Match function must not return null.");

        return value;
    }

    /// <summary>
    /// Executes one of two actions according to whether this Option is Some or None.
    /// <para>この Option が Some または None のどちらであるかに応じたアクションを実行する。</para>
    /// </summary>
    /// <param name="onSome">
    /// The action to execute when this Option contains a value.
    /// <para>値が存在する場合に実行するアクション。</para>
    /// </param>
    /// <param name="onNone">
    /// The action to execute when this Option is None.
    /// <para>値が存在しない場合に実行するアクション。</para>
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="onSome"/> or <paramref name="onNone"/> is null.
    /// <para><paramref name="onSome"/> または <paramref name="onNone"/> が null の場合にスローされる。</para>
    /// </exception>
    public void Match(Action<T> onSome, Action onNone)
    {
        ArgumentNullException.ThrowIfNull(onSome);
        ArgumentNullException.ThrowIfNull(onNone);

        if (HasValue)
            onSome(_value);
        else
            onNone();
    }

    /// <summary>
    /// Connects a Some value to another Option-producing operation.
    /// <para>Some の値を、次の Option を返す処理へ接続する。</para>
    /// </summary>
    /// <typeparam name="U">
    /// The value type of the next Option.
    /// <para>次の Option の値の型。</para>
    /// </typeparam>
    /// <param name="binder">
    /// A function that receives the contained value and produces the next Option.
    /// <para>保持している値を受け取り、次の Option を生成する関数。</para>
    /// </param>
    /// <returns>
    /// The Option returned by <paramref name="binder"/> when this Option is Some;
    /// otherwise None.
    /// <para>
    /// Some の場合は <paramref name="binder"/> が返した Option。
    /// None の場合は None。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="binder"/> is null.
    /// <para><paramref name="binder"/> が null の場合にスローされる。</para>
    /// </exception>
    public Option<U> Bind<U>(Func<T, Option<U>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);

        return HasValue ? binder(_value) : Option<U>.None;
    }

    /// <summary>
    /// Transforms the contained value when this Option is Some.
    /// <para>この Option が Some の場合に、保持している値を変換する。</para>
    /// </summary>
    /// <typeparam name="U">
    /// The transformed value type.
    /// <para>変換後の値の型。</para>
    /// </typeparam>
    /// <param name="selector">
    /// A function that transforms the contained value.
    /// If it returns null, the result is converted to None.
    /// <para>
    /// 保持している値を変換する関数。
    /// null を返した場合は None に変換される。
    /// </para>
    /// </param>
    /// <returns>
    /// A Some containing the transformed value when the selector returns a non-null value;
    /// otherwise None.
    /// <para>
    /// selector が null ではない値を返した場合は、その値を保持する Some。
    /// それ以外は None。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> is null.
    /// <para><paramref name="selector"/> が null の場合にスローされる。</para>
    /// </exception>
    public Option<U> Map<U>(Func<T, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (!HasValue)
            return Option<U>.None;

        var value = selector(_value);

        return value is null ? Option<U>.None : Option<U>.Some(value);
    }

    /// <summary>
    /// Validates the contained value and converts this Option to None when the predicate is not satisfied.
    /// <para>保持している値を検証し、条件を満たさない場合は None に変換する。</para>
    /// </summary>
    /// <param name="predicate">
    /// A function that determines whether the contained value satisfies the required condition.
    /// <para>保持している値が条件を満たすかどうかを判定する関数。</para>
    /// </param>
    /// <returns>
    /// This Option when it is None or when the predicate succeeds;
    /// otherwise None.
    /// <para>
    /// この Option が None、または条件を満たす場合は元の Option。
    /// 条件を満たさない場合は None。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="predicate"/> is null.
    /// <para><paramref name="predicate"/> が null の場合にスローされる。</para>
    /// </exception>
    public Option<T> Ensure(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        if (!HasValue)
            return this;

        return predicate(_value) ? this : Option<T>.None;
    }

    /// <summary>
    /// Executes a side effect when this Option is Some without changing the Option.
    /// <para>この Option が Some の場合に副作用を実行し、Option 自体は変更しない。</para>
    /// </summary>
    /// <param name="action">
    /// The action to execute for the contained value.
    /// <para>保持している値に対して実行するアクション。</para>
    /// </param>
    /// <returns>
    /// The original Option unchanged.
    /// <para>変更されていない元の Option。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="action"/> is null.
    /// <para><paramref name="action"/> が null の場合にスローされる。</para>
    /// </exception>
    public Option<T> Tap(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (HasValue)
            action(_value);

        return this;
    }

    /// <summary>
    /// Executes a side effect when this Option is None without changing the Option.
    /// <para>この Option が None の場合に副作用を実行し、Option 自体は変更しない。</para>
    /// </summary>
    /// <param name="action">
    /// The action to execute when no value exists.
    /// <para>値が存在しない場合に実行するアクション。</para>
    /// </param>
    /// <returns>
    /// The original Option unchanged.
    /// <para>変更されていない元の Option。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="action"/> is null.
    /// <para><paramref name="action"/> が null の場合にスローされる。</para>
    /// </exception>
    public Option<T> TapNone(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (!HasValue)
            action();

        return this;
    }

    /// <summary>
    /// Returns the string representation of this Option.
    /// <para>この Option の文字列表現を返す。</para>
    /// </summary>
    /// <returns>
    /// <c>Some(value)</c> when this Option contains a value; otherwise <c>None</c>.
    /// <para>値が存在する場合は <c>Some(value)</c>、存在しない場合は <c>None</c>。</para>
    /// </returns>
    public override string ToString()
    {
        return HasValue ? $"Some({_value})" : "None";
    }

    #region Equality

    /// <summary>
    /// Determines whether this Option is equal to another Option.
    /// <para>この Option と指定された Option が等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="other">
    /// The Option to compare with this instance.
    /// <para>このインスタンスと比較する Option。</para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> when both Options are None,
    /// or when both are Some and contain equal values;
    /// otherwise <see langword="false"/>.
    /// <para>
    /// 両方が None、または両方が Some で保持する値が等しい場合は <see langword="true"/>。
    /// それ以外は <see langword="false"/>。
    /// </para>
    /// </returns>
    public bool Equals(Option<T> other)
    {
        if (HasValue != other.HasValue) return false;
        if (!HasValue) return true;

        return EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    /// <summary>
    /// Determines whether this Option is equal to the specified object.
    /// <para>この Option と指定されたオブジェクトが等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="obj">
    /// The object to compare with this instance.
    /// <para>このインスタンスと比較するオブジェクト。</para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> when <paramref name="obj"/> is an equal Option;
    /// otherwise <see langword="false"/>.
    /// <para><paramref name="obj"/> が等しい Option の場合は <see langword="true"/>、それ以外は <see langword="false"/>。</para>
    /// </returns>
    public override bool Equals(object? obj)
    {
        return obj is Option<T> other && Equals(other);
    }

    /// <summary>
    /// Returns the hash code for this Option.
    /// <para>この Option のハッシュコードを返す。</para>
    /// </summary>
    /// <returns>
    /// A hash code based on whether a value exists and, when present, the contained value.
    /// <para>値の存在状態と、値が存在する場合はその値に基づくハッシュコード。</para>
    /// </returns>
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + HasValue.GetHashCode();

            if (HasValue)
                hash = hash * 23 + EqualityComparer<T>.Default.GetHashCode(_value!);

            return hash;
        }
    }

    /// <summary>
    /// Determines whether two Options are equal.
    /// <para>2つの Option が等しいかどうかを判定する。</para>
    /// </summary>
    /// <param name="left">
    /// The left Option.
    /// <para>左辺の Option。</para>
    /// </param>
    /// <param name="right">
    /// The right Option.
    /// <para>右辺の Option。</para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> when both Options are equal; otherwise <see langword="false"/>.
    /// <para>両方の Option が等しい場合は <see langword="true"/>、それ以外は <see langword="false"/>。</para>
    /// </returns>
    public static bool operator ==(Option<T> left, Option<T> right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Determines whether two Options are not equal.
    /// <para>2つの Option が等しくないかどうかを判定する。</para>
    /// </summary>
    /// <param name="left">
    /// The left Option.
    /// <para>左辺の Option。</para>
    /// </param>
    /// <param name="right">
    /// The right Option.
    /// <para>右辺の Option。</para>
    /// </param>
    /// <returns>
    /// <see langword="true"/> when both Options are not equal; otherwise <see langword="false"/>.
    /// <para>両方の Option が等しくない場合は <see langword="true"/>、それ以外は <see langword="false"/>。</para>
    /// </returns>
    public static bool operator !=(Option<T> left, Option<T> right)
    {
        return !(left == right);
    }

    #endregion
}
