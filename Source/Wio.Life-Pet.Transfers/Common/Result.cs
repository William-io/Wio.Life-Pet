using System.Diagnostics.CodeAnalysis;

namespace Wio.Life_Pet.Transfer.Common;

public class Result
{
    public Result(bool isSuccess, Error error, string message = "")
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException();
            
        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException();
            
        IsSuccess = isSuccess;
        Error = error;
        Message = message;
    }
    
    public bool IsSuccess { get; }
    
    public bool IsFailure => !IsSuccess;
    
    public Error Error { get; }
    
    public string Message { get; }
    
    public static Result Success(string message = "") => new(true, Error.None, message);
    
    public static Result Failure(Error error, string message = "") => new(false, error, message);
    
    public static Result<TValue> Success<TValue>(TValue value, string message = "") => new(value, true, Error.None, message);
    
    public static Result<TValue> Failure<TValue>(Error error, string message = "") => new(default, false, error, message);
    
    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}
    
public sealed class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error error, string message = "")
        : base(isSuccess, error, message)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException($"{Error.Name} - {Message}");

    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}

public record Error(string Code, string Name)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Valor nulo foi fornecido");
}