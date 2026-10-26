using System.Net;
using System.Net.Http.Json;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.IntegrationTests.Auth;

[Collection("Integration")]
public class ResetPasswordEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string ForgotPasswordEndpoint = "/api/v1/auth/forgot-password";
    private const string ResetPasswordOtpEndpoint = "/api/v1/auth/reset-password-otp";

    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    public async ValueTask DisposeAsync()
    {
        await factory.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async ValueTask InitializeAsync()
    {
        await _database.InitializeAsync();
    }

    // ✅ Reset via OTP — valid
    [Fact]
    public async Task ResetPasswordWithOtp_ValidCode_Returns200()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        // Request reset
        await _client.PostAsJsonAsync(ForgotPasswordEndpoint, new { Email = email },
            cancellationToken: TestContext.Current.CancellationToken);

        // Get OTP from DB
        var user = await _database.DbContext.Users.FirstAsync(u => u.Email == email,
            cancellationToken: TestContext.Current.CancellationToken);

        var token = await _database.DbContext.PasswordResetTokens.FirstAsync(t => t.UserId == user.Id && !t.IsUsed,
            cancellationToken: TestContext.Current.CancellationToken);

        // Act — reset with OTP
        var response = await _client.PostAsJsonAsync(ResetPasswordOtpEndpoint, new
        {
            Email = email,
            token.OtpCode,
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!",
        }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ResetPasswordWithOtp_CanLoginAfterReset()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        const string oldPassword = "OldPassword123!";
        const string newPassword = "NewPassword456!";

        await AuthHelper.RegisterAsync(_client, email, oldPassword);

        // Request reset
        await _client.PostAsJsonAsync(ForgotPasswordEndpoint, new { Email = email },
            cancellationToken: TestContext.Current.CancellationToken);

        var user = await _database.DbContext.Users.FirstAsync(u => u.Email == email,
            cancellationToken: TestContext.Current.CancellationToken);

        var token = await _database.DbContext.PasswordResetTokens.FirstAsync(t => t.UserId == user.Id && !t.IsUsed,
            cancellationToken: TestContext.Current.CancellationToken);

        // Reset password
        await _client.PostAsJsonAsync(ResetPasswordOtpEndpoint, new
        {
            Email = email,
            token.OtpCode,
            NewPassword = newPassword,
            ConfirmPassword = newPassword,
        }, cancellationToken: TestContext.Current.CancellationToken);

        // Act — login with new password
        var loginResult = await AuthHelper.LoginAsync(_client, email, newPassword);

        // Assert
        loginResult.Should().NotBeNull();
        loginResult!.Token.Should().NotBeNullOrEmpty();
    }

    // ❌ Reset with wrong OTP
    [Fact]
    public async Task ResetPasswordWithOtp_InvalidCode_Returns400()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        await _client.PostAsJsonAsync(ForgotPasswordEndpoint, new { Email = email },
            cancellationToken: TestContext.Current.CancellationToken);

        // Act — wrong OTP
        var response = await _client.PostAsJsonAsync(ResetPasswordOtpEndpoint, new
        {
            Email = email,
            OtpCode = "000000",
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!",
        }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ❌ Password mismatch
    [Fact]
    public async Task ResetPassword_PasswordMismatch_Returns400()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        await _client.PostAsJsonAsync(ForgotPasswordEndpoint, new { Email = email },
            cancellationToken: TestContext.Current.CancellationToken);

        var user = await _database.DbContext.Users.FirstAsync(u => u.Email == email,
            cancellationToken: TestContext.Current.CancellationToken);

        var token = await _database.DbContext.PasswordResetTokens.FirstAsync(t => t.UserId == user.Id && !t.IsUsed,
            cancellationToken: TestContext.Current.CancellationToken);

        // Act — passwords don't match
        var response = await _client.PostAsJsonAsync(ResetPasswordOtpEndpoint, new
        {
            Email = email,
            token.OtpCode,
            NewPassword = "Password123!",
            ConfirmPassword = "DifferentPassword!",
        }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ❌ OTP expired
    [Fact]
    public async Task ResetPasswordWithOtp_ExpiredToken_Returns400()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        await _client.PostAsJsonAsync(ForgotPasswordEndpoint, new { Email = email },
            cancellationToken: TestContext.Current.CancellationToken);

        // Manually expire the token
        var user = await _database.DbContext.Users.FirstAsync(u => u.Email == email,
            cancellationToken: TestContext.Current.CancellationToken);

        var token = await _database.DbContext.PasswordResetTokens.FirstAsync(t => t.UserId == user.Id,
            cancellationToken: TestContext.Current.CancellationToken);

        token.ExpireDate = DateTime.UtcNow.AddMinutes(-1); // expired
        await _database.DbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var response = await _client.PostAsJsonAsync(ResetPasswordOtpEndpoint, new
        {
            Email = email,
            token.OtpCode,
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!",
        }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ❌ OTP used twice
    [Fact]
    public async Task ResetPasswordWithOtp_UsedToken_Returns400()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        await _client.PostAsJsonAsync(ForgotPasswordEndpoint, new { Email = email },
            cancellationToken: TestContext.Current.CancellationToken);

        var user = await _database.DbContext.Users.FirstAsync(u => u.Email == email,
            cancellationToken: TestContext.Current.CancellationToken);

        var token = await _database.DbContext.PasswordResetTokens.FirstAsync(t => t.UserId == user.Id && !t.IsUsed,
            cancellationToken: TestContext.Current.CancellationToken);

        var resetPayload = new
        {
            Email = email,
            token.OtpCode,
            NewPassword = "NewPassword123!",
            ConfirmPassword = "NewPassword123!",
        };

        // Use token once
        await _client.PostAsJsonAsync(ResetPasswordOtpEndpoint, resetPayload,
            cancellationToken: TestContext.Current.CancellationToken);

        // Act — try to use again
        var response = await _client.PostAsJsonAsync(ResetPasswordOtpEndpoint, resetPayload,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert — token already used
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ❌ Password too short
    [Fact]
    public async Task ResetPassword_PasswordTooShort_Returns400()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        await _client.PostAsJsonAsync(ForgotPasswordEndpoint, new { Email = email },
            cancellationToken: TestContext.Current.CancellationToken);

        var user = await _database.DbContext.Users.FirstAsync(u => u.Email == email,
            cancellationToken: TestContext.Current.CancellationToken);

        var token = await _database.DbContext.PasswordResetTokens.FirstAsync(t => t.UserId == user.Id && !t.IsUsed,
            cancellationToken: TestContext.Current.CancellationToken);

        // Act — too short
        var response = await _client.PostAsJsonAsync(ResetPasswordOtpEndpoint, new
        {
            Email = email,
            token.OtpCode,
            NewPassword = "short",
            ConfirmPassword = "short",
        }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ✅ Old password no longer works after reset
    [Fact]
    public async Task ResetPassword_OldPasswordFails_AfterReset()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        var oldPassword = "OldPassword123!";

        await AuthHelper.RegisterAsync(_client, email, oldPassword);

        await _client.PostAsJsonAsync(ForgotPasswordEndpoint, new { Email = email },
            cancellationToken: TestContext.Current.CancellationToken);

        var user = await _database.DbContext.Users.FirstAsync(u => u.Email == email,
            cancellationToken: TestContext.Current.CancellationToken);

        var token = await _database.DbContext.PasswordResetTokens.FirstAsync(t => t.UserId == user.Id && !t.IsUsed,
            cancellationToken: TestContext.Current.CancellationToken);

        // Reset password
        await _client.PostAsJsonAsync(ResetPasswordOtpEndpoint, new
        {
            Email = email,
            token.OtpCode,
            NewPassword = "NewPassword456!",
            ConfirmPassword = "NewPassword456!",
        }, cancellationToken: TestContext.Current.CancellationToken);

        // Act — try old password
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login",
            new { Email = email, Password = oldPassword }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert — old password rejected
        loginResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}