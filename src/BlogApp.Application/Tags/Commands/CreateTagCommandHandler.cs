using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Application.Tags.Commands;

public class CreateTagCommandHandler(ITagService tagService) : IRequestHandler<CreateTagCommand,Result<TagResult>>
{
    public async Task<Result<TagResult>> Handle(CreateTagCommand request, CancellationToken cancellationToken = default)
    {
        return await tagService.CreateAsync(request, cancellationToken);
    }
}