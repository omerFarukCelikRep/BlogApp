using BlogApp.Application.Blogs.Commands;
using BlogApp.Core.Security.Attributes;

namespace BlogApp.Api.Endpoints.Blogs.Requests;

public record CreateBlogRequest(
    [Sanitize] string Title,
    [Sanitize(AllowHtml = true)] string Content,
    string? Thumbnail,
    List<int> Categories,
    List<int> Tags)
{
    public static explicit operator CreateBlogCommand(CreateBlogRequest request) => new(request.Title, request.Content,
        request.Thumbnail,
        request.Categories,
        request.Tags);
}