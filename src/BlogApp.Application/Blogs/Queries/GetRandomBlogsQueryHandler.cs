using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public class GetRandomBlogsQueryHandler(IBlogService blogService) : IRequestHandler<GetRandomBlogsQuery, Result<List<BlogSummaryResult>>>
{
    public async Task<Result<List<BlogSummaryResult>>> Handle(GetRandomBlogsQuery request, CancellationToken cancellationToken = default)
    {
        return await blogService.GetRandomAsync(request, cancellationToken);
    }
}