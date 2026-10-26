using BlogApp.Core.DataAccess.Enums;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Enums;
using BlogApp.Infrastructure.EFCore.Contexts;
using Bogus;

namespace BlogApp.IntegrationTests.Helpers;

public static class HomePageDataSeeder
{
    private static readonly Faker _faker = new();

    public static async Task<(List<Blog> Blogs, List<Tag> Tags, User Author)> SeedAsync(
        BlogAppDbContext context)
    {
        User author = new()
        {
            Id = Guid.CreateVersion7(),
            FirstName = _faker.Name.FirstName(),
            LastName = _faker.Name.LastName(),
            Email = _faker.Internet.Email(),
            Username = _faker.Internet.UserName(),
            Password = "hashed",
            EmailConfirmed = true,
            CreatedBy = "Seed",
            CreatedDate = DateTimeOffset.UtcNow,
            Status = Status.Added
        };
        context.Users.Add(author);

        var tags = Enumerable.Range(1, 12).Select(i => new Tag
        {
            Name = i + _faker.Lorem.Word(),
            Slug = i + _faker.Lorem.Slug(),
            CreatedBy = "Seed",
            CreatedDate = DateTimeOffset.UtcNow,
            Status = Status.Added
        }).ToList();
        await context.Tags.AddRangeAsync(tags, TestContext.Current.CancellationToken);

        List<Blog> blogs = [];
        var draftBlogs = Enumerable.Range(1, 2).Select(i =>
        {
            var blog = new Blog
            {
                Title = _faker.Lorem.Text() + " Draft",
                Content = _faker.Lorem.Paragraph(),
                Slug = _faker.Lorem.Slug(),
                ReadingTimeInMinutes = 5,
                PostStatus = PostStatus.Draft,
                AuthorId = author.Id,
                CreatedBy = "Seed",
                CreatedDate = DateTimeOffset.UtcNow,
                Status = Status.Added
            };
            blogs.Add(blog);
            return blog;
        });
        await context.Blogs.AddRangeAsync(draftBlogs, TestContext.Current.CancellationToken);

        var publishedBlogs = Enumerable.Range(1, 15).Select(i =>
        {
            var blog = new Blog
            {
                Title = _faker.Lorem.Text(),
                Content = _faker.Lorem.Paragraph(),
                Slug = _faker.Lorem.Slug(),
                ReadingTimeInMinutes = 5,
                PostStatus = PostStatus.Draft,
                AuthorId = author.Id,
                CreatedBy = "Seed",
                CreatedDate = DateTimeOffset.UtcNow,
                Status = Status.Added
            };
            blogs.Add(blog);
            return blog;
        });
        await context.Blogs.AddRangeAsync(publishedBlogs, TestContext.Current.CancellationToken);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        List<Blog> filteredBlogs = [.. blogs.Where(x => x.PostStatus == PostStatus.Published)];

        foreach (var (blog, i) in filteredBlogs.Select((x, i) => (x, i)))
        {
            context.BlogTags.Add(new BlogTag()
            {
                BlogId = blog.Id,
                TagId = tags[i % tags.Count].Id
            });

            if (i < 5)
            {
                context.BlogTags.Add(new BlogTag()
                {
                    BlogId = blog.Id,
                    TagId = tags[0].Id
                });
            }
        }

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        return (blogs, tags, author);
    }
}