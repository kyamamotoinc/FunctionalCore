namespace FunctionalCore
{
    public static class ResultExtensions
    {
        public static Result<E, IEnumerable<T>> Sequence<E, T>(this IEnumerable<Result<E, T>> results)
        {
            var lst = new List<T>();
            foreach (var r in results)
            {
                if (r.IsSuccess == false)
                {
                    return Result<E, IEnumerable<T>>.Fail(r.ErrorData);
                }
                lst.Add(r.Value);
            }
            return Result<E, IEnumerable<T>>.Ok(lst);
        }

        public static Result<E, IEnumerable<U>> Traverse<E, T, U>(this IEnumerable<T> items, Func<T, Result<E, U>> f)
        {
            var lst = new List<U>();
            foreach (var item in items)
            {
                var r = f(item);
                if (r.IsSuccess == false)
                {
                    return Result<E, IEnumerable<U>>.Fail(r.ErrorData);
                }
                lst.Add(r.Value);
            }
            return Result<E, IEnumerable<U>>.Ok(lst);
        }
    }
}
