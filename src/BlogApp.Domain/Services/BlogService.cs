using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Blogs;
using BlogApp.Domain.Utils;

namespace BlogApp.Domain.Services;

public class BlogService(
    IBlogRepository blogRepository,
    IDomainPrincipal domainPrincipal) : IBlogService
{
    public async Task<Result<List<BlogSummaryResult>>> GetTopReadAsync(TopReadBlogArgs args,
        CancellationToken cancellationToken = default)
    {
        var blogs = await blogRepository.GetTopReadAsync(args.Count, cancellationToken);

        List<BlogSummaryResult> result = [.. blogs.Select(x => (BlogSummaryResult)x)];
        return Result<List<BlogSummaryResult>>.Success(data: result);
    }

    public async Task<Result<List<BlogSummaryResult>>> GetRandomAsync(RandomBlogArgs args,
        CancellationToken cancellationToken = default)
    {
        var blogs = await blogRepository.GetRandomAsync(args, cancellationToken);

        List<BlogSummaryResult> result = [.. blogs.Select(x => (BlogSummaryResult)x)];
        return Result<List<BlogSummaryResult>>.Success(data: result);
    }
}