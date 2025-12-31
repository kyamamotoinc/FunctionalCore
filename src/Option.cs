using System;

namespace FunctionalCore
{
    namespace FunctionalCore
    {
        public sealed class Option<T>
        {
            public bool HasValue { get; }

            private readonly T _value;
            public T Value
            {
                get
                {
                    if (HasValue == false)
                    {
                        throw new InvalidOperationException("Option has no value");
                    }
                    return _value;
                }
            }

            private Option(T value)
            {
                HasValue = true;
                _value = value;
            }

            private Option()
            {
                HasValue = false;
                _value = default;
            }

            public static Option<T> Some(T value) => new Option<T>(value);

            public static Option<T> None() => new Option<T>();

            public Option<U> Map<U>(Func<T, U> f)
            {
                return HasValue ? Option<U>.Some(f(Value)) : Option<U>.None();
            }

            public Option<U> Bind<U>(Func<T, Option<U>> f)
            {
                return HasValue ? f(Value) : Option<U>.None();
            }

            #region "For LINQ"
            public Option<U> Select<U>(Func<T, U> f)
            {
                return Map(f);
            }

            public Option<V> SelectMany<U, V>(Func<T, Option<U>> selector, Func<T, U, V> projector)
            {
                return Bind(x => selector(x).Map(y => projector(x, y)));
            }
            #endregion

            public Option<T> Tap(Action<Option<T>> act)
            {
                if (HasValue == false)
                {
                    return this;
                }
                act(this);
                return this;
            }

            public Option<T> TapError(Action<Option<T>> act)
            {
                if (HasValue)
                {
                    return this;
                }
                act(this);
                return this;
            }

            public Option<T> TapEither(Action<Option<T>> act)
            {
                act(this);
                return this;
            }

            public U Match<U>(Func<T, U> funcOnSuccess, Func<U> funcOnFailure)
            {
                return HasValue ? funcOnSuccess(Value) : funcOnFailure();
            }

            public Option<U> Combine<R, U>(Option<R> other, Func<T, R, U> f)
            {
                if (HasValue == false)
                {
                    return Option<U>.None();
                }
                if (other.HasValue == false)
                {
                    return Option<U>.None();
                }
                return Option<U>.Some(f(Value, other.Value));
            }

            public T ValueOrDefault(T defaultValue)
            {
                return HasValue ? Value : defaultValue;
            }

            public T ValueOrThrow(Func<Exception> toException)
            {
                if (HasValue)
                {
                    return Value;
                }
                throw toException();
            }
        }
    }
}
