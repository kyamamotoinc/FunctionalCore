namespace FunctionalCore
{
    /// <summary>
    /// Extension methods for Option&lt;T&gt;.
    /// Option&lt;T&gt; の拡張メソッド。
    /// </summary>
    public static class OptionExtensions
    {
        // ------------------------------
        // Value Access / 値の取得
        // ------------------------------

        /// <summary>
        /// Returns the value if present; otherwise returns the provided default value.
        /// 値が存在する場合は値を返し、存在しない場合は指定したデフォルト値を返す。
        /// </summary>
        public static T ValueOrDefault<T>(this Option<T> option, T defaultValue)
        {
            return option.HasValue ? option.Value : defaultValue;
        }

        /// <summary>
        /// Returns the value if present; otherwise throws an exception created by the factory.
        /// 値が存在する場合は値を返し、存在しない場合は例外ファクトリで生成した例外を投げる。
        /// </summary>
        public static T ValueOrThrow<T>(this Option<T> option, Func<Exception> toException)
        {
            ArgumentNullException.ThrowIfNull(toException);
            return option.HasValue ? option.Value : throw toException();
        }

        // ------------------------------
        // Default Handling / デフォルト処理
        // ------------------------------

        /// <summary>
        /// Returns this Option if it has a value; otherwise returns the provided default Option.
        /// 値が存在する場合は自身を返し、存在しない場合は指定した Option を返す。
        /// </summary>
        public static Option<T> OrDefault<T>(this Option<T> option, Option<T> defaultOption)
        {
            return option.HasValue ? option : defaultOption;
        }

        /// <summary>
        /// Recovers from None using a fallback function.
        /// None の場合にフォールバック関数で値を生成して回復する。
        /// </summary>
        public static Option<T> Recover<T>(this Option<T> option, Func<T> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);

            if (option.HasValue)
                return option;

            var value = fallback();
            return value is null ? Option<T>.None() : Option<T>.Some(value);
        }

        /// <summary>
        /// Recovers from None using a fallback Option.
        /// None の場合にフォールバック Option を返す。
        /// </summary>
        public static Option<T> RecoverWith<T>(this Option<T> option, Func<Option<T>> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);
            return option.HasValue ? option : fallback();
        }

        // ------------------------------
        // Combine / 組み合わせ
        // ------------------------------

        /// <summary>
        /// Combines two Option values using a selector function.
        /// 2つの Option を関数で組み合わせる。
        /// </summary>
        public static Option<U> Combine<T, R, U>(
            this in Option<T> option,
            in Option<R> other,
            Func<T, R, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (!option.HasValue || !other.HasValue)
                return Option<U>.None();

            return Option<U>.Some(selector(option.Value, other.Value));
        }

        /// <summary>
        /// Combines three Option values using a selector function.
        /// 3つの Option を関数で組み合わせる。
        /// </summary>
        public static Option<U> Combine<T, R1, R2, U>(
            this in Option<T> option,
            in Option<R1> other1,
            in Option<R2> other2,
            Func<T, R1, R2, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (!option.HasValue || !other1.HasValue || !other2.HasValue)
                return Option<U>.None();

            return Option<U>.Some(selector(option.Value, other1.Value, other2.Value));
        }

        // ------------------------------
        // LINQ / LINQ互換
        // ------------------------------

        /// <summary>
        /// Filters the Option based on a predicate.
        /// 条件に合致する場合のみ Option を返す。
        /// </summary>
        public static Option<T> Where<T>(this Option<T> option, Func<T, bool> predicate)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            return option.HasValue && predicate(option.Value) ? option : Option<T>.None();
        }

        // ------------------------------
        // Collections / コレクション
        // ------------------------------

        /// <summary>
        /// Sequences a list of Option into a single Option of List.
        /// Option のリストを Option&lt;List&lt;T&gt;&gt; にまとめる。
        /// </summary>
        public static Option<List<T>> Sequence<T>(this IEnumerable<Option<T>> options)
        {
            ArgumentNullException.ThrowIfNull(options);

            var list = new List<T>();
            foreach (var opt in options)
            {
                if (!opt.HasValue)
                    return Option<List<T>>.None();

                list.Add(opt.Value);
            }
            return Option<List<T>>.Some(list);
        }

        /// <summary>
        /// Applies a function returning Option to each item and sequences the results.
        /// 各要素に Option を返す関数を適用し、結果を Option&lt;List&lt;U&gt;&gt; にまとめる。
        /// </summary>
        public static Option<List<U>> Traverse<T, U>(
            this IEnumerable<T> items,
            Func<T, Option<U>> selector)
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(selector);

            var list = new List<U>();
            foreach (var item in items)
            {
                var opt = selector(item);
                if (!opt.HasValue)
                    return Option<List<U>>.None();

                list.Add(opt.Value);
            }
            return Option<List<U>>.Some(list);
        }

        // ------------------------------
        // Conversions / 変換
        // ------------------------------

        /// <summary>
        /// Flattens nested Option values.
        /// ネストされた Option を平坦化する。
        /// </summary>
        public static Option<T> Flatten<T>(this Option<Option<T>> option)
        {
            return option.HasValue ? option.Value : Option<T>.None();
        }

        /// <summary>
        /// Converts Result to Option.
        /// Result を Option に変換する。
        /// </summary>
        public static Option<T> ToOptionFromResult<T, E>(this Result<E, T> result)
        {
            return result.IsSuccess ? Option<T>.Some(result.Value) : Option<T>.None();
        }

        /// <summary>
        /// Converts a value to Option; returns None if null.
        /// 値を Option に変換し、null の場合は None を返す。
        /// </summary>
        public static Option<T> ToOption<T>(this T value)
        {
            return value is null ? Option<T>.None() : Option<T>.Some(value);
        }
    }
}
