using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Blogs;

namespace BlogApp.Application.Blogs.Commands;

public class ToggleLikeCommandHandler(IBlogService blogService)
    : IRequestHandler<ToggleLikeCommand, Result<ToggleLikeResult>>
{
    public async Task<Result<ToggleLikeResult>> Handle(ToggleLikeCommand request,
        CancellationToken cancellationToken = default)
    {
        return await blogService.ToggleLikeAsync(request, cancellationToken);
    }
}