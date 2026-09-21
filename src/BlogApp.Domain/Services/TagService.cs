using System.Data.Common;
using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Shared;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Domain.Services;

public class TagService(ITagRepository tagRepository) : ITagService
{
    public async Task<Result<List<TagResult>>> GetTopAsync(TopTagArgs args,
        CancellationToken cancellationToken = default)
    {
        var tags = await tagRepository.GetTopAsync(args.Count, cancellationToken);

        List<TagResult> result = [.. tags.Select(x => (TagResult)x)];
        return Result<List<TagResult>>.Success(data: result);
    }
}