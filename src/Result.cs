namespace FunctionalCore
{
    public sealed class Result<E, T>
    {
        public bool IsSuccess { get; }

        private readonly T _value;
        public T Value
        {
            get
            {
                if (IsSuccess == false)
                {
                    throw new InvalidOperationException("No value available");
                }
                return _value;
            }
        }

        private readonly E _errorData;

        public E ErrorData
        {
            get
            {
                if (IsSuccess)
                {
                    throw new InvalidOperationException("No error data available");
                }
                return _errorData;
            }
        }

        private Result(T value)
        {
            IsSuccess = true;
            _errorData = default;
            _value = value;
        }

        private Result(E error)
        {
            IsSuccess = false;
            _errorData = error;
            _value = default;
        }

        public static Result<E, T> Ok(T value) => new Result<E, T>(value);

        public static Result<E, T> Fail(E errorData) => new Result<E, T>(errorData);

        public Result<E, U> Map<U>(Func<T, U> f)
        {
            return IsSuccess ? Result<E, U>.Ok(f(Value)) : Result<E, U>.Fail(ErrorData);
        }

        public Result<E, U> Bind<U>(Func<T, Result<E, U>> f)
        {
            return IsSuccess ? f(Value) : Result<E, U>.Fail(ErrorData);
        }

        #region "For LINQ"

        public Result<E, U> Select<U>(Func<T, U> f)
        {
            return Map(f);
        }

        public Result<E, V> SelectMany<U, V>(Func<T, Result<E, U>> selector, Func<T, U, V> projector)
        {
            return Bind(x => selector(x).Map(y => projector(x, y)));
        }
        #endregion


        public Result<E, T> Tap(Action<Result<E,T>> act)
        {
            if (IsSuccess == false)
            {
                return this;
            }
            act(this);
            return this;
        }

        public Result<E, T> TapError(Action<Result<E, T>> act)
        {
            if (IsSuccess)
            {
                return this;
            }
            act(this);
            return this;
        }

        public Result<E, T> TapAll(Action<Result<E, T>> act)
        {
            act(this);
            return this;
        }

        public U Match<U>(Func<T, U> funcOnSuccess, Func<E, U> funcOnFailure)
        {
            return IsSuccess ? funcOnSuccess(Value) : funcOnFailure(ErrorData);
        }

        public Result<E, U> Combine<R, U>(Result<E, R> other, Func<T, R, U> f)
        {
            if (IsSuccess == false)
            {
                return Result<E, U>.Fail(ErrorData);
            }
            if (other.IsSuccess == false)
            {
                return Result<E, U>.Fail(other.ErrorData);
            }
            return Result<E, U>.Ok(f(Value, other.Value));
        }

        public Result<E, U> Combine<R1, R2, U>(Result<E, R1> other1, Result<E, R2> other2, Func<T, R1, R2, U> f)
        {
            if (IsSuccess == false)
            {
                return Result<E, U>.Fail(ErrorData);
            }
            if (other1.IsSuccess == false)
            {
                return Result<E, U>.Fail(other1.ErrorData);
            }
            if (other2.IsSuccess == false)
            {
                return Result<E, U>.Fail(other2.ErrorData);
            }
            return Result<E, U>.Ok(f(Value, other1.Value, other2.Value));
        }

        public Result<E, U> Combine<R1, R2, R3, U>(Result<E, R1> other1, Result<E, R2> other2, Result<E, R3> other3, Func<T, R1, R2, R3, U> f)
        {
            if (IsSuccess == false)
            {
                return Result<E, U>.Fail(ErrorData);
            }
            if (other1.IsSuccess == false)
            {
                return Result<E, U>.Fail(other1.ErrorData);
            }
            if (other2.IsSuccess == false)
            {
                return Result<E, U>.Fail(other2.ErrorData);
            }
            if (other3.IsSuccess == false)
            {
                return Result<E, U>.Fail(other3.ErrorData);
            }
            return Result<E, U>.Ok(f(Value, other1.Value, other2.Value, other3.Value));
        }
        public T ValueOrDefault(T defaultValue)
        {
            return IsSuccess ? Value : defaultValue;
        }

        public T ValueOrThrow(Func<E, Exception> toException)
        {
            if (IsSuccess)
            {
                return Value;
            }
            throw toException(ErrorData);
        }
    }
}
