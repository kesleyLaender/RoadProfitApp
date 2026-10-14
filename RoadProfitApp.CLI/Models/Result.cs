using System;

namespace RoadProfitApp.CLI.Models
{
    public readonly struct Result<T>
    {
        public T? Value { get; }
        public string Error {  get; }
        public bool IsSuccess {  get; }
        public bool IsFailure {  get; }

        private Result(T value) 
        { 
            Value = value;
            Error = string.Empty;
            IsSuccess = true;
        }
        private Result(string error) 
        {
            Value = default;
            Error = error;
            IsSuccess = false;
        }

        public static Result<T> Success(T value) => new (value);
        public static Result<T> Failure(string error) => new (error);
    }
}
