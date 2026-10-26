using BlogApp.Core.Results;
using BlogApp.Domain.Models.Shared;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Domain.Abstractions.Services;

public interface ITagService
{
    Task<Result<List<TagResult>>> GetTopAsync(TopTagArgs args, CancellationToken cancellationToken = default);
    Task<Result<TagResult>> GetBySlugAsync(GetBySlugArgs args, CancellationToken cancellationToken = default);
    Task<Result<TagResult>> CreateAsync(CreateTagArgs args, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(DeleteTagArgs args, CancellationToken cancellationToken = default);
    Task<PaginatedResult<TagResult>> GetAllForAdminAsync(GetAllTagsForAdminArgs args,
        CancellationToken cancellationToken = default);
}