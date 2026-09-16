using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Comments;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Comments;

[Collection("Integration")]
public class GetAllByBlogEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "api/v1/comments";

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

    [Fact]
    public async Task GetComments_PublishedBlog_Returns200()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetComments_EmptyBlog_ReturnsEmptyList()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<List<CommentResult>>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetComments_IsPublic_NoAuthRequired()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
}