using BlogApp.Core.DataAccess.Models;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public class GetAllByTagQueryHandler(IBlogService blogService)
    : IRequestHandler<GetAllByTagQuery, PaginatedResult<BlogSummaryResult>>
{
    public async Task<PaginatedResult<BlogSummaryResult>> Handle(GetAllByTagQuery request,
        CancellationToken cancellationToken = default)
    {
        return await blogService.GetAllByTagAsync(request, cancellationToken);
    }
}