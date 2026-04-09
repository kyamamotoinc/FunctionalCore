using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionalCore
{
    public static class ResultExtensions1
    {
        /// <summary>
        /// Converts failure into success by applying a handler to the error
        /// 失敗時にエラーを値に変換して成功Resultにする
        /// </summary>
        /// <typeparam name="E">Type of error / エラーの型</typeparam>
        /// <typeparam name="T">Type of value / 値の型</typeparam>
        /// <param name="result"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static Result<E, T> Recover<E, T>(this Result<E, T> result, Func<E, T> handler)
        {
            if (result.IsSuccess)
            {
                return result;
            }

            ArgumentNullException.ThrowIfNull(handler);

            var newValue = handler(result.Error);

            if (newValue is null)
            {
                throw new InvalidOperationException("Recover handler returned null");
            }

            return Result<E, T>.Ok(newValue);
        }

        /// <summary>
        /// Converts failure into another Result using a handler
        /// 失敗時に別のResultを返して回復する
        /// </summary>
        /// <typeparam name="E">Type of error / エラーの型</typeparam>
        /// <typeparam name="T">Type of value / 値の型</typeparam>
        /// <param name="result"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public static Result<E, T> RecoverWith<E, T>(this Result<E, T> result, Func<E, Result<E, T>> handler)
        {
            if (result.IsSuccess)
            {
                return result;
            }

            ArgumentNullException.ThrowIfNull(handler);

            var resultLocal = handler(result.Error);

            //if (resultLocal == null)
            //{
            //    throw new InvalidOperationException("RecoverWith handler returned null");
            //}

            return resultLocal;
        }
        #region Try
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="errorMapper"></param>
        /// <returns></returns>
        public static Result<E, T> Try<E, T>(Func<T> f, Func<Exception, E> errorMapper)
        {
            ArgumentNullException.ThrowIfNull(f);
            ArgumentNullException.ThrowIfNull(errorMapper);

            try
            {
                var val = f();
                return Result<E, T>.Ok(val);
            }
            catch (Exception ex)
            {
                return Result<E, T>.Fail(errorMapper(ex));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="result"></param>
        /// <param name="f"></param>
        /// <param name="errorMapper"></param>
        /// <returns></returns>
        public static Result<E, U> TryMap<E, T, U>(this Result<E, T> result, Func<T, U> f, Func<Exception, E> errorMapper)
        {
            ArgumentNullException.ThrowIfNull(f);
            ArgumentNullException.ThrowIfNull(errorMapper);

            if (result.IsSuccess == false)
            {
                return Result<E, U>.Fail(result.Error);
            }
            try
            {
                var mapped = f(result.Value);
                return Result<E, U>.Ok(mapped);
            }
            catch (Exception ex)
            {
                return Result<E, U>.Fail(errorMapper(ex));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="result"></param>
        /// <param name="f"></param>
        /// <param name="errorMapper"></param>
        /// <returns></returns>
        public static Result<E, U> TryBind<E, T, U>(this Result<E, T> result, Func<T, Result<E, U>> f, Func<Exception, E> errorMapper)
        {
            ArgumentNullException.ThrowIfNull(f);
            ArgumentNullException.ThrowIfNull(errorMapper);

            if (result.IsSuccess == false)
            {
                return Result<E, U>.Fail(result.Error);
            }
            try
            {
                return f(result.Value);
            }
            catch (Exception ex)
            {
                return Result<E, U>.Fail(errorMapper(ex));
            }
        }

        #endregion

        #region Combine / 組み合わせ

        /// <summary>
        /// Combines two results into one using the provided function
        /// 2つのResultを組み合わせて1つのResultにする
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="result"></param>
        /// <param name="other"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Result<E, U> Combine<E, T, R, U>(this Result<E, T> result, Result<E, R> other, Func<T, R, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (result.IsSuccess == false)
            {
                return Result<E, U>.Fail(result.Error);
            }
            if (other.IsSuccess == false)
            {
                return Result<E, U>.Fail(other.Error);
            }

            return Result<E, U>.Ok(selector(result.Value, other.Value));
        }

        /// <summary>
        /// Combines three results into one using the provided function
        /// 3つのResultを組み合わせて1つのResultにする
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R1"></typeparam>
        /// <typeparam name="R2"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="result"></param>
        /// <param name="other1"></param>
        /// <param name="other2"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Result<E, U> Combine<E, T, R1, R2, U>(this Result<E, T> result, Result<E, R1> other1, Result<E, R2> other2, Func<T, R1, R2, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (result.IsSuccess == false)
            {
                return Result<E, U>.Fail(result.Error);
            }
            if (other1.IsSuccess == false)
            {
                return Result<E, U>.Fail(other1.Error);
            }
            if (other2.IsSuccess == false)
            {
                return Result<E, U>.Fail(other2.Error);
            }

            return Result<E, U>.Ok(selector(result.Value, other1.Value, other2.Value));
        }

        /// <summary>
        /// Combines four results into one using the provided function
        /// 4つのResultを組み合わせて1つのResultにする
        /// </summary>
        /// <typeparam name="E"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="R1"></typeparam>
        /// <typeparam name="R2"></typeparam>
        /// <typeparam name="R3"></typeparam>
        /// <typeparam name="U"></typeparam>
        /// <param name="result"></param>
        /// <param name="other1"></param>
        /// <param name="other2"></param>
        /// <param name="other3"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static Result<E, U> Combine<E, T, R1, R2, R3, U>(this Result<E, T> result, Result<E, R1> other1, Result<E, R2> other2, Result<E, R3> other3, Func<T, R1, R2, R3, U> selector)
        {
            ArgumentNullException.ThrowIfNull(selector);

            if (result.IsSuccess == false)
            {
                return Result<E, U>.Fail(result.Error);
            }
            if (other1.IsSuccess == false)
            {
                return Result<E, U>.Fail(other1.Error);
            }
            if (other2.IsSuccess == false)
            {
                return Result<E, U>.Fail(other2.Error);
            }
            if (other3.IsSuccess == false)
            {
                return Result<E, U>.Fail(other3.Error);
            }

            return Result<E, U>.Ok(selector(result.Value, other1.Value, other2.Value, other3.Value));
        }

        #endregion
        /// <summary>
        /// Flatten a nested Result into a single Result.
        /// ネストした Result を平坦化して単一の Result に変換する
        /// Example: Result<E, Result<E, T>> → Result<E, T>
        /// </summary>
        /// <typeparam name="E">Type of error / エラーの型</typeparam>
        /// <typeparam name="T">Type of value / 値の型</typeparam>
        /// <param name="result">Nested Result / ネストされた Result</param>
        /// <returns>Flattened Result / 平坦化された Result</returns>
        public static Result<E, T> Flatten<E, T>(this Result<E, Result<E, T>> result)
        {
            // If outer Result is failure, return the error
            // 外側が失敗の場合はそのまま返す
            if (result.IsSuccess == false)
            {
                return Result<E, T>.Fail(result.Error);
            }

            // If outer Result is success, return inner Result
            // 外側が成功なら内側の Result を返す
            var inner = result.Value;

            //if (inner == null)
            //{
            //    throw new InvalidOperationException("Inner Result cannot be null");
            //}

            return inner;
        }
    }
}
