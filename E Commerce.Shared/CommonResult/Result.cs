using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Shared.CommonResult
{
    public class Result
    {
        protected readonly List<Error> errors = [];
        public bool IsSuccess => errors.Count == 0; // true
        public bool IsFailure => !IsSuccess; // false
        public IReadOnlyList<Error> Errors => errors;

        // ok result
        protected Result()
        {
        }
        // fail with error
        protected Result(Error error)
        {
            errors.Add(error);
        }
        // fail with errors
        protected Result(List<Error> _errors)
        {
            errors.AddRange(_errors);
        }
        public static Result Ok() => new Result();
        public static Result Fail(Error error) => new Result(error);
        public static Result Fail(List<Error> errors) => new Result(errors);
    }

    public class Result<T> : Result
    {
        private readonly T _value;
        public T Value => IsSuccess ? _value : throw new InvalidOperationException("Can Not Access the Value");
        private Result(T value) : base()
        {
            _value = value;
        }

        private Result(Error error) : base(error)
        {
            _value = default!;
        }
        private Result(List<Error> errors) : base(errors)
        {
            _value = default!;
        }
        public static Result<T> Ok(T value) => new Result<T>(value);
        public static new Result<T> Fail(Error value) => new Result<T>(value);
        public static new Result<T> Fail(List<Error> value) => new Result<T>(value);

        public static implicit operator Result<T>(T value) => Ok(value);

        public static implicit operator Result<T>(Error value) => Fail(value);

        public static implicit operator Result<T>(List<Error> value) => Fail(value);
    }
}
