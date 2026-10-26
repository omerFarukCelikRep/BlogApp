using BlogApp.Core.Caching.Markers;
using BlogApp.Domain.Models.Shared;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Application.Tags.Queries;

public record GetBySlugQuery(string Slug) : GetBySlugArgs(Slug), IRequest<Result<TagResult>>, ICacheable
{
    public string Key => $"tags:slug:{Slug}";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(10);
}