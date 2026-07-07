namespace FunctionalCore.AsyncExtensions
{
    /// <summary>
    /// <para>Option の非同期パイプラインを構成するための拡張メソッドを提供する。</para>
    /// <para>Provides extension methods for composing asynchronous Option pipelines.</para>
    /// </summary>
    public static class OptionAsyncExtensions
    {
        /// <summary>
        /// <para>Option を Task<Option> に持ち上げ、非同期パイプラインへ入場させる。</para>
        /// <para>Lifts an Option into Task<Option> and enters the asynchronous pipeline.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Option が保持する値の型。</para>
        /// <para>The value type contained in the Option.</para>
        /// </typeparam>
        /// <param name="option">
        /// <para>非同期パイプラインへ持ち上げる Option。</para>
        /// <para>The Option to lift into the asynchronous pipeline.</para>
        /// </param>
        /// <returns>
        /// <para>指定された Option を保持する完了済み Task。</para>
        /// <para>A completed Task that contains the specified Option.</para>
        /// </returns>
        public static Task<Option<T>> AsTask<T>(this Option<T> option)
        {
            return Task.FromResult(option);
        }

        /// <summary>
        /// <para>値を持つ Option の値を使って、次の非同期 Option 処理へ接続する。</para>
        /// <para>Binds an Option value to the next asynchronous Option-producing operation when the Option has a value.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>現在の値の型。</para>
        /// <para>The current value type.</para>
        /// </typeparam>
        /// <typeparam name="U">
        /// <para>次の値の型。</para>
        /// <para>The next value type.</para>
        /// </typeparam>
        /// <param name="optionTask">
        /// <para>接続元の非同期 Option。</para>
        /// <para>The source asynchronous Option.</para>
        /// </param>
        /// <param name="binder">
        /// <para>値から次の非同期 Option を生成する関数。</para>
        /// <para>A function that produces the next asynchronous Option from the value.</para>
        /// </param>
        /// <returns>
        /// <para>接続後の非同期 Option。元の Option が None の場合は None を返す。</para>
        /// <para>The bound asynchronous Option. If the source Option is None, None is returned.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="optionTask"/> または <paramref name="binder"/> が null の場合にスローされる。</para>
        /// <para>Thrown when <paramref name="optionTask"/> or <paramref name="binder"/> is null.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="binder"/> が null Task を返した場合にスローされる。</para>
        /// <para>Thrown when <paramref name="binder"/> returns a null Task.</para>
        /// </exception>
        public static async Task<Option<U>> BindAsync<T, U>(this Task<Option<T>> optionTask, Func<T, Task<Option<U>>> binder)
        {
            ArgumentNullException.ThrowIfNull(optionTask);
            ArgumentNullException.ThrowIfNull(binder);

            var option = await optionTask.ConfigureAwait(false);

            if (!option.HasValue)
                return Option<U>.None;

            var nextTask = binder(option.Value);

            if (nextTask is null)
                throw new InvalidOperationException("Binder must not return null task.");

            return await nextTask.ConfigureAwait(false);
        }

        /// <summary>
        /// <para>値を持つ Option の値を、非同期 selector で別の値に変換する。</para>
        /// <para>Maps an Option value to another value by using an asynchronous selector when the Option has a value.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>変換前の値の型。</para>
        /// <para>The value type before mapping.</para>
        /// </typeparam>
        /// <typeparam name="U">
        /// <para>変換後の値の型。</para>
        /// <para>The value type after mapping.</para>
        /// </typeparam>
        /// <param name="optionTask">
        /// <para>変換元の非同期 Option。</para>
        /// <para>The source asynchronous Option.</para>
        /// </param>
        /// <param name="selector">
        /// <para>値を非同期に変換する関数。</para>
        /// <para>A function that asynchronously maps the value.</para>
        /// </param>
        /// <returns>
        /// <para>変換後の非同期 Option。元の Option が None の場合、または selector の結果が null の場合は None を返す。</para>
        /// <para>The mapped asynchronous Option. If the source Option is None, or if the selector returns a null value, None is returned.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="optionTask"/> または <paramref name="selector"/> が null の場合にスローされる。</para>
        /// <para>Thrown when <paramref name="optionTask"/> or <paramref name="selector"/> is null.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="selector"/> が null Task を返した場合にスローされる。</para>
        /// <para>Thrown when <paramref name="selector"/> returns a null Task.</para>
        /// </exception>
        public static async Task<Option<U>> MapAsync<T, U>(this Task<Option<T>> optionTask, Func<T, Task<U>> selector)
        {
            ArgumentNullException.ThrowIfNull(optionTask);
            ArgumentNullException.ThrowIfNull(selector);

            var option = await optionTask.ConfigureAwait(false);

            if (!option.HasValue)
                return Option<U>.None;

            var valueTask = selector(option.Value);

            if (valueTask is null)
                throw new InvalidOperationException("Selector must not return null task.");

            var value = await valueTask.ConfigureAwait(false);

            return value is null
                ? Option<U>.None
                : Option<U>.Some(value);
        }

        /// <summary>
        /// <para>値を持つ Option に対して、非同期の副作用を実行する。</para>
        /// <para>Executes an asynchronous side effect when the Option has a value.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Option が保持する値の型。</para>
        /// <para>The value type contained in the Option.</para>
        /// </typeparam>
        /// <param name="optionTask">
        /// <para>副作用を挟む対象の非同期 Option。</para>
        /// <para>The asynchronous Option to tap.</para>
        /// </param>
        /// <param name="onSome">
        /// <para>値に対して実行する非同期の副作用。</para>
        /// <para>The asynchronous side effect to execute for the value.</para>
        /// </param>
        /// <returns>
        /// <para>元の Option を保持する非同期 Option。</para>
        /// <para>The asynchronous Option that contains the original Option.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="optionTask"/> または <paramref name="onSome"/> が null の場合にスローされる。</para>
        /// <para>Thrown when <paramref name="optionTask"/> or <paramref name="onSome"/> is null.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="onSome"/> が null Task を返した場合にスローされる。</para>
        /// <para>Thrown when <paramref name="onSome"/> returns a null Task.</para>
        /// </exception>
        public static async Task<Option<T>> TapAsync<T>(this Task<Option<T>> optionTask, Func<T, Task> onSome)
        {
            ArgumentNullException.ThrowIfNull(optionTask);
            ArgumentNullException.ThrowIfNull(onSome);

            var option = await optionTask.ConfigureAwait(false);

            if (option.HasValue)
            {
                var actionTask = onSome(option.Value);

                if (actionTask is null)
                    throw new InvalidOperationException("OnSome must not return null task.");

                await actionTask.ConfigureAwait(false);
            }

            return option;
        }

        /// <summary>
        /// <para>None の Option に対して、非同期の副作用を実行する。</para>
        /// <para>Executes an asynchronous side effect when the Option is None.</para>
        /// </summary>
        /// <typeparam name="T">
        /// <para>Option が保持する値の型。</para>
        /// <para>The value type contained in the Option.</para>
        /// </typeparam>
        /// <param name="optionTask">
        /// <para>副作用を挟む対象の非同期 Option。</para>
        /// <para>The asynchronous Option to tap.</para>
        /// </param>
        /// <param name="onNone">
        /// <para>None の場合に実行する非同期の副作用。</para>
        /// <para>The asynchronous side effect to execute when the Option is None.</para>
        /// </param>
        /// <returns>
        /// <para>元の Option を保持する非同期 Option。</para>
        /// <para>The asynchronous Option that contains the original Option.</para>
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="optionTask"/> または <paramref name="onNone"/> が null の場合にスローされる。</para>
        /// <para>Thrown when <paramref name="optionTask"/> or <paramref name="onNone"/> is null.</para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para><paramref name="onNone"/> が null Task を返した場合にスローされる。</para>
        /// <para>Thrown when <paramref name="onNone"/> returns a null Task.</para>
        /// </exception>
        public static async Task<Option<T>> TapNoneAsync<T>(this Task<Option<T>> optionTask, Func<Task> onNone)
        {
            ArgumentNullException.ThrowIfNull(optionTask);
            ArgumentNullException.ThrowIfNull(onNone);

            var option = await optionTask.ConfigureAwait(false);

            if (!option.HasValue)
            {
                var actionTask = onNone();

                if (actionTask is null)
                    throw new InvalidOperationException("OnNone must not return null task.");

                await actionTask.ConfigureAwait(false);
            }

            return option;
        }
    }
}
