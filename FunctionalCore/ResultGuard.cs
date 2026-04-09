namespace FunctionalCore
{
    internal static class ResultGuard
    {
        internal static void ThrowIfNotInitialized<T, E>(this Result<E, T> result)
        {
            if (!result.IsInitialized)
                throw new InvalidOperationException($"Result<{typeof(E).Name}, {typeof(T).Name}> is not initialized.");
        }
    }
}
