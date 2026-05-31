using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Core.Results;

namespace BlogApp.Api.Extensions;

public static class ResultExtensions
{
    public static Response ToResponse(this Result result) => new(result);
    public static Response<T> ToResponse<T>(this Result<T> result) => new(result);
}