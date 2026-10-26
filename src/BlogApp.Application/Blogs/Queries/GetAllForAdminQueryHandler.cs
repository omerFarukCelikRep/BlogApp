using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public class GetAllForAdminQueryHandler(IBlogService blogService)
    : IRequestHandler<GetAllForAdminQuery, PaginatedResult<AdminBlogSummaryResult>>
{
    public async Task<PaginatedResult<AdminBlogSummaryResult>> Handle(GetAllForAdminQuery request,
        CancellationToken cancellationToken = default)
    {
        return await blogService.GetAllForAdminAsync(request, cancellationToken);
    }
}