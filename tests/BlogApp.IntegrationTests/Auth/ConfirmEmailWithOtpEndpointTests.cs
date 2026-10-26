using System.Net;
using System.Net.Http.Json;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.IntegrationTests.Auth;

[Collection("Integration")]
public class ConfirmEmailWithOtpEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
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

    // ✅ Confirm email with OTP
    [Fact]
    public async Task ConfirmEmailWithOtp_ValidOtp_Returns200()
    {
        // Arrange — register + get OTP from DB
        var email = $"otp_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var login = await AuthHelper.LoginAsync(_client, email, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue(
                "Bearer", login!.Token);

        // Send confirmation (creates token in DB)
        await _client.PostAsync("/api/v1/auth/send-confirmation", null, TestContext.Current.CancellationToken);

        // Get OTP from DB
        var token = await _database.DbContext.EmailConfirmations
            .Where(t => t.User.Email == email && !t.IsUsed)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/confirm-email-otp", new { token!.OtpCode },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ❌ Confirm email with invalid OTP
    [Fact]
    public async Task ConfirmEmailWithOtp_InvalidOtp_Returns400()
    {
        // Arrange
        var email = $"bad_otp_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var login = await AuthHelper.LoginAsync(_client, email, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login!.Token);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/confirm-email-otp", new { OtpCode = "000000" },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}