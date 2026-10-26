using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Blogs;

[Collection("Integration")]
public class UpdateEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "api/v1/blogs";

    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private async Task<(string Token, int BlogId)> SeedBlogWithAuthorAsync()
    {
        var email = $"author_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        await BlogTestHelper.AssignRoleAsync(_database.DbContext, email, "Author");
        var loginResult = await AuthHelper.LoginAsync(_client, email, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);

        var createResponse = await _client.PostAsJsonAsync(
            Endpoint, new
            {
                Title = "Original Blog Title",
                Content = new string('x', 100),
                Categories = new[] { category.Id },
                Tags = new[] { tags[0].Id }
            });

        var createResult = await createResponse.Content.ReadFromJsonAsync<Result<CreateBlogResult>>();

        return (loginResult.Token, createResult!.Data!.Id);
    }

    private static object ValidUpdateRequest(int tagId, int categoryId) => new
    {
        Title = "Updated Blog Title",
        Content = new string('x', 100),
        Thumbnail = (string?)null,
        Categories = new[] { categoryId },
        Tags = new[] { tagId }
    };

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
    public async Task UpdateBlog_ValidRequest_Returns200()
    {
        // Arrange
        var (token, blogId) = await SeedBlogWithAuthorAsync();
        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsJsonAsync($"{Endpoint}/{blogId}", ValidUpdateRequest(tags[0].Id, category.Id),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateBlog_ValidRequest_TitleUpdated()
    {
        // Arrange
        var (token, blogId) = await SeedBlogWithAuthorAsync();
        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsJsonAsync($"{Endpoint}/{blogId}", new
        {
            Title = "My Brand New Title",
            Content = new string('x', 100),
            TagIds = new[] { tags[0].Id },
            CategoryIds = new[] { category.Id },
            PostStatus = "Draft",
        }, cancellationToken: TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<BlogDetailsResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result!.Data!.Title.Should().Be("My Brand New Title");
    }

    // ❌ Not author
    [Fact]
    public async Task UpdateBlog_DifferentUser_Returns403()
    {
        // Arrange
        var (_, blogId) = await SeedBlogWithAuthorAsync();
        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);

        // Login as different user
        var otherEmail = $"other_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, otherEmail);
        var otherLogin = await AuthHelper.LoginAsync(_client, otherEmail, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", otherLogin!.Token);

        // Act
        var response = await _client.PutAsJsonAsync($"{Endpoint}/{blogId}", ValidUpdateRequest(tags[0].Id, category.Id),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    // ❌ Unauthenticated
    [Fact]
    public async Task UpdateBlog_NoAuth_Returns401()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = null;

        // Act
        var response = await _client.PutAsJsonAsync($"{Endpoint}/1", ValidUpdateRequest(1, 1),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ❌ Validation failure
    [Fact]
    public async Task UpdateBlog_EmptyTitle_Returns400()
    {
        // Arrange
        var (token, blogId) = await SeedBlogWithAuthorAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsJsonAsync($"{Endpoint}/{blogId}", new
        {
            Title = "",
            Content = new string('x', 100),
            TagIds = new[] { 1 },
            CategoryIds = new[] { 1 }
        }, cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ❌ Blog not found
    [Fact]
    public async Task UpdateBlog_UnknownId_Returns404()
    {
        // Arrange
        var (token, _) = await SeedBlogWithAuthorAsync();
        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.PutAsJsonAsync($"{Endpoint}/99999", ValidUpdateRequest(tags[0].Id, category.Id),
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ✅ GetBlogForEdit — author can access
    [Fact]
    public async Task GetBlogForEdit_Author_Returns200WithPrefilledData()
    {
        // Arrange
        var (token, blogId) = await SeedBlogWithAuthorAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client
            .GetAsync($"{Endpoint}/{blogId}/edit", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<BlogDetailsResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Data!.Title.Should().Be("Original Blog Title");
        result.Data.Tags.Should().NotBeEmpty();
        result.Data.Categories.Should().NotBeEmpty();
    }

    // ❌ GetBlogForEdit — non-author gets 403
    [Fact]
    public async Task GetBlogForEdit_NonAuthor_Returns403()
    {
        // Arrange
        var (_, blogId) = await SeedBlogWithAuthorAsync();

        var otherEmail = $"other_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, otherEmail);
        var otherLogin = await AuthHelper.LoginAsync(
            _client, otherEmail, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", otherLogin!.Token);

        // Act
        var response = await _client.GetAsync($"{Endpoint}/{blogId}/edit", TestContext.Current.CancellationToken);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}