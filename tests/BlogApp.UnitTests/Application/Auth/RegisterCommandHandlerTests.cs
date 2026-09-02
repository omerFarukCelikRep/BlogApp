using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Services;
using BlogApp.Domain.Constants;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;

namespace BlogApp.UnitTests.Application.Auth;

public class RegisterCommandHandlerTests
{
    private readonly IAuthenticationService _authenticationService;
    private readonly RegisterCommandHandler _handler;

    public RegisterCommandHandlerTests()
    {
        _authenticationService = Substitute.For<IAuthenticationService>();
        _handler = new RegisterCommandHandler(_authenticationService);
    }

    private static RegisterCommand ValidCommand() => new(
        FirstName: "John",
        LastName: "Doe",
        Email: "john@example.com",
        Username: "johndoe",
        Password: "Password123!",
        ConfirmedPassword: "Password123!");

    [Fact]
    public async Task Handle_ValidCommand_Returns201()
    {
        var command = ValidCommand();

        _authenticationService.RegisterAsync(command, CancellationToken.None).Returns(Result.Success(201));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Handle_DuplicateEmail_Returns400()
    {
        var command = ValidCommand();

        _authenticationService.RegisterAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result.Failed(400, Error.Create(Errors.Auth.EmailAlreadyExists)));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Error!.Code.Should().Be(Errors.Auth.EmailAlreadyExists);
    }

    [Fact]
    public async Task Handle_DefaultRoleNotFound_Returns400()
    {
        var command = ValidCommand();

        _authenticationService.RegisterAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result.Failed(400, Error.Create(Errors.Role.NotFound)));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.Error?.Code.Should().Be(Errors.Role.NotFound);
    }

    [Fact]
    public async Task Handle_ServiceThrows_PropagatesException()
    {
        var command = ValidCommand();

        _authenticationService.RegisterAsync(command, Arg.Any<CancellationToken>())
            .Throws(new InvalidOperationException("Unexpected error"));

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Unexpected error");
    }

    [Fact]
    public async Task Handle_Always_CallsServiceExactlyOnce()
    {
        var command = ValidCommand();

        _authenticationService.RegisterAsync(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>())
            .Returns(Result.Success(201));

        await _handler.Handle(command, CancellationToken.None);

        await _authenticationService.Received(1)
            .RegisterAsync(Arg.Any<RegisterCommand>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_CancellationRequested_PassesTokenToService()
    {
        var command = ValidCommand();
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        _authenticationService.RegisterAsync(command, cancellationToken).Returns(Result.Success(201));

        await _handler.Handle(command, cancellationToken);

        await _authenticationService.Received(1).RegisterAsync(command, cancellationToken);
    }
}