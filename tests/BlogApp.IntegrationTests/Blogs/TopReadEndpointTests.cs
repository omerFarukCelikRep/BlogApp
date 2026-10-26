using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Blogs;

[Collection("Integration")]
public class TopReadEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "api/v1/blogs/top-read?count=";

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
    public async Task GetTopReadBlogs_ReturnsOk()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(Endpoint + "10", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetTopReadBlogs_ReturnsCorrectCount()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(Endpoint + "5", TestContext.Current.CancellationToken);
        var result =
            await response.Content.ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current
                .CancellationToken);

        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetTopReadBlogs_ReturnsOnlyPublishedBlogs()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(Endpoint + "20", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current
                .CancellationToken);

        result!.Data.Should().NotBeNull();
        result.Data.Should().AllSatisfy(blog => blog.Title.Should().NotContain("Draft"));
    }

    [Fact]
    public async Task GetTopReadBlogs_OrderedByReadCountDescending()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(Endpoint + "10", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current
                .CancellationToken);

        var readCounts = result!.Data!.Select(x => x.ReadCount).ToList();
        readCounts.Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetTopReadBlogs_EmptyDatabase_ReturnsEmptyList()
    {
        var response = await _client.GetAsync(Endpoint + "10", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current
                .CancellationToken);

        result!.Data.Should().BeNullOrEmpty();
        result.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTopReadBlogs_NoCountParam_DefaultsTo10()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(Endpoint, TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current
                .CancellationToken);

        var blog = result!.Data!.First();
        blog.Id.Should().BeGreaterThan(0);
        blog.Title.Should().NotBeNullOrEmpty();
        blog.Excerpt.Should().NotBeNullOrEmpty();
        blog.AuthorFullName.Should().NotBeNullOrEmpty();
        blog.ReadingTimeInMinutes.Should().BeGreaterThan(0);
        blog.ReadCount.Should().BeGreaterThanOrEqualTo(0);
        blog.Slug.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetTopReadBlogs_IsPublicEndpoint_NoAuthRequired()
    {
        var response = await _client.GetAsync(Endpoint, TestContext.Current.CancellationToken);

        response.Should().NotBe((int)HttpStatusCode.Unauthorized);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetTopReadBlogs_VariousCountParams_ReturnsCorrectCount(int count)
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(Endpoint + $"{count}", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current
                .CancellationToken);

        result!.Data.Should().HaveCount(count);
    }
}