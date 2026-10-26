using System.Net;
using System.Net.Http.Json;
using BlogApp.Core.Results;
using BlogApp.Domain.Models.Blogs;
using BlogApp.IntegrationTests.Fixtures;
using BlogApp.IntegrationTests.Helpers;
using FluentAssertions;

namespace BlogApp.IntegrationTests.Blogs;

[Collection("Integration")]
public class GetByIdEndpointTests(BlogAppFactory factory) : IAsyncLifetime
{
    private const string Endpoint = "/api/v1/blogs";
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
    public async Task GetById_PublishedBlog_Returns200()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_PublishedBlog_ReturnsFullDetails()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<BlogDetailsResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();

        var blog = result.Data!;
        blog.Id.Should().Be(seed.PublishedBlog.Id);
        blog.Title.Should().Be(seed.PublishedBlog.Title);
        blog.Content.Should().NotBeNullOrEmpty();
        blog.Slug.Should().NotBeNullOrEmpty();
        blog.ReadingTimeInMinutes.Should().BeGreaterThan(0);
        blog.Author.Should().NotBeNull();
        blog.Author.FullName.Should().Be(seed.Author.FullName);
        blog.Tags.Should().HaveCount(1);
        blog.Tags.First().Name.Should().Be(seed.Tag.Name);
        blog.Categories.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        var response = await _client.GetAsync($"{Endpoint}/99999", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_DraftBlog_UnauthenticatedUser_Returns403()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        var response = await _client.GetAsync($"{Endpoint}/{seed.DraftBlog.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetById_DraftBlog_NonAuthor_Returns403()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);

        var otherEmail = seed.OtherUser.Email;
        await AuthHelper.RegisterAsync(_client, otherEmail);
        var loginResult = await AuthHelper.LoginAsync(_client, otherEmail, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        var response = await _client.GetAsync($"{Endpoint}/{seed.DraftBlog.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetById_DraftBlog_Author_Returns200()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);

        await AuthHelper.RegisterAsync(_client, seed.Author.Email, "Password123!");
        var loginResult = await AuthHelper.LoginAsync(_client, seed.Author.Email, "Password123!");

        _client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult!.Token);

        var response = await _client.GetAsync($"{Endpoint}/{seed.DraftBlog.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_PublishedBlog_NoAuth_Returns200()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_UnauthenticatedUser_IsLikedFalse()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);
        _client.DefaultRequestHeaders.Authorization = null;

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        var result =
            await response.Content.ReadFromJsonAsync<Result<BlogDetailsResult>>(
                cancellationToken: TestContext.Current.CancellationToken);

        result!.Data!.IsLiked.Should().BeFalse();
    }

    [Fact]
    public async Task GetById_Returns_JsonContentType()
    {
        var seed = await BlogDetailsDataSeeder.SeedAsync(_database.DbContext);

        var response =
            await _client.GetAsync($"{Endpoint}/{seed.PublishedBlog.Id}", TestContext.Current.CancellationToken);

        response.Content.Headers.ContentType?.MediaType.Should().Be("application/json");
    }
}