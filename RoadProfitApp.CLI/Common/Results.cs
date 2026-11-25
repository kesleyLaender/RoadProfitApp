namespace RoadProfitApp.CLI.Common;

public readonly record struct Success;

// ErrorOr
public readonly record struct ErrorOr<TValue>
{
    private readonly TValue? _value;
    private readonly Error _error;

    public bool IsSuccess { get; }
    public bool IsFail => !IsSuccess;

    private ErrorOr(TValue value)
    {
        _value = value;
        _error = default;
        IsSuccess = true;
    }

    private ErrorOr(Error error)
    {
        _value = default;
        _error = error;
        IsSuccess = false;
    }

    public TValue Value => IsSuccess
        ? _value
        : throw new InvalidOperationException(
            "Não é possível acessar Value de um ErrorOr com falha.");

    public Error Errors => IsFail
        ? _error
        : throw new InvalidOperationException(
            "Não é possivel acessar Error de um ErrorOr com Sucesso.");

    public static implicit operator ErrorOr<TValue>(TValue value) => new (value);
    public static implicit operator ErrorOr<TValue>(Error error) => new(error);

    public static ErrorOr<TValue> Ok(TValue value) => new (value);
    public static ErrorOr<TValue> Fail(Error error) => new (error);

    public void Switch(Action<TValue> onSuccess, Action<Error> onError)
    {
        if(IsFail)
        {
            onError(_error);
            return;
        }

        onSuccess(_value!);
    }

    public TResult Match<TResult>(Func<TValue, TResult> onSuccess, Func<Error, TResult> onError)
        => IsSuccess ? onSuccess(_value!) : onError(_error);

}


// Error
public readonly record struct Error(
    ErrorType Type,
    string Message,
    string? InnerError = null)
{
    public static Error Failure(string message)
        => new (ErrorType.Failure, message);

    public static Error Confirmation(string message)
        => new (ErrorType.Confirmation, message);

    public static Error Validation(string message)
        => new (ErrorType.Validation, message);

    public static Error Warning(string message)
        => new (ErrorType.Warning, message);

    public static Error Warning(string message, string innerError)
        => new (ErrorType.Warning, message, innerError);

    public static Error NotFound(string message)
        => new (ErrorType.NotFound, message);

    
}

public enum ErrorType
{
    Failure,
    Validation,
    NotFound,
    Warning,
    Confirmation
}

