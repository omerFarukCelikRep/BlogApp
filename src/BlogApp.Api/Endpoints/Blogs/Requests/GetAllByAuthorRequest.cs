using BlogApp.Application.Blogs.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs.Requests;

public record GetAllByAuthorRequest(
    [FromRoute] string Username,
    [FromQuery] int? Page,
    [FromQuery] int? PageSize,
    [FromQuery] string? SortBy)
{
    public static explicit operator GetAllByAuthorQuery(GetAllByAuthorRequest request) =>
        new(request.Username, request.Page, request.PageSize, request.SortBy);
}