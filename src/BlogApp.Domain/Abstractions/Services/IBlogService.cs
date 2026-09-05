using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Domain.Abstractions.Services;

public interface IBlogService
{
    Task<Result<List<BlogSummaryResult>>> GetTopReadAsync(TopReadBlogArgs args,
        CancellationToken cancellationToken = default);

    Task<Result<List<BlogSummaryResult>>> GetRandomAsync(RandomBlogArgs args,
        CancellationToken cancellationToken = default);
}