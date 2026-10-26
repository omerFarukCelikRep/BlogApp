using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Categories;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Categories;

[Collection("Integration")]
public class GetAllEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "/api/v1/categories";

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
    public async Task GetAllCategories_Returns200()
    {
        // Arrange
        await HomePageDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync(Endpoint, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAllCategories_ReturnsTreeStructure()
    {
        // Arrange
        await CategoryDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync(Endpoint, TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<List<CategoryDetailsResult>>>(
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        // Top level only — no parents
        result.Data!.Should().AllSatisfy(c =>
            c.Children.Should().NotBeNull());
    }

    [Fact]
    public async Task GetAllCategories_IsPublic_NoAuthRequired()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.GetAsync(Endpoint, TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }
}