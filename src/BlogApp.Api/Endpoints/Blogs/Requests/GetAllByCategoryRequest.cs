using BlogApp.Application.Blogs.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs.Requests;

public record GetAllByCategoryRequest(
    [FromRoute] string Slug,
    [FromQuery] int? Page,
    [FromQuery] int? PageSize,
    [FromQuery] string? SortBy,
    [FromQuery] bool? IncludeChildren)
{
    public static explicit operator GetAllByCategoryQuery(GetAllByCategoryRequest request) =>
        new(request.Slug, request.Page, request.PageSize, request.SortBy,request.IncludeChildren ?? false);
}