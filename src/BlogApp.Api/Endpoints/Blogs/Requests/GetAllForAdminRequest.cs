using BlogApp.Application.Blogs.Queries;
using BlogApp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Blogs.Requests;

public record GetAllForAdminRequest(
    [FromQuery] string? Search,
    [FromQuery] PostStatus? Status,
    [FromQuery] string? AuthorName,
    [FromQuery] int? Page,
    [FromQuery] int? PageSize)
{
    public static explicit operator GetAllForAdminQuery(GetAllForAdminRequest request) => new(request.Search,
        request.Status, request.AuthorName, request.Page, request.PageSize);
}