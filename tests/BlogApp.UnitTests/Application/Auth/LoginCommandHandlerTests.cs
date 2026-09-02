using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using BlogApp.Domain.Models.Auth;
using FluentAssertions;
using FluentAssertions.Common;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BlogApp.UnitTests.Application.Auth;

public class LoginCommandHandlerTests
{
    private readonly IAuthenticationService _authenticationService;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _authenticationService = Substitute.For<IAuthenticationService>();
        _handler = new LoginCommandHandler(_authenticationService);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnSuccessResult()
    {
        var command = new LoginCommand("john@example.com", "Password123!");

        var loginResult = new LoginResult(
            Token: "eyJ..",
            RefreshToken: "refresh-token-123",
            ExpiresAt: DateTimeOffset.UtcNow.AddHours(1));

        _authenticationService.LoginAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<LoginResult>.Success(data: loginResult));
        
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
        result.Data.Should().NotBeNull();
        result.Data.Token.Should().Be("eyJ..");
        result.Data.RefreshToken.Should().Be("refresh-token-123");
    }

    [Fact]
    public async Task Handle_InvalidCredentials_Returns401()
    {
        var command = new LoginCommand("john@example.com", "WrongPassword!");

        _authenticationService.LoginAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<LoginResult>.Failed(401, Error.Create(Errors.Auth.InvalidCredentials)));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Data.Should().BeNull();
        result.Error!.Code.Should().Be(Errors.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_UserNotFound_Returns401()
    {
        var command = new LoginCommand("jon@example.com", "Password123!");

        _authenticationService.LoginAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<LoginResult>.Failed(401, Error.Create(Errors.Auth.InvalidCredentials)));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Data.Should().BeNull();
        result.Error!.Code.Should().Be(Errors.Auth.InvalidCredentials);
    }

    [Fact]
    public async Task Handle_UserAccountLocked_Returns401()
    {
        var command = new LoginCommand("john@example.com", "WrongPassword!");

        _authenticationService.LoginAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result<LoginResult>.Failed(401, Error.Create(Errors.Auth.AccountLocked)));
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
        result.Data.Should().BeNull();
        result.Error!.Code.Should().Be(Errors.Auth.AccountLocked);
    }

    [Fact]
    public async Task Handle_ServiceThrowsException_PropagatesException()
    {
        var command = new LoginCommand("john@example.com", "Password123!");

        _authenticationService.LoginAsync(command, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Some Error"));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("Some Error");
    }

    [Fact]
    public async Task Handle_CancellationRequested_PassesTokenToService()
    {
        var command = new LoginCommand("john@example.com", "Password123!");
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _authenticationService.LoginAsync(command, cancellationToken)
            .Returns(Result<LoginResult>.Failed(401, Errors.Auth.InvalidCredentials));

        await _handler.Handle(command, cancellationToken);

        await _authenticationService.Received(1).LoginAsync(command, cancellationToken);
    }

    [Fact]
    public async Task Handle_Always_CallsServiceExactlyOnce()
    {
        var command = new LoginCommand("john@example.com", "Password123!");

        _authenticationService.LoginAsync(Arg.Any<LoginCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result<LoginResult>.Failed(401, Errors.Auth.InvalidCredentials));

        await _handler.Handle(command, CancellationToken.None);

        await _authenticationService.Received(1).LoginAsync(command, Arg.Any<CancellationToken>());
    }
}