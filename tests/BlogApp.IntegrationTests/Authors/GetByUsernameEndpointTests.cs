using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Authors;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Authors;

[Collection("Integration")]
public class GetByUsernameEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private static string GetEndpoint(string slug) => $"/api/v1/authors/{slug}";

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
    public async Task GetAuthor_ValidUsername_Returns200()
    {
        // Arrange
        var seed = await AuthorDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync(GetEndpoint(seed.Author.Username), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAuthor_ValidUsername_ReturnsFullProfile()
    {
        // Arrange
        var seed = await AuthorDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync(GetEndpoint(seed.Author.Username), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<AuthorProfileResult>>(TestContext.Current
                .CancellationToken);

        // Assert
        result!.IsSuccess.Should().BeTrue();
        result.Data!.Username.Should().Be(seed.Author.Username);
        result.Data.FullName.Should().NotBeNullOrEmpty();
        result.Data.BlogCount.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TotalReads.Should().BeGreaterThanOrEqualTo(0);
        result.Data.TotalLikes.Should().BeGreaterThanOrEqualTo(0);
    }

    [Fact]
    public async Task GetAuthor_StatsOnlyIncludePublishedBlogs()
    {
        // Arrange
        var seed = await AuthorDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync(GetEndpoint(seed.Author.Username), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<AuthorProfileResult>>(TestContext.Current
                .CancellationToken);

        // Assert — draft blog reads NOT counted
        result!.Data!.BlogCount.Should().Be(seed.PublishedBlogCount);
    }

    [Fact]
    public async Task GetAuthor_UnknownUsername_Returns404()
    {
        // Act
        var response =
            await _client.GetAsync($"{GetEndpoint("unknown-user-xyz")}", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAuthor_IsPublic_NoAuthRequired()
    {
        // Arrange
        var seed = await AuthorDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync(GetEndpoint(seed.Author.Username), TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAuthor_TopTagsOrdered()
    {
        // Arrange
        var seed = await AuthorDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client
            .GetAsync(GetEndpoint(seed.Author.Username), TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<AuthorProfileResult>>(TestContext.Current
                .CancellationToken);

        // Assert — tags ordered by use count
        if (result!.Data!.TopTags.Count > 1)
        {
            var useCounts = result.Data.TopTags.Select(t => t.BlogCount).ToList();
            useCounts.Should().BeInDescendingOrder();
        }
    }
}