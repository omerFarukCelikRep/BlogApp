using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Queries;

public class GetByIdQueryHandler(IBlogService blogService) : IRequestHandler<GetByIdQuery, Result<BlogDetailsResult>>
{
    public async Task<Result<BlogDetailsResult>> Handle(GetByIdQuery request,
        CancellationToken cancellationToken = default)
    {
        return await blogService.GetByIdAsync(request, cancellationToken);
    }
}