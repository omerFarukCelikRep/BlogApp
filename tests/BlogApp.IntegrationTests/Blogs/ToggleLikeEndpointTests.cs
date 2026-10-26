using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Blogs;

[Collection("Integration")]
public class ToggleLikeEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "/api/v1/blogs";
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

    private async Task<string> LoginUserAsync(string? email = null)
    {
        email ??= $"user_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var result = await AuthHelper.LoginAsync(_client, email, "Password123!");
        return result!.Token;
    }

    [Fact]
    public async Task ToggleLike_AuthenticatedUser_Like_Returns200()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await LoginUserAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/like", null,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ToggleLike_FirstLike_IsLikedTrue()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await LoginUserAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/like", null,
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<ToggleLikeResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.IsSuccess.Should().BeTrue();
        result.Data!.IsLiked.Should().BeTrue();
        result.Data.LikesCount.Should().Be(1);
    }

    [Fact]
    public async Task ToggleLike_LikeThenUnlike_IsLikedFalse()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await LoginUserAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/like", null,
            TestContext.Current.CancellationToken);

        var response = await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/like", null,
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<ToggleLikeResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.Data!.IsLiked.Should().BeFalse();
        result.Data.LikesCount.Should().Be(0);
    }

    [Fact]
    public async Task ToggleLike_MultipleUsers_LikeCountAccumulates()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token1 = await LoginUserAsync();
        var token2 = await LoginUserAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token1);
        await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/like", null,
            TestContext.Current.CancellationToken);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token2);
        var response = await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/like", null,
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<ToggleLikeResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.Data!.LikesCount.Should().Be(2);
    }

    [Fact]
    public async Task ToggleLike_NoAuth_Returns401()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/like", null,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task ToggleLike_UnknownBlog_Returns404()
    {
        var token = await LoginUserAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsync($"{Endpoint}/99999/like", null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ToggleLike_AfterLike_BlogDetailShowsIsLikedTrue()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await LoginUserAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/like", null,
            TestContext.Current.CancellationToken);

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<BlogDetailsResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.Data!.IsLiked.Should().BeTrue();
        result.Data.LikeCount.Should().Be(1);
    }
}