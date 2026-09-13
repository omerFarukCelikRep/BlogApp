using BlogApp.Application.Search;
using BlogApp.Core.Security.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Search.Requests;

public record SearchRequest(
    [FromQuery] [Sanitize] string Q,
    [FromQuery] string? Type,
    [FromQuery] string? Tag,
    [FromQuery] string? Author,
    [FromQuery] string? DateFrom,
    [FromQuery] string? DateTo,
    [FromQuery] string? SortBy,
    [FromQuery] int? Page,
    [FromQuery] int? PageSize)
{
    public static explicit operator SearchQuery(SearchRequest request) => new(request.Q,
        request.Type ?? "all",
        request.Tag,
        request.Author,
        request.DateFrom,
        request.DateTo,
        request.SortBy ?? "relevance",
        request.Page ?? 1,
        request.PageSize ?? 10);
}