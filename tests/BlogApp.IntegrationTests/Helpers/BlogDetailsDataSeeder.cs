using BlogApp.Core.DataAccess.Enums;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Enums;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.IntegrationTests.Helpers;

public static class BlogDetailsDataSeeder
{
    public record SeedResult(
        Blog PublishedBlog,
        Blog DraftBlog,
        User Author,
        User OtherUser,
        Tag Tag,
        Category Category);

    public static async Task<SeedResult> SeedAsync(BlogAppDbContext context)
    {
        var author = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Jane",
            LastName = "Doe",
            Email = $"author_{Guid.NewGuid():N}@example.com",
            Username = $"janedoe_{Guid.NewGuid():N}",
            Password = "hashed",
            EmailConfirmed = true,
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };

        var otherUser = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Bob",
            LastName = "Smith",
            Email = $"bob_{Guid.NewGuid():N}@example.com",
            Username = $"bobsmith_{Guid.NewGuid():N}",
            Password = "hashed",
            EmailConfirmed = true,
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };

        context.Users.AddRange(author, otherUser);

        var tag = new Tag
        {
            Name = "DotNet",
            Slug = $"dotnet-{Guid.NewGuid():N}",
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };

        var category = new Category
        {
            Name = "Backend",
            Slug = $"backend-{Guid.NewGuid():N}",
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };

        await context.Tags.AddAsync(tag);
        await context.Categories.AddAsync(category);

        await context.SaveChangesAsync();

        var publishedBlog = new Blog
        {
            Title = "Published Blog Post",
            Content = new string('x', 200),
            Slug = $"published-blog-{Guid.NewGuid():N}",
            ReadCount = 42,
            ReadingTimeInMinutes = 3,
            PostStatus = PostStatus.Published,
            AuthorId = author.Id,
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };

        var draftBlog = new Blog
        {
            Title = "Draft Blog Post",
            Content = new string('x', 200),
            Slug = $"draft-blog-{Guid.NewGuid():N}",
            ReadCount = 0,
            ReadingTimeInMinutes = 2,
            PostStatus = PostStatus.Draft,
            AuthorId = author.Id,
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };

        context.Blogs.AddRange(publishedBlog, draftBlog);
        await context.SaveChangesAsync();

        await context.BlogCategories.AddRangeAsync(
            new BlogCategory() { BlogId = publishedBlog.Id, CategoryId = category.Id },
            new BlogCategory() { BlogId = draftBlog.Id, CategoryId = category.Id });

        await context.BlogTags.AddRangeAsync(
            new BlogTag() { BlogId = publishedBlog.Id, TagId = tag.Id },
            new BlogTag() { BlogId = draftBlog.Id, TagId = tag.Id });

        await context.SaveChangesAsync();

        return new SeedResult(publishedBlog, draftBlog, author, otherUser, tag, category);
    }
}