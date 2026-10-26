using BlogApp.Application.Comments.Queries;

namespace BlogApp.Api.Endpoints.Comments.Requests;

public record GetAllForAdminRequest(
    int? Page,
    int? PageSize)
{
    public static explicit operator GetAllForAdminQuery(GetAllForAdminRequest request) =>
        new(request.Page, request.PageSize);
}