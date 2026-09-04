using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Security.Abstractions;
using BlogApp.Domain.Abstractions.Services;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BlogApp.UnitTests.Application.Auth;

public class LogoutCommandHandlerTests
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IDomainPrincipal _domainPrincipal;
    private readonly LogoutCommandHandler _handler;

    public LogoutCommandHandlerTests()
    {
        _domainPrincipal = Substitute.For<IDomainPrincipal>();
        _refreshTokenService = Substitute.For<IRefreshTokenService>();
        _handler = new LogoutCommandHandler(_refreshTokenService);
    }

    [Fact]
    public async Task Handle_AuthenticatedUser_RevokesAllTokensAndReturns200()
    {
        _domainPrincipal.UserId.Returns(Guid.NewGuid());

        _refreshTokenService.RevokeAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(
            new LogoutCommand(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);

        await _refreshTokenService.Received(1).RevokeAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ServiceThrows_PropagatesException()
    {
        _domainPrincipal.UserId.Returns(Guid.NewGuid());

        _refreshTokenService.RevokeAllAsync(Arg.Any<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("DB error"));

        var act = () => _handler.Handle(new LogoutCommand(), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}