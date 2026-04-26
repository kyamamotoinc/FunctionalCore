namespace FunctionalCore
{
    /// <summary>
    /// Guard class for inspecting the state of Result<E, T>.
    /// Result<E, T> の状態を検査するためのガードクラス。
    ///
    /// This class is internal and should not be exposed to public API.
    /// このクラスは内部用であり、公開APIに露出させるべきではない。
    /// </summary>
    internal static class ResultGuard
    {
        /// <summary>
        /// Throws if the result is not initialized.
        /// Resultが初期化されていない場合は例外を投げる。
        /// </summary>
        /// <typeparam name="E">The error type. / エラーの型。</typeparam>
        /// <typeparam name="T">The success value type. / 成功時の値の型。</typeparam>
        /// <param name="result">The result to check. / 検査するResult。</param>
        /// <exception cref="InvalidOperationException">Thrown if the result is not initialized. / 初期化されていない場合に投げられる。</exception>
        internal static void ThrowIfNotInitialized<E, T>(this Result<E, T> result)
        {
            if (!result.IsInitialized)
                throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> is not initialized.");
        }
    }
}
