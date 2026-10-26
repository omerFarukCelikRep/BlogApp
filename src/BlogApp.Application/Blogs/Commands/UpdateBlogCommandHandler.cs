using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Commands;

public class UpdateBlogCommandHandler(IBlogService blogService) :
    IRequestHandler<UpdateBlogCommand, Result<BlogDetailsResult>>
{
    public async Task<Result<BlogDetailsResult>> Handle(UpdateBlogCommand request,
        CancellationToken cancellationToken = default)
    {
        return await blogService.UpdateAsync(request, cancellationToken);
    }
}