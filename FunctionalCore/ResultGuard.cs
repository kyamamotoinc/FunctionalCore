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
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="E"></typeparam>
        /// <param name="result"></param>
        /// <exception cref="InvalidOperationException"></exception>
        internal static void ThrowIfNotInitialized<T, E>(this Result<E, T> result)
        {
            if (!result.IsInitialized)
                throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> is not initialized.");
        }
    }
}
