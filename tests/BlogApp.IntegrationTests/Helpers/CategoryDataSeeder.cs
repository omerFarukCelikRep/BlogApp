using BlogApp.Core.DataAccess.Enums;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Enums;
using BlogApp.Infrastructure.EFCore.Contexts;

namespace BlogApp.IntegrationTests.Helpers;

public static class CategoryDataSeeder
{
    public record SeedResult(
        Category ParentCategory,
        Category ChildCategory,
        User Author);

    public static async Task<SeedResult> SeedAsync(BlogAppDbContext db)
    {
        // Author
        var author = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "Author",
            Email = $"author_{Guid.NewGuid():N}@example.com",
            Username = $"testauthor_{Guid.NewGuid():N}",
            Password = "hashed",
            EmailConfirmed = true,
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };
        db.Users.Add(author);

        // Parent category
        var parent = new Category
        {
            Name = "Backend",
            Slug = $"backend-{Guid.NewGuid():N}",
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };
        db.Categories.Add(parent);
        await db.SaveChangesAsync();

        // Child category
        var child = new Category
        {
            Name = "API Design",
            Slug = $"api-design-{Guid.NewGuid():N}",
            ParentId = parent.Id,
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };
        db.Categories.Add(child);
        await db.SaveChangesAsync();

        // Tag for blogs
        var tag = new Tag
        {
            Name = "DotNet",
            Slug = $"dotnet-{Guid.NewGuid():N}",
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };
        db.Tags.Add(tag);
        await db.SaveChangesAsync();

        // Published blog in parent category
        var blogInParent = new Blog
        {
            Title = "Blog in Parent Category",
            Content = new string('x', 200),
            Slug = $"blog-parent-{Guid.NewGuid():N}",
            ReadCount = 10,
            ReadingTimeInMinutes = 2,
            PostStatus = PostStatus.Published,
            AuthorId = author.Id,
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };

        // Published blog in child category
        var blogInChild = new Blog
        {
            Title = "Blog in Child Category",
            Content = new string('x', 200),
            Slug = $"blog-child-{Guid.NewGuid():N}",
            ReadCount = 20,
            ReadingTimeInMinutes = 3,
            PostStatus = PostStatus.Published,
            AuthorId = author.Id,
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };

        // Draft blog — should not appear
        var draftBlog = new Blog
        {
            Title = "Draft Blog in Category",
            Content = new string('x', 200),
            Slug = $"draft-{Guid.NewGuid():N}",
            ReadCount = 999,
            ReadingTimeInMinutes = 1,
            PostStatus = PostStatus.Draft,
            AuthorId = author.Id,
            CreatedBy = "seed",
            CreatedDate = DateTime.UtcNow,
            Status = Status.Added,
        };

        db.Blogs.AddRange(blogInParent, blogInChild, draftBlog);
        await db.SaveChangesAsync();

        // BlogCategories + BlogTags
        db.BlogCategories.AddRange(
            new BlogCategory { BlogId = blogInParent.Id, CategoryId = parent.Id },
            new BlogCategory { BlogId = blogInChild.Id, CategoryId = child.Id },
            new BlogCategory { BlogId = draftBlog.Id, CategoryId = parent.Id });

        db.BlogTags.AddRange(
            new BlogTag { BlogId = blogInParent.Id, TagId = tag.Id },
            new BlogTag { BlogId = blogInChild.Id, TagId = tag.Id });

        await db.SaveChangesAsync();

        return new SeedResult(parent, child, author);
    }
}