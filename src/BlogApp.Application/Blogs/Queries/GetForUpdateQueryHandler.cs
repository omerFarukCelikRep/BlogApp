using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public class GetForUpdateQueryHandler(IBlogService blogService) : IRequestHandler<GetForUpdateQuery, Result<BlogForUpdateResult>>
{
    public async Task<Result<BlogForUpdateResult>> Handle(GetForUpdateQuery request, CancellationToken cancellationToken = default)
    {
        return await blogService.GetForUpdateAsync(request, cancellationToken);
    }
}