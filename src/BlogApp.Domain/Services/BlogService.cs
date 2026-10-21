using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Core.Telemetry.Abstractions;
using BlogApp.Domain.Abstractions.Repositories;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Blogs;
using BlogApp.Domain.Utils;

namespace BlogApp.Domain.Services;

public class BlogService(
    IBlogRepository blogRepository,
    ICategoryRepository categoryRepository,
    ITagRepository tagRepository,
    ILikeRepository likeRepository,
    IUserRepository userRepository,
    ITelemetryService telemetryService,
    IDomainPrincipal domainPrincipal) : IBlogService
{
    private async Task<string> CreateSlug(string title, CancellationToken cancellationToken = default)
    {
        var slug = BlogHelper.GenerateSlug(title);
        var slugExists = await blogRepository.AnyAsync(x => x.Slug.Equals(slug), cancellationToken);
        if (slugExists)
            slug = $"{slug}-{Guid.CreateVersion7().ToString()[..8]}";
        return slug;
    }

    public async Task<Result<List<BlogSummaryResult>>> GetTopReadAsync(TopReadBlogArgs args,
        CancellationToken cancellationToken = default)
    {
        var blogs = await blogRepository.GetTopReadAsync(args.Count, cancellationToken);

        List<BlogSummaryResult> result = [.. blogs.Select(x => (BlogSummaryResult)x)];
        return Result<List<BlogSummaryResult>>.Success(data: result);
    }

    public async Task<Result<List<BlogSummaryResult>>> GetRandomAsync(RandomBlogArgs args,
        CancellationToken cancellationToken = default)
    {
        var blogs = await blogRepository.GetRandomAsync(args, cancellationToken);

        List<BlogSummaryResult> result = [.. blogs.Select(x => (BlogSummaryResult)x)];
        return Result<List<BlogSummaryResult>>.Success(data: result);
    }

    public async Task<Result<CreateBlogResult>> CreateAsync(CreateBlogArgs args,
        CancellationToken cancellationToken = default)
    {
        List<Category> categories =
        [
            ..
            await categoryRepository.GetAllAsync(x => args.Categories.Contains(x.Id), false, cancellationToken)
        ];
        if (!categories.Count.Equals(args.Categories.Count))
            return Result<CreateBlogResult>.Failed(404, Error.Create(Errors.Category.NotFound));

        List<Tag> tags = [.. await tagRepository.GetAllAsync(x => args.Tags.Contains(x.Id), false, cancellationToken)];
        if (!tags.Count.Equals(args.Tags.Count))
            return Result<CreateBlogResult>.Failed(404, Error.Create(Errors.Tag.NotFound));

        var slug = await CreateSlug(args.Title, cancellationToken);

        var blog = new Blog
        {
            Title = args.Title,
            Content = args.Content,
            Thumbnail = args.Thumbnail,
            Slug = slug,
            PostStatus = PostStatus.Draft,
            AuthorId = domainPrincipal.UserId
        };

        blog.InitializeReadingTime();
        foreach (var category in categories)
        {
            blog.BlogCategories.Add(new BlogCategory()
            {
                Category = category,
            });
        }

        foreach (var tag in tags)
        {
            blog.BlogTags.Add(new BlogTag()
            {
                Tag = tag
            });
        }

        blog = await blogRepository.AddAsync(blog, cancellationToken);
        await blogRepository.SaveChangesAsync(cancellationToken);

        telemetryService.RecordBlogCreate(nameof(PostStatus.Draft));

        return Result<CreateBlogResult>.Success(data: (CreateBlogResult)blog);
    }
}