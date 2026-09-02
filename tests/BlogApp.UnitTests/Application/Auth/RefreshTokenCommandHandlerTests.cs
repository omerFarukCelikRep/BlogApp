using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.RefreshTokens;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BlogApp.UnitTests.Application.Auth;

public class RefreshTokenCommandHandlerTests
{
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _refreshTokenService = Substitute.For<IRefreshTokenService>();
        _handler = new RefreshTokenCommandHandler(_refreshTokenService);
    }
    
    private static RefreshTokenCommand ValidCommand() => new RefreshTokenCommand("eyJ..");

    [Fact]
    public async Task Handle_ValidCToken_ReturnSuccessResult()
    {
        var command = ValidCommand();
        var refreshTokenResult = new RefreshTokenResult(
            Token: "new-access-token",
            RefreshToken: "new-refresh-token-123",
            ExpireDate:DateTime.UtcNow.AddDays(7));

        _refreshTokenService.RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<RefreshTokenResult>.Success(data: refreshTokenResult));
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data!.Token.Should().Be("new-access-token");
        result.Data!.RefreshToken.Should().Be("new-refresh-token-123");
        result.Data.ExpireDate.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task Handle_InvalidToken_Return401()
    {
        var command = ValidCommand();

        _refreshTokenService.RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<RefreshTokenResult>.Failed(401, Error.Create(Errors.Auth.InvalidCredentials)));
        
        var result = await _handler.Handle(command, CancellationToken.None);
        
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Data.Should().BeNull();
        result.Error!.Code.Should().Be(Errors.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_ExpiredToken_Returns401()
    {
        var command = ValidCommand();
        
        _refreshTokenService.RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<RefreshTokenResult>.Failed(401, Error.Create(Errors.Auth.InvalidCredentials)));
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Data.Should().BeNull();
        result.Error!.Code.Should().Be(Errors.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_RevokedToken_Returns401()
    {
        var command = ValidCommand();
        
        _refreshTokenService.RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<RefreshTokenResult>.Failed(401, Error.Create(Errors.Auth.InvalidCredentials)));
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Data.Should().BeNull();
        result.Error!.Code.Should().Be(Errors.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_AlreadyUsedToken_Returns401()
    {
        var command = ValidCommand();
        
        _refreshTokenService.RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<RefreshTokenResult>.Failed(401, Error.Create(Errors.Auth.InvalidCredentials)));
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Data.Should().BeNull();
        result.Error!.Code.Should().Be(Errors.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_PropagatesException()
    {
        var command = ValidCommand();

        _refreshTokenService.RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Some Error"));
        
        var act = () => _handler.Handle(command, CancellationToken.None);
        
        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Some Error");
    }

    [Fact]
    public async Task Handle_CancellationRequested_PassesTokenToService()
    {
        var command = ValidCommand();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _refreshTokenService.RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<RefreshTokenResult>.Failed(401, Errors.Auth.InvalidCredentials));

        await _handler.Handle(command, cancellationToken);

        await _refreshTokenService.Received(1).RefreshTokenAsync(command, cancellationToken);
    }

    [Fact]
    public async Task Handle_Always_CallsServiceExactlyOnce()
    {
        var command = ValidCommand();

        _refreshTokenService.RefreshTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<RefreshTokenResult>.Failed(401, Errors.Auth.InvalidCredentials));

        await _handler.Handle(command, CancellationToken.None);

        await _refreshTokenService.Received(1).RefreshTokenAsync(command, Arg.Any<CancellationToken>());
    }
}