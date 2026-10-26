using BlogApp.Application.Authors;
using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Entities;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models.Authors;
using FluentAssertions;
using NSubstitute;

namespace BlogApp.UnitTests.Application.Authors;

public class GetByUsernameHandlerTests
{
    private readonly IAuthorService _authorService;
    private readonly GetByUsernameQueryHandler _handler;

    public GetByUsernameHandlerTests()
    {
        _authorService = Substitute.For<IAuthorService>();
        _handler = new(_authorService);
    }


    private static AuthorProfileResult ValidResult() => new(
        Id: Guid.NewGuid(),
        FullName: "John Doe",
        Username: "janedoe",
        Bio: null,
        ProfilePicture: null,
        WebSiteUrl: null,
        TwitterHandle: null,
        GithubHandle: null,
        JoinedDate: DateTime.UtcNow.AddMonths(-6),
        BlogCount: 3,
        TotalReads: 0,
        TotalLikes: 0,
        TopTags: []
    );

    [Fact]
    public async Task Handle_ValidUsername_ReturnsSuccessResult()
    {
        // Arrange
        var query = new GetByUsernameQuery("janedoe");
        _authorService.GetByUsernameAsync(query, Arg.Any<CancellationToken>())
            .Returns(Result<AuthorProfileResult>.Success(data: ValidResult()));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data!.FullName.Should().Be("Jane Doe");
        result.Data.Username.Should().Be("janedoe");
    }

    // ❌ User not found
    [Fact]
    public async Task Handle_UnknownUsername_Returns404()
    {
        // Arrange
        var query = new GetByUsernameQuery("unkown");
        _authorService.GetByUsernameAsync(query, Arg.Any<CancellationToken>())
            .Returns(Result<AuthorProfileResult>.Failed(404, Error.Create(Errors.User.NotFound)));

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.Error!.Code.Should().Be(Errors.User.NotFound);
    }

    [Fact]
    public async Task Handle_ValidCommand_PassesCancellationTokenToService()
    {
        var command = new GetByUsernameQuery("janedoe");
        var cancellationToken = new CancellationTokenSource().Token;

        var user = ValidResult();

        _authorService.GetByUsernameAsync(command, cancellationToken)
            .Returns(Result<AuthorProfileResult>.Success(data: ValidResult()));

        await _handler.Handle(command, cancellationToken);

        await _authorService.Received(1).GetByUsernameAsync(command, cancellationToken);
    }
}