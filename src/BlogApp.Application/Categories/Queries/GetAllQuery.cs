using BlogApp.Core.Caching.Markers;
using BlogApp.Domain.Models.Categories;

namespace BlogApp.Application.Categories.Queries;

public record GetAllQuery : IRequest<Result<List<CategoryDetailsResult>>>, ICacheable
{
    public string Key => "categories:all";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(30);
}