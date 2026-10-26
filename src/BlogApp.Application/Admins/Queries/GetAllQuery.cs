using BlogApp.Domain.Models.Admins;

namespace BlogApp.Application.Admins.Queries;

public record GetAllQuery(
    string? Search,
    string? Role,
    bool? IsBanned,
    int? Page,
    int? PageSize) : GetAllAdminsArgs(Search, Role, IsBanned, Page, PageSize),
    IRequest<PaginatedResult<AdminUserResult>>;