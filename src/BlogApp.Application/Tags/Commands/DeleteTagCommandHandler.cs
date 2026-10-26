using BlogApp.Domain.Abstractions.Services;

namespace BlogApp.Application.Tags.Commands;

public class DeleteTagCommandHandler(ITagService tagService) : IRequestHandler<DeleteTagCommand, Result>
{
    public async Task<Result> Handle(DeleteTagCommand request, CancellationToken cancellationToken = default)
    {
        return await tagService.DeleteAsync(request, cancellationToken);
    }
}