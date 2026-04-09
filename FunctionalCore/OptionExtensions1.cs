using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalCore
{
    public static class OptionExtensions1
    {
        /// <summary>
        /// Recovers from a missing value using the provided fallback function.
        /// 値がない場合にフォールバック関数で回復する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static Option<T> Recover<T>(this Option<T> option, Func<T> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);

            if (option.HasValue)
                return option;

            var val = fallback();
            return val is null ? Option<T>.None : Option<T>.Some(val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        public static Option<T> Try<T>(Func<T> func)
        {
            try
            {
                var val = func();
                return val == null ? Option<T>.None : Option<T>.Some(val);
            }
            catch (Exception)
            {
                return Option<T>.None;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="option"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Option<U> TryMap<T, U>(this Option<T> option, Func<T, U> selector)
        {
            if (option.HasValue == false)
            {
                return Option<U>.None;
            }
            try
            {
                var val = selector(option.Value);
                return val == null ? Option<U>.None : Option<U>.Some(val);
            }
            catch (Exception)
            {
                return Option<U>.None;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="option"></param>
        /// <param name="binder"></param>
        /// <returns></returns>
        public static Option<U> TryBind<T, U>(this Option<T> option, Func<T, Option<U>> binder)
        {
            if (option.HasValue == false)
            {
                return Option<U>.None;
            }
            try
            {
                var val = binder(option.Value);
                return val;
            }
            catch (Exception)
            {
                return Option<U>.None;
            }
        }

        /// <summary>
        /// Recovers from a missing value using a fallback Option.
        /// 値がない場合にフォールバック Option で回復する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static Option<T> RecoverWith<T>(this Option<T> option, Func<Option<T>> fallback)
        {
            ArgumentNullException.ThrowIfNull(fallback);

            return option.HasValue ? option : fallback();
        }

        #region Combine / 組み合わせ

        /// <summary>
        /// Combines two Option values using a function.
        /// 2つの Option を関数で組み合わせる
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="option"></param>
        /// <param name="other"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Option<U> Combine<T, R, U>(this Option<T> option, Option<R> other, Func<T, R, U> f)
        {
            ArgumentNullException.ThrowIfNull(f);

            if (option.HasValue == false)
            {
                return Option<U>.None;
            }
            if (other.HasValue == false)
            {
                return Option<U>.None;
            }

            return Option<U>.Some(f(option.Value, other.Value));
        }

        /// <summary>
        /// Combines three Option values using a function.
        /// 3つの Option を関数で組み合わせる
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R1"></typeparam>
        /// <typeparam name="R2"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="option"></param>
        /// <param name="other1"></param>
        /// <param name="other2"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Option<U> Combine<T, R1, R2, U>(this Option<T> option, Option<R1> other1, Option<R2> other2, Func<T, R1, R2, U> f)
        {
            ArgumentNullException.ThrowIfNull(f);

            if (option.HasValue == false)
            {
                return Option<U>.None;
            }
            if (other1.HasValue == false)
            {
                return Option<U>.None;
            }
            if (other2.HasValue == false)
            {
                return Option<U>.None;
            }

            return Option<U>.Some(f(option.Value, other1.Value, other2.Value));
        }

        /// <summary>
        /// Combines four Option values using a function.
        /// 4つの Option を関数で組み合わせる
        /// </summary>
        /// <typeparam name="T">Type of value / 値の型</typeparam>
        /// <typeparam name="R1"></typeparam>
        /// <typeparam name="R2"></typeparam>
        /// <typeparam name="R3"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="option"></param>
        /// <param name="other1"></param>
        /// <param name="other2"></param>
        /// <param name="other3"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static Option<U> Combine<T, R1, R2, R3, U>(this Option<T> option, Option<R1> other1, Option<R2> other2, Option<R3> other3, Func<T, R1, R2, R3, U> f)
        {
            ArgumentNullException.ThrowIfNull(f);

            if (option.HasValue == false)
            {
                return Option<U>.None;
            }
            if (other1.HasValue == false)
            {
                return Option<U>.None;
            }
            if (other2.HasValue == false)
            {
                return Option<U>.None;
            }
            if (other3.HasValue == false)
            {
                return Option<U>.None;
            }

            return Option<U>.Some(f(option.Value, other1.Value, other2.Value, other3.Value));
        }

        #endregion

        /// <summary>
        /// Flattens nested Option values.
        /// ネストされたOption<Option<T>>からOption<T>に変換する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="option"></param>
        /// <returns></returns>
        public static Option<T> Flatten<T>(this Option<Option<T>> option)
        {
            return option.HasValue ? option.Value : Option<T>.None;
        }
    }
}
