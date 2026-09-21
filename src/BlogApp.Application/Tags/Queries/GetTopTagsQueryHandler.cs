using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Application.Tags.Queries;

public class GetTopTagsQueryHandler(ITagService tagService) : IRequestHandler<GetTopTagsQuery, Result<List<TagResult>>>
{
    public async Task<Result<List<TagResult>>> Handle(GetTopTagsQuery request, CancellationToken cancellationToken = default)
    {
        return await tagService.GetTopAsync(request, cancellationToken);
    }
}