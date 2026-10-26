using BlogApp.Core.DataAccess.Enums;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Enums;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.IntegrationTests.Helpers;

public static class AuthorDataSeeder
{
     public record SeedResult(
        User Author,
        int  PublishedBlogCount);

    public static async Task<SeedResult> SeedAsync(BlogAppDbContext db)
    {
        // Author
        var author = new User
        {
            Id             = Guid.NewGuid(),
            FirstName      = "Jane",
            LastName       = "Doe",
            Email          = $"jane_{Guid.NewGuid():N}@example.com",
            Username       = $"janedoe_{Guid.NewGuid():N}",
            Password   = "hashed",
            EmailConfirmed = true,
            Bio            = "A passionate .NET developer",
            CreatedBy      = "seed",
            CreatedDate    = DateTime.UtcNow.AddMonths(-6),
            Status         = Status.Added,
        };
        db.Users.Add(author);

        // Tags
        var tag1 = new Tag
        {
            Name        = "DotNet",
            Slug        = $"dotnet-{Guid.NewGuid():N}",
            CreatedBy   = "seed",
            CreatedDate = DateTime.UtcNow,
            Status      = Status.Added,
        };
        var tag2 = new Tag
        {
            Name        = "CSharp",
            Slug        = $"csharp-{Guid.NewGuid():N}",
            CreatedBy   = "seed",
            CreatedDate = DateTime.UtcNow,
            Status      = Status.Added,
        };

        db.Tags.AddRange(tag1, tag2);
        await db.SaveChangesAsync();

        // Published blogs
        var blog1 = new Blog
        {
            Title                = "Blog Post One",
            Content              = new string('x', 200),
            Slug                 = $"blog-one-{Guid.NewGuid():N}",
            ReadCount            = 150,
            ReadingTimeInMinutes = 3,
            PostStatus           = PostStatus.Published,
            AuthorId             = author.Id,
            CreatedBy            = "seed",
            CreatedDate          = DateTime.UtcNow.AddDays(-10),
            Status               = Status.Added,
        };

        var blog2 = new Blog
        {
            Title                = "Blog Post Two",
            Content              = new string('x', 200),
            Slug                 = $"blog-two-{Guid.NewGuid():N}",
            ReadCount            = 80,
            ReadingTimeInMinutes = 2,
            PostStatus           = PostStatus.Published,
            AuthorId             = author.Id,
            CreatedBy            = "seed",
            CreatedDate          = DateTime.UtcNow.AddDays(-5),
            Status               = Status.Added,
        };

        // Draft blog — should not appear
        var draftBlog = new Blog
        {
            Title                = "Draft Blog Post",
            Content              = new string('x', 200),
            Slug                 = $"draft-{Guid.NewGuid():N}",
            ReadCount            = 999,
            ReadingTimeInMinutes = 1,
            PostStatus           = PostStatus.Draft,
            AuthorId             = author.Id,
            CreatedBy            = "seed",
            CreatedDate          = DateTime.UtcNow,
            Status               = Status.Added,
        };

        db.Blogs.AddRange(blog1, blog2, draftBlog);
        await db.SaveChangesAsync();

        // BlogTags
        db.BlogTags.AddRange(
            new BlogTag { BlogId = blog1.Id, TagId = tag1.Id },
            new BlogTag { BlogId = blog1.Id, TagId = tag2.Id },
            new BlogTag { BlogId = blog2.Id, TagId = tag1.Id });

        await db.SaveChangesAsync();

        return new SeedResult(
            Author:            author,
            PublishedBlogCount: 2);
    }
}