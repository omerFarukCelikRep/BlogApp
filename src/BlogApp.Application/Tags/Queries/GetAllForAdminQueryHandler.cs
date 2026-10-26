using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Application.Tags.Queries;

public class GetAllForAdminQueryHandler(ITagService tagService) : IRequestHandler<GetAllForAdminQuery, PaginatedResult<TagResult>>
{
    public async Task<PaginatedResult<TagResult>> Handle(GetAllForAdminQuery request, CancellationToken cancellationToken = default)
    {
        return await tagService.GetAllForAdminAsync(request, cancellationToken);
    }
}