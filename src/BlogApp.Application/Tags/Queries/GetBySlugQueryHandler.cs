using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Application.Tags.Queries;

public class GetBySlugQueryHandler(ITagService tagService) : IRequestHandler<GetBySlugQuery, Result<TagResult>>
{
    public async Task<Result<TagResult>> Handle(GetBySlugQuery request, CancellationToken cancellationToken = default)
    {
        return await tagService.GetBySlugAsync(request, cancellationToken);
    }
}