using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Blogs;

[Collection("Integration")]
public class CreateEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "api/v1/blogs";

    private readonly HttpClient _client = factory.CreateClient();
    private readonly DatabaseFixture _database = new(factory);

    private async Task<string> GetAuthorTokenAsync()
    {
        var email = $"author_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);

        await BlogTestHelper.AssignRoleAsync(_database.DbContext, email, "Author");

        var loginResult = await AuthHelper.LoginAsync(
            _client, email, "Password123!");
        return loginResult!.Token;
    }

    private static object ValidRequest(int tagId, int categoryId) => new
    {
        Title = "My First .NET Blog Post",
        Content = new string('x', 100),
        Thumbnail = (string?)null,
        Tags = new[] { tagId },
        Categories = new[] { categoryId }
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
    public async Task CreateBlog_ValidRequest_Returns200()
    {
        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);
        var token = await GetAuthorTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(Endpoint, ValidRequest(tags[0].Id, category.Id),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateBlog_ValidRequest_ReturnsResult()
    {
        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);
        var token = await GetAuthorTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Title = "My Unique Blog Post Title",
            Content = new string('x', 100),
            Thumbnail = (string?)null,
            Tags = new[] { tags[0].Id },
            Categories = new[] { category.Id }
        };

        var response = await _client.PostAsJsonAsync(Endpoint, request, TestContext.Current.CancellationToken);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CreateBlogResult>>(TestContext.Current.CancellationToken);

        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("My Unique Blog Post Title");
        result.Data.Slug.Should().NotBeNullOrEmpty();
        result.Data.ReadingTimeInMinutes.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateBlog_NoToken_Returns401()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);

        var response = await _client.PostAsJsonAsync(Endpoint, ValidRequest(tags[0].Id, category.Id),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task CreateBlog_ReaderRole_Returns403()
    {
        var email = $"reader_{Guid.NewGuid():N}@example.com";
        await AuthHelper.RegisterAsync(_client, email);
        var loginResult = await AuthHelper.LoginAsync(_client, email, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);

        var response = await _client.PostAsJsonAsync(Endpoint, ValidRequest(tags[0].Id, category.Id),
            TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateBlog_EmptyTitle_Returns400()
    {
        var token = await GetAuthorTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Title = "",
            Content = new string('x', 100),
            Tags = new[] { 1 },
            Categories = new[] { 1 },
        };

        var response = await _client.PostAsJsonAsync(Endpoint, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBlog_ContentTooShort_Returns400()
    {
        var token = await GetAuthorTokenAsync();
        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Title = "Valid Title Here",
            Content = "Too short",
            Tags = new[] { 1 },
            Categories = new[] { 1 },
        };

        var response = await _client
            .PostAsJsonAsync(Endpoint, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBlog_InvalidTagId_Returns404()
    {
        var token = await GetAuthorTokenAsync();
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var request = new
        {
            Title = "Valid Blog Title Here",
            Content = new string('x', 100),
            Tags = new[] { 99999 }, // doesn't exist
            Categories = new[] { category.Id }
        };

        var response = await _client
            .PostAsJsonAsync(Endpoint, request, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateBlog_AsDraft_NotInTopReadFeed()
    {
        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);
        var token = await GetAuthorTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        await _client.PostAsJsonAsync(Endpoint, new
        {
            Title = "My Draft Blog",
            Content = new string('x', 100),
            Tags = new[] { tags[0].Id },
            Categories = new[] { category.Id }
        }, TestContext.Current.CancellationToken);

        _client.DefaultRequestHeaders.Authorization = null;

        var feedResponse = await _client
            .GetAsync($"{Endpoint}/top-read?count=20", TestContext.Current.CancellationToken);

        var feedResult = await feedResponse.Content
            .ReadFromJsonAsync<Result<List<BlogSummaryResult>>>(TestContext.Current.CancellationToken);

        feedResult!.Data!.Should().NotContain(b => b.Title == "My Draft Blog");
    }

    [Fact]
    public async Task CreateBlog_ValidRequest_SlugGeneratedFromTitle()
    {
        var (_, tags, _) = await HomePageDataSeeder.SeedAsync(_database.DbContext);
        var category = await BlogTestHelper.SeedCategoryAsync(_database.DbContext);
        var token = await GetAuthorTokenAsync();

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync(Endpoint, new
        {
            Title = "Hello World From .NET",
            Content = new string('x', 100),
            Tags = new[] { tags[0].Id },
            Categories = new[] { category.Id }
        }, TestContext.Current.CancellationToken);

        var result = await response.Content
            .ReadFromJsonAsync<Result<CreateBlogResult>>(TestContext.Current.CancellationToken);

        result!.Data!.Slug.Should().Be("hello-world-from-net");
    }
}