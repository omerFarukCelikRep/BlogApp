using System.Net;
using System.Net.Http.Json;
using BlogApp.Domain.Models.RefreshTokens;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Auth;

[Collection("Integration")]
public class RefreshTokenEndpointTests(BlogAppFactory factory) : IClassFixture<DatabaseFixture>
{
    private const string RefreshTokenEndpoint = "/api/v1/api/refreshtoken";
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<(string AccessToken, string RefreshToken)> RegisterAndLoginAsync(string? email = null,
        string? password = null)
    {
        email ??= $"refresh_{Guid.NewGuid():N}@example.com";
        password ??= "Password123!";

        await AuthHelper.RegisterAsync(_client, email, password);
        var loginResult = await AuthHelper.LoginAsync(_client, email, password);

        return (loginResult!.Token, loginResult.RefreshToken);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_Returns200WithNewTokens()
    {
        var (_, refreshToken) = await RegisterAndLoginAsync();
        var request = new { Token = refreshToken };

        var response = await _client.PostAsJsonAsync(RefreshTokenEndpoint, request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result =
            await response.Content.ReadFromJsonAsync<RefreshTokenResult>(TestContext.Current.CancellationToken);

        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
        result.ExpireDate.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_Returns200WithDifferentTokens()
    {
        var (originalAccessToken, originalRefreshToken) = await RegisterAndLoginAsync();
        var request = new { Token = originalRefreshToken };

        var response =
            await _client.PostAsJsonAsync(RefreshTokenEndpoint, request, TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<RefreshTokenResult>(TestContext.Current.CancellationToken);

        result!.Token.Should().NotBe(originalAccessToken);
        result.RefreshToken.Should().NotBe(originalRefreshToken);
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_Returns401()
    {
        var request = new { Token = "invalid-token-that-does-not-exist" };

        var response =
            await _client.PostAsJsonAsync(RefreshTokenEndpoint, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RefreshToken_AlreadyInUsedToken_Returns401()
    {
        var (_, refreshToken) = await RegisterAndLoginAsync();
        var request = new { Token = refreshToken };

        var response =
            await _client.PostAsJsonAsync(RefreshTokenEndpoint, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task RefreshToken_EmptyToken_Returns400(string token)
    {
        var request = new { Token = token };

        var response =
            await _client.PostAsJsonAsync(RefreshTokenEndpoint, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_MissingTokenField_Returns400()
    {
        var request = new { };

        var response =
            await _client.PostAsJsonAsync(RefreshTokenEndpoint, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsValidJwt()
    {
        var (_, refreshToken) = await RegisterAndLoginAsync();
        var request = new { Token = refreshToken };

        var response =
            await _client.PostAsJsonAsync(RefreshTokenEndpoint, request, TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<RefreshTokenResult>(TestContext.Current.CancellationToken);

        result!.Token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_NewRefreshTokenIsUsable()
    {
        var (_, firstRefreshToken) = await RegisterAndLoginAsync();

        var firstResponse = await _client.PostAsJsonAsync(RefreshTokenEndpoint, new { Token = firstRefreshToken },
            TestContext.Current.CancellationToken);

        var firstResult =
            await firstResponse.Content.ReadFromJsonAsync<RefreshTokenResult>(TestContext.Current.CancellationToken);

        var secondResponse = await _client.PostAsJsonAsync(RefreshTokenEndpoint,
            new { Token = firstResult!.RefreshToken }, TestContext.Current.CancellationToken);

        secondResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RefreshToken_ValidToken_ReturnsJsonContentType()
    {
        var (_, refreshToken) = await RegisterAndLoginAsync();
        var request = new { Token = refreshToken };

        var response =
            await _client.PostAsJsonAsync(RefreshTokenEndpoint, request, TestContext.Current.CancellationToken);

        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
}