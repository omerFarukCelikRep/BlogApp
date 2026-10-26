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

    public async Task<Result<TagResult>> GetBySlugAsync(GetBySlugArgs args,
        CancellationToken cancellationToken = default)
    {
        var tag = await tagRepository.GetAsync(x => x.Slug.Equals(args.Slug), false, cancellationToken);
        return tag is null
            ? Result<TagResult>.Failed(404, Error.Create(Errors.Tag.NotFound))
            : Result<TagResult>.Success(data: (TagResult)tag);
    }

    public async Task<Result<TagResult>> CreateAsync(CreateTagArgs args, CancellationToken cancellationToken = default)
    {
        var slug = args.Name
            .ToLowerInvariant()
            .Replace(" ", "-")
            .Replace(".", "")
            .Trim();

        var exists = await tagRepository.AnyAsync(t => t.Slug.Equals(slug), cancellationToken);
        if (exists)
            return Result<TagResult>.Failed(400, Error.Create(Errors.Tag.AlreadyExists));

        var tag = new Tag
        {
            Name = args.Name,
            Slug = slug
        };
        await tagRepository.AddAsync(tag, cancellationToken);
        await tagRepository.SaveChangesAsync(cancellationToken);

        return Result<TagResult>.Success(data: (TagResult)tag);
    }

    public async Task<Result> DeleteAsync(DeleteTagArgs args, CancellationToken cancellationToken = default)
    {
        var tag = await tagRepository.GetByIdAsync(args.TagId, tracking: true, cancellationToken);
        if (tag is null)
            return Result.Failed(404, Error.Create(Errors.Tag.NotFound));

        await tagRepository.DeleteAsync(tag, cancellationToken);
        await tagRepository.SaveChangesAsync(cancellationToken);

        return Result.Success(statusCode: 200);
    }

    public async Task<PaginatedResult<TagResult>> GetAllForAdminAsync(GetAllTagsForAdminArgs args,
        CancellationToken cancellationToken = default)
    {
        var comments =
            await tagRepository.GetAllAsPaginateAsync(args.Page!.Value, args.PageSize!.Value, false, cancellationToken);

        var data = comments.Items.Select(x => (TagResult)x);
        return PaginatedResult<TagResult>.Success(data: data, comments.Index, comments.Size, comments.Pages);
    }
}