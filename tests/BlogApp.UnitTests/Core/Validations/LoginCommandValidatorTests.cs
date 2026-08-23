using BlogApp.Application.Auth.Commands;
using FluentAssertions;

namespace BlogApp.UnitTests.Core.Validations;

public class LoginCommandValidatorTests
{
    private readonly LoginCommandValidator _validator = new();

    [Fact]
    public async Task ValidateAsync_ValidCommand_ShouldPass()
    {
        var command = new LoginCommand("john@example.com", "password123");
        
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    // Email Rules
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_EmptyEmail_ShouldFail(string? email)
    {
        var command = new LoginCommand(email!, "password123");
        
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "'Email' must not be empty!");
    }

    [Theory]
    [InlineData("notanemail")]
    [InlineData("missing@")]
    [InlineData("@nodomain.com")]
    [InlineData("spaces in@email.com")]
    public async Task ValidateAsync_InvalidEmailFormat_ShouldFail(string email)
    {
        var command = new LoginCommand(email, "password123");
        
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "'Email' is not a valid email format");
    }
    
    // Password Rules
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ValidateAsync_EmptyPassword_ShouldFail(string? password)
    {
        var command = new LoginCommand("john@example.com", password!);
        
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "'Password' must not be empty!");
    }

    [Fact]
    public async Task ValidateAsync_BothFieldsEmpty_ShouldReturnMultipleErrors()
    {
        var command = new LoginCommand(string.Empty, string.Empty);
        
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);
        
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCountGreaterThan(1);
    }
}