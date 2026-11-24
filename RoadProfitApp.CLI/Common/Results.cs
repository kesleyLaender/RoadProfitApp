namespace RoadProfitApp.CLI.Common;

// Result
public record Result<T>
{
    public bool IsSuccess => this is Success<T>;
    public bool IsFailed => this is Failure<T>;

    public T GetValue() => this is Success<T>(var value)
        ? value 
        : throw new InvalidOperationException(
            "Não é possível acessar Value de um Result com falha.");

    public Error GetError() => this is Failure<T>(var error)
        ? error
        : throw new InvalidOperationException(
            "Não é possivel acessar Error de um Result com Sucesso.");
    public static Result<T> Ok(T value) => new Success<T>(value);
    public static Result<T> Fail(Error error) => new Failure<T>(error);

    /// <summary>
    /// Converte uma falha não genérica em um resultado tipado,
    /// permitindo retorná-la em métodos que esperam Result<T>.
    /// </summary>
    /// <param name="fail">Falha contendo o erro a ser preservado.</param>
    public static implicit operator Result<T>(Failure fail) => new Failure<T>(fail.Error);

    /// <summary>
    /// Converte um resultado tipado em um resultado não genérico.
    /// Somente resultados com falha podem ser convertidos.
    /// O erro original é preservado, sem descartar valores de sucesso.
    /// </summary>
    /// <param name="result">Resultado tipado a ser convertido.</param>
    /// <returns>Uma falha não genérica contendo o erro original.</returns>
    /// <exception cref="InvalidOperationException">
    /// Lançada quando a conversão é tentada sobre um resultado bem-sucedido.
    /// </exception>
    public static implicit operator Result(Result<T> result)
    {
        if (result is Failure<T>(var error))
            return Result.Fail(error);

        throw new InvalidOperationException(
            "Não é possível converter um Result<T> bem-sucedido em Result.");
    }
}

public sealed record Success<T>(T Value) : Result<T>;
public sealed record Failure<T>(Error Error) : Result<T>;

public abstract record Result
{
    public bool IsSuccess => this is Success;
    public bool IsFailed => this is Failure;

    public Error GetError() => this is Failure(var error)
        ? error
        : throw new InvalidOperationException(
              "Não é possivel acessar Error de um Result com Sucesso.");
    public static Result Ok() => new Success();
    public static Failure Fail(Error error) => new Failure(error);
    public static Result<T> Ok<T>(T value) => new Success<T>(value);

}

public sealed record Success() : Result;
public sealed record Failure(Error Error) : Result;


// Error
public abstract record Error(string Message)
{
    public static Operational Operational(string message)
        => new (message);

    public static Confirmation Confirmation(string message)
        => new (message);

    public static Validation Validation(string message)
        => new (message);
    public static Warning Warning(string message)
        => new (message);

    public static Warning Warning(string message, string innerError)
        => new (message, innerError);

    public static Critical Critical(string message)
        => new (message);

    
}

public sealed record Operational(string Message) : Error(Message);
public sealed record Confirmation(string Message) : Error(Message);
public sealed record Validation(string Message) : Error(Message);
public sealed record Warning(string Message, string? InnerError = null) : Error(Message);
public sealed record Critical(string Message) : Error(Message);


// UiResponse
public abstract record Response(string Message);

public sealed record SuccessResponse(string Message) : Response(Message);
public sealed record FailureResponse(string Message) : Response(Message);
public sealed record WarningResponse(string Message) : Response(Message);
public sealed record ConfirmationResponse(string Message) : Response(Message);
public sealed record AbortResponse(string Message) : Response(Message);

