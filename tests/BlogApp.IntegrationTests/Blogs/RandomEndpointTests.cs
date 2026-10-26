using System.Net;
using System.Net.Http.Json;
using BlogApp.Api.Endpoints.Shared.Responses;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.WebUtilities;

namespace BlogApp.IntegrationTests.Blogs;

[Collection("Integration")]
public class RandomEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private static string GetEndpoint(int? count = null, List<string>? categories = null)
    {
        var queryParams = new Dictionary<string, string?>();

        if (count.HasValue)
            queryParams["count"] = count.Value.ToString();

        if (categories is { Count: > 0 })
            queryParams["categories"] = string.Join(',', categories);

        return QueryHelpers.AddQueryString("api/v1/blogs/random", queryParams);
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
    public async Task GetRandomBlogs_ReturnsOk()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client
            .GetAsync(GetEndpoint(6), TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRandomBlogs_ReturnsCorrectCount()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client
            .GetAsync(GetEndpoint(3), TestContext.Current.CancellationToken);

        var result = await response.Content
            .ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current.CancellationToken);

        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetRandomBlogs_ReturnsOnlyPublishedBlogs()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client
            .GetAsync(GetEndpoint(20), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current
                .CancellationToken);

        result!.Data!.Should().AllSatisfy(blog => blog.Title.Should().NotContain("Draft"));
    }

    [Fact]
    public async Task GetRandomBlogs_EmptyDatabase_ReturnsEmptyList()
    {
        var response = await _client
            .GetAsync(GetEndpoint(6), TestContext.Current.CancellationToken);

        var result = await response.Content
            .ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current.CancellationToken);

        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GetRandomBlogs_IsPublicEndpoint_NoAuthRequired()
    {
        var response = await _client
            .GetAsync(GetEndpoint(6), TestContext.Current.CancellationToken);

        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetRandomBlogs_NoCountParam_DefaultsTo10()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response = await _client.GetAsync(GetEndpoint(), TestContext.Current.CancellationToken);

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
    public async Task GetRandomBlogs_CalledTwice_MayReturnDifferentOrder()
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response1 = await _client.GetAsync(GetEndpoint(5), TestContext.Current.CancellationToken);
        var response2 = await _client.GetAsync(GetEndpoint(5), TestContext.Current.CancellationToken);

        var result1 = await response1.Content
            .ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current.CancellationToken);
        var result2 = await response2.Content
            .ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current.CancellationToken);

        result1!.IsSuccess.Should().BeTrue();
        result2!.IsSuccess.Should().BeTrue();
        result1.Data.Should().HaveCount(5);
        result2.Data.Should().HaveCount(5);
    }

    [Theory]
    [InlineData(3, "DotNet", "React", "Java")]
    [InlineData(2, "React", "Java")]
    [InlineData(1, "React")]
    public async Task GetTopReadBlogs_VariousCountAndCategoryParams_ReturnsCorrectCount(int count,
        params string[] categories)
    {
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        var response =
            await _client.GetAsync(GetEndpoint(count, [.. categories]), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current
                .CancellationToken);

        result!.Data.Should().HaveCount(count);
    }
}