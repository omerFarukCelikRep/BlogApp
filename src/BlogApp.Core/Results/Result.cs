namespace BlogApp.Core.Results;

public record Result(bool IsSuccess, string Message, int StatusCode, Error? Error = null)
{
    public static Result Success(int statusCode = 200,string message = "") =>
        new(true, message, statusCode, Error.None);

    public static Result Failed(int statusCode, string message) =>
        new(false, message, statusCode, new Error(string.Empty, message));

    public static Result Failed(int statusCode, Error error) =>
        new(false, error.ErrorMessage, statusCode, error);
}

public record Result<T>(bool IsSuccess, string Message, int StatusCode, T? Data = default, Error? Error = null)
    : Result(IsSuccess, Message, StatusCode, Error)
{
    public static Result<T> Success(int statusCode = 200, string message = "", T? data = default) =>
        new(true, message, statusCode, data);

    public static Result<T> Failed(int statusCode, string message, T? data = default) =>
        new(false, message, statusCode, data, new Error(string.Empty, message));

    public static Result<T> Failed(int statusCode, Error error,T? data = default) =>
        new(false, error.ErrorMessage, statusCode, data, error);
}