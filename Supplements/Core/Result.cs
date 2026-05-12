namespace Supplements.Core;

public class Result
{
    public bool IsSuccess { get; }
    public string? Message { get; }

    protected Result(bool isSuccess, string? message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result Success() => new Result(true, null);
    public static Result Failure(string message) => new Result(false, message);
}

public class Result<T>
{
    public bool IsSuccess { get; }
    public string? Message { get; }
    public T? Data { get; }

    protected Result(T? data, bool isSuccess, string? message)
    {
        Data = data;
        IsSuccess = isSuccess;
        Message = message;
    }

    public static Result<T> Success(T data) => new Result<T>(data, true, null);
    public static Result<T> Failure(string message) => new Result<T>(default, false, message);
}
