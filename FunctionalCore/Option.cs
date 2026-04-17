namespace FunctionalCore;

/// <summary>
/// Represents an optional value (Some or None).
/// 値の「存在 / 不在」を明示的に扱うための型。
///
/// This type eliminates null by making absence explicit.
/// null を排除し、「値が無い」という状態を明示的に表現する。
///
/// Design rules:
/// - null is not allowed
/// - absence must be represented as None
/// - operations must preserve these invariants
///
/// 設計ルール:
/// - nullは禁止
/// - 値が無い場合は必ず None
/// - すべての操作はこの不変条件を守る。
/// </summary>
public readonly struct Option<T> : IEquatable<Option<T>>
{
    private static readonly Option<T> _none = new Option<T>(false);

    /// <summary>
    /// Indicates whether a value exists.
    /// 値が存在するかどうか
    /// </summary>
    public bool HasValue { get; }

    private readonly T _value;

    /// <summary>
    /// Gets the value if present.
    /// 値を取得する（Someの場合のみ）
    ///
    /// Throws if no value exists.
    /// None の場合は例外を投げる
    /// </summary>
    public T Value
    {
        get
        {
            if (!HasValue)
                throw new InvalidOperationException($"Option<{typeof(T).Name}> has no value.");

            return _value;
        }
    }

    /// <summary>
    /// Creates a Some(value).
    /// 値を持つ Option を生成する。
    ///
    /// null is not allowed and will throw.
    /// nullは禁止（例外を投げる）
    /// </summary>
    private Option(T value)
    {
        ArgumentNullException.ThrowIfNull(value);

        HasValue = true;
        _value = value;
    }

    /// Creates a None.
    /// 値なし(None)の Option を生成する。
    /// </summary>
    private Option(bool hasValue)
    {
        HasValue = hasValue;
        _value = default!;
    }

    /// <summary>
    /// <summary>
    /// Creates a Some(value).
    /// 値あり(Some)の Option を作成する。
    ///
    /// null is not allowed.
    /// nullは禁止
    /// </summary>
    public static Option<T> Some(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Option<T>(value);
    }

    /// <summary>
    /// Represents absence of value.
    /// 値が存在しない状態（None）
    ///
    /// This is the only way to represent "no value".
    /// 「値がない」は必ずこれで表現する
    /// </summary>
    public static Option<T> None => _none;

    /// <summary>
    /// Transforms the contained value if present.
    /// 値が存在する場合のみ変換を行う。
    ///
    /// If selector returns null, it is converted to None.
    /// selector が null を返した場合は None に変換される。
    ///
    /// This makes Map null-safe.
    /// Mapはnullを安全に扱う（null → None）
    /// </summary>
    public Option<U> Map<U>(Func<T, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (!HasValue)
            return Option<U>.None;

        var value = selector(_value);
        return value is null ? Option<U>.None : Option<U>.Some(value);
    }

    /// <summary>
    /// Applies a function returning Option and flattens the result.
    /// Optionを返す関数を適用し、ネストを解消する。
    ///
    /// Unlike Map, this does not convert null automatically.
    /// Mapとは異なり、nullの扱いは呼び出し側の責務
    /// </summary>
    public Option<U> Bind<U>(Func<T, Option<U>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);

        return HasValue ? binder(_value) : Option<U>.None;
    }

    #region LINQ

    /// <summary>
    /// Pattern matches Option into a value.
    /// Optionを分岐処理し、値を生成する。
    ///
    /// Forces handling of both Some and None.
    /// Some / None の両方を処理させる。
    /// </summary>
    public U Match<U>(Func<T, U> onSome, Func<U> onNone)
    {
        ArgumentNullException.ThrowIfNull(onSome);
        ArgumentNullException.ThrowIfNull(onNone);

        var value = HasValue ? onSome(_value) : onNone();

        if (value is null)
            throw new InvalidOperationException("Match must not return null.");

        return value;
    }

    /// <summary>
    /// Pattern matches Option into a value.
    /// Optionを分岐処理し、値を生成する。
    ///
    /// Forces handling of both Some and None.
    /// Some / None の両方を処理させる。
    /// </summary>
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
    /// Filters the value using a predicate.
    /// 条件を満たさない場合は None に変換する。
    ///
    /// Equivalent to validation.
    /// バリデーションとして使用する
    /// </summary>
    public Option<T> Ensure(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        if (!HasValue)
            return this;

        return predicate(_value) ? this : Option<T>.None;
    }

    #endregion

    /// <summary>
    /// Executes side-effect if value exists.
    /// 値が存在する場合のみ副作用を実行する。
    ///
    /// Does not change the Option.
    /// 状態は変更しない。
    /// </summary>
    public Option<T> Tap(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (HasValue)
            action(_value);

        return this;
    }

    /// <summary>
    /// Executes side-effect if value does not exist.
    /// 値が存在しない場合のみ副作用を実行する。
    ///
    /// Does not change the Option.
    /// 状態は変更しない
    /// </summary>
    public Option<T> TapNone(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (!HasValue)
            action();

        return this;
    }

    /// <summary>
    /// Executes side-effect regardless of state.
    /// 状態に関係なく副作用を実行する
    ///
    /// Does not change the Option.
    /// 状態は変更しない
    /// </summary>
    public Option<T> TapBoth(Action<Option<T>> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        action(this);
        return this;
    }

    /// <summary>
    /// Filters the value using a predicate.
    /// 条件を満たさない場合は None に変換する。
    ///
    /// Equivalent to validation.
    /// バリデーションとして使用する
    /// </summary>
    public Option<T> Ensure(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        if (!HasValue)
            return this;

        return predicate(_value) ? this : Option<T>.None;
    }

    /// <summary>
    /// Pattern matches Option into a value.
    /// Optionを分岐処理し値を生成する。
    ///
    /// Forces handling of both Some and None.
    /// Some / None の両方を処理させる
    /// </summary>
    public U Match<U>(Func<T, U> onSome, Func<U> onNone)
    {
        ArgumentNullException.ThrowIfNull(onSome);
        ArgumentNullException.ThrowIfNull(onNone);

        var value = HasValue ? onSome(_value) : onNone();

        if (value is null)
            throw new InvalidOperationException("Match must not return null.");

        return value;
    }

    /// <summary>
    /// Returns value if present, otherwise default(T).
    /// 値が存在すればそれを返し、無ければ既定値
    /// </summary>
    public T GetValueOrDefault()
    {
        return HasValue ? _value : default!;
    }

    /// <summary>
    /// Returns value if present, otherwise fallback.
    /// 値が存在すればそれを返し、無ければ代替値
    /// </summary>
    public T GetValueOr(T defaultValue)
    {
        return HasValue ? _value : defaultValue;
    }

    /// <summary>
    /// Returns this if Some, otherwise other.
    /// Someなら自身、Noneなら代替
    /// </summary>
    public Option<T> Or(Option<T> other)
    {
        return HasValue ? this : other;
    }

    /// <summary>
    /// Returns this if Some, otherwise factory result.
    /// Someなら自身、Noneなら生成結果
    /// </summary>
    public Option<T> Or(Func<Option<T>> otherFactory)
    {
        ArgumentNullException.ThrowIfNull(otherFactory);

        return HasValue ? this : otherFactory();
    }

    public override string ToString()
    {
        return HasValue ? $"Some({_value})" : "None";
    }

    #region Equality
    public bool Equals(Option<T> other)
    {
        if (HasValue != other.HasValue) return false;
        if (!HasValue) return true;

        return EqualityComparer<T>.Default.Equals(_value, other._value);
    }

    public override bool Equals(object? obj)
    {
        return obj is Option<T> other && Equals(other);
    }

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

    public static bool operator ==(Option<T> left, Option<T> right)
    {
        return left.Equals(right);
    }


    public static bool operator !=(Option<T> left, Option<T> right)
    {
        return !(left == right);
    }

    #endregion   
}