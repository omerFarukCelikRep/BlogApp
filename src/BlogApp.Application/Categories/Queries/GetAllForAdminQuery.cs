using BlogApp.Core.Security.Attributes;
using BlogApp.Core.Security.Enums;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Application.Categories.Queries;

[Authorize(Role.Admin)]
public record GetAllForAdminQuery(int? Page, int? PageSize)
    : GetAllCategoriesForAdminArgs(Page, PageSize), IRequest<PaginatedResult<CategoryResult>>;