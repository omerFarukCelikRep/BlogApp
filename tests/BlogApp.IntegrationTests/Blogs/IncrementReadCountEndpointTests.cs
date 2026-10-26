using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Blogs;

[Collection("Integration")]
public class IncrementReadCountEndpointTests(BlogAppFactory factory) : IAsyncLifetime
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

    [Fact]
    public async Task IncrementReadCount_ValidBlog_Returns200()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/read", null,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task IncrementReadCount_ValidBlog_IncrementsCount()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var initialCount = seed.PublishedBlog.ReadCount;

        await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/read", null,
            TestContext.Current.CancellationToken);

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<BlogDetailsResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.Data!.ReadCount.Should().Be(initialCount + 1);
    }

    [Fact]
    public async Task IncrementReadCount_CalledMultipleTimes_IncrementsEachTime()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        var initialCount = seed.PublishedBlog.ReadCount;

        for (int i = 0; i < 3; i++)
            await _client.PostAsync($"/{Endpoint}/{seed.PublishedBlog.Id}/read", null,
                TestContext.Current.CancellationToken);

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<BlogDetailsResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.Data!.ReadCount.Should().Be(initialCount + 3);
    }

    [Fact]
    public async Task IncrementReadCount_UnknownBlog_Returns404()
    {
        var response = await _client.PostAsync($"{Endpoint}/99999/read", null, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task IncrementReadCount_NoAuth_Returns200()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.PostAsync($"{Endpoint}/{seed.PublishedBlog.Id}/read", null,
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}