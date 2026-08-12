using System.Reflection;

namespace FunctionalCore;

/// <summary>
/// Represents an optional value (Some or None).
/// <para>値の「存在 / 不在」を明示的に扱うための型。</para>
///
/// This type eliminates null by making absence explicit.
/// <para>null を排除し、「値が無い」という状態を明示的に表現する。</para>
///
/// <para>
/// Design rules:
/// <para>- null is not allowed</para>
/// <para>- absence must be represented as None</para>
/// <para>- operations must preserve these invariants</para>
///</para>
///
/// <para>
/// 設計ルール:
/// <para>- nullは禁止</para>
/// <para>- 値が無い場合は必ず None</para>
/// <para>- すべての操作はこの不変条件を守る。</para>
/// </para>
/// </summary>
/// <typeparam name="T">The type of the value. / 値の型。</typeparam>
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
    /// <para>値を取得する（Someの場合のみ）。</para>
    ///
    /// Throws if no value exists.
    /// <para>None の場合は例外を投げる。</para>
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if no value exists (None).
    /// <para>値が存在しない場合（None）に投げられる。</para>
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

    /// <summary>
    /// Creates a Some(value).
    /// <para>値を持つ Option を生成する。</para>
    ///
    /// null is not allowed and will throw.
    /// <para>nullは禁止（例外を投げる）。</para>
    /// </summary>
    /// <param name="value">The value to wrap. / ラップする値。</param>
    private Option(T value)
    {
        HasValue = true;
        _value = value;
    }

    /// <summary>
    /// Creates a None.
    /// <para>値なし(None)の Option を生成する。</para>
    /// </summary>
    /// <param name="hasValue">Always false for None. / Noneの場合は常にfalse。</param>
    private Option(bool hasValue)
    {
        HasValue = hasValue;
        _value = default!;
    }

    /// <summary>
    /// Creates a Some(value).
    /// <para>値あり(Some)の Option を作成する。</para>
    ///
    /// null is not allowed.
    /// <para>nullは禁止。</para>
    /// </summary>
    /// <param name="value">The value to wrap. Must not be null. / ラップする値。nullは禁止。</param>
    /// <returns>
    /// An Option containing the value.
    /// <para>値を持つOption。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if value is null. / valueがnullの場合に投げられる。</exception>
    public static Option<T> Some(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return new Option<T>(value);
    }

    /// <summary>
    /// Represents absence of value.
    /// <para>値が存在しない状態（None）</para>
    ///
    /// This is the only way to represent "no value".
    /// <para>「値がない」は必ずこれで表現する。</para>
    /// </summary>
    public static Option<T> None => _none;

    /// <summary>
    /// Pattern matches Option into a value.
    /// <para>Optionを分岐処理し、値を生成する。</para>
    ///
    /// Forces handling of both Some and None.
    /// <para>Some / None の両方を処理させる。</para>
    /// </summary>
    /// <typeparam name="U">The return type. / 戻り値の型。</typeparam>
    /// <param name="onSome">A function to apply if a value exists. Must not return null. / 値が存在する場合に適用する関数。nullを返してはならない。</param>
    /// <param name="onNone">A function to apply if no value exists. Must not return null. / 値が存在しない場合に適用する関数。nullを返してはならない。</param>
    /// <returns>
    /// The result of the applied function.
    /// <para>適用した関数の戻り値。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if onSome or onNone is null. / onSomeまたはonNoneがnullの場合に投げられる。</exception>
    /// <exception cref="InvalidOperationException">Thrown if the applied function returns null. / 適用した関数がnullを返した場合に投げられる。</exception>
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
    /// Pattern matches Option and executes an action.
    /// <para>Optionを分岐処理し、アクションを実行する。</para>
    ///
    /// Forces handling of both Some and None.
    /// <para>Some / None の両方を処理させる。</para>
    /// </summary>
    /// <param name="onSome">An action to execute if a value exists. / 値が存在する場合に実行するアクション。</param>
    /// <param name="onNone">An action to execute if no value exists. / 値が存在しない場合に実行するアクション。</param>
    /// <exception cref="ArgumentNullException">Thrown if onSome or onNone is null. / onSomeまたはonNoneがnullの場合に投げられる。</exception>
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
    /// Applies a function returning Option and flattens the result.
    /// <para>Optionを返す関数を適用し、ネストを解消する。</para>
    ///
    /// The binder is responsible for representing absence as None.
    /// <para>binder は値が存在しない場合を None として表現する。</para>
    /// </summary>
    /// <typeparam name="U">The type of the value in the returned Option. / 返されるOptionの値の型。</typeparam>
    /// <param name="binder">A function that takes the value and returns a new Option. / 値を受け取り新しいOptionを返す関数。</param>
    /// <returns>
    /// The Option returned by binder, or None.
    /// <para>binderが返すOption、またはNone。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if binder is null. / binderがnullの場合に投げられる。</exception>
    public Option<U> Bind<U>(Func<T, Option<U>> binder)
    {
        ArgumentNullException.ThrowIfNull(binder);

        return HasValue ? binder(_value) : Option<U>.None;
    }

    /// <summary>
    /// Transforms the contained value if present.
    /// <para>値が存在する場合のみ変換を行う。</para>
    ///
    /// If selector returns null, it is converted to None.
    /// <para>selector が null を返した場合は None に変換される。</para>
    ///
    /// This makes Map null-safe.
    /// <para>Mapはnullを安全に扱う（null → None）。</para>
    /// </summary>
    /// <typeparam name="U">The type of the transformed value. / 変換後の値の型。</typeparam>
    /// <param name="selector">A function to transform the value. Returning null is converted to None. / 値を変換する関数。nullを返した場合はNoneに変換される。</param>
    /// <returns>
    /// An Option with the transformed value, or None.
    /// <para>変換後の値を持つOption、またはNone。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if selector is null. / selectorがnullの場合に投げられる。</exception>
    public Option<U> Map<U>(Func<T, U> selector)
    {
        ArgumentNullException.ThrowIfNull(selector);

        if (!HasValue)
            return Option<U>.None;

        var value = selector(_value);

        return value is null ? Option<U>.None : Option<U>.Some(value);
    }

    /// <summary>
    /// Filters the value using a predicate.
    /// <para>条件を満たさない場合は None に変換する。</para>
    ///
    /// Equivalent to validation.
    /// <para>バリデーションとして使用する。</para>
    /// </summary>
    /// <param name="predicate">A function to test the value. / 値を検証する関数。</param>
    /// <returns>
    /// The original Option if the predicate passes; otherwise None.
    /// <para>条件を満たす場合は元のOption、それ以外はNone。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if predicate is null. / predicateがnullの場合に投げられる。</exception>
    public Option<T> Ensure(Func<T, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        if (!HasValue)
            return this;

        return predicate(_value) ? this : Option<T>.None;
    }

    /// <summary>
    /// Executes side-effect if value exists.
    /// <para>値が存在する場合のみ副作用を実行する。</para>
    ///
    /// Does not change the Option.
    /// <para>状態は変更しない。</para>
    /// </summary>
    /// <param name="action">An action to execute on the value if present. / 値が存在する場合に実行するアクション。</param>
    /// <returns>
    /// The original Option unchanged.
    /// <para>変更されていない元のOption。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if action is null. / actionがnullの場合に投げられる。</exception>
    public Option<T> Tap(Action<T> action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (HasValue)
            action(_value);

        return this;
    }

    /// <summary>
    /// Executes side-effect if value does not exist.
    /// <para>値が存在しない場合のみ副作用を実行する。</para>
    ///
    /// Does not change the Option.
    /// <para>状態は変更しない。</para>
    /// </summary>
    /// <param name="action">An action to execute if no value exists. / 値が存在しない場合に実行するアクション。</param>
    /// <returns>
    /// The original Option unchanged.
    /// <para>変更されていない元のOption。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown if action is null. / actionがnullの場合に投げられる。</exception>
    public Option<T> TapNone(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (!HasValue)
            action();

        return this;
    }

    /// <summary>
    /// Returns the string representation of Option.
    /// <para>Option の文字列表現を返す。</para>
    /// </summary>
    /// <returns>
    /// "Some(value)" if a value exists; otherwise "None".
    /// <para>値が存在する場合は "Some(value)"、存在しない場合は "None"。</para>
    /// </returns>
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
