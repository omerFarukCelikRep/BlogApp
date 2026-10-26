using BlogApp.Application.Admins.Queries;
using Microsoft.AspNetCore.Mvc;

namespace BlogApp.Api.Endpoints.Admins.Requests;

public record GetAllRequest(
    [FromQuery] string? Search,
    [FromQuery] string? Role,
    [FromQuery] bool? IsBanned,
    [FromQuery] int? Page,
    [FromQuery] int? PageSize)
{
    public static explicit operator GetAllQuery(GetAllRequest request) => new(request.Search, request.Role,
        request.IsBanned, request.Page, request.PageSize);
}