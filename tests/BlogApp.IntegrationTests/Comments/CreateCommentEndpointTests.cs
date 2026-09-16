using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Comments;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Comments;

[Collection("Integration")]
public class CreateCommentEndpointTests(BlogAppFactory factory) : IAsyncLifetime
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
    public async Task CreateComment_ValidRequest_Returns200()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await GetReaderTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            BlogId = seed.PublishedBlog.Id,
            Content = "Great blog post!"
        };

        var response = await _client.PostAsJsonAsync(Endpoint, request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateComment_ValidRequest_ReturnsCommentResult()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await GetReaderTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            BlogId = seed.PublishedBlog.Id,
            Content = "This is a fantastic article!"
        };

        var response = await _client.PostAsJsonAsync(Endpoint, request,
            cancellationToken: TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<CommentResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.IsSuccess.Should().BeTrue();
        result.Data!.Content.Should().Be("This is a fantastic article!");
        result.Data.Author.Should().NotBeNull();
        result.Data.Replies.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateComment_NoAuth_Returns401()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        var request = new
        {
            BlogId = seed.PublishedBlog.Id,
            Content = "Great post!"
        };

        var response = await _client.PostAsJsonAsync(Endpoint, request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateComment_EmptyContent_Returns400()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await GetReaderTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            BlogId = seed.PublishedBlog.Id,
            Content = ""
        };

        var response = await _client.PostAsJsonAsync(Endpoint, request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateComment_DraftBlog_Returns400()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await GetReaderTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            BlogId = seed.DraftBlog.Id,
            Content = "Great post!"
        };

        var response = await _client.PostAsJsonAsync(Endpoint, request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateComment_Reply_Returns200WithParentId()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await GetReaderTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var parentResponse = await _client.PostAsJsonAsync(Endpoint, new
            {
                BlogId = seed.PublishedBlog.Id,
                Content = "Parent comment here"
            },
            cancellationToken: TestContext.Current.CancellationToken);

        var parentResult =
            await parentResponse.Content.ReadFromJsonAsync<Result<CommentResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        var replyRequest = new
        {
            BlogId = seed.PublishedBlog.Id,
            Content = "Reply to parent!",
            ParentCommentId = parentResult!.Data!.Id,
        };

        var response = await _client.PostAsJsonAsync(Endpoint, replyRequest,
            cancellationToken: TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<CommentResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        result!.Data!.ParentId.Should().Be(parentResult.Data.Id);
    }

    [Fact]
    public async Task CreateComment_ThenGet_CommentAppearsInList()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var token = await GetReaderTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsJsonAsync(Endpoint, new
        {
            BlogId = seed.PublishedBlog.Id,
            Content = "Unique comment content here"
        }, cancellationToken: TestContext.Current.CancellationToken);

        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync(Endpoint, TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<List<CommentResult>>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.Data!.Should().Contain(c => c.Content == "Unique comment content here");
    }
}