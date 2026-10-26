using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Blogs.Commands;

public class IncrementReadCountCommandHandler(IBlogService blogService)
    : IRequestHandler<IncrementReadCountCommand, Result>
{
    public async Task<Result> Handle(IncrementReadCountCommand request, CancellationToken cancellationToken = default)
    {
        return await blogService.IncrementReadCountAsync(request, cancellationToken);
    }
}