using System.Linq.Expressions;
using BlogApp.Application.Blogs.Commands;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models.Blogs;
using BlogApp.Domain.Utils;
using FluentAssertions;
using NSubstitute;

namespace BlogApp.UnitTests.Application.Blogs;

public class UpdateBlogCommandHandlerTests
{
    private readonly IBlogService _blogService;
    private readonly UpdateBlogCommandHandler _handler;

    public UpdateBlogCommandHandlerTests()
    {
        _blogService = Substitute.For<IBlogService>();
        _handler = new(_blogService);
    }

    private Blog ExistingBlog() => new()
    {
        Id = 1,
        Title = "Old Title",
        Content = "Old content",
        Slug = "old-title",
        PostStatus = PostStatus.Draft,
        AuthorId = Guid.CreateVersion7(),
        BlogTags = [],
        BlogCategories = [],
        Likes = [],
    };

    private static UpdateBlogCommand ValidCommand() => new(
        Id: 1,
        Title: "Updated Blog Post Title",
        Content: new string('x', 100),
        Thumbnail: null,
        Categories: [1],
        Tags: [1]);

    private static BlogDetailsResult ValidResult()
    {
        var command = ValidCommand();
        return new BlogDetailsResult(
            Id: command.Id,
            Title: command.Title,
            Content: command.Content,
            Slug: BlogHelper.GenerateSlug(command.Title),
            Thumbnail: null,
            ReadingTimeInMinutes: 2,
            ReadCount: 0,
            LikeCount: 0,
            IsLiked: false,
            PostStatus.Draft,
            PublishDate: null,
            Author: new(Guid.CreateVersion7(), "test", "test", null, null),
            Tags: [new(1, "Tag1", "tag-1")],
            Categories: [new(1, "Category1", "category-1")]
        );
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        var command = ValidCommand();
        var blog = ValidResult();

        _blogService.UpdateAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<BlogDetailsResult>.Success(data: blog));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data.Should().BeEquivalentTo(blog);

        await _blogService.Received(1).UpdateAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_BlogNotFound_Returns404()
    {
        var command = ValidCommand();

        _blogService.UpdateAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<BlogDetailsResult>.Failed(404, Errors.Blog.NotFound));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Error!.Code.Should().Be(Errors.Blog.NotFound);
    }

    [Fact]
    public async Task Handle_NotAuthor_Returns403()
    {
        var command = ValidCommand();

        _blogService.UpdateAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<BlogDetailsResult>.Failed(403, Errors.Blog.UnauthorizeUser));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(403);
        result.Error!.Code.Should().Be(Errors.Blog.UnauthorizeUser);
    }

    [Fact]
    public async Task Handle_TagNotFound_Returns404()
    {
        var command = ValidCommand();

        _blogService.UpdateAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<BlogDetailsResult>.Failed(404, Errors.Tag.NotFound));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Error.Should().Be(Errors.Tag.NotFound);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_Returns404()
    {
        var command = ValidCommand();

        _blogService.UpdateAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<BlogDetailsResult>.Failed(404, Errors.Category.NotFound));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Error.Should().Be(Errors.Category.NotFound);
    }

    [Fact]
    public async Task Handle_ValidCommand_PassesCancellationTokenToService()
    {
        var command = ValidCommand();
        var cancellationToken = new CancellationTokenSource().Token;

        var blog = ValidResult();

        _blogService
            .UpdateAsync(command, cancellationToken)
            .Returns(Result<BlogDetailsResult>.Success(data: blog));

        await _handler.Handle(command, cancellationToken);

        await _blogService
            .Received(1)
            .UpdateAsync(command, cancellationToken);
    }
}