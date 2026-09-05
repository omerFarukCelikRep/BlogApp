using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public class GetTopReadBlogsQueryHandler(IBlogService blogService) : IRequestHandler<GetTopReadBlogsQuery, Result<List<BlogSummaryResult>>>
{
    public async Task<Result<List<BlogSummaryResult>>> Handle(GetTopReadBlogsQuery request, CancellationToken cancellationToken = default)
    {
        return await blogService.GetTopReadAsync(request, cancellationToken);
    }
}