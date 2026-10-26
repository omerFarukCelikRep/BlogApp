using BlogApp.Core.Caching.Markers;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Application.Categories.Queries;

public record GetBySlugQuery(string Slug)
    : GetCategoryBySlugArgs(Slug), IRequest<Result<CategoryDetailsResult>>, ICacheable
{
    public string Key => $"categories:slug:{Slug}";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(15);
}