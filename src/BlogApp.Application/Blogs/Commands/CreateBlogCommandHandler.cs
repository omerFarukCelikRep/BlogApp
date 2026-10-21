using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Commands;

public class CreateBlogCommandHandler(IBlogService blogService)
    : IRequestHandler<CreateBlogCommand, Result<CreateBlogResult>>
{
    public async Task<Result<CreateBlogResult>> Handle(CreateBlogCommand request,
        CancellationToken cancellationToken = default)
    {
        return await blogService.CreateAsync(request, cancellationToken);
    }
}