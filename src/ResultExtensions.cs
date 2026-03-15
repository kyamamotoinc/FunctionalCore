namespace FunctionalCore
{
    /// <summary>
    /// Extension methods for Result&lt;E, T&gt;.
    /// Result&lt;E, T&gt; の拡張メソッド。
    /// </summary>
    public static class ResultExtensions
    {
        // ------------------------------
        // Value Access / 値の取得
        // ------------------------------

        /// <summary>
        /// Returns the value if successful; otherwise returns the provided default value.
        /// 成功時は値を返し、失敗時は指定したデフォルト値を返す。
        /// </summary>
        public static T ValueOrDefault<E, T>(this Result<E, T> result, T defaultValue)
        {
            return result.IsSuccess ? result.Value : defaultValue;
        }

        /// <summary>
        /// Returns the value if successful; otherwise throws an exception created by the factory.
        /// 成功時は値を返し、失敗時は例外ファクトリで生成した例外を投げる。
        /// </summary>
        public static T ValueOrThrow<E, T>(this Result<E, T> result, Func<E, Exception> toException)
        {
            ArgumentNullException.ThrowIfNull(toException);

            if (result.IsSuccess)
                return result.Value;

            var ex = toException(result.Error);
            if (ex is null)
                throw new InvalidOperationException("Exception factory returned null.");

            throw ex;
        }

        // ------------------------------
        // Default Handling / デフォルト処理
        // ------------------------------

        /// <summary>
        /// Returns this result if successful; otherwise returns the provided default result.
        /// 成功時は自身を返し、失敗時は指定した Result を返す。
        /// </summary>
        public static Result<E, T> OrDefault<E, T>(this Result<E, T> result, Result<E, T> defaultResult)
        {
            return result.IsSuccess ? result : defaultResult;
        }

        /// <summary>
        /// Converts failure into success using a fallback function.
        /// 失敗時にフォールバック関数で成功値へ変換する。
        /// </summary>
        public static Result<E, T> Recover<E, T>(this Result<E, T> result, Func<E, T> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);

            if (result.IsSuccess)
                return result;

            var value = fallback(result.Error);
            if (value is null)
                throw new InvalidOperationException("Recover fallback returned null.");

            return Result<E, T>.Ok(value);
        }

        /// <summary>
        /// Converts failure into another Result using a fallback function.
        /// 失敗時にフォールバック関数で別の Result を返す。
        /// </summary>
        public static Result<E, T> RecoverWith<E, T>(this Result<E, T> result, Func<E, Result<E, T>> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);
            return result.IsSuccess ? result : fallback(result.Error);
        }

        // ------------------------------
        // Combine / 組み合わせ
        // ------------------------------

        /// <summary>
        /// Combines two results using a selector function.
        /// 2つの Result を関数で組み合わせる。
        /// </summary>
        public static Result<E, U> Combine<E, T, R, U>(
            this in Result<E, T> result,
            in Result<E, R> other,
            Func<T, R, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (!result.IsSuccess)
                return Result<E, U>.Fail(result.Error);
            if (!other.IsSuccess)
                return Result<E, U>.Fail(other.Error);

            return Result<E, U>.Ok(selector(result.Value, other.Value));
        }

        /// <summary>
        /// Combines three results using a selector function.
        /// 3つの Result を関数で組み合わせる。
        /// </summary>
        public static Result<E, U> Combine<E, T, R1, R2, U>(
            this in Result<E, T> result,
            in Result<E, R1> other1,
            in Result<E, R2> other2,
            Func<T, R1, R2, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (!result.IsSuccess)
                return Result<E, U>.Fail(result.Error);
            if (!other1.IsSuccess)
                return Result<E, U>.Fail(other1.Error);
            if (!other2.IsSuccess)
                return Result<E, U>.Fail(other2.Error);

            return Result<E, U>.Ok(selector(result.Value, other1.Value, other2.Value));
        }

        // ------------------------------
        // Collections / コレクション
        // ------------------------------

        /// <summary>
        /// Converts a sequence of Result into a Result of List.
        /// Result のシーケンスを Result&lt;List&lt;T&gt;&gt; に変換する。
        /// </summary>
        public static Result<E, List<T>> Sequence<E, T>(this IEnumerable<Result<E, T>> results)
        {
            ArgumentNullException.ThrowIfNull(results);

            var list = new List<T>();
            foreach (var r in results)
            {
                if (!r.IsSuccess)
                    return Result<E, List<T>>.Fail(r.Error);

                list.Add(r.Value);
            }
            return Result<E, List<T>>.Ok(list);
        }

        /// <summary>
        /// Applies a function to each item and sequences the results.
        /// 各要素に関数を適用し、結果をまとめて Result&lt;List&lt;U&gt;&gt; にする。
        /// </summary>
        public static Result<E, List<U>> Traverse<E, T, U>(
            this IEnumerable<T> items,
            Func<T, Result<E, U>> selector)
        {
            ArgumentNullException.ThrowIfNull(items);
            ArgumentNullException.ThrowIfNull(selector);

            var list = new List<U>();
            foreach (var item in items)
            {
                var r = selector(item);
                if (!r.IsSuccess)
                    return Result<E, List<U>>.Fail(r.Error);

                list.Add(r.Value);
            }
            return Result<E, List<U>>.Ok(list);
        }

        // ------------------------------
        // Conversions / 変換
        // ------------------------------

        /// <summary>
        /// Flattens nested Result into a single Result.
        /// ネストされた Result を平坦化する。
        /// </summary>
        public static Result<E, T> Flatten<E, T>(this Result<E, Result<E, T>> result)
        {
            return result.IsSuccess ? result.Value : Result<E, T>.Fail(result.Error);
        }

        /// <summary>
        /// Converts Option to Result.
        /// Option を Result に変換する。
        /// </summary>
        public static Result<E, T> ToResultFromOption<E, T>(this Option<T> option, E errorIfNone)
        {
            ArgumentNullException.ThrowIfNull(errorIfNone);

            return option.HasValue ? Result<E, T>.Ok(option.Value) : Result<E, T>.Fail(errorIfNone);
        }

        /// <summary>
        /// Converts a value to Result; returns failure if null.
        /// 値を Result に変換し、null の場合は失敗を返す。
        /// </summary>
        public static Result<E, T> ToResult<E, T>(this T value, E errorIfNull)
        {
            ArgumentNullException.ThrowIfNull(errorIfNull);

            return value is null ? Result<E, T>.Fail(errorIfNull) : Result<E, T>.Ok(value);
        }
    }
}
