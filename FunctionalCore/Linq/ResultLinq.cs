namespace FunctionalCore.Linq;

/// <summary>
/// Provides LINQ query syntax support for Result&lt;E, T&gt;.
/// <para>Result&lt;E, T&gt; に LINQ クエリ構文を提供する。</para>
/// </summary>
public static class ResultLinq
{
    /// <summary>
    /// Projects the success value of a Result into a new form.
    /// <para>Result の成功値を新しい形式に変換する。</para>
    ///
    /// This method enables LINQ Select syntax.
    /// <para>LINQ の Select 構文を有効にする。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The source success value type.
    /// <para>元の成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The projected success value type.
    /// <para>変換後の成功値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The source Result.
    /// <para>元の Result。</para>
    /// </param>
    /// <param name="selector">
    /// A function that transforms the success value. Must not return null.
    /// <para>成功値を変換する関数。null を返してはならない。</para>
    /// </param>
    /// <returns>
    /// A Result containing the projected value when successful,
    /// or the original error when failed.
    /// <para>
    /// 成功時は変換後の値を保持する Result。
    /// 失敗時は元のエラーを保持する Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> is null.
    /// <para><paramref name="selector"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is uninitialized,
    /// or when <paramref name="selector"/> returns null.
    /// <para>
    /// <paramref name="result"/> が未初期化、
    /// または <paramref name="selector"/> が null を返した場合にスローされる。
    /// </para>
    /// </exception>
    public static Result<E, U> Select<E, T, U>(this Result<E, T> result, Func<T, U> selector)
    {
        return result.Map(selector);
    }

    /// <summary>
    /// Projects the success value into another Result and combines both values into a final result.
    /// <para>成功値を別の Result に変換し、両方の値から最終的な結果を生成する。</para>
    ///
    /// This method enables LINQ SelectMany syntax.
    /// <para>LINQ の SelectMany 構文を有効にする。</para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The source success value type.
    /// <para>元の成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The intermediate success value type.
    /// <para>中間の成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="V">
    /// The final success value type.
    /// <para>最終的な成功値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The source Result.
    /// <para>元の Result。</para>
    /// </param>
    /// <param name="selector">
    /// A function that transforms the success value into another Result.
    /// <para>成功値を別の Result に変換する関数。</para>
    /// </param>
    /// <param name="projector">
    /// A function that combines the original and intermediate success values.
    /// Must not return null.
    /// <para>
    /// 元の成功値と中間の成功値を組み合わせる関数。
    /// null を返してはならない。
    /// </para>
    /// </param>
    /// <returns>
    /// A Result containing the projected value when all operations succeed.
    /// If either Result is failed, the corresponding error is propagated.
    /// <para>
    /// すべての処理が成功した場合は最終的な値を保持する Result。
    /// 途中で失敗した場合は、そのエラーを引き継ぐ。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> or <paramref name="projector"/> is null.
    /// <para>
    /// <paramref name="selector"/> または <paramref name="projector"/> が null の場合にスローされる。
    /// </para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is uninitialized,
    /// when <paramref name="selector"/> returns an uninitialized Result,
    /// or when <paramref name="projector"/> returns null.
    /// <para>
    /// <paramref name="result"/> が未初期化、
    /// <paramref name="selector"/> が未初期化の Result を返した、
    /// または <paramref name="projector"/> が null を返した場合にスローされる。
    /// </para>
    /// </exception>
    public static Result<E, V> SelectMany<E, T, U, V>(this Result<E, T> result, Func<T, Result<E, U>> selector, Func<T, U, V> projector)
    {
        result.ThrowIfNotInitialized();
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(projector);

        return result.Bind(t => selector(t).Map(u => projector(t, u)));
    }
}
