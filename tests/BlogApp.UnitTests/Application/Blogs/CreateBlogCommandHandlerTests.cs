using System.Linq.Expressions;
using BlogApp.Application.Blogs.Commands;
using BlogApp.Core.Results;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Models.Blogs;
using BlogApp.Domain.Utils;
using FluentAssertions;
using NSubstitute;

namespace BlogApp.UnitTests.Application.Blogs;

public class CreateBlogCommandHandlerTests
{
    private readonly IBlogService _blogService;
    private readonly CreateBlogCommandHandler _handler;

    public CreateBlogCommandHandlerTests()
    {
        _blogService = Substitute.For<IBlogService>();
        _handler = new(_blogService);
    }

    private static CreateBlogCommand ValidCommand() => new(
        Title: "My First Blog Post About .NET",
        Content: new string('x', 100),
        Thumbnail: null,
        Tags: [1],
        Categories: [1]);

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        var command = ValidCommand();

        CreateBlogResult blog = new(1, command.Title, BlogHelper.GenerateSlug(command.Title), 5);

        _blogService.CreateAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<CreateBlogResult>.Success(data: blog));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().BeEquivalentTo(blog);

        await _blogService.Received(1).CreateAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TagNotFound_ReturnsNotFound()
    {
        var command = ValidCommand();

        _blogService.CreateAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<CreateBlogResult>.Failed(404, Error.Create(Errors.Tag.NotFound)));

        var result = await _handler.Handle(
            command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Error!.Code.Should().Be(Errors.Tag.NotFound);
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsNotFound()
    {
        var command = ValidCommand();
        _blogService.CreateAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<CreateBlogResult>.Failed(404, Error.Create(Errors.Category.NotFound)));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Error!.Code.Should().Be(Errors.Category.NotFound);
    }
}