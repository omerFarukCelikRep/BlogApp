using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Categories;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Categories;

[Collection("Integration")]
public class GetBySlugEndpointTests(BlogAppFactory factory) : IAsyncLifetime
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
    public async Task GetCategoryBySlug_ValidSlug_Returns200()
    {
        // Arrange
        var seed = await CategoryDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync($"{Endpoint}/{seed.ParentCategory.Slug}",
            TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetCategoryBySlug_ReturnsFullDetail()
    {
        // Arrange
        var seed = await CategoryDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client.GetAsync($"{Endpoint}/{seed.ParentCategory.Slug}",
            TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<CategoryDetailsResult>>(TestContext.Current
                .CancellationToken);

        // Assert
        result!.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be(seed.ParentCategory.Name);
        result.Data.Children.Should().NotBeEmpty();
        result.Data.Parent.Should().BeNull(); // it's a top-level category
    }

    [Fact]
    public async Task GetCategoryBySlug_ChildCategory_HasParent()
    {
        // Arrange
        var seed = await CategoryDataSeeder.SeedAsync(_database.DbContext);

        // Act
        var response = await _client
            .GetAsync($"{Endpoint}/{seed.ChildCategory.Slug}", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<CategoryDetailsResult>>(TestContext.Current
                .CancellationToken);

        // Assert
        result!.Data!.Parent.Should().NotBeNull();
        result.Data.Parent!.Name.Should().Be(seed.ParentCategory.Name);
    }

    [Fact]
    public async Task GetCategoryBySlug_UnknownSlug_Returns404()
    {
        // Act
        var response =
            await _client.GetAsync($"{Endpoint}/unknown-category-slug", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}