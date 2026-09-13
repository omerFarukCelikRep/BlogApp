namespace BlogApp.Core.Results;

/// <summary>
/// </summary>
/// <param name="Data"></param>
/// <param name="PageNumber"></param>
/// <param name="PageSize"></param>
/// <param name="TotalCount"></param>
/// <param name="Message"></param>
/// <typeparam name="T"></typeparam>
public sealed record PaginatedResult<T>(
    bool IsSuccess,
    string Message,
    int StatusCode,
    IEnumerable<T>? Data,
    Results.Error? Error,
    int PageNumber,
    int PageSize,
    int TotalCount)
    : Result<IEnumerable<T>>(IsSuccess, Message, StatusCode, Data, Error)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    public static PaginatedResult<T> Success(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount,
        int statusCode = 200, string message = "") =>
        new(true, message, statusCode, data, Results.Error.None, pageNumber, pageSize, totalCount);

    public new static PaginatedResult<T> Failed(int statusCode, string message, IEnumerable<T>? data = null) =>
        new(false, message, statusCode, data, new Error(string.Empty, message), 0, 0, 0);

    public new static PaginatedResult<T> Failed(int statusCode, Error error, IEnumerable<T>? data = null) =>
        new(false, error.ErrorMessage, statusCode, data, error, 0, 0, 0);
}