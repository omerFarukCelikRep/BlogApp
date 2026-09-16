using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Comments;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Comments;

[Collection("Integration")]
public class DeleteCommentEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "api/v1/comments";

    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private async Task<string> GetReaderTokenAsync()
    {
        var email = $"reader_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var result = await AuthHelper.LoginAsync(
            _client, email, "Password123!");
        return result!.Token;
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
    public async Task DeleteComment_OwnComment_Returns200()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await GetReaderTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var createResponse = await _client.PostAsJsonAsync(Endpoint, new
        {
            BlogId = seed.PublishedBlog.Id,
            Content = "Comment to delete"
        }, cancellationToken: TestContext.Current.CancellationToken);

        var createResult =
            await createResponse.Content.ReadFromJsonAsync<Result<CommentResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        var response = await _client.DeleteAsync($"{Endpoint}/{seed.PublishedBlog.Id}/{createResult!.Data!.Id}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task DeleteComment_OtherUsersComment_Returns403()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token1 = await GetReaderTokenAsync();
        var token2 = await GetReaderTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token1);

        var createResponse = await _client.PostAsJsonAsync(Endpoint, new
        {
            BlogId = seed.PublishedBlog.Id,
            Content = "User 1 comment"
        }, cancellationToken: TestContext.Current.CancellationToken);

        var createResult =
            await createResponse.Content.ReadFromJsonAsync<Result<CommentResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token2);

        var response = await _client.DeleteAsync($"{Endpoint}/{seed.PublishedBlog.Id}/{createResult!.Data!.Id}",
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task DeleteComment_NoAuth_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.DeleteAsync($"{Endpoint}/1/1", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task DeleteComment_UnknownComment_Returns404()
    {
        var token = await GetReaderTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.DeleteAsync($"{Endpoint}/1/99999", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}