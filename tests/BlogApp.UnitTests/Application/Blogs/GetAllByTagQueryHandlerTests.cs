using BlogApp.Application.Blogs.Queries;
using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Blogs;
using FluentAssertions;
using NSubstitute;

namespace BlogApp.UnitTests.Application.Blogs;

public class GetAllByTagQueryHandlerTests
{
    private readonly IBlogService _blogService;
    private readonly GetAllByTagQueryHandler _handler;

    public GetAllByTagQueryHandlerTests()
    {
        _blogService = Substitute.For<IBlogService>();
        _handler = new(_blogService);
    }

    private static GetAllByTagQuery ValidCommand() => new("tag-1", 1, 10, "latest");

    private static PaginatedResult<BlogSummaryResult> ValidResult()
    {
        var command = ValidCommand();
        return new PaginatedResult<BlogSummaryResult>(
            IsSuccess: true,
            Message: string.Empty,
            StatusCode: 200,
            Data: [],
            Error: null,
            PageNumber: command.Page!.Value,
            PageSize: command.PageSize!.Value,
            TotalCount: 200);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        var command = ValidCommand();
        var blog = ValidResult();

        _blogService.GetAllByTagAsync(command, Arg.Any<CancellationToken>())
            .Returns(ValidResult());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();

        await _blogService.Received(1).GetAllByTagAsync(command, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_TagNotFound_Returns404()
    {
        var command = ValidCommand();

        _blogService.GetAllByTagAsync(command, Arg.Any<CancellationToken>())
            .Returns(PaginatedResult<BlogSummaryResult>.Failed(404, Error.Create(Errors.Tag.NotFound)));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Error!.Code.Should().Be(Errors.Tag.NotFound);
    }

    [Fact]
    public async Task Handle_BlogNotFound_Returns404()
    {
        var command = ValidCommand();

        _blogService.GetAllByTagAsync(command, Arg.Any<CancellationToken>())
            .Returns(PaginatedResult<BlogSummaryResult>.Failed(404, Error.Create(Errors.Blog.NotFound)));

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Error!.Code.Should().Be(Errors.Blog.NotFound);
    }

    [Fact]
    public async Task Handle_ValidCommand_PassesCancellationTokenToService()
    {
        var command = ValidCommand();
        var cancellationToken = new CancellationTokenSource().Token;

        var blog = ValidResult();

        _blogService
            .GetAllByTagAsync(command, cancellationToken)
            .Returns(ValidResult());

        await _handler.Handle(command, cancellationToken);

        await _blogService
            .Received(1)
            .GetAllByTagAsync(command, cancellationToken);
    }
}