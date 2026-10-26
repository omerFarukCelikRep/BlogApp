using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Blogs;

[Collection("Integration")]
public class GetAllByTagEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private static string GetEndpoint(string slug) => $"/api/v1/blogs/{slug}/tag";

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
    public async Task GetBlogsByTag_ValidSlug_Returns200()
    {
        // Arrange
        var seed = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var tag = seed.Tags.First();

        // Act
        var response = await _client.GetAsync(GetEndpoint(tag.Slug), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBlogsByTag_ReturnsOnlyPublishedBlogs()
    {
        // Arrange
        var seed = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var tag = seed.Tags.First();

        // Act
        var response = await _client.GetAsync($"{GetEndpoint(tag.Slug)}?page=1&pageSize=20",
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert — no draft blogs
        result!.Data!.Should().AllSatisfy(b => { b.Title.Should().NotContain("Draft"); });
    }

    [Fact]
    public async Task GetBlogsByTag_Pagination_ReturnsCorrectPage()
    {
        // Arrange
        var seed = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var tag = seed.Tags.First();

        // Act
        var response = await _client.GetAsync($"{GetEndpoint(tag.Slug)}?page=1&pageSize=5",
            TestContext.Current.CancellationToken);

        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result!.Data!.Count().Should().BeLessThanOrEqualTo(5);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(5);
    }

    [Fact]
    public async Task GetBlogsByTag_SortByMostRead_ReturnsSortedBlogs()
    {
        // Arrange
        var seed = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var tag = seed.Tags.First();

        // Act
        var response = await _client.GetAsync($"{GetEndpoint(tag.Slug)}?sortBy=most-read",
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert — read counts descending
        if (result!.Data!.Count() > 1)
        {
            var readCounts = result.Data!
                .Select(b => b.ReadCount)
                .ToList();
            readCounts.Should().BeInDescendingOrder();
        }
    }

    [Fact]
    public async Task GetBlogsByTag_UnknownSlug_Returns404()
    {
        // Act
        var response = await _client.GetAsync(GetEndpoint("unknown-tag-slug"), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBlogsByTag_IsPublic_NoAuthRequired()
    {
        // Arrange
        var seed = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response =
            await _client.GetAsync(GetEndpoint(seed.Tags.First().Slug), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetBlogsByTag_ResponseHasRequiredFields()
    {
        // Arrange
        var seed = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var tag = seed.Tags.First();

        // Act
        var response =
            await _client.GetAsync($"{GetEndpoint(tag.Slug)}?pageSize=1", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result!.Data.Should().NotBeNull();
        result.PageNumber.Should().BeGreaterThan(0);
        result.PageSize.Should().BeGreaterThan(0);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(0);
        result.TotalPages.Should().BeGreaterThanOrEqualTo(0);
    }
}