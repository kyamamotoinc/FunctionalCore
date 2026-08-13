namespace FunctionalCore.Linq;

/// <summary>
/// Provides LINQ query syntax support for <c>Result&lt;E, T&gt;</c>.
/// <para><c>Result&lt;E, T&gt;</c> を LINQ クエリ構文で使用するための拡張メソッドを提供する。</para>
/// </summary>
/// <remarks>
/// These methods delegate to the corresponding Result operations and preserve their semantics.
/// <para>
/// 各メソッドは対応する Result の操作へ処理を委譲し、その意味と契約を維持する。
/// </para>
///
/// A <c>Where</c> operator is intentionally not provided because filtering a Result requires
/// an explicit error value when the predicate fails.
/// <para>
/// <c>Where</c> は意図的に提供しない。
/// Result では条件違反時に返すエラーを明示する必要があるため、
/// 絞り込みには <c>Ensure</c> を使用する。
/// </para>
/// </remarks>
public static class ResultLinq
{
    /// <summary>
    /// Transforms the success value when the Result is successful.
    /// Supports LINQ query syntax.
    /// <para>
    /// Result が成功している場合に成功値を変換する。
    /// LINQ クエリ構文に対応する。
    /// </para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The type of the original success value.
    /// <para>変換前の成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the transformed success value.
    /// <para>変換後の成功値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The Result to transform.
    /// <para>変換対象の Result。</para>
    /// </param>
    /// <param name="selector">
    /// A function that transforms the success value. Must not return null.
    /// <para>成功値を変換する関数。null を返してはならない。</para>
    /// </param>
    /// <returns>
    /// A successful Result containing the transformed value when <paramref name="result"/> is successful;
    /// otherwise a failed Result containing the original error.
    /// <para>
    /// <paramref name="result"/> が成功している場合は、変換後の値を保持する成功 Result。
    /// 失敗している場合は、元のエラーを保持する失敗 Result。
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
        ArgumentNullException.ThrowIfNull(selector);

        return result.Map(selector);
    }

    /// <summary>
    /// Connects a Result to another Result-producing operation and projects the values into a final Result.
    /// Supports LINQ query syntax with multiple <c>from</c> clauses.
    /// <para>
    /// Result を次の Result を返す処理へ接続し、元の成功値と中間値から最終 Result を生成する。
    /// 複数の <c>from</c> を使用する LINQ クエリ構文に対応する。
    /// </para>
    /// </summary>
    /// <typeparam name="E">
    /// The error type.
    /// <para>エラーの型。</para>
    /// </typeparam>
    /// <typeparam name="T">
    /// The type of the original success value.
    /// <para>元の成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of the intermediate success value.
    /// <para>中間 Result の成功値の型。</para>
    /// </typeparam>
    /// <typeparam name="V">
    /// The type of the projected success value.
    /// <para>最終的に生成される成功値の型。</para>
    /// </typeparam>
    /// <param name="result">
    /// The source Result.
    /// <para>接続元の Result。</para>
    /// </param>
    /// <param name="selector">
    /// A function that transforms the original success value into an intermediate Result.
    /// <para>元の成功値から中間 Result を生成する関数。</para>
    /// </param>
    /// <param name="projector">
    /// A function that combines the original success value and intermediate success value into the final value.
    /// Must not return null.
    /// <para>
    /// 元の成功値と中間の成功値から最終値を生成する関数。
    /// null を返してはならない。
    /// </para>
    /// </param>
    /// <returns>
    /// A successful Result containing the projected value when both the source Result
    /// and the intermediate Result are successful.
    /// If either Result is failed, the corresponding error is propagated.
    /// <para>
    /// 元の Result と中間 Result の両方が成功している場合は、
    /// 投影された値を保持する成功 Result。
    /// いずれかが失敗している場合は、そのエラーを保持する失敗 Result。
    /// </para>
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="selector"/> or <paramref name="projector"/> is null.
    /// <para><paramref name="selector"/> または <paramref name="projector"/> が null の場合にスローされる。</para>
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when <paramref name="result"/> is uninitialized,
    /// when <paramref name="selector"/> returns an uninitialized Result,
    /// or when <paramref name="projector"/> returns null.
    /// <para>
    /// <paramref name="result"/> が未初期化、
    /// <paramref name="selector"/> が未初期化の Result を返した場合、
    /// または <paramref name="projector"/> が null を返した場合にスローされる。
    /// </para>
    /// </exception>
    public static Result<E, V> SelectMany<E, T, U, V>(
        this Result<E, T> result,
        Func<T, Result<E, U>> selector,
        Func<T, U, V> projector)
    {
        ArgumentNullException.ThrowIfNull(selector);
        ArgumentNullException.ThrowIfNull(projector);

        return result.Bind(x => selector(x).Map(y => projector(x, y)));
    }

    // Where is intentionally omitted.
    // Use result.Ensure(predicate, errorFactory) directly,
    // as Result requires an explicit error when the predicate fails.
}
