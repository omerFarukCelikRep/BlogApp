using System.Net;
using System.Net.Http.Json;
using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Domain.Models.Tags;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;

namespace BlogApp.IntegrationTests.Tags;

[Collection("Integration")]
public class TopTagsEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private static string GetEndpoint(int? count = null)
    {
        var queryParams = new Dictionary<string, string?>();

        if (count.HasValue)
            queryParams["count"] = count.Value.ToString();

        return QueryHelpers.AddQueryString("api/v1/tags/top", queryParams);
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
    public async Task GetTopTags_ReturnsCorrectCount()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(5), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<TagResult>>>(TestContext.Current.CancellationToken);

        result!.StatusCode.Should().Be((int)HttpStatusCode.OK);
        result.Value.Should().HaveCount(5);
    }

    [Fact]
    public async Task GetTopTags_OrderedByPostCountDescending()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(10), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<TagResult>>>(TestContext.Current.CancellationToken);

        var postCounts = result!.Value!
            .Select(t => t.BlogCount)
            .ToList();

        postCounts.Should().BeInDescendingOrder();
    }

    [Fact]
    public async Task GetTopTags_Tag1HasMostPosts_AppearsFirst()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(10), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<TagResult>>>(TestContext.Current.CancellationToken);

        result!.Value!.First().Name.Should().StartWith("1");
    }

    [Fact]
    public async Task GetTopTags_EmptyDatabase_ReturnsEmptyList()
    {
        var response = await _client.GetAsync(GetEndpoint(10), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<TagResult>>>(TestContext.Current.CancellationToken);

        result!.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTopTags_IsPublicEndpoint_NoAuthRequired()
    {
        var response = await _client.GetAsync(GetEndpoint(10), TestContext.Current.CancellationToken);

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTopTags_ResponseHasRequiredFields()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(1), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<TagResult>>>(TestContext.Current.CancellationToken);

        var tag = result!.Value!.First();
        tag.Id.Should().BeGreaterThan(0);
        tag.Name.Should().NotBeNullOrEmpty();
        tag.Slug.Should().NotBeNullOrEmpty();
        tag.BlogCount.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(10)]
    public async Task GetTopTags_VariousCountParams_ReturnsCorrectCount(int count)
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(count), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Response<List<TagResult>>>(TestContext.Current.CancellationToken);

        result!.Value.Should().HaveCount(count);
    }
}