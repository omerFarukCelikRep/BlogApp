using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Enums;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Application.Tags.Queries;

[Authorize(Role.Admin)]
public record GetAllForAdminQuery(int? Page, int? PageSize)
    : GetAllTagsForAdminArgs(Page, PageSize), IRequest<PaginatedResult<TagResult>>;