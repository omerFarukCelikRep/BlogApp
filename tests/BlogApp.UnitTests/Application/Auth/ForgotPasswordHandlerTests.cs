using BlogApp.Application.Auth.Commands;
using BlogApp.Core.Results;
using BlogApp.Domain.Abstractions.Services;
using FluentAssertions;
using NSubstitute;

namespace BlogApp.UnitTests.Application.Auth;

public class ForgotPasswordHandlerTests
{
    private readonly IPasswordResetService _passwordResetService;
    private readonly ForgotPasswordCommandHandler _handler;

    public ForgotPasswordHandlerTests()
    {
        _passwordResetService = Substitute.For<IPasswordResetService>();
        _handler = new(_passwordResetService);
    }

    // ✅ Existing email — sends email + returns 200
    [Fact]
    public async Task Handle_ExistingEmail_SendsEmailAndReturns200()
    {
        var command = new ForgotPasswordCommand("jane@example.com");
        _passwordResetService
            .CreateForgotPasswordTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    // ✅ Non-existent email — still returns 200 (security)
    [Fact]
    public async Task Handle_NonExistentEmail_Returns200()
    {
        // Arrange
        var command = new ForgotPasswordCommand("nonexistent@example.com");

        _passwordResetService.CreateForgotPasswordTokenAsync(command, Arg.Any<CancellationToken>())
            .Returns(Result.Success());

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert — 200 to prevent email enumeration
        result.IsSuccess.Should().BeTrue();
        result.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Handle_ValidCommand_PassesCancellationTokenToService()
    {
        var command = new ForgotPasswordCommand("jane@example.com");
        var cancellationToken = new CancellationTokenSource().Token;

        _passwordResetService.CreateForgotPasswordTokenAsync(command, cancellationToken)
            .Returns(Result.Success());

        await _handler.Handle(command, cancellationToken);

        await _passwordResetService.Received(1).CreateForgotPasswordTokenAsync(command, cancellationToken);
    }
}