namespace FunctionalCore.AsyncExtensions
{
    /// <summary>
    /// <para>Result の非同期パイプラインを構成するための拡張メソッドを提供します。</para>
    /// <para>Provides extension methods for composing asynchronous Result pipelines.</para>
    /// </summary>
    public static class ResultAsyncExtensions
    {
        /// <summary>
        /// <para>Result を Task<Result> に持ち上げ、非同期パイプラインへ入場させる。</para>
        /// <para>Lifts a Result into Task<Result> and enters the asynchronous pipeline.</para>
        /// </summary>
        /// <typeparam name="E">
        /// <para>失敗時のエラー型。</para>
        /// <para>The error type used when the result is failed.</para>
        /// </typeparam>
        /// <typeparam name="T">
        /// <para>成功時の値の型。</para>
        /// <para>The value type used when the result is successful.</para>
        /// </typeparam>
        /// <param name="result">
        /// <para>非同期パイプラインへ持ち上げる Result。</para>
        /// <para>The Result to lift into the asynchronous pipeline.</para>
        /// </param>
        /// <returns>
        /// <para>指定された Result を保持する完了済み Task。</para>
        /// <para>A completed Task that contains the specified Result.</para>
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="result"/> が未初期化の場合にスローされる。</para>
        /// <para>Thrown when <paramref name="result"/> is not initialized.</para>
        /// </exception>
        public static Task<Result<E, T>> AsTask<E, T>(this Result<E, T> result)
        {
            result.ThrowIfNotInitialized();
            return Task.FromResult(result);
        }

        /// <summary>
        /// <para>成功している Result の値を使って、次の非同期 Result 処理へ接続する。</para>
        /// <para>Binds a successful Result value to the next asynchronous Result-producing operation.</para>
        /// </summary>
        /// <typeparam name="E">
        /// <para>失敗時のエラー型。</para>
        /// <para>The error type used when the result is failed.</para>
        /// </typeparam>
        /// <typeparam name="T">
        /// <para>現在の成功値の型。</para>
        /// <para>The current successful value type.</para>
        /// </typeparam>
        /// <typeparam name="U">
        /// <para>次の成功値の型。</para>
        /// <para>The next successful value type.</para>
        /// </typeparam>
        /// <param name="resultTask">
        /// <para>接続元の非同期 Result。</para>
        /// <para>The source asynchronous Result.</para>
        /// </param>
        /// <param name="binder">
        /// <para>成功値から次の非同期 Result を生成する関数。</para>
        /// <para>A function that produces the next asynchronous Result from the successful value.</para>
        /// </param>
        /// <returns>
        /// <para>接続後の非同期 Result。元の Result が失敗している場合は、その失敗をそのまま返す。</para>
        /// <para>The bound asynchronous Result. If the source Result is failed, the original failure is returned.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="resultTask"/> または <paramref name="binder"/> が null の場合にスローされる。</para>
        /// <para>Thrown when <paramref name="resultTask"/> or <paramref name="binder"/> is null.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>Result が未初期化の場合、または <paramref name="binder"/> が null Task を返した場合にスローされる。</para>
        /// <para>Thrown when a Result is not initialized, or when <paramref name="binder"/> returns a null Task.</para>
        /// </exception>
        public static async Task<Result<E, U>> BindAsync<E, T, U>(this Task<Result<E, T>> resultTask, Func<T, Task<Result<E, U>>> binder)
        {
            ArgumentNullException.ThrowIfNull(resultTask);
            ArgumentNullException.ThrowIfNull(binder);

            var result = await resultTask.ConfigureAwait(false);
            result.ThrowIfNotInitialized();

            if (!result.IsSuccess)
                return Result<E, U>.Fail(result.Error);

            var nextTask = binder(result.Value);

            if (nextTask is null)
                throw new InvalidOperationException("Binder must not return null task.");

            var next = await nextTask.ConfigureAwait(false);
            next.ThrowIfNotInitialized();

            return next;
        }

        /// <summary>
        /// <para>成功している Result の値を、非同期 selector で別の値に変換する。</para>
        /// <para>Maps a successful Result value to another value by using an asynchronous selector.</para>
        /// </summary>
        /// <typeparam name="E">
        /// <para>失敗時のエラー型。</para>
        /// <para>The error type used when the result is failed.</para>
        /// </typeparam>
        /// <typeparam name="T">
        /// <para>変換前の成功値の型。</para>
        /// <para>The successful value type before mapping.</para>
        /// </typeparam>
        /// <typeparam name="U">
        /// <para>変換後の成功値の型。</para>
        /// <para>The successful value type after mapping.</para>
        /// </typeparam>
        /// <param name="resultTask">
        /// <para>変換元の非同期 Result。</para>
        /// <para>The source asynchronous Result.</para>
        /// </param>
        /// <param name="selector">
        /// <para>成功値を非同期に変換する関数。</para>
        /// <para>A function that asynchronously maps the successful value.</para>
        /// </param>
        /// <returns>
        /// <para>変換後の非同期 Result。元の Result が失敗している場合は、その失敗をそのまま返す。</para>
        /// <para>The mapped asynchronous Result. If the source Result is failed, the original failure is returned.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="resultTask"/> または <paramref name="selector"/> が null の場合にスローされる。</para>
        /// <para>Thrown when <paramref name="resultTask"/> or <paramref name="selector"/> is null.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>Result が未初期化の場合、<paramref name="selector"/> が null Task を返した場合、または null の値を返した場合にスローされる。</para>
        /// <para>Thrown when a Result is not initialized, when <paramref name="selector"/> returns a null Task, or when it returns a null value.</para>
        /// </exception>
        public static async Task<Result<E, U>> MapAsync<E, T, U>(this Task<Result<E, T>> resultTask, Func<T, Task<U>> selector)
        {
            ArgumentNullException.ThrowIfNull(resultTask);
            ArgumentNullException.ThrowIfNull(selector);

            var result = await resultTask.ConfigureAwait(false);
            result.ThrowIfNotInitialized();

            if (!result.IsSuccess)
                return Result<E, U>.Fail(result.Error);

            var valueTask = selector(result.Value);

            if (valueTask is null)
                throw new InvalidOperationException("Selector must not return null task.");

            var val = await valueTask.ConfigureAwait(false);

            if (val is null)
                throw new InvalidOperationException("Selector must not return null.");

            return Result<E, U>.Ok(val);
        }

        /// <summary>
        /// <para>成功している Result に対して、非同期の副作用を実行する。</para>
        /// <para>Executes an asynchronous side effect for a successful Result.</para>
        /// </summary>
        /// <typeparam name="E">
        /// <para>失敗時のエラー型。</para>
        /// <para>The error type used when the result is failed.</para>
        /// </typeparam>
        /// <typeparam name="T">
        /// <para>成功時の値の型。</para>
        /// <para>The value type used when the result is successful.</para>
        /// </typeparam>
        /// <param name="resultTask">
        /// <para>副作用を挟む対象の非同期 Result。</para>
        /// <para>The asynchronous Result to tap.</para>
        /// </param>
        /// <param name="onSuccess">
        /// <para>成功値に対して実行する非同期の副作用。</para>
        /// <para>The asynchronous side effect to execute for the successful value.</para>
        /// </param>
        /// <returns>
        /// <para>元の Result を保持する非同期 Result。</para>
        /// <para>The asynchronous Result that contains the original Result.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="resultTask"/> または <paramref name="onSuccess"/> が null の場合にスローされる。</para>
        /// <para>Thrown when <paramref name="resultTask"/> or <paramref name="onSuccess"/> is null.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>Result が未初期化の場合、または <paramref name="onSuccess"/> が null Task を返した場合にスローされる。</para>
        /// <para>Thrown when a Result is not initialized, or when <paramref name="onSuccess"/> returns a null Task.</para>
        /// </exception>
        public static async Task<Result<E, T>> TapAsync<E, T>(this Task<Result<E, T>> resultTask, Func<T, Task> onSuccess)
        {
            ArgumentNullException.ThrowIfNull(resultTask);
            ArgumentNullException.ThrowIfNull(onSuccess);

            var result = await resultTask.ConfigureAwait(false);
            result.ThrowIfNotInitialized();

            if (result.IsSuccess)
            {
                var actionTask = onSuccess(result.Value);

                if (actionTask is null)
                    throw new InvalidOperationException("OnSuccess must not return null task.");

                await actionTask.ConfigureAwait(false);
            }

            return result;
        }

        /// <summary>
        /// <para>失敗している Result に対して、非同期の副作用を実行する。</para>
        /// <para>Executes an asynchronous side effect for a failed Result.</para>
        /// </summary>
        /// <typeparam name="E">
        /// <para>失敗時のエラー型。</para>
        /// <para>The error type used when the result is failed.</para>
        /// </typeparam>
        /// <typeparam name="T">
        /// <para>成功時の値の型。</para>
        /// <para>The value type used when the result is successful.</para>
        /// </typeparam>
        /// <param name="resultTask">
        /// <para>副作用を挟む対象の非同期 Result。</para>
        /// <para>The asynchronous Result to tap.</para>
        /// </param>
        /// <param name="onFailure">
        /// <para>エラーに対して実行する非同期の副作用。</para>
        /// <para>The asynchronous side effect to execute for the error.</para>
        /// </param>
        /// <returns>
        /// <para>元の Result を保持する非同期 Result。</para>
        /// <para>The asynchronous Result that contains the original Result.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="resultTask"/> または <paramref name="onFailure"/> が null の場合にスローされる。</para>
        /// <para>Thrown when <paramref name="resultTask"/> or <paramref name="onFailure"/> is null.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>Result が未初期化の場合、または <paramref name="onFailure"/> が null Task を返した場合にスローされる。</para>
        /// <para>Thrown when a Result is not initialized, or when <paramref name="onFailure"/> returns a null Task.</para>
        /// </exception>
        public static async Task<Result<E, T>> TapErrorAsync<E, T>(this Task<Result<E, T>> resultTask, Func<E, Task> onFailure)
        {
            ArgumentNullException.ThrowIfNull(resultTask);
            ArgumentNullException.ThrowIfNull(onFailure);

            var result = await resultTask.ConfigureAwait(false);
            result.ThrowIfNotInitialized();

            if (!result.IsSuccess)
            {
                var actionTask = onFailure(result.Error);

                if (actionTask is null)
                    throw new InvalidOperationException("OnFailure must not return null task.");

                await actionTask.ConfigureAwait(false);
            }

            return result;
        }
    }
}
