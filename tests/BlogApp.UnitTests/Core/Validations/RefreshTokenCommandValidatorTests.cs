using BlogApp.Application.Auth.Commands;
using FluentAssertions;

namespace BlogApp.UnitTests.Core.Validations;

public class RefreshTokenCommandValidatorTests
{
    private readonly RefreshTokenCommandValidator _validator = new();
    
    [Fact]
    public async Task ValidateAsync_ValidCommand_ShouldPass()
    {
        var command = new RefreshTokenCommand("valid-refresh-token-123");

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }
    
    //Empty Token
    [Theory]
    [InlineData(null)]
    [InlineData(" ")]
    [InlineData("")]
    public async Task ValidateAsync_EmptyToken_ShouldFail(string? token)
    {
        var command = new RefreshTokenCommand(token!);
        
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(RefreshTokenCommand));
    }
}