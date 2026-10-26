using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Blogs;

[Collection("Integration")]
public class GetAllByAuthorEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private static string GetEndpoint(string username) => $"/api/v1/blogs/{username}/author";

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
    public async Task GetBlogsByAuthor_ValidUsername_Returns200()
    {
        // Arrange
        var seed = await AuthorDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync(GetEndpoint(seed.Author.Username), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBlogsByAuthor_ReturnsOnlyPublishedBlogs()
    {
        // Arrange
        var seed = await AuthorDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync($"{GetEndpoint(seed.Author.Username)}?pageSize=20",
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(TestContext.Current
                .CancellationToken);

        // Assert
        result!.Data.Should().AllSatisfy(b => b.Title.Should().NotContain("Draft"));
        result.TotalCount.Should().Be(seed.PublishedBlogCount);
    }

    [Fact]
    public async Task GetBlogsByAuthor_SortByMostRead_ReturnsSortedBlogs()
    {
        // Arrange
        var seed = await AuthorDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync($"{GetEndpoint(seed.Author.Username)}?sortBy=most-read",
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(TestContext.Current
                .CancellationToken);

        // Assert
        if (result!.Data!.Count() > 1)
        {
            var readCounts = result.Data!.Select(b => b.ReadCount).ToList();
            readCounts.Should().BeInDescendingOrder();
        }
    }

    [Fact]
    public async Task GetBlogsByAuthor_Pagination_Works()
    {
        // Arrange
        var seed = await AuthorDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync($"{GetEndpoint(seed.Author.Username)}page=1&pageSize=2",
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(TestContext.Current
                .CancellationToken);

        // Assert
        result!.Data!.Count().Should().BeLessThanOrEqualTo(2);
        result.PageSize.Should().Be(2);
        result.PageNumber.Should().Be(1);
    }

    [Fact]
    public async Task GetBlogsByAuthor_UnknownUsername_Returns404()
    {
        // Act
        var response =
            await _client.GetAsync(GetEndpoint("unknown-user-xyz/blogs"), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBlogsByAuthor_IsPublic_NoAuthRequired()
    {
        // Arrange
        var seed = await AuthorDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync(GetEndpoint(seed.Author.Username), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
}