namespace BlogApp.Core.Results;

public record Result(bool IsSuccess, string Message, int StatusCode, Error Error)
{
    public static Result Success(string message = "", int statusCode = 200) =>
        new(true, message, statusCode, Error.None);

    public static Result Failed(string message, int statusCode) =>
        new(false, message, statusCode, new Error(string.Empty, message));

    public static Result Failed(string message, int statusCode, Error error) =>
        new(false, message, statusCode, error);
}

public record Result<T>(T? Data, bool IsSuccess, string Message, int StatusCode, Error Error)
    : Result(IsSuccess, Message, StatusCode, Error)
{
    public static Result<T> Success(T? data, string message = "", int statusCode = 200) =>
        new(data, true, string.Empty, statusCode, Error.None);

    public static Result<T> Failed(T? data, string message, int statusCode) =>
        new(data, false, message, statusCode, new Error(string.Empty, message));

    public static Result<T> Failed(T? data, string message, int statusCode, Error error) =>
        new(data, false, message, statusCode, error);
}