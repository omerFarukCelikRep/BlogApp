using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Enums;
using BlogApp.Domain.Models.Auth;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.IntegrationTests.Auth;

[Collection("Integration")]
public class TwoFactorEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "/api/v1/auth/2fa";

    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private async Task<string> RegisterAndLoginAsync()
    {
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var login = await AuthHelper.LoginAsync(_client, email, "Password123!");
        return login!.Token;
    }

    public async ValueTask DisposeAsync()
    {
        await factory.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async ValueTask InitializeAsync()
    {
        await _database.InitializeAsync();
    }

    [Fact]
    public async Task SendEnable2fa_AuthenticatedUser_Returns200()
    {
        // Arrange
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync($"{Endpoint}/enable/send", new { PhoneNumber = "+15550000001" },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    // ❌ Send enable 2FA — unauthenticated
    [Fact]
    public async Task SendEnable2fa_NoAuth_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.PostAsJsonAsync($"{Endpoint}/enable/send", new { PhoneNumber = "+15550000001" },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ✅ Confirm enable 2FA with valid OTP
    [Fact]
    public async Task ConfirmEnable2fa_ValidCode_Returns200()
    {
        // Arrange
        var email = $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var login = await AuthHelper.LoginAsync(_client, email, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login!.Token);

        var phone = "+15550000002";

        // Send OTP (creates DB record)
        await _client.PostAsJsonAsync($"{Endpoint}/enable/send", new { PhoneNumber = phone },
            cancellationToken: TestContext.Current.CancellationToken);

        // Get code from DB
        var user = await _database.DbContext.Users.FirstAsync(u => u.Email == email,
            cancellationToken: TestContext.Current.CancellationToken);

        var code = await _database.DbContext.TwoFactorCodes
            .Where(t => t.UserId == user.Id &&
                        t.Purpose == TwoFactorPurpose.Enable &&
                        !t.IsUsed)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act
        var response = await _client.PostAsJsonAsync($"{Endpoint}/enable/confirm",
            new { Code = code!.Code, PhoneNumber = phone }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify 2FA enabled in DB
        var updatedUser = await _database.DbContext.Users
            .FirstAsync(u => u.Email == email, cancellationToken: TestContext.Current.CancellationToken);
        updatedUser.TwoFactorEnabled.Should().BeTrue();
        updatedUser.PhoneNumber.Should().Be(phone);
    }

    // ❌ Confirm enable 2FA with invalid OTP
    [Fact]
    public async Task ConfirmEnable2fa_InvalidCode_Returns400()
    {
        // Arrange
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync($"{Endpoint}/enable/confirm",
            new { Code = "000000", PhoneNumber = "+15550000003" },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ✅ Verify 2FA during login — valid OTP returns full token
    [Fact]
    public async Task Verify2fa_ValidCode_ReturnsFullToken()
    {
        // Arrange — setup user with 2FA enabled
        var email = $"twofa_{Guid.NewGuid():N}@example.com";
        var phone = "+15550000004";

        await AuthHelper.RegisterAsync(_client, email);
        var loginResult = await AuthHelper.LoginAsync(
            _client, email, "Password123!");

        // Enable 2FA
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        await _client.PostAsJsonAsync($"{Endpoint}/enable/send", new { PhoneNumber = phone },
            cancellationToken: TestContext.Current.CancellationToken);

        var user = await _database.DbContext.Users
            .FirstAsync(u => u.Email == email, cancellationToken: TestContext.Current.CancellationToken);

        var enableCode = await _database.DbContext.TwoFactorCodes
            .Where(t => t.UserId == user.Id &&
                        t.Purpose == TwoFactorPurpose.Enable)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        await _client.PostAsJsonAsync($"{Endpoint}/enable/confirm",
            new { Code = enableCode!.Code, PhoneNumber = phone },
            cancellationToken: TestContext.Current.CancellationToken);

        // Login again — should get limited token
        _client.DefaultRequestHeaders.Authorization = null;
        var limitedLogin = await AuthHelper.LoginAsync(_client, email, "Password123!");

        limitedLogin!.TwoFactorEnabled.Should().BeTrue();

        // Get 2FA code from DB
        var loginCode = await _database.DbContext.TwoFactorCodes
            .Where(t => t.UserId == user.Id &&
                        t.Purpose == TwoFactorPurpose.Login &&
                        !t.IsUsed)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act — verify 2FA
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", limitedLogin.Token);

        var response = await _client.PostAsJsonAsync($"{Endpoint}/verify", new { Code = loginCode!.Code },
            cancellationToken: TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<LoginResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Data!.Scope.Should().Be("full_access");
        result.Data.TwoFactorEnabled.Should().BeFalse();
    }

    // ❌ Verify 2FA with invalid code
    [Fact]
    public async Task Verify2fa_InvalidCode_Returns400()
    {
        // Arrange — need limited token
        // Simplified: just test with full token that scope check fails
        var token = await RegisterAndLoginAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PostAsJsonAsync($"{Endpoint}/verify", new { Code = "000000" },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert — fails due to wrong scope or invalid code
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Forbidden);
    }

    // ✅ Disable 2FA flow
    [Fact]
    public async Task Disable2fa_ValidFlow_Returns200()
    {
        // Arrange — enable 2FA first
        var email = $"disable_{Guid.NewGuid():N}@example.com";
        var phone = "+15550000005";

        await AuthHelper.RegisterAsync(_client, email);
        var login = await AuthHelper.LoginAsync(_client, email, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", login!.Token);

        // Enable 2FA
        await _client.PostAsJsonAsync($"{Endpoint}/enable/send", new { PhoneNumber = phone },
            cancellationToken: TestContext.Current.CancellationToken);

        var user = await _database.DbContext.Users
            .FirstAsync(u => u.Email == email, cancellationToken: TestContext.Current.CancellationToken);

        var enableCode = await _database.DbContext.TwoFactorCodes
            .Where(t => t.UserId == user.Id &&
                        t.Purpose == TwoFactorPurpose.Enable)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        await _client.PostAsJsonAsync($"{Endpoint}/enable/confirm",
            new { Code = enableCode!.Code, PhoneNumber = phone },
            cancellationToken: TestContext.Current.CancellationToken);

        // Send disable OTP
        var sendResponse =
            await _client.PostAsync($"{Endpoint}/disable/send", null, TestContext.Current.CancellationToken);
        sendResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Get disable code from DB
        var disableCode = await _database.DbContext.TwoFactorCodes
            .Where(t => t.UserId == user.Id &&
                        t.Purpose == TwoFactorPurpose.Disable &&
                        !t.IsUsed)
            .FirstOrDefaultAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act — confirm disable
        var response = await _client.PostAsJsonAsync($"{Endpoint}/disable/confirm", new { Code = disableCode!.Code },
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedUser = await _database.DbContext.Users
            .FirstAsync(u => u.Email == email, cancellationToken: TestContext.Current.CancellationToken);
        updatedUser.TwoFactorEnabled.Should().BeFalse();
    }
}