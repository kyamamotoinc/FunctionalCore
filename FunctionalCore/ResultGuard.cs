namespace FunctionalCore;

/// <summary>
/// Provides internal guard operations for <c>Result&lt;E, T&gt;</c>.
/// <para><c>Result&lt;E, T&gt;</c> の内部状態を検証するためのガード処理を提供する。</para>
/// </summary>
/// <remarks>
/// This type is for internal use by FunctionalCore and is not part of the public API.
/// <para>
/// FunctionalCore 内部で使用するための型であり、公開 API の一部ではない。
/// </para>
/// </remarks>
internal static class ResultGuard
{
    /// <summary>
    /// Ensures that the specified Result is initialized.
    /// <para>指定された Result が初期化済みであることを検証する。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The success value type.
    /// <para>成功時の値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The Result to validate.
    /// <para>検証する Result。</para>
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is not initialized.
    /// <para><paramref name="result"/> が未初期化の場合にスローされる。</para>
    /// </exception>
    internal static void ThrowIfNotInitialized<E, T>(this Result<E, T> result)
    {
        if (!result.IsInitialized)
            throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> is not initialized.");
    }
}
