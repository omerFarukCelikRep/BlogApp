using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Authors;
using BlogApp.Domain.Models.Tags;

namespace BlogApp.Domain.Services;

public class AuthorService(IUserRepository userRepository) : IAuthorService
{
    public async Task<Result<AuthorProfileResult>> GetByUsernameAsync(GetByUsernameArgs args,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAuthorProfileAsync(args, cancellationToken);
        if (user is null)
            return Result<AuthorProfileResult>.Failed(404, Error.Create(Errors.User.NotFound));

        var publishedBlogs = user.Blogs
            .Where(x => x.IsPublished())
            .ToList();

        var totalReads = publishedBlogs.Sum(b => b.ReadCount);
        var totalLikes = publishedBlogs.Sum(b => b.Likes.Count);

        var topTags = publishedBlogs
            .SelectMany(b => b.BlogTags)
            .GroupBy(bt => bt.Tag)
            .Select(x => new TagResult(x.Key!.Id, x.Key.Name, x.Key.Slug, x.Count()))
            .OrderByDescending(t => t.BlogCount)
            .Take(8)
            .ToList();

        var userProfile = (AuthorProfileResult)user;
        var result = userProfile with
        {
            BlogCount = publishedBlogs.Count,
            TotalReads = totalReads,
            TotalLikes = totalLikes,
            TopTags = topTags
        };

        return Result<AuthorProfileResult>.Success(data: result);
    }
}