namespace FunctionalCore
{
    public static class OptionExtensions
    {
        public static Option<IEnumerable<T>> Sequence<T>(this IEnumerable<Option<T>> options)
        {
            var lst = new List<T>();
            foreach (var opt in options)
            {
                if (opt.HasValue == false)
                {
                    return Option<IEnumerable<T>>.None();
                }
                lst.Add(opt.Value);
            }
            return Option<IEnumerable<T>>.Some(lst);
        }

        public static Option<IEnumerable<U>> Traverse<T, U>(this IEnumerable<T> items, Func<T, Option<U>> f)
        {
            var lst = new List<U>();
            foreach (var item in items)
            {
                var opt = f(item);
                if (opt.HasValue == false)
                {
                    return Option<IEnumerable<U>>.None();
                }
                lst.Add(opt.Value);
            }
            return Option<IEnumerable<U>>.Some(lst);
        }
    }
}
