using BlogApp.Core.Caching.Markers;
using BlogApp.Domain.Models.Comments;

namespace BlogApp.Application.Comments.Queries;

public record GetBlogCommentsQuery(int BlogId)
    : GetBlogCommentsArgs(BlogId), IRequest<Result<List<CommentResult>>>, ICacheable
{
    public string Key => $"comments:blog:{BlogId}";
    public TimeSpan? Expiry => TimeSpan.FromMinutes(2);
}