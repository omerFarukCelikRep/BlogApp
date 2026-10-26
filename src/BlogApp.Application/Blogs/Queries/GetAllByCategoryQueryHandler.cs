using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public class GetAllByCategoryQueryHandler(IBlogService blogService)
    : IRequestHandler<GetAllByCategoryQuery, PaginatedResult<BlogSummaryResult>>
{
    public async Task<PaginatedResult<BlogSummaryResult>> Handle(GetAllByCategoryQuery request,
        CancellationToken cancellationToken = default)
    {
        return await blogService.GetAllByCategoryAsync(request, cancellationToken);
    }
}