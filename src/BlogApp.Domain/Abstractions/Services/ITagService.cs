using BlogApp.Core.Results;
using BlogApp.Domain.Models.Shared;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Domain.Abstractions.Services;

public interface ITagService
{
    Task<Result<List<TagResult>>> GetTopAsync(TopTagArgs args, CancellationToken cancellationToken = default);
}