namespace BlogApp.Core.Results;

public sealed record PaginatedResult<T>(
    IEnumerable<T> Data,
    int PageNumber,
    int PageSize,
    int TotalCount,
    string Message = "")
    : Result<IEnumerable<T>>(Data, true, Message, 200, Results.Error.None)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}