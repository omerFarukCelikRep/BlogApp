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

    public async Task<Result<BlogDetailsResult>> GetByIdAsync(GetBlogByIdArgs args,
        CancellationToken cancellationToken = default)
    {
        var blog = await blogRepository.GetDetailsAsync(args.Id, cancellationToken);
        if (blog is null)
            return Result<BlogDetailsResult>.Failed(404, Error.Create(Errors.Blog.NotFound));

        if (blog.PostStatus != PostStatus.Published && blog.Author!.Id != domainPrincipal.UserId)
            return Result<BlogDetailsResult>.Failed(403, Error.Create(Errors.Blog.UnauthorizeUser));

        var isLiked = domainPrincipal.IsAuthenticated && blog.Likes.Any(x => x.UserId == domainPrincipal.UserId);

        var result = (BlogDetailsResult)blog with { IsLiked = isLiked };
        return Result<BlogDetailsResult>.Success(data: result);
    }

    public async Task<Result> IncrementReadCountAsync(IncrementReadCountArgs args,
        CancellationToken cancellationToken = default)
    {
        var blog = await blogRepository.GetByIdAsync(args.Id, cancellationToken: cancellationToken);
        if (blog is null)
            return Result.Failed(404, Error.Create(Errors.Blog.NotFound));

        blog.IncrementReadCount();

        await blogRepository.SaveChangesAsync(cancellationToken);

        telemetryService.RecordBlogView(blog.Id, blog.Title);

        return Result.Success();
    }

    public async Task<Result<ToggleLikeResult>> ToggleLikeAsync(ToggleLikeArgs args,
        CancellationToken cancellationToken = default)
    {
        var blog = await blogRepository.GetByIdAsync(args.Id, cancellationToken: cancellationToken);
        if (blog is null)
            return Result<ToggleLikeResult>.Failed(404, Error.Create(Errors.Blog.NotFound));

        var userId = domainPrincipal.UserId;
        var existingLike =
            await likeRepository.GetAsync(x => x.BlogId == args.Id && x.UserId == userId,
                cancellationToken: cancellationToken);
        bool isLiked;
        if (existingLike is null)
        {
            await likeRepository.AddAsync(new Like()
            {
                BlogId = args.Id,
                UserId = userId
            }, cancellationToken);
            isLiked = true;
        }
        else
        {
            await likeRepository.DeleteAsync(existingLike, cancellationToken);
            isLiked = false;
        }

        await likeRepository.SaveChangesAsync(cancellationToken);

        telemetryService.RecordBlogLike(blog.Id, isLiked);

        var likeCount = await likeRepository.CountAsync(x => x.BlogId == args.Id, cancellationToken);
        return Result<ToggleLikeResult>.Success(data: new(isLiked, likeCount));
    }

    public async Task<Result<BlogForUpdateResult>> GetForUpdateAsync(GetBlogByIdArgs args,
        CancellationToken cancellationToken = default)
    {
        var blog = await blogRepository.GetDetailsAsync(args.Id, cancellationToken);
        if (blog is null)
            return Result<BlogForUpdateResult>.Failed(404, Error.Create(Errors.Blog.NotFound));

        return !blog.IsOwnedBy(domainPrincipal.UserId)
            ? Result<BlogForUpdateResult>.Failed(403, Error.Create(Errors.Blog.UnauthorizeUser))
            : Result<BlogForUpdateResult>.Success(data: (BlogForUpdateResult)blog);
    }

    public async Task<Result<BlogDetailsResult>> UpdateAsync(UpdateBlogArgs args,
        CancellationToken cancellationToken = default)
    {
        var blog = await blogRepository.GetDetailsAsync(args.Id, cancellationToken: cancellationToken);
        if (blog is null)
            return Result<BlogDetailsResult>.Failed(404, Error.Create(Errors.Blog.NotFound));

        if (!blog.IsOwnedBy(domainPrincipal.UserId))
            return Result<BlogDetailsResult>.Failed(403, Error.Create(Errors.Blog.UnauthorizeUser));

        var categories =
            await categoryRepository.GetAllAsync(x => args.Categories.Contains(x.Id), false, cancellationToken);
        if (categories.Count() != args.Tags.Distinct().Count())
            return Result<BlogDetailsResult>.Failed(404, Error.Create(Errors.Category.NotFound));

        var tags = await tagRepository.GetAllAsync(x => args.Tags.Contains(x.Id), false, cancellationToken);
        if (tags.Count() != args.Tags.Distinct().Count())
            return Result<BlogDetailsResult>.Failed(404, Error.Create(Errors.Tag.NotFound));

        blog.UpdateContent(args.Title, args.Content, args.Thumbnail);
        blog.Slug = await CreateSlug(args.Title, cancellationToken);
        blog.InitializeReadingTime();

        if (blog.PostStatus is PostStatus.Published or PostStatus.Archived)
            blog.PostStatus = PostStatus.Draft;

        blog.BlogCategories.Clear();
        blog.BlogTags.Clear();
        foreach (var categoryId in args.Categories)
        {
            blog.BlogCategories.Add(new()
            {
                BlogId = blog.Id,
                CategoryId = categoryId
            });
        }

        foreach (var tagId in args.Tags)
        {
            blog.BlogTags.Add(new()
            {
                BlogId = blog.Id,
                TagId = tagId
            });
        }

        await blogRepository.SaveChangesAsync(cancellationToken);

        return Result<BlogDetailsResult>.Success(data: (BlogDetailsResult)blog);
    }

    public async Task<PaginatedResult<BlogSummaryResult>> GetAllByTagAsync(GetBlogByTagArgs args,
        CancellationToken cancellationToken = default)
    {
        var tag = await tagRepository.GetAsync(x => x.Slug.Equals(args.Slug), false, cancellationToken);
        if (tag is null)
            return PaginatedResult<BlogSummaryResult>.Failed(404, Error.Create(Errors.Tag.NotFound));

        args = args with { TagId = tag.Id };
        var blogs = await blogRepository.GetAllByTagAsync(args, cancellationToken);
        if (blogs.Items is not { Count: > 0 })
            return PaginatedResult<BlogSummaryResult>.Failed(404, Error.Create(Errors.Blog.NotFound));

        var result = blogs.Items.Select(x => (BlogSummaryResult)x);
        return PaginatedResult<BlogSummaryResult>.Success(data: result, blogs.Index, blogs.Size, blogs.Pages,
            blogs.Count);
    }

    public async Task<PaginatedResult<BlogSummaryResult>> GetAllByCategoryAsync(GetBlogByCategoryArgs args,
        CancellationToken cancellationToken = default)
    {
        var category = await categoryRepository.GetAsync(x => x.Slug.Equals(args.Category), false, cancellationToken);
        if (category is null)
            return PaginatedResult<BlogSummaryResult>.Failed(404, Error.Create(Errors.Category.NotFound));

        List<int> categoryIds = [category.Id];
        if (args.IncludeChildren)
            categoryIds.AddRange(category.SubCategories.Select(x => x.Id));

        args = args with { Categories = categoryIds };
        var blogs = await blogRepository.GetAllByCategoryAsync(args, cancellationToken);
        if (blogs.Items is not { Count: > 0 })
            return PaginatedResult<BlogSummaryResult>.Failed(404, Error.Create(Errors.Blog.NotFound));

        var result = blogs.Items.Select(x => (BlogSummaryResult)x);
        return PaginatedResult<BlogSummaryResult>.Success(data: result, blogs.Index, blogs.Size, blogs.Pages,
            blogs.Count);
    }

    public async Task<PaginatedResult<BlogSummaryResult>> GetAllByAuthorAsync(GetBlogByAuthorArgs args,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.GetAsync(x => x.Username.Equals(args.Username), false, cancellationToken);
        if (user is null)
            return PaginatedResult<BlogSummaryResult>.Failed(404, Error.Create(Errors.User.NotFound));

        var blogs = await blogRepository.GetAllByAuthorAsync(args, cancellationToken);
        if (blogs.Items is not { Count: > 0 })
            return PaginatedResult<BlogSummaryResult>.Failed(404, Error.Create(Errors.Blog.NotFound));

        var result = blogs.Items.Select(x => (BlogSummaryResult)x);
        return PaginatedResult<BlogSummaryResult>.Success(data: result, blogs.Index, blogs.Size, blogs.Count,
            blogs.Pages);
    }

    public async Task<PaginatedResult<AdminBlogSummaryResult>> GetAllForAdminAsync(GetAllForAdminsArgs args,
        CancellationToken cancellationToken = default)
    {
        var blogs = await blogRepository.GetAllAsync(args, cancellationToken);

        var data = blogs.Items.Select(b => (AdminBlogSummaryResult)b);
        return PaginatedResult<AdminBlogSummaryResult>.Success(data: data, blogs.Index, blogs.Size, blogs.Pages);
    }

    public async Task<Result> DeleteForAdminAsync(DeleteBlogForAdminArgs args, CancellationToken cancellationToken = default)
    {
        var blog = await blogRepository.GetByIdAsync(args.BlogId, true, cancellationToken);
        if (blog is null)
            return Result.Failed(404, Error.Create(Errors.Blog.NotFound));

        await blogRepository.DeleteAsync(blog, cancellationToken);
        await blogRepository.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
}