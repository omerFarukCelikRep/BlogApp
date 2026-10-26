using BlogApp.Application.Blogs.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs.Requests;

public record GetAllByTagRequest(
    [FromRoute] string Slug,
    [FromQuery] int? Page,
    [FromQuery] int? PageSize,
    [FromQuery] string? SortBy)
{
    public static explicit operator GetAllByTagQuery(GetAllByTagRequest request) =>
        new(request.Slug, request.Page, request.PageSize, request.SortBy);
}