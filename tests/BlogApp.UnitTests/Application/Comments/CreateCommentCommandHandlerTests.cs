using BlogApp.Application.Comments.Commands;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models.Comments;
using FluentAssertions;
using NSubstitute;

namespace BlogApp.UnitTests.Application.Comments;

public class CreateCommentCommandHandlerTests
{
    private readonly ICommentService _commentService;
    private readonly CreateCommentCommandHandler _handler;
    private readonly IDomainPrincipal _domainPrincipal;

    public CreateCommentCommandHandlerTests()
    {
        _commentService = Substitute.For<ICommentService>();
        _handler = new(_commentService);

        _domainPrincipal = Substitute.For<IDomainPrincipal>();
        _domainPrincipal.UserId.Returns(Guid.CreateVersion7());
        _domainPrincipal.FullName.Returns("Jane Doe");
        _domainPrincipal.Username.Returns("janedoe");
    }

    private static CreateCommentCommand ValidCommand() => new(
        BlogId: 1,
        Content: "Great blog post!");

    private static Blog PublishedBlog() => new()
    {
        Id = 1,
        Title = "Test Blog",
        Content = "Content",
        Slug = "test-blog",
        PostStatus = PostStatus.Published,
        AuthorId = Guid.NewGuid(),
    };

    [Fact]
    public async Task Handle_ValidCommand_Returns201()
    {
        var args = ValidCommand();
        var commentResult = new CommentResult(1, args.Content, false, DateTime.Now,
            new(_domainPrincipal.UserId, _domainPrincipal.FullName, _domainPrincipal.Username!, null), null, []);
        _commentService.CreateAsync(ValidCommand(), Arg.Any<CancellationToken>())
            .Returns(Result<CommentResult>.Success(data: commentResult));

        var result = await _handler.Handle(
            ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Content.Should().Be("Great blog post!");
    }

    [Fact]
    public async Task Handle_BlogNotFound_Returns404()
    {
        _commentService.CreateAsync(ValidCommand(), Arg.Any<CancellationToken>())
            .Returns(Result<CommentResult>.Failed(404, Error.Create(Errors.Blog.NotFound)));

        var result = await _handler.Handle(
            ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Error!.Code.Should().Be(Errors.Blog.NotFound);
    }

    [Fact]
    public async Task Handle_DraftBlog_Returns400()
    {
        _commentService.CreateAsync(Arg.Any<CreateCommentArgs>(), Arg.Any<CancellationToken>())
            .Returns(Result<CommentResult>.Failed(400, Errors.Blog.NotPublished));

        var result = await _handler.Handle(
            ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Error.Should().Be(Errors.Blog.NotPublished);
    }

    [Fact]
    public async Task Handle_ValidCommand_CallsOnce()
    {
        var args = ValidCommand();
        _commentService.CreateAsync(args, Arg.Any<CancellationToken>())
            .Returns(Result<CommentResult>.Failed(400, Errors.Blog.NotPublished));

        var result = await _handler.Handle(
            ValidCommand(), CancellationToken.None);

        await _handler.Handle(ValidCommand(), CancellationToken.None);

        await _commentService.Received(1).CreateAsync(args, Arg.Any<CancellationToken>());
    }
}