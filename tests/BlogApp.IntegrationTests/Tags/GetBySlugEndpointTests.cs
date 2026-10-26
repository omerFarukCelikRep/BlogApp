using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Tags;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using Elastic.CommonSchema;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Tags;

[Collection("Integration")]
public class GetBySlugEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private static string GetEndpoint(string slug) => $"/api/v1/tags/{slug}";

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
    public async Task GetTagBySlug_ValidSlug_Returns200()
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
    public async Task GetTagBySlug_ValidSlug_ReturnsTagDetail()
    {
        // Arrange
        var seed = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var tag = seed.Tags.First();

        // Act
        var response = await _client.GetAsync(GetEndpoint(tag.Slug), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<TagResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result!.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be(tag.Name);
        result.Data.Slug.Should().Be(tag.Slug);
        result.Data.BlogCount.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetTagBySlug_UnknownSlug_Returns404()
    {
        // Act
        var response = await _client.GetAsync(GetEndpoint("unknown-tag-slug"), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTagBySlug_IsPublic_NoAuthRequired()
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
}