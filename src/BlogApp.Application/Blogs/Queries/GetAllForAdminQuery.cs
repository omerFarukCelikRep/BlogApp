using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Enums;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

[Authorize(Role.Admin)]
public record GetAllForAdminQuery(
    string? Search,
    PostStatus? Status,
    string? AuthorName,
    int? Page,
    int? PageSize) : GetAllForAdminsArgs(Search, Status, AuthorName, Page, PageSize),
    IRequest<PaginatedResult<AdminBlogSummaryResult>>;