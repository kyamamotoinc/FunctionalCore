namespace FunctionalCore.AsyncExtensions;

/// <summary>
/// Provides extension methods for composing asynchronous Result pipelines.
/// <para>Result の非同期パイプラインを構成するための拡張メソッドを提供する。</para>
/// </summary>
public static class ResultAsyncExtensions
{
    /// <summary>
    /// Lifts a Result into Task&lt;Result&gt; and enters the asynchronous pipeline.
    /// <para>Result を Task&lt;Result&gt; に持ち上げ、非同期パイプラインへ入場させる。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type used when the result is failed.
    /// <para>失敗時のエラー型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The value type used when the result is successful.
    /// <para>成功時の値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The Result to lift into the asynchronous pipeline.
    /// <para>非同期パイプラインへ持ち上げる Result。</para>
    /// </param>
    /// <returns>
    /// A completed Task that contains the specified Result.
    /// <para>指定された Result を保持する完了済み Task。</para>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is not initialized.
    /// <para><paramref name="result"/> が未初期化の場合にスローされる。</para>
    /// </exception>
    public static Task<Result<E, T>> AsTask<E, T>(this Result<E, T> result)
    {
        result.ThrowIfNotInitialized();
        return Task.FromResult(result);
    }

    /// <summary>
    /// Matches success or failure asynchronously and returns a value.
    /// <para>成功/失敗に応じた非同期関数を実行し、値を返す。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type.
    /// <para>成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The return type.
    /// <para>戻り値の型。</para>
    /// </typeparam>
    /// <param name="resultTask">
    /// A Task that produces the source Result.
    /// <para>元の Result を生成する Task。</para>
    /// </param>
    /// <param name="onSuccess">
    /// An asynchronous function to execute if successful. Must not return a null Task or a null value.
    /// <para>成功時に実行する非同期関数。null の Task または null の値を返してはならない。</para>
    /// </param>
    /// <param name="onFailure">
    /// An asynchronous function to execute if failed. Must not return a null Task or a null value.
    /// <para>失敗時に実行する非同期関数。null の Task または null の値を返してはならない。</para>
    /// </param>
    /// <returns>
    /// A Task that produces the value returned by the selected function.
    /// <para>選択された関数の戻り値を生成する Task。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if resultTask, onSuccess, or onFailure is null.
    /// <para>resultTask、onSuccess、または onFailure が null の場合に投げられる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the source Result is uninitialized,
    /// if the selected function returns a null Task,
    /// or if the Task returns null.
    /// <para>
    /// 元の Result が未初期化、選択された関数が null の Task を返した、
    /// または Task が null を返した場合に投げられる。
    /// </para>
    /// </exception>
    public static async Task<U> MatchAsync<E, T, U>(this Task<Result<E, T>> resultTask, Func<T, Task<U>> onSuccess, Func<E, Task<U>> onFailure)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(onSuccess);
        ArgumentNullException.ThrowIfNull(onFailure);

        var result = await resultTask.ConfigureAwait(false);
        result.ThrowIfNotInitialized();

        var valueTask = result.IsSuccess ? onSuccess(result.Value) : onFailure(result.Error);

        if (valueTask is null)
            throw new InvalidOperationException("Match function must not return a null Task.");

        var value = await valueTask.ConfigureAwait(false);

        if (value is null)
            throw new InvalidOperationException("Match function must not return null.");

        return value;
    }

    /// <summary>
    /// Binds a successful Result value to the next asynchronous Result-producing operation.
    /// <para>成功している Result の値を使って、次の非同期 Result 処理へ接続する。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type used when the result is failed.
    /// <para>失敗時のエラー型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The current successful value type.
    /// <para>現在の成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The next successful value type.
    /// <para>次の成功値の型。</para>
    /// </typeparam>
    /// <param name="resultTask">
    /// The source asynchronous Result.
    /// <para>接続元の非同期 Result。</para>
    /// </param>
    /// <param name="binder">
    /// A function that produces the next asynchronous Result from the successful value.
    /// <para>成功値から次の非同期 Result を生成する関数。</para>
    /// </param>
    /// <returns>
    /// The bound asynchronous Result. If the source Result is failed, the original failure is returned.
    /// <para>接続後の非同期 Result。元の Result が失敗している場合は、その失敗をそのまま返す。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="binder"/> is null.
    /// <para><paramref name="resultTask"/> または <paramref name="binder"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a Result is not initialized, or when <paramref name="binder"/> returns a null Task.
    /// <para>Result が未初期化の場合、または <paramref name="binder"/> が null の Task を返した場合にスローされる。</para>
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
            throw new InvalidOperationException("Binder must not return a null Task.");

        var next = await nextTask.ConfigureAwait(false);
        next.ThrowIfNotInitialized();

        return next;
    }

    /// <summary>
    /// Maps a successful Result value to another value by using an asynchronous selector.
    /// <para>成功している Result の値を、非同期 selector で別の値に変換する。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type used when the result is failed.
    /// <para>失敗時のエラー型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The successful value type before mapping.
    /// <para>変換前の成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The successful value type after mapping.
    /// <para>変換後の成功値の型。</para>
    /// </typeparam>
    /// <param name="resultTask">
    /// The source asynchronous Result.
    /// <para>変換元の非同期 Result。</para>
    /// </param>
    /// <param name="selector">
    /// A function that asynchronously maps the successful value.
    /// <para>成功値を非同期に変換する関数。</para>
    /// </param>
    /// <returns>
    /// The mapped asynchronous Result. If the source Result is failed, the original failure is returned.
    /// <para>変換後の非同期 Result。元の Result が失敗している場合は、その失敗をそのまま返す。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="selector"/> is null.
    /// <para><paramref name="resultTask"/> または <paramref name="selector"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a Result is not initialized, when <paramref name="selector"/> returns a null Task, or when it returns a null value.
    /// <para>Result が未初期化の場合、<paramref name="selector"/> が null の Task を返した場合、または null の値を返した場合にスローされる。</para>
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
            throw new InvalidOperationException("Selector must not return a null Task.");

        var val = await valueTask.ConfigureAwait(false);

        if (val is null)
            throw new InvalidOperationException("Selector must not return null.");

        return Result<E, U>.Ok(val);
    }

    /// <summary>
    /// Ensures that the successful value satisfies the specified asynchronous predicate.
    /// <para>成功値が指定された非同期条件を満たすことを確認する。</para>
    ///
    /// If the source Result is a failure, the original failure is returned without invoking
    /// <paramref name="predicate"/> or <paramref name="errorFactory"/>.
    /// If the source Result is successful and <paramref name="predicate"/> returns true,
    /// the original Result is returned unchanged.
    /// If <paramref name="predicate"/> returns false,
    /// <paramref name="errorFactory"/> is invoked and its error is used to create a failed Result.
    /// <para>
    /// 元の Result が失敗している場合は、<paramref name="predicate"/> および
    /// <paramref name="errorFactory"/> を実行せず、元の失敗 Result をそのまま返す。
    /// 元の Result が成功しており、<paramref name="predicate"/> が true を返した場合は、
    /// 元の Result をそのまま返す。
    /// <paramref name="predicate"/> が false を返した場合は、
    /// <paramref name="errorFactory"/> を実行し、そのエラーを使用して失敗 Result を生成する。
    /// </para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type.
    /// <para>成功値の型。</para>
    /// </typeparam>
    /// <param name="resultTask">
    /// A task that produces the source Result.
    /// <para>元の Result を生成する Task。</para>
    /// </param>
    /// <param name="predicate">
    /// An asynchronous predicate used to validate the successful value.
    /// Must not be null and must not return a null Task.
    /// <para>
    /// 成功値を検証する非同期条件関数。
    /// null は許可されず、null の Task を返してはならない。
    /// </para>
    /// </param>
    /// <param name="errorFactory">
    /// A function that creates the error when <paramref name="predicate"/> returns false.
    /// Must not be null and must not return null.
    /// <para>
    /// <paramref name="predicate"/> が false を返した場合にエラーを生成する関数。
    /// null は許可されず、null を返してはならない。
    /// </para>
    /// </param>
    /// <returns>
    /// A task that produces the original Result when the source is a failure or when
    /// <paramref name="predicate"/> returns true; otherwise, a failed Result containing
    /// the error created by <paramref name="errorFactory"/>.
    /// <para>
    /// 元の Result が失敗している場合、または <paramref name="predicate"/> が true を返した場合は
    /// 元の Result を返す Task。
    /// それ以外の場合は、<paramref name="errorFactory"/> が生成したエラーを保持する
    /// 失敗 Result を返す Task。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/>, <paramref name="predicate"/>,
    /// or <paramref name="errorFactory"/> is null.
    /// <para>
    /// <paramref name="resultTask"/>、<paramref name="predicate"/>、
    /// または <paramref name="errorFactory"/> が null の場合にスローされる。
    /// </para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the source Result is not initialized,
    /// when <paramref name="predicate"/> returns a null Task,
    /// or when <paramref name="errorFactory"/> returns null.
    /// <para>
    /// 元の Result が未初期化の場合、
    /// <paramref name="predicate"/> が null の Task を返した場合、
    /// または <paramref name="errorFactory"/> が null を返した場合にスローされる。
    /// </para>
    /// </exception>
    public static async Task<Result<E, T>> EnsureAsync<E, T>(this Task<Result<E, T>> resultTask, Func<T, Task<bool>> predicate, Func<T, E> errorFactory)
    {
        ArgumentNullException.ThrowIfNull(resultTask);
        ArgumentNullException.ThrowIfNull(predicate);
        ArgumentNullException.ThrowIfNull(errorFactory);

        var result = await resultTask.ConfigureAwait(false);
        result.ThrowIfNotInitialized();

        if (result.IsFailure)
            return result;

        var predicateTask = predicate(result.Value);

        if (predicateTask is null)
            throw new InvalidOperationException("Predicate must not return a null Task.");

        var isValid = await predicateTask.ConfigureAwait(false);

        if (isValid)
            return result;

        var error = errorFactory(result.Value);

        if (error is null)
            throw new InvalidOperationException("Error factory must not return null.");

        return Result<E, T>.Fail(error);
    }

    /// <summary>
    /// Executes an asynchronous side effect for a successful Result.
    /// <para>成功している Result に対して、非同期の副作用を実行する。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type used when the result is failed.
    /// <para>失敗時のエラー型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The value type used when the result is successful.
    /// <para>成功時の値の型。</para>
    /// </typeparam>
    /// <param name="resultTask">
    /// The asynchronous Result to tap.
    /// <para>副作用を挟む対象の非同期 Result。</para>
    /// </param>
    /// <param name="onSuccess">
    /// The asynchronous side effect to execute for the successful value.
    /// <para>成功値に対して実行する非同期の副作用。</para>
    /// </param>
    /// <returns>
    /// The asynchronous Result that contains the original Result.
    /// <para>元の Result を保持する非同期 Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="onSuccess"/> is null.
    /// <para><paramref name="resultTask"/> または <paramref name="onSuccess"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a Result is not initialized, or when <paramref name="onSuccess"/> returns a null Task.
    /// <para>Result が未初期化の場合、または <paramref name="onSuccess"/> が null の Task を返した場合にスローされる。</para>
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
                throw new InvalidOperationException("OnSuccess must not return a null Task.");

            await actionTask.ConfigureAwait(false);
        }

        return result;
    }

    /// <summary>
    /// Executes an asynchronous side effect for a failed Result.
    /// <para>失敗している Result に対して、非同期の副作用を実行する。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type used when the result is failed.
    /// <para>失敗時のエラー型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The value type used when the result is successful.
    /// <para>成功時の値の型。</para>
    /// </typeparam>
    /// <param name="resultTask">
    /// The asynchronous Result to tap.
    /// <para>副作用を挟む対象の非同期 Result。</para>
    /// </param>
    /// <param name="onFailure">
    /// The asynchronous side effect to execute for the error.
    /// <para>エラーに対して実行する非同期の副作用。</para>
    /// </param>
    /// <returns>
    /// The asynchronous Result that contains the original Result.
    /// <para>元の Result を保持する非同期 Result。</para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="resultTask"/> or <paramref name="onFailure"/> is null.
    /// <para><paramref name="resultTask"/> または <paramref name="onFailure"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when a Result is not initialized, or when <paramref name="onFailure"/> returns a null Task.
    /// <para>Result が未初期化の場合、または <paramref name="onFailure"/> が null の Task を返した場合にスローされる。</para>
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
                throw new InvalidOperationException("OnFailure must not return a null Task.");

            await actionTask.ConfigureAwait(false);
        }

        return result;
    }
}
