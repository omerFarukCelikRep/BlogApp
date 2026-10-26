using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public class GetAllByAuthorQueryHandler(IBlogService blogService)
    : IRequestHandler<GetAllByAuthorQuery, PaginatedResult<BlogSummaryResult>>
{
    public async Task<PaginatedResult<BlogSummaryResult>> Handle(GetAllByAuthorQuery request,
        CancellationToken cancellationToken = default)
    {
        return await blogService.GetAllByAuthorAsync(request, cancellationToken);
    }
}