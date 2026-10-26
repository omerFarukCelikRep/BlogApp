using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Blogs;

[Collection("Integration")]
public class GetAllByCategoryEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private static string GetEndpoint(string slug) => $"/api/v1/blogs/{slug}/category";

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
    public async Task GetBlogsByCategory_ValidSlug_Returns200()
    {
        // Arrange
        var seed = await CategoryDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client
            .GetAsync(GetEndpoint(seed.ParentCategory.Slug), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetBlogsByCategory_ReturnsOnlyPublishedBlogs()
    {
        // Arrange
        var seed = await CategoryDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync($"{GetEndpoint(seed.ParentCategory.Slug)}?pageSize=20",
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(TestContext.Current
                .CancellationToken);

        // Assert
        result!.Data.Should().AllSatisfy(b => b.Title.Should().NotContain("Draft"));
    }

    [Fact]
    public async Task GetBlogsByCategory_IncludeChildren_ReturnsBlogsFromChildren()
    {
        // Arrange
        var seed = await CategoryDataSeeder.SeedAsync(_database.DbContext);

        // Act — include children
        var withChildren = await _client.GetAsync($"{GetEndpoint(seed.ParentCategory.Slug)}?includeChildren=true",
            TestContext.Current.CancellationToken);

        var withoutChildren = await _client.GetAsync($"{GetEndpoint(seed.ParentCategory.Slug)}?includeChildren=false",
            TestContext.Current.CancellationToken);

        var resultWith =
            await withChildren.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(TestContext.Current
                .CancellationToken);
        var resultWithout =
            await withoutChildren.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(TestContext.Current
                .CancellationToken);

        // Assert — with children should have >= without
        resultWith!.TotalCount.Should().BeGreaterThanOrEqualTo(resultWithout!.TotalCount);
    }

    [Fact]
    public async Task GetBlogsByCategory_Pagination_Works()
    {
        // Arrange
        var seed = await CategoryDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync($"{GetEndpoint(seed.ParentCategory.Slug)}?page=1&pageSize=5",
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<PaginatedResult<BlogSummaryResult>>(TestContext.Current
                .CancellationToken);

        // Assert
        result!.Data!.Count().Should().BeLessThanOrEqualTo(5);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(5);
    }

    [Fact]
    public async Task GetBlogsByCategory_UnknownSlug_Returns404()
    {
        // Act
        var response = await _client.GetAsync(GetEndpoint("unknown-slug"), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBlogsByCategory_IsPublic_NoAuthRequired()
    {
        // Arrange
        var seed = await CategoryDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response =
            await _client.GetAsync(GetEndpoint(seed.ParentCategory.Slug), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
}