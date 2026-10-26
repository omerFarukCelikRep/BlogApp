using System.Net;
using System.Net.Http.Json;
using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Domain.Models.Categories;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;

namespace BlogApp.IntegrationTests.Categories;

[Collection("Integration")]
public class TopEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private static string GetEndpoint(int? count = null)
    {
        var queryParams = new Dictionary<string, string?>();

        if (count.HasValue)
            queryParams["count"] = count.Value.ToString();

        return QueryHelpers.AddQueryString("api/v1/categories/top", queryParams);
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
    public async Task GetTopCategories_ReturnsCorrectCount()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(5), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<CategoryResult>>>(TestContext.Current
                .CancellationToken);

        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetTopCategories_OrderedByPostCountDescending()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(10), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<CategoryResult>>>(TestContext.Current
                .CancellationToken);

        var postCounts = result!.Value!
            .Select(t => t.BlogCount)
            .ToList();

        postCounts.Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetTopCategories_KnownCategoriesHasMostPosts_AppearsFirst()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(10), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<CategoryResult>>>(TestContext.Current
                .CancellationToken);

        result!.Value![13].Name.Should().ContainAny("React");
    }

    [Fact]
    public async Task GetTopCategories_EmptyDatabase_ReturnsEmptyList()
    {
        var response = await _client.GetAsync(GetEndpoint(10), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<CategoryResult>>>(TestContext.Current
                .CancellationToken);

        result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTopCategories_IsPublicEndpoint_NoAuthRequired()
    {
        var response = await _client.GetAsync(GetEndpoint(10), TestContext.Current.CancellationToken);

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTopCategories_ResponseHasRequiredFields()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(1), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<CategoryResult>>>(TestContext.Current
                .CancellationToken);

        var category = result!.Value!.First();
        category.Id.Should().BeGreaterThan(0);
        category.Name.Should().NotBeNullOrEmpty();
        category.Slug.Should().NotBeNullOrEmpty();
        category.BlogCount.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetTopCategories_VariousCountParams_ReturnsCorrectCount(int count)
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(count), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<CategoryResult>>>(TestContext.Current
                .CancellationToken);

        result!.Value.Should().HaveCount(count);
    }
}